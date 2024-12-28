using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Business.MissionCategoryManagement.Dto;
using Business.MissionManagement;
using Business.Models;
using Business.Permissions;
using Business.ReportManagement.Dto;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using XCZ.Extensions;

namespace Business.ReportManagement;

[Authorize(BusinessPermissions.TaskReport.Default)]
[RemoteService(false)]
public class ReportAppService : ApplicationService, IReportAppService
{
    private readonly (
        IRepository<Language> Langugage,
        IRepository<MissionOverAllView> MissionOverAllView,
        IRepository<MissionCategoryView> MissionCategoryView,
        IRepository<MissionView> MissionView,
        IRepository<Language> Language,
        IRepository<LocalizationText> LocalizationText,
        IRepository<MissionI18N> MissionI18N,
        IRepository<MissionCategoryI18N> MissionCategoryI18N
        ) _repositorys;

    private readonly ILogger<MissionAppService> _logger;

    public ReportAppService(
        IRepository<Language> Langugage,
        IRepository<MissionOverAllView> MissionOverAllView,
        IRepository<MissionCategoryView> MissionCategoryView,
        IRepository<MissionView> MissionView,
        IRepository<Language> Language,
        IRepository<LocalizationText> LocalizationText,
        IRepository<MissionI18N> MissionI18N,
        IRepository<MissionCategoryI18N> MissionCategoryI18N,
        ILogger<MissionAppService> logger)
    {
        _repositorys = (Langugage, MissionOverAllView, MissionCategoryView, MissionView, Language, LocalizationText, MissionI18N, MissionCategoryI18N);
        _logger = logger;
    }

