using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class PaymentDetailViewModel
    {
        public int PaymentDetailId          { get; set; }

        [Display(Name = "Name on card")]
        public string CardOwnerName         { get; set; }

        [Display(Name = "Card Number")]
        [CreditCard]
        public string CardNumber            { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Expiration          { get; set; }

        public int CVV                      { get; set; }
        public CardType CardType            { get; set; }
        public IEnumerable<CardType> CardTypes { get; set; }
    }
}
