using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
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
            var volunteers = await applicationDbContext.Volunteers.Where(v => !v.isBlocked).Include(v => v.User).ToListAsync();

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

        public async Task<VolunteerStatisticsProfile?> GetVolunteerStatisticsProfile(Guid volunteerId)
        {
            var volunteer = await applicationDbContext.Volunteers
                .Include(v => v.User).FirstOrDefaultAsync(v => v.Id == volunteerId);

            if (volunteer != null)
            {
                var reportsCount = await applicationDbContext.Reports
                    .CountAsync(r => r.VolunteerId == volunteerId && !r.IsDeleted);

                var totalCostUsd = await applicationDbContext.Reports
                    .Where(r => r.VolunteerId == volunteerId && !r.IsDeleted)
                    .SelectMany(r => r.ReportDetails)
                    .SumAsync(rd => rd.CostUsd);

                var volunteerStatisticsProfile = new VolunteerStatisticsProfile
                {
                    Id = volunteer.Id,
                    UserId = volunteer.UserId,
                    User = volunteer.User,
                    Nickname = volunteer.Nickname,
                    ShortInfo = volunteer.ShortInfo,
                    Modified = volunteer.Modified,
                    BankLink = volunteer.BankLink,
                    HelpInfo = volunteer.HelpInfo,
                    ReportsCount = reportsCount,
                    TotalCostUsd = totalCostUsd
                };

                return volunteerStatisticsProfile;
            }

            return null;
        }

        public async Task<IEnumerable<CategoryCost>> GetVolunteerSpendings(Guid volunteerId)
        {
            var spendings = await applicationDbContext.Reports
            .Where(r => r.VolunteerId == volunteerId && !r.IsDeleted)
            .SelectMany(r => r.ReportDetails)
            .GroupBy(rd => rd.Category.Name)
            .Select(g => new CategoryCost
            {
                CategoryName = g.Key,
                CostUsd = g.Sum(rd => rd.CostUsd)
            })
            .ToListAsync();

            return spendings;
        }

        public async Task BlockVolunteer(Guid id)
        {
            var volunteer = await applicationDbContext.Volunteers.Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (volunteer is not null)
            { 
                volunteer.isBlocked = true;
                applicationDbContext.BlockedVolunteers.Add(new BlockedVolunteer
                {
                    Email = volunteer.User.Email,
                    UserId = volunteer.UserId,
                    VolunteerId = volunteer.Id,
                });

                await applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
