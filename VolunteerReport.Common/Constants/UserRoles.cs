namespace VolunteerReport.Common.Constants
{
    public static class UserRoles
    {
        public static readonly IEnumerable<string> AvailableRoles = new[] { Administrator, User, Volunteer };

        public const string Administrator = "administrator";
        public const string User = "user";
        public const string Volunteer = "volunteer";
    }
}
