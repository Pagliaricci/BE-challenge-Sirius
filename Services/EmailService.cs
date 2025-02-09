using EmailService.Models;
using EmailService.Repositories;
using EmailProviders;

namespace EmailService.Services
{
    public class EmailsService
    {
        private readonly IEnumerable<IEmailProvider> _providers;
        private readonly UserRepository _userRepository;

        public EmailsService(IEnumerable<IEmailProvider> providers, UserRepository userRepository)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<string> SendEmailAsync(SendEmailRequest email, int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
    
            if (user.EmailsSentToday >= 1000)
            {
                throw new Exception("User has reached the daily limit of emails");
            }
            Console.WriteLine($"User {user.Username} has sent {user.EmailsSentToday} emails today");
            if (DateTime.UtcNow.Date > user.LastEmailReset.Date)
            {
                user.EmailsSentToday = 0;
                user.LastEmailReset = DateTime.UtcNow;
            }
            await _userRepository.UpdateUserAsync(user);

            var exceptions = new List<Exception>();
            Console.WriteLine(_providers.Count());

            foreach (var provider in _providers)
            {
                try
                {
                    if (await provider.SendEmailAsync(email))
                    {
                        user.EmailsSentToday++;
                        await _userRepository.UpdateUserAsync(user);
                        return "Email sent successfully";
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            // Log all exceptions
            foreach (var ex in exceptions)
            {
                Console.WriteLine($"Provider failed: {ex.Message}");
            }

            throw new Exception("All email providers failed", new AggregateException(exceptions));
        }
    }
}