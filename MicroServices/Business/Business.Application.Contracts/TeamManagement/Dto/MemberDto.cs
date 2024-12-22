using System;
using Volo.Abp.Application.Dtos;

namespace Business.TeamManagement.Dto;

public class MemberDto : EntityDto<Guid>
{
    public string UserName { get; set; }
    
    public string Email { get; set; }
}