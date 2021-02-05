using RgSite.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RgSite.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(int id);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetAllOrdersForUserAsync(string userId);
        Task<List<OrderDetail>> GetOrderDetailsForOrder(int orderId);
        Task<bool> AddOrderAsync(Order order, IEnumerable<OrderDetail> items);

        Task<decimal> GetCartTotalCostWithShippingAsync(string userId);
    }
}
