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

        public FrmParameter()
        {
            InitializeComponent();
        }

        private void FrmParameter_Load(object sender, EventArgs e)
        {
            db.OpenConnection();

            dthdr = db.GetKompetisiView("567b3dca-80ec-4f51-b5e6-15fca7e523d8");

            SetHeader();

            dtdtl = db.GetParameterKompetisiView("567b3dca-80ec-4f51-b5e6-15fca7e523d8");            

            dataGridView1.AutoGenerateColumns = false;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView3.AutoGenerateColumns = false;

            dataGridView1.DataSource = dtdtl;
            dataGridView2.DataSource = dtdtl;
            dataGridView3.DataSource = dtdtl;

            ChangeColor();
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
    }
}
