using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain.Models;

namespace VolunteerReport.Infrastructure.Dtos
{
    public class VolunteerProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string? Nickname { get; set; }
        public string? ShortInfo { get; set; }
        public DateTime? Modified { get; set; } = DateTime.UtcNow;
        public string? BankLink { get; set; }
        public string? HelpInfo { get; set; }
        public List<ReportCategory> HelpCategories { get; set; } = new List<ReportCategory>();
    }
}
