using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionProgressDetailDto : EntityDto<Guid?>
{
    public string ParentMissionName { get; set; }
    
    public int MonProgress { get; set; }
    
    public int TueProgress { get; set; }
    
    public int WedProgress { get; set; }
    
    public int ThurProgress { get; set; }
    
    public int FriProgress { get; set; }
    
    public int SatProgress { get; set; }
    
    public int SunProgress { get; set; }
    
    public int Lang { get; set; }
}