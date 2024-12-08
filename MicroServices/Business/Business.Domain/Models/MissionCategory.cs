using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class MissionCategory: AuditedAggregateRoot<Guid>,ISoftDelete
{
    public Guid? UserId { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    /// <summary>
    /// 父類別 Id
    /// </summary>
    
    public Guid? ParentId { get; set; }

    public List<MissionCategoryI18N> MissionCategoryI18Ns { get; set; }

    public void AddCategoryI18N(MissionCategoryI18N missionCategoryI18N)
    {
        if (MissionCategoryI18Ns == null)
        {
            MissionCategoryI18Ns = new List<MissionCategoryI18N>();
        }
        MissionCategoryI18Ns.Add(missionCategoryI18N);
    }
    
    public bool IsDeleted { get; set; }

}