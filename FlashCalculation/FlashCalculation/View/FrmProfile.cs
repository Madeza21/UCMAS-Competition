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
    public partial class FrmProfile : Form
    {
        Peserta peserta;
        public FrmProfile(Peserta peserta)
        {
            InitializeComponent();
            this.peserta = peserta;
        }

        private void FrmProfile_Load(object sender, EventArgs e)
        {
            label14.Text = peserta.ID_PESERTA;
            label15.Text = peserta.NAMA_PESERTA;
            label16.Text = Properties.Settings.Default.trial == "Y" ? "-" : peserta.JENIS_KELAMIN == "L" ? "Laki-laki" : "Perempuan";
            label17.Text = peserta.TEMPAT_LAHIR;
            label18.Text = peserta.TANGGAL_LAHIR;
            label19.Text = peserta.SEKOLAH_PESERTA;
            label20.Text = peserta.EMAIL_PESERTA;
            label21.Text = peserta.NO_TELP_PESERTA;
            label13.Text = peserta.ALAMAT_PESERTA;

            TranslateControl();
        }

        private void TranslateControl()
        {
            if (Properties.Settings.Default.bahasa == "indonesia")
            {
                label1.Text = "ID Peserta";
                //label2.Text = "";
                label3.Text = "Nama";
                label4.Text = "Jenis Kelamin";
                label5.Text = "Tempat Lahir";
                label6.Text = "Tanggal Lahir";
                label7.Text = "Sekolah";
                label8.Text = "Email";
                label9.Text = "Telp";
                label10.Text = "Alamat";
            }
            else
            {
                label1.Text = "Participant ID";
                //label2.Text = "";
                label3.Text = "Name";
                label4.Text = "Gender";
                label5.Text = "Place of birth";
                label6.Text = "'Date of birth";
                label7.Text = "School";
                label8.Text = "Email";
                label9.Text = "Phone";
                label10.Text = "Address";
            }
        }
    }
}
