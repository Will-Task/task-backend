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
        var currentUserId = CurrentUser.Id;
        // 取得24小時內要完成事項
        var now = Clock.Now;
        var oneDayAfter = now.AddDays(1);
        var oneDayBefore = now.AddDays(-1);
        var missions = await _repositorys.MissionView.GetListAsync(
            m => m.MissionStartTime <= oneDayAfter && m.MissionStartTime >= oneDayBefore &&　m.UserId == currentUserId);
        var todos = new List<ToDoMissionViewDto>();
        
        foreach (var mission in missions)
        {
            var todo = new ToDoMissionViewDto();
            todo.Id = mission.MissionId;
            todo.Text = mission.MissionName;
            todo.Done = mission.MissionFinishTime.HasValue;
            todo.Lang = mission.Lang;
            todos.Add(todo);
        }
        return todos;
    }

    /// <summary>
    /// 取得每個父任務底下的子任務完成度(完成任務 / 總子任務數)
    /// </summary>
    public async Task<List<MissionProgressDto>> GetMissionFinishPercentage()
    {
         var currentUserId = CurrentUser.Id;
         // 計算每個父任務底下子任務完成度
         var query = await _repositorys.MissionView.GetQueryableAsync();
         query = query.Where(m => m.UserId == currentUserId);
         var parentMissionMap = query.Where(m => m.ParentMissionId == null)
                                                           .ToDictionary(m => m.MissionId , m => m.MissionName );
         var submissionMap = query.Where(m => m.ParentMissionId != null)
                   .GroupBy(m => new {m.ParentMissionId.Value , m.Lang}).ToDictionary(m => m.Key 
                   , m => m.Select(x => x.MissionFinishTime).ToList());

         var dtos = new List<MissionProgressDto>();
         foreach (var subMissions in submissionMap)
         {
             var parentMissionName = parentMissionMap[subMissions.Key.Value];
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
             missionProgressDto.Id = subMissions.Key.Value;
             missionProgressDto.ParentMissionName = parentMissionName;
             missionProgressDto.Lang = subMissions.Key.Lang;
             dtos.Add(missionProgressDto);
         }

         return dtos;
    }

    /// <summary>
    /// 獲取過去7天的任務進度
    /// </summary>
    public async Task<List<MissionProgressDetailDto>> GetMissionProgress()
    {
        var currentUserId = CurrentUser.Id;
        var now = Clock.Now;
        var sevenDayBefore = now.AddDays(-7);
        // 計算每個父任務底下子任務完成度
        var query = await _repositorys.MissionView.GetQueryableAsync();
        query = query.Where(m => m.UserId == currentUserId);
        var parentMissionMap = query.Where(m => m.ParentMissionId == null)
            .ToDictionary(m => m.MissionId , m => m.MissionName);
        var submissionMap = query.Where(m => m.ParentMissionId != null && m.MissionFinishTime != null && m.MissionFinishTime > sevenDayBefore)
            .GroupBy(m => new {m.ParentMissionId.Value , m.Lang}).ToDictionary(m => m.Key 
                , m => m.Select(x => x.MissionFinishTime).ToList());

        var dtos = new List<MissionProgressDetailDto>();
        foreach (var subMissions in submissionMap)
        {
            var parentMissionName = parentMissionMap[subMissions.Key.Value];
            // 計算任務完成為星期幾
            var propertyNames = new List<string>()
            {
                "MonProgress", "TueProgress", "WedProgress", "ThurProgress", "FriProgress", "SatProgress", "SunProgress"
            };
            var dto = new MissionProgressDetailDto();
            dto.Id = subMissions.Key.Value;
            dto.ParentMissionName = parentMissionName;
            dto.Lang = subMissions.Key.Lang;
            foreach (var missionFinishTime in subMissions.Value)
            {
                var dayOfWeek = (int)missionFinishTime.Value.DayOfWeek;
                var property = dto.GetType().GetProperty(propertyNames[dayOfWeek - 1]);
                property.SetValue(dto,property.GetInt() + 1);
            }
            dtos.Add(dto);
        }

        return dtos;
    }
}