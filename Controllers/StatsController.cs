using Microsoft.AspNetCore.Mvc;
using EmailService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EmailService.Services;


namespace EmailService.Controllers
{
    
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly StatsService _statsService;

        public StatsController(StatsService statsService)
        {
            _statsService = statsService;

        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Stats>>> GetAllStats()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            System.Console.WriteLine($"Rol: {role}");
            if (role != "Admin")
            {
                return Forbid();
            }
            return await _statsService.GetAllStatsAsync();
        }
    }
}