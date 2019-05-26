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
        Task SetProfileImage(string id, string url);
        Task<AppUser> GetCurrentUser();
        Task<string> GetCurrentUserRole();
        string GetCurrentUserName();
    }
}
