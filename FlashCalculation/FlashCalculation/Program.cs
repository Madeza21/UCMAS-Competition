using FlashCalculation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());
            //FrmLogin fLogin = new FrmLogin(peserta);
            /*if (fLogin.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FrmMain(peserta));
            }
            else
            {
                Application.Exit();
            }*/
        }
    }
}
