using Nigel.SignalR.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nigel.SignalR.Server.Hubs
{
    public interface IBroadcastHub
    {
        Task Push(BroadcastMessage message);
    }
}
