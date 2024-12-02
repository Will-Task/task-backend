using System;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Business.Models;
using Volo.Abp.Application.Services;

namespace Business.ReportManagement;

public interface IReportAppService : IApplicationService
{
    Task<MyFileInfoDto> GetFinishRateReport(Guid? teamId, string name, string code);
}