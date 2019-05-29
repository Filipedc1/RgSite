using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data.Models
{
    public class CustomerPrice 
    {
        public int Id        { get; set; }
        public string Size   { get; set; }
        public decimal Cost  { get; set; }

        public virtual Product Product { get; set; }
    }
}
