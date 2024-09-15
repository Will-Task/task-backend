using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Business.MissionManagement.Dto;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Business.MissionManagement;

public interface IMissionAppService : IApplicationService
{
    /// <summary>
    /// 獲取父任務下的子任務(多個)
    /// </summary>
    Task<IEnumerable<MissionViewDto>> GetSubMission(Guid id);

    /// <summary>
    /// 查詢所有父任務(多個)
    /// </summary>
    Task<IEnumerable<MissionViewDto>> GetParentMission();
    
    /// <summary>
    /// 查詢所有父任務(多個，分頁)
    /// </summary>
    Task<PagedResultDto<MissionViewDto>> GetParentMissionByPage(int page , int pageSize);
    
    /// <summary>
    /// 根據類別取得父任務
    /// </summary>
    Task<IEnumerable<MissionViewDto>> GetParentMissionByCategoryId(Guid categoryId);
    
    /// <summary>
    /// 查詢特定類別下的任務(多個)
    /// </summary>
    Task<IEnumerable<MissionViewDto>> GetMission(Guid id);
    
    /// <summary>
    /// 任務提醒通知
    /// </summary>
    Task MissionReminder();
    
    /// <summary>
    /// 設置任務提醒時間(結束時間多久前)
    /// </summary>
    Task setRemindTime(Guid id,int hour);

    /// <summary>
    /// 新增/修改任務和I18N
    /// </summary>
    Task<MissionI18NDto> DataPost(CreateOrUpdateMissionDto input);
    
    /// <summary>
    /// 變更任務狀態
    /// </summary>
    Task UpdateMissionState(Guid missionId,int state);

    /// <summary>
    /// 刪除任務(過期任務不會被刪除)
    /// </summary>
    Task Delete(Guid id, int lang);
    
    /// <summary>
    /// 刪除任務(過期任務不會被刪除，刪除某父任務下的子任務)
    /// </summary>
    Task DeleteGroup(List<Guid> subIds,Guid parentId);

    /// <summary>
    /// 範本下載
    /// </summary>
    Task<MyFileInfoDto> DNSample(string fileName,int lang);

    /// <summary>
    /// 資料匯入
    /// </summary>
    Task<IEnumerable<MissionImportDto>> ImportFile(IFormFile file,int lang);

    /// <summary>
    /// 資料匯出(子任務)
    /// </summary>
    Task<MyFileInfoDto> ExportFile(Guid parentId,int lang);
    
    /// <summary>
    /// 任務即將到期(24小時)通知
    /// </summary>
    Task IdentifyExpiringTasks();

    /// <summary>
    /// 任務報告產出Excel(1 week -> 7 days)傳送到email
    /// </summary>
    Task ExportReport();

    /// <summary>
    /// 檢查到期的任務
    /// </summary>
    Task CheckExpiredOrFinished();
}