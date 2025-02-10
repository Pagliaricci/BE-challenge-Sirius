namespace EmailService.Modules.Users.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }
        public required string Email { get; set; }
        public int EmailsSentToday { get; set; }
        public DateTime LastEmailReset { get; set; }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class CreateUser
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public required string Email { get; set; }
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
        public required string Email { get; set; }
    }

}