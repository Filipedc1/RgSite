using Microsoft.AspNetCore.Identity.UI.Services;
using RgSite.Data;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RgSite.Service
{
    public class EmailService : IEmail
    {
        // Not done
        public async Task<bool> SendEmailAsync(string fromEmail, string toEmail, string subject, string message)
        {
            var emailMessage = new MailMessage();

            emailMessage.From = new MailAddress(fromEmail);
            emailMessage.To.Add(toEmail);
            emailMessage.Subject = subject;
            emailMessage.Body = message;

            try
            {
                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    await client.SendMailAsync(emailMessage);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
