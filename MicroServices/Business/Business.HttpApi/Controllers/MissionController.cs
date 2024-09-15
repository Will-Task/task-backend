using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Business.MissionManagement;
using Business.MissionManagement.Dto;
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
    /// 獲取父任務下的子任務(多個)
    /// </summary>
    [HttpGet]
    [Route("sub")]
    public Task<IEnumerable<MissionViewDto>> GetSubMission(Guid id)
    {
        return _missionAppService.GetSubMission(id);
    }

    /// <summary>
    /// 查詢所有父任務(多個)
    /// </summary>
    [HttpGet]
    [Route("parent")]
    public Task<PagedResultDto<MissionViewDto>> GetParentMission(int page , int pageSize , bool allData)
    {
        return _missionAppService.GetParentMission(page , pageSize , allData);
    }

    /// <summary>
    /// 根據類別取得父任務
    /// </summary>
    [HttpGet]
    [Route("parent/{categoryId}")]
    public Task<IEnumerable<MissionViewDto>> GetParentMission(Guid categoryId)
    {
        return _missionAppService.GetParentMissionByCategoryId(categoryId);
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
    /// 查詢特定類別下的任務(多個)
    /// </summary>
    [HttpGet]
    [Route("category/{id}")]
    public Task<IEnumerable<MissionViewDto>> GetMission(Guid id)
    {
        return _missionAppService.GetMission(id);
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
    public async Task setRemindTime(Guid id,int hour)
    {
        await _missionAppService.setRemindTime(id,hour);
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
    /// 變更任務狀態
    /// </summary>
    [HttpPost]
    [Route("update-state")]
    public async Task UpdateMissionState(Guid missionId, int state)
    {
        await _missionAppService.UpdateMissionState(missionId, state);
    }

    /// <summary>
    /// 刪除任務(過期任務不會被刪除)
    /// </summary>
    [HttpPost]
    [Route("delete")]
    public async Task Delete([FromBody]List<Guid> ids , int lang = 1)
    {
        foreach (var id in ids)
        {
            await _missionAppService.Delete(id , lang);
        }
    }

    /// <summary>
    /// 範本下載
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("sample")]
    public async Task<IActionResult> DNSample(string fileName, int lang)
    {
        var fileDto = await _missionAppService.DNSample(fileName, lang);
        return File(fileDto.FileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileDto.FileName);
    }

    /// <summary>
    /// 資料匯入
    /// </summary>
    [HttpPost]
    [Route("import")]
    public async Task<IEnumerable<MissionImportDto>> ImportFile(IFormFile file, int lang)
    {
        return await _missionAppService.ImportFile(file,lang);
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
}