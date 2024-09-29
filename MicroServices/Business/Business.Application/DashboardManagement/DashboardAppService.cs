using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DashboardManagement;
using Business.DashboardManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Business.Dashboard;

[Authorize]
[RemoteService(false)]
public class DashboardAppService : ApplicationService , IDashboardAppService
{
    private (
        IRepository<MissionView> MissionView,
        IRepository<Mission> Mission
    ) _repositorys;

    public DashboardAppService(
        IRepository<MissionView> MissionView,
        IRepository<Mission> Mission)
    {
        _repositorys = (MissionView, Mission);
    }
    
    /// <summary>
    /// 取得最近要做任務清單
    /// </summary>
    public async Task<List<ToDoMissionViewDto>> GetToDoList(int page , int pageSize , bool allData)
    {
        // 取得24小時內要完成事項
        var now = Clock.Now;
        var oneDayAfter = now.AddDays(1);
        var oneDayBefore = now.AddDays(-1);
        var missions = await _repositorys.MissionView.GetListAsync(
            m => m.MissionStartTime <= oneDayAfter && m.MissionStartTime >= oneDayBefore);
        var todos = new List<ToDoMissionViewDto>();
        
        foreach (var mission in missions)
        {
            var todo = new ToDoMissionViewDto();
            todo.Id = mission.MissionId;
            todo.text = mission.MissionName;
            todo.done = mission.MissionFinishTime.HasValue;
            todos.Add(todo);
        }
        return todos;
    }
}