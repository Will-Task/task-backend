using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.MissionCategoryManagement.Dto;
using Business.MissionManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.CommonManagement
{
    public interface ICommonAppService : IApplicationService
    {
        /// <summary>
        /// 取得類別和子類別之前的對應關係
        /// </summary>
        Task<Dictionary<string, List<MissionCategoryViewDto>>> GetCategoryMapping(Guid? teamId);

        /// <summary>
        /// 處理多國語系資料對應
        /// </summary>
        Task<List<MissionViewDto>> GetMissionLangData(Guid? teamId);
        
        /// <summary>
        /// 處理多國語系資料對應
        /// </summary>
        Task<List<MissionCategoryViewDto>> GetCategoryLangData(Guid? teamId);
    }
}
