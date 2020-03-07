using System;
using System.Collections.Generic;
using System.Text;

namespace RgSite.Data.Models
{
    public class Order
    {
        public int OrderId                                      { get; set; }
        public decimal Total                                    { get; set; }
        public DateTime Placed                                  { get; set; }

        public virtual IEnumerable<OrderDetail> OrderDetails    { get; set; }
        public virtual BillingDetail BillingDetail              { get; set; }
        public virtual AppUser User                             { get; set; }

        //public string StripeChargeCustomerId { get; set; }
        //public bool HasShipped { get; set; }
        //public decimal OrderDiscount { get; set; }
        //public OrderStatus OrderStatus { get; set; } 
        //public PaymentStatus PaymentStatus { get; set; } 
        //public ShippingStatus ShippingStatus { get; set; } 
        //public string ShippingMethod { get; set; }
        //public string TrackingNumber { get; set; }
        //public decimal? TotalWeight { get; set; }
        //public DateTime? ShippedDateUtc { get; set; }
        //public DateTime? DeliveryDateUtc { get; set; }

        //Points earned for placing this order
        //public int? RewardPoints { get; set; }

        //Points used when placing this order
        //public int? RewardPointsUsed { get; set; }
    }
}
