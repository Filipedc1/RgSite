using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RgSite.Core.Helpers;
using RgSite.Core.Interfaces;
using RgSite.Data.Models;
using RgSite.ViewModels;

namespace RgSite.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        #region Fields

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IWebHostEnvironment _hosting;

        #endregion

        #region Controller

        public ProfileController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUserService userService, IOrderService orderService, IWebHostEnvironment he)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _orderService = orderService;
            _hosting = he;
        }

        #endregion

        #region Actions

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
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
            var user = await _userService.GetByIdAsync(id);
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
                OrderHistory = await _orderService.GetAllOrdersAsync(),
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
            var user = await _userService.GetByIdAsync(id);
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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { message = "failed" });

            if (profileVM.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, profileVM.Email);
                if (!setEmailResult.Succeeded)
                {
                    return Json(new { message = "failed" });
                }
            }

            if (profileVM.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, profileVM.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return Json(new { message = "failed" });
                }
            }

            if (image != null)
            {
                if (ImageUtil.UploadImage(image, _hosting.WebRootPath))
                {
                    string imageUrl = $"/images/{Path.GetFileName(image.FileName)}";
                    await _userService.SetProfileImageAsync(user, imageUrl);
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            return Json(new { message = "success" });
        }

        public async Task<IActionResult> OrderHistoryDetail(int id)
        {
            var orderDetails = await _orderService.GetOrderDetailsForOrder(id);

            var viewMod = new OrderDetailListViewModel
            {
                OrderDetails = orderDetails
            };

            return PartialView(viewMod);
        }
    }

    #endregion
}