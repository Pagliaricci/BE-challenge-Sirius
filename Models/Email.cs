
namespace EmailService.Models
{

    public class SendEmailRequest
    {
        public required string SenderEmail { get; set; }
        public required string SenderName { get; set; }
        public required string RecipientEmail { get; set; }
        public required string RecipientName { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}