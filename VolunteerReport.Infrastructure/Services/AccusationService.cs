using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.Infrastructure.Services
{
    public class AccusationService : IAccusationService
    {
        private readonly ApplicationDbContext applicationDbContext;

        public AccusationService(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<bool> IsAccusationAllowed(Guid userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var accusationsCount = await applicationDbContext.Accusations
                .Where(a => a.UserId == userId && a.Created >= today && a.Created < tomorrow)
                .CountAsync();

            return accusationsCount < 2;
        }

        public async Task Accuse(Guid volunteerId)
        {
            var accusedVolunteer = await applicationDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteerId);
            if (accusedVolunteer != null)
            {
                accusedVolunteer.isHidden = true;
                await applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
