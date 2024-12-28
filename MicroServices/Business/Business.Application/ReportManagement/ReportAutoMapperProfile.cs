using AutoMapper;
using Business.MissionCategoryManagement.Dto;
using Business.Models;
using Business.ReportManagement.Dto;

namespace Business.ReportManagement;

public class ReportAutoMapperProfile : Profile
{
    public ReportAutoMapperProfile()
    {
        CreateMap<MissionOverAllView, MissionOverAllViewDto>();
        CreateMap<MissionOverAllViewDto, ExportMissionOverAllViewDto>();
        CreateMap<MissionCategoryView, MissionCategoryViewDto>();
    }
}