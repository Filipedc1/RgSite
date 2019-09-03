using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class ContactUsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }


        public string To { get; } = "filipe_catarcione@hotmail.com";
    }
}
