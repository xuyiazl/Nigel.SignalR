using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Nigel.SignalR.Server.Models;

namespace Nigel.SignalR.Server.Hubs
{
    public class BroadcastHub : Hub, IBroadcastHub
    {
        const string groupName = "SignalRUsers";
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Push(BroadcastMessage message)
        {
            await Clients.All.SendAsync("BroadcastMessage", message);
        }
    }
}
