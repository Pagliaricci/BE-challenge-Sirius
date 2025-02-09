using EmailService.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;

namespace EmailProviders
{
    public class MailgunProvider : IEmailProvider
    {
        private readonly string _apiKey;
        private readonly string _domain;

        public MailgunProvider(IConfiguration configuration)
        {
            _apiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY") ?? throw new InvalidOperationException("Mailgun API key is not configured.");
            _domain = Environment.GetEnvironmentVariable("MAILGUN_DOMAIN") ?? throw new InvalidOperationException("Mailgun domain is not configured.");
        }

        public async Task<bool> SendEmailAsync(SendEmailRequest email)
        {
            Console.WriteLine("Sending email via Mailgun");
            Console.WriteLine($"API Key: {_apiKey}");

            var options = new RestClientOptions("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", _apiKey)
            };
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddParameter ("domain", "sandbox277ffdf9863e4ff891b8d3f184eb5e03.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter ("from", email.SenderEmail);
            request.AddParameter("to", email.RecipientEmail);
            request.AddParameter("subject", email.Subject);
            request.AddParameter("text", email.Body);
            request.Method = Method.Post;

            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
    }
}