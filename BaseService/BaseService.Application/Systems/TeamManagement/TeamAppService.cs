using System.Collections.Generic;
using System.Threading.Tasks;
using BaseService.BaseData;
using BaseService.Systems.TeamManagement.Dto;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace BaseService.Systems.TeamManagement;

[RemoteService(false)]
public class TeamAppService : ApplicationService, ITeamAppService
{
    private readonly IRepository<TeamView> _repository;

    public TeamAppService(IRepository<TeamView> repository)
    {
        _repository = repository;
    }

    public async Task<List<TeamViewDto>> GetAll()
    {
        var userId = CurrentUser.Id;
        var teams = await _repository.GetListAsync();
        return ObjectMapper.Map<List<TeamView>, List<TeamViewDto>>(teams);
    }
}