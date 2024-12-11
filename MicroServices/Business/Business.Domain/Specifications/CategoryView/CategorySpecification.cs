using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications.CategoryView;

public class CategorySpecification : Specification<MissionCategoryView>
{
    private Guid _categoryId;

    private int? _lang;

    public CategorySpecification(Guid categoryId)
    {
        _categoryId = categoryId;
    }

    public CategorySpecification(Guid categoryId, int lang)
    {
        _categoryId = categoryId;
        _lang = lang;
    }

    public override Expression<Func<MissionCategoryView, bool>> ToExpression()
    {
        return x => x.MissionCategoryId == _categoryId && (!_lang.HasValue || x.Lang == _lang);
    }
}