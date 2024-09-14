using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.MissionCategoryManagement;
using Business.MissionCategoryManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("missionCategory")]
[Route("api/business/category")]
public class MissionCategoryController : AbpController
{
    private readonly IMissionCategoryAppService _missionCategoryAppService;

    public MissionCategoryController(IMissionCategoryAppService missionCategoryAppService)
    {
        _missionCategoryAppService = missionCategoryAppService;
    }

    /// <summary>
    /// 查看當前使用者所建立的任務類別
    /// </summary>
    [HttpGet]
    [Route("all")]
    public Task<PagedResultDto<MissionCategoryViewDto>> GetAll()
    {
        return _missionCategoryAppService.GetAll();
    }
    
    /// <summary>
    /// 查看特定任務類別
    /// </summary>
    [HttpGet]
    [Route("{id}")]
    public Task<MissionCategoryViewDto> Get(Guid id)
    {
        return _missionCategoryAppService.Get(id);
    }

    /// <summary>
    /// 新增或修改當前使用者所建立的任務類別
    /// </summary>
    [HttpPost]
    [Route("data-post")]
    public Task<MissionCategoryI18Dto> DataPost([FromBody]CreateOrUpodateMissionCategoryDto input)
    {
        return _missionCategoryAppService.DataPost(input);
    }

    /// <summary>
    /// 刪除當前使用者所建立的任務類別
    /// </summary>
    [HttpDelete]
    [Route("delete")]
    public async Task Delete(Guid id)
    {
        await _missionCategoryAppService.Delete(id);
    }
}