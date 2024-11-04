using System;

namespace BaseService.Systems.TeamManagement.Dto;

public class DropFormData
{
    public Guid? UserId { get; set; }
    
    public Guid TeamId { get; set; }
}