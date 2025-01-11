using System;

namespace Business.DashboardManagement.Dto;

public class MissionKanbanDto
{
    public Guid MissionId { get; set; }
    
    public string MissionName { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public string CategoryName { get; set; }
    
    public int Month { get; set; }
    
    public int Day { get; set; }
    
    public int Lang { get; set; }
}