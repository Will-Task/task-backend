using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Business.Specifications.MissionView;

public class MissionSpecification : Specification<Models.MissionView>
{
    public Guid _id;

    public MissionSpecification(Guid id)
    {
        _id = id;
    }

    public override Expression<Func<Models.MissionView, bool>> ToExpression()
    {
        return x => x.MissionId == _id;
    }
}