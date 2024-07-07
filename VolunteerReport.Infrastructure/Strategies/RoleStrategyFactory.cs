using EnRoute.Infrastructure.Services.Interfaces;
using VolunteerReport.Common.Constants;

namespace EnRoute.Infrastructure.Strategies
{
    public class RoleStrategyFactory : IRoleStrategyFactory
    {
        public IRoleStrategy CreateStrategy(string role)
        {
            switch (role.ToLower())
            {
                case UserRoles.Volunteer:
                    return new VolunteerStrategy();
                case UserRoles.User:
                    return new UserStrategy();
                case UserRoles.Administrator:
                    return new AdministratorStrategy();
                default:
                    throw new ArgumentException($"Role {role} is not supported.", nameof(role));
            }
        }
    }
}
