using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

namespace Business.MissionTagManagement.Dto;

public class CreateOrUpdateMissionTagDto : EntityDto<Guid?>
{
    
    [Required]
    public string MissionTagName { get; set; }
    
    public int Lang { get; set; }
    
}