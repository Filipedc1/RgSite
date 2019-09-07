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
    public class OrderService : IOrder
    {
        private readonly ApplicationDbContext _database;

        public OrderService(ApplicationDbContext context)
        {
            _database = context;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _database.Orders
                                  .Include(o => o.OrderDetails)
                                  .Include(o => o.BillingDetail)
                                    .ThenInclude(o => o.Address)
                                  .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersForUserAsync(string userId)
        {
            return await _database.Orders
                                  .Include(o => o.OrderDetails)
                                  .Include(o => o.BillingDetail)
                                    .ThenInclude(o => o.Address)
                                  .Include(o => o.User)
                                  .Where(o => o.User.Id == userId)
                                  .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _database.Orders
                                  .Include(o => o.OrderDetails)
                                  .Include(o => o.BillingDetail)
                                    .ThenInclude(o => o.Address)
                                  .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<bool> AddOrderAsync(Order order, IEnumerable<OrderDetail> orderDetails)
        {
            if (order == null) return false;

            try
            {
                await _database.OrderDetails.AddRangeAsync(orderDetails);
                await _database.Orders.AddAsync(order);
                await _database.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<List<OrderDetail>> GetOrderDetailsForOrder(int orderId)
        {
            return await _database.OrderDetails
                                  .Include(o => o.Order)
                                  .Where(o => o.Order.OrderId == orderId)
                                  .ToListAsync();
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
