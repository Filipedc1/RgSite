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
    }
}
