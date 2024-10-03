using AutoMapper;
using VolunteerReport.API.Contracts.Auth.Requests;
using VolunteerReport.Infrastructure.Commands;

namespace VolunteerReport.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<RegisterRequest, RegisterCommand>();
            CreateMap<FillInProfileRequest, FillInProfileCommand>();
            // CreateMap<RegisterCompanyRequest, RegisterCommand>();
            // CreateMap<GetStatisticsDto, GetStatisticsResponse>();
        }
    }
}
