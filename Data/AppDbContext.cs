using EmailService.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }  // This must exist
    }
}
