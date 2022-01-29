using FlashCalculation.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
        arrsysconfig SysConfig = new arrsysconfig();
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

        public SystemConfiguration[] GetRequestSysConfig(string url)
        {
            //https://stackoverflow.com/questions/32716174/call-and-consume-web-api-in-winform-using-c-net/32716351            

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    SysConfig = (arrsysconfig)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(arrsysconfig));
                }

                SystemConfiguration[] obj = new SystemConfiguration[SysConfig.SystemConfig.Length];
                SysConfig.SystemConfig.CopyTo(obj, 0);

                return obj;

            }
            catch (Exception ex)
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

        public string PostRequestUpdateFlag(string url, string id)
        {
            HttpClient http = new HttpClient();

            http.BaseAddress = new Uri(Properties.Settings.Default.api_address);
            http.DefaultRequestHeaders.Accept.Clear();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.token);

            HttpResponseMessage response = new HttpResponseMessage();
            paramflag prm = new paramflag() { ID_PESERTA = Properties.Settings.Default.siswa_id, ROW_ID_KOMPETISI = id, FLAG = "Y"};

            OutputData data = new OutputData();

            response = http.PostAsJsonAsync(url, prm).Result;

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

        public string PostKirimJawaban(string url, DataRow dr)
        {
            HttpClient http = new HttpClient();

            http.BaseAddress = new Uri(Properties.Settings.Default.api_address);
            http.DefaultRequestHeaders.Accept.Clear();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.token);

            msginputjawaban obj = new msginputjawaban();

            HttpResponseMessage response = new HttpResponseMessage();
            inputjawaban prm = new inputjawaban() 
            {
                ROW_ID_KOMPETISI = dr["ROW_ID_KOMPETISI"].ToString(), ID_PESERTA = Properties.Settings.Default.siswa_id, 
                SOAL_NO = Convert.ToInt32(dr["SOAL_NO"].ToString()), 
                PERTANYAAN = dr["PERTANYAAN"].ToString(), JAWABAN_PESERTA = Convert.ToDecimal(dr["JAWABAN_PESERTA"].ToString()),
                JAWAB_DETIK_BERAPA = Convert.ToInt32(dr["JAWAB_DETIK_BERAPA"].ToString()), 
                JAWAB_DATE = Convert.ToDateTime(dr["JAWAB_DATE"].ToString()).ToString("yyyy-MM-dd hh:mm:ss"),
                KUNCI_JAWABAN = Convert.ToDecimal(dr["KUNCI_JAWABAN"].ToString()), SCORE_PESERTA = Convert.ToInt32(dr["SCORE_PESERTA"].ToString()),
                ENTRY_USER = Properties.Settings.Default.siswa_id, UPDATE_USER = Properties.Settings.Default.siswa_id
            };
            response = http.PostAsJsonAsync(url, prm).Result;

            if (response.IsSuccessStatusCode)
            {
                obj = (msginputjawaban)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(msginputjawaban));
            }
            else
            {
                obj = (msginputjawaban)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(msginputjawaban));
            }

            return obj.data[0].message;
        }

        public JawabanKompetisi[] PostGetJawaban(string url, string rowid)
        {
            HttpClient http = new HttpClient();

            http.BaseAddress = new Uri(Properties.Settings.Default.api_address);
            http.DefaultRequestHeaders.Accept.Clear();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.token);

            getjawaban obj = new getjawaban();

            HttpResponseMessage response = new HttpResponseMessage();
            paramjawaban prm = new paramjawaban()
            {
                ROW_ID_KOMPETISI = rowid,
                ID_PESERTA = Properties.Settings.Default.siswa_id,
                SOAL_NO = null
            };
            response = http.PostAsJsonAsync(url, prm).Result;

            if (response.IsSuccessStatusCode)
            {
                obj = (getjawaban)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(getjawaban));
            }
            else
            {
                obj = (getjawaban)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(getjawaban));
            }

            return obj.jawaban;
        }

        private class inputjawaban
        {
            public string ROW_ID_KOMPETISI { get; set; }
            public string ID_PESERTA { get; set; }
            public int SOAL_NO { get; set; }
            public string PERTANYAAN { get; set; }
            public decimal JAWABAN_PESERTA { get; set; }
            public int JAWAB_DETIK_BERAPA { get; set; }
            public string JAWAB_DATE { get; set; }
            public decimal KUNCI_JAWABAN { get; set; }
            public int SCORE_PESERTA { get; set; }
            public string ENTRY_USER { get; set; }
            public string UPDATE_USER { get; set; }

        }

        private class msginputjawaban
        {
            public OutputData[] data { get; set; }
        }

        private class paramjawaban
        {
            public string ROW_ID_KOMPETISI { get; set; }
            public string ID_PESERTA { get; set; }
            public int? SOAL_NO { get; set; }
        }

        private class paramflag
        {
            public string ID_PESERTA { get; set; }
            public string ROW_ID_KOMPETISI { get; set; }            
            public string FLAG { get; set; }
        }

        private class getjawaban
        {
            public OutputData[] data { get; set; }
            public JawabanKompetisi[] jawaban { get; set; }
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

        public class arrsysconfig
        {
            public SystemConfiguration[] SystemConfig { get; set; }
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
