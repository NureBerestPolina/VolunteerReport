using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain;
using VolunteerReport.Infrastructure.Dtos;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.Infrastructure.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly ApplicationDbContext applicationDbContext;

        public VolunteerService(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<IEnumerable<VolunteerProfile>> GetVolunteerProfiles()
        {
            var volunteers = await applicationDbContext.Volunteers.Where(v => !v.isBlocked && !v.isHidden).Include(v => v.User).ToListAsync();

            var volunteerProfiles = new List<VolunteerProfile>();

            foreach (var volunteer in volunteers)
            {
                var reports = await applicationDbContext.Reports
                    .Where(r => r.VolunteerId == volunteer.Id && !r.IsDeleted)
                    .Include(r => r.ReportDetails)
                    .ThenInclude(rd => rd.Category)
                    .ToListAsync();

                var helpCategories = reports
                    .SelectMany(r => r.ReportDetails)
                    .Where(rd => rd.Category != null)
                    .Select(rd => rd.Category)
                    .Distinct()
                    .ToList();

                var volunteerProfile = new VolunteerProfile
                {
                    Id = volunteer.Id,
                    UserId = volunteer.UserId,
                    User = volunteer.User,
                    Nickname = volunteer.Nickname,
                    ShortInfo = volunteer.ShortInfo,
                    Modified = volunteer.Modified,
                    BankLink = volunteer.BankLink,
                    HelpInfo = volunteer.HelpInfo,
                    HelpCategories = helpCategories
                };

                volunteerProfiles.Add(volunteerProfile);
            }

            return volunteerProfiles;
        }
    }
}
