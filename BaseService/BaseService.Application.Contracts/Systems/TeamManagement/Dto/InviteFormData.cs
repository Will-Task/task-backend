using System;
using System.Collections.Generic;

namespace BaseService.Systems.TeamManagement.Dto;

public class InviteFormData
{
    public List<Guid> UserIds { get; set; }
    
    public Guid TeamId { get; set; }
}