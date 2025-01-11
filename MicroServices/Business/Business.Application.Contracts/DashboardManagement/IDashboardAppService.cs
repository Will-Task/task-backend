using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.DashboardManagement;

public interface IDashboardAppService : IApplicationService
{
    /// <summary>
    /// 取得最近要做任務清單
    /// </summary>
    Task<List<ToDoMissionViewDto>> GetToDoList(int page , int pageSize , bool allData);

    /// <summary>
    /// 計算每個月的任務延遲狀況
    /// </summary>
    Task<List<MissionDelayDto>> GetMissionDelays();

    /// <summary>
    /// 取得每個父任務底下的子任務完成度(完成任務 / 總子任務數)
    /// </summary>
    Task<List<MissionProgressDto>> GetMissionFinishPercentage();

    /// <summary>
    /// 獲取過去7天的任務進度
    /// </summary>
    // Task<List<MissionProgressDetailDto>> GetMissionProgress();

    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    // Task<MissionGanttDto> GetGanttChart();

    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    // Task<List<MissionKanbanDto>> GetKanbanChart();

    /// <summary>
    /// 根據任務狀態陳列任務
    /// </summary>
    Task<Dictionary<int, List<MissionKanbanDto>>> GetKanbanData(Guid? teamId);

    /// <summary>
    /// 各類別統計完成
    /// </summary>
    Task<List<MissionProgressDto>> GetMissionProgress(Guid? teamId);

    /// <summary>
    /// 最近任務獲取(當天)
    /// </summary>
    Task<List<MissionRecentDto>> GetMissionRecent(Guid? teamId);
}