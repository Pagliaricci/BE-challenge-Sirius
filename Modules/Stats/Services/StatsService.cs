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
        var users = await GetUsersWithEmailsSentAsync();
        var (updatedUsers, changesMade) = ResetUserQuotasIfNecessary(users);
        if (changesMade){
            await UpdateUserDataAsync(updatedUsers);
            }
            var filteredUsers = FilterUsersWithEmailsSentToday(updatedUsers);
            return CreateStats(filteredUsers);
            }

private async Task<List<Users.Models.User>> GetUsersWithEmailsSentAsync()
{
    return await _userRepository.GetUsersWithEmailsSentAsync();
}

private static (List<Users.Models.User> updatedUsers, bool changesMade) ResetUserQuotasIfNecessary(List<Users.Models.User> users)
{
        if (users == null)
    {
        return (new List<Users.Models.User>(), false);
    }
    bool changesMade = false;
    var updatedUsers = new List<Users.Models.User>();

    foreach (var user in users)
    {
        if (DateTime.UtcNow.Date > user.LastEmailReset.Date)
        {
            user.EmailsSentToday = 0;
            user.LastEmailReset = DateTime.UtcNow;
            changesMade = true;
        }

        updatedUsers.Add(user);
    }

    return (updatedUsers, changesMade);
}


private async Task UpdateUserDataAsync(List<Users.Models.User> users)
{
    await _userRepository.UpdateUsersAsync(users);
}

private static List<Users.Models.User> FilterUsersWithEmailsSentToday(List<Users.Models.User> users)
{
    return [.. users.Where(u => u.EmailsSentToday > 0)];
}

private static List<Stat> CreateStats(List<Users.Models.User> filteredUsers)
{
    return [.. filteredUsers.Select(user => new Stat
    {
        Username = user.Username,
        EmailsSentToday = user.EmailsSentToday
    })];
}
    }
}