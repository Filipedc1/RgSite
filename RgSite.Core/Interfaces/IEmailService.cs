using System.Threading.Tasks;

namespace RgSite.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);
    }
}
