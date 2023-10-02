using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerMgrClient2.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServerStatusController : ControllerBase
    {
        private readonly ILogger<ServerStatusController> _logger;

        public ServerStatusController(ILogger<ServerStatusController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public Shared.HostsCollection Post()
        {
            var response = getDummy();
            return response;
        }

        Shared.HostsCollection getDummy()
        {   
            var host1 = new Shared.Host();
            host1.Name = "host1";
            host1.Programs.Add(new Shared.SProgram() {
                Version = 1,
                Name = "P1",
                Status = "none",
                LBStatus = "on"
            });

            var host2 = new Shared.Host();
            host2.Name = "host1";
            host2.Programs.Add(new Shared.SProgram()
            {
                Version = 2,
                Name = "P2",
                Status = "none",
                LBStatus = "off"
            });


            var hostList = new List<Shared.Host>();
            hostList.Add(host1);
            hostList.Add(host2);

            
            var response = new Shared.HostsCollection();
            response.Hosts = hostList;
            return response;
        }
    }
}
