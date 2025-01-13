using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement;
using Business.DashboardManagement.Dto;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("Dashboard")]
[Route("/api/business/dashboard")]
public class DashboardController : AbpController
{
    private readonly IDashboardAppService _dashboardAppService;
    public DashboardController(IDashboardAppService dashboardAppService)
    {
        _dashboardAppService = dashboardAppService;
    }

    /// <summary>
    /// 根據任務狀態陳列任務
    /// </summary>
    [HttpGet]
    [Route("kanbanData")]
    public async Task<Dictionary<int, List<MissionKanbanDto>>>　GetKanbanData(Guid? teamId)
    {
        return await _dashboardAppService.GetKanbanData(teamId);
    }

    /// <summary>
    /// 各類別完成度統計
    /// </summary>
    [HttpGet]
    [Route("progress")]
    public async Task<List<MissionProgressDto>> GetMissionProgress(Guid? teamId)
    {
        return await _dashboardAppService.GetMissionProgress(teamId);
    }
    
    /// <summary>
    /// 最近任務獲取(當天)
    /// </summary>
    [HttpGet]
    [Route("recent/mission")]
    public async Task<List<MissionRecentDto>> GetMissionRecent(Guid? teamId)
    {
        return await _dashboardAppService.GetMissionRecent(teamId);
    }

    /// <summary>
    /// 每個類別所花的時間佔比
    /// </summary>
    [HttpGet]
    [Route("category/percentage")]
    public async Task<List<CategoryPercentageDto>> GetCategoryPercentage(Guid? teamId)
    {
        return await _dashboardAppService.GetCategoryPercentage(teamId);
    }
    
    /// <summary>
    /// 每月任務完成數
    /// </summary>
    [HttpGet]
    [Route("month/mission")]
    public async Task<List<MissionOfEveryMonthDto>> GetMissionOfEveryMonth(Guid? teamId)
    {
        return await _dashboardAppService.GetMissionOfEveryMonth(teamId);
    }
}