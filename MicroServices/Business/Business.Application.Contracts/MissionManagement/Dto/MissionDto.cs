using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Business.MissionCategoryManagement.Dto;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionDto : EntityDto<Guid>
{
    // 任務重要程度
    public int MissionPriority { get; set; }
    
    // 父任務為空，子任務為屬於哪個父任務
    public Guid? ParentMissionId { get; set; }
    
    public MissionCategoryI18Dto MissionCategoryI18Dto { get; set; }
    
    [Required] 
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public int SubMissionCount { get; set; }

    public ICollection<MissionI18NDto> MissionI18NDtos { get; set; }

}