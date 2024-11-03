using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.TeamManagement.Dto;

public class MemberDetailDto : EntityDto<Guid?>
{
    public string UserName { get; set; }
}