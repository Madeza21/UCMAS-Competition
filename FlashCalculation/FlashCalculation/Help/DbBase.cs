using FlashCalculation.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Help
{
    public class DbBase
    {
        SQLiteConnection sqlite_conn;

        static SQLiteConnection CreateConnection()
        {
            //https://www.codeguru.com/dotnet/using-sqlite-in-a-c-application/
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=ucmas.db3; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch 
            {
                return null;
            }
            return sqlite_conn;
        }

        public void OpenConnection()
        {
            sqlite_conn = CreateConnection();
        }

        public void CloseConnection()
        {
            sqlite_conn.Close();
        }

        public SQLiteParameter SqlParam(string ParameterName, DbType type, ParameterDirection direction)
        {
            SQLiteParameter parm = new SQLiteParameter();
            parm.ParameterName = ParameterName;
            parm.DbType = type;
            parm.Direction = direction;

            return parm;
        }
        public DataTable GetCabang(string pcode)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM tb_cabang where CABANG_CODE=@pcode";

            SQLiteParameter parm = new SQLiteParameter();
            
            parm = SqlParam("@pcode", DbType.String, ParameterDirection.Input);
            parm.Value = pcode;
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }
        public DataTable GetKompetisi(string pid, string ptgl, string pflag)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT tb_kompetisi.ROW_ID,   
                                              tb_kompetisi.CABANG_CODE,   
                                              tb_kompetisi.KOMPETISI_NAME,   
                                              tb_kompetisi.TANGGAL_KOMPETISI,   
                                              tb_kompetisi.JAM_MULAI,   
                                              tb_kompetisi.JAM_SAMPAI,   
                                              tb_kompetisi.JENIS_CODE,   
                                              tb_kompetisi.JENIS_NAME,   
                                              tb_kompetisi.TIPE,   
                                              tb_kompetisi.ROW_ID_KATEGORI,   
                                              tb_kompetisi.KATEGORI_CODE,   
                                              tb_kompetisi.KATEGORI_NAME,   
                                              tb_kompetisi.LAMA_PERLOMBAAN,   
                                              tb_kompetisi.KECEPATAN  
                                         FROM tb_peserta_kompetisi, tb_kompetisi
                                        WHERE tb_peserta_kompetisi.ROW_ID_KOMPETISI = tb_kompetisi.ROW_ID
                                          AND tb_peserta_kompetisi.ID_PESERTA =@pid
                                          AND tb_kompetisi.TANGGAL_KOMPETISI =@ptgl
                                          AND tb_kompetisi.IS_TRIAL =@pflag";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = pid;
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@ptgl", DbType.String, ParameterDirection.Input);
            parm.Value = ptgl;
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pflag", DbType.String, ParameterDirection.Input);
            parm.Value = pflag;
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }
        public string GetAppConfig(string pcode)
        {
            string str = "";

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT CONFIG_PARAM FROM tb_application_configuration WHERE CONFIG_CODE =@pcode";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pcode", DbType.String, ParameterDirection.Input);
            parm.Value = pcode;
            sqlite_cmd.Parameters.Add(parm);

            str = Convert.ToString(sqlite_cmd.ExecuteScalar());

            return str;
        }
        public void InsertUrl(Url[] url)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_url ( URL_CODE, URL_PARAM, URL_BODY, METHOD ) 
								        VALUES
                                          (@prm1,@prm2,@prm3,@prm4) ";

            for (int i = 0; i< url.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);                
                parm.Value = url[i].URL_CODE;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = url[i].URL_PARAM;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = url[i].URL_BODY;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = url[i].METHOD;
                sqlite_cmd.Parameters.Add(parm);

                sqlite_cmd.ExecuteNonQuery();
            }            
        }

        public void InsertCabang(Cabang[] cabang)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_cabang ( CABANG_CODE, CABANG_NAME, LOKASI, IS_PUSAT, ALAMAT, NO_TELP, EMAIL ) 
								        VALUES
                                          (@prm1,@prm2,@prm3,@prm4,@prm5,@prm6,@prm7) ";

            for (int i = 0; i < cabang.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);
                parm.Value = cabang[i].CABANG_CODE;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = cabang[i].CABANG_NAME;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = cabang[i].LOKASI;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = cabang[i].IS_PUSAT;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.String, ParameterDirection.Input);
                parm.Value = cabang[i].ALAMAT;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.String, ParameterDirection.Input);
                parm.Value = cabang[i].NO_TELP;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.String, ParameterDirection.Input);
                parm.Value = cabang[i].EMAIL;
                sqlite_cmd.Parameters.Add(parm);

                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public void InsertAppConfig(AppConfiguration[] config)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_application_configuration ( CONFIG_CODE, CONFIG_NAME, CONFIG_PARAM ) 
								        VALUES
                                          (@prm1,@prm2,@prm3) ";

            for (int i = 0; i < config.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);
                parm.Value = config[i].CONFIG_CODE;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = config[i].CONFIG_NAME;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = config[i].CONFIG_PARAM;
                sqlite_cmd.Parameters.Add(parm);

                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public void InsertPeserta(Peserta[] peserta)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_peserta ( ID_PESERTA, NAMA_PESERTA, JENIS_KELAMIN, TEMPAT_LAHIR,
	                                    TANGGAL_LAHIR, ALAMAT_PESERTA, SEKOLAH_PESERTA, NO_TELP_PESERTA, EMAIL_PESERTA,
	                                    IS_USMAS, TOKEN_PESERTA, CABANG_CODE ) 
								        VALUES
                                          (@prm1,@prm2,@prm3,@prm4,@prm5,@prm6,@prm7,@prm8,@prm9,@prm10,@prm11,@prm12) ";

            for (int i = 0; i < peserta.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].ID_PESERTA;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].NAMA_PESERTA;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].JENIS_KELAMIN;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].TEMPAT_LAHIR;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].TANGGAL_LAHIR;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].ALAMAT_PESERTA;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].SEKOLAH_PESERTA;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm8", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].NO_TELP_PESERTA;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm9", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].EMAIL_PESERTA;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm10", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].IS_USMAS;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm11", DbType.String, ParameterDirection.Input);
                parm.Value = Properties.Settings.Default.token;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm12", DbType.String, ParameterDirection.Input);
                parm.Value = peserta[i].CABANG_CODE;
                sqlite_cmd.Parameters.Add(parm);

                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public void InsertKompetisi(Kompetisi[] kompetisi)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_kompetisi ( ROW_ID, CABANG_CODE, KOMPETISI_NAME, TANGGAL_KOMPETISI, JAM_MULAI, JAM_SAMPAI,
			                    JENIS_CODE, JENIS_NAME, TIPE, ROW_ID_KATEGORI, KATEGORI_CODE, KATEGORI_NAME, LAMA_PERLOMBAAN, KECEPATAN, IS_TRIAL ) 
								        VALUES
                                          (@prm1,@prm2,@prm3,@prm4,@prm5,@prm6,@prm7,@prm8,@prm9,@prm10,@prm11,@prm12,@prm13,@prm14,@prm15) ";

            for (int i = 0; i < kompetisi.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].ROW_ID;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].CABANG_CODE;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].KOMPETISI_NAME;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = Properties.Settings.Default.trial == "Y" ? DateTime.Now.ToString("yyyy-MM-dd") : kompetisi[i].TANGGAL_KOMPETISI;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].JAM_MULAI;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].JAM_SAMPAI;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].JENIS_CODE;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm8", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].JENIS_NAME;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm9", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].TIPE;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm10", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].ROW_ID_KATEGORI;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm11", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].KATEGORI_CODE;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm12", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].KATEGORI_NAME;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm13", DbType.Decimal, ParameterDirection.Input);
                parm.Value = kompetisi[i].LAMA_PERLOMBAAN == null ? 180 : Convert.ToDecimal(kompetisi[i].LAMA_PERLOMBAAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm14", DbType.Decimal, ParameterDirection.Input);
                parm.Value = kompetisi[i].KECEPATAN == null ? 0 : Convert.ToDecimal(kompetisi[i].KECEPATAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm15", DbType.String, ParameterDirection.Input);
                parm.Value = Properties.Settings.Default.trial;
                sqlite_cmd.Parameters.Add(parm);

                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public void InsertKompetisiPeserta(Kompetisi[] kompetisi)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_peserta_kompetisi ( ROW_ID_KOMPETISI, ID_PESERTA ) 
								        VALUES
                                          (@prm1,@prm2) ";

            for (int i = 0; i < kompetisi.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].ROW_ID;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Properties.Settings.Default.siswa_id;
                sqlite_cmd.Parameters.Add(parm);

                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public void InsertParameterKompetisi(ParameterKompetisi[] Prmkompetisi)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_parameter_kompetisi 
                    ( ROW_ID_KOMPETISI, PARAMETER_ID, SOAL_DARI, SOAL_SAMPAI, PANJANG_DIGIT, JUMLAH_MUNCUL, JML_BARIS_PER_MUNCUL,
					  MAX_PANJANG_DIGIT, MAX_JML_DIGIT_PER_SOAL, JML_BARIS_PER_SOAL, MUNCUL_ANGKA_MINUS, MUNCUL_ANGKA_PERKALIAN, DIGIT_PERKALIAN,
					  MUNCUL_ANGKA_PEMBAGIAN, DIGIT_PEMBAGIAN, MUNCUL_ANGKA_DECIMAL, DIGIT_DECIMAL, FONT_SIZE, KECEPATAN ) 
					VALUES
                    (@prm1,@prm2,@prm3,@prm4,@prm5,@prm6,@prm7,@prm8,@prm9,@prm10,@prm11,@prm12,@prm13,@prm14,@prm15,@prm16,@prm17,@prm18,@prm19) ";

            for (int i = 0; i < Prmkompetisi.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].ROW_ID_KOMPETISI;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].PARAMETER_ID;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].SOAL_DARI == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].SOAL_DARI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].SOAL_SAMPAI == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].SOAL_SAMPAI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].PANJANG_DIGIT == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].PANJANG_DIGIT);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].JUMLAH_MUNCUL == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].JUMLAH_MUNCUL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].JML_BARIS_PER_MUNCUL == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].JML_BARIS_PER_MUNCUL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm8", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MAX_PANJANG_DIGIT == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].MAX_PANJANG_DIGIT);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm9", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MAX_JML_DIGIT_PER_SOAL == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].MAX_JML_DIGIT_PER_SOAL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm10", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].JML_BARIS_PER_SOAL == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].JML_BARIS_PER_SOAL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm11", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MUNCUL_ANGKA_MINUS;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm12", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MUNCUL_ANGKA_PERKALIAN;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm13", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].DIGIT_PERKALIAN == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].DIGIT_PERKALIAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm14", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MUNCUL_ANGKA_PEMBAGIAN;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm15", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].DIGIT_PEMBAGIAN == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].DIGIT_PEMBAGIAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm16", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MUNCUL_ANGKA_DECIMAL;
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm17", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].DIGIT_DECIMAL == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].DIGIT_DECIMAL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm18", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].FONT_SIZE == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].FONT_SIZE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm19", DbType.Decimal, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].KECEPATAN == null ? 0 : Convert.ToDecimal(Prmkompetisi[i].KECEPATAN);
                sqlite_cmd.Parameters.Add(parm);

                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetSoalKompetisi(string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT *
                                         FROM tb_soal_kompetisi
                                        WHERE tb_soal_kompetisi.ROW_ID_KOMPETISI =@pid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = pid;
            sqlite_cmd.Parameters.Add(parm);            

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public void Query(string query)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = query;

            sqlite_cmd.ExecuteNonQuery();
        }
    }
}
