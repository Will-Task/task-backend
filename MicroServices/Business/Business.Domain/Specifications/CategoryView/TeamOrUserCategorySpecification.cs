using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Specifications;

namespace Business.Specifications.CategoryView
{
    public class TeamOrUserCategorySpecification : Specification<MissionCategoryView>
    {
        private Guid? TeamId { get; set; }

        private Guid? UserId { get; set; }

        public TeamOrUserCategorySpecification(Guid? _TeamId, Guid? _UserId)
        {
            TeamId = _TeamId;
            UserId = _UserId;
        }

        public override Expression<Func<MissionCategoryView, bool>> ToExpression()
        {
            // 屬於某團隊 or 不屬於團隊但屬於某個人
            return x => (TeamId.HasValue && x.TeamId == TeamId) || (!TeamId.HasValue && x.TeamId == TeamId && x.UserId == UserId);
        }
    }
}
