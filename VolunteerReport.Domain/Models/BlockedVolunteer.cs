using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain.Models.Interfaces;

namespace VolunteerReport.Domain.Models
{
    public class BlockedVolunteer : IODataEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public Guid VolunteerId { get; set; }
    }
}
