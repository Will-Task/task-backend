using Business.Core.Enums;
using Business.Models;
using Business.TeamManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.CommonManagement.Dto;
using Business.Specifications.TeamMember;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using Microsoft.EntityFrameworkCore;

namespace Business.TeamManagement
{
    [RemoteService(false)]
    public class TeamAppService : ApplicationService, ITeamAppService
    {
        private (
            IRepository<Team, Guid> Team,
            IRepository<TeamMember> TeamMember,
            IRepository<TeamInvitation, Guid> TeamInvitation,
            IRepository<AbpUserView, Guid> AbpUserView
            ) _repositorys;

        public TeamAppService(
            IRepository<Team, Guid> Team,
            IRepository<TeamMember> TeamMember,
            IRepository<TeamInvitation, Guid> TeamInvitation,
            IRepository<AbpUserView, Guid> AbpUserView
            )
        {
            _repositorys = (Team, TeamMember, TeamInvitation, AbpUserView);
        }

        #region CRUD方法

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
                var team = ObjectMapper.Map<CreateOrUpdateTeamDto, Team>(input);
                team.UserId = CurrentUser.Id.Value;
                result = await _repositorys.Team.InsertAsync(team, autoSave: true);
                // 把當前使用者加入團隊
                await _repositorys.TeamMember.InsertAsync(new TeamMember
                {
                    UserId = CurrentUser.Id.Value,
                    TeamId = team.Id
                });
            }

            return ObjectMapper.Map<Team, TeamDto>(result);
        }

        /// <summary>
        /// 獲取當前使用者所在的所有群組資訊
        /// </summary>
        public async Task<List<TeamDto>> GetAll(string name, int? year)
        {
            var userId = CurrentUser.Id;
            var teamMissionQuery = await _repositorys.TeamMember.GetQueryableAsync();
            // 取得當前使用者所在的所有團隊的TeamId
            var teamIds = await teamMissionQuery.Where(new UserTeamMemberSpecification(userId.Value))
                .Select(x => x.TeamId).ToListAsync();
            
            var teamQuery = await _repositorys.Team.GetQueryableAsync();
            var teams = await teamQuery.Where(x => teamIds.Contains(x.Id))
                .WhereIf(year.HasValue, x => x.Year == year)
                .WhereIf(!name.IsNullOrEmpty(), x => x.Name.Contains(name)).ToListAsync();
            
            return ObjectMapper.Map<List<Team>, List<TeamDto>>(teams);
        }
        
        /// <summary>
        /// 團隊刪除
        /// </summary>
        /// <param name="id">Team id</param>
        public async Task Delete(Guid id)
        {
            var query = await _repositorys.TeamMember.GetQueryableAsync();
            var count = await query.Where(x => x.TeamId == id).CountAsync();
            // 代表除了建立者還有別人
            if (count >= 2)
            {
                throw new BusinessException("該團隊下面還有成員無法直接刪除");
            }
            await _repositorys.Team.DeleteAsync(id);
        }
        
        /// <summary>
        /// 獲取邀請的條件
        /// 1. 受邀人為當前使用者
        /// 2. 邀請人為當前使用者
        /// </summary>
        public async Task<List<TeamInvitationDto>> GetInvitations(Guid? teamId, int? state, string name)
        {
            var currentUserId = CurrentUser.Id;
            var queryUser = await _repositorys.AbpUserView.GetQueryableAsync();
            var userMap = queryUser.ToDictionary(x => x.Id, x => x.UserName);
            var queryTeam = await _repositorys.Team.GetQueryableAsync();
            var teamMap = queryTeam.ToDictionary(x => x.Id, x => x.Name);
            
            var queryInvitation = await _repositorys.TeamInvitation.GetQueryableAsync();
            var invitations = await queryInvitation
                .Where(x => x.UserId == currentUserId || x.InvitedUserId == currentUserId)
                .WhereIf(state.HasValue, x => x.State == (Invitation)state)
                .WhereIf(!name.IsNullOrEmpty(),
                    x => teamMap[x.TeamId].Contains(name) || userMap[x.UserId].Contains(name) || userMap[x.InvitedUserId].Contains(name))
                .ToListAsync();

            var dtos = ObjectMapper.Map<List<TeamInvitation>, List<TeamInvitationDto>>(invitations);
            dtos.ForEach(x =>
            {
                x.IsShow = x.ResponseTime.HasValue ? 3 : x.UserId == currentUserId ? 2 : 1;
                x.TeamName = teamMap[x.TeamId];
                x.UserName = userMap[x.UserId];
                x.InvitedUserName = userMap[x.InvitedUserId];
            });
            return dtos;
        }

        /// <summary>
        /// 搜尋欲邀請成員名稱
        /// </summary>
        /// <param name="id">Team Id</param>
        public async Task<List<AbpUserViewDto>> GetUsers(string name)
        {
            var users = await _repositorys.AbpUserView.GetListAsync(x => x.Id != CurrentUser.Id);
            return ObjectMapper.Map<List<AbpUserView>, List<AbpUserViewDto>>(users);
        }
        
        #endregion CRUD方法

        /// <summary>
        /// 發起邀請人進入團隊的請求
        /// </summary>
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
            await _repositorys.TeamMember.DeleteAsync(t =>
                t.TeamId == formData.TeamId && t.UserId == (formData.UserId.HasValue ? formData.UserId : CurrentUser.Id));
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
            await _repositorys.TeamMember.InsertAsync(new TeamMember
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
            invite.ResponseTime = invitation == Invitation.Canceled ? null : Clock.Now;
            return invite;
        }

        /// <summary>
        /// 邀請記錄匯出
        /// </summary>
        public async Task Export(int? state, string name, string code)
        {
            throw new NotImplementedException();
        }
    }
}
