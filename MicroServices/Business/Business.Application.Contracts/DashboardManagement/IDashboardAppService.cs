using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement.Dto;
using Business.MissionManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.DashboardManagement;

public interface IDashboardAppService : IApplicationService
{
    /// <summary>
    /// 取得最近要做任務清單
    /// </summary>
    Task<List<ToDoMissionViewDto>> GetToDoList(int page , int pageSize , bool allData);

    /// <summary>
    /// 取得每個父任務底下的子任務完成度(完成任務 / 總子任務數)
    /// </summary>
    Task<List<MissionProgressDto>> GetMissionFinishPercentage();

    /// <summary>
    /// 獲取過去7天的任務進度
    /// </summary>
    Task<List<MissionProgressDetailDto>> GetMissionProgress();

    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    Task<List<MissionGanttChartDataDto>> GetGanttChart();

    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    Task<Dictionary<int,List<MissionKanbanChartDataDto>>> GetKanbanChart();
}