using System;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.Models;

public class CreateMissionAttachmentDto: EntityDto<Guid?>
{
    public Guid MissionId { get; set; }
 
    public int FileIndex { get; set; }

    public string ANote { get; set; }
}