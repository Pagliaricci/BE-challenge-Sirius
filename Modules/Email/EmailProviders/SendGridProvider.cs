using EmailService.Modules.Email.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailService.Modules.Email.EmailProviders
{

public class SendGridProvider : IEmailProvider
{
    private readonly string _apiKey;

    public SendGridProvider(IConfiguration configuration)
    {
        _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") 
                  ?? throw new InvalidOperationException("SendGrid API key is not configured.");

    }

    public async Task<bool> SendEmailAsync(SendEmailRequest email)
    {
        Console.WriteLine("Sending email via SendGrid");
        var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        if (string.IsNullOrEmpty(sendGridApiKey))
        {
            throw new InvalidOperationException("La clave API de SendGrid no se encuentra en las variables de entorno.");
        }

        var client = new SendGridClient(sendGridApiKey);
        var from = new EmailAddress(email.SenderEmail, email.SenderName);
        var to = new EmailAddress(email.RecipientEmail, email.RecipientName);
        var subject = email.Subject;
        var plainTextContent = email.Body;
        var htmlContent = email.Body;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        var response = await client.SendEmailAsync(msg);
        return response.IsSuccessStatusCode;
    }
}
}