using EmailService.Modules.Email.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;
using EmailService.Modules.Users.Models;

namespace EmailService.Modules.Email.EmailProviders
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

        public async Task<bool> SendEmailAsync(SendEmailRequest email, User sender)
        {


            var options = new RestClientOptions("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", _apiKey)
            };
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddParameter ("domain", "sandbox266d3a3995624957a34056827b92e92f.mailgun.org", ParameterType.UrlSegment);            request.Resource = "{domain}/messages";
            request.AddParameter ("from", sender.Email);
            request.AddParameter("to", email.To);
            request.AddParameter("subject", email.Subject);
            request.AddParameter("text", email.Body);
            request.Method = Method.Post;

            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
    }
}