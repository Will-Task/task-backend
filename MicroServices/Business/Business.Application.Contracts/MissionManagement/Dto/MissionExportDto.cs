using System;
using System.ComponentModel.DataAnnotations;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionExportDto
{
    //任務名稱
    [Required]
    public string? MissionName { get; set; }
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    [Required]
    public string MissionDescription { get; set; }
    
    [Required]
    public DateTime MissionStartTime { get; set; }
    
    [Required]
    public DateTime MissionEndTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }
    
    public int? MissionBeforeEnd { get; set; }
    
    // 任務重要程度
    [Required]
    public int MissionPriority { get; set; }

    [Required]
    public MissionState MissionState { get; set; } 
}