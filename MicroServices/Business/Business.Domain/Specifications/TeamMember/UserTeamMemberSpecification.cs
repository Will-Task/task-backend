using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Business.Specifications.TeamMember;

public class UserTeamMemberSpecification : Specification<Models.TeamMember>
{
    private Guid _userId;

    public UserTeamMemberSpecification(Guid userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Models.TeamMember, bool>> ToExpression()
    {
        return x => x.UserId == _userId;
    }
}