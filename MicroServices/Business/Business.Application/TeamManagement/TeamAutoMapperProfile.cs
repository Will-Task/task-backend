using AutoMapper;
using Business.Models;
using Business.TeamManagement.Dto;

namespace Business.TeamManagement
{
    public class TeamAutoMapperProfile : Profile
    {
        public TeamAutoMapperProfile()
        {
            CreateMap<TeamView, TeamViewDto>();
            CreateMap<CreateOrUpdateTeamDto, Team>();
            CreateMap<Team, TeamDto>();
            //CreateMap<IdentityUser, MemberDto>();
            CreateMap<CreateOrUpdateTeamInvitationDto, TeamInvitation>();
            CreateMap<TeamInvitation, TeamInvitationDto>();
            CreateMap<TeamInvitationView, TeamInvitationViewDto>();
        }
    }
}
