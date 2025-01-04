using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Business.MissionCategoryManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.ReportManagement;

public interface IReportAppService : IApplicationService
{
    /// <summary>
    /// 匯出任務完成度報告
    /// </summary>
    Task<MyFileInfoDto> GetFinishRateReport(List<Guid> ids ,Guid? teamId, string code);
    
    /// <summary>
    /// 獲取可匯出任務完成度報告的類別
    /// </summary>
    Task<List<MissionCategoryViewDto>> GetReportData(Guid? teamId);
}