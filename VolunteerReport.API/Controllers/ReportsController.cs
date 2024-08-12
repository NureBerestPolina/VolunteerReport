using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.API.Controllers
{
    public class ReportsController : ODataControllerBase<Report>
    {
        public ReportsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
