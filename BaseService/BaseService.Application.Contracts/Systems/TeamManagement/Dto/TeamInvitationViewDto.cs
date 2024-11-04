using System;
using BaseService.Enums;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.TeamManagement.Dto;

public class TeamInvitationViewDto : EntityDto<Int64>
{
    public Guid InvitationId { get; set; }
    
    public Guid TeamId { get; set; }
    
    public string TeamName { get; set; }
    
    /// <summary>
    /// 邀請人
    /// </summary>
    public Guid UserId { get; set; }
    
    public string UserName { get; set; }
    
    /// <summary>
    /// 受邀人
    /// </summary>
    
    public string InvitedUserName { get; set; }
    
    public Guid InvitedUserId { get; set; }
    
    public string Description { get; set; }
    
    public Invitation State { get; set; }
    
    public DateTime? ResponseTime { get; set; }
    
    /// <summary>
    /// 邀請發出時間
    /// </summary>
    public DateTime CreationTime { get; set; }
    
    // 是否顯示前端取消按鈕
    public int IsShow { get; set; }
}