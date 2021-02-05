using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RgSite.Core.Interfaces;
using RgSite.Core.Models;
using RgSite.Data;
using RgSite.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public UserService(ApplicationDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _database = context;
            _userManager = userManager;
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

        public async Task SetProfileImageAsync(AppUser user, string url)
        {
            user.ProfileImageUrl = url;
            _database.Update(user);
            await _database.SaveChangesAsync();
        }


        public async Task<AppUser> GetCurrentUserAsync()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<string> GetCurrentUserRoleAsync()
        {
            // If user is not logged in, assume Customer role
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return RoleConstants.Customer;

            var user = await GetCurrentUserAsync();
            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
