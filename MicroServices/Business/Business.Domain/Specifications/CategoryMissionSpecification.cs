using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications;

public class CategoryMissionSpecification : Specification<MissionView>
{
    /// <summary>
    /// 搜尋時條件可能為空
    /// </summary>
    private Guid? _categoryId;

    public CategoryMissionSpecification(Guid? categoryId)
    {
        _categoryId = categoryId;
    }
    
    public override Expression<Func<MissionView, bool>> ToExpression()
    {
        return x => !_categoryId.HasValue ||  x.MissionCategoryId == _categoryId.Value;
    }
}