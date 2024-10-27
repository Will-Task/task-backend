using Business.TeamManagement;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area(("Team"))]
[Route("api/business/team")]
public class TeamController : AbpController
{
    private readonly ITeamAppService _teamAppService;

    public TeamController(ITeamAppService teamAppService)
    {
        _teamAppService = teamAppService;
    }
}