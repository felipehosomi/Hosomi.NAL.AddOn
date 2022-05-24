using Hosomi.NAL.AddOn.Core.BLL;
using SBO.Hub;
using SBO.Hub.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hosomi.NAL.Addon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.Exit();
                return;
            }

            SBOApp sboApp = new SBOApp(args[0], $"{Application.StartupPath}\\Hosomi.NAL.AddOn.Core.dll");

            sboApp.InitializeApplication();
            SBOApp.AutoTranslateHana = true;

            InitializeBLL.Initialize();

            var oListener = new Listener();
            var oThread = new Thread(oListener.startListener) { IsBackground = true };
            oThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }
    }
}
