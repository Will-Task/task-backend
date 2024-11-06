using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseService.Systems.TeamManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.TeamManagement;

public interface ITeamAppService : IApplicationService
{
    /// <summary>
    /// 獲取當前使用者所在的所有群組資訊
    /// </summary>
    Task<List<TeamDto>> GetAll(string name);

    /// <summary>
    /// 獲取某團隊成員資訊
    /// </summary>
    /// <param name="id">Team Id</param>
    Task<List<MemberDto>> GetMembers(Guid? id, string name);

    /// <summary>
    /// 團隊建立或資訊修改
    /// </summary>
    Task<TeamDto> DataPost(CreateOrUpdateTeamDto input);

    /// <summary>
    /// 團隊刪除
    /// </summary>
    /// <param name="id">Team id</param>
    Task Delete(Guid id);

    /// <summary>
    /// 發起邀請人進入團隊的請求
    /// </summary>
    /// <param name="name">邀請人姓名</param>
    /// <param name="id">要被邀請到的團隊 Id</param>
    Task<List<TeamInvitationDto>> Invite(List<CreateOrUpdateTeamInvitationDto> input);

    /// <summary>
    /// 將團隊中某人逐出
    /// </summary>
    Task Drop(DropFormData formData);

    /// <summary>
    /// 獲取邀請的條件
    /// 1. 受邀人為當前使用者
    /// 2. 邀請人為當前使用者
    /// </summary>
    Task<List<TeamInvitationViewDto>> GetInvitations(int? state , string name);
    
    /// <summary>
    /// 取消團隊邀請請求
    /// </summary>
    /// <param name="id">invitation id</param>
    Task CancelInvitation(Guid id);

    /// <summary>
    /// 接受團隊邀請請求
    /// </summary>
    /// <param name="id">invitation id</param>
    Task AcceptInvitation(Guid id);
    
    /// <summary>
    /// 拒絕團隊邀請請求
    /// </summary>
    /// <param name="id">invitation id</param>
    Task DeclineInvitation(Guid id);

    /// <summary>
    /// 邀請記錄匯出
    /// </summary>
    Task Export(int? state, string name, string code);
}