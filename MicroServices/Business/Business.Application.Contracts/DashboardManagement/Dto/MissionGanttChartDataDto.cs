using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionGanttChartDataDto : EntityDto<Guid?>
{
    /// <summary>
    /// 任務名稱
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// 持續天數
    /// </summary>
    public int Duration { get; set; }
    
    /// <summary>
    /// 語系
    /// </summary>
    public int Lang { get; set; }
}