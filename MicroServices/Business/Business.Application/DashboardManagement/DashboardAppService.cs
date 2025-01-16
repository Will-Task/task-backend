using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Business.DashboardManagement;
using Business.DashboardManagement.Dto;
using Business.Enums;
using Business.Models;
using Business.Permissions;
using Business.Specifications;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
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

    private readonly MissionManager _missionManager;
    private readonly CategoryManager _categoryManager;

    public DashboardAppService(
        IRepository<MissionView> MissionView,
        IRepository<Mission> Mission,
        MissionManager missionManager,
        CategoryManager categoryManager
        )
    {
        _repositorys = (MissionView, Mission);
        _missionManager = missionManager;
        _categoryManager = categoryManager;
    }

    /// <summary>
    /// 根據任務狀態陳列任務
    /// </summary>
    public async Task<Dictionary<int, List<MissionKanbanDto>>> GetKanbanData(Guid? teamId)
    {
        var queryMission = await _repositorys.MissionView.GetQueryableAsync();
        var defaultMissionMap = await _missionManager.GetDefaultLangData();
        var defaultCategoryMap = await _categoryManager.GetDefaultLangData();
        
        var missionMap = queryMission.Where(new TeamOrUserMissionSpecification(teamId, CurrentUser.Id))
            .GroupBy(x => x.MissionState).ToDictionary(g => (int)g.Key,
                x => x.Select(c => new MissionKanbanDto
                {
                    MissionId = c.MissionId,
                    MissionName = c.MissionName.IsNullOrEmpty() ? defaultMissionMap[c.MissionId].MissionName : c.MissionName,
                    CategoryId = c.MissionCategoryId,
                    CategoryName = c.MissionCategoryName.IsNullOrEmpty() ? defaultCategoryMap[c.MissionCategoryId] : c.MissionCategoryName,
                    Month = c.MissionStartTime.Month,
                    Day = c.MissionStartTime.Day,
                    Lang = c.Lang
                }).ToList()
            );

        for (int i = (int)MissionState.TODO; i <= (int)MissionState.COMPLETED; i++)
        {
            if (!missionMap.ContainsKey(i))
            {
                missionMap.Add(i, new List<MissionKanbanDto>());
            }
        }

        missionMap = missionMap.OrderBy(x => x.Key).ToDictionary();
        
        return missionMap;
    }

    /// <summary>
    /// 各類別完成度統計
    /// </summary>
    public async Task<List<MissionProgressDto>> GetMissionProgress(Guid? teamId)
    {
        var queryMission = await _repositorys.MissionView.GetQueryableAsync();
        var defaultCategoryMap = await _categoryManager.GetDefaultLangData();

        var dtos = queryMission.Where(new TeamOrUserMissionSpecification(teamId, CurrentUser.Id))
            .GroupBy(x => new {x.MissionCategoryId, x.Lang})
            .Select(g => new MissionProgressDto
            {
                Id = g.Key.MissionCategoryId,
                CategoryName = defaultCategoryMap[g.Key.MissionCategoryId],
                Total = g.Count(),
                Finish = g.Count(m => m.MissionState == MissionState.COMPLETED),
                Lang = g.Key.Lang
            }).ToList();

        return dtos;
    }

    /// <summary>
    /// 最近任務獲取(當天)
    /// </summary>
    public async Task<List<MissionRecentDto>> GetMissionRecent(Guid? teamId)
    {
        var queryMission = await _repositorys.MissionView.GetQueryableAsync();
        var defaultMissionMap = await _missionManager.GetDefaultLangData();

        var dtos = queryMission.Where(new TeamOrUserMissionSpecification(teamId, CurrentUser.Id))
            .GroupBy(x => new {x.MissionId, x.Lang})
            .Select(g => new MissionRecentDto()
            {
                Id = g.Key.MissionId,
                MissionName = defaultMissionMap[g.Key.MissionId].MissionName,
                Priority = g.First().MissionPriority,
                StartTime = $"{g.First().MissionStartTime.Hour}:{g.First().MissionStartTime.Minute}",
                EndTime = $"{g.First().MissionEndTime.Hour}:{g.First().MissionEndTime.Minute}",
                Lang = g.Key.Lang
            }).ToList();
        
        return dtos;
    }

    /// <summary>
    /// 每個類別所花的時間佔比
    /// </summary>
    public async Task<List<CategoryPercentageDto>> GetCategoryPercentage(Guid? teamId)
    {
        var queryMission = await _repositorys.MissionView.GetQueryableAsync();
        var defaultCategoryMap = await _categoryManager.GetDefaultLangData();

        /// 每個月所花費的時間總和
        var totalMap = queryMission.Where(new TeamOrUserMissionSpecification(teamId, CurrentUser.Id))
            .Where(x => x.Lang == 1)
            .GroupBy(g => g.MissionEndTime.Month)
            .AsEnumerable()
            .Select(x => new
            {
                Total = x.Sum(c => c.MissionEndTime.Subtract(c.MissionStartTime).TotalMinutes),
                Month = x.Key
            })
            .ToList();
        
        var fullTotalMap = Enumerable.Range(1, 12)
            .Select(month => new
            {
                Month = month,
                TotalMinutes = totalMap.FirstOrDefault(x => x.Month == month)?.Total ?? 0
            })
            .ToList();

        var dtos = queryMission.Where(new TeamOrUserMissionSpecification(teamId, CurrentUser.Id))
            .GroupBy(x => new {x.MissionCategoryId, x.Lang})
            .AsEnumerable()
            .Select(g => new CategoryPercentageDto
            {
                CategoryName = defaultCategoryMap[g.Key.MissionCategoryId],
                Rates = g.GroupBy(g1 => g1.MissionEndTime.Month)
                         .Select(g1 =>(int)(100 * g1.Sum(c => 
                         c.MissionEndTime.Subtract(c.MissionStartTime).TotalMinutes) / fullTotalMap[g1.Key - 1].TotalMinutes)).ToList(),
                Lang = g.Key.Lang
            }).ToList();

        return dtos;
    }

    /// <summary>
    /// 每月任務完成數
    /// </summary>
    public async Task<List<MissionOfEveryMonthDto>> GetMissionOfEveryMonth(Guid? teamId)
    { 
        var queryMission = await _repositorys.MissionView.GetQueryableAsync();
        var defaultCategoryMap = await _categoryManager.GetDefaultLangData();
        
        var dtos = queryMission.Where(new TeamOrUserMissionSpecification(teamId, CurrentUser.Id))
            .Where(x => x.MissionFinishTime != null)
            .GroupBy(x => new {x.MissionCategoryId, x.Lang})
            .AsEnumerable()
            .Select(g =>
            {
                var months = new int[12];
                g.GroupBy(g1 => g1.MissionFinishTime.Value.Month).ToList()
                    .ForEach(x => months[x.Key - 1] = x.Count());  
                
                return new MissionOfEveryMonthDto
                {
                    CategoryName = defaultCategoryMap[g.Key.MissionCategoryId],
                    FinishAmount = months.ToList(),
                    Lang = g.Key.Lang
                };
            }).ToList();
        
        return dtos;
    }

    /// <summary>
    /// 根據時間成列任務和父子任務關係
    /// </summary>
    public async Task<List<MissioGanttDto>> GetGanttData(Guid? teamId)
    {
        var queryMission = await _repositorys.MissionView.GetQueryableAsync();
        var defaultMissionMap = await _missionManager.GetDefaultLangData();
        int count = 1;
        var dtos = queryMission.Where(new TeamOrUserMissionSpecification(teamId, CurrentUser.Id))
            .Where(x => x.Lang == 1)
            .AsEnumerable()
            .Select(g => new MissioGanttDto()
            {
                Id = g.MissionId,
                Title = g.MissionName ?? string.Empty,
                ParentId = g.ParentMissionId,
                OrderId = count++,
                Start = g.MissionStartTime.ToString("o"), 
                End = g.MissionEndTime.ToString("o"),
                Expanded = !g.ParentMissionId.HasValue,
                Lang = g.Lang
            }).ToList();

        return dtos;
    }

    /// <summary>
    /// 根據時間成列任務和父子任務關係
    /// </summary>
    public async Task<List<MissioGanttByOrderDto>> GetGanttDataByOrder(Guid? teamId)
    {
        var queryMission = await _repositorys.Mission.GetQueryableAsync();
        queryMission = queryMission.Where(new Business.Specifications.Mission.TeamOrUserMissionSpecification(teamId, CurrentUser.Id));

        var missionMap = queryMission.Where(x => x.ParentMissionId != null).GroupBy(x => x.ParentMissionId).ToDictionary(g => g.Key, x => x.OrderBy(x => x.MissionStartTime).ToList());
        var dtos = new List<MissioGanttByOrderDto>();
        int count = 1;

        foreach (var (key, values) in missionMap)
        {
            var parentDto = new MissioGanttByOrderDto
            {
                Id = count++,
                SuccessorId = values.First().Id,
            };
            dtos.Add(parentDto);
            Guid? pre = key.Value;
            for (int i = 0; i < values.Count; i++)
            {
                var missionId = values[i].Id;
                var dto = new MissioGanttByOrderDto
                {
                    Id = count++,
                    PredecessorId = pre,
                    SuccessorId = i + 1 < values.Count ? values[i + 1].Id : null
                };
                pre = missionId;
                dtos.Add(dto);
            }
        }

        return dtos;
    }
}