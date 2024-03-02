using MerryTest.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);


                System.Threading.Mutex mutex = new System.Threading.Mutex(false, "ThisShouldOnlyRunOnce");
                bool Running = !mutex.WaitOne(0, false);
                if (Running)
                {
                    MessageBox.Show("检测程序已经启动");
                    return;
                }

                if (args.Length == 2)
                {
                    IniHelpe.SetValue("Config", "NAME", args[0]);
                    IniHelpe.SetValue("Config", "Station", args[1]);
                }
                if (IniHelpe.GetValue("Flag", "MoreTestMode") == "1")
                {
                    Application.Run(new MoreTest());
                }
                else
                {
                    Application.Run(new SingleTest());
                }
            }
            catch (Exception EX)
            {

                MessageBox.Show($"程序即将闪退\r\n{EX}");
                Process.GetCurrentProcess().Kill();

            }


        }
    }
}
