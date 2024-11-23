using System;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class CreateMissionAttachmentDto: EntityDto<Guid?>
{
    public Guid MissionId { get; set; }
 
    public int FileIndex { get; set; }

    public string ANote { get; set; }
}