using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Data
{
    public interface IEmail
    {
        Task<bool> SendEmailAsync(string fromEmail, string toEmail, string subject, string message);
    }
}
