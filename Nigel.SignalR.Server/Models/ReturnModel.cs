using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nigel.SignalR.Server.Models
{
    public class ReturnModel
    {
        public int Code { get; set; }
        public string SubCode { get; set; }
        public string Message { get; set; }
        public string ObjectMessage { get; set; }
    }
}
