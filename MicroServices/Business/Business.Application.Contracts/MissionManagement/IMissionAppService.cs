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
    /// 查詢特定任務(單個)
    /// </summary>
    Task<MissionViewDto> Get(Guid id);

    /// <summary>
    /// 查詢所有父任務(多個)
    /// </summary>
    Task<PagedResultDto<MissionViewDto>> GetAll(int page, int pageSize, bool allData, Guid? teamId, Guid? categoryId,
        Guid? parentId);

    /// <summary>
    /// 查詢特定類別任務總攬
    /// </summary>
    /// <param name="categoryId">任務子類別 Id</param>
    Task<List<MissionOverviewDto>> GetOverview(Guid categoryId, Guid? teamId);

    /// <summary>
    /// 任務提醒通知
    /// </summary>
    Task MissionReminder();

    /// <summary>
    /// 設置任務提醒時間(結束時間多久前)
    /// </summary>
    Task SetRemindTime(Guid id, int hour);


    /// <summary>
    /// 變更任務狀態
    /// </summary>
    Task UpdateMissionState(MissionFormData formData);


    /// <summary>
    /// 範本下載
    /// </summary>
    Task<BlobDto> DNSample(Guid parentId, string code);

    /// <summary>
    /// 資料匯入檢查
    /// </summary>
    Task<List<MissionImportDto>> ImportFileCheck(Guid parentId, Guid? teamId, string code, IFormFile file);

    /// <summary>
    /// 資料匯入
    /// </summary>
    Task ImportFile(List<MissionImportDto> dtos);

    /// <summary>
    /// 資料匯出(子任務)
    /// </summary>
    Task<MyFileInfoDto> ExportFile(Guid parentId, string code);

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

    #region 任務附件

    /// <summary>
    /// 上傳任務附件
    /// </summary>
    Task<FileInfoDto> UploadFile(Guid? teamId, Guid missionId, string name, int fileIndex, string note, IFormFile file);

    /// <summary>
    /// 刪除任務附件
    /// </summary>
    /// <param name="id">附件 Id</param>
    Task DeleteFile(Guid id);

    /// <summary>
    /// 取得某一任務所有附件
    /// </summary>
    /// <param name="id">任務 Id</param>
    Task<List<FileInfoDto>> GetAllFiles(Guid id);

    /// <summary>
    /// 更新附件的備註
    /// </summary>
    /// <param name="id">附件 Id</param>
    Task UpdateAttachmentNote(Guid id, string note);


    /// <summary>
    /// 下載附件
    /// </summary>
    /// <param name="id">附件 Id</param>
    Task<MyFileInfoDto> DownloadFile(Guid id);

    #endregion 任務附件
}