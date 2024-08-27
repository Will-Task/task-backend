using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.MissionCategoryManagement.Dto;

public class MissionCategoryViewDto: EntityDto<Int64>
{
    [Required]
    public Guid MissionCategoryId { get; set; }
    
    public Guid? UserId { get; set; }
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    public int Lang { get; set; }
}