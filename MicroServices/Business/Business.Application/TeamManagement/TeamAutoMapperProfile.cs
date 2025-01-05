using AutoMapper;
using Business.CommonManagement.Dto;
using Business.Models;
using Business.TeamManagement.Dto;

namespace Business.TeamManagement
{
    public class TeamAutoMapperProfile : Profile
    {
        public TeamAutoMapperProfile()
        {
            CreateMap<Team, TeamDto>();
            CreateMap<CreateOrUpdateTeamDto, Team>();
            CreateMap<TeamInvitationDto, ExportTeamInvitationDto>();
            CreateMap<TeamInvitation, TeamInvitationDto>();
            CreateMap<CreateOrUpdateTeamInvitationDto, TeamInvitation>();
            CreateMap<AbpUserView, AbpUserViewDto>();
        }
    }
}
