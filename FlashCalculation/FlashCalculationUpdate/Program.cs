using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            //Register Speech
            RegisterSpeech.CopySpeechRegistryEntryFromOneCore();

            //Process Update https://github.com/jrz-soft-mx/MD5-Update
            //string strUrl = "http://update.ucmasidn.com/";
            if (MD5Update.Check(Properties.Settings.Default.url, true))
            {
                //
            }
            else
            {
                //Application.Exit();
                //return;
            }

            string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "FlashCalculation.exe");
            if (File.Exists(path))
            {
                Process.Start("FlashCalculation.exe");
            }
            
            Application.Exit();
        }
    }
}
