using FlashCalculation.Help;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation.View
{
    public partial class FrmParameter : Form
    {
        DbBase db = new DbBase();
        DataTable dthdr = new DataTable();
        DataTable dtdtl = new DataTable();

        string rowid;

        public FrmParameter(string prowid)
        {
            InitializeComponent();

            rowid = prowid;
        }

        private void FrmParameter_Load(object sender, EventArgs e)
        {
            db.OpenConnection();

            dthdr = Helper.DecryptDataTable(db.GetKompetisiView(rowid));
            dthdr.AcceptChanges();

            SetHeader();

            dtdtl = Helper.DecryptDataTable(db.GetParameterKompetisiView(rowid));
            dtdtl.AcceptChanges();
            dtdtl.Columns.Add("Int32_SOAL_DARI", typeof(int), "SOAL_DARI");

            dtdtl.DefaultView.Sort = "Int32_SOAL_DARI ASC";
            dtdtl = dtdtl.DefaultView.ToTable();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView3.AutoGenerateColumns = false;

            dataGridView1.DataSource = dtdtl;
            dataGridView2.DataSource = dtdtl;
            dataGridView3.DataSource = dtdtl;

            ChangeColor();
            Translate();
        }

        private void SetHeader()
        {
            if(dthdr.Rows.Count > 0)
            {
                textBox1.Text = dthdr.Rows[0]["CABANG_NAME"].ToString();
                textBox2.Text = dthdr.Rows[0]["KOMPETISI_NAME"].ToString();
                textBox3.Text = dthdr.Rows[0]["TANGGAL_KOMPETISI"].ToString();
                textBox4.Text = dthdr.Rows[0]["JAM_MULAI"].ToString().Length > 0 ? dthdr.Rows[0]["JAM_MULAI"].ToString().Substring(0,2) + ":" + dthdr.Rows[0]["JAM_MULAI"].ToString().Substring(2, 2) + ":" + dthdr.Rows[0]["JAM_MULAI"].ToString().Substring(4, 2) : "";
                textBox9.Text = dthdr.Rows[0]["JAM_SAMPAI"].ToString().Length > 0 ? dthdr.Rows[0]["JAM_SAMPAI"].ToString().Substring(0, 2) + ":" + dthdr.Rows[0]["JAM_SAMPAI"].ToString().Substring(2, 2) + ":" + dthdr.Rows[0]["JAM_SAMPAI"].ToString().Substring(4, 2) : "";

                textBox5.Text = dthdr.Rows[0]["TIPE"].ToString() == "F" ? "FLASH" : dthdr.Rows[0]["TIPE"].ToString() == "V" ? "VISUAL" : "LISTENING";
                textBox6.Text = dthdr.Rows[0]["JENIS_NAME"].ToString();
                textBox7.Text = dthdr.Rows[0]["KATEGORI_NAME"].ToString();
                textBox8.Text = dthdr.Rows[0]["LAMA_PERLOMBAAN"].ToString();
                textBox10.Text = dthdr.Rows[0]["KECEPATAN"].ToString();
            }            
        }

        private void ChangeColor()
        {
            Flash_5.HeaderCell.Style.BackColor = Color.Maroon;
            Flash_6.HeaderCell.Style.BackColor = Color.Teal;

            Visual_7.HeaderCell.Style.BackColor = Color.Maroon;
            Visual_8.HeaderCell.Style.BackColor = Color.Teal;
            Visual_9.HeaderCell.Style.BackColor = Color.Teal;
            Visual_16.HeaderCell.Style.BackColor = Color.Teal;
            Visual_10.HeaderCell.Style.BackColor = Color.Purple;
            Visual_11.HeaderCell.Style.BackColor = Color.Purple;
            Visual_12.HeaderCell.Style.BackColor = Color.Olive;
            Visual_13.HeaderCell.Style.BackColor = Color.Olive;

            Listen_4.HeaderCell.Style.BackColor = Color.Maroon;
            Listen_5.HeaderCell.Style.BackColor = Color.Teal;

            tabControl1.TabPages.Remove(tabPage1);
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);

            if (dthdr.Rows.Count > 0)
            {
                if(dthdr.Rows[0]["TIPE"].ToString() == "F")
                {
                    tabControl1.TabPages.Add(tabPage1);
                    //tabControl1.TabPages.Remove(tabPage2);
                    //tabControl1.TabPages.Remove(tabPage3);
                }
                else if (dthdr.Rows[0]["TIPE"].ToString() == "V")
                {
                    //tabControl1.TabPages.Remove(tabPage1);
                    tabControl1.TabPages.Add(tabPage2);
                    //tabControl1.TabPages.Remove(tabPage3);
                }
                else
                {
                    //tabControl1.TabPages.Remove(tabPage1);
                    //tabControl1.TabPages.Remove(tabPage2);
                    tabControl1.TabPages.Add(tabPage3);
                }
            }
        }

        private void FrmParameter_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Translate()
        {
            if (Properties.Settings.Default.bahasa == "indonesia")
            {
                this.Text = "Parameter Perlombaan";

                //header
                label1.Text = "Cabang :";
                label2.Text = "Nama Kompetisi :";
                label3.Text = "Tanggal :";
                label4.Text = "Jam Mulai :";
                label5.Text = "Tipe :";
                label6.Text = "Jenis Kompetisi :";
                label7.Text = "Kategori :";
                label8.Text = "Waktu Kompetisi :";
                //label9.Text = "";
                label10.Text = "Detik";
                label11.Text = "Kecepatan :";
                label12.Text = "Detik";

                //Visual
                Visual_1.HeaderText = "Soal Dari";
                Visual_2.HeaderText = "Soal Sampai";
                Visual_3.HeaderText = "Panjang Digit";
                Visual_4.HeaderText = "Maksimum Panjang Digit";
                Visual_5.HeaderText = "Jumlah Baris Per Soal";
                Visual_6.HeaderText = "Maksimum Jumlah Digit Per Soal";
                Visual_7.HeaderText = "Muncul Angka Minus";
                Visual_8.HeaderText = "Muncul Angka Perkalian";
                Visual_9.HeaderText = "Digit Perkalian";
                Visual_10.HeaderText = "Muncul Angka Pembagian";
                Visual_11.HeaderText = "Digit Pembagian";
                Visual_12.HeaderText = "Muncul Angka Decimal";
                Visual_13.HeaderText = "Digit Decimal";
                Visual_14.HeaderText = "Jumlah Muncul";
                Visual_15.HeaderText = "Jumlah Baris Per Muncul";
                Visual_16.HeaderText = "Kecepatan (Detik)";

                //Flash
                Flash_1.HeaderText = "Soal Dari";
                Flash_2.HeaderText = "Soal Sampai";
                Flash_3.HeaderText = "Panjang Digit";
                Flash_4.HeaderText = "Jumlah Muncul";
                Flash_5.HeaderText = "Muncul Angka Minus";
                Flash_6.HeaderText = "Kecepatan (Detik)";

                //Listening
                Listen_1.HeaderText = "Soal Dari";
                Listen_2.HeaderText = "Soal Sampai";
                Listen_3.HeaderText = "Panjang Digit";
                Listen_4.HeaderText = "Muncul Angka Minus";
                Listen_5.HeaderText = "Kecepatan";
            }
            else
            {
                this.Text = "Competition Parameters";

                //header
                label1.Text = "Branch :";
                label2.Text = "Competition Name :";
                label3.Text = "Date :";
                label4.Text = "Start Time :";
                label5.Text = "Type :";
                label6.Text = "Competition Type :";
                label7.Text = "Category :";
                label8.Text = "Competition Time :";
                //label9.Text = "";
                label10.Text = "Second";
                label11.Text = "Speed :";
                label12.Text = "Second";

                //Visual
                Visual_1.HeaderText = "Question From";
                Visual_2.HeaderText = "Question To";
                Visual_3.HeaderText = "Digit Length";
                Visual_4.HeaderText = "Maximum Digit Length";
                Visual_5.HeaderText = "Number Of Rows Per Question";
                Visual_6.HeaderText = "Maximum Digit Length Per Question";
                Visual_7.HeaderText = "Subtraction";
                Visual_8.HeaderText = "Multiplication";
                Visual_9.HeaderText = "Multiply Digits";
                Visual_10.HeaderText = "Division";
                Visual_11.HeaderText = "Division Digits";
                Visual_12.HeaderText = "Decimal";
                Visual_13.HeaderText = "Decimal Digits";
                Visual_14.HeaderText = "Number Of Display";
                Visual_15.HeaderText = "Number Of Rows Per Display";
                Visual_16.HeaderText = "Speed (Second)";

                //Flash
                Flash_1.HeaderText = "Question From";
                Flash_2.HeaderText = "Question To";
                Flash_3.HeaderText = "Digit Length";
                Flash_4.HeaderText = "Number Of Display";
                Flash_5.HeaderText = "Subtraction";
                Flash_6.HeaderText = "Speed (Second)";

                //Listening
                Listen_1.HeaderText = "Question From";
                Listen_2.HeaderText = "Question To";
                Listen_3.HeaderText = "Digit Length";
                Listen_4.HeaderText = "Subtraction";
                Listen_5.HeaderText = "Speed";
            }
        }
    }
}
