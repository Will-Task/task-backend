using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.CommonManagement;
using Business.MissionCategoryManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers
{
    [Area("Common")]
    [Route("/api/business/common")]
    public class CommonController : AbpController
    {
        private readonly ICommonAppService commonAppService;
        public CommonController(ICommonAppService _commonAppService)
        {
            commonAppService = _commonAppService;
        }

        /// <summary>
        /// 取得類別和子類別之前的對應關係
        /// </summary>
        [Route("category/mapping")]
        [HttpGet]
        public async Task<Dictionary<string, List<MissionCategoryViewDto>>> GetCategoryMapping(Guid? teamId)
        {
            return await commonAppService.GetCategoryMapping(teamId);
        }
    }
}
