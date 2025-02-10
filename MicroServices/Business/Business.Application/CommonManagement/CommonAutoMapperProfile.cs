using AutoMapper;
using Business.MissionCategoryManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;

namespace Business.CommonManagement;

public class CommonAutoMapperProfile : Profile
{
    public CommonAutoMapperProfile()
    {
        CreateMap<MissionView, MissionViewDto>();
        CreateMap<MissionCategoryView, MissionCategoryViewDto>();
    }
}