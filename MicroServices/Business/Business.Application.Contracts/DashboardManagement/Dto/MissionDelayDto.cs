using System;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto;

public class MissionDelayDto : EntityDto<Int64?>
{
    public int[] Count { get; set; } = new int[12];
}