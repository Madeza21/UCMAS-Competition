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
    public partial class FrmResult : Form
    {
        public FrmResult()
        {
            InitializeComponent();
        }

        private void FrmResult_Load(object sender, EventArgs e)
        {
            ChangeColor();
        }

        private void ChangeColor()
        {
            Rest_3.HeaderCell.Style.BackColor = Color.Maroon;
        }
    }
}
