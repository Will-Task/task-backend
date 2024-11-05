using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseService.BaseData;
using BaseService.Enums;
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
        IRepository<TeamMission> TeamMission,
        IRepository<TeamInvitation, Guid> TeamInvitation,
        IRepository<TeamInvitationView> TeamInvitationView
        ) _repositorys;

    protected IIdentityUserRepository _userRepository { get; }

    public TeamAppService(
        IRepository<Team, Guid> Team,
        IRepository<TeamView> TeamView,
        IRepository<TeamMission> TeamMission,
        IRepository<TeamInvitation, Guid> TeamInvitation,
        IRepository<TeamInvitationView> TeamInvitationView,
        IIdentityUserRepository userRepository)
    {
        _repositorys = (Team, TeamView, TeamMission, TeamInvitation, TeamInvitationView);
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
            result = await _repositorys.Team.InsertAsync(ObjectMapper.Map<CreateOrUpdateTeamDto, Team>(input),
                autoSave: true);
            // 把當前使用者加入團隊
            await _repositorys.TeamMission.InsertAsync(new TeamMission
            {
                UserId = CurrentUser.Id.Value,
                TeamId = result.Id
            });
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
    /// 發起邀請人進入團隊的請求
    /// </summary>
    /// <param name="name">邀請人姓名</param>
    /// <param name="id">要被邀請到的團隊 Id</param>
    public async Task<List<TeamInvitationDto>> Invite(List<CreateOrUpdateTeamInvitationDto> inputs)
    {
        var invitations = ObjectMapper.Map<List<CreateOrUpdateTeamInvitationDto>, List<TeamInvitation>>(inputs);
        foreach (var invitation in invitations)
        {
            invitation.UserId = CurrentUser.Id.Value;
            await _repositorys.TeamInvitation.InsertAsync(invitation, autoSave: true);
        }

        return ObjectMapper.Map<List<TeamInvitation>, List<TeamInvitationDto>>(invitations);
    }

    /// <summary>
    /// 將團隊中某人逐出
    /// </summary>
    public async Task Drop(DropFormData formData)
    {
        await _repositorys.TeamMission.DeleteAsync(t =>
            t.TeamId == formData.TeamId && t.UserId == (formData.UserId.HasValue ? formData.UserId : CurrentUser.Id));
    }

    /// <summary>
    /// 獲取邀請的條件
    /// 1. 受邀人為當前使用者
    /// 2. 邀請人為當前使用者
    /// </summary>
    public async Task<List<TeamInvitationViewDto>> GetInvitations(int? state, string name)
    {
        var currentUserId = CurrentUser.Id;
        var inviteQuery = await _repositorys.TeamInvitationView.GetQueryableAsync();
        var invitations = await inviteQuery.Where(x => x.UserId == currentUserId || x.InvitedUserId == currentUserId)
            .WhereIf(state.HasValue, x => x.State == (Invitation)state)
            .WhereIf(!name.IsNullOrEmpty(),
                x => x.TeamName.Contains(name) || x.UserName.Contains(name) || x.InvitedUserName.Contains(name))
            .ToListAsync();

        var dtos = ObjectMapper.Map<List<TeamInvitationView>, List<TeamInvitationViewDto>>(invitations);
        dtos.ForEach(x => x.IsShow = x.ResponseTime.HasValue ? 3 : x.UserId == currentUserId ? 2 : 1);
        return dtos;
    }

    /// <summary>
    /// 取消團隊邀請請求
    /// </summary>
    /// <param name="id">invitation id</param>
    public async Task CancelInvitation(Guid id)
    {
        await UpdateInviteState(id, Invitation.Canceled);
    }

    /// <summary>
    /// 接受團隊邀請請求
    /// </summary>
    /// <param name="id">invitation id</param>
    public async Task AcceptInvitation(Guid id)
    {
        var invitation = await UpdateInviteState(id, Invitation.Accepted);
        // 加入團隊
        await _repositorys.TeamMission.InsertAsync(new TeamMission
        {
            UserId = invitation.InvitedUserId,
            TeamId = invitation.TeamId
        });
    }

    /// <summary>
    /// 拒絕團隊邀請請求
    /// </summary>
    /// <param name="id">invitation id</param>
    public async Task DeclineInvitation(Guid id)
    {
        await UpdateInviteState(id, Invitation.Declined);
    }

    /// <summary>
    /// 更新邀請狀態
    /// </summary>
    private async Task<TeamInvitation> UpdateInviteState(Guid id, Invitation invitation)
    {
        var invite = await _repositorys.TeamInvitation.GetAsync(id);
        invite.State = invitation;
        invite.ResponseTime = Clock.Now;
        return invite;
    }
}