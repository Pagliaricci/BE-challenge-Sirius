using EmailService.Data;
using EmailService.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService.Modules.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {id} not found.");
            }
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> CreateUserAsync(CreateUser user)
        {
            var savedUser = _context.Users.Add(new User
            {
                Username = user.Username,
                PasswordHash = user.Password,
                Role = user.Role,
                Email = user.Email,
                EmailsSentToday = 0,
                LastEmailReset = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return savedUser.Entity;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<User?> FindUserByUsernameAndPassword(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetUsersWithEmailsSentAsync()
        {
            return await _context.Users
            .Where(u => u.EmailsSentToday > 0)
            .ToListAsync();
        }

        public async Task<bool> UpdateUsersAsync(List<User> users)
        {
            _context.Users.UpdateRange(users);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}