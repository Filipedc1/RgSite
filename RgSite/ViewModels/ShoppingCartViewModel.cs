using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class ShoppingCartViewModel
    {
        public decimal SubTotal     { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total        { get; set; }
        
        public int NumOfItems       { get; set; }

        public List<CartItemViewModel> CartItems { get; set; }
    }
}
