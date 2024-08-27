using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class MissionTag: AuditedAggregateRoot<Guid> , ISoftDelete
{
    public Guid? UserId { get; set; }
        
    public bool IsDeleted { get; set; }
    
    public virtual List<MissionTagI18N> MissionTagI18Ns { get; set; }
    
    public virtual List<Mission> Missions { get; set; } 

}