using System;
using System.Data;

namespace FlashCalculation.Help
{
    internal class Helper
    {
        public static DataTable DecryptDataTable(DataTable dt)
        {
            for(int i = 0; i< dt.Rows.Count; i++)
            {
                for(int j = 0; j < dt.Columns.Count; j++)
                {
                    dt.Rows[i][j] = Encryptor.Decrypt(dt.Rows[i][j].ToString());
                }
            }
            return dt;
        }

        public static DataTable EncryptDataTable(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dt.Rows[i][j] = Encryptor.Encrypt(dt.Rows[i][j].ToString());
                }
            }
            return dt;
        }

        public static DataTable EncryptDataTableSoal(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if(dt.Columns[j].ColumnName.ToUpper() == "NO_SOAL")
                    {
                        continue;
                    }
                    dt.Rows[i][j] = Encryptor.Encrypt(dt.Rows[i][j].ToString());
                }
            }
            return dt;
        }

        public static DataTable DecryptDataTableSoal(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].ColumnName.ToUpper() == "NO_SOAL")
                    {
                        continue;
                    }
                    dt.Rows[i][j] = Encryptor.Decrypt(dt.Rows[i][j].ToString());
                }
            }
            return dt;
        }
        
    }
}
