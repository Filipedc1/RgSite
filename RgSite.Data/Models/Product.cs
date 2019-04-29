using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data.Models
{
    public class Product
    {
        public int ProductId        { get; set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public string ImageUrl      { get; set; }


        public virtual IEnumerable<CustomerPrice> CustomerPrices    { get; set; }
        public virtual IEnumerable<SalonPrice> SalonPrices          { get; set; }

        public ICollection<CollectionProduct> CollectionProducts { get; set; }
    }
}
