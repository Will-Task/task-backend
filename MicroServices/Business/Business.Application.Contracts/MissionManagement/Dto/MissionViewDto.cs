using System;
using System.ComponentModel.DataAnnotations;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionViewDto : EntityDto<Int64>
{
    public Guid MissionId { get; set; }
    
    public MissionState MissionState { get; set; } 
        
    // 任務重要程度
    public int MissionPriority { get; set; }

    [Required] 
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public int? MissionBeforeEnd { get; set; }

    [Required] 
    public string MissionName { get; set; }

    // 任務說明
    public string MissionDescription { get; set; }

    // 對應語系
    public int Lang { get; set; }

    [Required] 
    public string MissionCategoryName { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string Email { get; set; }
    
    public Guid? ParentMissionId { get; set; }
    
    /// <summary>
    /// 父類別 Id
    /// </summary>
    
    public Guid? ParentCategoryId { get; set; }
    
    public Guid MissionCategoryId { get; set; }
    
    // 定時任務排成(0 -> 不會重複 1 -> weekly 2 -> daily 3-> monthly)
    public int? Schedule { get; set; }
    
    // 定時任務的主要原頭Id
    public Guid? ScheduleMissionId { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }

    public int AttachmentCount { get; set; }
}