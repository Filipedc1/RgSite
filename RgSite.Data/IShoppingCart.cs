using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Data
{
    public interface IShoppingCart
    {
        Task<List<CartItem>> GetAllAsync(string userId);
        Task<CartItem> GetByIdAsync(int itemId);
        Task<bool> AddItemAsync(CartItem item);
        Task<bool> DeleteItemAsync(CartItem item);
        Task<bool> IsInCartAsync(int itemId, int productSizeId);
        Task<bool> UpdateQuantityAsync(int itemId, bool isAdd, int quantity = 0);
        Task<bool> ClearCartAsync(string userId);
        Task<decimal> GetCartTotalCostAsync(string userId, string role, List<CartItem> cartItems);
        Task<decimal> GetCartTotalCostWithShippingAsync(string userId);
    }
}
