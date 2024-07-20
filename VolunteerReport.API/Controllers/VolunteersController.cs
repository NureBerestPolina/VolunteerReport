using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.API.Controllers
{
    public class VolunteersController : ODataControllerBase<Volunteer>
    {
        private readonly IVolunteerService volunteerService;

        public VolunteersController(ApplicationDbContext appDbContext, IVolunteerService volunteerService) : base(appDbContext)
        {
            this.volunteerService = volunteerService;
        }

        [HttpGet("VolunteerProfiles")]
        public async Task<IActionResult> GetAdminStatistics()
        {
            var volunteerProfiles = await volunteerService.GetVolunteerProfiles();

            return Ok(volunteerProfiles);
        }
    }
}
