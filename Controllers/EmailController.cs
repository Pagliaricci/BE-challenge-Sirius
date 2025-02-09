using EmailService.Services;
using Microsoft.AspNetCore.Mvc;
using EmailService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace EmailService.Controllers
{
    
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly EmailsService _emailService;

        public EmailController(EmailsService emailService)
        {
            _emailService = emailService;

        }

       [HttpPost]
[Authorize]
public async Task<ActionResult<string>> CreateEmail([FromBody] SendEmailRequest email)
{
    var username = User.FindFirst(ClaimTypes.Name)?.Value;
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized(new { message = "No se pudo obtener el ID del usuario" });
    }

    Console.WriteLine($"Usuario autenticado: {username}, Rol: {role}, ID: {userId}");

    var newEmail = await _emailService.SendEmailAsync(email, int.Parse(userId));
    return CreatedAtAction("CreateEmail", newEmail);
}

    }

}