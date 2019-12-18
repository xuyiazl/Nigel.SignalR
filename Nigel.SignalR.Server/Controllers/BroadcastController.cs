using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nigel.SignalR.Server.Hubs;
using Nigel.SignalR.Server.Models;

namespace Nigel.SignalR.Server.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class BroadcastController : ControllerBase
    {
        private IHubContext<BroadcastHub> messageHub;
        public BroadcastController(IHubContext<BroadcastHub> messageHub)
        {
            this.messageHub = messageHub;
        }

        [HttpPost]
        public async Task<ReturnModel> Push([FromBody]BroadcastMessage message)
        {
            ReturnModel res = new ReturnModel();
            try
            {
                await messageHub.Clients.All.SendAsync("BroadcastMessage", message);

                res.Code = 0;
                res.SubCode = "000001";
                res.Message = "推送成功";
                res.ObjectMessage = "";
            }
            catch (Exception e)
            {
                res.Code = -1;
                res.SubCode = "000002";
                res.Message = e.ToString();
                res.ObjectMessage = "";
            }

            return res;
        }
    }
}