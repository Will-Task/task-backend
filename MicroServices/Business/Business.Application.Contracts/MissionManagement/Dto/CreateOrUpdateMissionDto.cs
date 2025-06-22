using System;
using System.ComponentModel.DataAnnotations;
using Business.Enums;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Timing;
using Volo.Abp.Validation;

namespace Business.MissionManagement.Dto;

public class CreateOrUpdateMissionDto : EntityDto<Guid?>
{
    /// <summary>
    /// 任務重要程度
    /// </summary>
    [Required]
    public int MissionPriority { get; set; }
    
    /// <summary>
    /// 父任務為空，子任務為屬於哪個父任務
    /// </summary>
    public Guid? ParentMissionId { get; set; }
    
    [Required]
    public Guid MissionCategoryId { get; set; }

    [Required]
    public DateTime MissionStartTime { get; set; }

    [Required] 
    public DateTime MissionEndTime { get; set; }
    
    public DateTime? MissionFinishTime { get; set; }

    /// <summary>
    /// 用來設置要在結束前多久提醒
    /// </summary>
    public int? MissionBeforeEnd { get; set; }

    [Required]
    public MissionState MissionState { get; set; }
    
    /// <summary>
    ///  定時任務排成
    /// null -> 不會重複
    /// 1 -> weekly
    /// 2 -> daily
    /// 3 -> monthly
    /// </summary>
    public int? Schedule { get; set; }
    
    /// <summary>
    /// 定時任務的主要源頭Id
    /// </summary>
    public Guid? ScheduleMissionId { get; set; }
    
    /// <summary>
    /// 所屬哪個Team的任務
    /// </summary>
    public Guid? TeamId { get; set; }
    
    /// <summary>
    /// 任務名稱
    /// </summary>
    public string? MissionName { get; set; }

    /// <summary>
    /// 任務說明
    /// </summary>
    public string? MissionDescription { get; set; }

    /// <summary>
    /// 對應語系
    /// </summary>
    public int Lang { get; set; }
    
    public void SetMissionState(DateTime currentTime)
    {
        if (this.MissionState == MissionState.COMPLETED && MissionFinishTime == null)
        {
            MissionFinishTime = currentTime;
        }
    }
    
    public void CheckStartLessEnd()
    {
        if (this.MissionStartTime > this.MissionEndTime)
        {
            throw new AbpValidationException("任務開始時間大於結果時間");
        }
    }
}