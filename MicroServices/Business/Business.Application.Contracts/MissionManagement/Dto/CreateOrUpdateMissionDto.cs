using System;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class CreateOrUpdateMissionDto : EntityDto<Guid?>
{
    public MissionState MissionState { get; set; }
    
    // 任務重要程度
    public int MissionPriority { get; set; }

    // 父任務為空，子任務為屬於哪個父任務
    public Guid? ParentMissionId { get; set; }
    
    public Guid? MissionCategoryId { get; set; }
    
    public DateTime MissionStartTime { get; set; }
    
    public DateTime MissionEndTime { get; set; }
    
    /// <summary>
    /// 用來設置要在結束前多久提醒，預設為前一天
    /// </summary>
    public int MissionBeforeEnd { get; set; }

    //任務名稱
    public string? MissionName { get; set; }

    // 任務說明
    public string? MissionDescription { get; set; }

    // 對應語系
    public int Lang { get; set; }
    
    // 定時任務排成(0 -> 不會重複 1 -> weekly 2 -> daily 3-> monthly)
    public int Schedule { get; set; }
}