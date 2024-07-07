using VolunteerReport.Domain.Models.Interfaces;

namespace VolunteerReport.Domain.Models
{
    public class Volunteer : IODataEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string? Nickname { get; set; }
        public string? ShortInfo { get; set; }
        public DateTime? Modified { get; set; } = DateTime.UtcNow;
        public bool isBlocked { get; set; } = false;
        public bool isHidden { get; set; } = false;
        public string? BankLink { get; set; }
        public string? HelpInfo { get; set;}
    }
}
