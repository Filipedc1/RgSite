using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RgSite.Data;
using RgSite.Models;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
    public class HomeController : Controller
    {
        #region Fields

        private readonly IProduct productService;

        #endregion

        #region Constructor

        public HomeController(IProduct productService)
        {
            this.productService = productService;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var products = await productService.GetAllProductsAsync();

            var vM = new HomeIndexViewModel
            {
                Products = products
            };

            return View(vM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
