using VolunteerReport.Domain.Models;

namespace VolunteerReport.API.Contracts.Auth.Requests
{
    public class FillInProfileRequest
    { 
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public string ShortInfo { get; set; }
        public string BankLink { get; set; }
        public string HelpInfo { get; set; }
    }
}
