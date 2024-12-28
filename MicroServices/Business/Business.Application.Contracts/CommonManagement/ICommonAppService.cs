using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.MissionCategoryManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.CommonManagement
{
    public interface ICommonAppService : IApplicationService
    {
        /// <summary>
        /// 取得類別和子類別之前的對應關係
        /// </summary>
        Task<Dictionary<string, List<MissionCategoryViewDto>>> GetCategoryMapping(Guid? teamId);
    }
}
