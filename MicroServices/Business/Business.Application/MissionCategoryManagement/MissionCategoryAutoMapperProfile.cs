using AutoMapper;
using Business.MissionCategoryManagement.Dto;
using Business.Models;

namespace Business.MissionCategoryManagement;

public class MissionCategoryAutoMapperProfile : Profile
{
    public MissionCategoryAutoMapperProfile()
    {
        CreateMap<MissionCategory, MissionCategoryDto>();
        CreateMap<MissionCategoryI18N, MissionCategoryI18Dto>();
        CreateMap<CreateOrUpodateMissionCategoryDto, MissionCategory>();
        CreateMap<CreateOrUpodateMissionCategoryDto, MissionCategoryI18Dto>();
        CreateMap<MissionCategoryView, MissionCategoryViewDto>();
    }
}