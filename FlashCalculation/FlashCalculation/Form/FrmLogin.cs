using FlashCalculation.Help;
using FlashCalculation.Model;
using Newtonsoft.Json;
using SpeechLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation
{
    public partial class FrmLogin : Form
    {
        static HttpClient client = new HttpClient();
        arrurl license = new arrurl();
        arrcabang cabangtemp = new arrcabang();

        public FrmLogin()
        {
            InitializeComponent();

            timer1.Start();

            // Put the following code where you want to initialize the class
            // It can be the static constructor or a one-time initializer
            client.BaseAddress = new Uri(Properties.Settings.Default.api_address);
                client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
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
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            //this.Close();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DbBase obj = new DbBase();

            DataTable dt = obj.GetCabang(comboBox1.SelectedValue.ToString());
            MessageBox.Show("Data : " + dt.Rows[0]["CABANG_NAME"].ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsConnectedToInternet())
            {
                lblStatus.Text = "Internet Connected";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = "Internet Disconnected";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            //https://www.codeproject.com/Articles/19334/Text-to-Speech-using-Windows-SAPI
            if (IsConnectedToInternet())
            {
                LoadDataFromApi();
            }

            LoadListSpeech();


            /**/
        }

        private void LoadListSpeech()
        {
            IList<VoiceInfo> voiceInfos = new List<VoiceInfo>();
            var reader = new SpeechSynthesizer();

            var installedVoices = reader.GetInstalledVoices();
            if (installedVoices.Count == 0)
            {
                MessageBox.Show(this,
                    "Your system don't have a 'Text to Speech' to make this work. Try install one for continue.",
                    "Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                foreach (InstalledVoice voice in installedVoices)
                {
                    voiceInfos.Add(voice.VoiceInfo);
                }
                comboBox2.DataSource = voiceInfos;
                comboBox2.DisplayMember = "Name";
                comboBox2.ValueMember = "Id";
            }

            /*reader.SelectVoice(comboBox2.DisplayMember); //[0,1] - std english synthesizers, [2] - Nikolay
            reader.Volume = 100; // от 0 до 100
            reader.Rate = 0; //от -10 до 10
            reader.SpeakAsync("TES");*/

            reader.Dispose();            
        }

        private async void LoadDataFromApi()
        {
            //https://stackoverflow.com/questions/32716174/call-and-consume-web-api-in-winform-using-c-net/32716351            

            HttpResponseMessage response = new HttpResponseMessage();

            response = await client.GetAsync("api/url");
            if (response.IsSuccessStatusCode)
            {
                license = (arrurl)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof (arrurl));
            }
            
            for(int i = 0; i < license.license.Length; i++)
            {
                if(license.license[i].URL_CODE == "GetAllCabang")
                {
                    response = await client.GetAsync(license.license[i].URL_PARAM);
                    if (response.IsSuccessStatusCode)
                    {
                        cabangtemp = (arrcabang)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(arrcabang));
                    }
                }
            }

            Dictionary<string, string> item = new Dictionary<string, string>();
            for (int i = 0; i < cabangtemp.cabang.Length; i++)
            {
                item.Add(cabangtemp.cabang[i].CABANG_CODE, cabangtemp.cabang[i].CABANG_NAME);
            }

            comboBox1.DataSource = new BindingSource(item, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            //var product = new Product() { Name = "P1", Price = 100, Category = "C1" };
            //var response = await client.PostAsJsonAsync("products", product);
        }        

        public class arrurl
        {
            public Url[] license { get; set; }
        }

        public class arrcabang
        {
            public Cabang[] cabang { get; set; }
        }
    }
}
