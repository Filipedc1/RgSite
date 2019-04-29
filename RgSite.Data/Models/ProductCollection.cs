using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data.Models
{
    public class ProductCollection
    {
        public int ProductCollectionId  { get; set; }
        public string Name              { get; set; }
        public string Description       { get; set; }
        public string ImageUrl          { get; set; }

        public ICollection<CollectionProduct> CollectionProducts { get; set; }
    }
}
