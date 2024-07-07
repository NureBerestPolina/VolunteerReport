using EnRoute.Infrastructure.Commands;
using EnRoute.Infrastructure.Services.Interfaces;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;

namespace EnRoute.Infrastructure.Strategies
{
    public class AdministratorStrategy : IRoleStrategy
    {
        public Task ExecuteRoleSpecificActionAsync(User user, RegisterCommand command, ApplicationDbContext dbContext)
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
