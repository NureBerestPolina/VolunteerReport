using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain.Models.Interfaces;

namespace VolunteerReport.Domain.Models
{
    public class Report : IODataEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Direction { get; set; }
        public Guid VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Modified { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public string PhotoUrl { get; set; }
    }
}
