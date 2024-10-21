using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement;
using Business.DashboardManagement.Dto;
using Business.MissionManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("Dashboard")]
[Route("/api/business/dashboard")]
public class DashboardController : AbpController
{
    private readonly IDashboardAppService dashboardAppService;
    public DashboardController(IDashboardAppService _dashboardAppService)
    {
        dashboardAppService = _dashboardAppService;
    }

    /// <summary>
    /// 取得最近要做任務清單
    /// </summary>
    [HttpGet]
    [Route("todo")]
    public Task<List<ToDoMissionViewDto>> GetToDoList(int page , int pageSize , bool allData)
    {
        return dashboardAppService.GetToDoList(page , pageSize , allData);
    }
    
    /// <summary>
    /// 計算每個月的任務延遲狀況
    /// </summary>
    [HttpGet]
    [Route("delay")]
    public Task<List<MissionDelayDto>> GetMissionDelays()
    {
        return dashboardAppService.GetMissionDelays();
    }
    
    /// <summary>
    /// 取得每個父任務底下的子任務完成度(完成任務 / 總子任務數)
    /// </summary>
    [HttpGet]
    [Route("percentage")]
    public Task<List<MissionProgressDto>> GetMissionFinishPercentage()
    {
        return dashboardAppService.GetMissionFinishPercentage();
    }

    /// <summary>
    /// 獲取過去7天的任務進度
    /// </summary>
    [HttpGet]
    [Route("progress")]
    public Task<List<MissionProgressDetailDto>> GetMissionProgress()
    {
        return dashboardAppService.GetMissionProgress();
    }
    
    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    [HttpGet]
    [Route("gantt/chart")]
    public Task<MissionGanttDto> GetGanttChart()
    {
        return dashboardAppService.GetGanttChart();
    }
    
    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    [HttpGet]
    [Route("kanban/chart")]
    public Task<List<MissionKanbanDto>> GetKanbanChart()
    {
        return dashboardAppService.GetKanbanChart();
    }
}