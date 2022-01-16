using ClosedXML.Excel;
using FlashCalculation.Help;
using FlashCalculation.Model;
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

namespace FlashCalculation.View
{
    public partial class FrmResult : Form
    {
        DbBase db = new DbBase();
        DataTable dthdr = new DataTable();
        DataTable dtdtl = new DataTable();

        HttpRequest client = new HttpRequest();

        string rowid;

        public FrmResult(string prowid)
        {
            InitializeComponent();
            rowid = prowid;
        }

        private void FrmResult_Load(object sender, EventArgs e)
        {
            db.OpenConnection();

            dthdr = Helper.DecryptDataTable(db.GetKompetisiView(rowid));
            dthdr.AcceptChanges();

            SetHeader();

            dtdtl = Helper.DecryptDataTable(db.GetJawabanView(rowid, Properties.Settings.Default.siswa_id));
            dtdtl.AcceptChanges();
            dtdtl.Columns.Add("Int32_SOAL_NO", typeof(int), "SOAL_NO");
            dtdtl.DefaultView.Sort = "ROW_ID_KOMPETISI ASC, Int32_SOAL_NO ASC";
            dtdtl = dtdtl.DefaultView.ToTable();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dtdtl;

            ChangeColor();
            Translate();

            label8.Text = Total(dtdtl).ToString();          

            if (Properties.Settings.Default.trial == "Y")
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
                label4.Text = "Kategori :";

                //Result
                Rest_1.HeaderText = "#";
                Rest_2.HeaderText = "Pertanyaan";
                Rest_3.HeaderText = "Kunci Jawaban";
                Rest_4.HeaderText = "Jawaban Peserta";
                Rest_5.HeaderText = "Waktu Jawab (Detik)";
                Rest_6.HeaderText = "Tanggal Jawab";
                Rest_7.HeaderText = "Skor";
                Rest_8.HeaderText = "Terkirim";

                label7.Text = "Skor";
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
                label4.Text = "Category :";

                //Result
                Rest_1.HeaderText = "#";
                Rest_2.HeaderText = "Question";
                Rest_3.HeaderText = "Answer Key";
                Rest_4.HeaderText = "Participant Answer";
                Rest_5.HeaderText = "Answer Time (Second)";
                Rest_6.HeaderText = "Answer Date";
                Rest_7.HeaderText = "Score";
                Rest_8.HeaderText = "Send";

                label7.Text = "Score";
            }
        }

        private void FrmResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(dtdtl.Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;

                if (client.IsConnectedToInternet())
                {
                    for (int i = 0; i < dtdtl.Rows.Count; i++)
                    {
                        if (dtdtl.Rows[i]["is_kirim"].ToString() != "Y")
                        {
                            string msg = client.PostKirimJawaban("api/kompetisi/input", dtdtl.Rows[i]);
                            if (msg == "Berhasil input jawaban")
                            {
                                string flag = Encryptor.Encrypt("Y");

                                db.Query("Update tb_jawaban_kompetisi set is_kirim = '" + flag + "' where ROW_ID_KOMPETISI = '" +
                                    Encryptor.Encrypt(dtdtl.Rows[i]["ROW_ID_KOMPETISI"].ToString()) + "' AND ID_PESERTA = '" +
                                    Encryptor.Encrypt(dtdtl.Rows[i]["ID_PESERTA"].ToString()) + "' AND SOAL_NO ='" + Encryptor.Encrypt(dtdtl.Rows[i]["SOAL_NO"].ToString()) + "'");

                                dtdtl.Rows[i]["is_kirim"] = "Y";
                            }
                        }
                    }
                }

                //Cek data di db server
                if (client.IsConnectedToInternet())
                {
                    JawabanKompetisi[] jawaban = client.PostGetJawaban("api/kompetisi/jawaban", dtdtl.Rows[0]["ROW_ID_KOMPETISI"].ToString());
                    if (jawaban.Length > 0)
                    {
                        if (jawaban.Length != dtdtl.Rows.Count)
                        {
                            //update is_kirim db local jadi N
                            for (int i = 0; i < dtdtl.Rows.Count; i++)
                            {
                                bool ada = false;
                                for (int j = 0; j < jawaban.Length; j++)
                                {
                                    if (dtdtl.Rows[i]["SOAL_NO"].ToString() == jawaban[j].SOAL_NO.ToString())
                                    {
                                        ada = true;
                                        break;
                                    }
                                    else
                                    {
                                        ada = false;
                                    }
                                }
                                if (ada == false)
                                {
                                    string flag = Encryptor.Encrypt("N");

                                    db.Query("Update tb_jawaban_kompetisi set is_kirim = 'N' where ROW_ID_KOMPETISI = '" +
                                    Encryptor.Encrypt(dtdtl.Rows[i]["ROW_ID_KOMPETISI"].ToString()) + "' AND ID_PESERTA = '" +
                                    Encryptor.Encrypt(dtdtl.Rows[i]["ID_PESERTA"].ToString()) + "' AND SOAL_NO ='" + Encryptor.Encrypt(dtdtl.Rows[i]["SOAL_NO"].ToString()) + "'");

                                    dtdtl.Rows[i]["is_kirim"] = "N";
                                }
                            }
                        }
                    }
                }                    

                dataGridView1.DataSource = dtdtl;

                this.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            sfd.FileName = textBox2.Text + "_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (!(dtdtl.Rows.Count == 0))
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dtdtl, "Sheet1");
                        wb.SaveAs(sfd.FileName);
                    }
                }
            }
        }

        private decimal Total(DataTable dt)
        {
            if(dt.Rows.Count > 0)
            {
                decimal total = 0;
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    total += dt.Rows[i]["SCORE_PESERTA"].ToString() == "" ? 0 : Convert.ToDecimal(dt.Rows[i]["SCORE_PESERTA"].ToString());
                }
                return total;
            }
            else
            {
                return 0;
            }
        }
    }
}
