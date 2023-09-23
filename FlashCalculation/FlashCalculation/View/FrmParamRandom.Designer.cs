namespace FlashCalculation.View
{
    partial class FrmParamRandom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmParamRandom));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.SOAL_DARI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SOAL_SAMPAI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PANJANG_DIGIT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JUMLAH_MUNCUL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MUNCUL_ANGKA_MINUS = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.KECEPATAN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(103)))), ((int)(((byte)(214)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1110, 10);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(175)))), ((int)(((byte)(192)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1110, 135);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 722);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1110, 5);
            this.panel3.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 145);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1110, 577);
            this.panel4.TabIndex = 3;
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeight = 29;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SOAL_DARI,
            this.SOAL_SAMPAI,
            this.PANJANG_DIGIT,
            this.JUMLAH_MUNCUL,
            this.MUNCUL_ANGKA_MINUS,
            this.KECEPATAN});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1110, 577);
            this.dataGridView1.TabIndex = 0;
            // 
            // SOAL_DARI
            // 
            this.SOAL_DARI.HeaderText = "Soal Dari";
            this.SOAL_DARI.MinimumWidth = 6;
            this.SOAL_DARI.Name = "SOAL_DARI";
            this.SOAL_DARI.Width = 125;
            // 
            // SOAL_SAMPAI
            // 
            this.SOAL_SAMPAI.HeaderText = "Soal Sampai";
            this.SOAL_SAMPAI.MinimumWidth = 6;
            this.SOAL_SAMPAI.Name = "SOAL_SAMPAI";
            this.SOAL_SAMPAI.Width = 125;
            // 
            // PANJANG_DIGIT
            // 
            this.PANJANG_DIGIT.HeaderText = "Panjang Digit";
            this.PANJANG_DIGIT.MinimumWidth = 6;
            this.PANJANG_DIGIT.Name = "PANJANG_DIGIT";
            this.PANJANG_DIGIT.Width = 125;
            // 
            // JUMLAH_MUNCUL
            // 
            this.JUMLAH_MUNCUL.HeaderText = "Jumlah Muncul";
            this.JUMLAH_MUNCUL.MinimumWidth = 6;
            this.JUMLAH_MUNCUL.Name = "JUMLAH_MUNCUL";
            this.JUMLAH_MUNCUL.Width = 125;
            // 
            // MUNCUL_ANGKA_MINUS
            // 
            this.MUNCUL_ANGKA_MINUS.HeaderText = "Muncul Angka Minus";
            this.MUNCUL_ANGKA_MINUS.MinimumWidth = 6;
            this.MUNCUL_ANGKA_MINUS.Name = "MUNCUL_ANGKA_MINUS";
            this.MUNCUL_ANGKA_MINUS.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MUNCUL_ANGKA_MINUS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.MUNCUL_ANGKA_MINUS.Width = 160;
            // 
            // KECEPATAN
            // 
            this.KECEPATAN.HeaderText = "Kecepatan";
            this.KECEPATAN.MinimumWidth = 6;
            this.KECEPATAN.Name = "KECEPATAN";
            this.KECEPATAN.Width = 125;
            // 
            // FrmParamRandom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1110, 727);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmParamRandom";
            this.Text = "Peraturan Random";
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SOAL_DARI;
        private System.Windows.Forms.DataGridViewTextBoxColumn SOAL_SAMPAI;
        private System.Windows.Forms.DataGridViewTextBoxColumn PANJANG_DIGIT;
        private System.Windows.Forms.DataGridViewTextBoxColumn JUMLAH_MUNCUL;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MUNCUL_ANGKA_MINUS;
        private System.Windows.Forms.DataGridViewTextBoxColumn KECEPATAN;
    }
}