using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace MerryTest.SingleTestAPI.SingleTestAddTool
{
    public partial class AddToolForms : Form
    {
        static AddToolForms form1;
        static string ConfigPath;
        static string MessagePath;
        bool load = false;
        public static AddToolForms GetForm1(string configPath, string messagePath)
        {

            ConfigPath = configPath;
            MessagePath = messagePath;
            if (form1 == null || form1.IsDisposed)
            {
                form1 = new AddToolForms();

            }
            form1.load = false;

            return form1;
        }
        public AddToolForms()
        {
            InitializeComponent();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
        private  void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = File.ReadAllText(ConfigPath, Encoding.UTF8);

            load = true;
        }
        private void txt_LostFocus(object sender, EventArgs e)
        {

            if (!load) return;
            File.WriteAllText(ConfigPath, textBox1.Text);

        }
    }
}
