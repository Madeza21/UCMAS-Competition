using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation.Help
{
    internal class RandomTrans
    {
        static Random rnd = new Random();

        private static string RandomAngka(int pdigit)
        {
            string ret = "";
            ret = rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() +
            rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() +
            rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() +
            rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString();

            return ret.Substring(0, pdigit);
        }

        private static string RandomAngkaDec(int pdigit)
        {
            string ret = "";

            ret = rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() +
                rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() + rnd.Next(1, 10).ToString() +
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() +
                rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString() + rnd.Next(1, 11).ToString();

            return ret.Substring(0, pdigit);
        }

        private static string NumberTranslate(int amount)
        {
            string ret = "";
            string[] aUnit, aTen;
            int nHundred, nTen;

            aUnit = new string[19]{ "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };

            aTen = new string[9] { "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (amount > 100)
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

        private static string NumberInWordEnglish(decimal amount)
        {
            string str = "", str2;
            decimal nTrillion;//, nMillion, nThousand;

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
            if (amount >= 1000000000000.00M)
            {
                nTrillion = Math.Truncate(amount / 1000000000000);
                str += NumberTranslate(Convert.ToInt32(nTrillion)) + " trillion ";
                amount -= (nTrillion * 1000000000000);
            }
            if (amount >= 1000000000)
            {
                nTrillion = Math.Truncate(amount / 1000000000);
                str += NumberTranslate(Convert.ToInt32(nTrillion)) + " billion ";
                amount -= (nTrillion * 1000000000);
            }
            if (amount >= 1000000)
            {
                nTrillion = Math.Truncate(amount / 1000000);
                str += NumberTranslate(Convert.ToInt32(nTrillion)) + " million ";
                amount -= (nTrillion * 1000000);
            }
            if (amount >= 1000)
            {
                nTrillion = Math.Truncate(amount / 1000);
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

        private static string NumberInWordIndonesia(decimal ad_amount)
        {
            decimal ld_divisor, ld_large_amount, ld_tiny_amount, ld_dividen, ld_dummy;
            string ls_word;
            string ls_weight1, ls_unit, ls_follower;
            string[] ls_prefix = { "SE ", "DUA ", "TIGA ", "EMPAT ", "LIMA ", "ENAM ", "TUJUH ", "DELAPAN ", "SEMBILAN " };
            string[] ls_sufix = { "SATU ", "DUA ", "TIGA ", "EMPAT ", "LIMA ", "ENAM ", "TUJUH ", "DELAPAN ", "SEMBILAN " };

            ls_word = "";
            ld_large_amount = Math.Abs(Math.Truncate(ad_amount));
            ld_tiny_amount = Math.Round((Math.Abs(ad_amount) - ld_large_amount) * 100, 0);
            ld_divisor = 1000000000000.00M;

            if (ad_amount > 999999999999997) return "OUT OF RANGE";
            if (ad_amount < 0) return "OUT OF RANGE";

            do
            {
                ld_dividen = Math.Truncate(ld_large_amount / ld_divisor);
                ld_large_amount = ld_large_amount % ld_divisor;
                ls_unit = "";

                if (ld_dividen > 0)
                {
                    switch (ld_divisor)
                    {
                        case 1000000000000.00M:
                            ls_unit = "TRILYUN ";
                            break;
                        case 1000000000.00M:
                            ls_unit = "MILYAR ";
                            break;
                        case 1000000.00M:
                            ls_unit = "JUTA ";
                            break;
                        case 1000.00M:
                            ls_unit = "RIBU ";
                            break;
                    }
                }

                ls_weight1 = "";
                ld_dummy = ld_dividen;
                if (ld_dummy >= 100) ls_weight1 = ls_prefix[Convert.ToInt32(Math.Truncate(ld_dummy / 100)) - 1] + "RATUS ";

                ld_dummy = ld_dividen % 100;


                if (ld_dummy < 10)
                {
                    if (ld_dummy == 1 && ls_unit == "RIBU ")
                    {
                        ls_weight1 += "SE ";
                    }
                    else if (ld_dummy > 0)
                    {
                        ls_weight1 += ls_sufix[Convert.ToInt32(ld_dummy) - 1];
                    }
                }
                else if (ld_dummy >= 11 && ld_dummy <= 19)
                {
                    ls_weight1 += ls_prefix[Convert.ToInt32(ld_dummy % 10) - 1] + "BELAS ";
                }
                else
                {
                    ls_weight1 += ls_prefix[Convert.ToInt32(Math.Truncate(ld_dummy / 10)) - 1] + "PULUH ";
                    if ((ld_dummy % 10) > 0) ls_weight1 += ls_sufix[Convert.ToInt32(ld_dummy % 10) - 1];
                }

                ls_word += ls_weight1 + ls_unit;
                ld_divisor /= 1000.00M;

            } while (ld_divisor >= 1);

            if (Math.Truncate(ad_amount) == 0) ls_word = "NOL ";
            ls_follower = "";

            if (ld_tiny_amount < 10)
            {
                if (ld_tiny_amount > 0) ls_follower = "KOMA NOL " + ls_sufix[Convert.ToInt32(ld_tiny_amount) - 1];
            }
            else
            {
                ls_follower = "KOMA " + ls_sufix[Convert.ToInt32(Math.Truncate(ld_tiny_amount / 10)) - 1];
                if ((ld_tiny_amount % 10) > 0) ls_follower += ls_sufix[Convert.ToInt32(ld_tiny_amount - 10) - 1];
            }

            //ls_word += 'Rupiah ' + ls_follower  --> tulisan rupiah
            ls_word += ls_follower;

            if (ad_amount < 0) ls_word = "MINUS " + ls_word;

            return ls_word.ToLower().Trim();
        }

        public static DataTable RandomFlash(int soaldari, int soalsampai, int pjgdigit, int jmlrow, 
            string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, 
            decimal kecepatan, DataTable dtSoal, CultureInfo culture)
        {
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
                            dr["angkamuncul" + angkamunculke.ToString()] = strangka.TrimEnd(Environment.NewLine.ToCharArray());
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
                throw new Exception("Error : " + e.Message + " On Soal Dari : " + soaldari.ToString());
            }

            return dtSoal;
        }

        public static DataTable RandomListening(int soaldari, int soalsampai, int pjgdigit, int jmlrow, 
            string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, 
            decimal kecepatan, DataTable dtSoal, CultureInfo culture)
        {
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
                            dr["angkamuncul" + angkamunculke.ToString()] = strangka.TrimEnd(Environment.NewLine.ToCharArray());
                            dr["angkalistening" + angkamunculke.ToString()] = strangkalisten.TrimEnd(Environment.NewLine.ToCharArray());
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
                throw new Exception("Error : " + e.Message + " On Soal Dari : " + soaldari.ToString());
            }

            return dtSoal;
        }

        public static DataTable RandomListeningIndonesia(int soaldari, int soalsampai, int pjgdigit, int jmlrow, 
            string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus, 
            decimal kecepatan, DataTable dtSoal, CultureInfo culture)
        {
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
                                strangka = strangka + "kurang " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(-" + strrandom + ") " + "kurang " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;

                                bminus = true;

                                strrandom = '-' + strrandom;
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                            }
                            else
                            {
                                if (bminus)
                                {
                                    strangka = strangka + "tambah " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                    strangkalisten = strangkalisten + "(" + strrandom + ") " + "tambah " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                }
                                else
                                {
                                    strangka = strangka + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                    strangkalisten = strangkalisten + "(" + strrandom + ") " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                }

                                bminus = false;
                                dr["angka" + row.ToString()] = Convert.ToDecimal(strrandom, culture);
                            }
                        }
                        else
                        {
                            if (bminus)
                            {
                                strangka = strangka + "tambah " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(" + strrandom + ") " + "tambah " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                            }
                            else
                            {
                                strangka = strangka + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
                                strangkalisten = strangkalisten + "(" + strrandom + ") " + NumberInWordIndonesia(Convert.ToDecimal(strrandom, culture)) + Environment.NewLine;
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
                            dr["angkamuncul" + angkamunculke.ToString()] = strangka.TrimEnd(Environment.NewLine.ToCharArray());
                            dr["angkalistening" + angkamunculke.ToString()] = strangkalisten.TrimEnd(Environment.NewLine.ToCharArray());
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
                throw new Exception("Error : " + e.Message + " On Soal Dari : " + soaldari.ToString());
            }

            return dtSoal;
        }

        public static DataTable RandomVisual(int soaldari, int soalsampai, int pjgdigit, int jmlrow, 
            string idperlombaan, int jmlbarispermuncul, int jmlmuncul, string munculminus,
            string munculperkalian, int digitperkalian, string munculpembagian, int digitpembagian, 
            string munculdec, int digitdec, decimal kecepatan, int maxdigitsoal,
            DataTable dtSoal, CultureInfo culture)
        {
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
                                    dr["angkamuncul" + angkamunculke.ToString()] = strangka.TrimEnd(Environment.NewLine.ToCharArray());
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
                                    dr["angkamuncul" + angkamunculke.ToString()] = strangka.TrimEnd(Environment.NewLine.ToCharArray());
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
                                dr["angkamuncul" + angkamunculke.ToString()] = strangka.TrimEnd(Environment.NewLine.ToCharArray());
                                strangka = "";
                            }
                        }
                    }

                    string filter = "";

                    if (munculpembagian == "Y")
                    {
                        filter = "Bagi";
                    }
                    else if (munculperkalian == "Y")
                    {
                        filter = "Kali";
                    }
                    else
                    {
                        if (filter == "")
                        {
                            filter = "TambahKurang";
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
                    dr["filter"] = filter;

                    dtSoal.Rows.Add(dr);
                }

                strrandomprev = "";
                strrandom = "";
            }
            catch (Exception e)
            {
                throw new Exception("Error : " + e.Message + " On Soal Dari : " + soaldari.ToString());
            }

            return dtSoal;
        }

    }
}
