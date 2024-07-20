using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;

namespace VolunteerReport.API.Controllers
{
    public class VolunteersController : ODataControllerBase<Volunteer>
    {
        public VolunteersController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
