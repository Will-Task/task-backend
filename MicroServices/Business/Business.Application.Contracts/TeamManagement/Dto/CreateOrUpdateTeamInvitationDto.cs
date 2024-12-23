using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.TeamManagement.Dto;

public class CreateOrUpdateTeamInvitationDto : EntityDto<Guid?>
{
    [Required] 
    public Guid TeamId { get; set; }
    
    [Required]
    public Guid InvitedUserId { get; set; }
}