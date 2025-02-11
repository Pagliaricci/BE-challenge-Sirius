using EmailService.Modules.Email.Models;
using EmailService.Modules.Users.Repositories;
using EmailService.Modules.Email.EmailProviders;


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
            var sender = await _userRepository.GetUserByIdAsync(userId) ?? throw new Exception("Sender or recipient not found");
            if (DateTime.UtcNow.Date > sender.LastEmailReset.Date)
            {
                sender.EmailsSentToday = 0;
                sender.LastEmailReset = DateTime.UtcNow;
            }

            if (sender.EmailsSentToday >= 1000)
            {
                throw new Exception("User has reached the daily limit of emails");
            }


            foreach (var provider in _providers)
            {
                try
                {
                    var result = await provider.SendEmailAsync(email, sender);
                    if (result)
                    {
                        sender.EmailsSentToday++;
                        await _userRepository.UpdateUserAsync(sender);
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