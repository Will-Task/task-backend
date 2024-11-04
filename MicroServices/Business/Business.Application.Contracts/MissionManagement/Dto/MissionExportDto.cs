using System;
using System.ComponentModel.DataAnnotations;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionExportDto : EntityDto<Guid?>
{
    //任務名稱
    [Required]
    public string? MissionName { get; set; }
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    // 任務重要程度
    [Required]
    public int MissionPriority { get; set; }

    [Required]
    public MissionState MissionState { get; set; } 
    
    [Required]
    public DateTime MissionStartTime { get; set; }
    
    [Required]
    public DateTime MissionEndTime { get; set; }
    
    [Required]
    public string? SubMissionName { get; set; }
    
    [Required]
    public string? SubMissionDescription { get; set; }
 
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
}