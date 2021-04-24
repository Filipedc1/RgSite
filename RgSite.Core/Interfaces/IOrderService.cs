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
        Task<(bool, int)> AddOrderAsync(Order order, IEnumerable<OrderDetail> items);
        //Task<bool> UpdateOrderAsync(Order order, AppUser user, Stripe.Checkout.Session session);
        Task<(bool, int)> CreateOrderAsync(List<CartItem> cartItems, AppUser user);
        Task<bool> ProcessOrderAsync(decimal total, string stripeToken, BillingDetail billingDetail, List<CartItem> cartItems, AppUser user);
        string CreateStripePaymentIntent(decimal amount);
    }
}
