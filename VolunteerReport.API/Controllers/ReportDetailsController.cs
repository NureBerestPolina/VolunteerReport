using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerReport.Domain.Models;
using VolunteerReport.Domain;

namespace VolunteerReport.API.Controllers
{
    public class ReportDetailsController : ODataControllerBase<ReportDetail>
    {
        public ReportDetailsController(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
