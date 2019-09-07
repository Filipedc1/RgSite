using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RgSite.Data;
using RgSite.Data.Models;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        #region Fields

        private readonly UserManager<AppUser> userManager;
        private readonly IAppUser userService;
        private readonly IOrder orderService;
        private readonly IHostingEnvironment _hosting;

        #endregion

        #region Controller

        public ProfileController(UserManager<AppUser> userManager, IAppUser userService, IOrder orderService, IHostingEnvironment he)
        {
            this.userManager = userManager;
            this.userService = userService;
            this.orderService = orderService;
            this._hosting = he;
        }

        #endregion

        #region Actions

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var profiles = await userService.GetAllAsync();
            if (!profiles.Any()) return NotFound();

            var profilesVM = profiles.Select(user => new ProfileViewModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                MemberSince = user.MemberSince
            });

            var viewMod = new ProfileListViewModel()
            {
                Profiles = profilesVM
            };

            return View(viewMod);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var viewMod = new ProfileViewModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                MemberSince = user.MemberSince,
                OrderHistory = await orderService.GetAllOrdersAsync(),
                ProfileImageUrl = user.ProfileImageUrl,
                IsAdmin = User.IsInRole("Admin") ? true : false
            };

            return View(viewMod);
        }

        public async Task<IActionResult> OrderHistoryDetail(int id)
        {
            var orderDetails = await orderService.GetOrderDetailsForOrder(id);

            var viewMod = new OrderDetailListViewModel
            {
                OrderDetails = orderDetails
            };

            return PartialView(viewMod);
        }
    }

    #endregion
}