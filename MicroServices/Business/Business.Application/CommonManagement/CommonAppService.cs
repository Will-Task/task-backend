using Business.MissionCategoryManagement.Dto;
using Business.Models;
using Business.Specifications.CategoryView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Business.CommonManagement
{
    public class CommonAppService : ApplicationService, ICommonAppService
    {
        private readonly (
            IRepository<MissionCategoryView> MissionCategoryView,
            IRepository<MissionI18N, Guid> MissionI18N
            ) _repositorys;

        public CommonAppService(
            IRepository<MissionCategoryView> MissionCategoryView,
            IRepository<MissionI18N, Guid> MissionI18N
            )
        {
            _repositorys = (MissionCategoryView, MissionI18N);
        }

        /// <summary>
        /// 取得類別和子類別之前的對應關係
        /// </summary>
        public async Task<Dictionary<string, List<MissionCategoryViewDto>>> GetCategoryMapping(Guid? teamId)
        {
            var categorys = await _repositorys.MissionCategoryView.GetListAsync(new TeamOrUserCategorySpecification(teamId, CurrentUser.Id));
            var defaultCategory = categorys.OrderBy(x => x.Lang == 1 ? 0 : x.Lang).OrderBy(x => x.Lang)
                .GroupBy(x => x.MissionCategoryId).ToDictionary(g => g.Key, x => x.First().MissionCategoryName);
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
    }
}
