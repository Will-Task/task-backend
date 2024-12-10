using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications;

public class TeamMissionSpecification : Specification<MissionView>
{
    private Guid? _teamId;

    public TeamMissionSpecification(Guid? teamId)
    {
        _teamId = teamId;
    }
    
    public override Expression<Func<MissionView, bool>> ToExpression()
    {
        return x => x.TeamId == _teamId;
    }
}