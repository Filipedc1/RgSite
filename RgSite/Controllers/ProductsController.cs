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
            var collections = await productService.GetAllProductCollectionsForCustomersAsync();

            var reorderedCollections = ReorderCollectionsForProductsPage(collections);

            var vM = new CollectionIndexViewModel
            {
                Collections = reorderedCollections
            };

            return View(vM);
        }

        public async Task<IActionResult> CollectionDetail(int id)
        {
            // If user is not logged in, assume Customer role
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRole() : RoleName.Customer;

            var collection = new ProductCollection();

            if (role == RoleName.Customer || role == RoleName.Admin)
                collection = await productService.GetProductCollectionForCustomersByIdAsync(id);
            else
                collection = await productService.GetProductCollectionForSalonsByIdAsync(id);


            var products = collection.CollectionProducts
                                     .Select(prod => new ProductViewModel
                                     {
                                         ProductId = prod.Product.ProductId,
                                         Name = prod.Product.Name,
                                         Description = prod.Product.Description,
                                         ImageUrl = prod.Product.ImageUrl,
                                         CustomerPrices = prod.Product.CustomerPrices,
                                         SalonPrices = prod.Product.SalonPrices,
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
            // If user is not logged in, assume Customer role
            string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRole() : RoleName.Customer;

            var product = new Product();

            if (role == RoleName.Customer || role == RoleName.Admin)
                product = await productService.GetProductForCustomerByIdAsync(id);
            else
                product = await productService.GetProductForSalonByIdAsync(id);

            var vM = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CustomerPrices = product.CustomerPrices,
                SalonPrices = product.SalonPrices,
                PriceRange = productService.GetProductPriceRange(product, role),
                Prices = productService.GetPrices(product, role)
            };

            return View(vM);
        }

        //public async Task<IActionResult> GetPrice(int productId, int selectedSizeId, string selectedQuantity)
        //{
        //    // If user is not logged in, assume Customer role
        //    string role = HttpContext.User.Identity.IsAuthenticated ? await userService.GetCurrentUserRole() : RoleName.Customer;

        //    var product = new Product();
        //    decimal cost = 0;

        //    if (role == RoleName.Customer || role == RoleName.Admin)
        //        product = await productService.GetProductForCustomerByIdAsync(productId);
        //    else
        //        product = await productService.GetProductForSalonByIdAsync(productId);

        //    if (product != null)
        //        cost = productService.GetPrices(product, role).FirstOrDefault(p => p.Id == selectedSizeId).Cost;

        //    int quantity = int.Parse(selectedQuantity);
        //    var result = $"${cost * quantity}";

        //    return Json(new { price = result });
        //}


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