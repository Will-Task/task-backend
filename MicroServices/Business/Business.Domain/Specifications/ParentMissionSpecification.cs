using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications;

public class ParentMissionSpecification : Specification<MissionView>
{
    private Guid? _parentId;

    public ParentMissionSpecification(Guid? parentId)
    {
        _parentId = parentId;
    }

    public override Expression<Func<MissionView, bool>> ToExpression()
    {
        return x => x.ParentMissionId == _parentId;
    }
}