﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RgSite.Core.Interfaces;
using RgSite.Core.Models;
using RgSite.Data.Models;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
    public class ProductsController : Controller
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IUserService _userService;

        #endregion

        #region Constructor

        public ProductsController(IProductService productService, IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var collections = await _productService.GetAllProductCollectionsAsync();

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
            string role = await _userService.GetCurrentUserRoleAsync();

            var collection = await _productService.GetProductCollectionByIdAsync(id);
            if (collection == null) return BadRequest();

            var products = collection.CollectionProducts
                                     .Select(prod => new ProductViewModel
                                     {
                                         ProductId = prod.Product.ProductId,
                                         Name = prod.Product.Name,
                                         Description = prod.Product.Description,
                                         ImageUrl = prod.Product.ImageUrl,
                                         Prices = prod.Product.Prices,
                                         PriceRange = _productService.GetProductPriceRange(prod.Product, role)
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
            string role = await _userService.GetCurrentUserRoleAsync();

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return BadRequest();

            var vM = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Comments = await _productService.GetAllCommentsByProductIdAsync(product.ProductId),
                ImageUrl = product.ImageUrl,
                Prices = product.Prices,
                PriceRange = _productService.GetProductPriceRange(product, role),
            };

            return View(vM);
        }

        public async Task<IActionResult> GetPrice(int productId, string selectedSize)
        {
            string role = await _userService.GetCurrentUserRoleAsync();

            var product = await _productService.GetProductByIdAsync(productId);
            decimal cost = 0;

            if (product != null)
            {
                var price = product.Prices.FirstOrDefault(p => p.Size == selectedSize);
                cost = (role == RoleConstants.Customer || role == RoleConstants.Admin) ? price.CustomerCost : price.SalonCost;
            }

            string result = $"${cost}";

            return Json(new { price = result });
        }

        #region Comment Methods

        [HttpPost]
        public async Task<IActionResult> AddComment(ProductViewModel vm)
        {
            if (!ModelState.IsValid || vm.Comment == null)
                return BadRequest();

            var user = await _userService.GetCurrentUserAsync();

            if (await _productService.AddNewCommentAsync(vm.ProductId, vm.Comment, user))
                return RedirectToAction("ProductDetail", "Products", new { @id = vm.ProductId });

            return BadRequest();
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _productService.GetCommentByIdAsync(id);
            var product = await _productService.GetProductByIdAsync(comment.Product.ProductId);

            if (await _productService.DeleteCommentAsync(comment))
                return RedirectToAction("ProductDetail", new { id = product.ProductId });

            return BadRequest();
        }

        #endregion

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