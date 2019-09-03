using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmail emailService;

        #endregion

        #region Constructor

        public HomeController(IProduct productService, IEmail emailService)
        {
            this.productService = productService;
            this.emailService = emailService;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var vM = new HomeIndexViewModel();
            return View(vM);
        }

        public async Task<IActionResult> ContactUs()
        {
            var vM = new ContactUsViewModel();
            return View(vM);
        }

        [HttpPost]
        public async Task<IActionResult> ContactUsSubmit(ContactUsViewModel model)
        {
            if (!ModelState.IsValid) return View("ContactUs", model);

            bool good = await emailService.SendEmailAsync(model.Email, model.To, model.Subject, model.Message);

            return View("Index");
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
