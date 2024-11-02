using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseService.Systems.TeamManagement.Dto;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.TeamManagement;

public interface ITeamAppService : IApplicationService
{
    /// <summary>
    /// 獲取當前使用者所在的所有群組資訊
    /// </summary>
    Task<List<TeamViewDto>> GetAll();

    /// <summary>
    /// 獲取某團隊成員資訊
    /// </summary>
    /// <param name="id">Team Id</param>
    Task<List<MemberDto>> GetMembers(Guid id);

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
    /// 邀請人進入團隊
    /// </summary>
    /// <param name="name">邀請人姓名</param>
    /// <param name="id">要被邀請到的團隊 Id</param>
    Task Invite(string name, Guid id);
}