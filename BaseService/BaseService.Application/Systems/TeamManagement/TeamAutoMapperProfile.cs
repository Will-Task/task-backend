using AutoMapper;
using BaseService.BaseData;
using BaseService.Systems.TeamManagement.Dto;

namespace BaseService.Systems.TeamManagement;

public class TeamAutoMapperProfile: Profile
{
    public TeamAutoMapperProfile()
    {
        CreateMap<TeamView, TeamViewDto>();
    }
}