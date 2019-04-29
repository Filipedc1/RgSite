using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RgSite.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }

        public DateTime MemberSince { get; set; }
        public Address Address { get; set; }
        public Salon Salon { get; set; }
    }
}
