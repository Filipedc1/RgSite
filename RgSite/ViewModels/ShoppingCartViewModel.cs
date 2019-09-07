using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class ShoppingCartViewModel
    {
        public decimal SubTotal         { get; set; } = 0.00M;
        public decimal ShippingCost     { get; set; } = 0.00M;
        public decimal Total            { get; set; } = 0.00M;

        public string DisplaySubTotal       => $"${SubTotal}";
        public string DisplayShippingCost   => $"${ShippingCost}";
        public string DisplayTotal          => $"${Total}";

        // User.Id Represents the shoppingcartId
        public AppUser User { get; set; } = new AppUser();

        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    }
}
