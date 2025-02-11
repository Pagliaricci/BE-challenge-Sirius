using EmailService.Modules.Email.Services;
using Microsoft.AspNetCore.Mvc;
using EmailService.Modules.Email.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EmailService.Modules.Email.Controllers
{

    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Lets a user send an email.
        /// </summary>
        /// <param name="email">Should include the recipient email, a subject and an email body.</param>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> CreateEmail([FromBody] SendEmailRequest email)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out _))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _emailService.SendEmailAsync(email, int.Parse(userId));
            return Ok(result);
        }
    }
}