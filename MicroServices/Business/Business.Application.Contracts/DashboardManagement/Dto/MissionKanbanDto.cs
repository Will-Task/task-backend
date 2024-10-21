using System.Collections.Generic;

namespace Business.DashboardManagement.Dto;

public class MissionKanbanDto
{
    public string Name { get; set; }

    public List<MissionKanbanChartDataDto> Tasks { get; set; } = new List<MissionKanbanChartDataDto>();
}