using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.CommonManagement.Dto;
using Business.FileManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Business.MissionManagement;

public interface IMissionAppService : IApplicationService
{
    /// <summary>
    /// 新增/修改任務
    /// </summary>
    Task<MissionI18NDto> DataPost(CreateOrUpdateMissionDto input);

    /// <summary>
    /// 刪除任務(單 or 多筆)
    /// </summary>
    Task Delete(Guid id, int lang);

    /// <summary>
    /// 獲取父任務下的子任務(多個)
    /// </summary>
    Task<PagedResultDto<MissionViewDto>> GetSubMission(Guid id, int page, int pageSize, bool allData);

    /// <summary>
    /// 查詢所有父任務(多個)
    /// </summary>
    Task<PagedResultDto<MissionViewDto>> GetParentMission(int page, int pageSize, bool allData, Guid? teamId);

    /// <summary>
    /// 根據類別取得父任務
    /// </summary>
    Task<IEnumerable<MissionViewDto>> GetParentMissionByCategoryId(Guid categoryId);

    /// <summary>
    /// 查詢特定任務(單個)
    /// </summary>
    Task<MissionViewDto> Get(Guid id);

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
    Task setRemindTime(Guid id, int hour);


    /// <summary>
    /// 變更任務狀態
    /// </summary>
    Task UpdateMissionState(MissionFormData formData);


    /// <summary>
    /// 範本下載
    /// </summary>
    Task<MyFileInfoDto> DNSample(string fileName, int lang);

    /// <summary>
    /// 資料匯入檢查
    /// </summary>
    Task<IEnumerable<MissionImportDto>> ImportFileCheck(IFormFile file, int lang);

    /// <summary>
    /// 資料匯入
    /// </summary>
    Task ImportFile(List<MissionImportDto> dtos);

    /// <summary>
    /// 資料匯出(子任務)
    /// </summary>
    Task<MyFileInfoDto> ExportFile(List<Guid> parentIds, int lang);

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