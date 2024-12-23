using System;
using Volo.Abp.Domain.Entities;

namespace Business.Models;

public class TeamMember : Entity
{
    public Guid UserId { get; set; }
    
    public Guid TeamId { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { UserId, TeamId };
    }
}