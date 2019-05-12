using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RgSite.Data;
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

            var vM = new CollectionIndexViewModel
            {
                Collections = collections
            };

            return View(vM);
        }
    }
}