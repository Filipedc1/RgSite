using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RgSite.Core.Interfaces;
using RgSite.Models;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
    public class HomeController : Controller
    {
        #region Fields

        private readonly IProductService productService;
        private readonly IEmailService emailService;

        #endregion

        #region Constructor

        public HomeController(IProductService productService, IEmailService emailService)
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

            bool result = await emailService.SendEmailAsync(model.Email, model.Subject, model.Message);

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
