using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MerryTest.Froms
{
    public partial class ProgramInformation : Form
    {
        Dictionary<string, object> Config = new Dictionary<string, object>();
        public ProgramInformation(Dictionary<string, object> Config)
        {
            InitializeComponent();
            this.Config = Config;
            X = this.Width;
            Y = this.Height;
        }
        int X;
        int Y;
        private void ProgramInformation_Load(object sender, EventArgs e)
        {
            this.lv_ProgramInformation.Items.Clear();
            this.pb_TypeNameImage.Visible = true;
            Dictionary<string, string> PartNumberInfos = (Dictionary<string, string>)Config["PartNumberInfos"];

            if ((bool)Config["LogiLogFlag"])
            {
                lv_ProgramInformation.Items.Add(new ListViewItem(new string[] { "LogiBU", (string)Config["LogiBU"] }));
                lv_ProgramInformation.Items.Add(new ListViewItem(new string[] { "LogiProject", (string)Config["LogiProject"] }));
                lv_ProgramInformation.Items.Add(new ListViewItem(new string[] { "LogiStation", (string)Config["LogiStation"] }));
                lv_ProgramInformation.Items.Add(new ListViewItem(new string[] { "LogioemSource", (string)Config["LogioemSource"] }));
                lv_ProgramInformation.Items.Add(new ListViewItem(new string[] { "LogiStage", (string)Config["LogiStage"] }));
            }
            this.pb_TypeNameImage.Visible = PartNumberInfos.ContainsKey("ImagePath");
            if (PartNumberInfos.ContainsKey("ImagePath"))
            {
                this.pb_TypeNameImage.ImageLocation = $@"{Config["adminPath"]}\TestItem\{Config["Name"]}\Image\{PartNumberInfos["ImagePath"]}";
                PartNumberInfos.Remove("ImagePath");
            }
            var dic = PartNumberInfos.OrderBy(x => x.Key.StartsWith("-")).ToArray();
            foreach (var item in dic)
            {
                lv_ProgramInformation.Items.Add(new ListViewItem(new string[] { item.Key, item.Value })).BackColor =
                     item.Key.Contains('-') ?
                     System.Drawing.SystemColors.ActiveBorder :
                     System.Drawing.SystemColors.Window;
            }

            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Location = (Point)new Size(5, 5);         //窗体的起始位置为(x,y)
        }

        static ProgramInformation forms;
        public static ProgramInformation ShowPicture(Dictionary<string, object> Config)
        {
            forms?.Dispose();
            Thread.Sleep(100);
            forms = new ProgramInformation(Config);
            return forms;
        }
        public void ShowEvent()
        {
            if (this.lv_ProgramInformation.Items.Count <= 0) this.Dispose();
        }
    }
}
