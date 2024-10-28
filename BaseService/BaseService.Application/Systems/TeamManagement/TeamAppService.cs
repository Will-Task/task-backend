using System;
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

    /// <summary>
    /// 獲取當前使用者所在的所有群組資訊
    /// </summary>
    public async Task<List<TeamViewDto>> GetAll()
    {
        var userId = CurrentUser.Id;
        var teams = await _repository.GetListAsync(t => t.UserId == userId);
        return ObjectMapper.Map<List<TeamView>, List<TeamViewDto>>(teams);
    }

    /// <summary>
    /// 獲取某團隊成員資訊
    /// </summary>
    /// <param name="id">Team Id</param>
    public async Task<List<TeamViewDto>> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 團隊建立或資訊修改
    /// </summary>
    public async Task DataPost(CreateOrUpdateTeamDto input)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 團隊刪除
    /// </summary>
    /// <param name="id">Team id</param>
    public async Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 邀請人進入團隊
    /// </summary>
    /// <param name="name">邀請人姓名</param>
    /// <param name="id">要被邀請到的團隊 Id</param>
    public async Task Invite(string name, Guid id)
    {
        throw new NotImplementedException();
    }
}