using FlashCalculation.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Help
{
    public class HttpRequest
    {
        static HttpClient client = new HttpClient();
        arrurl license = new arrurl();
        arrcabang cabang = new arrcabang();
        arrconfig config = new arrconfig();
        ArrLogin login = new ArrLogin();

        // Put the following code where you want to initialize the class
        // It can be the static constructor or a one-time initializer
        public void initialize()
        {
            client.BaseAddress = new Uri(Properties.Settings.Default.api_address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public bool IsConnectedToInternet()
        {
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(Properties.Settings.Default.ip_address, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { }
            return result;
        }

        public bool CheckForInternetConnection()
        {
            try 
            {
                var request = (HttpWebRequest)WebRequest.Create("https://"+Properties.Settings.Default.ip_address);
                request.KeepAlive = false;
                request.Timeout = 3000;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public Url[] GetRequestUrl(string url)
        {
            //https://stackoverflow.com/questions/32716174/call-and-consume-web-api-in-winform-using-c-net/32716351            

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    license = (arrurl)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(arrurl));
                }

                Url[] obj = new Url[license.license.Length];
                license.license.CopyTo(obj, 0);

                return obj;

            }
            catch(Exception ex)
            {
                new Exception("");
                return null;
            }    
        }

        public Cabang[] GetRequestCabang(string url)
        {    
            HttpResponseMessage response = new HttpResponseMessage();            
            response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                cabang = (arrcabang)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(arrcabang));
            }
            
            Cabang[] obj = new Cabang[cabang.cabang.Length];
            cabang.cabang.CopyTo(obj, 0);

            return obj;
        }

        public AppConfiguration[] PostRequestConfig(string url, string cabangcode)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            prmcabang prm = new prmcabang() {  CABANG_CODE = cabangcode };
            response = client.PostAsJsonAsync(url, prm).Result;

            if (response.IsSuccessStatusCode)
            {
                config = (arrconfig)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(arrconfig));
            }

            AppConfiguration[] obj = new AppConfiguration[config.config.Length];
            config.config.CopyTo(obj, 0);

            return obj;
        }

        public ArrLogin PostRequestLogin(string url, string id, string password, string cabangcode)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            prmlogin prm = new prmlogin() { ID_PESERTA = id, PASSWORD_PESERTA = password, CABANG_CODE = cabangcode };
            response = client.PostAsJsonAsync(url, prm).Result;

            if (response.IsSuccessStatusCode)
            {
                login = (ArrLogin)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(ArrLogin));
                login.Status = "OK";
            }
            else
            {
                login = (ArrLogin)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(ArrLogin));
                login.Status = "Error";
            }

            return login;
        }

        public Peserta PostRequestCheckPeserta(string url, string id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            prmlogin prm = new prmlogin() { ID_PESERTA = id };
            Peserta obj = new Peserta();
            CheckPeserta login = new CheckPeserta();

            response = client.PostAsJsonAsync(url, prm).Result;

            if (response.IsSuccessStatusCode)
            {
                login = (CheckPeserta)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(CheckPeserta));
                obj = login.peserta[0];
            }
            else
            {
                login = (CheckPeserta)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(CheckPeserta));
            }

            return obj;
        }

        public string PostRequestChangePassword(string url, string id, string password)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            prmlogin prm = new prmlogin() { ID_PESERTA = id, PASSWORD_PESERTA = password };
            
            OutputData data = new OutputData();

            response = client.PostAsJsonAsync(url, prm).Result;

            if (response.IsSuccessStatusCode)
            {
                data = (OutputData)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(OutputData)); 
            }
            else
            {
                data = (OutputData)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(OutputData));
            }
            return data.message;
        }

        public class prmlogin
        {
            public string ID_PESERTA { get; set; }
            public string PASSWORD_PESERTA { get; set; }
            public string CABANG_CODE { get; set; }
        }

        public class prmcabang
        {
            public string CABANG_CODE { get; set; }
        }

        public class arrurl
        {
            public Url[] license { get; set; }
        }

        public class arrcabang
        {
            public Cabang[] cabang { get; set; }
        }

        public class arrconfig
        {
            public AppConfiguration[] config { get; set; }
        }

        public class CheckPeserta
        {
            public OutputData[] data { get; set; }
            public Peserta[] peserta { get; set; }
        }

        public class OutputData
        {
            public string message { get; set; }
        }
    }
}
