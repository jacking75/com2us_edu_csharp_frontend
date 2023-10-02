using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;

using System.ComponentModel;


namespace ServerMgrClient2.Client.Pages
{
    public partial class MainForm
    {
        protected override void OnInitialized()
        {
            severStatusList.Add(new()
            {
                Id = 0,
                HostName = "none",
                ServerName = "none",
                Version = 0,
                Status = "none",
                LBStatus = "none"
            });

            serverStatusTimer.Elapsed += (sender, eventArgs) => OnServerStatusTimerCallback();
            serverStatusTimer.Start();
        }

        public void Dispose() => serverStatusTimer.Dispose();


        private async Task OnServerStatusTimerCallback()
        {
            if (checkBoxValueAutoServerStatus)
            {
                var response = await HttpCallServerStatus();
                if (response != null)
                {
                    ChangeServerStatus(response);
                }
            }           
        }

        public async Task<ServerMgrClient2.Shared.HostsCollection> HttpCallServerStatus()
        {
            Console.WriteLine(@"[{Date.Now}] 타이머 호출");

            try
            {
                var response = await Http.PostAsJsonAsync<ServerMgrClient2.Shared.HostsCollection>("ServerStatus", null);

                var serverStatus = response.Content.ReadFromJsonAsync<ServerMgrClient2.Shared.HostsCollection>().Result;
                return serverStatus;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public void ChangeServerStatus(ServerMgrClient2.Shared.HostsCollection hostList)
        {
            severStatusList.Clear();

            for (var i = 0; i < hostList.Hosts.Count; ++i)
            {
                var programList = hostList.Hosts[i].Programs;
                for (var j = 0; j < programList.Count; ++j)
                {
                    severStatusList.Add(new()
                    {
                        Id = i + 1,
                        HostName = hostList.Hosts[i].Name,
                        ServerName = programList[j].Name,
                        Version = programList[j].Version,
                        Status = programList[j].Status,
                        LBStatus = programList[j].LBStatus
                    });
                }                
            }

            StateHasChanged();
        }


        public class ServerStatusData
        {
            [DisplayName("Id")]
            public int Id { get; set; }

            [DisplayName("Host Name")]
            public string HostName { get; set; }

            [DisplayName("Server Name")]
            public string ServerName { get; set; }

            [DisplayName("Version")]
            public int Version { get; set; }

            [DisplayName("Status")]
            public string Status { get; set; }

            [DisplayName("LB Status")]
            public string LBStatus { get; set; }
        }
    }
}
