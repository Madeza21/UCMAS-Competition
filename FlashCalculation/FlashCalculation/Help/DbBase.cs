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

        public void Query(string query)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = query;

            sqlite_cmd.ExecuteNonQuery();
        }
    }
}
