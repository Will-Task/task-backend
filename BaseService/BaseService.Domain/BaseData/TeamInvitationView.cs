using System;
using BaseService.Enums;
using Volo.Abp.Domain.Entities;

namespace BaseService.BaseData;

public class TeamInvitationView : Entity<Int64>
{
    public Guid InvitationId { get; set; }
    
    public Guid TeamId { get; set; }
    
    public string TeamName { get; set; }
    
    public Guid UserId { get; set; }
    
    public string UserName { get; set; }
    
    public string InvitedUserName { get; set; }
    
    public Guid InvitedUserId { get; set; }
    
    public string Description { get; set; }
    
    public Invitation State { get; set; }
    
    public DateTime? ResponseTime { get; set; }
    
    public DateTime CreationTime { get; set; }
}
