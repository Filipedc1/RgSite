using RgSite.Data;
using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RgSite.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId        { get; set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public string ImageUrl      { get; set; }
        public string PriceRange    { get; set; }
        public int Quantity         { get; set; } = 1;

        public Price Price          { get; set; }
        public Comment Comment      { get; set; } //used for adding new comment.

        public virtual IEnumerable<Price> Prices        { get; set; }
        public virtual IEnumerable<Comment> Comments    { get; set; } = new List<Comment>();

        public string DisplayName => Name?.ToUpper();
    }
}
