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

        //public string PriceRange =>


        public virtual IEnumerable<CustomerPrice> CustomerPrices    { get; set; }
        public virtual IEnumerable<SalonPrice> SalonPrices          { get; set; }



        //private string GetProductPriceRange(Product product)
        //{
        //    string range = string.Empty;

              //need to use HttpContext to get user name or user role
        //    if (App.UserRole == RoleType.Customer && product.CustomerPrices != null && product.CustomerPrices.Count() > 0)
        //    {
        //        range = $"${product.CustomerPrices.FirstOrDefault().Cost} - ${product.CustomerPrices.LastOrDefault().Cost}";
        //    }
        //    else if (App.UserRole == RoleType.Salon && product.SalonPrices != null && product.SalonPrices.Count() > 0)
        //    {
        //        range = $"${product.SalonPrices.FirstOrDefault()} - ${product.SalonPrices.LastOrDefault()}";
        //    }

        //    return range ?? "N/A";
        //}
    }
}
