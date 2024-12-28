using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.CommonManagement.Dto;
using Business.Core.Enums;
using Business.FileManagement;
using Business.FileManagement.Dto;
using Business.Models;
using Business.Permissions;
using Business.Specifications.TeamMember;
using Business.TeamManagement.Dto;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using XCZ.Extensions;

namespace Business.TeamManagement
{
    [Authorize(BusinessPermissions.TaskTeam.Default)]
    [RemoteService(false)]
    public class TeamAppService : ApplicationService, ITeamAppService
    {
        private (
            IRepository<Team, Guid> Team,
            IRepository<TeamMember> TeamMember,
            IRepository<TeamInvitation, Guid> TeamInvitation,
            IRepository<AbpUserView, Guid> AbpUserView,
            IRepository<LocalizationText> LocalizationText
            ) _repositorys;

        private readonly IFileAppService _fileAppService;

        private readonly ILogger<TeamAppService> _logger;

        public TeamAppService(
            IRepository<Team, Guid> Team,
            IRepository<TeamMember> TeamMember,
            IRepository<TeamInvitation, Guid> TeamInvitation,
            IRepository<AbpUserView, Guid> AbpUserView,
            IRepository<LocalizationText> LocalizationText,
            IFileAppService fileAppService,
            ILogger<TeamAppService> logger
            )
        {
            _repositorys = (Team, TeamMember, TeamInvitation, AbpUserView, LocalizationText);
            _fileAppService = fileAppService;
            _logger = logger;
        }

        #region CRUD方法

        /// <summary>
        /// 團隊建立或資訊修改
        /// </summary>
        public async Task<TeamDto> DataPost(CreateOrUpdateTeamDto input)
        {
            Team result = null;
            /// For 修改
            if (input.Id.HasValue)
            {
                var team = await _repositorys.Team.GetAsync(input.Id.Value);
                result = ObjectMapper.Map(input, team);
            }
            /// For 新增
            else
            {
                var team = ObjectMapper.Map<CreateOrUpdateTeamDto, Team>(input);
                team.UserId = CurrentUser.Id.Value;
                result = await _repositorys.Team.InsertAsync(team, autoSave: true);
                /// 把當前使用者加入團隊
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
            /// 取得當前使用者所在的所有團隊的TeamId
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
            /// 代表除了建立者還有別人
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
            var dtos = await SearchInvitations(teamId, state, name);
            dtos.ForEach(x =>
            {
                x.IsShow = x.ResponseTime.HasValue ? 3 : x.UserId == CurrentUser.Id ? 2 : 1;
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
        public async Task<BlobDto> Export(Guid? teamId, int? state, string name, string code)
        {
            var file = await _repositorys.LocalizationText.GetAsync(x => x.LanguageCode == code
                                        && x.Category == "Template" && x.ItemKey == "21");

            var blobDto = await _fileAppService.DNFile(file.ItemValue);

            using var memoryStream = new MemoryStream(blobDto.Content);
            using var workBook = new XLWorkbook(memoryStream);
            var workSheet = workBook.Worksheet(1);

            var dtos = await SearchInvitations(teamId, state, name);
            var exportDtos = ObjectMapper.Map<List<TeamInvitationDto>, List<ExportTeamInvitationDto>>(dtos);

            var nextChar = 'A';
            int row = 2;
            foreach (var dto in exportDtos)
            {
                var props = dto.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (prop.GetValue(dto).IsNullOrEmpty())
                    {
                        continue;
                    }
                    workSheet.Cell($"{nextChar++}{row}").Value = prop.GetValue(dto).ToString();
                }
                row++;
                nextChar = 'A';
            }

            using var savingMemoryStream = new MemoryStream();
            workBook.SaveAs(savingMemoryStream);
            /// 更新檔案內容
            blobDto.Content = savingMemoryStream.ToArray();

            return blobDto;
        }

        private async Task<List<TeamInvitationDto>> SearchInvitations(Guid? teamId, int? state, string name)
        {
            var currentUserId = CurrentUser.Id;
            var queryUser = await _repositorys.AbpUserView.GetQueryableAsync();
            var userIds = await queryUser.Where(x => x.UserName.Contains(name)).Select(x => x.Id).ToListAsync();
            var userMap = queryUser.ToDictionary(x => x.Id, x => x.UserName);
            var queryTeam = await _repositorys.Team.GetQueryableAsync();
            var teamIds = await queryTeam.Where(x => x.Name.Contains(name)).Select(x => x.Id).ToListAsync();
            var teamMap = queryTeam.ToDictionary(x => x.Id, x => x.Name);

            var queryInvitation = await _repositorys.TeamInvitation.GetQueryableAsync();
            var invitations = await queryInvitation
                .Where(x => x.TeamId == teamId)
                .WhereIf(state.HasValue, x => x.State == (Invitation)state)
                .WhereIf(!name.IsNullOrEmpty(),
                    x => userIds.Contains(x.UserId) || userIds.Contains(x.InvitedUserId))
                .ToListAsync();

            var dtos = ObjectMapper.Map<List<TeamInvitation>, List<TeamInvitationDto>>(invitations);

            dtos.ForEach(x =>
            {
                x.TeamName = teamMap[x.TeamId];
                x.UserName = userMap[x.UserId];
                x.InvitedUserName = userMap[x.InvitedUserId];
            });

            

            return dtos;
        }
    }
}
