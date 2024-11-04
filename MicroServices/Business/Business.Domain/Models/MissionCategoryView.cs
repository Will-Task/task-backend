using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Business.Models;

public class MissionCategoryView: Entity<Int64>
{
    [Required]
    public Guid MissionCategoryId { get; set; }
    
    public Guid? UserId { get; set; }
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    public int Lang { get; set; }

}