using System;
using System.Linq.Expressions;
using Business.Models;
using Volo.Abp.Specifications;

namespace Business.Specifications;

public class MissionI18NSpecification : Specification<MissionI18N>
{
    public Guid _id;
    public int? _lang;

    public MissionI18NSpecification(Guid id, int lang)
    {
        _id = id;
        _lang = lang;
    }

    public MissionI18NSpecification(Guid id)
    {
        _id = id;
    }

    public override Expression<Func<MissionI18N, bool>> ToExpression()
    {
        return x => x.MissionId == _id && (!_lang.HasValue || x.Lang == _lang);
    }
}