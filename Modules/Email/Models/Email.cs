
namespace EmailService.Modules.Email.Models
{

    public class SendEmailRequest
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}