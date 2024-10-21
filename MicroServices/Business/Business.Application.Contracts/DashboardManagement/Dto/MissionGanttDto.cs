using System.Collections.Generic;

namespace Business.DashboardManagement.Dto;

public class MissionGanttDto
{
    public List<MissionGanttChartDataDto> Tasks { get; set; }
    
    public List<MissionLinkDto> Links { get; set; }
}