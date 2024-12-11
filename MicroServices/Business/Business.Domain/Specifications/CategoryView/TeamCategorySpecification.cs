using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications.CategoryView;

public class TeamCategorySpecification : Specification<MissionCategoryView>
{
    private Guid? _teamId;

    public TeamCategorySpecification(Guid? teamId)
    {
        _teamId = teamId;
    }
    
    public override Expression<Func<MissionCategoryView, bool>> ToExpression()
    {
        return x => x.TeamId == _teamId;
    }
}