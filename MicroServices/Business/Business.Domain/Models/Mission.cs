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
    
     /// <summary>
     /// 任務重要程度
     /// </summary>
    [Required]
    public int MissionPriority { get; set; }
    
    /// <summary>
    /// 父任務為空，子任務為屬於哪個父任務
    /// </summary>
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
    /// 用來設置要在結束前多久提醒
    /// </summary>
    public int? MissionBeforeEnd { get; set; }

    [Required] 
    public MissionState MissionState { get; set; }
    
    /// <summary>
    ///  定時任務排成
    /// null -> 不會重複
    /// 1 -> weekly
    /// 2 -> daily
    /// 3 -> monthly
    /// </summary>
    public int? Schedule { get; set; }
    
    /// <summary>
    /// 定時任務的主要源頭Id
    /// </summary>
    public Guid? ScheduleMissionId { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 資料庫不須對應欄位，也可以在查詢時加仔關聯資訊
    /// </summary>
    public virtual List<MissionI18N> MissionI18Ns { get; set; }

    public void AddMissionI18N(MissionI18N missionI18N)
    {
        if (MissionI18Ns == null)
        {
            MissionI18Ns = new List<MissionI18N>();
        }
        MissionI18Ns.Add(missionI18N);
    }
    
    public virtual List<MissionTag> MissionTags { get; set; }
}