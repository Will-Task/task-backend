using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.MissionCategoryManagement.Dto;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Business.MissionCategoryManagement;

[Authorize]
[RemoteService(false)]
public class MissionCategoryAppService : ApplicationService, IMissionCategoryAppService
{
    private (
        IRepository<MissionCategory, Guid> MissionCategory,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18N,
        IRepository<MissionCategoryView> MissionCategoryView
        ) _repositorys;

    private readonly ILogger<MissionCategoryAppService> _logger;

    public MissionCategoryAppService(IRepository<MissionCategory, Guid> MissionCategory,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18N,
        IRepository<MissionCategoryView> MissionCategoryView,
        ILogger<MissionCategoryAppService> logger)
    {
        _repositorys = (MissionCategory, MissionCategoryI18N, MissionCategoryView);
        _logger = logger;
    }

    /// <summary>
    /// 查看當前使用者所建立的任務類別
    /// </summary>
    public async Task<PagedResultDto<MissionCategoryViewDto>> GetAll()
    {
        // 1. 取出當前使用者Id
        var currentUserId = CurrentUser.Id;
        var missionCategoryViews =
            await _repositorys.MissionCategoryView.GetListAsync(mcv => mcv.UserId == currentUserId);
        var dtos = ObjectMapper.Map<List<MissionCategoryView>, List<MissionCategoryViewDto>>(missionCategoryViews);

        return new PagedResultDto<MissionCategoryViewDto>(dtos.Count, dtos);
    }

    /// <summary>
    /// 查看特定任務類別
    /// </summary>
    public async Task<MissionCategoryViewDto> Get(Guid id)
    {
        var category = await _repositorys.MissionCategoryView.GetAsync(mc => mc.MissionCategoryId == id);
        return ObjectMapper.Map<MissionCategoryView, MissionCategoryViewDto>(category);
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
            var missionCategory = await _repositorys.MissionCategory.GetAsync(input.Id.Value);
            await _repositorys.MissionCategory.EnsureCollectionLoadedAsync(missionCategory, c
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

            await _repositorys.MissionCategory.UpdateAsync(missionCategory);
        }
        // 新增categeory
        else
        {
            var newMissionCategory = ObjectMapper.Map<CreateOrUpodateMissionCategoryDto, MissionCategory>(input);
            newMissionCategory.UserId = CurrentUser.Id;
            newMissionCategory.MissionCategoryI18Ns = new List<MissionCategoryI18N>();

            newMissionCategory.MissionCategoryI18Ns.Add(newMissionCategoryI18N);
            await _repositorys.MissionCategory.InsertAsync(newMissionCategory, autoSave: true);
            input.Id = newMissionCategory.Id;
        }

        return ObjectMapper.Map<CreateOrUpodateMissionCategoryDto, MissionCategoryI18Dto>(input);
    }

    /// <summary>
    /// 刪除當前使用者所建立的任務類別
    /// </summary>
    public async Task Delete(List<Guid> ids)
    {
        try
        {
            var categories =
                await _repositorys.MissionCategoryI18N.GetListAsync(mc => ids.Contains(mc.MissionCategoryId));
            // 1. 刪除任務類別I18N
            await _repositorys.MissionCategoryI18N.DeleteManyAsync(categories, autoSave: true);

            foreach (var id in ids)
            {
                var query = await _repositorys.MissionCategoryI18N.GetQueryableAsync();
                query = query.Where(mc => mc.MissionCategoryId == id);
                var count = await query.CountAsync();

                // 2. 沒有關聯I18N刪除
                if (count == 0)
                {
                    await _repositorys.MissionCategory.DeleteAsync(id);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation("=========================刪除任務類別失敗====================" + e.StackTrace.ToString());
        }
    }
}