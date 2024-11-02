using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseService.Controllers;
using BaseService.Systems.TeamManagement;
using BaseService.Systems.TeamManagement.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BaseService.HttpApi.Systems;

[Area("Team")]
[Route("api/base/team")]
public class TeamController : BaseServiceController
{
    private readonly ITeamAppService _teamAppService;

    public TeamController(ITeamAppService teamAppService)
    {
        _teamAppService = teamAppService;
    }

    /// <summary>
    /// 獲取當前使用者所在的所有群組資訊
    /// </summary>
    [HttpGet]
    [Route("all")]
    public async Task<List<TeamViewDto>> GetAll()
    {
        return await _teamAppService.GetAll();
    }

    /// <summary>
    /// 獲取某團隊成員資訊
    /// </summary>
    /// <param name="id">Team Id</param>
    [HttpGet]
    [Route("{id}/members")]
    public async Task<List<MemberDto>> GetMembers(Guid id)
    {
        return await _teamAppService.GetMembers(id);
    }

    /// <summary>
    /// 團隊建立或資訊修改
    /// </summary>
    [HttpPost]
    [Route("data-post")]
    public async Task<TeamDto> DataPost(CreateOrUpdateTeamDto input)
    {
        return await _teamAppService.DataPost(input);
    }

    /// <summary>
    /// 團隊刪除
    /// </summary>
    /// <param name="ids">Team id 的集合</param>
    [HttpPost]
    [Route("delete")]
    public async Task Delete(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            await _teamAppService.Delete(id);
        }
    }

    /// <summary>
    /// 邀請人進入團隊
    /// </summary>
    /// <param name="name">邀請人姓名</param>
    /// <param name="id">要被邀請到的團隊 Id</param>
    [HttpGet]
    [Route("invite")]
    public async Task Invite(string name, Guid id)
    {
        await _teamAppService.Invite(name, id);
    }
}