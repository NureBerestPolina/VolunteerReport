using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerReport.Infrastructure.Services.Interfaces
{
    public interface IPhotoPlagiarismService
    {
        Task<bool> IsPhotoPlagiarizedAsync(string uploadedFilePath, string storedFilePath);
        Task<bool> IsPhotoPlagiarizedInDirectoryAsync(string uploadedFilePath, string storedDirectoryPath);
    }
}
