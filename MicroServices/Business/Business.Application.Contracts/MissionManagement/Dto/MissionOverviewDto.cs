using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionOverviewDto : EntityDto<Int64>
{
    public Guid MissionId { get; set; }
    
    /// <summary>
    /// 父任務資訊
    /// </summary>
    [Required]
    public string MissionName { get; set; }
    
    [Required] 
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }

    /// <summary>
    /// 子任務資訊
    /// </summary>
    public List<SubMissionOverviewDto> SubMissions { get; set; } = new List<SubMissionOverviewDto>();
}