using System;
using System.Collections.Generic;
using System.Text;

namespace RgSite.Data.Models
{
    public class Comment
    {
        public int Id               { get; set; }
        public string Content       { get; set; }
        public DateTime TimeSent    { get; set; }
        public Product Product      { get; set; }

        public virtual AppUser User { get; set; }
    }
}
