using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Models;
using Business.Specifications.MissionI18N;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Business.DomainServices;

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

    /// <summary>
    /// 該任務本體是否存在其他語系的資料
    /// </summary>
    public async Task<Boolean> IsExistMissionI18NAsync(Guid id)
    {
        var count = await _repository.CountAsync(new MissionI18NSpecification(id));
        return count != 0;
    }

    /// <summary>
    /// 取得所有語系的任務資料
    /// </summary>
    public async Task GetAllLangDataOfMissionAsync(List<MissionView> missions)
    {
        var queryMission = await _repository.GetQueryableAsync();
        // 指定語系 -> 中文 -> 任一語系 ， 符合規則的第一筆
        var defaultMission = queryMission.OrderBy(x => x.Lang == 1 ? 0 : x.Lang)
            .ThenBy(x => x.Lang).ToList().GroupBy(x => x.MissionId)
            .ToDictionary(g => g.Key, x => x.First());
        
        foreach (var mission in missions)
        {
            if (mission.MissionName.IsNullOrEmpty())
            {
                mission.MissionName = defaultMission[mission.MissionId].MissionName;
            }

            if (mission.MissionDescription.IsNullOrEmpty())
            {
                mission.MissionDescription = defaultMission[mission.MissionId].MissionDescription;
            }

        }
    }
}