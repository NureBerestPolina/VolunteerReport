using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerReport.API.Contracts.Reports.Requests;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services;
using VolunteerReport.Infrastructure.Services.Interfaces;

namespace VolunteerReport.API.Controllers
{
    public class ReportsController : ODataControllerBase<Report>
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly IWebHostEnvironment _env;

        public ReportsController(ApplicationDbContext appDbContext, IWebHostEnvironment env) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
            _env = env;
        }

        [HttpPost("Reports/UploadPhoto")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                string uploadsFolder = Path.Combine(_env.ContentRootPath, "report_photos");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

               return Ok(new { url = filePath.Replace(@"\\", "/") });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Reports/MakeReport")]
        public async Task<IActionResult> MakeReport([FromBody] MakeReportRequest request)
        {
            var volunteer = await appDbContext.Volunteers.FirstOrDefaultAsync(v => v.UserId == request.UserId);

            if (volunteer != null)
            {
                var newReport = new Report
                {
                    Description = request.Description,
                    Direction = request.Direction,
                    VolunteerId = volunteer.Id,
                    PhotoUrl = request.PhotoUrl,
                };

                await appDbContext.Reports.AddAsync(newReport);
                await appDbContext.SaveChangesAsync();

                return Ok(new { Id = newReport.Id });
            }

            return NotFound();
        }
    }
}
