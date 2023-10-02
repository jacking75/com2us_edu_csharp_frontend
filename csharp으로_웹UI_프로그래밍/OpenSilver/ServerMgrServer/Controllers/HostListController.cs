using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServerMsgServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HostListController : ControllerBase
    {
        [HttpPost]
        public async Task<HttpReqHostListMsg> Post()
        {
            Console.WriteLine($"[Request HostList] {DateTime.Now}");
            var response = new HttpReqHostListMsg();
            response.HostList.Add(new() { Id = 1, HostName = "P1", ProgramName = "Game Server 1" });
            response.HostList.Add(new() { Id = 2, HostName = "P2", ProgramName = "Lobby Server 1" });
            return response;
        }
    }


    public class HostInfo
    {
        public int Id { get; set; }
        public string HostName { get; set; }
        public string ProgramName { get; set; }
    }

    public class HttpReqHostListMsg
    {
        public List<HostInfo> HostList { get; set; } = new List<HostInfo>();
    }
}