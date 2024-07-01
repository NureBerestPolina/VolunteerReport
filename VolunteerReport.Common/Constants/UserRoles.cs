using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerReport.Common.Constants
{
    public static class UserRoles
    {
        public static readonly IEnumerable<string> AvailableRoles = new[] { Administrator, User, Volunteer };

        public const string Administrator = "admin";
        public const string User = "user";
        public const string Volunteer = "volunteer";
    }
}
