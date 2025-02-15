using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shipwreck.Phash;
using System.Drawing;
using VolunteerReport.API.Contracts.Reports.Requests;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services;
using VolunteerReport.Infrastructure.Services.Interfaces;
using Shipwreck.Phash.Bitmaps;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace VolunteerReport.API.Controllers
{
    public class ReportsController : ODataControllerBase<Report>
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IPhotoPlagiarismService plagiarizmService;

        public ReportsController(ApplicationDbContext appDbContext, IWebHostEnvironment env, IPhotoPlagiarismService plagiarizmService) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
            _env = env;
            this.plagiarizmService = plagiarizmService;
        }

        [HttpPost("Reports/UploadPhoto")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (await isPhotoPlagiarizedAsync(file))
            {
                return BadRequest("Plagiarized photo uploaded.");
            }

            try
            {
                string uploadsFolder = Path.Combine(_env.ContentRootPath, "report_photos");
                string uploadsFolderToStoreInDb = "http://localhost:5105/report_photos/";

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                filePath = Path.Combine(uploadsFolderToStoreInDb, uniqueFileName);

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

        private async Task<bool> isPhotoPlagiarizedAsync(IFormFile file)
        {
            string tempFilePath = Path.Combine(_env.ContentRootPath, "temp", Guid.NewGuid() + Path.GetExtension(file.FileName));
            Directory.CreateDirectory(Path.GetDirectoryName(tempFilePath));

            try
            {
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                string uploadsFolder = Path.Combine(_env.ContentRootPath, "report_photos");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                return await plagiarizmService.IsPhotoPlagiarizedInDirectoryAsync(tempFilePath, uploadsFolder);

                using (var uploadedBitmap = (Bitmap)System.Drawing.Image.FromFile(tempFilePath))
                {
                    // Convert Bitmap to IByteImage using ToLuminanceImage
                    var uploadedImage = uploadedBitmap.ToLuminanceImage();

                    // Compute pHash
                    var uploadedHash = ImagePhash.ComputeDigest(uploadedImage);

                    var storedPhotoPaths = appDbContext.Reports
                        .Select(r => r.PhotoUrl)
                        .Where(url => !string.IsNullOrEmpty(url))
                        .ToList();

                    foreach (var photoPath in storedPhotoPaths)
                    {
                        string fullPath = Path.Combine(_env.ContentRootPath, photoPath);

                        if (System.IO.File.Exists(fullPath))
                        {
                            using (var storedBitmap = (Bitmap)System.Drawing.Image.FromFile(fullPath))
                            {
                                var storedImage = storedBitmap.ToLuminanceImage();
                                var storedHash = ImagePhash.ComputeDigest(storedImage);

                                var score = ImagePhash.GetCrossCorrelation(uploadedHash, storedHash);

                                if (score > 0.9) // Adjust similarity threshold as needed
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception e)
            { 

            }

            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }

            return false;
        }
    }
}
