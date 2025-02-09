namespace EmailService.Modules.Users.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
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
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
    }
}