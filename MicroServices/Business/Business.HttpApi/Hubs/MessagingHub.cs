using System;
using System.Linq;
using System.Threading.Tasks;
using Business.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Domain.Repositories;

namespace Business.Hubs
{
    public class MessagingHub : AbpHub
    {
        private readonly IRepository<TeamMember> _repository;
        public MessagingHub(IRepository<TeamMember> repository)
        {
            _repository = repository;
        }

        public override Task OnConnectedAsync()
        {
            Logger.LogInformation($"User connected: {CurrentUser.Id}");
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string message, Guid teamId)
        {
            var query = await _repository.GetQueryableAsync();
            var userIds = await query.Where(x => x.TeamId == teamId).Select(x => x.UserId).ToListAsync();

            foreach (var userId in userIds)
            {
                await Clients.User(userId.ToString()).SendAsync("ReceiveMessage", CurrentUser.UserName, message, teamId);
            }
        }
    }
}
