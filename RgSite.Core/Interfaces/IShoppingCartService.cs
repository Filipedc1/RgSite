using RgSite.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RgSite.Core.Interfaces
{
    public interface IShoppingCartService
    {
        Task<List<CartItem>> GetAllAsync(string userId);
        Task<CartItem> GetByIdAsync(int itemId);
        Task<bool> AddItemAsync(CartItem item);
        Task<bool> DeleteItemAsync(CartItem item);
        Task<bool> IsInCartAsync(int itemId, int productSizeId);
        Task<bool> UpdateQuantityAsync(int itemId, bool isAdd, int quantity = 0);
        Task<bool> ClearCartAsync(string userId);
        decimal GetCartTotalCost(bool isCustomer, List<CartItem> cartItems);
        Task<List<State>> GetStatesAsync(string country, string webRootPath);
    }
}
