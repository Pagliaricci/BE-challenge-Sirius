using EmailService.Modules.Email.Services;
using Microsoft.AspNetCore.Mvc;
using EmailService.Modules.Email.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> CreateEmail([FromBody] SendEmailRequest email)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is missing.");
            }

            var result = await _emailService.SendEmailAsync(email, int.Parse(userId));
            return Ok(result);
        }
    }
}