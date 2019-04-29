using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data.Models
{
    public class Salon
    {
        public int Id               { get; set; }
        public string Name          { get; set; }
        public string License       { get; set; }
        public string PhoneNumber   { get; set; }
        public Address Address      { get; set; }
    }
}
