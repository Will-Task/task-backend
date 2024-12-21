using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.MissionCategoryManagement.Dto;

public class MissionCategoryViewDto: EntityDto<Int64>
{
    [Required]
    public Guid MissionCategoryId { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string? MissionCategoryName { get; set; }
    
    public int Lang { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    /// <summary>
    /// 父類別 Id
    /// </summary>
    
    public Guid? ParentId { get; set; }
    
    public string ParentName { get; set; }
}