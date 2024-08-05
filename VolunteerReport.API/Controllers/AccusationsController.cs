using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccusationsController : ODataControllerBase<Accusation>
    {
        private readonly IAccusationService accusationService;

        public AccusationsController(
            ApplicationDbContext appDbContext, 
            IAccusationService accusationService) : base(appDbContext)
        {
            this.accusationService = accusationService;
        }
    }
}
