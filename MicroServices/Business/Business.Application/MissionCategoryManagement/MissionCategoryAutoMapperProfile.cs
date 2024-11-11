using AutoMapper;
using Business.MissionCategoryManagement.Dto;
using Business.Models;

namespace Business.MissionCategoryManagement;

public class MissionCategoryAutoMapperProfile : Profile
{
    public MissionCategoryAutoMapperProfile()
    {

        CreateMap<CreateOrUpdateMissionCategoryDto, MissionCategory>();
        CreateMap<CreateOrUpdateMissionCategoryDto, MissionCategoryI18N>();
        CreateMap<CreateOrUpdateMissionCategoryDto, MissionCategoryI18Dto>();
        CreateMap<MissionCategoryView, MissionCategoryViewDto>();
    }
}