using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Models;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Business.DomainServices;

public class CategoryManager: DomainService
{
    private readonly IRepository<MissionCategoryI18N> _repository;

    public CategoryManager(IRepository<MissionCategoryI18N> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 設定多國語系資料
    /// </summary>
    public async Task<Dictionary<Guid, string>> GetDefaultLangData()
    {
        var query = await _repository.GetQueryableAsync();
        var defaultDataMap = query.GroupBy(x => x.MissionCategoryId)
            .ToDictionary(g => g.Key, 
                x => x.OrderBy(c => c.Lang).Select(x => x.MissionCategoryName).First());
        return defaultDataMap;
    }
}