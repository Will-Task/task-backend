using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Business.Specifications.MissionView;

public class ParentMissionSpecification : Specification<Models.MissionView>
{
    private Guid? _parentId;

    public ParentMissionSpecification(Guid? parentId)
    {
        _parentId = parentId;
    }

    public override Expression<Func<Models.MissionView, bool>> ToExpression()
    {
        return x => x.ParentMissionId == _parentId;
    }
}