using EmailService.Models;
using EmailService.Repositories;
using EmailProviders;

namespace EmailService.Services
{
    public class StatsService
    {
        private readonly UserRepository _userRepository;

        public StatsService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<Stats>> GetAllStatsAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var stats = new List<Stats>();
            foreach (var user in users)
            {
                if (user.EmailsSentToday == 0)
                {
                    continue;
                }
            
                stats.Add(new Stats
                {
                    Username = user.Username,
                    EmailsSentToday = user.EmailsSentToday
                });
            }
            return stats;
        }
    }
}
