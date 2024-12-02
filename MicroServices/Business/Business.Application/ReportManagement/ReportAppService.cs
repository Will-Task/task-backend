using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Business.MissionCategoryManagement.Dto;
using Business.MissionManagement;
using Business.Models;
using Business.Permissions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using XCZ.Extensions;

namespace Business.ReportManagement;

[Authorize(BusinessPermissions.TaskReport.Default)]
public class ReportAppService : ApplicationService, IReportAppService
{
    private readonly (
        IRepository<Language> Langugage,
        IRepository<MissionOverAllView> MissionOverAllView,
        IRepository<MissionCategoryView> MissionCategoryView
        ) _repositorys;

    private readonly ILogger<MissionAppService> _logger;

    public ReportAppService(
        IRepository<Language> Langugage,
        IRepository<MissionOverAllView> MissionOverAllView,
        IRepository<MissionCategoryView> MissionCategoryView,
        ILogger<MissionAppService> logger)
    {
        _repositorys = (Langugage, MissionOverAllView, MissionCategoryView);
        _logger = logger;
    }

    /// <summary>
    /// 匯出任務完成度報告
    /// </summary>
    public async Task<MyFileInfoDto> GetFinishRateReport(List<Guid> ids ,Guid? teamId, string name, string code)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet($"{name}的任務完成度報告");

        var lang = 1;
        var language = await _repositorys.Langugage.GetAsync(x => x.Code == code);
        lang = !lang.IsNullOrEmpty() ? language.Id : lang;

        // 獲取任務overAllView
        var missions = await _repositorys.MissionOverAllView
            .GetListAsync(x => x.TeamId == teamId && x.Lang == lang && ids.Contains(x.SubCategoryId));

        var startColumn = 1;
        var startRow = 1;
        var endRow = 1;
        var endColumn = 9;

        // 設定標頭團隊名稱
        worksheet.Range(startRow, startColumn, startRow, endColumn).Merge();
        // 設背景顏色
        worksheet.Cell(startRow, startColumn).Style.Fill.BackgroundColor = XLColor.Yellow;
        // 水平對齊
        worksheet.Cell(startRow, startColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell(startRow++, startColumn).Value = $"{name}";

        // 設定標頭團隊名稱
        worksheet.Range(startRow, startColumn, startRow + missions.Count, startColumn + 1).Merge();
        // 設背景顏色
        worksheet.Cell(startRow, startColumn).Style.Fill.BackgroundColor = XLColor.Red;
        // 水平對齊
        worksheet.Cell(startRow, startColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Cell(startRow, startColumn++).Value = $"{name}";

        // 寫入Title
        int titleStartColumn = startColumn;
        var titles = new List<string> { "類別", "子類別", "父任務", "是否完成", "子任務", "是否完成", "父任務完成狀態(%)" };
        // 設背景顏色
        worksheet.Range(startRow, ++startColumn, startRow, endColumn).Style.Fill.BackgroundColor = XLColor.Blue;
        foreach (var title in titles)
        {
            worksheet.Cell(startRow, ++titleStartColumn).Value = title;
        }

        startRow++;

        var startDataColumn = startColumn;
        for (int i = 0; i < missions.Count; i++)
        {
            var properties = missions[i].GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(Int64))
                {
                    continue;
                }

                if (property.PropertyType == typeof(DateTime?))
                {
                    worksheet.Cell(startRow, startDataColumn++).Value =
                        property.GetValue(missions[i]).IsNullOrEmpty() ? "否" : "是";
                }
                else
                {
                    worksheet.Cell(startRow, startDataColumn++).Value = $"{property.GetValue(missions[i])}";
                }
            }

            startRow++;
            startDataColumn = startColumn;
        }

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);

        return new MyFileInfoDto { FileContent = memoryStream.ToArray(), FileName = $"{name}的任務完成度報告.xlsx" };
    }

    /// <summary>
    /// 獲取可匯出任務完成度報告的類別
    /// </summary>
    public async Task<List<MissionCategoryViewDto>> GetReportData(Guid? teamId, string code)
    {
        var lang = 1;
        var lauguage = await _repositorys.Langugage.GetAsync(x => x.Code == code);
        lang = !lauguage.IsNullOrEmpty() ? lauguage.Id : lang;
        // 取得父任務和子任務同筆的數據
        var categories = await _repositorys.MissionCategoryView
            .GetListAsync(x => x.TeamId == teamId && x.Lang == lang && x.ParentId != null);

        return ObjectMapper.Map<List<MissionCategoryView>, List<MissionCategoryViewDto>>(categories);
    }
}