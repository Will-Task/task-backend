using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.DashboardManagement;

public interface IDashboardAppService : IApplicationService
{
    /// <summary>
    /// 根據任務狀態陳列任務
    /// </summary>
    Task<Dictionary<int, List<MissionKanbanDto>>> GetKanbanData(Guid? teamId);

    /// <summary>
    /// 各類別完成度統計
    /// </summary>
    Task<List<MissionProgressDto>> GetMissionProgress(Guid? teamId);

    /// <summary>
    /// 最近任務獲取(當天)
    /// </summary>
    Task<List<MissionRecentDto>> GetMissionRecent(Guid? teamId);

    /// <summary>
    /// 每個類別所花的時間佔比
    /// </summary>
    Task<List<CategoryPercentageDto>> GetCategoryPercentage(Guid? teamId);

    /// <summary>
    /// 每月任務完成數
    /// </summary>
    Task<List<MissionOfEveryMonthDto>> GetMissionOfEveryMonth(Guid? teamId);
}