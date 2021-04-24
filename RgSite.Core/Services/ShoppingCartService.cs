using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RgSite.Core.Interfaces;
using RgSite.Core.Models;
using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Core.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _database;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public ShoppingCartService(ApplicationDbContext context, IUserService userService, IProductService productService)
        {
            _database = context;
            this._userService = userService;
            this._productService = productService;
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

        public decimal GetCartTotalCost(bool isCustomer, List<CartItem> cartItems)
        {
            decimal total = 0;

            foreach (var item in cartItems)
            {
                item.Price.Cost = isCustomer ? item.Price.CustomerCost : item.Price.SalonCost;
                total += item.Price.Cost * item.Quantity;
            }

            return total;
        }

        public async Task<List<State>> GetStatesAsync(string country, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(country))
                return null;

            string filename = string.Empty;

            if (country == "United States")
                filename = "us-states.json";
            else if (country == "Canada")
                filename = "canada-provinces.json";

            if (string.IsNullOrWhiteSpace(filename))
                return null;

            try
            {
                string filePath = Path.Combine($"{webRootPath}\\countryFiles", filename);

                using var streamReader = new StreamReader(filePath);
                string json = await streamReader.ReadToEndAsync();

                var states = JsonConvert.DeserializeObject<List<State>>(json);

                return states;
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
