using AutoMapper;
using Azure.Core;
using EnRoute.Infrastructure.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using VolunteerReport.API.Contracts.Auth.Requests;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Commands;
using VolunteerReport.Infrastructure.Services;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.API.Controllers
{
    public class VolunteersController : ODataControllerBase<Volunteer>
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly IVolunteerService volunteerService;
        private readonly IMapper mapper;

        public VolunteersController(ApplicationDbContext appDbContext, 
            IVolunteerService volunteerService,
             IMapper mapper) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
            this.volunteerService = volunteerService;
            this.mapper = mapper;
        }

        [HttpGet("VolunteerProfiles")]
        public async Task<IActionResult> GetVolunteerProfiles()
        {
            var volunteerProfiles = await volunteerService.GetVolunteerProfiles();

            return Ok(volunteerProfiles);
        }

        [HttpGet("VolunteerStatisticsProfile/{id}")]
        public async Task<IActionResult> GetVolunteerStatisticsProfile(Guid id)
        {
            var volunteerProfile = await volunteerService.GetVolunteerStatisticsProfile(id);

            if (volunteerProfile == null)
                return NotFound();

            return Ok(volunteerProfile);
        }

        [AllowAnonymous]
        [HttpGet("VolunteerSpendingsStatistics/{id}")]
        public async Task<IActionResult> GetVolunteerSpendingsStatistics(Guid id)
        {
            var spendings = await volunteerService.GetVolunteerSpendings(id);
            return Ok(spendings);
        }

        [HttpPut("Block/{id}")]
        public async Task<IActionResult> BlockVolunteer(Guid id)
        {
            if (appDbContext.Volunteers.Any(v => v.Id == id))
            {
                await volunteerService.BlockVolunteer(id);
                return Ok();
            }
            else {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPut("FillVolunteerProfile")]
        public async Task<IActionResult> FillInVolunteerProfile([FromBody] FillInProfileRequest profile)
        {
            var volunteer = await appDbContext.Volunteers.FirstOrDefaultAsync(v => v.UserId == profile.UserId);
            if (volunteer is not null)
            {
                var command = mapper.Map<FillInProfileCommand>(profile);
                command.VolunteerId = volunteer.Id;

                await volunteerService.FillInProfile(command);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPut("Unblock/{id}")]
        public async Task<IActionResult> UnblockVolunteer(Guid id)
        {
            if (appDbContext.BlockedVolunteers.Any(v => v.VolunteerId == id))
            {
                await volunteerService.UnblockVolunteer(id);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
