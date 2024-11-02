using AutoMapper;
using BaseService.BaseData;
using BaseService.Systems.TeamManagement.Dto;

namespace BaseService.Systems.TeamManagement;

public class TeamAutoMapperProfile : Profile
{
    public TeamAutoMapperProfile()
    {
        CreateMap<TeamView, TeamViewDto>();
        CreateMap<CreateOrUpdateTeamDto, Team>();
        CreateMap<Team, TeamDto>();
        CreateMap<TeamView, MemberDto>();
    }
}