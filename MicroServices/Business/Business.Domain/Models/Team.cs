using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace Business.Models
{
    public class Team : AuditedAggregateRoot<Guid>, ISoftDelete
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Year { get; set; }

        /// <summary>
        /// 建立團隊者資訊
        /// </summary>
        public Guid UserId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
