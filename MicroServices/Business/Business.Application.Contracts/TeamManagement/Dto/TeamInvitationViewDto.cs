using Business.Core.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace Business.TeamManagement.Dto;

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
    
    /// <summary>
    /// 是否顯示前端取消按鈕
    /// 1 -> 顯示接受和拒絕
    /// 2 -> 顯示取消
    /// 3 -> 都不顯示
    /// </summary>
    public int IsShow { get; set; }
}