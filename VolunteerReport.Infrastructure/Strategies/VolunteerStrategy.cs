using EnRoute.Infrastructure.Commands;
using EnRoute.Infrastructure.Services.Interfaces;
using VolunteerReport.Domain.Models;
using VolunteerReport.Domain;

namespace EnRoute.Infrastructure.Strategies
{
    public class VolunteerStrategy : IRoleStrategy
    {
        public async Task ExecuteRoleSpecificActionAsync(User user, RegisterCommand command, ApplicationDbContext dbContext)
        {
            var volunteer = new Volunteer
            {
                UserId = user.Id
            };
            await dbContext.Volunteers.AddAsync(volunteer);
            await dbContext.SaveChangesAsync();
        }
    }
}
