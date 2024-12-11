using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications;

public class UserMissionSpecification : Specification<MissionView>
{
    private Guid? _currentUserId;
    
    public UserMissionSpecification(Guid? currentUserId)
    {
        _currentUserId = currentUserId;
    }
    
    public override Expression<Func<MissionView, bool>> ToExpression()
    {
        return x => x.UserId == _currentUserId;
    }
}