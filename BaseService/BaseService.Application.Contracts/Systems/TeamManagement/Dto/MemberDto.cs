using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.TeamManagement.Dto;

public class MemberDto : EntityDto<int?>
{
    public Guid UserId { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }
}