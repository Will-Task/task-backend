using System.Collections.Generic;
using System.Threading.Tasks;
using BaseService.Systems.TeamManagement.Dto;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.TeamManagement;

public interface ITeamAppService : IApplicationService
{
    Task<List<TeamViewDto>> GetAll();
}