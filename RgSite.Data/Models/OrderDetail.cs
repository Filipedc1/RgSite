using System;
using System.Collections.Generic;
using System.Text;

namespace RgSite.Data.Models
{
    public class OrderDetail
    {
        public int Id                   { get; set; }
        public int ProductId            { get; set; }
        public string ProductName       { get; set; }
        public int ProductQuantity      { get; set; }
        public string ProductSize       { get; set; }
        public decimal ProductCost      { get; set; }

        public Price ProductPrice       { get; set; }
        public Order Order              { get; set; }
    }
}
