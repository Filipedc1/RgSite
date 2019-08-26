﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        #region Constructor

        public ShoppingCartController(IProduct productService, IShoppingCart cartService, IAppUser userService)
        {
            this.productService = productService;
            this.userService = userService;
            this.cartService = cartService;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRole() : RoleName.Customer;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartItems = await cartService.GetAllAsync(userId);
            var subTotal = await cartService.GetCartTotalCostAsync(userId, role);

            var items = cartItems.Select(item => new CartItemViewModel
            {
                Id = item.Id,
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
                NumOfItems = cartItems.Count
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
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRole() : RoleName.Customer;
            var user = await userService.GetCurrentUser();

            var product = await productService.GetProductByIdAsync(model.ProductId);

            var price = productService.GetPrices(product, role)
                                      .FirstOrDefault(p => p.Id == model.Price.Id);

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

            if (await cartService.DeleteItemAsync(item))
                return RedirectToAction("Index");

            return BadRequest();
        }

        // Updates cart when quantity value is modified
        public async Task<IActionResult> UpdateCart(int productId, int selectedSizeId, string selectedQuantity)
        {
            //If user is not logged in, assume Customer role
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRole() : RoleName.Customer;

            var product = await productService.GetProductByIdAsync(productId);
            decimal cost = 0;

            if (product != null)
                cost = productService.GetPrices(product, role).FirstOrDefault(p => p.Id == selectedSizeId).Cost;

            int quantity = int.Parse(selectedQuantity);
            var result = $"${cost * quantity}";

            await cartService.UpdateQuantityAsync(productId, false, quantity);

            return Json(new { price = result });
        }
    }
}