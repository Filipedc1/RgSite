using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RgSite.Core.Extensions;
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
        private readonly IEmailService _emailService;

        private readonly IWebHostEnvironment _hosting;
        private readonly IConfiguration _config;

        #region Constructor

        public ShoppingCartController(IProductService productService, IShoppingCartService cartService, IUserService userService,
                                      IOrderService orderService, IEmailService emailService, IWebHostEnvironment he,
                                      IConfiguration config)
        {
            _productService = productService;
            _userService = userService;
            _cartService = cartService;
            _orderService = orderService;
            _emailService = emailService;
            _hosting = he;
            _config = config;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            bool isCustomer = User.IsCustomer() || User.IsAdmin();
            string userId = User.GetUserId();
            decimal subTotal = 0;

            var cartItems = await _cartService.GetAllAsync(userId);
            if (!cartItems.Any())
                return View(new ShoppingCartViewModel());

            subTotal = _cartService.GetCartTotalCost(isCustomer, cartItems);

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

        [HttpDelete]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _cartService.GetByIdAsync(id);

            if (item == null) return BadRequest();

            if (await _cartService.DeleteItemAsync(item))
                return RedirectToAction("Index");

            return BadRequest();
        }

        // Updates cart when quantity value is modified
        public async Task<IActionResult> UpdateCart(int itemId, int productId, int selectedSizeId, string selectedQuantity)
        {
            bool isCustomer = User.IsCustomer() || User.IsAdmin();

            var product = await _productService.GetProductByIdAsync(productId);
            decimal cost = 0;

            if (product != null)
            {
                var price = product.Prices.FirstOrDefault(p => p.Id == selectedSizeId);
                cost = isCustomer ? price.CustomerCost : price.SalonCost;
            }

            int quantity = int.Parse(selectedQuantity);
            string result = $"${cost * quantity}";

            await _cartService.UpdateQuantityAsync(itemId, false, quantity);

            return Json(new { price = result });
        }

        //public IActionResult CalculateShipping()
        //{
        //    // pass in address info from view

        //    var payload = new JsonPayload();

        //    var shipment = _shippingService.CreateShipment();
        //    if (shipment is null)
        //        payload.Exception = "Could not calculate shipping information.";

        //    payload.Payload = shipment.rates;

        //    return Json(payload);
        //}
    }
}