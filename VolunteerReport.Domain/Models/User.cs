using EnRoute.Domain.Models;
using VolunteerReport.Domain.Models.Interfaces;

namespace VolunteerReport.Domain.Models
{
    public class User : IODataEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
        public List<IssuedToken> IssuedTokens { get; set; } = new();
    }
}
