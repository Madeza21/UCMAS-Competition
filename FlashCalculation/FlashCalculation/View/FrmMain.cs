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

            //Load dummy soal kompetisi
            dtSoal = db.GetSoalKompetisi("");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private string RandomAngka(int pdigit)
        {
            string ret = "";
            
            Random rnd = new Random();

            ret = rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString();

            return ret.Substring(0, pdigit);
        }

        private string RandomAngkaDec(int pdigit)
        {
            string ret = "";
            Random rnd = new Random();

            ret = rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() +
                rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + 
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString();

            return ret.Substring(0, pdigit);
        }

        private string RandomFlash(int soaldari, int soalsampai, int pjgdigit, int jmlrow, string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, decimal kecepatan)
        {
            string ret = "";
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
                    if ((strrandomprev == strrandom) || (strrandomprev.Substring(0,1) == strrandom.Substring(0, 1))  || (strrandomprev.Substring(strrandomprev.Length-1) == strrandom.Substring(strrandom.Length - 1)))
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

                    if(munculminus == "Y")
                    {
                        if (Convert.ToDecimal(strrandomprev) > Convert.ToDecimal(strrandom))
                        {
                            strrandom = '-' + strrandom;
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                        }
                        else
                        {
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                        }
                    }
                    else
                    {
                        dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                    }

                    decangkarandom = Convert.ToDecimal(strrandom);
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

            return ret;
        }

        private string RandomListening(int soaldari, int soalsampai, int pjgdigit, int jmlrow, string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, decimal kecepatan)
        {
            string ret = "";
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
                        if (Convert.ToDecimal(strrandomprev) > Convert.ToDecimal(strrandom))
                        {
                            strangka = strangka + "minus " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                            strangkalisten = strangkalisten + "(-" + strrandom + ") " + "minus " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;

                            bminus = true;

                            strrandom = '-' + strrandom;
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                        }
                        else
                        {
                            if (bminus)
                            {
                                strangka = strangka + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(" + strrandom + ") " + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                            }
                            else
                            {
                                strangka = strangka + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(" + strrandom + ") " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                            }

                            bminus = false;
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                        }
                    }
                    else
                    {
                        if (bminus)
                        {
                            strangka = strangka + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                            strangkalisten = strangkalisten + "(" + strrandom + ") " + "plus " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                        }
                        else
                        {
                            strangka = strangka + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                            strangkalisten = strangkalisten + "(" + strrandom + ") " + NumberInWordEnglish(Convert.ToDecimal(strrandom)) + Environment.NewLine;
                        }

                        bminus = false;
                        dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                    }

                    decangkarandom = Convert.ToDecimal(strrandom);
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

            return ret;
        }

        private string RandomVisual(int soaldari, int soalsampai, int pjgdigit, int jmlrow, string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, 
            string munculperkalian, int digitperkalian, string munculpembagian, int digitpembagian, string munculdec, int digitdec, decimal kecepatan, int maxdigitsoal)
        {
            string ret = "";
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

                    if(munculpembagian == "Y" || munculperkalian == "Y")
                    {
                        //Pembagian
                        if (munculpembagian == "Y")
                        {
                            //random 						
                            strrandompembagian = RandomAngkaDec(digitpembagian);
                            if(strrandompembagian.Trim() == "1")
                            {
                                strrandompembagian = RandomAngkaDec(digitpembagian);
                                if (strrandompembagian.Trim() == "1")
                                {
                                    strrandompembagian = RandomAngkaDec(digitpembagian);
                                }
                            }

                            dtest = Convert.ToDecimal(strrandom) % Convert.ToDecimal(strrandompembagian);
                            if(dtest == 0)
                            {

                            }
                            else
                            {
                                dmod = (Convert.ToDecimal(strrandom) - dtest);
                                dtest = dmod % Convert.ToDecimal(strrandompembagian);
                                strrandom = dmod.ToString();
                            }
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                            dr["pembagian" + row.ToString()] = Convert.ToDecimal(strrandompembagian);

                            //kunci jawaban
                            decangkarandom = Convert.ToDecimal(strrandom);
                            kunci = decangkarandom / Convert.ToDecimal(strrandompembagian);
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
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                            strrandomperkalian = RandomAngkaDec(digitperkalian);
                            if (strrandomperkalian.Trim() == "1")
                            {
                                strrandomperkalian = RandomAngkaDec(digitperkalian);
                                if (strrandomperkalian.Trim() == "1")
                                {
                                    strrandomperkalian = RandomAngkaDec(digitperkalian);
                                }
                            }
                            dr["perkalian" + row.ToString()] = Convert.ToDecimal(strrandomperkalian);

                            //kunci jawaban
                            decangkarandom = Convert.ToDecimal(strrandom);
                            kunci = (decangkarandom * Convert.ToDecimal(strrandomperkalian));
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
                            if (Convert.ToDecimal(strrandomprev) > Convert.ToDecimal(strrandom))
                            {
                                strrandom = '-' + strrandom;
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                            }
                            else
                            {
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                            }
                        }
                        else
                        {
                            dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom);
                        }

                        //kunci jawaban
                        decangkarandom = Convert.ToDecimal(strrandom);
                        kunci = kunci + decangkarandom;
                        strrandomprev = strrandom;

                        if (munculdec == "Y")
                        {
                            if (digitdec == 1)
                            {
                                strrandomformat = Convert.ToInt32(strrandom).ToString("###,###,###.#");
                            }else if (digitdec == 2)
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
    }
}
