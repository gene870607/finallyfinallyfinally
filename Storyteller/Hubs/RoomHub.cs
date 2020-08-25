using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Storyteller.Interface;
using Storyteller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storyteller.Hubs
{
    [Flags]
    public enum ScriptStatus : ushort
    {
        End = 90,
        PerEnd = 98,
        
    }
    public class RoomHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            //連上線
            await base.OnConnectedAsync();
        }


        public async Task AddToGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

            await Clients.Group(groupId).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupId}.");
        }

        public async Task RemoveFromGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupId}.");
        }

        public Task SendMessageInGames(string groupId, string userName, string message, string rolesImg, int goToSectionIndex)
        {
            return Clients.Group(groupId).SendAsync("ReceiveGroupMessage", userName, message, rolesImg, goToSectionIndex);
        }
        public Task NoMessageEvent(string groupId, string userName, int goToSectionIndex)
        {
            return Clients.Group(groupId).SendAsync("NoMessageEvent", userName, goToSectionIndex);
        }

        public async Task PassResult(string groupId, int id, int index, string userName)
        {
            if (index > (int)ScriptStatus.End)
            {
                await Clients.Group(groupId).SendAsync("GoEnd", id, index);
            }
            else
            {

                await Clients.Group(groupId).SendAsync("GoToNext", id, index, userName);
            }

        }

        public async Task Like (string index, string groupId, string userName,bool IsLike)
        {
            await Clients.Group(groupId).SendAsync("BeLiked", index, userName, IsLike);
        }


    }
}