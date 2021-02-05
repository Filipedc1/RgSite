using RgSite.Core.Interfaces;
using System.Threading.Tasks;

namespace RgSite.Core.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            //var client = new SendGridClient(_sendGridApiKey);

            //var from = new EmailAddress(_sendGridFromEmail, _sendGridUser);
            //var to = new EmailAddress(email);

            //var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            //var response = await client.SendEmailAsync(msg);

            //return response.StatusCode == HttpStatusCode.Accepted;
            return false;
        }
    }
}
