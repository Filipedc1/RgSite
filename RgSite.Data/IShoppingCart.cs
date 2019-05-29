using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Data
{
    public interface IShoppingCart
    {
        Task<IEnumerable<CartItem>> GetAllAsync();
        Task<CartItem> GetByIdAsync(int id);
        Task<bool> AddItemAsync(CartItem item);
        Task<bool> DeleteItemAsync(CartItem item);
        Task<bool> IsInCartAsync(int itemId);
        Task<bool> UpdateQuantityAsync(int itemId);
    }
}
