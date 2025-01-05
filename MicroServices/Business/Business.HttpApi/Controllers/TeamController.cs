using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.CommonManagement.Dto;
using Business.TeamManagement;
using Business.TeamManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers
{
    [Area("Team")]
    [Route("api/business/team")]
    public class TeamController : AbpController
    {
        private readonly ITeamAppService _teamAppService;

        public TeamController(ITeamAppService teamAppService)
        {
            _teamAppService = teamAppService;
        }

        /// <summary>
        /// 獲取當前使用者所在的所有群組資訊
        /// </summary>
        [HttpGet]
        [Route("all")]
        public async Task<List<TeamDto>> GetAll(string name, int? year)
        {
            return await _teamAppService.GetAll(name, year);
        }

        /// <summary>
        /// 搜尋欲邀請成員名稱
        /// </summary>
        [HttpGet]
        [Route("users")]
        public async Task<List<AbpUserViewDto>> GetUsers(string name)
        {
            return await _teamAppService.GetUsers(name);
        }

        /// <summary>
        /// 團隊建立或資訊修改
        /// </summary>
        [HttpPost]
        [Route("data-post")]
        public async Task<TeamDto> DataPost([FromBody] CreateOrUpdateTeamDto input)
        {
            return await _teamAppService.DataPost(input);
        }

        /// <summary>
        /// 團隊刪除
        /// </summary>
        /// <param name="ids">Team id 的集合</param>
        [HttpPost]
        [Route("delete")]
        public async Task Delete([FromBody] List<Guid> ids)
        {
            foreach (var id in ids)
            {
                await _teamAppService.Delete(id);
            }
        }

        /// <summary>
        /// 發起邀請人進入團隊的請求
        /// </summary>
        [HttpPost]
        [Route("invite")]
        public async Task<List<TeamInvitationDto>> Invite([FromBody] List<CreateOrUpdateTeamInvitationDto> inputs)
        {
            return await _teamAppService.Invite(inputs);
        }

        /// <summary>
        /// 獲取邀請的條件
        /// 1. 受邀人為當前使用者
        /// 2. 邀請人為當前使用者
        /// </summary>
        [HttpGet]
        [Route("invites")]
        public async Task<List<TeamInvitationDto>> GetInvitations(int? state, string name)
        {
            return await _teamAppService.GetInvitations(state, name);
        }

        /// <summary>
        /// 取消團隊邀請請求
        /// </summary>
        /// <param name="id">invitation id</param>
        [HttpPost]
        [Route("invite/cancel")]
        public async Task CancelInvitation(Guid id)
        {
            await _teamAppService.CancelInvitation(id);
        }

        /// <summary>
        /// 接受團隊邀請請求
        /// </summary>
        /// <param name="id">invitation id</param>
        [HttpPost]
        [Route("invite/accept")]
        public async Task AcceptInvitation(Guid id)
        {
            await _teamAppService.AcceptInvitation(id);
        }

        /// <summary>
        /// 拒絕團隊邀請請求
        /// </summary>
        /// <param name="id">invitation id</param>
        [HttpPost]
        [Route("invite/decline")]
        public async Task DeclineInvitation(Guid id)
        {
            await _teamAppService.DeclineInvitation(id);
        }


        /// <summary>
        /// 將團隊中某人逐出
        /// </summary>
        [HttpPost]
        [Route("drop")]
        public async Task Drop([FromBody] DropFormData formData)
        {
            await _teamAppService.Drop(formData);
        }

        /// <summary>
        /// 邀請記錄匯出
        /// </summary>
        [HttpPost]
        [Route("export")]
        public async Task<IActionResult> Export(int? state, string name, string code)
        {
            var blobDto = await _teamAppService.Export(state, name, code);
            return File(blobDto.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                blobDto.Name);
        }
    }
}