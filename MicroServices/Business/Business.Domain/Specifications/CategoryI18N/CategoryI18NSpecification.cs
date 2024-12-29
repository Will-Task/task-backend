using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications.CategoryI18N;

public class CategoryI18NSpecification : Specification<MissionCategoryI18N>
{
    private Guid _categoryId;

    private int _lang;

    public CategoryI18NSpecification(Guid categoryId)
    {
        _categoryId = categoryId;
    }

    public CategoryI18NSpecification(Guid categoryId, int lang)
    {
        _categoryId = categoryId;
        _lang = lang;
    }

    public override Expression<Func<MissionCategoryI18N, bool>> ToExpression()
    {
        return x => x.MissionCategoryId == _categoryId && x.Lang == _lang;
    }
}