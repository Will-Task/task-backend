using System;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionI18NDto : EntityDto<Guid?>
{
    // 任務重要程度
    public int MissionPriority { get; set; }

    // 父任務為空，子任務為屬於哪個父任務
    public Guid? ParentMissionId { get; set; }
    
    public DateTime MissionStartTime { get; set; }
    
    public DateTime MissionEndTime { get; set; }
    
    public Guid? MissionCategoryId { get; set; }

    //任務名稱
    public string? MissionName { get; set; }

    // 任務說明
    public string? MissionDescription { get; set; }

    // 對應語系
    public int Lang { get; set; }
}