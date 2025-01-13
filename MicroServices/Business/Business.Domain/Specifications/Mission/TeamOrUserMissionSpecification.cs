using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Business.Specifications.Mission
{
    public class TeamOrUserMissionSpecification : Specification<Business.Models.Mission>
    {
        private Guid? TeamId { get; set; }

        private Guid? UserId { get; set; }

        public TeamOrUserMissionSpecification(Guid? _TeamId, Guid? _UserId)
        {
            TeamId = _TeamId;
            UserId = _UserId;
        }

        public override Expression<Func<Business.Models.Mission, bool>> ToExpression()
        {
            // 屬於某團隊 or 不屬於團隊但屬於某個人
            return x => (TeamId.HasValue && x.TeamId == TeamId) || (!TeamId.HasValue && x.TeamId == TeamId && x.UserId == UserId);
        }
    }
}
