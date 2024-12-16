using System;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class CreateMissionAttachmentDto
{
    public Guid MissionId { get; set; }

    public Guid? TeamId { get; set; }

    public string Name { get; set; }
 
    public int FileIndex { get; set; }

    public string Note { get; set; }
}