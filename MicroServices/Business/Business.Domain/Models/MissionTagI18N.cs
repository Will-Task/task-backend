using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class MissionTagI18N: AuditedAggregateRoot<Guid> , ISoftDelete
{
    [Required]
    [ForeignKey(nameof(MissionTagId))]
    public Guid MissionTagId { get; set; }
    public MissionTag? MissionTag { get; set; }
    
    [Required]
    public string MissionTagName { get; set; }
    
    public int Lang { get; set; }
    
    public bool IsDeleted { get; set; }

}