using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class MissionI18N: AuditedAggregateRoot<Guid> , ISoftDelete
{
    [Required]
    [ForeignKey(nameof(MissionId))] 
    public Guid MissionId { get; set; }
    public Mission? Mission { get; set; }
    
    //任務名稱
    [Required] 
    public string MissionName { get; set; }

    // 任務說明
    public string MissionDescription { get; set; }

    // 對應語系
    public int Lang { get; set; }

    public bool IsDeleted { get; set; }

}