using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RgSite.Data.Models
{
    public class BillingDetail
    {
        public int BillingDetailId      { get; set; }

        [Required]
        public string FirstName         { get; set; }

        [Required]
        public string LastName          { get; set; }

        [Required]
        public Address Address          { get; set; }

        [Required]
        [EmailAddress]
        public string Email             { get; set; }

        [Required]
        [Phone]
        public string Phone             { get; set; }

        [Required]
        public bool IsResidential       { get; set; }


        public string CompanyName       { get; set; }
        public string OrderNotes        { get; set; }
    }
}
