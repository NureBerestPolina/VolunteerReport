using EnRoute.Infrastructure.Commands;
using VolunteerReport.Domain.Models;
using VolunteerReport.Domain;

namespace EnRoute.Infrastructure.Services.Interfaces
{
    public interface IRoleStrategy
    {
        Task ExecuteRoleSpecificActionAsync(User user, RegisterCommand command, ApplicationDbContext dbContext);
    }
}
