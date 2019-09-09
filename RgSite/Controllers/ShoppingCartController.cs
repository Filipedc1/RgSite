using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RgSite.Data;
using RgSite.Data.Models;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProduct productService;
        private readonly IShoppingCart cartService;
        private readonly IAppUser userService;
        private readonly IOrder orderService;
        private readonly IEmailSender emailService;

        #region Constructor

        public ShoppingCartController(IProduct productService, IShoppingCart cartService, IAppUser userService, 
                                      IOrder orderService, IEmailSender emailService)
        {
            this.productService = productService;
            this.userService = userService;
            this.cartService = cartService;
            this.orderService = orderService;
            this.emailService = emailService;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRoleAsync() : RoleName.Customer;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            decimal subTotal = 0;

            var cartItems = await cartService.GetAllAsync(userId);
            if (!cartItems.Any())
                return View(new ShoppingCartViewModel());

            subTotal = cartService.GetCartTotalCostAsync(userId, role, cartItems);

            var items = cartItems.Select(item => new CartItemViewModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Name = item.Name.ToUpper(),
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                Quantity = item.Quantity,
                Price = item.Price,
                User = item.User
            })
            .ToList();

            var vM = new ShoppingCartViewModel
            {
                CartItems = items,
                SubTotal = subTotal,
                Total = subTotal,
                User = items.FirstOrDefault().User
            };

            return View(vM);
        }

        public IActionResult Detail(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // If user is not logged in, assume Customer role. Only keep this if users can add to cart without logging in.
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRoleAsync() : RoleName.Customer;
            var user = await userService.GetCurrentUserAsync();

            var product = await productService.GetProductByIdAsync(model.ProductId);
            var price = product.Prices.FirstOrDefault(p => p.Id == model.Price.Id);

            if (product == null || price == null)
                return BadRequest();

            if (await cartService.IsInCartAsync(model.ProductId, price.Id))
            {
                await cartService.UpdateQuantityAsync(model.ProductId, true);
                return RedirectToAction("ProductDetail", "Products", new { id = product.ProductId });
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Quantity = model.Quantity,
                    Price = price,
                    User = user
                };

                if (await cartService.AddItemAsync(cartItem))
                    return RedirectToAction("ProductDetail", "Products", new { id = product.ProductId });
            }

            return BadRequest();
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await cartService.GetByIdAsync(id);

            if (item == null) return BadRequest();

            if (await cartService.DeleteItemAsync(item))
                return RedirectToAction("Index");

            return BadRequest();
        }

        public async Task<IActionResult> Checkout(string id)
        {
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRoleAsync() : RoleName.Customer;
            string userId = id;
            decimal subTotal = 0;

            var cartItems = await cartService.GetAllAsync(userId);
            if (cartItems != null)
                subTotal = cartService.GetCartTotalCostAsync(userId, role, cartItems);

            var items = cartItems.Select(item => new CartItemViewModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Name = item.Name.ToUpper(),
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                Quantity = item.Quantity,
                Price = item.Price,
                User = item.User
            })
            .ToList();

            var cart = new ShoppingCartViewModel
            {
                CartItems = items,
                SubTotal = subTotal,
                Total = subTotal,
                User = items.FirstOrDefault().User
            };

            var vM = new CheckoutFormViewModel
            {
                ShoppingCart = cart,
                States = await cartService.GetStatesAsync()
            };

            return View("CheckoutForm", vM);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                return RedirectToAction("Checkout");
            }

            var paymentDetail = new PaymentDetail
            {
                PaymentDetailId = model.PaymentDetail.PaymentDetailId,
                CardOwnerName = model.PaymentDetail.CardOwnerName,
                CardNumber = model.PaymentDetail.CardNumber,
                Expiration = model.PaymentDetail.Expiration,
                CVV = model.PaymentDetail.CVV,
                CardType = model.PaymentDetail.CardType
            };

            var billingDetail = new BillingDetail
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = await BuildAddressModel(model.Address),
                Email = model.Email,
                Phone = model.Phone,
                IsResidential = model.IsResidential,
                CompanyName = model.CompanyName,
                OrderNotes = model.OrderNotes
            };

            var user = await userService.GetCurrentUserAsync();
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRoleAsync() : RoleName.Customer;
            decimal subTotal = 0;
            decimal total = 0;

            var cartItems = await cartService.GetAllAsync(user.Id);
            if (cartItems != null)
                subTotal = cartService.GetCartTotalCostAsync(user.Id, role, cartItems);

            // need calculate total with shipping
            // For now skip it.
            total = subTotal;

            //need to check if shiptodifferentaddress is true, if so, use the ShippingFormViewModel property. SKIP this


            var order = new Order
            {
                Total = total,
                Placed = DateTime.Now,
                BillingDetail = billingDetail,
                User = user
            };

            var orderDetails = cartItems.Select(item => new OrderDetail
            {
                ProductId = item.ProductId,
                ProductName = item.Name,
                ProductQuantity = item.Quantity,
                ProductPrice = item.Price,
                ProductSize = item.Price.Size,
                ProductCost = item.Price.Cost,
                Order = order
            })
            .ToList();

            // need to process payment


            if (await orderService.AddOrderAsync(order, orderDetails))
            {
                await emailService.SendEmailAsync(model.Email, "Your Order Confirmation", "Please login to your account to view your orders");

                // empty out shoppingcart once order is completed
                await cartService.ClearCartAsync(user.Id);

                return View("Success");
            }

            return View("Error");
        }

        // Updates cart when quantity value is modified
        public async Task<IActionResult> UpdateCart(int itemId, int productId, int selectedSizeId, string selectedQuantity)
        {
            //If user is not logged in, assume Customer role
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRoleAsync() : RoleName.Customer;

            var product = await productService.GetProductByIdAsync(productId);
            decimal cost = 0;

            if (product != null)
            {
                var price = product.Prices.FirstOrDefault(p => p.Id == selectedSizeId);
                cost = (role == RoleName.Customer || role == RoleName.Admin) ? price.CustomerCost : price.SalonCost;
            }

            int quantity = int.Parse(selectedQuantity);
            string result = $"${cost * quantity}";

            await cartService.UpdateQuantityAsync(itemId, false, quantity);

            return Json(new { price = result });
        }

        #region Helpers

        private async Task<Address> BuildAddressModel(AddressViewModel addressVM)
        {
            var state = await cartService.GetStateByIdAsync(addressVM.State.Id);

            return new Address
            {
                Id = addressVM.Id,
                Street = addressVM.Street,
                City = addressVM.City,
                State = state.Name,
                Zip = addressVM.Zip
            };
        }

        #endregion

    }
}