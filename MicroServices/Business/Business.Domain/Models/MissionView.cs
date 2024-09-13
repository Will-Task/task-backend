using System;
using System.ComponentModel.DataAnnotations;
using Business.Enums;
using Volo.Abp.Domain.Entities;

namespace Business.Models;

public class MissionView: Entity<Int64>
{
    
    public Guid MissionId { get; set; }
    
    public MissionState MissionState { get; set; }
    
    // 任務重要程度
    public int MissionPriority { get; set; }
    
    [Required] 
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }
    
    [Required] 
    public string MissionName { get; set; }

    // 任務說明
    public string? MissionDescription { get; set; }

    // 對應語系
    public int Lang { get; set; }
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string Email { get; set; }
    
    public Guid? ParentMissionId { get; set; }
    
    public Guid? MissionCategoryId { get; set; }
}