using MerryTest.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.MoreTestAPI
{
    internal class Envent
    {
        public Envent(Dictionary<string, object> Config)
            => this.Config = Config;
        Dictionary<string, object> Config = new Dictionary<string, object>();
        object obj_lock = new object();
        Thread obj_thread;
        public void BugLogEnvent()
        {
            if (obj_thread != null)
            {
                if (obj_thread.IsAlive) obj_thread.Abort();
                obj_thread = null;

            }
            obj_thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (Config["BugLog"].ToString() != "False")
                        {
                            lock (obj_lock)
                            {
                                if (!Directory.Exists($@"{MoreProperty.Config["adminPath"]}\Log\")) Directory.CreateDirectory($@"{MoreProperty.Config["adminPath"]}\Log\");
                                File.AppendAllText($@"{MoreProperty.Config["adminPath"]}\Log\错误信息{DateTime.Now:MM_dd}.txt", $"{DateTime.Now}\r\n{Config["BugLog"]}\r\n", Encoding.UTF8);
                                Config["BugLog"] = "False";
                                Thread.Sleep(50);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                    WindowsAPI.MessageBoxShow_MoreTest($"记录报错信息方法报错了，拍照将信息发给TE\r\n{ex}");
                }
            });
            obj_thread.Start();

        }

    }
}
