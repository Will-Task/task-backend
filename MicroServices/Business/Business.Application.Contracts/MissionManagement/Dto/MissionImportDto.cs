using System;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

/// <summary>
/// For 下載範本 、 匯入 、 匯入檢查
/// </summary>
public class MissionImportDto : EntityDto<Int64?>
{
    public Guid? TeamId { get; set; }
    
    public Guid? ParentMissionId { get; set; }
    
    public string MissionName { get; set; }
    
    public Guid MissionCategoryId { get; set; }
    
    public string MissionDescription { get; set; }
    
    public string MissionCategoryName { get; set; }
    
    public DateTime MissionStartTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }
    
    public DateTime MissionEndTime { get; set; }
    
    public int? MissionBeforeEnd { get; set; }
    
    public int MissionPriority { get; set; }

    public MissionState MissionState { get; set; }
    
    public int Lang { get; set; }
}