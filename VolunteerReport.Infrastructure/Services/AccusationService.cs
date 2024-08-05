using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.Infrastructure.Services
{
    public class AccusationService : IAccusationService
    {
        public Task<bool> IsAccusationAllowed(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
