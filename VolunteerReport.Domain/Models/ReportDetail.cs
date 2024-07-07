using VolunteerReport.Domain.Models.Interfaces;

namespace VolunteerReport.Domain.Models
{
    public class ReportDetail : IODataEntity
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public Report? Report { get; set; }
        public Guid CategoryId { get; set; }
        public ReportCategory? Category { get; set; }
        public double Amount { get; set; }
        public string MeasurementUnit { get; set; }
        public decimal CostUsd { get; set; }
    }
}
