using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.MissionManagement.Dto;
using Business.MissionTagManagement.Dto;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Business.MissionTagManagement;

[Authorize]
[RemoteService(false)]
public class MissionTagAppService : ApplicationService, IMissionTagAppService
{
    private readonly IRepository<MissionTag, Guid> _MissionTagRepository;
    private readonly IRepository<MissionTagI18N, Guid> _MissionTagI18NRepository;
    private readonly IRepository<Mission, Guid> _MissionRepository;
    private readonly IRepository<MissionView> _MissionViewRepository;

    public MissionTagAppService(IRepository<MissionTag, Guid> missionTagRepository,
        IRepository<MissionTagI18N, Guid> missionTagI18NRepository,
        IRepository<Mission, Guid> MissionRepository,
        IRepository<MissionView> MissionViewRepository)
    {
        _MissionTagRepository = missionTagRepository;
        _MissionTagI18NRepository = missionTagI18NRepository;
        _MissionRepository = MissionRepository;
        _MissionViewRepository = MissionViewRepository;
    }

    /// <summary>
    /// 創建/修改標籤
    /// </summary>
    public async Task<MissionTagDto> DataPost(CreateOrUpdateMissionTagDto input)
    {
        // 建立新tagI18N
        var newMissionTagI18N = ObjectMapper.Map<CreateOrUpdateMissionTagDto, MissionTagI18N>(input);
        if (input.Id.HasValue)
        {
            newMissionTagI18N.MissionTagId = input.Id.Value;
        }

        // 判斷新增或修改標籤
        // 修改(新增新語系標籤)
        if (input.Id.HasValue)
        {
            // 找存在的tag
            var missionTag = await _MissionTagRepository.GetAsync(t => t.Id == input.Id);
            // 加載關聯tag I18N
            await _MissionTagRepository.EnsureCollectionLoadedAsync(missionTag, t => t.MissionTagI18Ns);
            var missionTagMissionTagI18Ns = missionTag.MissionTagI18Ns;
            var missionTagI18N =
                missionTagMissionTagI18Ns.FirstOrDefault(tn => tn.MissionTagId == input.Id && tn.Lang == input.Lang);

            // 新增新語系標籤
            if (missionTagI18N == null)
            {
                missionTagMissionTagI18Ns.Add(newMissionTagI18N);
            }
            // 修改舊標籤
            else
            {
                missionTagI18N.MissionTagName = input.MissionTagName;
                missionTagI18N.Lang = input.Lang;
            }

            await _MissionTagRepository.UpdateAsync(missionTag);
        }
        // 新增標籤
        else
        {
            // 1. 抓當前使用者
            var currentUserId = CurrentUser.Id;

            // 2. 建立新tag
            var newMissionTag = new MissionTag { UserId = currentUserId };
            newMissionTag.MissionTagI18Ns = new List<MissionTagI18N>();
            newMissionTag.MissionTagI18Ns.Add(newMissionTagI18N);
            
            await _MissionTagRepository.InsertAsync(newMissionTag, autoSave: true);
            input.Id = newMissionTag.Id;
        }

        return ObjectMapper.Map<CreateOrUpdateMissionTagDto, MissionTagDto>(input);
    }

    /// <summary>
    /// 根據標籤篩選任務
    /// </summary>
    public async Task<IEnumerable<MissionViewDto>> FilterTag(Guid id)
    {
        // 取得對應tag & 他所放到的mission
        var missionTag = await _MissionTagRepository.GetAsync(id);
        await _MissionTagRepository.EnsureCollectionLoadedAsync(missionTag, t => t.Missions);
        var ids = missionTag.Missions.Select(m => m.Id);

        var missionViews = await _MissionViewRepository.GetListAsync(mv => ids.Contains(mv.MissionId));

        return ObjectMapper.Map<List<MissionView>,List<MissionViewDto>>(missionViews);
    }

    /// <summary>
    /// 為任務添加標籤
    /// </summary>
    public async Task AddTagToMission(Guid id, Guid missionId)
    {
        await EditTagOnMission(id, missionId, 1);
    }
    
    /// <summary>
    /// 從任務上移除標籤
    /// </summary>
    public async Task RemoveTagFromMission(Guid id, Guid missionId)
    {
        await EditTagOnMission(id, missionId, 0);
    }

    /// <summary>
    /// 為任務添加or移除標籤
    /// </summary>
    /// <param name="opt">新增(1)還是刪除(0)</param>
    private async Task EditTagOnMission(Guid id, Guid missionId, int opt)
    {
        // 1. 撈tag & 他所放到的mission
        var missionTag = await _MissionTagRepository.GetAsync(id);
        await _MissionTagRepository.EnsureCollectionLoadedAsync(missionTag, t => t.Missions);

        // 2. 撈mission
        var mission = await _MissionRepository.GetAsync(missionId);

        // 3. 為任務加上標籤
        if (opt == 1)
        {
            missionTag.Missions.Add(mission);
        }
        // 移除任務上標籤
        else
        {
            missionTag.Missions.Remove(mission);
        }
        await _MissionTagRepository.UpdateAsync(missionTag);
    }

    /// <summary>
    /// 刪除標籤
    /// </summary>
    public async Task Delete(Guid id, int lang)
    {
        await _MissionTagI18NRepository.DeleteAsync(tn => tn.MissionTagId == id && tn.Lang == lang);
    }
}