using AutoMapper;
using BaseService.BaseData;
using BaseService.Systems.TeamManagement.Dto;
using Volo.Abp.Identity;

namespace BaseService.Systems.TeamManagement;

public class TeamAutoMapperProfile : Profile
{
    public TeamAutoMapperProfile()
    {
        CreateMap<TeamView, TeamViewDto>();
        CreateMap<CreateOrUpdateTeamDto, Team>();
        CreateMap<Team, TeamDto>();
        CreateMap<IdentityUser, MemberDto>();
    }
}