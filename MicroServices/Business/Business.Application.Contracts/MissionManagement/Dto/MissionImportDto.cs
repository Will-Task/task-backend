using System;
using System.ComponentModel.DataAnnotations;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

/// <summary>
/// 只匯入子任務
/// </summary>
public class MissionImportDto : EntityDto<Guid?>
{
    public Guid? ParentMissionId { get; set; }
    
    /// <summary>
    /// 父任務為空，子任務為屬於哪個父任務
    /// </summary>
    public string MissionName { get; set; }
    
    [Required]
    public Guid MissionCategoryId { get; set; }
    
    public string MissionDescription { get; set; }
    
    public string SubMissionName { get; set; }

    public string SubMissionDescription { get; set; }
    
    [Required] 
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }    
    
    /// <summary>
    /// 用來設置要在結束前多久提醒
    /// </summary>
    public int? MissionBeforeEnd { get; set; }
    
    /// <summary>
    /// 任務重要程度
    /// </summary>
    [Required]
    public int MissionPriority { get; set; }
    
    [Required] 
    public MissionState MissionState { get; set; }
    
    /// <summary>
    ///  定時任務排成
    /// null -> 不會重複
    /// 1 -> weekly
    /// 2 -> daily
    /// 3 -> monthly
    /// </summary>
    public int? Schedule { get; set; }
    
    /// <summary>
    /// 定時任務的主要源頭Id
    /// </summary>
    public Guid? ScheduleMissionId { get; set; }
    
    public int Lang { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    public Guid? UserId { get; set; }
}