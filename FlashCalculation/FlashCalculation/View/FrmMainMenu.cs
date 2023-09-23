using FlashCalculation.Help;
using FlashCalculation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation.View
{    
    public partial class FrmMainMenu : Form
    {
        Peserta peserta;
        DbBase db = new DbBase();
        private string ptitle;
        private FrmMain openForm;

        public FrmMainMenu(Peserta obj)
        {
            InitializeComponent();
            peserta = obj;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if(openForm != null)
            {
                if (openForm.lamalomba > 0)
                {
                    if (Properties.Settings.Default.bahasa == "indonesia")
                    {
                        MessageBox.Show("Waktu masih tersedia, aplikasi tidak dapat di tutup", "Informasi");
                    }
                    else
                    {
                        MessageBox.Show("Time is still available, the application cannot be closed", "Information");
                    }
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private void FrmMainMenu_Load(object sender, EventArgs e)
        {
            ttime.Start();
            db.OpenConnection();
            //client.initialize();

            //set tittle
            ptitle = db.GetAppConfig("LBL") + "  (Ver. " + Properties.Settings.Default.version + ") - " + peserta.NAMA_PESERTA +
                " ---> " + db.GetAppConfig("LBK");

            FrmHome main = new FrmHome(peserta.NAMA_PESERTA);
            main.TopLevel = false;
            pnlContent.Controls.Add(main);
            main.BringToFront();
            main.Show();
            //openForm = main;
            btnHome.BackColor = Color.FromArgb(64, 82, 100);
        }

        private void ttime_Tick(object sender, EventArgs e)
        {
            lblTitle.Text = ptitle + "  ||  " + DateTime.Now.ToString("dddd, dd-MMM-yyyy HH:mm:ss");
            if (openForm != null)
            {
                if (openForm.lamalomba > 0)
                {
                    btnHome.Enabled = false;
                    btnProfile.Enabled = false;
                    btnBasic.Enabled = false;
                    btnRandom.Enabled = false;
                }
                else
                {
                    btnHome.Enabled = true;
                    btnProfile.Enabled = true;
                    btnBasic.Enabled = true;
                    btnRandom.Enabled = true;
                }
            }            
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            pnlContent.Controls.Clear();

            btnHome.BackColor = Color.FromArgb(44, 62, 80);
            btnBasic.BackColor = Color.FromArgb(44, 62, 80);
            btnRandom.BackColor = Color.FromArgb(44, 62, 80);
            btnProfile.BackColor = Color.FromArgb(64, 82, 100);
            FrmProfile p = new FrmProfile(peserta);
            p.TopLevel = false;
            pnlContent.Controls.Add(p);
            p.BringToFront();
            p.Show();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            pnlContent.Controls.Clear();

            btnProfile.BackColor = Color.FromArgb(44, 62, 80);
            btnBasic.BackColor = Color.FromArgb(44, 62, 80);
            btnRandom.BackColor = Color.FromArgb(44, 62, 80);
            btnHome.BackColor = Color.FromArgb(64, 82, 100);
            FrmHome p = new FrmHome(peserta.NAMA_PESERTA);
            p.TopLevel = false;
            pnlContent.Controls.Add(p);
            p.BringToFront();
            p.Show();
        }

        private void btnBasic_Click(object sender, EventArgs e)
        {
            pnlContent.Controls.Clear();

            btnBasic.BackColor = Color.FromArgb(64, 82, 100);
            btnHome.BackColor = Color.FromArgb(44, 62, 80);
            btnRandom.BackColor = Color.FromArgb(44, 62, 80);
            btnProfile.BackColor = Color.FromArgb(44, 62, 80);
            FrmMain main = new FrmMain(peserta);
            main.TopLevel = false;
            pnlContent.Controls.Add(main);
            main.BringToFront();
            main.Show();
            openForm = main;
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            pnlContent.Controls.Clear();

            btnProfile.BackColor = Color.FromArgb(44, 62, 80);
            btnBasic.BackColor = Color.FromArgb(44, 62, 80);
            btnHome.BackColor = Color.FromArgb(44, 62, 80);
            btnRandom.BackColor = Color.FromArgb(64, 82, 100);
            FrmRandom p = new FrmRandom(peserta);
            p.TopLevel = false;
            pnlContent.Controls.Add(p);
            p.BringToFront();
            p.Show();
        }
    }
}
