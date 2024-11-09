using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Business.Enums;
using Business.MissionCategoryManagement.Dto;
using Business.Models;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionDto : EntityDto<Guid>
{
    /// <summary>
    /// 任務重要程度
    /// </summary>
    [Required]
    public int MissionPriority { get; set; }
    
    /// <summary>
    /// 父任務為空，子任務為屬於哪個父任務
    /// </summary>
    public Guid? ParentMissionId { get; set; }
    
    [ForeignKey(nameof(MissionCategoryId))]
    [Required]
    public Guid MissionCategoryId { get; set; }
    public MissionCategory? MissionCategory { get; set; }
    
    [Required] 
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }

    /// <summary>
    /// 用來設置要在結束前多久提醒
    /// </summary>
    public int? MissionBeforeEnd { get; set; }

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
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
}