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


        public virtual IEnumerable<Price> Prices        { get; set; }
        public virtual IEnumerable<Comment> Comments    { get; set; }

        public ICollection<CollectionProduct> CollectionProducts { get; set; }
    }
}
