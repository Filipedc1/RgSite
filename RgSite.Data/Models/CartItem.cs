using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RgSite.Data.Models
{
    public class CartItem
    {
        public int Id                   { get; set; }
        public int ProductId            { get; set; }
        public string Name              { get; set; }
        public string Description       { get; set; }
        public string ImageUrl          { get; set; }
        public int Quantity             { get; set; }

        [NotMapped]
        public Price Price              { get; set; }
        public int PriceId              { get; set; }

        public virtual AppUser User     { get; set; }
    }
}
