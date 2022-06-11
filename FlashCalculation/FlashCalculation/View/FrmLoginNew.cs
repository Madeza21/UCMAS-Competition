using FlashCalculation.Help;
using FlashCalculation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation.View
{
    public partial class FrmLoginNew : Form
    {
        HttpRequest client = new HttpRequest();
        SystemConfiguration[] sysconfig;
        AppConfiguration[] config;

        Peserta peserta;

        DbBase db = new DbBase();

        public FrmLoginNew()
        {
            InitializeComponent();
        }

        private void FrmLoginNew_Load(object sender, EventArgs e)
        {
            try
            {
                label10.Text = "Ver. " + Properties.Settings.Default.version;
                client.initialize();

                if (client.IsConnectedToInternet())
                {
                    sysconfig = client.GetRequestSysConfig("/api/sysconfig");
                    if (sysconfig != null)
                    {
                        if (sysconfig[0].APP_VERSION != Properties.Settings.Default.version)
                        {
                            MessageBox.Show("Please update application to version " + sysconfig[0].APP_VERSION);
                            Application.Exit();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Tidak ada akses internet", "Warning!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ArrLogin login;
                string urllogin = "/api/kompetisitrial/searchnew";
                string user = "TRL000000001";
                string password = "Peserta Trial";
                string cabang = "UJA";

                if (client.IsConnectedToInternet())
                {
                    login = client.PostRequestLogin(urllogin, user,password, cabang);
                    config = client.PostRequestConfig("/api/config/search", cabang);

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
                        Properties.Settings.Default.siswa_id = user;
                        //Properties.Settings.Default.voice = comboBox2.SelectedValue.ToString();
                        Properties.Settings.Default.cabang = cabang;
                        Properties.Settings.Default.trial = "Y";
                        Properties.Settings.Default.Save();
                    }
                    db.OpenConnection();
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
                        string strial = Encryptor.Encrypt("Y");
                        db.Query("DELETE FROM tb_kompetisi where IS_TRIAL = '" + strial + "'");

                        db.InsertKompetisi(login.kompetisi);
                        db.InsertKompetisiPeserta(login.kompetisi);
                    }
                    //tb_parameter_kompetisi
                    if (login.parameterkompetisi != null)
                    {
                        db.InsertParameterKompetisi(login.parameterkompetisi);
                    }

                    db.Query("DELETE FROM tb_application_configuration");
                    db.InsertAppConfig(config);

                    peserta = login.peserta[0];
                }
                else
                {
                    db.CloseConnection();
                    MessageBox.Show("Internet Disconnected", "Warning!");
                    return;
                }

                db.CloseConnection();

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning!");
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Open login
            
            this.Hide();
            FrmLogin frm = new FrmLogin();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                //Jika Berhasil
                this.Show();
            }
            else
            {
                Application.Exit();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Application.Exit();
        }
    }
}
