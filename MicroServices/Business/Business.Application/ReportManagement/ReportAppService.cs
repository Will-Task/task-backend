using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Business.MissionManagement;
using Business.Models;
using Business.Permissions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NUglify.Helpers;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Business.ReportManagement;

[Authorize(BusinessPermissions.TaskReport.Default)]
public class ReportAppService : ApplicationService, IReportAppService
{
    private (
        IRepository<MissionView> MissionView,
        IRepository<MissionCategoryView> MissionCategoryView
        ) _repositoys;

    private readonly ILogger<MissionAppService> _logger;

    public ReportAppService(
        IRepository<MissionView> MissionView,
        IRepository<MissionCategoryView> MissionCategoryView,
        ILogger<MissionAppService> logger)
    {
        _repositoys = (MissionView, MissionCategoryView);
        _logger = logger;
    }

    public async Task<MyFileInfoDto> GetFinishRateReport(Guid? teamId, string name)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet($"{name}的任務完成度報告");
        

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);

        return new MyFileInfoDto { FileContent = memoryStream.ToArray(), FileName = $"{name}的任務完成度報告" };
    }
}