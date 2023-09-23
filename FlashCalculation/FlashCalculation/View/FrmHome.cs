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
    public partial class FrmHome : Form
    {
        DbBase db = new DbBase();
        string nama;
        public FrmHome(string peserta)
        {
            InitializeComponent();
            this.nama = peserta;
        }

        private void FrmHome_Load(object sender, EventArgs e)
        {
            db.OpenConnection();

            label1.Text = @"""Welcome " + nama + @"""" + Environment.NewLine + db.GetAppConfig("LBK");
        }
    }
}
