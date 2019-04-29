using Microsoft.EntityFrameworkCore;
using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Service
{
    public class AppUserService : IAppUser
    {
        private readonly ApplicationDbContext _database;

        public AppUserService(ApplicationDbContext context)
        {
            _database = context;
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
    }
}
