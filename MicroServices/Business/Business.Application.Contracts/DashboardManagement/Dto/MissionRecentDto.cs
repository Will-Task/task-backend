using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionRecentDto : EntityDto<Guid>
{
    public string MissionName { get; set; }
    
    public int Priority { get; set; }
    
    public string StartTime { get; set; }
    
    public string EndTime { get; set; }
    
    public int Lang { get; set; }
}