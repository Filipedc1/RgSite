using RgSite.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RgSite.Core.Interfaces
{
    public interface IUserService
    {
        Task<AppUser> GetByIdAsync(string id);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task SetProfileImageAsync(AppUser user, string url);
        Task<AppUser> GetCurrentUserAsync();
        Task<string> GetCurrentUserRoleAsync();
        string GetCurrentUserName();
    }
}
