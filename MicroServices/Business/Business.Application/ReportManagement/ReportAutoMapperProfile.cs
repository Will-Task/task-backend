using AutoMapper;
using Business.MissionCategoryManagement.Dto;
using Business.Models;

namespace Business.ReportManagement;

public class ReportAutoMapperProfile : Profile
{
    public ReportAutoMapperProfile()
    {
        CreateMap<MissionOverAllView, MissionOverAllView>();
        CreateMap<MissionCategoryView, MissionCategoryViewDto>();
    }
}