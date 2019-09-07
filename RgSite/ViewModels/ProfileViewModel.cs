using Microsoft.AspNetCore.Http;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId                        { get; set; }
        public string Username                      { get; set; }
        public string FirstName                     { get; set; }
        public string LastName                      { get; set; }
        public string ProfileImageUrl               { get; set; }
        public bool IsAdmin                         { get; set; }
        public bool IsEmailConfirmed                { get; set; }

        public DateTime MemberSince                 { get; set; }
        public IFormFile ImageUpload                { get; set; }

        public IEnumerable<Order> OrderHistory      { get; set; }

        [Required]
        [EmailAddress]
        public string Email                         { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber                   { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
