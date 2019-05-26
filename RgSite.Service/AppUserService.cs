using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Service
{
    public class AppUserService : IAppUser
    {
        private readonly ApplicationDbContext _database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> userManager;

        public AppUserService(ApplicationDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _database = context;
            this.userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppUser> GetByIdAsync(string id)
        {
            return await _database.AppUsers
                                  .Include(x => x.Address)
                                  .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _database.AppUsers.ToListAsync();
        }

        public async Task SetProfileImage(string id, string url)
        {
            var user = await GetByIdAsync(id);
            user.ProfileImageUrl = url;
            _database.Update(user);
            await _database.SaveChangesAsync();
        }


        public async Task<AppUser> GetCurrentUser()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            return await userManager.FindByNameAsync(username);
        }

        public async Task<string> GetCurrentUserRole()
        {
            var user = await GetCurrentUser();
            var role = await userManager.GetRolesAsync(user);

            return role.FirstOrDefault();
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
