using EmailService.Modules.Email.Models;
using EmailService.Modules.Users.Repositories;
using EmailService.Modules.Email.EmailProviders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService.Modules.Email.Services
{
    public class EmailsService : IEmailService
    {
        private readonly IEnumerable<IEmailProvider> _providers;
        private readonly IUserRepository _userRepository;

        public EmailsService(IEnumerable<IEmailProvider> providers, IUserRepository userRepository)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<string> SendEmailAsync(SendEmailRequest email, int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
    
                if (DateTime.UtcNow.Date > user.LastEmailReset.Date)
            {
                user.EmailsSentToday = 0;
                user.LastEmailReset = DateTime.UtcNow;
            }

            if (user.EmailsSentToday >= 1000)
            {
                throw new Exception("User has reached the daily limit of emails");
            }
            Console.WriteLine($"User {user.Username} has sent {user.EmailsSentToday} emails today");


            foreach (var provider in _providers)
            {
                try
                {
                    var result = await provider.SendEmailAsync(email);
                    if (result)
                    {
                        user.EmailsSentToday++;
                        await _userRepository.UpdateUserAsync(user);
                        return "Email sent successfully";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Provider failed: {ex.Message}");
                }
            }

            throw new Exception("All email providers failed");
        }
    }
}