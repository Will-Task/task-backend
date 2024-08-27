using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.MissionManagement.Dto;
using Business.MissionTagManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.MissionTagManagement;

public interface IMissionTagAppService : IApplicationService
{
    /// <summary>
    /// 創建/修改標籤
    /// </summary>
    Task<MissionTagDto> DataPost(CreateOrUpdateMissionTagDto input);

    /// <summary>
    /// 根據標籤篩選任務
    /// </summary>
    Task<IEnumerable<MissionViewDto>> FilterTag(Guid id);

    /// <summary>
    /// 為任務添加標籤
    /// </summary>
    Task AddTagToMission(Guid id, Guid missionId);

    /// <summary>
    /// 從任務上移除標籤
    /// </summary>
    Task RemoveTagFromMission(Guid id, Guid missionId);

    /// <summary>
    /// 移除標籤
    /// </summary>
    Task Delete(Guid id, int lang);
}