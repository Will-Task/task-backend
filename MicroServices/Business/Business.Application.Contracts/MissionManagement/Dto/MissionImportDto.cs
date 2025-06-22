using System;
using Business.Enums;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

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
    
    private DateTime _MissionEndTime;
    
    public DateTime MissionEndTime { get; set; }
    
    public int? MissionBeforeEnd { get; set; }

    public int MissionPriority { get; set; } = 5;

    public MissionState MissionState { get; set; } = MissionState.TODO;
    
    public int Lang { get; set; }
    
    public void CheckStartLessEnd()
    {
        if (this.MissionStartTime > this.MissionEndTime)
        {
            throw new AbpValidationException("任務開始時間大於結果時間");
        }
    }
}