    /// <summary>
    /// 匯出任務完成度報告
    /// </summary>
    public async Task<MyFileInfoDto> GetFinishRateReport(List<Guid> ids ,Guid? teamId, string name, string code)
    {
        var workbook = new XLWorkbook();
        var sheetName = (await _repositorys.LocalizationText.GetAsync(x => x.LanguageCode == code &&
                            x.Category == "SheetName" && x.ItemKey == "11")).ItemValue;
        var worksheet = workbook.AddWorksheet($"{name}${sheetName}");

        var queryLocalization = await _repositorys.LocalizationText.GetQueryableAsync();
        var isFinishMap = queryLocalization.Where(x => x.LanguageCode == code && x.Category == "MissionState")
            .ToDictionary(x => x.ItemKey, x => x.ItemValue);
        var language = await _repositorys.Langugage.GetAsync(x => x.Code == code);
        
        /// 多國語系資料
        var queryMissionI18N = await _repositorys.MissionI18N.GetQueryableAsync();
        var defaultMissionMap = queryMissionI18N.OrderBy(x => x.Lang == language.Id ? 0 : x.Lang)
            .OrderBy(x => x.Lang).GroupBy(x => x.MissionId)
            .ToDictionary(g => g.Key, x => x.First().MissionName );
    
        var queryCategoryI18N = await _repositorys.MissionCategoryI18N.GetQueryableAsync();
        var defaultCategoryMap = queryCategoryI18N.OrderBy(x => x.Lang == language.Id ? 0 : x.Lang)
            .OrderBy(x => x.Lang).GroupBy(x => x.MissionCategoryId)
            .ToDictionary(g => g.Key, x => x.First().MissionCategoryName );

        /// 獲取任務overAllView
        var missions = await _repositorys.MissionOverAllView
            .GetListAsync(x => x.TeamId == teamId && x.Lang == language.Id && ids.Contains(x.SubCategoryId));
        var dtos = ObjectMapper.Map<List<MissionOverAllView>, List<MissionOverAllViewDto>>(missions);
        
        foreach (var dto in dtos)
        {
            if (dto.MissionName.IsNullOrEmpty())
            {
                dto.MissionName = defaultMissionMap[dto.MissionId];
            }
            if (dto.SubMissionName.IsNullOrEmpty())
            {
                dto.SubMissionName = defaultMissionMap[dto.MissionId];
            }
            if (dto.CategoryName.IsNullOrEmpty())
            {
                dto.CategoryName = defaultCategoryMap[dto.CategoryId];
            }
            if (dto.SubCategoryName.IsNullOrEmpty())
            {
                dto.SubCategoryName = defaultCategoryMap[dto.CategoryId];
            }
        }
        
        var exportDtos = ObjectMapper.Map<List<MissionOverAllViewDto>, List<ExportMissionOverAllViewDto>>(dtos);

        var startColumn = 1;
        var startRow = 1;
        var endRow = 1;
        var endColumn = 9;

        /// 設定標頭團隊名稱
        worksheet.Range(startRow, startColumn, startRow, endColumn).Merge();
        /// 設背景顏色
        worksheet.Cell(startRow, startColumn).Style.Fill.BackgroundColor = XLColor.Yellow;
        /// 水平對齊
        worksheet.Cell(startRow, startColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell(startRow++, startColumn).Value = $"{name}";

        /// 設定標頭團隊名稱
        worksheet.Range(startRow, startColumn, startRow + missions.Count, startColumn + 1).Merge();
        /// 設背景顏色
        worksheet.Cell(startRow, startColumn).Style.Fill.BackgroundColor = XLColor.Red;
        /// 水平對齊
        worksheet.Cell(startRow, startColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Cell(startRow, startColumn++).Value = $"{name}";

        /// 寫入Title
        int titleStartColumn = startColumn;
        var titles = new List<string> { "類別", "子類別", "父任務", "是否完成", "子任務", "是否完成", "父任務完成狀態(%)" };
        /// 設背景顏色
        worksheet.Range(startRow, ++startColumn, startRow, endColumn).Style.Fill.BackgroundColor = XLColor.BallBlue;
        foreach (var title in titles)
        {
            worksheet.Cell(startRow, ++titleStartColumn).Value = title;
        }

        startRow++;

        /// 計算子任務數量來計算出父任務是否完成(是 -> 下面子任務全完成)
        var queryMission = await _repositorys.MissionView.GetQueryableAsync();
        var isMissionFinishMap = queryMission.Where(x => ids.Contains(x.MissionCategoryId) && x.ParentMissionId != null && !string.IsNullOrEmpty(x.MissionName))
            .GroupBy(x => x.ParentMissionId).ToDictionary(g => g.Key, x => x.All(y => y.MissionFinishTime != null));

        var startDataColumn = startColumn;
        foreach (var exportDto in exportDtos)
        {
            var properties = exportDto.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Guid))
                {
                    continue;
                }

                if (property.PropertyType == typeof(DateTime?))
                {
                    /// 0 -> 否 , 1 -> 是
                    string isFinish = property.Name == "MissionFinishTime" 
                        ? isMissionFinishMap[exportDto.MissionId] ? isFinishMap["1"] : isFinishMap["0"]
                        : property.GetValue(exportDto).IsNullOrEmpty() ? isFinishMap["0"] : isFinishMap["1"];
                    if(property.Name == "MissionFinishTime")
                    {
                        var b = isMissionFinishMap[exportDto.MissionId] ? isFinishMap["1"] : isFinishMap["0"];
                    }
                    worksheet.Cell(startRow, startDataColumn++).Value = $"{isFinish}";
                }
                else
                {
                    worksheet.Cell(startRow, startDataColumn++).Value = $"{property.GetValue(exportDto)}";
                }
            }

            startRow++;
            startDataColumn = startColumn;
        }

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);

        var fileNmae = (await _repositorys.LocalizationText.GetAsync(x => x.LanguageCode == code &&
                            x.Category == "Export" && x.ItemKey == "11")).ItemValue;
        return new MyFileInfoDto { FileContent = memoryStream.ToArray(), FileName = $"{name}{fileNmae}" };
    }

    /// <summary>
    /// 獲取可匯出任務完成度報告的類別
    /// </summary>
    public async Task<List<MissionCategoryViewDto>> GetReportData(Guid? teamId, string code)
    {
        var lang = 1;
        var lauguage = await _repositorys.Langugage.FindAsync(x => x.Code == code);
        lang = !lauguage.IsNullOrEmpty() ? lauguage.Id : lang;
        // 取得父任務和子任務同筆的數據
        var categories = await _repositorys.MissionCategoryView
            .GetListAsync(x => x.TeamId == teamId && x.Lang == lang && x.ParentId != null);

        return ObjectMapper.Map<List<MissionCategoryView>, List<MissionCategoryViewDto>>(categories);
    }
}