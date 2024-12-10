using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Business.CommonManagement.Dto;
using Business.MissionManagement;
using Business.MissionManagement.Dto;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("Mission")]
[Route("api/business/mission")]
public class MissionController : AbpController
{
    private readonly IMissionAppService _missionAppService;

    public MissionController(IMissionAppService missionAppService)
    {
        _missionAppService = missionAppService;
    }

    /// <summary>
    /// 新增/修改任務
    /// </summary>
    [HttpPost]
    [Route("data-post")]
    public Task<MissionI18NDto> DataPost([FromBody] CreateOrUpdateMissionDto input)
    {
        return _missionAppService.DataPost(input);
    }

    /// <summary>
    /// 刪除任務(單 or 多筆)
    /// </summary>
    [HttpPost]
    [Route("delete")]
    public async Task Delete([FromBody] List<Guid> ids, int lang = 1)
    {
        foreach (var id in ids)
        {
            await _missionAppService.Delete(id, lang);
        }
    }

    /// <summary>
    /// 查詢特定任務(單個)
    /// </summary>
    [HttpGet]
    [Route("{id}")]
    public Task<MissionViewDto> Get(Guid id)
    {
        return _missionAppService.Get(id);
    }

    /// <summary>
    /// 獲取父 or 子任務(多個)
    /// </summary>
    [HttpGet]
    [Route("all")]
    public Task<PagedResultDto<MissionViewDto>> GetAll(int page, int pageSize, bool allData, Guid? teamId,
        Guid? categoryId, Guid? parentId)
    {
        return _missionAppService.GetAll(page, pageSize, allData, teamId, categoryId, parentId);
    }

    /// <summary>
    /// 查詢特定類別任務總攬
    /// </summary>
    /// <param name="categoryId">任務子類別 Id</param>
    [HttpGet]
    [Route("overview")]
    public Task<List<MissionOverviewDto>> GetOverview(Guid categoryId, Guid? teamId)
    {
        return _missionAppService.GetOverview(categoryId, teamId);
    }

    /// <summary>
    /// 任務提醒通知(寄email)
    /// </summary>
    [HttpGet]
    [Route("remind")]
    public async Task MissionReminder()
    {
        await _missionAppService.MissionReminder();
    }

    /// <summary>
    /// 設置任務提醒時間(結束時間多久前)
    /// </summary>
    [HttpGet]
    [Route("set-remind")]
    public async Task SetRemindTime(Guid id, int hour)
    {
        await _missionAppService.SetRemindTime(id, hour);
    }


    /// <summary>
    /// 變更任務狀態
    /// </summary>
    [HttpPost]
    [Route("update-state")]
    public async Task UpdateMissionState([FromBody] MissionFormData formData)
    {
        await _missionAppService.UpdateMissionState(formData);
    }


    /// <summary>
    /// 範本下載
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("sample")]
    public async Task<IActionResult> DNSample(Guid parentId, int lang)
    {
        var blobDto = await _missionAppService.DNSample(parentId, lang);
        return File(blobDto.Content, "application/octet-stream",
            blobDto.Name);
    }

    /// <summary>
    /// 資料匯入檢查
    /// </summary>
    [HttpPost]
    [Route("import-file-check")]
    public async Task<List<MissionImportDto>> ImportFileCheck(Guid parentId, Guid? teamId, int lang, IFormFile file)
    {
        return await _missionAppService.ImportFileCheck(parentId, teamId, lang, file);
    }

    /// <summary>
    /// 資料匯入
    /// </summary>
    [HttpPost]
    [Route("import-file")]
    public async Task ImportFile([FromBody] List<MissionImportDto> dtos)
    {
        await _missionAppService.ImportFile(dtos);
    }

    /// <summary>
    /// 資料匯出(子任務)
    /// </summary>
    [HttpPost]
    [Route("export")]
    public async Task<IActionResult> ExportFile(Guid parentId, int lang)
    {
        var fileDto = await _missionAppService.ExportFile(parentId, lang);
        return File(fileDto.FileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileDto.FileName);
    }

    /// <summary>
    /// 任務即將到期(24小時)通知
    /// </summary>
    [HttpGet]
    [Route("identify-expiring")]
    public async Task IdentifyExpiringTasks()
    {
        await _missionAppService.IdentifyExpiringTasks();
    }

    /// <summary>
    /// 任務報告產出Excel(1 week -> 7 days)傳送到email
    /// </summary>
    [HttpGet]
    [Route("export-report")]
    public async Task ExportReport()
    {
        await _missionAppService.ExportReport();
    }

    /// <summary>
    /// 檢查到期的任務
    /// </summary>
    [HttpGet]
    [Route("check")]
    public async Task CheckExpiredOrFinished()
    {
        await _missionAppService.CheckExpiredOrFinished();
    }

    /// <summary>
    /// 上傳任務附件
    /// </summary>
    [HttpPost]
    [Route("upload-file")]
    public Task<MissionAttachmentDto> UploadFile([FromBody] CreateMissionAttachmentDto input, IFormFile file)
    {
        return _missionAppService.UploadFile(input, file);
    }

    /// <summary>
    /// 刪除任務附件
    /// </summary>
    /// <param name="id">附件 Id</param>
    [HttpPost]
    [Route("delete-file")]
    public async Task DeleteFile(Guid id)
    {
        await _missionAppService.DeleteFile(id);
    }

    /// <summary>
    /// 取得某一任務所有附件
    /// </summary>
    /// <param name="id">任務 Id</param>
    [HttpGet]
    [Route("file/all")]
    public Task<List<MissionAttachmentDto>> GetAllFiles(Guid id)
    {
        return _missionAppService.GetAllFiles(id);
    }

    /// <summary>
    /// 更新附件的備註
    /// </summary>
    /// <param name="id">附件 Id</param>
    [HttpPost]
    [Route("update-note")]
    public Task UpdateAttachmentNote(Guid id, string aNote)
    {
        return _missionAppService.UpdateAttachmentNote(id);
    }
}