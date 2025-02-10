using System.ComponentModel.DataAnnotations;

namespace EmailService.Modules.Email.Models
{
    public class SendEmailRequest
    {
        [Required, EmailAddress]
        public required string To { get; set; }

        [Required, MinLength(3)]
        public required string Subject { get; set; }

        [Required, MinLength(5)]
        public required string Body { get; set; }
    }
}