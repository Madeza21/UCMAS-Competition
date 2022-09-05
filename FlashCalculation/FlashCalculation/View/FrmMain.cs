using ClosedXML.Excel;
using FlashCalculation.Help;
using FlashCalculation.Model;
using FlashCalculation.View;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace FlashCalculation
{
    public partial class FrmMain : Form
    {
        Peserta peserta;

        DataTable dtSoal = new DataTable();
        DataTable dtkompetisi = new DataTable();
        DataTable dtSoalLomba = new DataTable();
        DataTable dtSoalLombaV = new DataTable();
        DataTable dtsp = new DataTable();

        Random rnd = new Random();
        CultureInfo culture = new CultureInfo("en-US");
        SpeechSynthesizer speechSynthesizerObj;

        int errex = 0, datarow, speechRate = 0;
        decimal lamalomba, lamalombaori, speedmuncul, speedjeda, jumlahmuncul, speedbicara, lamajeda;
        string ptype, strvoice = "";
        string rowidtrial = "";
        string flagvisual = "";
        string flaglistening = "";

        DbBase db = new DbBase();
        HttpRequest client = new HttpRequest();

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
            try
            {
                tTanggal.Start();
                Visibled(true);
                //Properties.Settings.Default.siswa_id = "TES UBAH";
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                textBox10.Font = new Font(this.pfc.Families[0], 34, FontStyle.Bold);
                lblSoal.Font = new Font(this.pfc.Families[0], 72, FontStyle.Bold);
                lblNo.Font = new Font(this.pfc.Families[0], 10, FontStyle.Bold);

                label14.Text = peserta.ID_PESERTA;
                label15.Text = peserta.NAMA_PESERTA;
                label16.Text = peserta.JENIS_KELAMIN == "L" ? "Laki-laki" : "Perempuan";
                label17.Text = peserta.TEMPAT_LAHIR;
                label18.Text = peserta.TANGGAL_LAHIR;
                label19.Text = peserta.SEKOLAH_PESERTA;
                label20.Text = peserta.EMAIL_PESERTA;
                label21.Text = peserta.NO_TELP_PESERTA;
                label13.Text = peserta.ALAMAT_PESERTA;

                db.OpenConnection();
                //client.initialize();

                //set tittle
                this.Text = db.GetAppConfig("LBL") + "  (Ver. " + Properties.Settings.Default.version + ") - " + peserta.NAMA_PESERTA;
                label11.Text = db.GetAppConfig("LBK");

                //label23.Visible = false;
                //comboBox2.Visible = false;

                //Load Kompetisi
                DataTable dtkom = Helper.DecryptDataTable(db.GetKompetisi(peserta.ID_PESERTA, DateTime.Now.ToString("yyyy-MM-dd"), Properties.Settings.Default.trial));
                dtkom.DefaultView.Sort = "KOMPETISI_NAME ASC";
                dtkom = dtkom.DefaultView.ToTable();
                //if(dtkom.Rows.Count <= 0)
                //{
                //    MessageBox.Show("Data Kompetisi tidak ada di database !", "Warning!");
                //}
                comboBox1.DataSource = dtkom;
                comboBox1.DisplayMember = "KOMPETISI_NAME";
                comboBox1.ValueMember = "ROW_ID";

                //Load dummy soal kompetisi
                dtSoal = db.GetSoalKompetisi("");
                db.BeginTransaction();
                SetSoalKompetisi("");
                db.commit();
                //if (dtSoal.Rows.Count <= 0)
                //{
                //    MessageBox.Show("Soal Kompetisi tidak ada di database !", "Warning!");
                //}
                //dtJawaban = db.GetJawabanKompetisi("");

                lblNo.Text = "";
                lblDur.Text = "";

                TranslateControl();

                speechSynthesizerObj = new SpeechSynthesizer();
                LoadListSpeech();
                textBox10.Enabled = false;

                SetType(dtkom);

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            catch(Exception ex)
            {
                if (db.IsTransactionStarted())
                {
                    db.rollback();
                }
                MessageBox.Show(ex.Message, "Error !");
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //proses di pindahkan pakai mouse click
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
                    dtSoal.Rows[i]["angkamuncul1"] = angka1.TrimEnd(Environment.NewLine.ToCharArray());
                }
            }
        }

        private void SetSoalKompetisi(string prowidkompetisi)
        {
            try
            {
                string idperlombaan, parameterid, munculangkaminus, munculangkaperkalian, munculangkapembagian, munculangkadecimal, type;
                int soaldari, soalsampai, panjangdigit, jumlahmuncul, jmlbarispermuncul, maxpanjangdigit, maxjmldigitpersoal;
                int digitperkalian, digitpembagian, digitdecimal, fontsize, totalrow;
                decimal kecepatan;
                string idperlombaanprev = "", bahasa = "English";
                int soalsampaiprev = 0;
                string strtglkompetisi = DateTime.Now.ToString("yyyy-MM-dd");
                DataTable dtparm = Helper.DecryptDataTable(db.GetParameterKompetisi(peserta.ID_PESERTA, strtglkompetisi, prowidkompetisi));
                dtparm.AcceptChanges();
                dtparm.Columns.Add("SOAL_DARI_SORT", typeof(int), "SOAL_DARI");

                dtparm.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, SOAL_DARI_SORT ASC";
                dtparm = dtparm.DefaultView.ToTable();

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
                        bahasa = dtparm.Rows[i]["BAHASA"].ToString();

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
                        db.Query("DELETE FROM tb_soal_kompetisi where row_id_kompetisi = '" + Encryptor.Encrypt(idperlombaan) + "' AND no_soal between " + soaldari.ToString() + " and " + soalsampai.ToString());

                        if (i == dtparm.Rows.Count - 1)
                        {
                            db.Query("DELETE FROM tb_soal_kompetisi where row_id_kompetisi = '" + Encryptor.Encrypt(idperlombaanprev) + "' AND no_soal = " + (soalsampai + 1).ToString());
                        }
                        else
                        {
                            if (idperlombaanprev != idperlombaan && i > 0)
                            {
                                db.Query("DELETE FROM tb_soal_kompetisi where row_id_kompetisi = '" + Encryptor.Encrypt(idperlombaanprev) + "' AND no_soal = " + (soalsampaiprev + 1).ToString());
                            }
                        }

                        if (type == "F")
                        {
                            dtSoal = RandomTrans.RandomFlash(soaldari, soalsampai, panjangdigit, totalrow, idperlombaan, 
                                jmlbarispermuncul, jumlahmuncul, munculangkaminus, kecepatan,
                                dtSoal, culture);
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
                            dtSoal = RandomTrans.RandomVisual(soaldari, soalsampai, maxpanjangdigit, totalrow, 
                                idperlombaan, jmlbarispermuncul, jumlahmuncul, munculangkaminus, munculangkaperkalian, 
                                digitperkalian, munculangkapembagian, digitpembagian, munculangkadecimal, 
                                digitdecimal, kecepatan, maxjmldigitpersoal, dtSoal, culture);
                        }
                        else if (type == "L")
                        {
                            if(bahasa == "English")
                            {
                                dtSoal = RandomTrans.RandomListening(soaldari, soalsampai, panjangdigit, totalrow, 
                                    idperlombaan, jmlbarispermuncul, jumlahmuncul, munculangkaminus, kecepatan,
                                    dtSoal, culture);
                            }
                            else
                            {
                                dtSoal = RandomTrans.RandomListeningIndonesia(soaldari, soalsampai, panjangdigit, 
                                    totalrow, idperlombaan, jmlbarispermuncul, jumlahmuncul, munculangkaminus, kecepatan,
                                    dtSoal, culture);
                            }
                            
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
                #region Save Soal
                string[] lstrPrmHdrUpdateCol, lstrPrmHdrKeyCol;
                lstrPrmHdrUpdateCol = new string[236]{
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
                                "MUNCUL_ANGKA_DECIMAL", "DIGIT_DECIMAL", "filter" };
                lstrPrmHdrKeyCol = new string[2] { "row_id_kompetisi", "no_soal" };
                Helper.EncryptDataTableSoal(dtSoal).AcceptChanges();
                dtSoal.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, NO_SOAL ASC";
                dtSoal = dtSoal.DefaultView.ToTable();

                if (db.UpdateDataTable(dtSoal, "tb_soal_kompetisi", lstrPrmHdrUpdateCol, lstrPrmHdrKeyCol) != "OK")
                {
                    throw new Exception("Error save to tb_soal_kompetisi");
                }
                #endregion Save Soal
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error DB!");
            }   
        }

        private void textBox10_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    int idwsoal = 0;
                    decimal idetikberapa = 0, ikuncijawaban = 0, ijawaban = 0, isoalno = 0;
                    string strrowidkomp = "", strpertanyaanloop = "", strangkamuncul = "";
                    string skip = "";

                    if (lamalomba <= 0)
                    {
                        lblSoal.Text = "";
                        textBox10.Text = "";
                        textBox10.Enabled = false;
                        if (Properties.Settings.Default.bahasa == "indonesia")
                        {
                            MessageBox.Show("Waktu sudah berakhir.");
                        }
                        else
                        {
                            MessageBox.Show("Time is over.");
                        }
                        return;
                    }
                    else
                    {
                        lblSoal.Text = "";
                        if (datarow >= 0)
                        {
                            if (ptype == "V")
                            {
                                idwsoal = datarow;
                            }
                            else
                            {
                                idwsoal = datarow - 1;
                            }

                            if (textBox10.Text != "")
                            {
                                ijawaban = Convert.ToDecimal(textBox10.Text);
                            }
                            else
                            {
                                if (ptype == "V")
                                {
                                    skip = "Y";
                                    goto hell;
                                }
                            }
                            idetikberapa = lamalombaori - lamalomba;
                            strrowidkomp = dtSoalLomba.Rows[idwsoal]["row_id_kompetisi"].ToString();
                            ikuncijawaban = dtSoalLomba.Rows[idwsoal]["kunci_jawaban"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[idwsoal]["kunci_jawaban"].ToString());
                            isoalno = dtSoalLomba.Rows[idwsoal]["no_soal"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[idwsoal]["no_soal"].ToString());

                            strpertanyaanloop = "";
                            if (ptype == "L")
                            {
                                for (int i = 1; i <= 5; i++)
                                {
                                    strangkamuncul = dtSoalLomba.Rows[idwsoal]["angkalistening" + i].ToString();
                                    if (strangkamuncul == "")
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        strpertanyaanloop = strpertanyaanloop + strangkamuncul + Environment.NewLine;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 1; i <= 50; i++)
                                {
                                    strangkamuncul = dtSoalLomba.Rows[idwsoal]["angkamuncul" + i].ToString();
                                    if (strangkamuncul == "")
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        strpertanyaanloop = strpertanyaanloop + strangkamuncul + Environment.NewLine;
                                    }
                                }
                            }


                            #region Trial
                            if (Properties.Settings.Default.trial == "Y")
                            {
                                DataTable dtJawabanTrial = db.GetJawabanKompetisiTrial("", "");
                                DataRow dr3;
                                dr3 = (DataRow)dtJawabanTrial.NewRow();

                                dr3["row_id_hdr"] = rowidtrial;
                                dr3["row_id_kompetisi"] = strrowidkomp;
                                dr3["id_peserta"] = peserta.ID_PESERTA;
                                dr3["soal_no"] = isoalno;
                                dr3["pertanyaan"] = strpertanyaanloop.TrimEnd(Environment.NewLine.ToCharArray());
                                dr3["jawaban_peserta"] = ijawaban;
                                dr3["jawab_detik_berapa"] = idetikberapa;
                                dr3["jawab_date"] = DateTime.Now;
                                dr3["kunci_jawaban"] = ikuncijawaban;
                                if (ijawaban == ikuncijawaban)
                                {
                                    if ((strpertanyaanloop.Contains("÷") || strpertanyaanloop.Contains("x")) && ptype == "V")
                                    {
                                        dr3["score_peserta"] = 50;
                                    }
                                    else
                                    {
                                        dr3["score_peserta"] = 100;
                                    }
                                }
                                else
                                {
                                    dr3["score_peserta"] = 0;
                                }

                                dr3["is_kirim"] = "N";

                                dtJawabanTrial.Rows.Add(dr3);

                                string[] lstrPrmHdrUpdateCol2, lstrPrmHdrKeyCol2;
                                lstrPrmHdrUpdateCol2 = new string[11]{
                               "ROW_ID_HDR", "ROW_ID_KOMPETISI", "ID_PESERTA", "SOAL_NO", "PERTANYAAN", "JAWABAN_PESERTA", "JAWAB_DETIK_BERAPA",
                                "JAWAB_DATE", "KUNCI_JAWABAN", "SCORE_PESERTA", "IS_KIRIM" };
                                lstrPrmHdrKeyCol2 = new string[4] { "ROW_ID_HDR", "ROW_ID_KOMPETISI", "ID_PESERTA", "SOAL_NO" };

                                Helper.EncryptDataTable(dtJawabanTrial).AcceptChanges();
                                foreach (DataRow row in dtJawabanTrial.Rows)
                                {
                                    row.SetAdded();
                                }
                                db.BeginTransaction();
                                if (db.UpdateDataTable(dtJawabanTrial, "tb_jawaban_kompetisi_trial", lstrPrmHdrUpdateCol2, lstrPrmHdrKeyCol2) != "OK")
                                {
                                    db.rollback();
                                }
                                db.commit();
                            }
                            else
                            {
                                DataTable dtJawaban = db.GetJawabanKompetisi("");
                                DataRow dr;

                                dr = (DataRow)dtJawaban.NewRow();


                                dr["row_id_kompetisi"] = strrowidkomp;
                                dr["id_peserta"] = peserta.ID_PESERTA;
                                dr["soal_no"] = isoalno;
                                dr["pertanyaan"] = strpertanyaanloop.TrimEnd(Environment.NewLine.ToCharArray());
                                dr["jawaban_peserta"] = ijawaban;
                                dr["jawab_detik_berapa"] = idetikberapa;
                                dr["jawab_date"] = DateTime.Now;
                                dr["kunci_jawaban"] = ikuncijawaban;
                                if (ijawaban == ikuncijawaban)
                                {
                                    if ((strpertanyaanloop.Contains("÷") || strpertanyaanloop.Contains("x")) && ptype == "V")
                                    {
                                        dr["score_peserta"] = 50;
                                    }
                                    else
                                    {
                                        dr["score_peserta"] = 100;
                                    }
                                }
                                else
                                {
                                    dr["score_peserta"] = 0;
                                }

                                dr["is_kirim"] = "N";

                                dtJawaban.Rows.Add(dr);

                                string[] lstrPrmHdrUpdateCol, lstrPrmHdrKeyCol;
                                lstrPrmHdrUpdateCol = new string[10]{
                               "ROW_ID_KOMPETISI", "ID_PESERTA", "SOAL_NO", "PERTANYAAN", "JAWABAN_PESERTA", "JAWAB_DETIK_BERAPA",
                                "JAWAB_DATE", "KUNCI_JAWABAN", "SCORE_PESERTA", "IS_KIRIM" };
                                lstrPrmHdrKeyCol = new string[3] { "ROW_ID_KOMPETISI", "ID_PESERTA", "SOAL_NO" };

                                Helper.EncryptDataTable(dtJawaban).AcceptChanges();
                                foreach (DataRow row in dtJawaban.Rows)
                                {
                                    row.SetAdded();
                                }
                                db.BeginTransaction();
                                if (db.UpdateDataTable(dtJawaban, "tb_jawaban_kompetisi", lstrPrmHdrUpdateCol, lstrPrmHdrKeyCol) != "OK")
                                {
                                    db.rollback();
                                }
                                db.commit();
                            }
                            #endregion Trial
                        }

                        hell:
                        if (ptype == "V")
                        {
                            if (lblSoal.Text == "Next...")
                            {
                                textBox10.Enabled = false;
                            }
                            else
                            {
                                textBox10.Enabled = true;
                            }

                            textBox10.Text = "";
                            textBox10.Focus();
                            if (skip != "Y")
                            {
                                dtSoalLombaV.Rows[Convert.ToInt32(isoalno) - 1]["flag"] = "Y";
                            }

                            datarow = datarow + 1;
                            textBox10.Enabled = false;
                        }
                        else
                        {
                            textBox10.Enabled = false;
                        }

                        if (ptype == "V" || ptype == "L" || ptype == "F")
                        {
                            lamalomba = lamalomba + 1;
                        }

                        StartLomba();
                    }
                }
            }
            catch(Exception ex)
            {
                if (db.IsTransactionStarted())
                {
                    db.rollback();
                }
                MessageBox.Show(ex.Message, "Warning!");
            }
            
        }

        private void tdurlomba_Tick(object sender, EventArgs e)
        {
            FuncTimer();
        }

        private void tlomba_Tick(object sender, EventArgs e)
        {
            TimerLomba();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmParameter frm = new FrmParameter(comboBox1.SelectedValue.ToString());
            if (frm.ShowDialog() == DialogResult.OK)
            {
                //Jika Berhasil
            }
            else
            {
                //Jika Batal
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmResult frm = new FrmResult(comboBox1.SelectedValue.ToString());
            if (frm.ShowDialog() == DialogResult.OK)
            {
                //Jika Berhasil
            }
            else
            {
                //Jika Batal
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pilihperlombaan = comboBox1.SelectedValue.ToString();
            string tglkompetisi = DateTime.Now.ToString("yyyy-MM-dd");
            string pflag = db.GetFlagKompetisi(pilihperlombaan, tglkompetisi);
            if (pflag == "Y")
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }

            DataTable dt = Helper.DecryptDataTable(db.GetKompetisiID(peserta.ID_PESERTA, comboBox1.SelectedValue.ToString()));
            dt.AcceptChanges();
            SetType(dt);
        }

        private void SetType(DataTable dt)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["TIPE"].ToString() == "L")
                    {
                        label23.Visible = true;
                        comboBox2.Visible = true;

                        button4.Visible = false;
                        button5.Visible = false;
                        button6.Visible = false;

                        flaglistening = "";

                        if (dt.Rows[0]["BAHASA"].ToString() == "English")
                        {
                            DataTable dtt = null;
                            var dtfind = dtsp.Select("Name like '%English%'");
                            if (dtfind.Any())
                            {
                                dtt = dtfind.CopyToDataTable();
                                comboBox2.DataSource = dtt;

                            }
                            else
                            {
                                MessageBox.Show("Please install Speak English!", "Warning!");
                                return;
                            }

                            //DataTable dtt = dtsp.AsEnumerable()
                            //                    .Where(row => row.Field<String>("Name").Contains("English"))
                            //                    .CopyToDataTable();
                            //comboBox2.DataSource = dtt;

                            flaglistening = "English";
                            /*for (int i = 0; i < dtsp.Rows.Count; i++)
                            {
                                if (dtsp.Rows[i]["Name"].ToString().Contains("English"))
                                {
                                    //comboBox2.SelectedIndex = comboBox2.FindStringExact(dtsp.Rows[i]["Name"].ToString());
                                    comboBox2.Text = dtsp.Rows[i]["Name"].ToString();
                                    break;
                                }
                            }*/
                        }
                        else
                        {
                            DataTable dtt = null;
                            var dtfind = dtsp.Select("Name like '%Indonesian%'");
                            if (dtfind.Any())
                            {
                                dtt = dtfind.CopyToDataTable();
                                comboBox2.DataSource = dtt;

                            }
                            else
                            {
                                MessageBox.Show("Please install Speak Indonesian!", "Warning!");
                                return;
                            }

                            //DataTable dtt = dtsp.AsEnumerable()
                            //                    .Where(row => row.Field<String>("Name").Contains("Indonesian"))
                            //                    .CopyToDataTable();
                            //comboBox2.DataSource = dtt;

                            flaglistening = "Indonesian";
                            /*for (int i = 0; i < dtsp.Rows.Count; i++)
                            {
                                if (dtsp.Rows[i]["Name"].ToString().Contains("Indonesian"))
                                {
                                    comboBox2.Text = dtsp.Rows[i]["Name"].ToString();
                                    //comboBox2.SelectedIndex = comboBox2.FindStringExact(dtsp.Rows[i]["Name"].ToString());
                                    break;
                                }
                            }*/
                        }

                    }
                    else
                    {
                        label23.Visible = false;
                        comboBox2.Visible = false;

                        button4.Visible = false;
                        button5.Visible = false;
                        button6.Visible = false;

                        //Buatkan Proses untuk visual bisa pilih soal yang dikerjakan terlebih dahulu misal perkalian atau pembagian terlebih dahulu
                        if (dt.Rows[0]["TIPE"].ToString() == "V")
                        {
                            string strtglkompetisi = DateTime.Now.ToString("yyyy-MM-dd");
                            DataTable dtparm = Helper.DecryptDataTable(db.GetParameterKompetisi(peserta.ID_PESERTA, strtglkompetisi, dt.Rows[0]["ROW_ID"].ToString()));
                            dtparm.AcceptChanges();
                            dtparm.Columns.Add("SOAL_DARI_SORT", typeof(int), "SOAL_DARI");

                            dtparm.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, SOAL_DARI_SORT ASC";
                            dtparm = dtparm.DefaultView.ToTable();

                            if (dtparm.Rows.Count > 0)
                            {
                                DataRow[] foundRows_bagi, foundRows_kali, foundRows_tambah;
                                foundRows_bagi = dtparm.Select("MUNCUL_ANGKA_PEMBAGIAN='Y'");
                                foundRows_kali = dtparm.Select("MUNCUL_ANGKA_PERKALIAN='Y'");
                                foundRows_tambah = dtparm.Select("MUNCUL_ANGKA_PEMBAGIAN <> 'Y' or MUNCUL_ANGKA_PERKALIAN <> 'Y'");

                                if (foundRows_bagi.Length > 0)
                                {
                                    button6.Visible = true;
                                    button6.Enabled = false;
                                }

                                if (foundRows_kali.Length > 0)
                                {
                                    button5.Visible = true;
                                    button5.Enabled = false;
                                }

                                if (foundRows_tambah.Length > 0 && (foundRows_bagi.Length > 0 || foundRows_kali.Length > 0))
                                {
                                    button4.Visible = true;
                                    button4.Enabled = false;
                                }
                                else
                                {
                                    button4.Visible = false;
                                    button4.Enabled = false;
                                }
                            }

                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning!");
            }
            
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (lamalomba > 0)
            {
                if (Properties.Settings.Default.bahasa == "indonesia")
                {
                    MessageBox.Show("Waktu masih tersedia, aplikasi tidak dapat di tutup");
                }
                else
                {
                    MessageBox.Show("Time is still available, the application cannot be closed");
                }
                e.Cancel = true;
            }
        }
        
        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
                
        private void Start()
        {
            FuncTimer();
            
            tdurlomba.Interval = 1000;
            tdurlomba.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            return;
            /*DataTable dt = Helper.DecryptDataTableSoal(db.GetSoalKompetisiID(peserta.ID_PESERTA, comboBox1.SelectedValue.ToString()));
            dt.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, NO_SOAL ASC";
            dt = dt.DefaultView.ToTable();
            dt.AcceptChanges();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            sfd.FileName = comboBox1.Text + "_" + DateTime.Now.ToString("ddMMyyyy") +".xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (!(dt.Rows.Count == 0))
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Sheet1");
                        wb.SaveAs(sfd.FileName);
                    }
                }
            }   */      
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                string pilihperlombaan;
                string tglkompetisi;
                decimal dlamalomba;

                pilihperlombaan = comboBox1.SelectedValue.ToString();
                tglkompetisi = DateTime.Now.ToString("yyyy-MM-dd");

                if (pilihperlombaan == "")
                {
                    if (Properties.Settings.Default.bahasa == "indonesia")
                    {
                        MessageBox.Show("Mohon Kompetisi dipilih terlebih dahulu.");
                    }
                    else
                    {
                        MessageBox.Show("Please choose the competition first.");
                    }
                    return;
                }
                else
                {
                    db.BeginTransaction();
                    ptype = db.GetTypeKompetisi(pilihperlombaan, tglkompetisi);
                    if (Properties.Settings.Default.trial == "Y")
                    {
                        db.Query("DELETE FROM  tb_jawaban_kompetisi WHERE ROW_ID_KOMPETISI = '" + Encryptor.Encrypt(pilihperlombaan) + "' AND ID_PESERTA = '" + Encryptor.Encrypt(peserta.ID_PESERTA) + "'");
                        //Generate ulang soal
                        if (checkBox1.Checked)
                        {
                            //Load dummy soal kompetisi
                            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                            dtSoal = db.GetSoalKompetisi("");
                            SetSoalKompetisi(pilihperlombaan);
                            this.Cursor = System.Windows.Forms.Cursors.Default;
                        }
                        //selalu insert data trial
                        int linenum = db.CountKompetisiTrial(pilihperlombaan);

                        var gid = Guid.NewGuid();
                        rowidtrial = gid.ToString();
                        db.InsertKompetisiTrial(rowidtrial, linenum.ToString(), pilihperlombaan, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    //Sudah ikut kompetisi
                    if (db.GetJawabanKompetisi(pilihperlombaan, peserta.ID_PESERTA) > 0)
                    {
                        if (Properties.Settings.Default.bahasa == "indonesia")
                        {
                            MessageBox.Show("Anda sudah pernah berpartisipasi dalam kompetisi ini sebelumnya.");
                        }
                        else
                        {
                            MessageBox.Show("You have participated this competition before.");
                        }
                        db.rollback();
                        return;
                    }

                    if (Properties.Settings.Default.trial != "Y")
                    {
                        //Validasi flag
                        string pflag = db.GetFlagKompetisi(pilihperlombaan, tglkompetisi);
                        if (pflag == "Y")
                        {
                            if (Properties.Settings.Default.bahasa == "indonesia")
                            {
                                MessageBox.Show("Anda sudah pernah berpartisipasi dalam kompetisi ini sebelumnya.");
                            }
                            else
                            {
                                MessageBox.Show("You have participated this competition before.");
                            }
                            db.rollback();
                            return;
                        }
                        else
                        {
                            pflag = client.PostRequestUpdateFlag("api/pesertakompetisi/getflag", pilihperlombaan);
                            if (pflag == "Y")
                            {
                                if (Properties.Settings.Default.bahasa == "indonesia")
                                {
                                    MessageBox.Show("Anda sudah pernah berpartisipasi dalam kompetisi ini sebelumnya.");
                                }
                                else
                                {
                                    MessageBox.Show("You have participated this competition before.");
                                }
                                db.rollback();
                                return;
                            }
                        }

                        //Update Flag
                        string msg = client.PostRequestUpdateFlag("api/pesertakompetisi/updateflag", pilihperlombaan);
                        if (msg == "Flag update success")
                        {
                            //Update flag db local                    
                            string flag = Encryptor.Encrypt("Y");
                            db.Query("Update tb_kompetisi set START_FLAG = '" + flag + "' where ROW_ID = '" +
                                            Encryptor.Encrypt(pilihperlombaan) + "' AND TANGGAL_KOMPETISI = '" +
                                            Encryptor.Encrypt(tglkompetisi) + "'");
                        }
                    }

                    textBox10.Enabled = false;
                    if (ptype == "V")
                    {
                        textBox10.Enabled = true;
                    }
                    else
                    {
                        textBox10.Enabled = false;
                    }
                    lblSoal.Text = "READY ??";
                    comboBox1.Enabled = false;

                    if (ptype == "L")
                    {
                        comboBox2.Enabled = false;
                    }

                    dtkompetisi = Helper.DecryptDataTable(db.GetKompetisiID(peserta.ID_PESERTA, pilihperlombaan));
                    dtSoalLomba = Helper.DecryptDataTableSoal(db.GetSoalKompetisiID(peserta.ID_PESERTA, pilihperlombaan));
                    dtSoalLomba.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, NO_SOAL ASC";
                    dtSoalLomba = dtSoalLomba.DefaultView.ToTable();

                    dtkompetisi.AcceptChanges();
                    dtSoalLomba.AcceptChanges();

                    if (dtkompetisi.Rows.Count > 0)
                    {
                        dlamalomba = dtkompetisi.Rows[0]["lama_perlombaan"].ToString() == "" ? 0 : Convert.ToDecimal(dtkompetisi.Rows[0]["lama_perlombaan"].ToString());
                        lamalombaori = dlamalomba + 1;

                        if (ptype == "L")
                        {
                            dlamalomba = dlamalomba + 7;
                        }
                        else
                        {
                            dlamalomba = dlamalomba + 4;
                        }

                        lamalomba = dlamalomba;
                        if (dtSoalLomba.Rows.Count > 0)
                        {
                            //set kecepatan per soal
                            speedmuncul = dtSoalLomba.Rows[0]["kecepatan"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[0]["kecepatan"].ToString());
                            speedjeda = dtSoalLomba.Rows[0]["kecepatan"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[0]["kecepatan"].ToString());

                            if (ptype == "V")
                            {
                                dtSoalLombaV = Helper.DecryptDataTableSoal(db.GetSoalKompetisiID(peserta.ID_PESERTA, pilihperlombaan));
                                dtSoalLombaV.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, NO_SOAL ASC";
                                dtSoalLombaV = dtSoalLombaV.DefaultView.ToTable();
                                dtSoalLombaV.AcceptChanges();

                                flagvisual = "";
                            }

                            datarow = 0;
                            jumlahmuncul = 0;

                        }
                        //Start Waktu Perlombaan
                        Start();
                    }

                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;

                    db.commit();
                }
            }
            catch(Exception ex)
            {
                if (db.IsTransactionStarted())
                {
                    db.rollback();
                }

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

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_MouseClick(object sender, MouseEventArgs e)
        {
            flagvisual = "TambahKurang";
            SetButtonEnabledVisual(flagvisual);
            dtSoalLomba.Clear();
            var dtfind = dtSoalLombaV.Select("filter = 'TambahKurang' and flag = 'N'");
            if (dtfind.Any())
            {
                dtSoalLomba = dtfind.CopyToDataTable();
            }

            if (dtSoalLomba.Rows.Count > 0)
            {
                if (!textBox10.Enabled)
                {
                    textBox10.Enabled = true;
                }

                dtSoalLomba.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, NO_SOAL ASC";
                dtSoalLomba = dtSoalLomba.DefaultView.ToTable();

                datarow = 0;
                jumlahmuncul = 0;
                if (!tdurlomba.Enabled)
                {
                    tdurlomba.Start();
                }

                StartLomba();
            }
        }

        private void button5_MouseClick(object sender, MouseEventArgs e)
        {
            flagvisual = "Kali";
            SetButtonEnabledVisual(flagvisual);
            dtSoalLomba.Clear();
            var dtfind = dtSoalLombaV.Select("filter = 'Kali' and flag = 'N'");
            if (dtfind.Any())
            {
                dtSoalLomba = dtfind.CopyToDataTable();
            }

            if (dtSoalLomba.Rows.Count > 0)
            {
                if (!textBox10.Enabled)
                {
                    textBox10.Enabled = true;
                }

                dtSoalLomba.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, NO_SOAL ASC";
                dtSoalLomba = dtSoalLomba.DefaultView.ToTable();

                datarow = 0;
                jumlahmuncul = 0;
                if (!tdurlomba.Enabled)
                {
                    tdurlomba.Start();
                }
                StartLomba();
            }
        }

        private void button6_MouseClick(object sender, MouseEventArgs e)
        {
            flagvisual = "Bagi";
            SetButtonEnabledVisual(flagvisual);
            dtSoalLomba.Clear();
            var dtfind = dtSoalLombaV.Select("filter = 'Bagi' and flag = 'N'");
            if (dtfind.Any())
            {
                dtSoalLomba = dtfind.CopyToDataTable();
            }

            if (dtSoalLomba.Rows.Count > 0)
            {
                if (!textBox10.Enabled)
                {
                    textBox10.Enabled = true;
                }

                dtSoalLomba.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, NO_SOAL ASC";
                dtSoalLomba = dtSoalLomba.DefaultView.ToTable();

                datarow = 0;
                jumlahmuncul = 0;
                if (!tdurlomba.Enabled)
                {
                    tdurlomba.Start();
                }
                StartLomba();
            }
        }

        private void stop()
        {
            tdurlomba.Stop();
        }

        private void tTanggal_Tick(object sender, EventArgs e)
        {
            label22.Text = DateTime.Now.ToString("dddd, dd-MMM-yyyy hh:mm:ss");
        }

        private void StartLomba()
        {
            FuncTimer();

            if(ptype == "V")
            {
                tlomba.Interval = 1;
            }
            else
            {
                //1sec = 1000
                tlomba.Interval = Convert.ToInt32(speedmuncul * 850);
            }
            tlomba.Start();
        }

        private void StopLomba()
        {
            tlomba.Stop();
            if(ptype == "L")
            {
                if(strvoice == "")
                {
                    //START SISA WAKTU LOMBA
                    Start();
                }
                else
                {
                    //STOP SISA WAKTU LOMBA
                    stop();

                    textBox10.Enabled = false;

                    speechSynthesizerObj.Dispose();
                    speechSynthesizerObj = new SpeechSynthesizer();
                    speechSynthesizerObj.SetOutputToDefaultAudioDevice();
                    speechSynthesizerObj.Volume = 100; // от 0 до 100
                    speechSynthesizerObj.Rate = speechRate - 2; //от -10 до 10
                    speechSynthesizerObj.SelectVoice(comboBox2.SelectedValue.ToString());
                    speechSynthesizerObj.SpeakAsync(strvoice);
                    speechSynthesizerObj.SpeakCompleted += SpeakComplete;
                    lblSoal.Text = "";
                }
            }
        }

        private void FuncTimer()
        {
            lamalomba = lamalomba - 1;
            if(lamalomba == lamalombaori)
            {
                lblDur.Text = "";
                lblSoal.Text = "";

                //Start Perlombaan
                if (ptype == "L" || ptype == "F")
                {
                    stop();
                    StartLomba();
                }
                else
                {
                    if(!button4.Visible && !button5.Visible && !button6.Visible)
                    {
                        StartLomba();
                    }
                    else
                    {
                        stop();
                        StopLomba();

                        button4.Enabled = true;
                        button5.Enabled = true;
                        button6.Enabled = true;

                        button4.Focus();

                        lblDur.Text = "";//lamalomba.ToString()
                        textBox10.Enabled = false;

                        return;
                    }                    
                }
            }

            if(lamalomba <= 0)
            {
                lblSoal.Font = new Font(this.pfc.Families[0], 72, FontStyle.Bold);
                lblSoal.Text = "Thanks ^_^";
                lblNo.Text = "";
                speechRate = 0;
                if (Properties.Settings.Default.trial == "Y")
                {
                    button1.Enabled = true;
                    checkBox1.Visible = false;
                    checkBox1.Checked = true;
                }
                else
                {
                    button1.Enabled = false;
                    checkBox1.Visible= false;
                    checkBox1.Checked = false;
                }
                    
                button2.Enabled = true;
                button3.Enabled = true;

                lblDur.Text = "";
                textBox10.Text = "";
                textBox10.Enabled = false;
                comboBox1.Enabled = true;

                stop();
                StopLomba();
                if (ptype == "L")
                {
                    comboBox2.Enabled = true;
                    //Gets the current speaking state of the SpeechSynthesizer object.   
                    if (speechSynthesizerObj.State == SynthesizerState.Speaking)
                    {
                        //close the SpeechSynthesizer object.   
                        speechSynthesizerObj.SpeakAsyncCancelAll();
                    }
                    speechSynthesizerObj.Dispose();
                }else if (ptype == "V")
                {
                    button4.Enabled = false;
                    button5.Enabled = false;
                    button6.Enabled = false;
                }
                button3.Focus();
            }
            else
            {
                if(lamalomba <= lamalombaori)
                {
                    lblDur.Text = lamalomba.ToString();
                }                
            }
        }

        private void TimerLomba()
        {
            int idatarow = dtSoalLomba.Rows.Count - 1;
            decimal djumlahmuncul, djmlbarispermuncul = 0;
            string strAngka = "";

            if (idatarow >= 0 && datarow <= idatarow)
            {
                djumlahmuncul = dtSoalLomba.Rows[datarow]["jumlah_muncul"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[datarow]["jumlah_muncul"].ToString());
                if (jumlahmuncul == djumlahmuncul)
                {
                    if (ptype == "V")
                    {
                        //
                    }
                    else
                    {                        
                        datarow = datarow + 1;
                    }

                    textBox10.Text = "";
                    if (lblSoal.Text == "Next...")
                    {
                        textBox10.Enabled = false;
                    }
                    else
                    {
                        textBox10.Enabled = true;
                        textBox10.Focus();
                    }
                        
                    jumlahmuncul = 0;
                    if (ptype == "F")
                    {
                        Start();
                    }

                    StopLomba();
                    
                    if (ptype == "F")
                    {
                        lblNo.Text = "";
                        lblSoal.Text = " = ";
                        lblSoal.Font = new Font(this.pfc.Families[0], 400, FontStyle.Bold);
                    }
                    
                    return;
                }
                else
                {
                    lblSoal.Text = "";
                    if (ptype == "F")
                    {
                        stop();
                    }
                }

                jumlahmuncul = jumlahmuncul + 1;
                if (datarow > idatarow)
                {
                    //
                }
                else
                {
                    //set kecepatan per soal
                    speedmuncul = dtSoalLomba.Rows[datarow]["kecepatan"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[datarow]["kecepatan"].ToString());
                    speedbicara = dtSoalLomba.Rows[datarow]["kecepatan"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[datarow]["kecepatan"].ToString());
                    if (ptype == "V")
                    {
                        lamajeda = speedmuncul;
                    }

                    strAngka = dtSoalLomba.Rows[datarow]["angkamuncul" + jumlahmuncul].ToString();
                    djmlbarispermuncul = dtSoalLomba.Rows[datarow]["jml_baris_per_muncul"].ToString() == "" ? 0 : Convert.ToDecimal(dtSoalLomba.Rows[datarow]["jml_baris_per_muncul"].ToString());

                }

                if (strAngka.Contains("Thanks"))
                {
                    lblNo.Text = "";
                    lblSoal.Text = "";
                    if (ptype == "V")
                    {
                        if(flagvisual == "TambahKurang")
                        {
                            button4.Enabled = false;
                        }
                        else if (flagvisual == "Kali")
                        {
                            button5.Enabled = false;
                        }
                        else if (flagvisual == "Bagi")
                        {
                            button6.Enabled = false;
                        }

                        if (button4.Enabled || button5.Enabled || button6.Enabled)
                        {
                            lblSoal.Text = "Next...";
                            textBox10.Enabled = false;
                        }                        
                    }
                }
                else
                {
                    if (ptype != "L")
                    {
                        //
                        //lblNo.Text = "No. " + (datarow + 1).ToString();
                        lblNo.Text = "No. " + dtSoalLomba.Rows[datarow]["no_soal"].ToString();
                    }
                    
                    //lblSoal.Text = strAngka.;

                    lblSoal.Font = new Font(this.pfc.Families[0], 72, FontStyle.Bold);

                    if (ptype == "V")
                    {
                        if (strAngka.Contains("÷"))
                        {
                            lblSoal.Text = strAngka.TrimEnd(Environment.NewLine.ToCharArray());
                        }
                        else if (strAngka.Contains("x"))
                        {
                            lblSoal.Text = strAngka.TrimEnd(Environment.NewLine.ToCharArray());
                        }
                        else
                        {
                            strAngka = strAngka.TrimEnd(Environment.NewLine.ToCharArray());
                            strAngka = strAngka.Replace(Environment.NewLine, "|");
                            string[] arr = strAngka.Split('|');
                            int maxdigit = 0;
                            if (arr.Length > 0)
                            {
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (maxdigit < arr[i].Length)
                                    {
                                        maxdigit = arr[i].Length;
                                    }
                                }

                                strAngka = "";
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (maxdigit - arr[i].Length == 0)
                                    {
                                        strAngka += arr[i] + Environment.NewLine;
                                    }
                                    else if (maxdigit - arr[i].Length > 0)
                                    {
                                        if (arr[i].Substring(0, 1) == "-")
                                        {
                                            if (maxdigit - arr[i].Length > 0)
                                            {
                                                strAngka += "-" + arr[i].Substring(1).PadLeft(maxdigit - 1, ' ') + Environment.NewLine;
                                            }
                                            else
                                            {
                                                strAngka += arr[i].PadLeft(maxdigit, ' ') + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            strAngka += arr[i].PadLeft(maxdigit, ' ') + Environment.NewLine;
                                        }
                                    }
                                }
                            }

                            lblSoal.Text = strAngka.TrimEnd(Environment.NewLine.ToCharArray());
                        }

                        if (djmlbarispermuncul <= 4)
                        {
                            lblSoal.Font = new Font(this.pfc.Families[0], 72, FontStyle.Bold);
                        }

                        if (djmlbarispermuncul == 5)
                        {
                            lblSoal.Font = new Font(this.pfc.Families[0], 40, FontStyle.Bold);
                        }

                        if (djmlbarispermuncul == 6)
                        {
                            lblSoal.Font = new Font(this.pfc.Families[0], 35, FontStyle.Bold);
                        }

                        if (djmlbarispermuncul == 7)
                        {
                            lblSoal.Font = new Font(this.pfc.Families[0], 30, FontStyle.Bold);
                        }

                        if (djmlbarispermuncul == 8)
                        {
                            lblSoal.Font = new Font(this.pfc.Families[0], 25, FontStyle.Bold);
                        }

                        if (djmlbarispermuncul >= 9)
                        {
                            lblSoal.Font = new Font(this.pfc.Families[0], 20, FontStyle.Bold);
                        }

                        textBox10.Enabled = true;
                    }
                    if (ptype == "F")
                    {
                        lblSoal.Text = strAngka.TrimEnd(Environment.NewLine.ToCharArray());
                    }
                    if (ptype == "L")
                    {
                        if (strAngka.Contains("Thanks"))
                        {
                            speechRate = 0;
                            strvoice = "";
                        }
                        else
                        {
                            if (strAngka == "")
                            {
                                speechRate = 0;
                                strvoice = "";
                            }
                            else
                            {
                                speechRate = Convert.ToInt32(speedbicara);
                                if(flaglistening == "English")
                                {
                                    strvoice = "ready?" + Environment.NewLine + Environment.NewLine + strAngka + Environment.NewLine + Environment.NewLine + " that is";
                                }
                                else
                                {
                                    strvoice = "Siap?" + Environment.NewLine + Environment.NewLine + strAngka + Environment.NewLine + Environment.NewLine + " jadi";
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        speechRate = 0;
                        strvoice = "";
                    }

                    if (idatarow < datarow)
                    {
                        if (ptype == "V")
                        {
                            if (flagvisual == "TambahKurang")
                            {
                                button4.Enabled = false;
                            }
                            else if (flagvisual == "Kali")
                            {
                                button5.Enabled = false;
                            }
                            else if (flagvisual == "Bagi")
                            {
                                button6.Enabled = false;
                            }

                            if (button4.Enabled || button5.Enabled || button6.Enabled)
                            {
                                lblSoal.Text = "Next...";
                                textBox10.Enabled = false;
                            }
                            else
                            {
                                button1.Enabled = false;
                                button2.Enabled = true;
                                button3.Enabled = true;

                                textBox10.Enabled = false;
                                textBox10.Text = "";
                                comboBox1.Enabled = true;

                                lblNo.Text = "";
                                lblSoal.Text = "";

                                StopLomba();
                            }
                        }
                        else
                        {
                            button1.Enabled = false;
                            button2.Enabled = true;
                            button3.Enabled = true;

                            textBox10.Enabled = false;
                            textBox10.Text = "";
                            comboBox1.Enabled = true;

                            lblNo.Text = "";
                            lblSoal.Text = "";

                            if (ptype == "L")
                            {
                                comboBox2.Enabled = false;
                            }

                            StopLomba();
                        }                        
                    }
                }
            }
            else if(datarow > idatarow)
            {
                lblNo.Text = "";
                lblSoal.Text = "";
                if (ptype == "V")
                {
                    if (flagvisual == "TambahKurang")
                    {
                        button4.Enabled = false;
                    }
                    else if (flagvisual == "Kali")
                    {
                        button5.Enabled = false;
                    }
                    else if (flagvisual == "Bagi")
                    {
                        button6.Enabled = false;
                    }

                    if (button4.Enabled || button5.Enabled || button6.Enabled)
                    {
                        lblSoal.Text = "Next...";
                        textBox10.Enabled = false;
                    }
                }
            }
        }

        public void SpeakComplete(object sender, SpeakCompletedEventArgs e)
        {
            if (e.Prompt.IsCompleted)
            {
                Start();

                textBox10.Text = "";
                textBox10.Enabled = true;
                textBox10.Focus();
            }

            if (e.Cancelled)
            {
                MessageBox.Show("Error Cancelled Speak Object", "Information!");
                textBox10.Enabled = false;
                lamalomba = lamalomba + 1;
                StartLomba();
            }

            if (e.Error != null)
            {
                MessageBox.Show("Error Speak Object " + Environment.NewLine + e.Error.Message, "Information!");
                textBox10.Enabled = false;
                lamalomba = lamalomba + 1;
                StartLomba();
            }
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
                label12.Text = "Kompetisi"; 
                label23.Text = "Suara";

                button1.Text = "Mulai";
                button2.Text = "Peraturan";
                button3.Text = "Hasil";

                checkBox1.Text = "Soal Baru";
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
                label12.Text = "Competition";
                label23.Text = "Voice";

                button1.Text = "Start";
                button2.Text = "Setting";
                button3.Text = "Result";

                checkBox1.Text = "New Question";
            }
        }

        private void LoadListSpeech()
        {
            speechSynthesizerObj.Dispose();
            speechSynthesizerObj = new SpeechSynthesizer();

            var installedVoices = speechSynthesizerObj.GetInstalledVoices();
            if (installedVoices.Count == 0)
            {
                MessageBox.Show(this,
                    "Your system don't have a 'Text to Speech' to make this work. Try install one for continue.",
                    "Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dtsp.Clear();
                dtsp.Columns.Add("Id", typeof(string));
                dtsp.Columns.Add("Name", typeof(string));
                dtsp.Columns.Add("Gender", typeof(string));

                foreach (InstalledVoice voice in installedVoices)
                {
                    dtsp.Rows.Add(voice.VoiceInfo.Name, voice.VoiceInfo.Description.Replace("Microsoft ", "").Replace(" (United States)", "").Replace(" (Indonesia)", ""), voice.VoiceInfo.Gender);
                }

                dtsp.DefaultView.Sort = "Name ASC";
                dtsp = dtsp.DefaultView.ToTable();

                comboBox2.DataSource = dtsp;
                comboBox2.DisplayMember = "Name";
                comboBox2.ValueMember = "Id";
            }            
        }

        private void Visibled(bool flag)
        {
            label1.Visible = flag;
            label2.Visible = flag;
            label3.Visible = flag;
            label4.Visible = flag;
            label5.Visible = flag;
            label6.Visible = flag;
            label7.Visible = flag;
            label8.Visible = flag;
            label9.Visible = flag;
            label10.Visible = flag;

            label13.Visible = flag;
            label14.Visible = flag;
            label15.Visible = flag;
            label16.Visible = flag;
            label17.Visible = flag;
            label18.Visible = flag;
            label19.Visible = flag;
            label20.Visible = flag;
            label21.Visible = flag;
            label22.Visible = flag;
        }

        private void SetButtonEnabledVisual(string flag)
        {
            var dtfind1 = dtSoalLombaV.Select("filter = 'TambahKurang' and flag = 'N'");    
            var dtfind2 = dtSoalLombaV.Select("filter = 'Kali' and flag = 'N'");      
            var dtfind3 = dtSoalLombaV.Select("filter = 'Bagi' and flag = 'N'");            

            if (flag == "TambahKurang")
            {
                if (dtfind2.Any())
                {
                    if (!button5.Enabled)
                    {
                        button5.Enabled = true;
                    }                                       
                }
                else
                {
                    button5.Enabled = false;
                }
                if (dtfind3.Any())
                {
                    if (!button6.Enabled)
                    {
                        button6.Enabled = true;
                    }                    
                }
                else
                {
                    button6.Enabled = false;
                }
            }
            else if (flag == "Kali")
            {
                if (dtfind1.Any())
                {
                    if (!button4.Enabled)
                    {
                        button4.Enabled = true;
                    }                    
                }
                else
                {
                    button4.Enabled = false;
                }
                if (dtfind3.Any())
                {
                    if (!button6.Enabled)
                    {
                        button6.Enabled = true;
                    }                    
                }
                else
                {
                    button6.Enabled = false;
                }
            }
            else if(flag == "Bagi")
            {
                if (dtfind1.Any())
                {
                    if (!button4.Enabled)
                    {
                        button4.Enabled = true;
                    }                    
                }
                else
                {
                    button4.Enabled = false;
                }
                if (dtfind2.Any())
                {
                    if (!button5.Enabled)
                    {
                        button5.Enabled = true;
                    }
                    
                }
                else
                {
                    button5.Enabled = false;
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //250 66
            if (panel2.Width == 250)
            {
                Visibled(false);
                this.timer2.Start();
            }
            else if (panel2.Width == 65)
            {
                Visibled(true);
                this.timer3.Start();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (panel2.Width <= 65)
            {
                this.timer2.Stop();
            }
            else
            {
                panel2.Width = panel2.Width - 5;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (panel2.Width >= 250)
            {
                this.timer3.Stop();
            }
            else
            {
                panel2.Width = panel2.Width + 5;
            }
        }
    }
}
