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
    public class ProductsController : Controller
    {
        #region Fields

        private readonly IProduct productService;
        private readonly IAppUser userService;

        #endregion

        #region Constructor

        public ProductsController(IProduct productService, IAppUser userService)
        {
            this.productService = productService;
            this.userService = userService;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var collections = await productService.GetAllProductCollectionsAsync();

            if (collections == null || collections.Count == 0)
                return BadRequest();

            var reorderedCollections = ReorderCollectionsForProductsPage(collections);

            var vM = new CollectionIndexViewModel
            {
                Collections = reorderedCollections
            };

            return View(vM);
        }

        public async Task<IActionResult> CollectionDetail(int id)
        {
            string role = await userService.GetCurrentUserRoleAsync();

            var collection = await productService.GetProductCollectionByIdAsync(id);
            if (collection == null) return BadRequest();

            var products = collection.CollectionProducts
                                     .Select(prod => new ProductViewModel
                                     {
                                         ProductId = prod.Product.ProductId,
                                         Name = prod.Product.Name,
                                         Description = prod.Product.Description,
                                         ImageUrl = prod.Product.ImageUrl,
                                         Prices = prod.Product.Prices,
                                         PriceRange = productService.GetProductPriceRange(prod.Product, role)
                                     })
                                     .ToList();

            var vM = new CollectionDetailViewModel
            {
                ProductCollectionId = collection.ProductCollectionId,
                Name = collection.Name,
                Description = collection.Description,
                ImageUrl = collection.ImageUrl,
                Products = products
            };

            return View(vM);
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            string role = await userService.GetCurrentUserRoleAsync();

            var product = await productService.GetProductByIdAsync(id);
            if (product == null) return BadRequest();

            var vM = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Prices = product.Prices,
                PriceRange = productService.GetProductPriceRange(product, role),
            };

            return View(vM);
        }

        public async Task<IActionResult> GetPrice(int productId, string selectedSize)
        {
            string role = await userService.GetCurrentUserRoleAsync();

            var product = await productService.GetProductByIdAsync(productId);
            decimal cost = 0;

            if (product != null)
            {
                var price = product.Prices.FirstOrDefault(p => p.Size == selectedSize);
                cost = (role == RoleName.Customer || role == RoleName.Admin) ? price.CustomerCost : price.SalonCost;
            }

            string result = $"${cost}";

            return Json(new { price = result });
        }


        #region Helpers

        //Reorder collections to be in the same order as the old Rg Site.
        private List<ProductCollection> ReorderCollectionsForProductsPage(List<ProductCollection> collections)
        {
            var reorderedList = new List<ProductCollection>
            {
                collections.FirstOrDefault(c => c.Name.Contains("hairb")),
                collections.FirstOrDefault(c => c.Name.Contains("amazon")),
                collections.FirstOrDefault(c => c.Name.Contains("amazon advanced")),
                collections.FirstOrDefault(c => c.Name.Contains("bb creme")),
                collections.FirstOrDefault(c => c.Name.Contains("sleek now")),
                collections.FirstOrDefault(c => c.Name.Contains("filler")),
                collections.FirstOrDefault(c => c.Name.Contains("chemical protection")),
                collections.FirstOrDefault(c => c.Name.Contains("maintenance")),
                collections.FirstOrDefault(c => c.Name.Contains("collagen")),
                collections.FirstOrDefault(c => c.Name.Contains("cc")),
                collections.FirstOrDefault(c => c.Name.Contains("fix and mold")),
            };

            return reorderedList;
        }

        #endregion
    }
}