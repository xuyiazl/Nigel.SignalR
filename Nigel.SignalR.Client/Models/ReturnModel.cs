using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nigel.SignalR.Client.Models
{

    public class ReturnModel
    {
        public int Code { get; set; }
        public string SubCode { get; set; }
        public string Message { get; set; }
        public string ObjectMessage { get; set; }
    }

}
