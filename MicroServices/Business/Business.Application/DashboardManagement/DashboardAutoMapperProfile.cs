using AutoMapper;
using Business.MissionManagement.Dto;
using Business.Models;

namespace Business.DashboardManagement;

public class DashboardAutoMapperProfile : Profile
{
    public DashboardAutoMapperProfile()
    {
        CreateMap<MissionView, MissionViewDto>();
    }
}