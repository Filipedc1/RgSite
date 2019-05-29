using System;
using System.Collections.Generic;
using System.Text;

namespace RgSite.Data.Models
{
    public class CartItem
    {
        public int Id               { get; set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public string ImageUrl      { get; set; }
        public int Quantity         { get; set; }

        public Price Price { get; set; }
    }
}
