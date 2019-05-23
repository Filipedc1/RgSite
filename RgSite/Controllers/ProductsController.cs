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

        #endregion

        #region Constructor

        public ProductsController(IProduct productService)
        {
            this.productService = productService;
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
    }
}