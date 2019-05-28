using RgSite.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RgSite.Data
{
    public interface IPrice
    {
        int Id { get; set; }
        string Size { get; set; }
        decimal Cost { get; set; }

        Product Product { get; set; }
    }
}
