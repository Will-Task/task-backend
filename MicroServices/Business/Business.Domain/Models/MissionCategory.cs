using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class MissionCategory: AuditedAggregateRoot<Guid>,ISoftDelete
{
    // 現在可為null，之後再做修改
    public Guid? UserId { get; set; }

    public List<MissionCategoryI18N> MissionCategoryI18Ns { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    public bool IsDeleted { get; set; }

}