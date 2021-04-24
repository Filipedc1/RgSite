using Microsoft.EntityFrameworkCore;
using RgSite.Core.Interfaces;
using RgSite.Data;
using RgSite.Data.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Order = RgSite.Data.Models.Order;

namespace RgSite.Core.Services
{
    public class OrderService : IOrderService
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

        public async Task<(bool, int)> AddOrderAsync(Order order, IEnumerable<OrderDetail> orderDetails)
        {
            if (order == null) return (false, 0);

            try
            {
                await _database.OrderDetails.AddRangeAsync(orderDetails);
                await _database.Orders.AddAsync(order);
                await _database.SaveChangesAsync();
            }
            catch (Exception)
            {
                return (false, 0);
            }

            return (true, order.OrderId);
        }

        //public async Task<bool> UpdateOrderAsync(Order order, AppUser user, Session session)
        //{
        //    if (order is null) return false;

        //    try
        //    {
        //        order.Total = (decimal)session.AmountTotal / 100;
        //        order.SubTotal = (decimal)session.AmountSubtotal / 100;
        //        order.StripeCustomerId = session.CustomerId;
        //        order.StripeCustomerEmail = session.CustomerEmail;
        //        order.Currency = session.Currency;
        //        order.PaymentIntentId = session.PaymentIntentId;
        //        order.PaymentStatus = session.PaymentStatus;

        //        //order.ShippingDetail = new ShippingDetail
        //        //{
        //        //    RecipientName = session.Shipping.Name,
        //        //    RecipientPhone = session.Shipping.Phone,
        //        //    Carrier = session.Shipping.Carrier,
        //        //    TrackingNumber = session.Shipping.TrackingNumber,
        //        //    Address = new Data.Models.Address
        //        //    {
        //        //        AddressLine1 = session.Shipping.Address.Line1,
        //        //        AddressLine2 = session.Shipping.Address.Line2,
        //        //        Country = session.Shipping.Address.Country,
        //        //        City = session.Shipping.Address.City,
        //        //        State = session.Shipping.Address.State,
        //        //        Zip = session.Shipping.Address.PostalCode
        //        //    }
        //        //};

        //        if (string.IsNullOrWhiteSpace(user.StripeCustomerId))
        //        {
        //            user.StripeCustomerId = session.CustomerId;
        //            _database.AppUsers.Update(user);
        //        }

        //        _database.Orders.Update(order);

        //        await _database.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public async Task<List<OrderDetail>> GetOrderDetailsForOrder(int orderId)
        {
            return await _database.OrderDetails
                                  .Include(o => o.Order)
                                  .Where(o => o.Order.OrderId == orderId)
                                  .ToListAsync();
        }

        public async Task<(bool, int)> CreateOrderAsync(List<CartItem> cartItems, AppUser user)
        {
            var order = new Order
            {
                Placed = DateTime.UtcNow,
                User = user
            };

            var orderDetails = cartItems.Select(item => new OrderDetail
            {
                ProductId = item.ProductId,
                ProductName = item.Name,
                ProductQuantity = item.Quantity,
                ProductCost = item.Price.Cost,
                Order = order
            }).ToList();

            (bool success, int orderId) = await AddOrderAsync(order, orderDetails);

            return (success, orderId);
        }

        // Working on this
        public async Task<bool> ProcessOrderAsync(decimal total, string stripeToken, BillingDetail billingDetail, List<CartItem> cartItems, AppUser user)
        {
            // Process payment - Stripe
            string clientSecret = CreateStripePaymentIntent(total);

            // if charge succeeded
            if (true)
            {
                var order = new Order
                {
                    Total = total,
                    Placed = DateTime.UtcNow,
                    BillingDetail = billingDetail,
                    User = user,
                    //StripeChargeId = charge.Id
                };

                var orderDetails = cartItems.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    ProductName = item.Name,
                    ProductQuantity = item.Quantity,
                    ProductCost = item.Price.Cost,
                    Order = order
                }).ToList();

                (bool success, int orderId) = await AddOrderAsync(order, orderDetails);

                return success;
            }
        }

        public string CreateStripePaymentIntent(decimal amount)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)amount,
                    Currency = "usd",
                    // Verify your integration in this guide by including this parameter
                    Metadata = new Dictionary<string, string>
                    {
                        { "integration_check", "accept_a_payment" }
                    },
                };

                var paymentIntent = new PaymentIntentService().Create(options);

                return paymentIntent.ClientSecret;
            }
            catch (Exception ex)
            {
            }

            return null;
        }
    }
}
