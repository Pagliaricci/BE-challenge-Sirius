namespace EmailService.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }
        public required string Email { get; set; }
        public int EmailsSentToday { get; set; } // Cantidad de emails enviados hoy
        public DateTime LastEmailReset { get; set; } // Última vez que se reinició la cuota
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
}
