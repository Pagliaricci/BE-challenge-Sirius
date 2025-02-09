using Microsoft.AspNetCore.Mvc;
using EmailService.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmailService.Controllers
{
   [ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) 
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest login)
    {
        var token = await _authService.AuthenticateAsync(login.Username, login.Password);
        if (token == null)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        return Ok(new { token });
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult ProtectedEndpoint()
    {
        return Ok(new { message = "¡Has accedido a un endpoint protegido!" });
    }
}
}
