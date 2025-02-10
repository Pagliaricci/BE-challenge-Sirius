using EmailService.Modules.Email.Models;
using EmailService.Modules.Users.Models;

namespace EmailService.Modules.Email.EmailProviders
{
public interface IEmailProvider
{
    Task<bool> SendEmailAsync(SendEmailRequest email, User sender);
}
}