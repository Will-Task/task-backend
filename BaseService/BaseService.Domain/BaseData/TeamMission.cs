using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace BaseService.BaseData;

public class TeamMission : Entity
{
    public Guid UserId { get; set; }
    
    public Guid TeamId { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { UserId, TeamId };
    }
}