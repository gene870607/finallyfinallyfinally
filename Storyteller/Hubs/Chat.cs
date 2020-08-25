using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storyteller.Hubs
{
    public class Chat : Hub
    {

        public async Task AddToGroup(string groupId,string myName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

            await Clients.Group(groupId).SendAsync("Send", $"{myName} 加入聊天.");
        }

        public Task SendMessage(string groupId, string userName, string message)
        {
            return Clients.Group(groupId).SendAsync("ReceiveGroupMessage", userName, message);
        }

        public override async Task OnConnectedAsync()
        {
            //連上線
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //斷線
            await base.OnDisconnectedAsync(exception);
        }
    }
}
