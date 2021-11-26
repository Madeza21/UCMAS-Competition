using FlashCalculation.Help;
using FlashCalculation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation
{
    public partial class FrmMain : Form
    {
        Peserta peserta;
        DbBase db = new DbBase();
        private PrivateFontCollection pfc;
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(
         IntPtr pbFont,
         uint cbFont,
         IntPtr pdv,
         [In] ref uint pcFonts);

        public FrmMain(Peserta obj)
        {
            InitializeComponent();
            initCustomFont();

            peserta = obj;
        }

        private void initCustomFont()
        {
            pfc = new PrivateFontCollection();
            byte[] ucmaskgfont2 = Properties.Resources.ucmaskgfont_2;

            IntPtr num = Marshal.AllocCoTaskMem(ucmaskgfont2.Length);
            Marshal.Copy(ucmaskgfont2, 0, num, ucmaskgfont2.Length);
            uint pcFonts = 0;
            this.pfc.AddMemoryFont(num, Properties.Resources.ucmaskgfont_2.Length);
            FrmMain.AddFontMemResourceEx(num, (uint)Properties.Resources.ucmaskgfont_2.Length, IntPtr.Zero, ref pcFonts);
            Marshal.FreeCoTaskMem(num);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //Properties.Settings.Default.siswa_id = "TES UBAH";
            textBox10.Font = new Font(this.pfc.Families[0], 34, FontStyle.Bold);

            textBox1.Text = peserta.ID_PESERTA;
            label14.Text = peserta.ID_PESERTA;
            textBox2.Text = peserta.NAMA_PESERTA;
            label15.Text = peserta.NAMA_PESERTA;
            textBox3.Text = peserta.JENIS_KELAMIN;
            label16.Text = peserta.JENIS_KELAMIN == "L" ? "Laki-laki" : "Perempuan";
            textBox4.Text = peserta.TEMPAT_LAHIR;
            label17.Text = peserta.TEMPAT_LAHIR;
            textBox5.Text = peserta.TANGGAL_LAHIR;
            label18.Text = peserta.TANGGAL_LAHIR;
            textBox6.Text = peserta.SEKOLAH_PESERTA;
            label19.Text = peserta.SEKOLAH_PESERTA;
            textBox7.Text = peserta.EMAIL_PESERTA;
            label20.Text = peserta.EMAIL_PESERTA;
            textBox8.Text = peserta.NO_TELP_PESERTA;
            label21.Text = peserta.NO_TELP_PESERTA;
            textBox9.Text = peserta.ALAMAT_PESERTA;
            label13.Text = peserta.ALAMAT_PESERTA;

            db.OpenConnection();

            //set tittle
            this.Text = db.GetAppConfig("LBL");
            label1.Text = db.GetAppConfig("LBK");

            //Load Kompetisi
            DataTable dtkom = db.GetKompetisi(peserta.ID_PESERTA, DateTime.Now.ToString("yyyy-MM-dd"), Properties.Settings.Default.trial);
            comboBox1.DataSource = dtkom;
            comboBox1.DisplayMember = "KOMPETISI_NAME";
            comboBox1.ValueMember = "ROW_ID";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
