using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.MissionManagement.Dto;
using Business.MissionTagManagement;
using Business.MissionTagManagement.Dto;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("MissionTag")]
[Route("api/business/mission-tag")]
public class MissionTagController : AbpController
{
    private readonly IMissionTagAppService _missionTagAppService;

    public MissionTagController(IMissionTagAppService missionTagAppService)
    {
        _missionTagAppService = missionTagAppService;
    }

    /// <summary>
    /// 創建/修改標籤
    /// </summary>
    [HttpPost]
    [Route("data-post")]
    public Task<MissionTagDto> DataPost([FromBody] CreateOrUpdateMissionTagDto input)
    {
        return _missionTagAppService.DataPost(input);
    }   

    /// <summary>
    /// 根據標籤篩選任務
    /// </summary>
    [HttpGet]
    [Route("filter-tag")]
    public Task<IEnumerable<MissionViewDto>> FilterTag(Guid id)
    {
        return _missionTagAppService.FilterTag(id);
    }

    /// <summary>
    /// 為任務添加標籤
    /// </summary>
    [HttpPost]
    [Route("add-tag")]
    public Task AddTagToMission(Guid id, Guid missionId)
    {
        return _missionTagAppService.AddTagToMission(id, missionId);
    }

    /// <summary>
    /// 從任務上標籤
    /// </summary>
    [HttpDelete]
    [Route("remove-tag")]
    public Task RemoveTagFromMission(Guid id, Guid missionId)
    {
        return _missionTagAppService.RemoveTagFromMission(id, missionId);
    }

    /// <summary>
    /// 移除標籤
    /// </summary>
    [HttpDelete]
    [Route("delete")]
    public async Task Delete(Guid id, int lang = 1)
    {
        await _missionTagAppService.Delete(id, lang);
    }
}