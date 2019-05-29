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
        private readonly IAppUser userService;

        #region Constructor

        public ShoppingCartController(IProduct productService, IAppUser userService)
        {
            this.productService = productService;
            this.userService = userService;
        }

        #endregion

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
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

            var cartItem = new CartItem
            {
                Id = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Quantity = model.Quantity,
                Price = price
            };

            //productId value is not being passed to Detail action why??
            return RedirectToAction("ProductDetail", "Products", new { product.ProductId });
        }
    }
}