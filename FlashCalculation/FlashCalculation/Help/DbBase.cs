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
        SQLiteTransaction sqlite_trans = null;

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

        public void BeginTransaction()
        {
            sqlite_trans = sqlite_conn.BeginTransaction();
        }
        public Boolean IsTransactionStarted()
        {
            return sqlite_trans != null;
        }
        public void commit()
        {
            sqlite_trans.Commit();
            sqlite_trans = null;
        }
        public void rollback()
        {
            sqlite_trans.Rollback();
            sqlite_trans = null;
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
                                              tb_kompetisi.KECEPATAN,
                                              tb_kompetisi.IS_TRIAL,   
                                              tb_kompetisi.BAHASA,   
                                              tb_kompetisi.START_FLAG
                                         FROM tb_peserta_kompetisi, tb_kompetisi
                                        WHERE tb_peserta_kompetisi.ROW_ID_KOMPETISI = tb_kompetisi.ROW_ID
                                          AND tb_peserta_kompetisi.ID_PESERTA =@pid
                                          AND tb_kompetisi.TANGGAL_KOMPETISI =@ptgl
                                          AND tb_kompetisi.IS_TRIAL =@pflag
                                        ORDER BY tb_kompetisi.KOMPETISI_NAME ASC";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@ptgl", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(ptgl);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pflag", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pflag);
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
            parm.Value = Encryptor.Encrypt(pcode);
            sqlite_cmd.Parameters.Add(parm);

            str = Convert.ToString(sqlite_cmd.ExecuteScalar());

            return Encryptor.Decrypt(str);
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
                parm.Value = Encryptor.Encrypt(url[i].URL_CODE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(url[i].URL_PARAM);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(url[i].URL_BODY);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(url[i].METHOD);
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
                parm.Value = Encryptor.Encrypt(cabang[i].CABANG_CODE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(cabang[i].CABANG_NAME);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(cabang[i].LOKASI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(cabang[i].IS_PUSAT);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(cabang[i].ALAMAT);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(cabang[i].NO_TELP);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(cabang[i].EMAIL);
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
                parm.Value = Encryptor.Encrypt(config[i].CONFIG_CODE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(config[i].CONFIG_NAME);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(config[i].CONFIG_PARAM);
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
                parm.Value = Encryptor.Encrypt(peserta[i].ID_PESERTA);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].NAMA_PESERTA);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].JENIS_KELAMIN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].TEMPAT_LAHIR);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].TANGGAL_LAHIR);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].ALAMAT_PESERTA);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].SEKOLAH_PESERTA);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm8", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].NO_TELP_PESERTA);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm9", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].EMAIL_PESERTA);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm10", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].IS_USMAS);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm11", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Properties.Settings.Default.token);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm12", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(peserta[i].CABANG_CODE);
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
			                    JENIS_CODE, JENIS_NAME, TIPE, ROW_ID_KATEGORI, KATEGORI_CODE, KATEGORI_NAME, LAMA_PERLOMBAAN, KECEPATAN, IS_TRIAL,
                                BAHASA, START_FLAG) 
								        VALUES
                                          (@prm1,@prm2,@prm3,@prm4,@prm5,@prm6,@prm7,@prm8,@prm9,@prm10,@prm11,@prm12,@prm13,@prm14,@prm15,@prm16,@prm17) ";

            for (int i = 0; i < kompetisi.Length; i++)
            {
                sqlite_cmd.Parameters.Clear();

                parm = SqlParam("@prm1", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].ROW_ID);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].CABANG_CODE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].KOMPETISI_NAME);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = Properties.Settings.Default.trial == "Y" ? Encryptor.Encrypt(DateTime.Now.ToString("yyyy-MM-dd")) : Encryptor.Encrypt(kompetisi[i].TANGGAL_KOMPETISI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].JAM_MULAI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].JAM_SAMPAI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].JENIS_CODE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm8", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].JENIS_NAME);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm9", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].TIPE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm10", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].ROW_ID_KATEGORI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm11", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].KATEGORI_CODE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm12", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].KATEGORI_NAME);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm13", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].LAMA_PERLOMBAAN == null ? Encryptor.Encrypt("180") : Encryptor.Encrypt(kompetisi[i].LAMA_PERLOMBAAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm14", DbType.String, ParameterDirection.Input);
                parm.Value = kompetisi[i].KECEPATAN == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(kompetisi[i].KECEPATAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm15", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Properties.Settings.Default.trial);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm16", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].BAHASA);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm17", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(kompetisi[i].FLAG);
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
                parm.Value = Encryptor.Encrypt(kompetisi[i].ROW_ID);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Properties.Settings.Default.siswa_id);
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
                parm.Value = Encryptor.Encrypt(Prmkompetisi[i].ROW_ID_KOMPETISI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm2", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Prmkompetisi[i].PARAMETER_ID);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm3", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].SOAL_DARI == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].SOAL_DARI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm4", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].SOAL_SAMPAI == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].SOAL_SAMPAI);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm5", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].PANJANG_DIGIT == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].PANJANG_DIGIT);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm6", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].JUMLAH_MUNCUL == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].JUMLAH_MUNCUL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm7", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].JML_BARIS_PER_MUNCUL == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].JML_BARIS_PER_MUNCUL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm8", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MAX_PANJANG_DIGIT == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].MAX_PANJANG_DIGIT);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm9", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].MAX_JML_DIGIT_PER_SOAL == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].MAX_JML_DIGIT_PER_SOAL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm10", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].JML_BARIS_PER_SOAL == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].JML_BARIS_PER_SOAL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm11", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Prmkompetisi[i].MUNCUL_ANGKA_MINUS);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm12", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Prmkompetisi[i].MUNCUL_ANGKA_PERKALIAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm13", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].DIGIT_PERKALIAN == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].DIGIT_PERKALIAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm14", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Prmkompetisi[i].MUNCUL_ANGKA_PEMBAGIAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm15", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].DIGIT_PEMBAGIAN == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].DIGIT_PEMBAGIAN);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm16", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(Prmkompetisi[i].MUNCUL_ANGKA_DECIMAL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm17", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].DIGIT_DECIMAL == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].DIGIT_DECIMAL);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm18", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].FONT_SIZE == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].FONT_SIZE);
                sqlite_cmd.Parameters.Add(parm);
                parm = SqlParam("@prm19", DbType.String, ParameterDirection.Input);
                parm.Value = Prmkompetisi[i].KECEPATAN == null ? Encryptor.Encrypt("0") : Encryptor.Encrypt(Prmkompetisi[i].KECEPATAN);
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
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);            

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetParameterKompetisi(string pid, string pdate, string prowid)
        {
            string sql = "";
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sql = @"SELECT tb_parameter_kompetisi.ROW_ID_KOMPETISI, PARAMETER_ID, SOAL_DARI, SOAL_SAMPAI, PANJANG_DIGIT, JUMLAH_MUNCUL,
                            JML_BARIS_PER_MUNCUL, MAX_PANJANG_DIGIT, MAX_JML_DIGIT_PER_SOAL, JML_BARIS_PER_SOAL, MUNCUL_ANGKA_MINUS,
                            MUNCUL_ANGKA_PERKALIAN, DIGIT_PERKALIAN, MUNCUL_ANGKA_PEMBAGIAN, DIGIT_PEMBAGIAN, MUNCUL_ANGKA_DECIMAL,
                            DIGIT_DECIMAL, FONT_SIZE, tb_parameter_kompetisi.kecepatan, tb_kompetisi.KOMPETISI_NAME,
                            tb_kompetisi.JENIS_NAME, tb_kompetisi.TIPE, tb_kompetisi.KATEGORI_NAME, tb_kompetisi.BAHASA
                        FROM tb_parameter_kompetisi, tb_peserta_kompetisi, tb_kompetisi
                    WHERE tb_parameter_kompetisi.ROW_ID_KOMPETISI = tb_peserta_kompetisi.ROW_ID_KOMPETISI
                        AND tb_parameter_kompetisi.ROW_ID_KOMPETISI = tb_kompetisi.ROW_ID
                        AND ID_PESERTA =@pid
                        AND tb_kompetisi.TANGGAL_KOMPETISI =@pdate ";

            if(prowid != "")
            {
                sql += " AND tb_kompetisi.ROW_ID =@prowid ";
            }

            sql += " ORDER BY tb_parameter_kompetisi.ROW_ID_KOMPETISI ASC, SOAL_DARI ASC ";

            sqlite_cmd.CommandText = sql;
            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pdate", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pdate);
            sqlite_cmd.Parameters.Add(parm);
            if (prowid != "")
            {
                parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
                parm.Value = Encryptor.Encrypt(prowid);
                sqlite_cmd.Parameters.Add(parm);
            }

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

        public string UpdateDataTable(DataTable dt, string astrUpdateTable,
            string[] astrUpdateColumns, string[] astrUpdateKeys)
        {
            //todo: develop generic procedure for update database
            //      from  datatable object
            Boolean lblnFirst;
            Boolean lblnTest;
            string sqlString = "";
            string sqlString2 = "";
            string lstrUpdateColumn;
            SQLiteParameter prm = new SQLiteParameter();
            string lstrTypeTest = "";
            DateTime ldtTypeTest = DateTime.Now;
            decimal ldTypetest = 0;

            //SQLiteConnection connection = new SQLiteConnection("Data Source=ucmas.db3; Version = 3; New = True; Compress = True;");
            //connection.Open();
            SQLiteCommand command = sqlite_conn.CreateCommand();
            //SQLiteTransaction transaction = connection.BeginTransaction();

            foreach (DataRow therow in dt.Rows)
            {                
                //Inserting
                if (therow.RowState == DataRowState.Added)
                {
                    try
                    {
                        command.Parameters.Clear();
                        //updatable column
                        sqlString = "Insert Into " + astrUpdateTable;
                        lstrUpdateColumn = "";
                        lblnFirst = true;
                        foreach (string astrUpdateColumn in astrUpdateColumns)
                        {
                            lblnTest = false;
                            foreach (DataColumn thecol1 in dt.Columns)
                            {
                                if (astrUpdateColumn == thecol1.ColumnName)
                                {
                                    lstrUpdateColumn = astrUpdateColumn;
                                    lblnTest = true;
                                    break;
                                }
                            }
                            if (lblnTest == false)
                            {
                                //transaction.Rollback();
                                command.Dispose();
                                //connection.Dispose();

                                throw new Exception("The column '" + astrUpdateColumn + "' not exists while insert!");
                            }

                            DataColumn thecol = dt.Columns[astrUpdateColumn];
                            if (lblnFirst)
                            {
                                sqlString += " (";
                                sqlString2 = " values (";
                                lblnFirst = false;
                            }
                            else
                            {
                                sqlString += ", ";
                                sqlString2 += ",";
                            }
                            sqlString += thecol.ColumnName;
                            sqlString2 += "@" + thecol.ColumnName;

                            if (thecol.DataType == lstrTypeTest.GetType())
                            {
                                prm = SqlParam("@" + lstrUpdateColumn, DbType.String, ParameterDirection.Input);
                                prm.Value = therow[thecol.ColumnName, DataRowVersion.Current];
                            }
                            else if (thecol.DataType == ldTypetest.GetType())
                            {
                                prm = SqlParam("@" + lstrUpdateColumn, DbType.Decimal, ParameterDirection.Input);
                                prm.Value = therow[thecol.ColumnName, DataRowVersion.Current];
                            }
                            else if (thecol.DataType == ldtTypeTest.GetType())
                            {
                                prm = SqlParam("@" + lstrUpdateColumn, DbType.DateTime, ParameterDirection.Input);
                                prm.Value = therow[thecol.ColumnName, DataRowVersion.Current];
                            }
                            else
                            {
                                prm = SqlParam("@" + lstrUpdateColumn, DbType.String, ParameterDirection.Input);
                                prm.Value = therow[thecol.ColumnName, DataRowVersion.Current];
                            }

                            command.Parameters.Add(prm);
                        }
                        command.CommandText = sqlString + ")" + sqlString2 + ")";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        //transaction.Rollback();
                        command.Dispose();
                        //connection.Dispose();

                        throw new Exception(ee.Message);
                    }
                }
            }

            //transaction.Commit();
            command.Dispose();
            //connection.Dispose();

            return "OK";
        }

        public string GetTypeKompetisi(string prowid, string ptgl)
        {
            string str = "";

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT TIPE
	                                    FROM tb_kompetisi
	                                    WHERE ROW_ID =@prowid
	                                    AND TANGGAL_KOMPETISI =@ptgl";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@ptgl", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(ptgl);
            sqlite_cmd.Parameters.Add(parm);

            str = Convert.ToString(sqlite_cmd.ExecuteScalar());

            return Encryptor.Decrypt(str);
        }

        public string GetFlagKompetisi(string prowid, string ptgl)
        {
            string str = "";

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT START_FLAG
	                                    FROM tb_kompetisi
	                                    WHERE ROW_ID =@prowid
	                                    AND TANGGAL_KOMPETISI =@ptgl";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@ptgl", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(ptgl);
            sqlite_cmd.Parameters.Add(parm);

            str = Convert.ToString(sqlite_cmd.ExecuteScalar());

            return Encryptor.Decrypt(str);
        }

        public int GetJawabanKompetisi(string prowid, string ppeserta)
        {
            int str = 0;

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT COUNT(*)
	                                    FROM  tb_jawaban_kompetisi 
	                                    WHERE ROW_ID_KOMPETISI =@prowid
	                                    AND ID_PESERTA =@ppeserta";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@ppeserta", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(ppeserta);
            sqlite_cmd.Parameters.Add(parm);

            string data = Convert.ToString(sqlite_cmd.ExecuteScalar());
            str = Convert.ToInt32(data);

            return str;
        }

        public DataTable GetKompetisiID(string pid, string prowid)
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
                                              tb_kompetisi.KECEPATAN,
                                              tb_kompetisi.IS_TRIAL,
                                              tb_kompetisi.BAHASA
                                         FROM tb_peserta_kompetisi, tb_kompetisi
                                        WHERE tb_peserta_kompetisi.ROW_ID_KOMPETISI = tb_kompetisi.ROW_ID
                                          AND tb_peserta_kompetisi.ID_PESERTA =@pid
                                          AND tb_kompetisi.ROW_ID =@prowid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetSoalKompetisiID(string pid, string prowid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT tb_soal_kompetisi.row_id_kompetisi,   
                                                tb_soal_kompetisi.no_soal,   
                                                tb_soal_kompetisi.jumlah_muncul,   
                                                tb_soal_kompetisi.jml_baris_per_muncul,   
                                                tb_soal_kompetisi.angka1,   
                                                tb_soal_kompetisi.angka2,   
                                                tb_soal_kompetisi.angka3,   
                                                tb_soal_kompetisi.angka4,   
                                                tb_soal_kompetisi.angka5,   
                                                tb_soal_kompetisi.angka6,   
                                                tb_soal_kompetisi.angka7,   
                                                tb_soal_kompetisi.angka8,   
                                                tb_soal_kompetisi.angka9,   
                                                tb_soal_kompetisi.angka10,   
                                                tb_soal_kompetisi.angka11,   
                                                tb_soal_kompetisi.angka12,   
                                                tb_soal_kompetisi.angka13,   
                                                tb_soal_kompetisi.angka14,   
                                                tb_soal_kompetisi.angka15,   
                                                tb_soal_kompetisi.angka16,   
                                                tb_soal_kompetisi.angka17,   
                                                tb_soal_kompetisi.angka18,   
                                                tb_soal_kompetisi.angka19,   
                                                tb_soal_kompetisi.angka20,   
                                                tb_soal_kompetisi.angka21,   
                                                tb_soal_kompetisi.angka22,   
                                                tb_soal_kompetisi.angka23,   
                                                tb_soal_kompetisi.angka24,   
                                                tb_soal_kompetisi.angka25,   
                                                tb_soal_kompetisi.angka26,   
                                                tb_soal_kompetisi.angka27,   
                                                tb_soal_kompetisi.angka28,   
                                                tb_soal_kompetisi.angka29,   
                                                tb_soal_kompetisi.angka30,   
                                                tb_soal_kompetisi.angka31,   
                                                tb_soal_kompetisi.angka32,   
                                                tb_soal_kompetisi.angka33,   
                                                tb_soal_kompetisi.angka34,   
                                                tb_soal_kompetisi.angka35,   
                                                tb_soal_kompetisi.angka36,   
                                                tb_soal_kompetisi.angka37,   
                                                tb_soal_kompetisi.angka38,   
                                                tb_soal_kompetisi.angka39,   
                                                tb_soal_kompetisi.angka40,   
                                                tb_soal_kompetisi.angka41,   
                                                tb_soal_kompetisi.angka42,   
                                                tb_soal_kompetisi.angka43,   
                                                tb_soal_kompetisi.angka44,   
                                                tb_soal_kompetisi.angka45,   
                                                tb_soal_kompetisi.angka46,   
                                                tb_soal_kompetisi.angka47,   
                                                tb_soal_kompetisi.angka48,   
                                                tb_soal_kompetisi.angka49,   
                                                tb_soal_kompetisi.angka50,   
                                                tb_soal_kompetisi.angka51,   
                                                tb_soal_kompetisi.angka52,   
                                                tb_soal_kompetisi.angka53,   
                                                tb_soal_kompetisi.angka54,   
                                                tb_soal_kompetisi.angka55,   
                                                tb_soal_kompetisi.angka56,   
                                                tb_soal_kompetisi.angka57,   
                                                tb_soal_kompetisi.angka58,   
                                                tb_soal_kompetisi.angka59,   
                                                tb_soal_kompetisi.angka60,   
                                                tb_soal_kompetisi.angka61,   
                                                tb_soal_kompetisi.angka62,   
                                                tb_soal_kompetisi.angka63,   
                                                tb_soal_kompetisi.angka64,   
                                                tb_soal_kompetisi.angka65,   
                                                tb_soal_kompetisi.angka66,   
                                                tb_soal_kompetisi.angka67,   
                                                tb_soal_kompetisi.angka68,   
                                                tb_soal_kompetisi.angka69,   
                                                tb_soal_kompetisi.angka70,   
                                                tb_soal_kompetisi.angka71,   
                                                tb_soal_kompetisi.angka72,   
                                                tb_soal_kompetisi.angka73,   
                                                tb_soal_kompetisi.angka74,   
                                                tb_soal_kompetisi.angka75,   
                                                tb_soal_kompetisi.angka76,   
                                                tb_soal_kompetisi.angka77,   
                                                tb_soal_kompetisi.angka78,   
                                                tb_soal_kompetisi.angka79,   
                                                tb_soal_kompetisi.angka80,   
                                                tb_soal_kompetisi.angka81,   
                                                tb_soal_kompetisi.angka82,   
                                                tb_soal_kompetisi.angka83,   
                                                tb_soal_kompetisi.angka84,   
                                                tb_soal_kompetisi.angka85,   
                                                tb_soal_kompetisi.angka86,   
                                                tb_soal_kompetisi.angka87,   
                                                tb_soal_kompetisi.angka88,   
                                                tb_soal_kompetisi.angka89,   
                                                tb_soal_kompetisi.angka90,   
                                                tb_soal_kompetisi.angka91,   
                                                tb_soal_kompetisi.angka92,   
                                                tb_soal_kompetisi.angka93,   
                                                tb_soal_kompetisi.angka94,   
                                                tb_soal_kompetisi.angka95,   
                                                tb_soal_kompetisi.angka96,   
                                                tb_soal_kompetisi.angka97,   
                                                tb_soal_kompetisi.angka98,   
                                                tb_soal_kompetisi.angka99,   
                                                tb_soal_kompetisi.angka100,   
                                                tb_soal_kompetisi.angka101,   
                                                tb_soal_kompetisi.angka102,   
                                                tb_soal_kompetisi.angka103,   
                                                tb_soal_kompetisi.angka104,   
                                                tb_soal_kompetisi.angka105,   
                                                tb_soal_kompetisi.angka106,   
                                                tb_soal_kompetisi.angka107,   
                                                tb_soal_kompetisi.angka108,   
                                                tb_soal_kompetisi.angka109,   
                                                tb_soal_kompetisi.angka110,   
                                                tb_soal_kompetisi.angka111,   
                                                tb_soal_kompetisi.angka112,   
                                                tb_soal_kompetisi.angka113,   
                                                tb_soal_kompetisi.angka114,   
                                                tb_soal_kompetisi.angka115,   
                                                tb_soal_kompetisi.angka116,   
                                                tb_soal_kompetisi.angka117,   
                                                tb_soal_kompetisi.angka118,   
                                                tb_soal_kompetisi.angka119,   
                                                tb_soal_kompetisi.angka120,   
                                                tb_soal_kompetisi.angka121,   
                                                tb_soal_kompetisi.angka122,   
                                                tb_soal_kompetisi.angka123,   
                                                tb_soal_kompetisi.angka124,   
                                                tb_soal_kompetisi.angka125,   
                                                tb_soal_kompetisi.angka126,   
                                                tb_soal_kompetisi.angka127,   
                                                tb_soal_kompetisi.angka128,   
                                                tb_soal_kompetisi.angka129,   
                                                tb_soal_kompetisi.angka130,   
                                                tb_soal_kompetisi.angka131,   
                                                tb_soal_kompetisi.angka132,   
                                                tb_soal_kompetisi.angka133,   
                                                tb_soal_kompetisi.angka134,   
                                                tb_soal_kompetisi.angka135,   
                                                tb_soal_kompetisi.angka136,   
                                                tb_soal_kompetisi.angka137,   
                                                tb_soal_kompetisi.angka138,   
                                                tb_soal_kompetisi.angka139,   
                                                tb_soal_kompetisi.angka140,   
                                                tb_soal_kompetisi.angka141,   
                                                tb_soal_kompetisi.angka142,   
                                                tb_soal_kompetisi.angka143,   
                                                tb_soal_kompetisi.angka144,   
                                                tb_soal_kompetisi.angka145,   
                                                tb_soal_kompetisi.angka146,   
                                                tb_soal_kompetisi.angka147,   
                                                tb_soal_kompetisi.angka148,   
                                                tb_soal_kompetisi.angka149,   
                                                tb_soal_kompetisi.angka150,   
                                                tb_soal_kompetisi.perkalian1,   
                                                tb_soal_kompetisi.perkalian2,   
                                                tb_soal_kompetisi.perkalian3,   
                                                tb_soal_kompetisi.perkalian4,   
                                                tb_soal_kompetisi.perkalian5,   
                                                tb_soal_kompetisi.perkalian6,   
                                                tb_soal_kompetisi.perkalian7,   
                                                tb_soal_kompetisi.perkalian8,   
                                                tb_soal_kompetisi.perkalian9,   
                                                tb_soal_kompetisi.perkalian10,   
                                                tb_soal_kompetisi.pembagian1,   
                                                tb_soal_kompetisi.pembagian2,   
                                                tb_soal_kompetisi.pembagian3,   
                                                tb_soal_kompetisi.pembagian4,   
                                                tb_soal_kompetisi.pembagian5,   
                                                tb_soal_kompetisi.pembagian6,   
                                                tb_soal_kompetisi.pembagian7,   
                                                tb_soal_kompetisi.pembagian8,   
                                                tb_soal_kompetisi.pembagian9,   
                                                tb_soal_kompetisi.pembagian10,   
                                                tb_soal_kompetisi.kunci_jawaban,   
                                                tb_soal_kompetisi.angkamuncul1,   
                                                tb_soal_kompetisi.angkamuncul2,   
                                                tb_soal_kompetisi.angkamuncul3,   
                                                tb_soal_kompetisi.angkamuncul4,   
                                                tb_soal_kompetisi.angkamuncul5,   
                                                tb_soal_kompetisi.angkamuncul6,   
                                                tb_soal_kompetisi.angkamuncul7,   
                                                tb_soal_kompetisi.angkamuncul8,   
                                                tb_soal_kompetisi.angkamuncul9,   
                                                tb_soal_kompetisi.angkamuncul10,   
                                                tb_soal_kompetisi.angkamuncul11,   
                                                tb_soal_kompetisi.angkamuncul12,   
                                                tb_soal_kompetisi.angkamuncul13,   
                                                tb_soal_kompetisi.angkamuncul14,   
                                                tb_soal_kompetisi.angkamuncul15,   
                                                tb_soal_kompetisi.angkamuncul16,   
                                                tb_soal_kompetisi.angkamuncul17,   
                                                tb_soal_kompetisi.angkamuncul18,   
                                                tb_soal_kompetisi.angkamuncul19,   
                                                tb_soal_kompetisi.angkamuncul20,   
                                                tb_soal_kompetisi.angkamuncul21,   
                                                tb_soal_kompetisi.angkamuncul22,   
                                                tb_soal_kompetisi.angkamuncul23,   
                                                tb_soal_kompetisi.angkamuncul24,   
                                                tb_soal_kompetisi.angkamuncul25,   
                                                tb_soal_kompetisi.angkamuncul26,   
                                                tb_soal_kompetisi.angkamuncul27,   
                                                tb_soal_kompetisi.angkamuncul28,   
                                                tb_soal_kompetisi.angkamuncul29,   
                                                tb_soal_kompetisi.angkamuncul30,   
                                                tb_soal_kompetisi.angkamuncul31,   
                                                tb_soal_kompetisi.angkamuncul32,   
                                                tb_soal_kompetisi.angkamuncul33,   
                                                tb_soal_kompetisi.angkamuncul34,   
                                                tb_soal_kompetisi.angkamuncul35,   
                                                tb_soal_kompetisi.angkamuncul36,   
                                                tb_soal_kompetisi.angkamuncul37,   
                                                tb_soal_kompetisi.angkamuncul38,   
                                                tb_soal_kompetisi.angkamuncul39,   
                                                tb_soal_kompetisi.angkamuncul40,   
                                                tb_soal_kompetisi.angkamuncul41,   
                                                tb_soal_kompetisi.angkamuncul42,   
                                                tb_soal_kompetisi.angkamuncul43,   
                                                tb_soal_kompetisi.angkamuncul44,   
                                                tb_soal_kompetisi.angkamuncul45,   
                                                tb_soal_kompetisi.angkamuncul46,   
                                                tb_soal_kompetisi.angkamuncul47,   
                                                tb_soal_kompetisi.angkamuncul48,   
                                                tb_soal_kompetisi.angkamuncul49,   
                                                tb_soal_kompetisi.angkamuncul50,   
                                                tb_soal_kompetisi.kecepatan,
                                                tb_soal_kompetisi.angkalistening1,
                                                tb_soal_kompetisi.angkalistening2,
                                                tb_soal_kompetisi.angkalistening3,
                                                tb_soal_kompetisi.angkalistening4,
                                                tb_soal_kompetisi.angkalistening5,
                                                tb_soal_kompetisi.filter,
                                                @pflag as flag
                                         FROM tb_soal_kompetisi, tb_peserta_kompetisi 
                                        WHERE tb_peserta_kompetisi.ROW_ID_KOMPETISI = tb_soal_kompetisi.ROW_ID_KOMPETISI
                                          AND tb_peserta_kompetisi.ID_PESERTA =@pid
                                          AND tb_soal_kompetisi.row_id_kompetisi =@prowid
                                        ORDER BY tb_soal_kompetisi.no_soal ASC";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pflag", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt("N");
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetJawabanKompetisi(string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT *
                                         FROM tb_jawaban_kompetisi
                                        WHERE tb_jawaban_kompetisi.ROW_ID_KOMPETISI =@pid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetKompetisiView(string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT tb_kompetisi.ROW_ID,   
                                             tb_kompetisi.CABANG_CODE,      
                                             (SELECT 	tb_cabang.CABANG_NAME FROM tb_cabang WHERE tb_cabang.CABANG_CODE = tb_kompetisi.CABANG_CODE) as CABANG_NAME,   
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
                                        FROM tb_kompetisi   
                                    WHERE tb_kompetisi.ROW_ID =@pid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetParameterKompetisiView(string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT *    
                                        FROM tb_parameter_kompetisi   
                                    where tb_parameter_kompetisi.ROW_ID_KOMPETISI =@pid
                                     order by tb_parameter_kompetisi.SOAL_DARI ASC";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetJawabanView(string prowid, string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"  SELECT tb_jawaban_kompetisi.ROW_ID_KOMPETISI,   
                                                 tb_jawaban_kompetisi.ID_PESERTA,   
                                                 tb_jawaban_kompetisi.SOAL_NO,   
                                                 tb_jawaban_kompetisi.PERTANYAAN,   
                                                 tb_jawaban_kompetisi.JAWABAN_PESERTA,   
                                                 tb_jawaban_kompetisi.JAWAB_DETIK_BERAPA,   
                                                 tb_jawaban_kompetisi.JAWAB_DATE,   
                                                 tb_jawaban_kompetisi.KUNCI_JAWABAN,   
                                                 tb_jawaban_kompetisi.SCORE_PESERTA,   
                                                 tb_jawaban_kompetisi.IS_KIRIM  
                                            FROM tb_jawaban_kompetisi  
                                        WHERE tb_jawaban_kompetisi.ROW_ID_KOMPETISI =@prowid 
                                        AND tb_jawaban_kompetisi.ID_PESERTA =@pid ";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetJawabanTerkirim(string prowid, string pid, string flagkirim)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"  SELECT tb_jawaban_kompetisi.ROW_ID_KOMPETISI,   
                                                 tb_jawaban_kompetisi.ID_PESERTA,   
                                                 tb_jawaban_kompetisi.SOAL_NO,   
                                                 tb_jawaban_kompetisi.PERTANYAAN,   
                                                 tb_jawaban_kompetisi.JAWABAN_PESERTA,   
                                                 tb_jawaban_kompetisi.JAWAB_DETIK_BERAPA,   
                                                 tb_jawaban_kompetisi.JAWAB_DATE,   
                                                 tb_jawaban_kompetisi.KUNCI_JAWABAN,   
                                                 tb_jawaban_kompetisi.SCORE_PESERTA,   
                                                 tb_jawaban_kompetisi.IS_KIRIM  
                                            FROM tb_jawaban_kompetisi  
                                        WHERE tb_jawaban_kompetisi.ROW_ID_KOMPETISI =@prowid 
                                        AND tb_jawaban_kompetisi.ID_PESERTA =@pid
                                        AND tb_jawaban_kompetisi.IS_KIRIM =@flagkirim";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@flagkirim", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(flagkirim);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public void InsertKompetisiTrial(string rowid, string linenum, string rowidkompetisi, string tglmulai)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            SQLiteParameter parm = new SQLiteParameter();

            sqlite_cmd.CommandText = @"INSERT INTO tb_kompetisi_trial ( ROW_ID, LINE_NUM, ROW_ID_KOMPETISI, CABANG_CODE, KOMPETISI_NAME, TANGGAL_KOMPETISI, JAM_MULAI, JAM_SAMPAI,
			                    JENIS_CODE, JENIS_NAME, TIPE, ROW_ID_KATEGORI, KATEGORI_CODE, KATEGORI_NAME, LAMA_PERLOMBAAN, KECEPATAN, IS_TRIAL,
                                BAHASA, START_FLAG, TANGGAL_MULAI) 
                                SELECT @prowid,@plinenum, ROW_ID, CABANG_CODE, KOMPETISI_NAME, TANGGAL_KOMPETISI, JAM_MULAI, JAM_SAMPAI,
			                    JENIS_CODE, JENIS_NAME, TIPE, ROW_ID_KATEGORI, KATEGORI_CODE, KATEGORI_NAME, LAMA_PERLOMBAAN, KECEPATAN, IS_TRIAL,
                                BAHASA, START_FLAG,@ptglmulai FROM tb_kompetisi WHERE ROW_ID =@prowidkompetisi ";

            sqlite_cmd.Parameters.Clear();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(rowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@plinenum", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(linenum);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@ptglmulai", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(tglmulai);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@prowidkompetisi", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(rowidkompetisi);
            sqlite_cmd.Parameters.Add(parm);

            sqlite_cmd.ExecuteNonQuery();
        }

        public int CountKompetisiTrial(string prowid)
        {
            int str = 0;

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT COUNT(*)
	                                    FROM  tb_kompetisi_trial 
	                                    WHERE ROW_ID_KOMPETISI =@prowid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);

            string data = Convert.ToString(sqlite_cmd.ExecuteScalar());
            str = Convert.ToInt32(data);

            return str + 1;
        }

        public DataTable GetJawabanKompetisiTrial(string prowid, string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT *
                                         FROM tb_jawaban_kompetisi_trial
                                        WHERE tb_jawaban_kompetisi_trial.ROW_ID_HDR =@prowid 
                                          AND tb_jawaban_kompetisi_trial.ROW_ID_KOMPETISI =@pid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetKompetisiTrialView(string pid, string prowid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT tb_kompetisi_trial.ROW_ID,
                                             tb_kompetisi_trial.LINE_NUM,
                                             tb_kompetisi_trial.ROW_ID_KOMPETISI,
                                             tb_kompetisi_trial.CABANG_CODE,      
                                             (SELECT tb_cabang.CABANG_NAME FROM tb_cabang WHERE tb_cabang.CABANG_CODE = tb_kompetisi_trial.CABANG_CODE) as CABANG_NAME,   
                                             tb_kompetisi_trial.KOMPETISI_NAME,   
                                             tb_kompetisi_trial.TANGGAL_KOMPETISI,   
                                             tb_kompetisi_trial.JAM_MULAI,   
                                             tb_kompetisi_trial.JAM_SAMPAI,   
                                             tb_kompetisi_trial.JENIS_CODE,   
                                             tb_kompetisi_trial.JENIS_NAME,   
                                             tb_kompetisi_trial.TIPE,   
                                             tb_kompetisi_trial.ROW_ID_KATEGORI,   
                                             tb_kompetisi_trial.KATEGORI_CODE,   
                                             tb_kompetisi_trial.KATEGORI_NAME,   
                                             tb_kompetisi_trial.LAMA_PERLOMBAAN,   
                                             tb_kompetisi_trial.KECEPATAN  
                                        FROM tb_kompetisi_trial   
                                    WHERE tb_kompetisi_trial.ROW_ID =@pid
                                      AND tb_kompetisi_trial.ROW_ID_KOMPETISI =@prowid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetKompetisiTrialListView(string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"SELECT tb_kompetisi_trial.ROW_ID,
                                             tb_kompetisi_trial.LINE_NUM,
                                             tb_kompetisi_trial.ROW_ID_KOMPETISI,
                                             tb_kompetisi_trial.CABANG_CODE,      
                                             (SELECT tb_cabang.CABANG_NAME FROM tb_cabang WHERE tb_cabang.CABANG_CODE = tb_kompetisi_trial.CABANG_CODE) as CABANG_NAME,   
                                             tb_kompetisi_trial.KOMPETISI_NAME,   
                                             tb_kompetisi_trial.TANGGAL_KOMPETISI,   
                                             tb_kompetisi_trial.JAM_MULAI,   
                                             tb_kompetisi_trial.JAM_SAMPAI,   
                                             tb_kompetisi_trial.JENIS_CODE,   
                                             tb_kompetisi_trial.JENIS_NAME,   
                                             tb_kompetisi_trial.TIPE,   
                                             tb_kompetisi_trial.ROW_ID_KATEGORI,   
                                             tb_kompetisi_trial.KATEGORI_CODE,   
                                             tb_kompetisi_trial.KATEGORI_NAME,   
                                             tb_kompetisi_trial.LAMA_PERLOMBAAN,   
                                             tb_kompetisi_trial.KECEPATAN  
                                        FROM tb_kompetisi_trial   
                                    WHERE tb_kompetisi_trial.ROW_ID_KOMPETISI =@pid";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }

        public DataTable GetJawabanTrialView(string pidhdr, string prowid, string pid)
        {
            DataTable dt = new DataTable();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"  SELECT tb_jawaban_kompetisi_trial.ROW_ID_HDR, 
                                                tb_jawaban_kompetisi_trial.ROW_ID_KOMPETISI,   
                                                tb_jawaban_kompetisi_trial.ID_PESERTA,   
                                                tb_jawaban_kompetisi_trial.SOAL_NO,   
                                                tb_jawaban_kompetisi_trial.PERTANYAAN,   
                                                tb_jawaban_kompetisi_trial.JAWABAN_PESERTA,   
                                                tb_jawaban_kompetisi_trial.JAWAB_DETIK_BERAPA,   
                                                tb_jawaban_kompetisi_trial.JAWAB_DATE,   
                                                tb_jawaban_kompetisi_trial.KUNCI_JAWABAN,   
                                                tb_jawaban_kompetisi_trial.SCORE_PESERTA,   
                                                tb_jawaban_kompetisi_trial.IS_KIRIM  
                                           FROM tb_jawaban_kompetisi_trial  
                                          WHERE tb_jawaban_kompetisi_trial.ROW_ID_HDR =@pidhdr
                                            AND tb_jawaban_kompetisi_trial.ROW_ID_KOMPETISI =@prowid 
                                            AND tb_jawaban_kompetisi_trial.ID_PESERTA =@pid ";

            SQLiteParameter parm = new SQLiteParameter();

            parm = SqlParam("@pidhdr", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pidhdr);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@prowid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(prowid);
            sqlite_cmd.Parameters.Add(parm);
            parm = SqlParam("@pid", DbType.String, ParameterDirection.Input);
            parm.Value = Encryptor.Encrypt(pid);
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            return dt;
        }
    }
}
