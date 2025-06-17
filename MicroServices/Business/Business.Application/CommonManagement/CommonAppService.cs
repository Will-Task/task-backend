using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DomainServices;
using Business.MissionCategoryManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;
using Business.Specifications.CategoryView;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Business.CommonManagement
{
    public class CommonAppService : ApplicationService, ICommonAppService
    {
        private readonly (
            IRepository<MissionCategoryView> MissionCategoryView,
            IRepository<MissionI18N, Guid> MissionI18N,
            IRepository<MissionView> MissionView
            ) _repositorys;

        private readonly CategoryManager _categoryManager;

        public CommonAppService(
            IRepository<MissionCategoryView> MissionCategoryView,
            IRepository<MissionI18N, Guid> MissionI18N,
            IRepository<MissionView> MissionView,
            CategoryManager categoryManager
            )
        {
            _repositorys = (MissionCategoryView, MissionI18N, MissionView);
            _categoryManager = categoryManager;
        }

        /// <summary>
        /// 取得類別和子類別之前的對應關係
        /// </summary>
        public async Task<Dictionary<string, List<MissionCategoryViewDto>>> GetCategoryMapping(Guid? teamId)
        {
            var categorys = await _repositorys.MissionCategoryView.GetListAsync(new TeamOrUserCategorySpecification(teamId, CurrentUser.Id));
            var defaultCategory = await _categoryManager.GetDefaultLangData();
            var dtos = ObjectMapper.Map<List<MissionCategoryView>, List<MissionCategoryViewDto>>(categorys);

            /// 多國語系文字設定
            foreach (var dto in dtos)
            {
                if (dto.MissionCategoryName.IsNullOrEmpty())
                {
                    dto.MissionCategoryName = defaultCategory[dto.MissionCategoryId];
                }
            }

            var categoryMapping = dtos.Where(x => x.ParentId != null).GroupBy(x => x.ParentId.ToString())
                .ToDictionary(g => g.Key, x => x.ToList());

            return categoryMapping;

        }

        /// <summary>
        /// 處理多國語系資料對應
        /// </summary>
        public async Task<List<MissionViewDto>> GetMissionLangData(Guid? teamId)
        {
            var missions = await _repositorys.MissionView.GetListAsync(x => x.TeamId == teamId);
            /// 預設資料
            var defaultMissionMap = missions.GroupBy(x => x.MissionId)
                .ToDictionary(g => g.Key, x => x.OrderBy(c => c.Lang)
                .Select(x => new {x.MissionName, x.MissionDescription}).First()); 
            
            foreach (var mission in missions)
            {
                if (mission.MissionName.IsNullOrEmpty())
                {
                    mission.MissionName = defaultMissionMap[mission.MissionId].MissionName;
                }
                if (mission.MissionDescription.IsNullOrEmpty())
                {
                    mission.MissionDescription = defaultMissionMap[mission.MissionId].MissionDescription;
                }
            }

            return ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(missions);
        }

        /// <summary>
        /// 處理多國語系資料對應
        /// </summary>
        public async Task<List<MissionCategoryViewDto>> GetCategoryLangData(Guid? teamId)
        {
            var categories = await _repositorys.MissionCategoryView.GetListAsync(x => x.TeamId == teamId);
            /// 預設資料
            var defaultCategoryMap = categories.GroupBy(x => x.MissionCategoryId)
                    .ToDictionary(g => g.Key, x => x.OrderBy(c => c.Lang)
                    .First().MissionCategoryName); 
            
            foreach (var category in categories)
            {
                if (category.MissionCategoryName.IsNullOrEmpty())
                {
                    category.MissionCategoryName = defaultCategoryMap[category.MissionCategoryId];
                }
            }

            return ObjectMapper.Map<List<MissionCategoryView>, List<MissionCategoryViewDto>>(categories);
        }
    }
}
