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
using Newtonsoft.Json;
using RgSite.Data;
using RgSite.Data.Models;
using RgSite.Helpers;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        #region Fields

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IAppUser userService;
        private readonly IOrder orderService;
        private readonly IHostingEnvironment _hosting;

        #endregion

        #region Controller

        public ProfileController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAppUser userService, IOrder orderService, IHostingEnvironment he)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userService = userService;
            this.orderService = orderService;
            this._hosting = he;
        }

        #endregion

        #region Actions

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await userService.GetAllAsync();
            if (!users.Any()) return NotFound();

            var profilesVM = users.Select(user => new ProfileViewModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                ProfileImageUrl = user.ProfileImageUrl,
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
                UserId = user.Id,
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

        public async Task<IActionResult> DeleteUser(string id)
        {
            return View();
        }

        public async Task<IActionResult> AddUser(string id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(ProfileViewModel profileVM, IFormFile image)
        {
            return View();
        }

        public async Task<IActionResult> UpdateAccount(string id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var viewMod = new ProfileViewModel
            {
                UserId = id,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return PartialView("ProfileForm", viewMod);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccount(ProfileViewModel profileVM, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                var vM = new ProfileViewModel();
                return View("ProfileForm", vM);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { message = "failed" });

            if (profileVM.Email != user.Email)
            {
                var setEmailResult = await userManager.SetEmailAsync(user, profileVM.Email);
                if (!setEmailResult.Succeeded)
                {
                    return Json(new { message = "failed" });
                }
            }

            if (profileVM.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await userManager.SetPhoneNumberAsync(user, profileVM.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return Json(new { message = "failed" });
                }
            }

            if (image != null)
            {
                if (ImageUtil.UploadImage(image, _hosting))
                {
                    var imageUrl = "/images/" + Path.GetFileName(image.FileName);
                    await userService.SetProfileImageAsync(user, imageUrl);
                }
            }

            await signInManager.RefreshSignInAsync(user);

            return Json(new { message = "success" });
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