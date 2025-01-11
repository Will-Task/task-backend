using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement;
using Business.DashboardManagement.Dto;
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
    /// 取得最近要做任務清單
    /// </summary>
    [HttpGet]
    [Route("todo")]
    public Task<List<ToDoMissionViewDto>> GetToDoList(int page , int pageSize , bool allData)
    {
        return _dashboardAppService.GetToDoList(page , pageSize , allData);
    }
    
    /// <summary>
    /// 計算每個月的任務延遲狀況
    /// </summary>
    [HttpGet]
    [Route("delay")]
    public Task<List<MissionDelayDto>> GetMissionDelays()
    {
        return _dashboardAppService.GetMissionDelays();
    }
    
    /// <summary>
    /// 取得每個父任務底下的子任務完成度(完成任務 / 總子任務數)
    /// </summary>
    [HttpGet]
    [Route("percentage")]
    public Task<List<MissionProgressDto>> GetMissionFinishPercentage()
    {
        return _dashboardAppService.GetMissionFinishPercentage();
    }

    /// <summary>
    /// 獲取過去7天的任務進度
    /// </summary>
    // [HttpGet]
    // [Route("progress")]
    // public Task<List<MissionProgressDetailDto>> GetMissionProgress()
    // {
    //     return _dashboardAppService.GetMissionProgress();
    // }
    
    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    // [HttpGet]
    // [Route("gantt/chart")]
    // public Task<MissionGanttDto> GetGanttChart()
    // {
    //     return dashboardAppService.GetGanttChart();
    // }
    
    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    // [HttpGet]
    // [Route("kanban/chart")]
    // public Task<List<MissionKanbanDto>> GetKanbanChart()
    // {
    //     return dashboardAppService.GetKanbanChart();
    // }

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
    /// 各類別統計完成
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
}