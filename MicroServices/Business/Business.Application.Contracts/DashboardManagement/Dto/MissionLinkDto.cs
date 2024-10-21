using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionLinkDto : EntityDto<Guid?>
{ 
    public Guid Source { get; set; }
    
    public Guid Target { get; set; }

    public string Type { get; set; } = "0";
}