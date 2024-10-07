using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Business.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class Mission: AuditedAggregateRoot<Guid> , ISoftDelete,IIsActive
{
    public Guid? UserId { get; set; }
    
    public string Email { get; set; }
    
    // 任務重要程度
    public int MissionPriority { get; set; }
    
    // 父任務為空，子任務為屬於哪個父任務
    public Guid? ParentMissionId { get; set; }
    
    [ForeignKey(nameof(MissionCategoryId))]
    [Required]
    public Guid MissionCategoryId { get; set; }
    public MissionCategory? MissionCategory { get; set; }
    
    [Required] 
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }

    /// <summary>
    /// 用來設置要在結束前多久提醒，預設為前一天
    /// </summary>
    public int MissionBeforeEnd { get; set; } = 24;

    [Required] 
    public MissionState MissionState { get; set; } = MissionState.IN_PROCESS;
    
    // 定時任務排成(0 -> 不會重複 1 -> weekly 2 -> daily 3-> monthly)
    public int Schedule { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; } = true;

    // 資料庫不須對應欄位，也可以在查詢時加仔關聯資訊
    public virtual List<MissionI18N> MissionI18Ns { get; set; }
    
    public virtual List<MissionTag> MissionTags { get; set; }

}