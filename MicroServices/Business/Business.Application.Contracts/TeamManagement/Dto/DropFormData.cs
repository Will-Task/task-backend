using System;

namespace Business.TeamManagement.Dto;

public class DropFormData
{
    public Guid? UserId { get; set; }
    
    public Guid TeamId { get; set; }
}