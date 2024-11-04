using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseService.BaseData;
using BaseService.Systems.TeamManagement.Dto;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace BaseService.Systems.TeamManagement;

[RemoteService(false)]
public class TeamAppService : ApplicationService, ITeamAppService
{
    private (
        IRepository<Team, Guid> Team,
        IRepository<TeamView> TeamView,
        IRepository<TeamMission> TeamMission
        ) _repositorys;

    protected IIdentityUserRepository _userRepository { get; }

    public TeamAppService(
        IRepository<Team, Guid> Team,
        IRepository<TeamView> TeamView,
        IRepository<TeamMission> TeamMission,
        IIdentityUserRepository userRepository)
    {
        _repositorys = (Team, TeamView, TeamMission);
        _userRepository = userRepository;
    }

    /// <summary>
    /// 獲取當前使用者所在的所有群組資訊
    /// </summary>
    public async Task<List<TeamDto>> GetAll(string name)
    {
        var userId = CurrentUser.Id;
        var teamMissionQuery = await _repositorys.TeamMission.GetQueryableAsync();
        // 取得當前所在所有TeamId
        var teamIds = await teamMissionQuery.Where(x => x.UserId == userId).Select(x => x.TeamId).ToListAsync();
        var teamQuery = await _repositorys.Team.GetQueryableAsync();
        var teams = await teamQuery.Where(x => teamIds.Contains(x.Id))
            .WhereIf(!name.IsNullOrEmpty(), x => x.Name.Contains(name)).ToListAsync();
        return ObjectMapper.Map<List<Team>, List<TeamDto>>(teams);
    }

    /// <summary>
    /// 獲取某團隊成員資訊
    /// </summary>
    /// <param name="id">Team Id</param>
    public async Task<List<MemberDto>> GetMembers(Guid? id, string name)
    {
        var teamMissionQuery = await _repositorys.TeamMission.GetQueryableAsync();
        var userIds = await teamMissionQuery.WhereIf(id.HasValue, x => x.TeamId == id.Value)
            .Select(x => x.UserId).ToListAsync();
        var users = await _userRepository.GetListByIdsAsync(userIds);
        users = users.WhereIf(!name.IsNullOrEmpty(), t => t.UserName.Contains(name)).ToList();
        return ObjectMapper.Map<List<IdentityUser>, List<MemberDto>>(users);
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
    public async Task Invite(InviteFormData formData)
    {
        foreach (var userId in formData.UserIds)
        {
            await _repositorys.TeamMission.InsertAsync(new TeamMission
            {
                UserId = userId,
                TeamId = formData.TeamId
            });
        }
    }

    public async Task Drop(DropFormData formData)
    {
        await _repositorys.TeamMission.DeleteAsync(t => t.TeamId == formData.TeamId && t.UserId == formData.UserId);
    }
}