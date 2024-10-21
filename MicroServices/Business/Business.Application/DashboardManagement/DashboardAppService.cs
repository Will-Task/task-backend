using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Business.DashboardManagement;
using Business.DashboardManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;
using Business.Permissions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Authorization;
using NUglify.Helpers;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using XCZ.Extensions;

namespace Business.Dashboard;

[Authorize(BusinessPermissions.TaskDashboard.Default)]
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
    /// 計算每個月的任務延遲狀況
    /// </summary>
    public async Task<List<MissionDelayDto>> GetMissionDelays()
    {
        var currentUser = CurrentUser.Id;
        var query = await _repositorys.MissionView.GetQueryableAsync();
        // 根據月份，計算未完成任務
        var maps = query.Where(m => m.UserId == currentUser && m.MissionFinishTime == null).GroupBy(m => m.MissionStartTime.Month)
                                .ToDictionary(m => m.Key , m => m.Count());

        var dtos = new List<MissionDelayDto>();
        int index = 0;
        foreach (var map in maps)
        {
            var dto = new MissionDelayDto();
            dto.Id = index++;
            dto.Count[map.Key - 1] = map.Value;
            dtos.Add(dto);
        }

        return dtos;
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

    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    public async Task<MissionGanttDto> GetGanttChart()
    {
        var resultDto = new MissionGanttDto();
        var currentId = CurrentUser.Id;
        var missions = await _repositorys.MissionView.GetListAsync(m => m.UserId == currentId);
        // 子任務根據 結束時間和重要程度進行link
        var maps = missions.Where(m => m.ParentMissionId.HasValue).GroupBy(m => new { m.ParentMissionId, m.Lang }).ToDictionary(m => m.Key 
            , ms => ms.OrderBy(m => m.MissionEndTime).OrderBy(m => m.MissionPriority));
        var dtos = new List<MissionGanttChartDataDto>();
        var linkDtos = new List<MissionLinkDto>();
        foreach (var map in maps)
        {
            // 任務完成度計算
            int finish =  map.Value.Count(m => m.MissionFinishTime != null);
            int total = map.Value.Count();;
            decimal finishRate = finish / total;

            var parent = await _repositorys.MissionView.FindAsync(m => m.MissionId == map.Key.ParentMissionId);
            var dto = new MissionGanttChartDataDto();
            string startDateString = "";
            if (!parent.IsNullOrEmpty())
            {
                startDateString = parent.MissionStartTime.ToString();
                dto.Id = parent.MissionId;
                dto.Text = parent.MissionName;
                dto.Start_date = DateTime.Parse(startDateString.Substring(0,startDateString.IndexOf(" ")) , new CultureInfo("fr-FR")).ToString("dd-MM-yyyy");
                dto.Duration = (parent.MissionEndTime - parent.MissionStartTime).Days;
                dto.Order = parent.MissionPriority;
                dto.Progress = finishRate;
                dto.Parent = null;
                dto.Lang = parent.Lang;
                dtos.Add(dto);
            }

            Guid? last = null;
            
            map.Value.ForEach(m =>
            {
                if (last != null)
                {
                    var linkDto = new MissionLinkDto();
                    linkDto.Id = last;
                    linkDto.Source = last.Value;
                    linkDto.Target = m.MissionId;
                    linkDtos.Add(linkDto);
                }
                dto = new MissionGanttChartDataDto();
                dto.Id = m.MissionId;
                dto.Text = m.MissionName;
                startDateString = m.MissionStartTime.ToString();
                dto.Start_date = DateTime.Parse(startDateString.Substring(0,startDateString.IndexOf(" ")) , new CultureInfo("fr-FR")).ToString("dd-MM-yyyy");
                dto.Duration = (m.MissionEndTime - m.MissionStartTime).Days;
                dto.Order = m.MissionPriority;
                dto.Progress = m.MissionFinishTime.IsNullOrEmpty() ? 0 : 1;
                dto.Parent = m.ParentMissionId;
                dto.Lang = m.Lang;
                last = m.MissionId;
                dtos.Add(dto);
            });
        }

        resultDto.Tasks = dtos;
        resultDto.Links = linkDtos;

        return resultDto;
    }

    /// <summary>
    /// 獲取任務進度來呈現甘特圖（Gantt Chart）資料
    /// </summary>
    public async Task<List<MissionKanbanDto>> GetKanbanChart()
    {
        var currentUser = CurrentUser.Id;
        var dtos = new List<MissionKanbanDto>();
        var query = await _repositorys.MissionView.GetQueryableAsync();
        var maps = query.Where(m => m.UserId == currentUser).GroupBy(m => m.MissionState).ToDictionary(m => m.Key , 
                            m => m.OrderBy(m => m.MissionEndTime).OrderBy(m => m.MissionPriority));

        foreach (var map in maps)
        {
            var dto = new MissionKanbanDto();
            dto.Name = map.Key.ToString();
            map.Value.ForEach(m =>
            {
                dto.Tasks.Add(new MissionKanbanChartDataDto
                {
                    Id = m.MissionId,
                    Title = m.MissionName
                });
            });
            dtos.Add(dto);
        }

        return dtos;
    }
}