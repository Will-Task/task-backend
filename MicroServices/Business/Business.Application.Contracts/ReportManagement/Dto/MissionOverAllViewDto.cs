using System;
using Volo.Abp.Application.Dtos;

namespace Business.ReportManagement.Dto;

public class MissionOverAllViewDto: EntityDto<Int64>
{
    public Guid TeamId { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public string CategoryName { get; set; }
    
    public Guid SubCategoryId { get; set; }
    
    public string SubCategoryName { get; set; }
    
    public Guid MissionId { get; set; }
    
    public string MissionName { get; set; }
    
    public Guid SubMissionId { get; set; }
    
    public Guid SubMissionName { get; set; }
    
    public int Lang { get; set; }
}