using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.MissionCategoryManagement.Dto;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Business.MissionCategoryManagement;

[Authorize]
[RemoteService(false)]
public class MissionCategoryAppService : ApplicationService, IMissionCategoryAppService
{
    private readonly IRepository<MissionCategory, Guid> _MissionCategoryRepository;
    private readonly IRepository<MissionCategoryI18N, Guid> _MissionCategoryI18NRepository;
    private readonly IRepository<MissionCategoryView> _MissionCategoryViewRepository;

    public MissionCategoryAppService(IRepository<MissionCategory, Guid> MissionCategoryRepository,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18NRepository,
        IRepository<MissionCategoryView> MissionCategoryViewRepository)
    {
        _MissionCategoryRepository = MissionCategoryRepository;
        _MissionCategoryI18NRepository = MissionCategoryI18NRepository;
        _MissionCategoryViewRepository = MissionCategoryViewRepository;
    }

    /// <summary>
    /// 查看當前使用者所建立的任務類別
    /// </summary>
    public async Task<IEnumerable<MissionCategoryViewDto>> GetAll()
    {

        // 1. 取出當前使用者Id
        var currentUserId = CurrentUser.Id;
        var missionCategoryViews = await _MissionCategoryViewRepository.GetListAsync(mcv => mcv.UserId == currentUserId);
        return ObjectMapper.Map<List<MissionCategoryView>,List<MissionCategoryViewDto>>(missionCategoryViews);
    }

    /// <summary>
    /// 新增或修改當前使用者所建立的任務類別
    /// </summary>
    public async Task<MissionCategoryI18Dto> DataPost(CreateOrUpodateMissionCategoryDto input)
    {
        var newMissionCategoryI18N = new MissionCategoryI18N()
        {
            MissionCategoryName = input.MissionCategoryName,
            Lang = input.Lang
        };
        if (input.Id.HasValue)
        {
            newMissionCategoryI18N.MissionCategoryId = input.Id.Value;
        }

        // 1. 判斷進行修改還是刪除
        // 修改(修改category或新增categoryI18N)
        if (input.Id.HasValue)
        {
            // 取出category和對應I18N(若有的話)
            var missionCategory = await _MissionCategoryRepository.GetAsync(input.Id.Value);
            await _MissionCategoryRepository.EnsureCollectionLoadedAsync(missionCategory, c
                => c.MissionCategoryI18Ns);

            var missionCategoryI18Ns = missionCategory.MissionCategoryI18Ns;
            var missionCategoryI18N = missionCategoryI18Ns.FirstOrDefault(cn =>
                cn.MissionCategoryId == input.Id && cn.Lang == input.Lang);

            // 修改categoryI18N
            if (missionCategoryI18N != null)
            {
                missionCategoryI18N.MissionCategoryName = input.MissionCategoryName;
                missionCategoryI18N.Lang = input.Lang;
            }
            // 新增categoryI18N
            else
            {
                missionCategoryI18Ns.Add(newMissionCategoryI18N);
            }

            await _MissionCategoryRepository.UpdateAsync(missionCategory);
        }
        // 新增categeory
        else
        {
            var newMissionCategory = ObjectMapper.Map<CreateOrUpodateMissionCategoryDto, MissionCategory>(input);
            newMissionCategory.UserId = CurrentUser.Id;
            newMissionCategory.MissionCategoryI18Ns = new List<MissionCategoryI18N>();
            
            newMissionCategory.MissionCategoryI18Ns.Add(newMissionCategoryI18N);
            await _MissionCategoryRepository.InsertAsync(newMissionCategory,autoSave:true);
            input.Id = newMissionCategory.Id;
        }

        return ObjectMapper.Map<CreateOrUpodateMissionCategoryDto,MissionCategoryI18Dto>(input);
    }

    /// <summary>
    /// 刪除當前使用者所建立的任務類別
    /// </summary>
    public async Task Delete(Guid id)
    {
        // 1. 刪除任務類別I18N
        await _MissionCategoryI18NRepository.DeleteAsync(id);
        var num = await _MissionCategoryI18NRepository.GetCountAsync();
        
        // 2. 沒有關聯I18N刪除
        if (num == 0)
        {
            await _MissionCategoryRepository.DeleteAsync(id);
        }
    }
}