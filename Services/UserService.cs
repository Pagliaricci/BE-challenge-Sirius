using EmailService.Models;
using EmailService.Repositories;

namespace EmailService.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> CreateUserAsync(CreateUser user)
        {
            try
            {
                await CheckIfUserExists(user);
                CheckIfDataIsCorrect(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return await _userRepository.CreateUserAsync(user);
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
            var existingUser = await _userRepository.FindUserByUsernameAndPassword(user.Username, user.Password);
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
        }
    }
}
