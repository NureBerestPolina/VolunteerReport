using System;

namespace VolunteerReport.API.Contracts.Reports.Requests
{
    public class MakeReportRequest
    {
        public string Description { get; set; }
        public string Direction { get; set; }
        public Guid UserId { get; set; }
        public string PhotoUrl { get; set; }
    }
}
