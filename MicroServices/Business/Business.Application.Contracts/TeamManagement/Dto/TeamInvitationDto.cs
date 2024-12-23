using Business.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.TeamManagement.Dto;

public class TeamInvitationDto : EntityDto<Guid?>
{
    [Required]
    public Guid TeamId { get; set; }
    
    public string TeamName { get; set; }
    
    public string Description { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    public string UserName { get; set; }
    
    [Required]
    public Guid InvitedUserId { get; set; }
    
    public string InvitedUserName { get; set; }
    
    [Required]
    public Invitation State { get; set; }
    
    public DateTime? ResponseTime { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public int IsShow { get; set; }
}