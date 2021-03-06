using FlashCalculation.Help;
using FlashCalculation.Model;
using FlashCalculation.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation
{
    public partial class FrmLogin : Form
    {

        Url[] url;
        Cabang[] cabang;
        AppConfiguration[] config;
        SystemConfiguration[] sysconfig;

        Peserta peserta;

        DbBase db = new DbBase();
        HttpRequest client = new HttpRequest();
        SpeechSynthesizer speechSynthesizerObj;
        DataTable dtsp = new DataTable();

        string urlconfig, loadSpeech, textSpeech;
        bool isdispose = false, isload = false;
        public FrmLogin()
        {
            InitializeComponent();            
        }
              
        private void button2_Click(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ArrLogin login;

                string urllogin = "/api/login/pesertanew"; 

                if (chkTrial.Checked)
                {
                    //textBox1.Text = "TRL000000001";
                    //textBox2.Text = "Peserta Trial";
                    urllogin = "/api/kompetisitrial/searchnew";
                }

                if(textBox1.Text.Trim() == "")
                {
                    if(Properties.Settings.Default.bahasa == "indonesia")
                    {
                        MessageBox.Show("ID Peserta harus di isi.");
                    }
                    else
                    {
                        MessageBox.Show("Participant ID must be filled in.");
                    }
                    return;
                }

                if (textBox2.Text.Trim() == "")
                {
                    if (Properties.Settings.Default.bahasa == "indonesia")
                    {
                        MessageBox.Show("Kata sandi harus di isi.");
                    }
                    else
                    {
                        MessageBox.Show("Password must be filled in.");
                    }
                    return;
                }

                if (client.IsConnectedToInternet())
                {
                    login = client.PostRequestLogin(urllogin, textBox1.Text, textBox2.Text, comboBox1.SelectedValue.ToString());
                    config = client.PostRequestConfig(urlconfig, comboBox1.SelectedValue.ToString());

                    if (login.Status == "Error")
                    {
                        if (Properties.Settings.Default.bahasa == "indonesia")
                        {
                            if (login.data[0].message == "Lisensi cabang tidak valid")
                            {
                                MessageBox.Show("Lisensi cabang tidak valid");
                            }
                            else if (login.data[0].message == "Peserta tidak terdaftar kompetisi")
                            {
                                MessageBox.Show("Peserta tidak terdaftar kompetisi");
                            }
                            else if (login.data[0].message == "Tidak ada jadwal kompetisi peserta")
                            {
                                MessageBox.Show("Tidak ada jadwal kompetisi peserta");
                            }
                            else if (login.data[0].message == "Id peserta/Password tidak valid")
                            {
                                MessageBox.Show("Id peserta/Password tidak valid");
                            }
                        }
                        else
                        {
                            if (login.data[0].message == "Lisensi cabang tidak valid")
                            {
                                MessageBox.Show("Invalid branch license");
                            }
                            else if (login.data[0].message == "Peserta tidak terdaftar kompetisi")
                            {
                                MessageBox.Show("Participants are not registered in the competition");
                            }
                            else if (login.data[0].message == "Tidak ada jadwal kompetisi peserta")
                            {
                                MessageBox.Show("There is no participant competition schedule");
                            }
                            else if (login.data[0].message == "Id peserta/Password tidak valid")
                            {
                                MessageBox.Show("Invalid participant id/password");
                            }
                        }
                        return;
                    }
                    else
                    {
                        Properties.Settings.Default.token = login.data[0].token;
                        Properties.Settings.Default.siswa_id = textBox1.Text;
                        Properties.Settings.Default.voice = comboBox2.SelectedValue.ToString();
                        Properties.Settings.Default.cabang = comboBox1.SelectedValue.ToString();
                        Properties.Settings.Default.trial = chkTrial.Checked == true ? "Y" : "N";
                        Properties.Settings.Default.Save();
                    }

                    db.BeginTransaction();

                    //tb_peserta
                    if (login.peserta != null)
                    {
                        for (int i = 0; i < login.peserta.Length; i++)
                        {
                            db.Query("DELETE FROM tb_peserta where ID_PESERTA = '" + Encryptor.Encrypt(login.peserta[i].ID_PESERTA) + "'");
                        }

                        db.InsertPeserta(login.peserta);
                    }
                    //tb_kompetisi & tb_peserta_kompetisi
                    if (login.kompetisi != null)
                    {
                        if (chkTrial.Checked)
                        {
                            string strial = Encryptor.Encrypt("Y");
                            db.Query("DELETE FROM tb_kompetisi where IS_TRIAL = '" + strial + "'");
                        }
                        for (int i = 0; i < login.kompetisi.Length; i++)
                        {
                            db.Query("DELETE FROM tb_kompetisi where ROW_ID = '" + Encryptor.Encrypt(login.kompetisi[i].ROW_ID) + "'");
                            db.Query("DELETE FROM tb_parameter_kompetisi where ROW_ID_KOMPETISI = '" + Encryptor.Encrypt(login.kompetisi[i].ROW_ID) + "'");
                            db.Query("DELETE FROM tb_peserta_kompetisi where ROW_ID_KOMPETISI = '" + Encryptor.Encrypt(login.kompetisi[i].ROW_ID) + "' AND ID_PESERTA = '" + Encryptor.Encrypt(textBox1.Text) + "'");
                        }

                        db.InsertKompetisi(login.kompetisi);
                        db.InsertKompetisiPeserta(login.kompetisi);
                    }
                    //tb_parameter_kompetisi
                    if (login.parameterkompetisi != null)
                    {
                        db.InsertParameterKompetisi(login.parameterkompetisi);
                    }

                    UpdateDb("Login");
                    peserta = login.peserta[0];

                    db.commit();
                }
                else
                {
                    MessageBox.Show("Internet Disconnected");
                    return;
                }
                
                db.CloseConnection();
                timer1.Stop();

                if (!isdispose)
                {
                    //Gets the current speaking state of the SpeechSynthesizer object.   
                    if (speechSynthesizerObj.State == SynthesizerState.Speaking)
                    {
                        //close the SpeechSynthesizer object.   
                        speechSynthesizerObj.SpeakAsyncCancelAll();
                    }
                    speechSynthesizerObj.Dispose();
                }

                //Open Main Form
                this.Hide();
                FrmMain frm = new FrmMain(peserta);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    //Jika Berhasil
                    this.Show();
                }
                else
                {
                    Application.Exit();
                }

                //frm.Show();
                //this.DialogResult = DialogResult.OK;
                //this.Hide();
                //this.Close();
            }
            catch (Exception ex)
            {
                if (db.IsTransactionStarted())
                {
                    db.rollback();
                }
                MessageBox.Show(ex.Message);
            }            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (client.IsConnectedToInternet())
            {
                lblStatus.Text = "Internet Connected";
                lblStatus.ForeColor = Color.Green;

                if (!isload)
                {                    
                    LoadDataFromApi();
                    if (url != null)
                    {
                        UpdateDb("FrmLoad");
                    }

                    isload = true;
                }
            }
            else
            {
                lblStatus.Text = "Internet Disconnected";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                label10.Text = "Ver. " + Properties.Settings.Default.version;
                db.OpenConnection();
                //client.initialize();
                timer1.Start();
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;
                WaitSomeTime();

                /*if (client.IsConnectedToInternet())
                {
                    client.initialize();
                    LoadDataFromApi();
                    UpdateDb("FrmLoad");
                }*/

                LoadListSpeech();                
                loadSpeech = "N";
                SetImg();
                radioButton1_CheckedChanged(null, null);
                
                textBox1.Focus();
                if (sysconfig != null)
                {
                    if(sysconfig[0].APP_VERSION != Properties.Settings.Default.version)
                    {
                        MessageBox.Show("Please update application to version " + sysconfig[0].APP_VERSION);
                        CloseApp();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning!");
            }
            
        }

        private void UpdateDb(string flag)
        {
            if(flag == "FrmLoad")
            {
                //tb_url
                db.Query("DELETE FROM tb_url");
                db.InsertUrl(url);
                //tb_cabang
                db.Query("DELETE FROM tb_cabang");
                db.InsertCabang(cabang);
            }else if (flag == "Login")
            {
                //tb_application_configuration
                db.Query("DELETE FROM tb_application_configuration");
                db.InsertAppConfig(config);
            }
        }

        private void LoadListSpeech()
        {
            //https://www.codeproject.com/Articles/19334/Text-to-Speech-using-Windows-SAPI
            //IList<VoiceInfo> voiceInfos = new List<VoiceInfo>();
            speechSynthesizerObj = new SpeechSynthesizer();

            var installedVoices = speechSynthesizerObj.GetInstalledVoices();
            if (installedVoices.Count == 0)
            {
                MessageBox.Show(this,
                    "Your system don't have a 'Text to Speech' to make this work. Try install one for continue.",
                    "Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                loadSpeech = "Y";
                dtsp.Clear();
                dtsp.Columns.Add("Id", typeof(string));
                dtsp.Columns.Add("Name", typeof(string));
                dtsp.Columns.Add("Gender", typeof(string));

                foreach (InstalledVoice voice in installedVoices)
                {
                    //voiceInfos.Add(voice.VoiceInfo);
                    dtsp.Rows.Add(voice.VoiceInfo.Name, voice.VoiceInfo.Description.Replace("Microsoft ","").Replace(" (United States)", "").Replace(" (Indonesia)",""), voice.VoiceInfo.Gender);                    
                }

                dtsp.DefaultView.Sort = "Name ASC";
                dtsp = dtsp.DefaultView.ToTable();

                comboBox2.DataSource = dtsp;
                comboBox2.DisplayMember = "Name";
                comboBox2.ValueMember = "Id";
            }
            speechSynthesizerObj.Dispose();
            isdispose = true;

        }

        private void LoadDataFromApi()
        {
            try
            {
                url = client.GetRequestUrl("/api/url");
                if(url != null)
                {
                    for (int i = 0; i < url.Length; i++)
                    {
                        if (url[i].URL_CODE == "GetAllCabang")
                        {
                            cabang = client.GetRequestCabang(url[i].URL_PARAM);
                        }
                        else if (url[i].URL_CODE == "GetAllConfigCabang")
                        {
                            urlconfig = url[i].URL_PARAM;
                        }
                    }

                    Dictionary<string, string> item = new Dictionary<string, string>();
                    for (int i = 0; i < cabang.Length; i++)
                    {
                        item.Add(cabang[i].CABANG_CODE, cabang[i].CABANG_NAME);
                    }

                    comboBox1.DataSource = new BindingSource(item, null);
                    comboBox1.DisplayMember = "Value";
                    comboBox1.ValueMember = "Key";
                }
                sysconfig = client.GetRequestSysConfig("/api/sysconfig");
            }
            catch(Exception ex)
            {
                if (Properties.Settings.Default.bahasa == "indonesia")
                {
                    MessageBox.Show("Tidak ada akses internet");
                }
                else
                {
                    MessageBox.Show("Can't access internet");
                }                    
            }
        }

        private void SetImg()
        {
            for(int i = 0; i < dtsp.Rows.Count; i++)
            {
                if (comboBox2.SelectedValue.ToString() == dtsp.Rows[i]["Id"].ToString())
                {
                    if(dtsp.Rows[i]["Gender"].ToString() == "Female")
                    {
                        pictureBox3.Image = Properties.Resources.female;
                    }
                    else
                    {
                        pictureBox3.Image = Properties.Resources.male;
                    }

                    if (dtsp.Rows[i]["Name"].ToString().Contains("Indonesian"))
                    {
                        textSpeech = @"Namaku " + dtsp.Rows[i]["Name"].ToString() + @". Senang bertemu denganmu!
                                        Misalnya
                                        Apakah kamu siap
                                        sembilan ratus sepuluh
                                        enam ratus empat puluh delapan
                                        lima ratus enam puluh satu
                                        Itu adalah";
                    }
                    else
                    {
                        textSpeech = @"My Name is " + dtsp.Rows[i]["Name"].ToString() + @".It's nice to meet you!
                                        for example
                                        are you ready
                                        nine hundred ten
                                        six hundred forty eigh
                                        five hundred sixty one
                                        That is";
                    }
                    break;
                }
            }            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Properties.Settings.Default.bahasa = "indonesia";
                Properties.Settings.Default.Save();
                label2.Text = "ID Peserta";
                label3.Text = "Kata Sandi";
                label4.Text = "Cabang";
                label5.Text = "Bahasa";
                label6.Text = "Suara";
                label7.Text = "* Suara untuk kompetisi Listening";

                button1.Text = "Masuk";
                button2.Text = "Batal";

                chkTrial.Text = "Latihan";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                Properties.Settings.Default.bahasa = "english";
                Properties.Settings.Default.Save();
                label2.Text = "Participant ID";
                label3.Text = "Password";
                label4.Text = "Branch";
                label5.Text = "Language";
                label6.Text = "Voice";
                label7.Text = "* Voice for Listening Competition";

                button1.Text = "Sign In";
                button2.Text = "Cancel";

                chkTrial.Text = "Practice";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //Peserta perserta = new Peserta();

                peserta = client.PostRequestCheckPeserta("/api/user/GetPeserta", textBox1.Text);

                if (peserta.ID_PESERTA == null)
                {
                    if (Properties.Settings.Default.bahasa == "indonesia")
                    {
                        MessageBox.Show("ID Peserta tidak valid");
                    }
                    else
                    {
                        MessageBox.Show("Participant ID not valid");
                    }

                    return;
                }

                this.Hide();

                FrmResetPswd frm = new FrmResetPswd(peserta);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    //Jika Berhasil
                }
                else
                {
                    //Jika Batal
                }
                this.Show();
            }
            catch(Exception ex)
            {
                if (Properties.Settings.Default.bahasa == "indonesia")
                {
                    MessageBox.Show("Tidak ada akses internet");
                }
                else
                {
                    MessageBox.Show("Can't access internet");
                }
            }
            
        }

        private void chkTrial_CheckedChanged(object sender, EventArgs e)
        {
            if(chkTrial.Checked == true)
            {
                textBox1.Text = "TRL000000001";
                textBox2.Text = "Peserta Trial";
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetImg();
            if(loadSpeech != "Y")
            {
                speechSynthesizerObj.Dispose();
                speechSynthesizerObj = new SpeechSynthesizer();
                isdispose = false;
                speechSynthesizerObj.Volume = 100; // от 0 до 100
                speechSynthesizerObj.Rate = 0; //от -10 до 10
                speechSynthesizerObj.SelectVoice(comboBox2.SelectedValue.ToString());
                speechSynthesizerObj.SpeakAsync(textSpeech);
                //speechSynthesizerObj.SpeakCompleted += SpeakComplete;

                //MessageBox.Show("SELESAI Riyan Madeza");
            }
        }

        public void SpeakComplete(object sender, EventArgs e)
        {
            //MessageBox.Show("SELESAI");
        }

        public async void WaitSomeTime()
        {
            await Task.Delay(1200);
            this.Enabled = true;
            this.Cursor = Cursors.Default;

            if (url == null)
            {
                if (Properties.Settings.Default.bahasa == "indonesia")
                {
                    MessageBox.Show("Tidak ada akses ke server");
                }
                else
                {
                    MessageBox.Show("Can't access to server");
                }
            }
        }

        private void CloseApp()
        {
            try
            {
                timer1.Stop();
                db.CloseConnection();

                if (!isdispose)
                {
                    //Gets the current speaking state of the SpeechSynthesizer object.   
                    if (speechSynthesizerObj.State == SynthesizerState.Speaking)
                    {
                        //close the SpeechSynthesizer object.   
                        speechSynthesizerObj.SpeakAsyncCancelAll();
                    }
                    speechSynthesizerObj.Dispose();
                }

                this.DialogResult = DialogResult.Cancel;
                //this.Close();
                Application.Exit();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning!");
            }
            
        }
    }
}
