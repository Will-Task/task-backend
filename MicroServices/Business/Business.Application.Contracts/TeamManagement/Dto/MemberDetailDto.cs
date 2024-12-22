using System;
using Volo.Abp.Application.Dtos;

namespace Business.TeamManagement.Dto;

public class MemberDetailDto : EntityDto<Guid?>
{
    public string UserName { get; set; }
}