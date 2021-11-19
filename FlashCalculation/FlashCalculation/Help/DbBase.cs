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
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        public DataTable GetCabang(string pcode)
        {
            DataTable dt = new DataTable();

            sqlite_conn = CreateConnection();

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM tb_cabang where CABANG_CODE=@pcode";

            SQLiteParameter parm = new SQLiteParameter();
            parm.ParameterName = "@pcode";
            parm.DbType = DbType.String;
            parm.Direction = ParameterDirection.Input;
            parm.Value = pcode;
            sqlite_cmd.Parameters.Add(parm);

            SQLiteDataAdapter dda = new SQLiteDataAdapter(sqlite_cmd);

            dda.Fill(dt);

            sqlite_conn.Close();

            return dt;
        }

    }
}
