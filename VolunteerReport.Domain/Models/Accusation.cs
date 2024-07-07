using EnRoute.Domain.Constants;
using VolunteerReport.Domain.Models.Interfaces;

namespace VolunteerReport.Domain.Models
{
    public class Accusation : IODataEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }
        public string ReasonDescription { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Modified { get; set; } = DateTime.UtcNow;
        public AccusationStatus Status { get; set; } = AccusationStatus.New;
    }
}
