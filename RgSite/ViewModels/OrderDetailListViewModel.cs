using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class OrderDetailListViewModel
    {
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
