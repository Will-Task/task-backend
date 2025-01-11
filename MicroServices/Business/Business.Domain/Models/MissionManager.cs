using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Business.Models;

public class MissionManager : DomainService
{
    private readonly IRepository<MissionI18N> _repository;

    public MissionManager(IRepository<MissionI18N> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 設定多國語系資料
    /// </summary>
    public async Task<Dictionary<Guid, (string MissionName, string MissionDescription)>> GetDefaultLangData()
    {
        var query = await _repository.GetQueryableAsync();
        var defaultDataMap = query.GroupBy(x => x.MissionId)
            .ToDictionary(g => g.Key, 
            x => x.OrderBy(c => c.Lang).Select(x => (x.MissionName, x.MissionDescription)).First());
        return defaultDataMap;
    }
}