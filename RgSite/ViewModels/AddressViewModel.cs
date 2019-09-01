using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class AddressViewModel
    {
        public int Id { get; set; }

        public string Street { get; set; }
        public string City { get; set; }

        public State State { get; set; }
        public string Zip { get; set; }
    }
}
