using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications.CategoryView;

public class UserCategorySpecification : Specification<MissionCategoryView>
{
    private Guid? _currentUserId;
    
    public UserCategorySpecification(Guid? currentUserId)
    {
        _currentUserId = currentUserId;
    }
    
    public override Expression<Func<MissionCategoryView, bool>> ToExpression()
    {
        return x => x.UserId == _currentUserId;
    }
}