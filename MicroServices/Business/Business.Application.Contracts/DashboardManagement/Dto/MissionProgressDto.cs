using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionProgressDto : EntityDto<Guid?>
{
    public string CategoryName { get; set; }
    
    /// <summary>
    /// 該類別下所有任務數量
    /// </summary>
    public int Total { get; set; }
    
    /// <summary>
    /// 該類別下完成任務數量
    /// </summary>
    public int Finish { get; set; }
    
    public int Lang { get; set; }
}