using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace Business.Hubs
{
    public class MessagingHub : AbpHub
    {
        public async Task SendMessage(string targetUserName, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", targetUserName, message);
        }
    }
}
