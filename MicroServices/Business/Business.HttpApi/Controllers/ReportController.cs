using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.MissionCategoryManagement.Dto;
using Business.ReportManagement;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("Report")]
[Route("/api/business/report")]
public class ReportController : AbpController
{
    private readonly IReportAppService _reportAppService;

    public ReportController(IReportAppService reportAppService)
    {
        _reportAppService = reportAppService;
    }

    /// <summary>
    /// 匯出任務完成度報告
    /// </summary>
    [Route("finishRate")]
    [HttpPost]
    public async Task<IActionResult> GetFinishRateReport([FromBody]List<Guid> ids ,Guid? teamId, string name, string code)
    {
        var fileDto = await _reportAppService.GetFinishRateReport(ids, teamId, name, code);
        return File(fileDto.FileContent, "application/octet-stream", fileDto.FileName);
    }

    /// <summary>
    /// 獲取可匯出任務完成度報告的類別
    /// </summary>
    [Route("reportData")]
    [HttpGet]
    public async Task<List<MissionCategoryViewDto>> GetReportData(Guid? teamId)
    {
        return await _reportAppService.GetReportData(teamId);
    }
}