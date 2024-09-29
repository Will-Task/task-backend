using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement;
using Business.DashboardManagement.Dto;
using Business.MissionManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("Dashboard")]
[Route("/api/business/dashboard")]
public class DashboardController : AbpController
{
    private readonly IDashboardAppService dashboardAppService;
    public DashboardController(IDashboardAppService _dashboardAppService)
    {
        dashboardAppService = _dashboardAppService;
    }

    /// <summary>
    /// 取得最近要做任務清單
    /// </summary>
    [HttpGet]
    [Route("todo")]
    public Task<List<ToDoMissionViewDto>> GetToDoList(int page , int pageSize , bool allData)
    {
        return dashboardAppService.GetToDoList(page , pageSize , allData);
    }
}