using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class MissionCategoryI18N: AuditedAggregateRoot<Guid>,ISoftDelete
{
    
    [Required]
    [ForeignKey(nameof(MissionCategoryId))]
    public Guid MissionCategoryId { get; set; }
    public MissionCategory? MissionCategory { get; set; }
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    public int Lang { get; set; }
    
    public bool IsDeleted { get; set; }

}