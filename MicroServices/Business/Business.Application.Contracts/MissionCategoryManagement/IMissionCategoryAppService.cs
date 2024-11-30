using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.MissionCategoryManagement.Dto;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Business.MissionCategoryManagement;

public interface IMissionCategoryAppService : IApplicationService
{
    /// <summary>
    /// 查看當前使用者所建立的任務類別
    /// </summary>
    Task<PagedResultDto<MissionCategoryViewDto>> GetAll(string name, Guid? teamId, Guid? parentId, int page,
        int pageSize, bool allData);

    /// <summary>
    /// 查看特定任務類別
    /// </summary>
    Task<MissionCategoryViewDto> Get(Guid id, int lang);

    /// <summary>
    /// 新增或修改當前使用者所建立的任務類別
    /// </summary>
    Task<MissionCategoryI18Dto> DataPost(CreateOrUpdateMissionCategoryDto input);

    /// <summary>
    /// 刪除當前使用者所建立的任務類別
    /// </summary>
    Task Delete(Guid id, int lang);
}