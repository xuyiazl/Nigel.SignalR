using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nigel.SignalR.Server.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class BroadcastMessage
    {
        public string ConnectionId { get; set; }
        public string Content { get; set; }
    }
}
