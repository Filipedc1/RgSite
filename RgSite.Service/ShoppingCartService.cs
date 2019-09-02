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
        private readonly IProduct productService;

        public ShoppingCartService(ApplicationDbContext context, IAppUser userService, IProduct productService)
        {
            _database = context;
            this.userService = userService;
            this.productService = productService;
        }

        public async Task<List<CartItem>> GetAllAsync(string userId)
        {
            return await _database.ShoppingCartItems
                                  .Include(i => i.User)
                                  .Include(i => i.Price)
                                  .Where(i => i.User.Id == userId)
                                  .ToListAsync();
        }

        public async Task<CartItem> GetByIdAsync(int itemId)
        {
            return await _database.ShoppingCartItems
                                  .Include(i => i.User)
                                  .Include(i => i.Price)
                                  .FirstOrDefaultAsync(i => i.Id == itemId);
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

        public async Task<bool> IsInCartAsync(int itemId, int productSizeId)
        {
            return await _database.ShoppingCartItems.AnyAsync(item => item.Id == itemId && item.Price.Id == productSizeId);
        }

        public async Task<bool> UpdateQuantityAsync(int itemId, bool isAdd, int quantity = 0)
        {
            var item = await _database.ShoppingCartItems.FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null) return false;

            if (isAdd)
                item.Quantity += 1;
            else
                item.Quantity = quantity;

            await _database.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartItems = await GetAllAsync(userId);

            if (cartItems == null || cartItems.Count == 0) return false;

            _database.ShoppingCartItems.RemoveRange(cartItems);
            await _database.SaveChangesAsync();

            return true;
        }

        public decimal GetCartTotalCostAsync(string userId, string role, List<CartItem> cartItems)
        {
            decimal total = 0;

            foreach (var item in cartItems)
            {
                item.Price.Cost = (role == RoleName.Customer || role == RoleName.Admin) ? item.Price.CustomerCost : item.Price.SalonCost;
                total += item.Price.Cost * item.Quantity;
            }

            return total;
        }

        public async Task<IEnumerable<State>> GetStatesAsync()
        {
            return await _database.States.ToListAsync();
        }

        public async Task<State> GetStateByIdAsync(int id)
        {
            return await _database.States.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
