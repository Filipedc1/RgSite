using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId        { get; set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public string ImageUrl      { get; set; }
        public string PriceRange    { get; set; }


        public virtual IEnumerable<CustomerPrice> CustomerPrices    { get; set; }
        public virtual IEnumerable<SalonPrice> SalonPrices          { get; set; }
    }
}
