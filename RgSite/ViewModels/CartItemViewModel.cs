﻿using RgSite.Data.Models;

namespace RgSite.ViewModels
{
    public class CartItemViewModel
    {
        public int Id               { get; set; }
        public int ProductId        { get; set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public string ImageUrl      { get; set; }
        public int Quantity         { get; set; }

        public Price Price          { get; set; }
        public AppUser User         { get; set; }

        public decimal Total        =>  Price.Cost * Quantity;
        public string DisplayPrice  =>  $"${Price.Cost}";
        public string DisplayTotal  =>  $"${Total}";
        public string DisplaySize => Price.Size.ToUpper();

    }
}
