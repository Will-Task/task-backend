using System;
using System.Threading.Tasks;
using Business.Models;
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

    [Route("/finishRate")]
    [HttpGet]
    public async Task<IActionResult> GetFinishRateReport(Guid? teamId, string name)
    {
        var fileDto = await _reportAppService.GetFinishRateReport(teamId, name);
        return File(fileDto.FileContent, "application/octet-stream", fileDto.FileName);
    }
}