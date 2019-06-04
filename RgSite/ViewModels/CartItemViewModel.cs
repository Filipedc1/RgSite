using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class CartItemViewModel
    {
        public int Id               { get; set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public string ImageUrl      { get; set; }
        public int Quantity         { get; set; }

        public Price Price          { get; set; }
        public AppUser User         { get; set; }

        public decimal Total =>  Price.Cost * Quantity;

    }
}
