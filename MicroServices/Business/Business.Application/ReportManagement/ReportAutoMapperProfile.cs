using AutoMapper;
using Business.Models;

namespace Business.ReportManagement;

public class ReportAutoMapperProfile : Profile
{
    public ReportAutoMapperProfile()
    {
        CreateMap<MissionOverAllView, MissionOverAllView>();
    }
}