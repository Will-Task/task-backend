using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications;

public class MissionSpecification : Specification<MissionView>
{
    public Guid _id;

    public MissionSpecification(Guid id)
    {
        _id = id;
    }

    public override Expression<Func<MissionView, bool>> ToExpression()
    {
        return x => x.MissionId == _id;
    }
}