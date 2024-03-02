using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.Froms
{
    public partial class ProgressBars : Form
    {
        Dictionary<string, object> Config;
        public ProgressBars(Dictionary<string, object> keys)
        {
            InitializeComponent();
            Thread.Sleep(100);
            Control.CheckForIllegalCrossThreadCalls = false;
            Config = keys;
            Config["LoadMessageboxFlag"] = false;
        }
        private void ProgressBars_Load(object sender, EventArgs e)
        {
            i = 0;
            timer1.Enabled = true;
        }
        int i;
        private void timer1_Tick(object sender, EventArgs e)
        {
            i += 1;
            progressBar1.Value = i;
            if (i >= 499) i = 0;
            label1.Text = Config["LoadMessagebox"].ToString();
            if ((bool)Config["LoadMessageboxFlag"])
            {
                this.Close();
                this.Dispose();
            }
        }


    }
}
