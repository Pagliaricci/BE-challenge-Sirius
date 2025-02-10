namespace EmailService.Modules.Stats.Models
{
    public class Stat
    {
        public required string Username { get; set; }
        public int EmailsSentToday { get; set; }
    }
}