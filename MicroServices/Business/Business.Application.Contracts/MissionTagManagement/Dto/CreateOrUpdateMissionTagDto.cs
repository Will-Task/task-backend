using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.MissionTagManagement.Dto;

public class CreateOrUpdateMissionTagDto : EntityDto<Guid?>
{
    
    [Required]
    public string MissionTagName { get; set; }
    
    public int Lang { get; set; }
    
}