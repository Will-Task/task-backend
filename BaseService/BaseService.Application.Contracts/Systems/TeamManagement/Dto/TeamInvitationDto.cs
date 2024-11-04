using System;
using System.ComponentModel.DataAnnotations;
using BaseService.Enums;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.TeamManagement.Dto;

public class TeamInvitationDto : EntityDto<Guid?>
{
    [Required]
    public Guid TeamId { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid InvitedUserId { get; set; }
    
    [Required]
    public Invitation State { get; set; }
    
    public DateTime? ResponseTime { get; set; }
    
    public bool IsDeleted { get; set; }
}