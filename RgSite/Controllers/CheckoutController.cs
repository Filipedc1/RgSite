using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RgSite.Core.Extensions;
using RgSite.Core.Interfaces;
using RgSite.Data.Models;
using RgSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _cartService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;

        private readonly IWebHostEnvironment _hosting;
        private readonly IConfiguration _config;

        public CheckoutController(IProductService productService, IShoppingCartService cartService, IUserService userService,
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

        // Working on this
        public async Task<IActionResult> Index(string id)
        {
            bool isCustomer = User.IsCustomer() || User.IsAdmin();
            string userId = id;
            decimal subTotal = 0;

            var cartItems = await _cartService.GetAllAsync(userId);
            if (cartItems != null)
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
                States = await _cartService.GetStatesAsync("United States", _hosting.WebRootPath),
                StripePublicKey = _config["Stripe:PublicKey"].ToString()
            };

            return View("CheckoutForm", vM);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(decimal total)
        {
            string clientSecret = _orderService.CreateStripePaymentIntent(total);
            if (string.IsNullOrWhiteSpace(clientSecret))
                return BadRequest();

            ViewData["ClientSecret"] = clientSecret;
            return View();
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
            bool isCustomer = User.IsCustomer() || User.IsAdmin();
            decimal subTotal = 0;
            decimal total = 0;

            var cartItems = await _cartService.GetAllAsync(user.Id);
            if (cartItems != null)
                subTotal = _cartService.GetCartTotalCost(isCustomer, cartItems);

            // need calculate total with shipping
            // For now skip it.
            total = subTotal;

            //need to check if shiptodifferentaddress is true, if so, use the ShippingFormViewModel property. SKIP this

            bool success = await _orderService.ProcessOrderAsync(total, stripeToken, billingDetail, cartItems, user);
            if (success)
            {
                //await emailService.SendEmailAsync(model.Email, "Your Order Confirmation", "Please login to your account to view your orders");

                await _cartService.ClearCartAsync(user.Id);

                return View("Success");
            }

            return View("Error");
        }

        #region Helpers

        private async Task<Address> BuildAddressModel(AddressViewModel addressVM)
        {
            return new Address
            {
                Id = addressVM.Id,
                Street = addressVM.Street,
                City = addressVM.City,
                State = addressVM.State.Name,
                Zip = addressVM.Zip
            };
        }

        #endregion
    }
}
