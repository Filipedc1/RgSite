using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RgSite.Core.Interfaces;
using RgSite.Core.Models;
using RgSite.Data.Models;
using RgSite.ViewModels;
using Stripe;

namespace RgSite.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _cartService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IEmailSender _emailService;
        private readonly IConfiguration _config;

        #region Constructor

        public ShoppingCartController(IProductService productService, IShoppingCartService cartService, IUserService userService,
                                      IOrderService orderService, IEmailSender emailService, IConfiguration config)
        {
            _productService = productService;
            _userService = userService;
            _cartService = cartService;
            _orderService = orderService;
            _emailService = emailService;
            _config = config;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            string role = await _userService.GetCurrentUserRoleAsync();
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            decimal subTotal = 0;

            var cartItems = await _cartService.GetAllAsync(userId);
            if (!cartItems.Any())
                return View(new ShoppingCartViewModel());

            subTotal = _cartService.GetCartTotalCostAsync(userId, role, cartItems);

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

            string role = await _userService.GetCurrentUserRoleAsync();
            var user = await _userService.GetCurrentUserAsync();

            var product = await _productService.GetProductByIdAsync(model.ProductId);
            var price = product.Prices.FirstOrDefault(p => p.Id == model.Price.Id);

            if (product == null || price == null)
                return BadRequest();

            if (await _cartService.IsInCartAsync(model.ProductId, price.Id))
            {
                await _cartService.UpdateQuantityAsync(model.ProductId, true);
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

                if (await _cartService.AddItemAsync(cartItem))
                    return RedirectToAction("ProductDetail", "Products", new { id = product.ProductId });
            }

            return BadRequest();
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _cartService.GetByIdAsync(id);

            if (item == null) return BadRequest();

            if (await _cartService.DeleteItemAsync(item))
                return RedirectToAction("Index");

            return BadRequest();
        }

        public async Task<IActionResult> Checkout(string id)
        {
            string role = await _userService.GetCurrentUserRoleAsync();
            string userId = id;
            decimal subTotal = 0;

            var cartItems = await _cartService.GetAllAsync(userId);
            if (cartItems != null)
                subTotal = _cartService.GetCartTotalCostAsync(userId, role, cartItems);

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
                States = await _cartService.GetStatesAsync(),
                StripePublicKey = _config["Stripe:PublicKey"].ToString()
            };

            return View("CheckoutForm", vM);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutFormViewModel model, string stripeToken)
        {
            if (!ModelState.IsValid)
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                return RedirectToAction("Checkout");
            }

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

            var user = await _userService.GetCurrentUserAsync();
            string role = await _userService.GetCurrentUserRoleAsync();
            decimal subTotal = 0;
            decimal total = 0;

            var cartItems = await _cartService.GetAllAsync(user.Id);
            if (cartItems != null)
                subTotal = _cartService.GetCartTotalCostAsync(user.Id, role, cartItems);

            // need calculate total with shipping
            // For now skip it.
            total = subTotal;

            //need to check if shiptodifferentaddress is true, if so, use the ShippingFormViewModel property. SKIP this

            var order = new Data.Models.Order
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
            }).ToList();

            // Process payment
            var charges = new ChargeService();

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = (int)(total * 100),
                Description = "Purchase",
                Currency = "usd",
                Source = stripeToken
            });

            // Note update Order model to include a StripeCustomerId so we can save this in the DB. This id will link to the order.
            // the value will come from charge.CustomerId
            if (charge.Status == "succeeded" && await _orderService.AddOrderAsync(order, orderDetails))
            {
                //await emailService.SendEmailAsync(model.Email, "Your Order Confirmation", "Please login to your account to view your orders");

                // empty out shoppingcart once order is completed
                await _cartService.ClearCartAsync(user.Id);

                return View("Success");
            }

            return View("Error");
        }

        // Updates cart when quantity value is modified
        public async Task<IActionResult> UpdateCart(int itemId, int productId, int selectedSizeId, string selectedQuantity)
        {
            string role = await _userService.GetCurrentUserRoleAsync();

            var product = await _productService.GetProductByIdAsync(productId);
            decimal cost = 0;

            if (product != null)
            {
                var price = product.Prices.FirstOrDefault(p => p.Id == selectedSizeId);
                cost = (role == RoleConstants.Customer || role == RoleConstants.Admin) ? price.CustomerCost : price.SalonCost;
            }

            int quantity = int.Parse(selectedQuantity);
            string result = $"${cost * quantity}";

            await _cartService.UpdateQuantityAsync(itemId, false, quantity);

            return Json(new { price = result });
        }

        #region Helpers

        private async Task<Data.Models.Address> BuildAddressModel(AddressViewModel addressVM)
        {
            var state = await _cartService.GetStateByIdAsync(addressVM.State.Id);

            return new Data.Models.Address
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