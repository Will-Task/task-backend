using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.MissionCategoryManagement.Dto;

public class MissionCategoryI18Dto : EntityDto<Guid>
{
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    public int Lang { get; set; }
}