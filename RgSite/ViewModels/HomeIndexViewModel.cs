using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        //public IEnumerable<Story> Stories { get; set; }
    }
}
