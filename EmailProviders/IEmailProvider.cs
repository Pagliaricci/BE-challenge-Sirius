using EmailService.Models;

namespace EmailProviders{
public interface IEmailProvider
{
    Task<bool> SendEmailAsync(SendEmailRequest email);
}
}