using System.Collections.Generic;
using System.Threading.Tasks;
using BaseService.Controllers;
using BaseService.Systems.TeamManagement;
using BaseService.Systems.TeamManagement.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BaseService.HttpApi.Systems;

[Area("Team")]
[Route("api/base/team")]
public class TeamController : BaseServiceController , ITeamAppService
{
    private readonly ITeamAppService _teamAppService;

    public TeamController(ITeamAppService teamAppService)
    {
        _teamAppService = teamAppService;
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<List<TeamViewDto>> GetAll()
    {
        return await _teamAppService.GetAll();
    }
}