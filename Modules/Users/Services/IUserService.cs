using System.Collections.Generic;
using System.Threading.Tasks;
using EmailService.Modules.Users.Models;


namespace EmailService.Modules.Users.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<User> CreateUserAsync(CreateUser user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    }
}