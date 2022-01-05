using FlashCalculation.Help;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace FlashCalculation.View
{
    public partial class FrmResult : Form
    {
        DbBase db = new DbBase();
        DataTable dthdr = new DataTable();
        DataTable dtdtl = new DataTable();

        string rowid;

        public FrmResult(string prowid)
        {
            InitializeComponent();
            rowid = prowid;
        }

        private void FrmResult_Load(object sender, EventArgs e)
        {
            db.OpenConnection();

            dthdr = db.GetKompetisiView(rowid);

            SetHeader();

            dtdtl = db.GetJawabanView(rowid, Properties.Settings.Default.siswa_id);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dtdtl;

            ChangeColor();
            Translate();

            if(Properties.Settings.Default.trial == "Y")
            {
                button2.Visible = false;
            }
            else
            {
                button2.Visible = true;
            }
        }

        private void SetHeader()
        {
            if (dthdr.Rows.Count > 0)
            {
                textBox1.Text = dthdr.Rows[0]["CABANG_NAME"].ToString();
                textBox2.Text = dthdr.Rows[0]["KOMPETISI_NAME"].ToString();
                textBox3.Text = dthdr.Rows[0]["TANGGAL_KOMPETISI"].ToString();
                textBox5.Text = dthdr.Rows[0]["TIPE"].ToString() == "F" ? "FLASH" : dthdr.Rows[0]["TIPE"].ToString() == "V" ? "VISUAL" : "LISTENING";
                textBox6.Text = dthdr.Rows[0]["JENIS_NAME"].ToString();
                textBox7.Text = dthdr.Rows[0]["KATEGORI_NAME"].ToString();
            }
        }

        private void ChangeColor()
        {
            Rest_3.HeaderCell.Style.BackColor = Color.Maroon;
        }

        private void Translate()
        {
            if (Properties.Settings.Default.bahasa == "indonesia")
            {
                this.Text = "Hasil Kompetisi";
                //button1.Text = "";
                button2.Text = "Kirim Jawaban";
                tabPage1.Text = "Jawaban";

                //header
                label1.Text = "Cabang :";
                label2.Text = "Nama Kompetisi :";
                label3.Text = "Tanggal :";
                label5.Text = "Tipe :";
                label6.Text = "Jenis Kompetisi :";
                label7.Text = "Kategori :";

                //Result
                Rest_1.HeaderText = "#";
                Rest_2.HeaderText = "Pertanyaan";
                Rest_3.HeaderText = "Kunci Jawaban";
                Rest_4.HeaderText = "Jawaban Peserta";
                Rest_5.HeaderText = "Waktu Jawab (Detik)";
                Rest_6.HeaderText = "Tanggal Jawab";
                Rest_7.HeaderText = "Skor";
                Rest_8.HeaderText = "Terkirim";
            }
            else
            {
                this.Text = "Competition Result";
                //button1.Text = "";
                button2.Text = "Send the answer";
                tabPage1.Text = "Answer";

                //header
                label1.Text = "Branch :";
                label2.Text = "Competition Name :";
                label3.Text = "Date :";
                label5.Text = "Type :";
                label6.Text = "Competition Type :";
                label7.Text = "Category :";

                //Result
                Rest_1.HeaderText = "#";
                Rest_2.HeaderText = "Question";
                Rest_3.HeaderText = "Answer Key";
                Rest_4.HeaderText = "Participant Answer";
                Rest_5.HeaderText = "Answer Time (Second)";
                Rest_6.HeaderText = "Answer Date";
                Rest_7.HeaderText = "Score";
                Rest_8.HeaderText = "Send";
            }
        }

        private void FrmResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xls)|*.xls";
            sfd.FileName = textBox2.Text + "_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Copy DataGridView results to clipboard
                copyAlltoClipboard();

                object misValue = System.Reflection.Missing.Value;
                Excel.Application xlexcel = new Excel.Application();

                xlexcel.DisplayAlerts = false; // Without this you will get two confirm overwrite prompts
                Excel.Workbook xlWorkBook = xlexcel.Workbooks.Add(misValue);
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // Format column D as text before pasting results, this was required for my data
                Excel.Range rng = xlWorkSheet.get_Range("D:D").Cells;
                rng.NumberFormat = "@";

                // Paste clipboard results to worksheet range
                Excel.Range CR = (Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

                // For some reason column A is always blank in the worksheet. ¯\_(ツ)_/¯
                // Delete blank column A and select cell A1
                Excel.Range delRng = xlWorkSheet.get_Range("A:A").Cells;
                delRng.Delete(Type.Missing);
                xlWorkSheet.get_Range("A1").Select();

                // Save the excel file under the captured location from the SaveFileDialog
                xlWorkBook.SaveAs(sfd.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlexcel.DisplayAlerts = true;
                xlWorkBook.Close(true, misValue, misValue);
                xlexcel.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlexcel);

                // Clear Clipboard and DataGridView selection
                Clipboard.Clear();
                dataGridView1.ClearSelection();

                // Open the newly saved excel file
                if (File.Exists(sfd.FileName))
                    System.Diagnostics.Process.Start(sfd.FileName);
            }
        }

        private void copyAlltoClipboard()
        {
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
