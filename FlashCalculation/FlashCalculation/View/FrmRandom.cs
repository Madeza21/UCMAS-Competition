using FlashCalculation.Model;
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
    public partial class FrmRandom : Form
    {
        Peserta peserta;
        public FrmRandom(Peserta obj)
        {
            InitializeComponent();
            peserta = obj;
        }

        private void FrmRandom_Load(object sender, EventArgs e)
        {
            pnlKiri.Width = this.Width / 2;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text == "Luas")
            {
                pnl1.Width = (pnlKiri.Width / 1) - 40;
                pnl1.Height = (pnlKiri.Height / 1) - 40 - txtKiri.Height;
                pnl1.Left = 20;
                pnl1.Top = 20;

                pnl2.Width = (pnlKanan.Width / 1) - 40;
                pnl2.Height = (pnlKanan.Height / 1) - 40 - txtKanan.Height;
                pnl2.Left = 20;
                pnl2.Top = 20;
            }
            else if(comboBox1.Text == "Sedang")
            {
                // Set pnl1 menjadi center kanannya pnlKiri
                pnl1.Width = pnlKiri.Width / 2;
                pnl1.Height = pnlKiri.Height / 2;
                pnl1.Left = (pnlKiri.Width - pnl1.Width) - 20;
                pnl1.Top = (pnlKiri.Height - pnl1.Height) / 2;

                // Set pnl2 menjadi center kirinya pnlKanan
                pnl2.Width = pnlKanan.Width / 2;
                pnl2.Height = pnlKanan.Height / 2;
                pnl2.Left = 20;
                pnl2.Top = (pnlKanan.Height - pnl2.Height) / 2;
            }
            else
            {
                // Set pnl1 menjadi center kanannya pnlKiri
                pnl1.Width = pnlKiri.Width / 3;
                pnl1.Height = pnlKiri.Height / 3;
                pnl1.Left = (pnlKiri.Width - pnl1.Width) - 20;
                pnl1.Top = (pnlKiri.Height - pnl1.Height) / 2;

                // Set pnl2 menjadi center kirinya pnlKanan
                pnl2.Width = pnlKanan.Width / 3;
                pnl2.Height = pnlKanan.Height / 3;
                pnl2.Left = 20;
                pnl2.Top = (pnlKanan.Height - pnl2.Height) / 2;
            }
        }
    }
}
