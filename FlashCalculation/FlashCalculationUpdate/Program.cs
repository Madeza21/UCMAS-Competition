using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCalculationUpdate
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
            //Application.Run(new FrmUpdate());

            //Process Update https://github.com/jrz-soft-mx/MD5-Update
            //string strUrl = "http://update.ucmasidn.com/";
            if (MD5Update.Check(Properties.Settings.Default.url, true))
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"updt.exe", AppDomain.CurrentDomain.FriendlyName + " " + Process.GetCurrentProcess().ProcessName);
            }
            else
            {
                Application.Exit();
            }

            Process.Start("FlashCalculation.exe");
            Application.Exit();
        }
    }
}
