using EmailService.Modules.Email.Models;

namespace EmailService.Modules.Email.EmailProviders
{
public interface IEmailProvider
{
    Task<bool> SendEmailAsync(SendEmailRequest email);
}
}