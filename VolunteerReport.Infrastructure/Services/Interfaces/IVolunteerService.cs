using EnRoute.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Dtos;

namespace VolunteerReport.Infrastructure.Services.Interfaces
{
    public interface IVolunteerService
    {
        Task<IEnumerable<VolunteerProfile>> GetVolunteerProfiles();

        Task<VolunteerStatisticsProfile?> GetVolunteerStatisticsProfile(Guid volunteerId);
        Task<IEnumerable<CategoryCost>> GetVolunteerSpendings(Guid volunteerId);
    }
}
