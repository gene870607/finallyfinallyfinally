using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Storyteller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storyteller.Hubs
{
    public class PairingHub : Hub
    {
        public static List<PairingModel> roleList = new List<PairingModel>();
        public override async Task OnConnectedAsync()
        {
            //連上線
            await base.OnConnectedAsync();
        }
        public async Task CreateRole(string roleValue, string img)
        {
            roleList.Add(new PairingModel { ConnectionId = Context.ConnectionId, Role = roleValue, Img = img });
            var jsonString = JsonConvert.SerializeObject(roleList);
            await Clients.All.SendAsync("CreateRole", jsonString);
            await Clients.All.SendAsync("CountUsers", roleList.Count);
        }

        public async Task RemoveRole()
        {
            var user = roleList.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            roleList.Remove(user);
            var jsonString = JsonConvert.SerializeObject(roleList);
            await Clients.All.SendAsync("RemoveRole", jsonString, roleList.Count);
            await Clients.All.SendAsync("CountUsers", roleList.Count);
        }


        public async Task Pairing(string roleValue, string img)
        {

            var result = roleList.FirstOrDefault((x) => x.ConnectionId != Context.ConnectionId && x.Role != roleValue);
            var myself = roleList.FirstOrDefault((x) => x.ConnectionId != Context.ConnectionId);
            if (result != null)
            { 
                roleList.Remove(result);
                roleList.Remove(myself);
                string roomId = $"beanfun{DateTime.Now.GetHashCode()}";
                await Clients.Client(result.ConnectionId).SendAsync("PairingSuccess", result.Role, img, roomId);
                await Clients.Client(Context.ConnectionId).SendAsync("PairingSuccess", roleValue, result.Img, roomId);
                await Clients.All.SendAsync("CountUsers", roleList.Count);
            }

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //斷線
            var user = roleList.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            roleList.Remove(user);
            await Clients.Others.SendAsync("Leave", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
