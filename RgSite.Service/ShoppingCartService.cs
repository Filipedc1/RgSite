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
    public class ShoppingCartService : IShoppingCart
    {
        private readonly ApplicationDbContext _database;
        private readonly IAppUser userService;

        public ShoppingCartService(ApplicationDbContext context, IAppUser userService)
        {
            _database = context;
            this.userService = userService;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            return await _database.ShoppingCartItems
                                  .Include(i => i.User)
                                  .ToListAsync();
        }

        public async Task<CartItem> GetByIdAsync(int id)
        {
            return await _database.ShoppingCartItems
                                  .Include(i => i.User)
                                  .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> AddItemAsync(CartItem item)
        {
            if (item == null) return false;

            await _database.ShoppingCartItems.AddAsync(item);
            await _database.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteItemAsync(CartItem item)
        {
            if (item == null) return false;

            _database.ShoppingCartItems.Remove(item);
            await _database.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsInCartAsync(int itemId)
        {
            return await _database.ShoppingCartItems.AnyAsync(item => item.Id == itemId);
        }

        public async Task<bool> UpdateQuantityAsync(int itemId)
        {
            var item = await _database.ShoppingCartItems.FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null) return false;

            item.Quantity += 1;
            await _database.SaveChangesAsync();

            return true;
        }
    }
}
