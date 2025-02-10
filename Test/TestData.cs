using EmailService.Modules.Email.Models;
using EmailService.Modules.Users.Models;
using System;
using System.Collections.Generic;
public static class TestData
{
    public static List<User> GetTestUsers() => new()
    {
        new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "admin",
            Role = "admin",
            Email = "admin@gmail.com",
            EmailsSentToday = 0,
            LastEmailReset = DateTime.UtcNow
        },
        new User
        {
            Id = 2,
            Username = "user",
            PasswordHash = "user",
            Role = "user",
            Email = "user@gmail.com",
            EmailsSentToday = 0,
            LastEmailReset = DateTime.UtcNow
        }
    };

    public static readonly SendEmailRequest EmailRequest1 = new()
    {
        To = "admin@gmail.com",
        Subject = "Test Subject",
        Body = "Test Body"
    };

}
