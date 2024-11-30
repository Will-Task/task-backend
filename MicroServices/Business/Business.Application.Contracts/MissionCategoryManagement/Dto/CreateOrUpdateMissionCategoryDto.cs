using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.MissionCategoryManagement.Dto;

public class CreateOrUpdateMissionCategoryDto : EntityDto<Guid?>
{
    
    [Required]
    public string MissionCategoryName { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    /// <summary>
    /// 父類別 Id
    /// </summary>
    
    public Guid? ParentId { get; set; }
    
    public int Lang { get; set; }
}