using Microsoft.Extensions.Configuration;
using RgSite.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RgSite.ViewModels
{
    public class CheckoutFormViewModel
    {
        public int Id                                           { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName                                 { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName                                  { get; set; }

        [Required]
        public AddressViewModel Address                         { get; set; }

        [Required]
        [EmailAddress]
        public string Email                                     { get; set; }

        [Required]
        [Phone]
        public string Phone                                     { get; set; }

        [Required]
        [Display(Name ="Is it Residential?")]
        public bool IsResidential                               { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName                               { get; set; }

        [Display(Name = "Order Notes (Optional)")]
        public string OrderNotes                                { get; set; }

        [Display(Name = "Ship to a different address?")]
        public bool ShipToDifferentAddress                      { get; set; }


        //public PaymentDetailViewModel PaymentDetail             { get; set; } = new PaymentDetailViewModel();
        public ShoppingCartViewModel ShoppingCart               { get; set; }

        public State State                                      { get; set; }
        public IEnumerable<State> States                        { get; set; }


        // Shipping Info IF different from Billing info
        public ShippingFormViewModel ShippingInfo { get; set; } = new ShippingFormViewModel();

        public string StripePublicKey { get; set; }
    }
}
