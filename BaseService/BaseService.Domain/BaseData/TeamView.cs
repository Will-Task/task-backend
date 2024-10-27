using System;
using Volo.Abp.Domain.Entities;

namespace BaseService.BaseData;

public class TeamView : Entity<Int64>
{
    public Guid TeamId { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }
}