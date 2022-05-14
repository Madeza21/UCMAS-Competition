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
    public partial class FrmResetPswd : Form
    {
        Peserta peserta;
        HttpRequest client = new HttpRequest();
        public FrmResetPswd(Peserta obj)
        {
            InitializeComponent();

            peserta = obj;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (peserta.TANGGAL_LAHIR == textBox6.Text)
                {
                    string msg = client.PostRequestChangePassword("api/changepassword/peserta", textBox1.Text, textBox5.Text);
                    if (msg == "Change password success")
                    {
                        MessageBox.Show(msg);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error");
                        return;
                    }
                }
                else
                {
                    if (Properties.Settings.Default.bahasa == "indonesia")
                    {
                        MessageBox.Show("Konfirmasi Tanggal Lahir beda dengan Tanggal Lahir yang di registrasi");
                    }
                    else
                    {
                        MessageBox.Show("Date of Birth Confirmation is different with Date of Birth on registration");
                    }
                    return;
                }
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

        private void FrmResetPswd_Load(object sender, EventArgs e)
        {
            textBox1.Text = peserta.ID_PESERTA;
            textBox2.Text = peserta.NAMA_PESERTA;
            textBox3.Text = peserta.EMAIL_PESERTA;
            textBox4.Text = peserta.ALAMAT_PESERTA;

            textBox5.Focus();
        }
    }
}
