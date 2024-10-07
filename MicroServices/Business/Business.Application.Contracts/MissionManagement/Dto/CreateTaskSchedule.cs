using System;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class CreateTaskSchedule : EntityDto<Guid>
{
    // 定時任務規則
    // 1 -> weekly 2 -> daily 3-> monthly
    public int Frequency { get; set; }
    
    // 提醒時間設定
    public int RemindHour { get; set; }
}