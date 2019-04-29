using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.Data.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Street { get; set; }

        [Required]
        [MaxLength(90)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        [Required]
        [MaxLength(20)]
        public string Zip { get; set; }
    }
}
