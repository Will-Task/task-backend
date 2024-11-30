using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Business.MissionCategoryManagement.Dto;

public class MissionCategoryDto : EntityDto<Guid>
{
    // 現在可為null，之後再做修改
    public Guid? userId { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    /// <summary>
    /// 父類別 Id
    /// </summary>
    
    public Guid? ParentId { get; set; }
    
    public ICollection<MissionCategoryI18Dto> MissionCategoryI18Dtos { get; set; }
}