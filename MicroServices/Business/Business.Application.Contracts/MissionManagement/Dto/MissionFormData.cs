using System;

namespace Business.MissionManagement.Dto;

public class MissionFormData
{
    public Guid missionId { get; set; }
    public int state { get; set; }
}