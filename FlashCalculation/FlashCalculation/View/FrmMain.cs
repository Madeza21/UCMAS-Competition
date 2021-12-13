using FlashCalculation.Help;
using FlashCalculation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
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

        DataTable dtSoal = new DataTable();
        Random rnd = new Random();
        CultureInfo culture = new CultureInfo("en-US");

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
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
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

            //Load dummy soal kompetisi
            dtSoal = db.GetSoalKompetisi("");
            SetSoalKompetisi();
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private string RandomAngka(int pdigit)
        {
            string ret = "";

            ret = rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString();

            return ret.Substring(0, pdigit);
        }

        private string RandomAngkaDec(int pdigit)
        {
            string ret = "";

            ret = rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() +
                rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString();

            return ret.Substring(0, pdigit);
        }

        private string RandomFlash(int soaldari, int soalsampai, int pjgdigit, int jmlrow, string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, decimal kecepatan)
        {
            string ret = "";
            try
            {
                decimal kunci = 0, decangkarandom = 0;
                int angkamuncul = 0, angkamunculke = 0;
                string strangka = "", strrandomprev = "", strrandom = "", strrandomformat = "";
                DataRow dr;

                for (int idx = soaldari; idx <= soalsampai; idx++)
                {
                    kunci = 0;
                    angkamuncul = jmlbarispermuncul;
                    angkamunculke = 0;
                    strangka = "";
                    strrandomprev = "0";
                    strrandom = "0";

                    dr = (DataRow)dtSoal.NewRow();

                    for (int row = 1; row <= jmlrow; row++)
                    {
                        strrandom = RandomAngka(pjgdigit);
                        //Angka sama
                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                        {
                            strrandom = RandomAngka(pjgdigit);
                            //Angka sama 2
                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                            {
                                strrandom = RandomAngka(pjgdigit);
                                //Angka sama 3
                                if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                {
                                    strrandom = RandomAngka(pjgdigit);
                                    //Angka sama 4
                                    if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                    {
                                        strrandom = RandomAngka(pjgdigit);
                                        //Angka sama 5
                                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                        {
                                            strrandom = RandomAngka(pjgdigit);
                                            //Angka sama 6
                                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                            {
                                                strrandom = RandomAngka(pjgdigit);
                                                //Angka sama 7
                                                if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                {
                                                    strrandom = RandomAngka(pjgdigit);
                                                    //Angka sama 8
                                                    if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                    {
                                                        strrandom = RandomAngka(pjgdigit);
                                                        //Angka sama 9
                                                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                        {
                                                            strrandom = RandomAngka(pjgdigit);
                                                            //Angka sama 10
                                                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                            {
                                                                strrandom = RandomAngka(pjgdigit);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (munculminus == "Y")
                        {
                            if (Convert.ToDecimal(strrandomprev, culture) > Convert.ToDecimal(strrandom, culture))
                            {
                                strrandom = '-' + strrandom;
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                            }
                            else
                            {
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                            }
                        }
                        else
                        {
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                        }

                        decangkarandom = Convert.ToDecimal(strrandom, culture);
                        kunci = kunci + decangkarandom;
                        strrandomprev = strrandom;

                        strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###"); //("#,#",CultureInfo.InvariantCulture)
                        strangka = strangka + strrandomformat + Environment.NewLine;

                        if (angkamuncul == row)
                        {
                            angkamunculke = angkamunculke + 1;
                            angkamuncul = angkamuncul + jmlbarispermuncul;
                            dr["angkamuncul" + angkamunculke.ToString()] = strangka;
                            strangka = "";
                        }
                    }

                    dr["kunci_jawaban"] = kunci;
                    dr["row_id_kompetisi"] = idperlombaan;
                    dr["no_soal"] = idx;
                    dr["jml_baris_per_muncul"] = jmlbarispermuncul;
                    dr["jumlah_muncul"] = jmlmuncul;
                    dr["kecepatan"] = kecepatan;
                    dr["max_jml_digit_per_soal"] = 0;

                    dtSoal.Rows.Add(dr);
                }

                strrandomprev = "";
                strrandom = "";
            }
            catch (Exception e)
            {
                MessageBox.Show("Error : " + e.Message + " On Soal Dari : " + soaldari.ToString(), "Visual");
            }

            return ret;
        }

        private string RandomListening(int soaldari, int soalsampai, int pjgdigit, int jmlrow, string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, decimal kecepatan)
        {
            string ret = "";
            try
            {
                decimal kunci = 0, decangkarandom = 0;
                int angkamuncul = 0, angkamunculke = 0;
                string strangka = "", strrandomprev = "", strrandom = "";
                string strangkalisten = "";
                bool bminus = false;

                DataRow dr;

                for (int idx = soaldari; idx <= soalsampai; idx++)
                {
                    kunci = 0;
                    angkamuncul = jmlbarispermuncul;
                    angkamunculke = 0;
                    strangka = "";
                    strangkalisten = "";
                    strrandomprev = "0";
                    strrandom = "0";
                    bminus = false;

                    dr = (DataRow)dtSoal.NewRow();

                    for (int row = 1; row <= jmlrow; row++)
                    {
                        strrandom = RandomAngka(pjgdigit);
                        //Angka sama
                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                        {
                            strrandom = RandomAngka(pjgdigit);
                            //Angka sama 2
                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                            {
                                strrandom = RandomAngka(pjgdigit);
                                //Angka sama 3
                                if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                {
                                    strrandom = RandomAngka(pjgdigit);
                                    //Angka sama 4
                                    if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                    {
                                        strrandom = RandomAngka(pjgdigit);
                                        //Angka sama 5
                                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                        {
                                            strrandom = RandomAngka(pjgdigit);
                                            //Angka sama 6
                                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                            {
                                                strrandom = RandomAngka(pjgdigit);
                                                //Angka sama 7
                                                if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                {
                                                    strrandom = RandomAngka(pjgdigit);
                                                    //Angka sama 8
                                                    if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                    {
                                                        strrandom = RandomAngka(pjgdigit);
                                                        //Angka sama 9
                                                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                        {
                                                            strrandom = RandomAngka(pjgdigit);
                                                            //Angka sama 10
                                                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                            {
                                                                strrandom = RandomAngka(pjgdigit);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (munculminus == "Y")
                        {
                            if (Convert.ToDecimal(strrandomprev, culture) > Convert.ToDecimal(strrandom, culture))
                            {
                                strangka = strangka + "minus " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(-" + strrandom + ") " + "minus " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;

                                bminus = true;

                                strrandom = '-' + strrandom;
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                            }
                            else
                            {
                                if (bminus)
                                {
                                    strangka = strangka + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                    strangkalisten = strangkalisten + "(" + strrandom + ") " + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                }
                                else
                                {
                                    strangka = strangka + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                    strangkalisten = strangkalisten + "(" + strrandom + ") " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                }

                                bminus = false;
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                            }
                        }
                        else
                        {
                            if (bminus)
                            {
                                strangka = strangka + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(" + strrandom + ") " + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                            }
                            else
                            {
                                strangka = strangka + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(" + strrandom + ") " + NumberInWordEnglish(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                            }

                            bminus = false;
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                        }

                        decangkarandom = Convert.ToDecimal(strrandom, culture);
                        kunci = kunci + decangkarandom;
                        strrandomprev = strrandom;

                        //strrandomformat = Convert.ToInt32(strrandom).ToString("#,#"); //("#,#",CultureInfo.InvariantCulture)
                        //strangka = strangka + strrandomformat + Environment.NewLine;

                        if (angkamuncul == row)
                        {
                            angkamunculke = angkamunculke + 1;
                            angkamuncul = angkamuncul + jmlbarispermuncul;
                            dr["angkamuncul" + angkamunculke.ToString()] = strangka;
                            dr["angkalistening" + angkamunculke.ToString()] = strangkalisten;
                            strangka = "";
                            strangkalisten = "";
                        }
                    }

                    dr["kunci_jawaban"] = kunci;
                    dr["row_id_kompetisi"] = idperlombaan;
                    dr["no_soal"] = idx;
                    dr["jml_baris_per_muncul"] = jmlbarispermuncul;
                    dr["jumlah_muncul"] = jmlmuncul;
                    dr["kecepatan"] = kecepatan;
                    dr["max_jml_digit_per_soal"] = 0;

                    dtSoal.Rows.Add(dr);
                }

                strrandomprev = "";
                strrandom = "";
            }
            catch (Exception e)
            {
                MessageBox.Show("Error : " + e.Message + " On Soal Dari : " + soaldari.ToString(), "Listening");
            }            

            return ret;
        }

        private string RandomVisual(int soaldari, int soalsampai, int pjgdigit, int jmlrow, string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, 
            string munculperkalian, int digitperkalian, string munculpembagian, int digitpembagian, string munculdec, int digitdec, decimal kecepatan, int maxdigitsoal)
        {
            string ret = "";
            try
            {
                decimal kunci = 0, decangkarandom = 0, dtest = 0, dmod = 0;
                int angkamuncul = 0, angkamunculke = 0, totaldigitpersoal = 0;
                string strangka = "", strrandomprev = "", strrandom = "", strrandomformat = "";
                string strrandomdecimal = "", strrandomperkalian = "", strrandompembagian = "";
                DataRow dr;

                for (int idx = soaldari; idx <= soalsampai; idx++)
                {
                    kunci = 0;
                    angkamuncul = jmlbarispermuncul;
                    angkamunculke = 0;
                    strangka = "";
                    strrandomprev = "0";
                    strrandom = "0";
                    totaldigitpersoal = 0;

                    dr = (DataRow)dtSoal.NewRow();
                    /*if(soaldari == 151)
                    {
                        strangka = "";
                    }*/

                    for (int row = 1; row <= jmlrow; row++)
                    {
                        strrandom = RandomAngka(pjgdigit);
                        totaldigitpersoal = totaldigitpersoal + pjgdigit;

                        //Angka sama
                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                        {
                            strrandom = RandomAngka(pjgdigit);
                            //Angka sama 2
                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                            {
                                strrandom = RandomAngka(pjgdigit);
                                //Angka sama 3
                                if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                {
                                    strrandom = RandomAngka(pjgdigit);
                                    //Angka sama 4
                                    if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                    {
                                        strrandom = RandomAngka(pjgdigit);
                                        //Angka sama 5
                                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                        {
                                            strrandom = RandomAngka(pjgdigit);
                                            //Angka sama 6
                                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                            {
                                                strrandom = RandomAngka(pjgdigit);
                                                //Angka sama 7
                                                if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                {
                                                    strrandom = RandomAngka(pjgdigit);
                                                    //Angka sama 8
                                                    if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                    {
                                                        strrandom = RandomAngka(pjgdigit);
                                                        //Angka sama 9
                                                        if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                        {
                                                            strrandom = RandomAngka(pjgdigit);
                                                            //Angka sama 10
                                                            if ((strrandomprev == strrandom) || (strrandomprev.Substring(0, 1) == strrandom.Substring(0, 1)) || (strrandomprev.Substring(strrandomprev.Length - 1) == strrandom.Substring(strrandom.Length - 1)))
                                                            {
                                                                strrandom = RandomAngka(pjgdigit);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (munculpembagian == "Y" || munculperkalian == "Y")
                        {
                            //Pembagian
                            if (munculpembagian == "Y")
                            {
                                //random 						
                                strrandompembagian = RandomAngkaDec(digitpembagian);
                                if (strrandompembagian.Trim() == "1")
                                {
                                    strrandompembagian = RandomAngkaDec(digitpembagian);
                                    if (strrandompembagian.Trim() == "1")
                                    {
                                        strrandompembagian = RandomAngkaDec(digitpembagian);
                                    }
                                }

                                dtest = Convert.ToDecimal(strrandom, culture) % Convert.ToDecimal(strrandompembagian, culture);
                                if (dtest == 0)
                                {

                                }
                                else
                                {
                                    dmod = (Convert.ToDecimal(strrandom, culture) - dtest);
                                    dtest = dmod % Convert.ToDecimal(strrandompembagian, culture);
                                    strrandom = dmod.ToString();
                                }
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                                dr["pembagian" + row.ToString()] = Convert.ToDecimal(strrandompembagian, culture);

                                //kunci jawaban
                                decangkarandom = Convert.ToDecimal(strrandom, culture);
                                kunci = decangkarandom / Convert.ToDecimal(strrandompembagian, culture);
                                strrandomprev = strrandom;

                                strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###");
                                strangka = strangka + strrandomformat + " ÷ " + strrandompembagian + Environment.NewLine;

                                if (angkamuncul == row)
                                {
                                    angkamunculke = angkamunculke + 1;
                                    angkamuncul = angkamuncul + jmlbarispermuncul;
                                    dr["angkamuncul" + angkamunculke.ToString()] = strangka;
                                    strangka = "";
                                }
                            }
                            //PERKALIAN
                            if (munculperkalian == "Y")
                            {
                                //angka digit
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                                strrandomperkalian = RandomAngkaDec(digitperkalian);
                                if (strrandomperkalian.Trim() == "1")
                                {
                                    strrandomperkalian = RandomAngkaDec(digitperkalian);
                                    if (strrandomperkalian.Trim() == "1")
                                    {
                                        strrandomperkalian = RandomAngkaDec(digitperkalian);
                                    }
                                }
                                dr["perkalian" + row.ToString()] = Convert.ToDecimal(strrandomperkalian, culture);

                                //kunci jawaban
                                decangkarandom = Convert.ToDecimal(strrandom, culture);
                                kunci = (decangkarandom * Convert.ToDecimal(strrandomperkalian, culture));
                                strrandomprev = strrandom;

                                strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###");
                                strangka = strangka + strrandomformat + " x " + strrandomperkalian + Environment.NewLine;

                                if (angkamuncul == row)
                                {
                                    angkamunculke = angkamunculke + 1;
                                    angkamuncul = angkamuncul + jmlbarispermuncul;
                                    dr["angkamuncul" + angkamunculke.ToString()] = strangka;
                                    strangka = "";
                                }
                            }
                        }
                        else
                        {
                            if (munculdec == "Y")
                            {
                                strrandomdecimal = RandomAngkaDec(digitdec);
                                strrandom = strrandom + "." + strrandomdecimal;
                            }

                            if (munculminus == "Y")
                            {
                                if (Convert.ToDecimal(strrandomprev, culture) > Convert.ToDecimal(strrandom, culture))
                                {
                                    strrandom = '-' + strrandom;
                                    dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                                }
                                else
                                {
                                    dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                                }
                            }
                            else
                            {
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                            }

                            //kunci jawaban
                            decangkarandom = Convert.ToDecimal(strrandom, culture);
                            kunci = kunci + decangkarandom;
                            strrandomprev = strrandom;

                            if (munculdec == "Y")
                            {
                                if (digitdec == 1)
                                {
                                    strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###.#");
                                }
                                else if (digitdec == 2)
                                {
                                    strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###.##");
                                }
                                else if (digitdec == 3)
                                {
                                    strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###.###");
                                }
                                else if (digitdec == 4)
                                {
                                    strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###.####");
                                }
                                else if (digitdec == 5)
                                {
                                    strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###.#####");
                                }
                                else if (digitdec == 6)
                                {
                                    strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###.######");
                                }
                            }
                            else
                            {
                                strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###");
                            }

                            strangka = strangka + strrandomformat + Environment.NewLine;
                            if (angkamuncul == row)
                            {
                                angkamunculke = angkamunculke + 1;
                                angkamuncul = angkamuncul + jmlbarispermuncul;
                                dr["angkamuncul" + angkamunculke.ToString()] = strangka;
                                strangka = "";
                            }
                        }
                    }

                    dr["kunci_jawaban"] = kunci;
                    dr["row_id_kompetisi"] = idperlombaan;
                    dr["no_soal"] = idx;
                    dr["jml_baris_per_muncul"] = jmlbarispermuncul;
                    dr["jumlah_muncul"] = jmlmuncul;
                    dr["kecepatan"] = kecepatan;
                    dr["max_jml_digit_per_soal"] = maxdigitsoal;
                    dr["total_digit_per_soal"] = totaldigitpersoal;
                    dr["muncul_angka_decimal"] = munculdec;
                    dr["digit_decimal"] = digitdec;

                    dtSoal.Rows.Add(dr);
                }

                strrandomprev = "";
                strrandom = "";
            }
            catch (Exception e)
            {
                MessageBox.Show("Error : " + e.Message + " On Soal Dari : " + soaldari.ToString(), "Visual");
            }            

            return ret;
        }

        private string NumberTranslate(int amount)
        {
            string ret = "";
            string[] aUnit, aTen; 
            int nHundred, nTen;

            aUnit = new string[19]{ "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };

            aTen = new string[9] { "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if(amount > 100)
            {
                nHundred = amount / 100;
                ret += aUnit[nHundred - 1] + " hundred ";
                if (amount > 0) amount -= nHundred * 100;
            }

            if (amount >= 20)
            {
                nTen = amount / 10;
                ret += aTen[nTen - 1] + " ";
                amount -= nTen * 10;
                if (amount > 0) ret += aUnit[amount - 1];
            }
            else
            {
                if (amount > 0) ret += aUnit[amount - 1];
            }

            return ret;
        }

        private string NumberInWordEnglish(decimal amount)
        {
            string str = "", str2;
            decimal nTrillion, nMillion, nThousand;

            // cannot greater then 999,999,999,999,997
            if (amount > 999999999999997) return "OUT OF RANGE";
            if (amount < 0) return "OUT OF RANGE";
            if (amount < 1)
            {
                str = "zero";
                str2 = "";
            }
            else
            {
                //str2 = "s"
                str2 = "";
            }

            /*var f = 0f; // float
              var d = 0d; // double
              var m = 0m; // decimal (money)
              var u = 0u; // unsigned int
              var l = 0l; // long
              var ul = 0ul; // unsigned long*/
            if(amount >= 1000000000000.00M)
            {
                nTrillion = amount / 1000000000000.00M;
                str += NumberTranslate(Convert.ToInt32(nTrillion)) + " trillion ";
                amount -= (nTrillion * 1000000000000.00M);
            }
            if (amount >= 1000000000)
            {
                nTrillion = amount / 1000000000;
                str += NumberTranslate(Convert.ToInt32(nTrillion)) + " billion ";
                amount -= (nTrillion * 1000000000);
            }
            if (amount >= 1000000)
            {
                nTrillion = amount / 1000000;
                str += NumberTranslate(Convert.ToInt32(nTrillion)) + " million ";
                amount -= (nTrillion * 1000000);
            }
            if (amount >= 1000)
            {
                nTrillion = amount / 1000;
                str += NumberTranslate(Convert.ToInt32(nTrillion)) + " thousand ";
                amount -= (nTrillion * 1000);
            }

            str += NumberTranslate(Convert.ToInt32(amount));

            str += str2;
            amount -= amount;
            if (amount > 0)
            {
                amount *= 100;
                //str += " and " + f_Translate(amount) + " sen" --> tulisan sen
                str += " point " + NumberTranslate(Convert.ToInt32(amount));
            }

            return str;
        }

        private string Thanks(string idperlombaan, int row)
        {
            string ret = "";
            DataRow dr;
            dr = (DataRow)dtSoal.NewRow();

            dr["row_id_kompetisi"] = idperlombaan;
            dr["no_soal"] = row;
            dr["angkamuncul1"] = "Thanks";
            dr["jml_baris_per_muncul"] = 1;
            dr["jumlah_muncul"] = 1;
            dr["kecepatan"] = 1;
            dr["max_jml_digit_per_soal"] = 0;

            dtSoal.Rows.Add(dr);

            return ret;
        }

        private void ZigZag()
        {
            string angkamuncul1, munculangkadecimal, randomformat, angka1, angka;
            int maxkarakter, totalkarakter, decrease, digitdecimal;
            decimal kuncijwb, angkax, dangka;

            if(dtSoal.Rows.Count > 0)
            {
                for(int i = 0; i < dtSoal.Rows.Count; i++)
                {
                    angkamuncul1 = dtSoal.Rows[i]["angkamuncul1"].ToString();
                    maxkarakter = dtSoal.Rows[i]["max_jml_digit_per_soal"].ToString() == "" ? 0 : Convert.ToInt32(dtSoal.Rows[i]["max_jml_digit_per_soal"].ToString());
                    
                    if (maxkarakter > 0)
                    {

                    }
                    else
                    {
                        continue;
                    }

                    totalkarakter = dtSoal.Rows[i]["total_digit_per_soal"].ToString() == "" ? 0 : Convert.ToInt32(dtSoal.Rows[i]["total_digit_per_soal"].ToString());
                    decrease = totalkarakter - maxkarakter;
                    munculangkadecimal = dtSoal.Rows[i]["muncul_angka_decimal"].ToString();
                    if(munculangkadecimal != "Y")
                    {
                        munculangkadecimal = "N";
                    }
                    digitdecimal = dtSoal.Rows[i]["digit_decimal"].ToString() == "" ? 0 : Convert.ToInt32(dtSoal.Rows[i]["digit_decimal"].ToString());
                    kuncijwb = 0;
                    randomformat = "";
                    angka1 = "";

                    if (angkamuncul1 == "Thanks")
                    {
                        continue;
                    }
                    if (angkamuncul1.Contains("x"))
                    {
                        continue;
                    }
                    if (angkamuncul1.Contains("÷"))
                    {
                        continue;
                    }

                    if (decrease > 0)
                    {
                        //minus
                        for(int x = 1; x <= 150; x++)
                        {
                            if(munculangkadecimal == "Y")
                            {
                                angkax = Decimal.Round(dtSoal.Rows[i]["angka" + x.ToString()].ToString() == "" ? 0 : Convert.ToDecimal(dtSoal.Rows[i]["angka" + x.ToString()].ToString(), culture), digitdecimal);                                
                            }
                            else
                            {
                                angkax = dtSoal.Rows[i]["angka" + x.ToString()].ToString() == "" ? 0 : Convert.ToDecimal(dtSoal.Rows[i]["angka" + x.ToString()].ToString(), culture);
                            }
                            angka = angkax.ToString();
                            if (angka.Contains("-"))
                            {
                                if (munculangkadecimal == "Y")
                                {
                                    angka = angka.Substring(0, angka.IndexOf(".") - 2) + angka.Substring(angka.IndexOf("."), angka.Length);
                                }
                                else
                                {
                                    angka = angka.Substring(0, angka.Length - 1);
                                }

                                dtSoal.Rows[i]["angka" + x.ToString()] = angka;
                                decrease = decrease - 1;
                                if(decrease < 1)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    if (decrease > 0)
                    {
                        //plus
                        for (int x = 1; x <= 150; x++)
                        {
                            if (munculangkadecimal == "Y")
                            {
                                angkax = Decimal.Round(dtSoal.Rows[i]["angka" + x.ToString()].ToString() == "" ? 0 : Convert.ToDecimal(dtSoal.Rows[i]["angka" + x.ToString()].ToString(), culture), digitdecimal);
                            }
                            else
                            {
                                angkax = dtSoal.Rows[i]["angka" + x.ToString()].ToString() == "" ? 0 : Convert.ToDecimal(dtSoal.Rows[i]["angka" + x.ToString()].ToString(), culture);
                            }
                            angka = angkax.ToString();
                            if (angka.Contains("-"))
                            {
                                continue;
                            }
                            else
                            {
                                if (munculangkadecimal == "Y")
                                {
                                    angka = angka.Substring(0, angka.IndexOf(".") - 2) + angka.Substring(angka.IndexOf("."), angka.Length);
                                }
                                else
                                {
                                    angka = angka.Substring(0, angka.Length - 1);
                                }

                                dtSoal.Rows[i]["angka" + x.ToString()] = angka;
                                decrease = decrease - 1;
                                if (decrease < 1)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    //Generate Soal dan kunci jawaban
                    for (int x = 1; x <= 150; x++)
                    {
                        if (munculangkadecimal == "Y")
                        {
                            angkax = Decimal.Round(dtSoal.Rows[i]["angka" + x.ToString()].ToString() == "" ? 0 : Convert.ToDecimal(dtSoal.Rows[i]["angka" + x.ToString()].ToString(), culture), digitdecimal);
                        }
                        else
                        {
                            angkax = dtSoal.Rows[i]["angka" + x.ToString()].ToString() == "" ? 0 : Convert.ToDecimal(dtSoal.Rows[i]["angka" + x.ToString()].ToString(), culture);
                        }
                        angka = angkax.ToString();
                        if(angka == "")
                        {
                            dangka = 0;
                        }
                        else
                        {
                            dangka = Decimal.Round(dtSoal.Rows[i]["angka" + x.ToString()].ToString() == "" ? 0 : Convert.ToDecimal(dtSoal.Rows[i]["angka" + x.ToString()].ToString(), culture), digitdecimal);
                            if (munculangkadecimal == "Y")
                            {
                                if (digitdecimal == 1)
                                {
                                    randomformat = dangka.ToString("###,###,###.#");
                                }
                                else if (digitdecimal == 2)
                                {
                                    randomformat = dangka.ToString("###,###,###.##");
                                }
                                else if (digitdecimal == 3)
                                {
                                    randomformat = dangka.ToString("###,###,###.###");
                                }
                                else if (digitdecimal == 4)
                                {
                                    randomformat = dangka.ToString("###,###,###.####");
                                }
                                else if (digitdecimal == 5)
                                {
                                    randomformat = dangka.ToString("###,###,###.#####");
                                }
                                else if (digitdecimal == 6)
                                {
                                    randomformat = dangka.ToString("###,###,###.######");
                                }
                            }
                            else
                            {
                                randomformat = dangka.ToString("###,###,###");
                            }
                            angka1 = angka1 + randomformat + Environment.NewLine;
                        }
                        kuncijwb = kuncijwb + dangka;
                    }
                    dtSoal.Rows[i]["kunci_jawaban"] = kuncijwb;
                    dtSoal.Rows[i]["angkamuncul1"] = angka1;
                }
            }
        }

        private void SetSoalKompetisi()
        {
            try
            {
                string idperlombaan, parameterid, munculangkaminus, munculangkaperkalian, munculangkapembagian, munculangkadecimal, type;
                int soaldari, soalsampai, panjangdigit, jumlahmuncul, jmlbarispermuncul, maxpanjangdigit, maxjmldigitpersoal;
                int digitperkalian, digitpembagian, digitdecimal, fontsize, totalrow;
                decimal kecepatan;
                string idperlombaanprev = "";
                int soalsampaiprev = 0;
                string strtglkompetisi = DateTime.Now.ToString("yyyy-MM-dd");
                DataTable dtparm = db.GetParameterKompetisi(peserta.ID_PESERTA, strtglkompetisi);

                if (dtparm.Rows.Count > 0)
                {
                    for (int i = 0; i < dtparm.Rows.Count; i++)
                    {
                        idperlombaan = dtparm.Rows[i]["row_id_kompetisi"].ToString();
                        parameterid = dtparm.Rows[i]["parameter_id"].ToString();
                        munculangkaminus = dtparm.Rows[i]["muncul_angka_minus"].ToString();
                        munculangkaperkalian = dtparm.Rows[i]["muncul_angka_perkalian"].ToString();
                        munculangkapembagian = dtparm.Rows[i]["muncul_angka_pembagian"].ToString();
                        munculangkadecimal = dtparm.Rows[i]["muncul_angka_decimal"].ToString();
                        type = dtparm.Rows[i]["tipe"].ToString();

                        soaldari = dtparm.Rows[i]["soal_dari"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["soal_dari"].ToString());
                        soalsampai = dtparm.Rows[i]["soal_sampai"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["soal_sampai"].ToString());
                        panjangdigit = dtparm.Rows[i]["panjang_digit"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["panjang_digit"].ToString());
                        jumlahmuncul = dtparm.Rows[i]["jumlah_muncul"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["jumlah_muncul"].ToString());
                        jmlbarispermuncul = dtparm.Rows[i]["jml_baris_per_muncul"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["jml_baris_per_muncul"].ToString());
                        maxpanjangdigit = dtparm.Rows[i]["max_panjang_digit"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["max_panjang_digit"].ToString());
                        maxjmldigitpersoal = dtparm.Rows[i]["max_jml_digit_per_soal"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["max_jml_digit_per_soal"].ToString());

                        digitperkalian = dtparm.Rows[i]["digit_perkalian"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["digit_perkalian"].ToString());
                        digitpembagian = dtparm.Rows[i]["digit_pembagian"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["digit_pembagian"].ToString());
                        digitdecimal = dtparm.Rows[i]["digit_decimal"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["digit_decimal"].ToString());
                        fontsize = dtparm.Rows[i]["font_size"].ToString() == "" ? 0 : Convert.ToInt32(dtparm.Rows[i]["font_size"].ToString());

                        kecepatan = dtparm.Rows[i]["kecepatan"].ToString() == "" ? 0 : Convert.ToDecimal(dtparm.Rows[i]["kecepatan"].ToString(), culture);

                        if (kecepatan <= 0)
                        {
                            kecepatan = 1;
                        }

                        totalrow = jmlbarispermuncul * jumlahmuncul;

                        //delete soal
                        db.Query("DELETE FROM tb_soal_kompetisi where row_id_kompetisi = '" + idperlombaan + "' AND no_soal between " + soaldari.ToString() + " and " + soalsampai.ToString());

                        if (i == dtparm.Rows.Count - 1)
                        {
                            db.Query("DELETE FROM tb_soal_kompetisi where row_id_kompetisi = '" + idperlombaanprev + "' AND no_soal = " + (soalsampai + 1).ToString());
                        }
                        else
                        {
                            if (idperlombaanprev != idperlombaan && i > 0)
                            {
                                db.Query("DELETE FROM tb_soal_kompetisi where row_id_kompetisi = '" + idperlombaanprev + "' AND no_soal = " + (soalsampaiprev + 1).ToString());
                            }
                        }

                        if (type == "F")
                        {
                            RandomFlash(soaldari, soalsampai, panjangdigit, totalrow, idperlombaan, jmlbarispermuncul, jumlahmuncul, munculangkaminus, kecepatan);
                        }
                        else if (type == "V")
                        {
                            if (munculangkaperkalian == "Y")
                            {
                                if (digitperkalian == 0)
                                {
                                    digitperkalian = 1;
                                }
                            }
                            if (munculangkapembagian == "Y")
                            {
                                if (digitpembagian == 0)
                                {
                                    digitpembagian = 1;
                                }
                            }
                            if (munculangkadecimal == "Y")
                            {
                                if (digitdecimal == 0)
                                {
                                    digitdecimal = 1;
                                }
                            }
                            RandomVisual(soaldari, soalsampai, maxpanjangdigit, totalrow, idperlombaan, jmlbarispermuncul, jumlahmuncul, munculangkaminus, munculangkaperkalian, digitperkalian, munculangkapembagian, digitpembagian, munculangkadecimal, digitdecimal, kecepatan, maxjmldigitpersoal);
                        }
                        else if (type == "L")
                        {
                            RandomListening(soaldari, soalsampai, panjangdigit, totalrow, idperlombaan, jmlbarispermuncul, jumlahmuncul, munculangkaminus, kecepatan);
                        }

                        //Thanks
                        if (i == dtparm.Rows.Count - 1)
                        {
                            Thanks(idperlombaanprev, soalsampai + 1);
                        }
                        else
                        {
                            if (idperlombaanprev != idperlombaan && i > 0)
                            {
                                Thanks(idperlombaanprev, soalsampaiprev + 1);
                            }
                        }

                        idperlombaanprev = idperlombaan;
                        soalsampaiprev = soalsampai;
                    }
                }

                //ZigZag
                ZigZag();

                //Save Data dtSoal
                string[] lstrPrmHdrUpdateCol, lstrPrmHdrKeyCol;
                lstrPrmHdrUpdateCol = new string[235]{
                               "row_id_kompetisi", "no_soal", "jumlah_muncul", "jml_baris_per_muncul", "angka1", "angka2", "angka3", "angka4", "angka5", "angka6", "angka7",
                                "angka8", "angka9", "angka10", "angka11", "angka12", "angka13", "angka14", "angka15", "angka16", "angka17", "angka18", "angka19", "angka20",
                                "angka21", "angka22", "angka23", "angka24", "angka25", "angka26", "angka27", "angka28", "angka29", "angka30", "angka31", "angka32", "angka33",
                                "angka34", "angka35", "angka36", "angka37", "angka38", "angka39", "angka40", "angka41", "angka42", "angka43", "angka44", "angka45", "angka46",
                                "angka47", "angka48", "angka49", "angka50", "angka51", "angka52", "angka53", "angka54", "angka55", "angka56", "angka57", "angka58", "angka59",
                                "angka60", "angka61", "angka62", "angka63", "angka64", "angka65", "angka66", "angka67", "angka68", "angka69", "angka70", "angka71", "angka72",
                                "angka73", "angka74", "angka75", "angka76", "angka77", "angka78", "angka79", "angka80", "angka81", "angka82", "angka83", "angka84", "angka85",
                                "angka86", "angka87", "angka88", "angka89", "angka90", "angka91", "angka92", "angka93", "angka94", "angka95", "angka96", "angka97", "angka98",
                                "angka99", "angka100", "angka101", "angka102", "angka103", "angka104", "angka105", "angka106", "angka107", "angka108", "angka109", "angka110",
                                "angka111", "angka112", "angka113", "angka114", "angka115", "angka116", "angka117", "angka118", "angka119", "angka120", "angka121", "angka122",
                                "angka123", "angka124", "angka125", "angka126", "angka127", "angka128", "angka129", "angka130", "angka131", "angka132", "angka133", "angka134",
                                "angka135", "angka136", "angka137", "angka138", "angka139", "angka140", "angka141", "angka142", "angka143", "angka144", "angka145", "angka146",
                                "angka147", "angka148", "angka149", "angka150", "perkalian1", "perkalian2", "perkalian3", "perkalian4", "perkalian5", "perkalian6", "perkalian7",
                                "perkalian8", "perkalian9", "perkalian10", "pembagian1", "pembagian2", "pembagian3", "pembagian4", "pembagian5", "pembagian6", "pembagian7",
                                "pembagian8", "pembagian9", "pembagian10", "kunci_jawaban", "angkamuncul1", "angkamuncul2", "angkamuncul3", "angkamuncul4", "angkamuncul5",
                                "angkamuncul6", "angkamuncul7", "angkamuncul8", "angkamuncul9", "angkamuncul10", "angkamuncul11", "angkamuncul12", "angkamuncul13", "angkamuncul14",
                                "angkamuncul15", "angkamuncul16", "angkamuncul17", "angkamuncul18", "angkamuncul19", "angkamuncul20", "angkamuncul21", "angkamuncul22", "angkamuncul23",
                                "angkamuncul24", "angkamuncul25", "angkamuncul26", "angkamuncul27", "angkamuncul28", "angkamuncul29", "angkamuncul30", "angkamuncul31", "angkamuncul32",
                                "angkamuncul33", "angkamuncul34", "angkamuncul35", "angkamuncul36", "angkamuncul37", "angkamuncul38", "angkamuncul39", "angkamuncul40", "angkamuncul41",
                                "angkamuncul42", "angkamuncul43", "angkamuncul44", "angkamuncul45", "angkamuncul46", "angkamuncul47", "angkamuncul48", "angkamuncul49", "angkamuncul50",
                                "kecepatan", "angkalistening1", "angkalistening2", "angkalistening3", "angkalistening4", "angkalistening5", "max_jml_digit_per_soal", "total_digit_per_soal",
                                "MUNCUL_ANGKA_DECIMAL", "DIGIT_DECIMAL" };
                lstrPrmHdrKeyCol = new string[2] { "row_id_kompetisi", "no_soal" };
                if (db.UpdateDataTable(dtSoal, "tb_soal_kompetisi", lstrPrmHdrUpdateCol, lstrPrmHdrKeyCol) != "OK")
                {

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }           

        }
    }
}
