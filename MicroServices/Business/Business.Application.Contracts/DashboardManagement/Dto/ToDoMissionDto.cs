using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class ToDoMissionViewDto : EntityDto<Guid?>
{
    public string Text { get; set; }
    
    public bool Done { get; set; }
    
    public int Lang { get; set; }
}