using EmailService.Modules.Users.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService.Modules.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(CreateUser user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<User?> FindUserByUsernameAndPassword(string username, string passwordHash);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetUsersWithEmailsSentAsync();
        Task<bool> UpdateUsersAsync(List<User> users);

    }
}