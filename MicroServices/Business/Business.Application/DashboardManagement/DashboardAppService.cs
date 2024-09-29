using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
using XCZ.Extensions;

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

    /// <summary>
    /// 取得每個父任務底下的子任務完成度(完成任務 / 總子任務數)
    /// </summary>
    public async Task<List<MissionProgressDto>> GetMissionFinishPercentage()
    {
         // 計算每個父任務底下子任務完成度
         var query = await _repositorys.MissionView.GetQueryableAsync();

         var parentMissionMap = query.Where(m => m.ParentMissionId == null)
                                                           .ToDictionary(m => m.MissionId , m => m.MissionName);
         var submissionMap = query.Where(m => m.ParentMissionId != null)
                   .GroupBy(m => m.ParentMissionId.Value).ToDictionary(m => m.Key 
                   , m => m.Select(x => x.MissionFinishTime).ToList());

         var dtos = new List<MissionProgressDto>();
         foreach (var subMissions in submissionMap)
         {
             var parentMissionName = parentMissionMap[subMissions.Key];
             int totalCount = subMissions.Value.Count();
             int finishCount = 0;
             // 計算任務完成比數
             foreach (var missionFinishTime in subMissions.Value)
             {
                 if (!missionFinishTime.IsNullOrEmpty())
                 {
                     finishCount++;
                 }
             }

             var missionProgressDto = new MissionProgressDto();
             missionProgressDto.Percentage = finishCount * 100 / totalCount;
             missionProgressDto.Id = subMissions.Key;
             missionProgressDto.ParentMissionName = parentMissionName;
             dtos.Add(missionProgressDto);
         }

         return dtos;
    }
}