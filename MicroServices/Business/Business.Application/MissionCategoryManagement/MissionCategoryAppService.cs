using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.MissionCategoryManagement.Dto;
using Business.Models;
using Business.Permissions;
using Business.Specifications.CategoryI18N;
using Business.Specifications.CategoryView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Specifications;

namespace Business.MissionCategoryManagement;

[Authorize(BusinessPermissions.TaskCategory.Default)]
[RemoteService(false)]
public class MissionCategoryAppService : ApplicationService, IMissionCategoryAppService
{
    private readonly (
        IRepository<MissionCategory, Guid> MissionCategory,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18N,
        IRepository<MissionCategoryView> MissionCategoryView,
        IRepository<Mission> Mission
        ) _repositorys;

    private readonly ILogger<MissionCategoryAppService> _logger;

    public MissionCategoryAppService(IRepository<MissionCategory, Guid> missionCategory,
        IRepository<MissionCategoryI18N, Guid> missionCategoryI18N,
        IRepository<MissionCategoryView> missionCategoryView,
        IRepository<Mission> mission,
        ILogger<MissionCategoryAppService> logger)
    {
        _repositorys = (missionCategory, missionCategoryI18N, missionCategoryView, mission);
        _logger = logger;
    }

    #region CRUD方法

    /// <summary>
    /// 查看當前使用者所建立的任務類別
    /// </summary>
    public async Task<PagedResultDto<MissionCategoryViewDto>> GetAll(string name, Guid? teamId, Guid? parentId,
        int page,
        int pageSize, bool allData)
    {
        try
        {
            // 1. 取出當前使用者Id
            var currentUserId = CurrentUser.Id;
            var query = await _repositorys.MissionCategoryView.GetQueryableAsync();
            query = query.Where((new TeamCategorySpecification(teamId)
                        .Or(new UserCategorySpecification(currentUserId)))
                    .And(new ParentCategorySpecification(parentId)).ToExpression())
                .WhereIf(!name.IsNullOrEmpty(), x => x.MissionCategoryName.Contains(name));

            var count = await query.CountAsync();
            // 拿全部or分頁
            var categories = allData
                ? await query.ToListAsync()
                : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtos = ObjectMapper.Map<List<MissionCategoryView>, List<MissionCategoryViewDto>>(categories);

            return new PagedResultDto<MissionCategoryViewDto>(count, dtos);
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"========================= 獲取多筆任務類別失敗 !!!!!!!!!==================== {e.StackTrace.ToString()}");
            throw new BusinessException("獲取多筆任務類別失敗");
        }
    }

    /// <summary>
    /// 查看特定任務類別
    /// </summary>
    public async Task<MissionCategoryViewDto> Get(Guid id, int lang)
    {
        try
        {
            var category =
                await _repositorys.MissionCategoryView.GetAsync(new CategorySpecification(id, lang));
            return ObjectMapper.Map<MissionCategoryView, MissionCategoryViewDto>(category);
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"========================= 獲取單筆任務類別失敗 !!!!!!!!!==================== {e.StackTrace.ToString()}");
            throw new BusinessException("獲取單筆任務類別失敗");
        }
    }

    /// <summary>
    /// 新增或修改當前使用者所建立的任務類別
    /// </summary>
    public async Task<MissionCategoryI18Dto> DataPost(CreateOrUpdateMissionCategoryDto input)
    {
        try
        {
            var currentUserId = CurrentUser.Id;
            var newI18N = ObjectMapper.Map<CreateOrUpdateMissionCategoryDto, MissionCategoryI18N>(input);

            // 修改(修改category或新增categoryI18N)
            if (input.Id.HasValue)
            {
                // 連結I18N和主體
                newI18N.MissionCategoryId = input.Id.Value;
                // 取出category和對應I18N(若有的話)
                var category = await _repositorys.MissionCategory.GetAsync(input.Id.Value);

                var categoryI18Ns = category.MissionCategoryI18Ns;
                var categoryI18N = categoryI18Ns.FirstOrDefault(cn =>
                    cn.MissionCategoryId == input.Id && cn.Lang == input.Lang);

                // 修改categoryI18N
                if (categoryI18N != null)
                {
                    ObjectMapper.Map(input, category);
                    categoryI18N.MissionCategoryName = input.MissionCategoryName;
                }
                // 新增categoryI18N
                else
                {
                    category.AddCategoryI18N(newI18N);
                }

                await _repositorys.MissionCategory.UpdateAsync(category);
            }
            // 新增categeory
            else
            {
                var newCategory = ObjectMapper.Map<CreateOrUpdateMissionCategoryDto, MissionCategory>(input);
                newCategory.UserId = currentUserId;
                newCategory.AddCategoryI18N(newI18N);

                await _repositorys.MissionCategory.InsertAsync(newCategory, autoSave: true);
            }

            return ObjectMapper.Map<CreateOrUpdateMissionCategoryDto, MissionCategoryI18Dto>(input);
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"======================== 任務類別新增失敗!!!!!!!!!! ====================================== {e.StackTrace.ToString()}");
            throw new BusinessException("任務類別新增失敗");
        }
    }

    /// <summary>
    /// 刪除當前使用者所建立的任務類別
    /// </summary>
    public async Task Delete(Guid id, int lang)
    {
        try
        {
            // 判斷是否任務是此類別(不管語系)
            var queryMission = await _repositorys.Mission.GetQueryableAsync();
            var missionCount = await queryMission.Where(x => x.MissionCategoryId == id).CountAsync();
            if (missionCount != 0)
            {
                throw new BusinessException("有任務根據此任務類別，刪除失敗!!");
            }

            var query = await _repositorys.MissionCategoryI18N.GetQueryableAsync();
            // 1. 刪除任務類別I18N
            await _repositorys.MissionCategoryI18N.DeleteAsync(new CategoryI18NSpecification(id, lang),
                autoSave: true);

            query = query.Where(new CategoryI18NSpecification(id));
            var count = await query.CountAsync();

            // 2. 沒有關聯I18N刪除
            if (count == 0)
            {
                await _repositorys.MissionCategory.DeleteAsync(id);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"========================= 刪除任務類別失敗 !!!!!!!!!==================== {e.StackTrace.ToString()}");
            throw new BusinessException("刪除任務類別失敗");
        }
    }

    #endregion CRUD方法
}