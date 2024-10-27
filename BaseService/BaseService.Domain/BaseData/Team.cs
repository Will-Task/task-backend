using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace BaseService.BaseData;

public class Team : AuditedAggregateRoot<Guid>, ISoftDelete
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public bool IsDeleted { get; set; }
}