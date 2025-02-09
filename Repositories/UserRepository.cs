using EmailService.Data;
using EmailService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Repositories
{
    public class UserRepository
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
            var savedUser =_context.Users.Add(new User
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

        public async Task<User?> FindUserByUsernameAndPassword(string username, string passwordHash)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash);
        }
    }

}