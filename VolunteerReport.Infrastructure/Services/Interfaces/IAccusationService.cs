using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Domain.Models;

namespace VolunteerReport.Infrastructure.Services.Interfaces
{
    public interface IAccusationService
    {
        Task<bool> IsAccusationAllowed(Guid userId);
    }
}
