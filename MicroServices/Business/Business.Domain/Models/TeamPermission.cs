using System;
using Business.Core.Enums;
using Volo.Abp.Domain.Entities;

namespace Business.Models
{
    public class TeamPermission : Entity
    {
        public Guid UserId { get; set; }

        public Guid TeamId { get; set; }

        public TeamPermissions Permission { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { UserId, TeamId, Permission };
        }
    }
}
