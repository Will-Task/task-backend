using System;
using Volo.Abp.Domain.Entities;

namespace Business.Models;

public class MissionOverAllView : Entity<Int64>
{
    public Guid TeamId { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public string CategoryName { get; set; }
    
    public Guid SubCategoryId { get; set; }
    
    public string SubCategoryName { get; set; }
    
    public Guid MissionId { get; set; }
    
    public string MissionName { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }
    
    public Guid SubMissionId { get; set; }
    
    public string SubMissionName { get; set; }
    
    public DateTime? SubMissionFinishTime { get; set; } 
    
    public int Lang { get; set; }
}