using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ServerMgrClient
{
    public partial class MainPage : Page
    {
        List<HostInfo> _hostInfos = new List<HostInfo>();
        DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();

            // Enter construction logic here...

            // 데이터 그리드에 데이터 넣기
            _hostInfos.Add(new HostInfo() { Id=1, HostName="Host 01", ProgramName="Game Server" } );
            _hostInfos.Add(new HostInfo() { Id = 2, HostName = "Host 02", ProgramName = "Lobby Server" });

            dataGrid1.ItemsSource = _hostInfos;


            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Start();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        async void DispatcherTimer_Tick(object sender, object e)
        {
            if(isServerStatusAutoUpdate.IsChecked == true)
            {
                var response = await HttpRequestHostList();

                if (response != null)
                {
                    _hostInfos.Clear();
                    _hostInfos = response.HostList;

                    dataGrid1.ItemsSource = _hostInfos;
                }
            }
        }


        

        async Task<HttpReqHostListMsg> HttpRequestHostList()
        {
            try
            {
                var client = new HttpClient();
                var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
                var requestUrl = "http://localhost:11500/" + "HostList";
                var response = await client.PostAsync(requestUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    string resultContent = await response.Content.ReadAsStringAsync();                    
                    var hostListMsg = JsonConvert.DeserializeObject<HttpReqHostListMsg>(resultContent);                    
                    return hostListMsg;
                    //MessageBox.Show("OK");
                }
                else
                {                    
                    MessageBox.Show("Fail");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await HttpRequestHostList();

            if (response != null)
            {
                _hostInfos.Clear();
                _hostInfos = response.HostList;

                dataGrid1.ItemsSource = _hostInfos;
            }
        }

        // http 요청
        /*void HttpReqRequestServerMsg(HttpReqHostListMsg reqObject)
        {
            try
            {
                //Newtonsoft.JSON를 이용한 Json 다루기  https://nowonbun.tistory.com/403
                var json = JsonConvert.SerializeObject(reqObject);
                
                var client = new HttpClient();
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var requestUrl = "http://localhost:5360/" + "ReqHostList";
                var response = client.PostAsync(requestUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("OK");
                }
                else
                {
                    MessageBox.Show("Fail");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }*/
    }

    public class HostInfo
    {
        public int Id { get; set; }
        public string HostName { get; set; }
        public string ProgramName { get; set; }
    }

    class HttpReqHostListMsg
    {
        public List<HostInfo> HostList { get; set; } = new List<HostInfo>();
    }
}
