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
    public async Task<List<TeamViewDto>> GetAll(string name)
    {
        var userId = CurrentUser.Id;
        var query = await _repositorys.TeamView.GetQueryableAsync();
        var teams = await query.Where(t => t.UserId == userId)
            .WhereIf(!name.IsNullOrEmpty(), t => t.Name.Contains(name)).ToListAsync();
        return ObjectMapper.Map<List<TeamView>, List<TeamViewDto>>(teams);
    }

    /// <summary>
    /// 獲取某團隊成員資訊
    /// </summary>
    /// <param name="id">Team Id</param>
    public async Task<List<MemberDto>> GetMembers(Guid id, string name)
    {
        var query = await _repositorys.TeamView.GetQueryableAsync();
        var teams = await query.Where(t => t.TeamId == id)
            .WhereIf(!name.IsNullOrEmpty(), t => t.UserName.Contains(name)).ToListAsync();
        return ObjectMapper.Map<List<TeamView>, List<MemberDto>>(teams);
    }

    /// <summary>
    /// 透過 name 搜尋使用者
    /// </summary>
    public async Task<List<MemberDto>> SearchMember(string name)
    {
        var users = await _userRepository.GetListAsync();
        var dtos = users.WhereIf(!name.IsNullOrEmpty(), u => u.UserName.Contains(name))
            .Select(u => new MemberDto()
            {
                UserId = u.Id,
                UserName = u.UserName
            }).ToList();
        return dtos;
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