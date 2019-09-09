using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data
{
    public interface IAppUser
    {
        Task<AppUser> GetByIdAsync(string id);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task SetProfileImageAsync(AppUser user, string url);
        Task<AppUser> GetCurrentUserAsync();
        Task<string> GetCurrentUserRoleAsync();
        string GetCurrentUserName();
    }
}
