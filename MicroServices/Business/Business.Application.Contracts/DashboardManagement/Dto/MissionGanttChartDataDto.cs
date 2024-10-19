using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionGanttChartDataDto : EntityDto<Guid?>
{
    /// <summary>
    /// 任務名稱
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// 開始時間
    /// </summary>
    public string Start_date { get; set; }
    
    /// <summary>
    /// 持續天數
    /// </summary>
    public int Duration { get; set; }
    
    /// <summary>
    /// 順序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 任務完成百分比
    /// </summary>
    public decimal Progress { get; set; }
    
    /// <summary>
    /// 指定父任務
    /// </summary>
    public Guid? ParentId { get; set; }
    
    /// <summary>
    /// 語系
    /// </summary>
    public int Lang { get; set; }
}