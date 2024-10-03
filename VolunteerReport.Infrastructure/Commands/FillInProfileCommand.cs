using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerReport.Infrastructure.Commands
{
    public class FillInProfileCommand
    {
        public Guid? VolunteerId { get; set; }
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public string ShortInfo { get; set; }
        public string BankLink { get; set; }
        public string HelpInfo { get; set; }
    }
}
