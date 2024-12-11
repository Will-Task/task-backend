using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications.CategoryView;

public class ParentCategorySpecification : Specification<MissionCategoryView>
{
    private Guid? _parentId;

    public ParentCategorySpecification(Guid? parentId)
    {
        _parentId = parentId;
    }

    public override Expression<Func<MissionCategoryView, bool>> ToExpression()
    {
        return x => x.ParentId == _parentId;
    }
}