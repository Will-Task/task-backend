using AutoMapper;
using Business.MissionManagement.Dto;
using Business.MissionTagManagement.Dto;
using Business.Models;

namespace Business.MissionTagManagement;

public class MissionTagAutoMapperProfile : Profile
{
    public MissionTagAutoMapperProfile()
    {
        CreateMap<CreateOrUpdateMissionTagDto, MissionTagI18N>();
        CreateMap<CreateOrUpdateMissionTagDto, MissionTagDto>();
        CreateMap<MissionView, MissionViewDto>();
    }
}