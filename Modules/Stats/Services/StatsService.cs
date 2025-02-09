using EmailService.Modules.Stats.Models;
using EmailService.Modules.Users.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService.Modules.Stats.Services
{
    public class StatsService : IStatsService
    {
        private readonly IUserRepository _userRepository;

        public StatsService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<Stat>> GetAllStatsAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var stats = new List<Stat>();
            foreach (var user in users)
            {
                if (DateTime.UtcNow.Date > user.LastEmailReset.Date){
                user.EmailsSentToday = 0;
                user.LastEmailReset = DateTime.UtcNow;
            }
                if (user.EmailsSentToday == 0)
                {
                    continue;
                }
            
                stats.Add(new Stat
                {
                    Username = user.Username,
                    EmailsSentToday = user.EmailsSentToday
                });
            }
            return stats;
        }
    }
}