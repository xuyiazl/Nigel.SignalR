using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nigel.Core.HttpFactory;
using Nigel.SignalR.Client.Models;

namespace Nigel.SignalR.Client.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SignalController : ControllerBase
    {
        private readonly IHttpService _httpService;

        public SignalController(IHttpService httpService)
        {
            _httpService = httpService;
        }

        [HttpPost]
        public async Task<ReturnModel> Broadcast([FromBody] BroadcastMessage message)
        {
            var url = UrlArguments.Create("http://127.0.0.1:8080/api/Broadcast/Push");

            return await _httpService.PostAsync<ReturnModel, BroadcastMessage>(url, message);
        }
    }
}