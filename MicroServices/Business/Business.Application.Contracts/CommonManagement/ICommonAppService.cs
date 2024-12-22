using Business.MissionCategoryManagement.Dto;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
