using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain.Models.Interfaces;

namespace VolunteerReport.Domain.Models
{
    public class User : IODataEntity
    {
        public Guid Id { get; set; }
        // РОЛЬ? раньше была в айдентити
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
    }
}
