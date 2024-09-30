using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionProgressDto : EntityDto<Guid?>
{
    public decimal Percentage { get; set; }
    
    public string ParentMissionName { get; set; }
    
    public int Lang { get; set; }
}