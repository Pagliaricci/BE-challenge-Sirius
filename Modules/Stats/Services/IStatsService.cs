using EmailService.Modules.Stats.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService.Modules.Stats.Services
{
    public interface IStatsService
    {
        Task<List<Stat>> GetAllStatsAsync();
    }
}