using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.API.Controllers
{
    public class AccusationsController : ODataControllerBase<Accusation>
    {
        private readonly IAccusationService accusationService;

        public AccusationsController(
            ApplicationDbContext appDbContext, 
            IAccusationService accusationService) : base(appDbContext)
        {
            this.accusationService = accusationService;
        }

        [AllowAnonymous]
        public override async Task<IActionResult> Post([FromBody] Accusation entity)
        {
            var isAllowed = await accusationService.IsAccusationAllowed(entity.UserId);

            if (!isAllowed) 
            {
                return BadRequest("Accusation is not allowed. More than 2 accusations in one day");
            }

            await accusationService.Accuse(entity.VolunteerId);
            return await base.Post(entity);
        }
    }
}
