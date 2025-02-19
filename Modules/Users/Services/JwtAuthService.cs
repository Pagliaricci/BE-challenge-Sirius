using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using EmailService.Data; 

namespace EmailService.Modules.Users.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.FindUserByUsernameAndPassword(username, password);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return null;
            }

            return GenerateToken(user);
        }
        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
            }
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User? GetUser(string username, string password)
        {
            return _userRepository.FindUserByUsernameAndPassword(username, password).Result;
        }
    }
}
