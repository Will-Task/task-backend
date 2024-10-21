using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionKanbanChartDataDto : EntityDto<Guid?>
{
    /// <summary>
    /// 任務名稱
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// 語系
    /// </summary>
    public int Lang { get; set; }
}