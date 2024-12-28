using System;
using System.ComponentModel.DataAnnotations;
using Business.Core.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models
{
    public class TeamInvitation : AuditedAggregateRoot<Guid>, ISoftDelete
    {
        [Required]
        public Guid TeamId { get; set; }

        /// <summary>
        /// 邀請人名稱
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid InvitedUserId { get; set; }

        [Required] 
        public Invitation State { get; set; } = Invitation.Pending;

        public DateTime? ResponseTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}
