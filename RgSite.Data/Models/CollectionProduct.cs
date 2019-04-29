using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data.Models
{
    // Junction table. Many to many relationship between ProductCollection and Product
    public class CollectionProduct
    {
        public int ProductCollectionId { get; set; }
        public ProductCollection ProductCollection { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
