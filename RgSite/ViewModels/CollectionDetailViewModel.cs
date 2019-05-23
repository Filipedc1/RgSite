using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class CollectionDetailViewModel
    {
        public int ProductCollectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
