using System;

namespace Business.ReportManagement.Dto;

public class ExportMissionOverAllViewDto
{
    public string CategoryName { get; set; }
    
    public string SubCategoryName { get; set; }
    
    public Guid MissionId { get; set; }
    
    public string MissionName { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }
    
    public string SubMissionName { get; set; }
    
    public DateTime? SubMissionFinishTime { get; set; } 
    
    public int Lang { get; set; }
}