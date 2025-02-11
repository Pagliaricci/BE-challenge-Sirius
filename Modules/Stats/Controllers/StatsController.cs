using Microsoft.AspNetCore.Mvc;
using EmailService.Modules.Stats.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EmailService.Modules.Stats.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService.Modules.Stats.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }
        
        /// <summary>
        /// Returns all user stats.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Stat>>> GetAllStats()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized("Invalid token: missing role");
            }
            if (role != "Admin")
            {
                return Forbid();
            }
            var stats = await _statsService.GetAllStatsAsync();
            return Ok(stats);
        }
    }
}