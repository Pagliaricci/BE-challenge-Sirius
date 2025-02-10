using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using System.Text.RegularExpressions;

namespace EmailService.Modules.Users.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return [.. users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                Email = u.Email
            })];
        }


        public async Task<User> CreateUserAsync(CreateUser user)
        {
            await CheckIfUserExists(user);
            CheckIfDataIsCorrect(user);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            return await _userRepository.CreateUserAsync(new CreateUser
            {
                Username = user.Username,
                Password = hashedPassword,
                Role = user.Role,
                Email = user.Email
            });
        }


        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }
        public async Task CheckIfUserExists(CreateUser user)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists.");
            }

        }


        public void CheckIfDataIsCorrect(CreateUser user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Role))
            {
                throw new Exception("Invalid data.");
            }
            if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new Exception("Invalid email format.");
            }
            if (user.Password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters long.");
            }
        }

    }
}
