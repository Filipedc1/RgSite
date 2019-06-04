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
                                  .Where(i => i.User.Id == userId)
                                  .ToListAsync();
        }

        public async Task<CartItem> GetByIdAsync(int itemId)
        {
            return await _database.ShoppingCartItems
                                  .Include(i => i.User)
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

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartItems = await GetAllAsync(userId);

            if (cartItems == null || cartItems.Count == 0) return false;

            _database.ShoppingCartItems.RemoveRange(cartItems);
            await _database.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetCartTotalCostAsync(string userId, string role)
        {
            decimal total = 0;

            var cartItems = await GetAllAsync(userId);

            if (cartItems == null || cartItems.Count == 0) return total;

            foreach (var item in cartItems)
            {
                item.Price = await GetPriceForCartItem(item.PriceId, role);
                total += item.Price.Cost * item.Quantity;
            }

            return total;
        }

        public async Task<Price> GetPriceForCartItem(int priceId, string role)
        {
            Price price = null;

            if (role == RoleName.Customer || role == RoleName.Admin)
            {
                var customerPrices = await productService.GetCustomerPrices();
                price = customerPrices.Select(p => new Price
                {
                    Id = p.Id,
                    Size = p.Size,
                    Cost = p.Cost
                }).FirstOrDefault(p => p.Id == priceId);
            }
            else
            {
                var salonPrices = await productService.GetSalonPrices();
                price = salonPrices.Select(p => new Price
                {
                    Id = p.Id,
                    Size = p.Size,
                    Cost = p.Cost
                }).FirstOrDefault(p => p.Id == priceId);
            }

            return price;
        }

        public async Task<decimal> GetCartTotalCostWithShippingAsync(string userId)
        {
            throw new NotImplementedException();
            //decimal total = 0;

            //var cartItems = await GetAllAsync(userId);
            ////var shippingCosts = 

            //if (cartItems == null || cartItems.Count == 0) return total;

            //cartItems.ForEach(i => total += i.Price.Cost * i.Quantity);

            //return total * shippingCosts;
        }
    }
}
