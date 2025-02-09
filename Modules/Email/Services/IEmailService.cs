using EmailService.Modules.Email.Models;
using System.Threading.Tasks;

namespace EmailService.Modules.Email.Services
{
    public interface IEmailService
    {
       Task<string> SendEmailAsync(SendEmailRequest email, int userId);
    }
}