using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nigel.SignalR.Client.Models
{
    public class BroadcastMessage
    {
        public string ConnectionId { get; set; }
        public string Content { get; set; }
    }
}
