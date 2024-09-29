using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class ToDoMissionViewDto : EntityDto<Guid?>
{
    public string text { get; set; }
    
    public bool done { get; set; }
}