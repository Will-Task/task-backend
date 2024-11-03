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
    private (
        IRepository<Team, Guid> Team,
        IRepository<TeamView> TeamView,
        IRepository<TeamMission> TeamMission
        ) _repositorys;

    public TeamAppService(
        IRepository<Team, Guid> Team,
        IRepository<TeamView> TeamView, 
        IRepository<TeamMission> TeamMission)
    {
        _repositorys = (Team, TeamView , TeamMission);
    }

    /// <summary>
    /// 獲取當前使用者所在的所有群組資訊
    /// </summary>
    public async Task<List<TeamViewDto>> GetAll()
    {
        var userId = CurrentUser.Id;
        var teams = await _repositorys.TeamView.GetListAsync(t => t.UserId == userId);
        return ObjectMapper.Map<List<TeamView>, List<TeamViewDto>>(teams);
    }

    /// <summary>
    /// 獲取某團隊成員資訊
    /// </summary>
    /// <param name="id">Team Id</param>
    public async Task<List<MemberDto>> GetMembers(Guid id)
    {
        var teams = await _repositorys.TeamView.GetListAsync(t => t.TeamId == id);
        return ObjectMapper.Map<List<TeamView>, List<MemberDto>>(teams);
    }

    /// <summary>
    /// 團隊建立或資訊修改
    /// </summary>
    public async Task<TeamDto> DataPost(CreateOrUpdateTeamDto input)
    {
        Team result = null;
        // For 修改
        if (input.Id.HasValue)
        {
            var team = await _repositorys.Team.GetAsync(input.Id.Value);
            result = ObjectMapper.Map(input, team);
        }
        // For 新增
        else
        {
            result = await _repositorys.Team.InsertAsync(ObjectMapper.Map<CreateOrUpdateTeamDto, Team>(input));
        }

        return ObjectMapper.Map<Team, TeamDto>(result);
    }

    /// <summary>
    /// 團隊刪除
    /// </summary>
    /// <param name="id">Team id</param>
    public async Task Delete(Guid id)
    {
        await _repositorys.Team.DeleteAsync(id);
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

    public async Task Drop(DropFormData formData)
    {
        await _repositorys.TeamMission.DeleteAsync(t => t.TeamId == formData.TeamId && t.UserId == formData.UserId);
    }
}