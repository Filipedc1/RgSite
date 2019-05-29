using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RgSite.Data;
using RgSite.Data.Models;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
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

        public IActionResult Index()
        {
            return View();
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

            // If user is not logged in, assume Customer role
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRole() : RoleName.Customer;

            var product = new Product();

            if (role == RoleName.Customer)
                product = await productService.GetProductForCustomerByIdAsync(model.ProductId);
            else
                product = await productService.GetProductForSalonByIdAsync(model.ProductId);

            var price = productService.GetPrices(product, role)
                                      .FirstOrDefault(p => p.Id == model.Price.Id);

            if (await cartService.IsInCartAsync(model.ProductId))
            {
                await cartService.UpdateQuantityAsync(model.ProductId);
                return RedirectToAction("ProductDetail", "Products", new { id = product.ProductId });
            }
            else
            {
                var cartItem = new CartItem
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Quantity = model.Quantity,
                    Price = price
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
    }
}