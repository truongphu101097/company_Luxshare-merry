using MerryTest.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.Froms
{
    public partial class ConfigDictionary : Form
    {
        Dictionary<string, object> Config;
        UIAdaptiveSize uisize;

        public ConfigDictionary(Dictionary<string, object> Config)
        {
            InitializeComponent();
            this.Config = Config;
            uisize = new UIAdaptiveSize
            {
                Width = Width,
                Height = Height,
                FormsName = this.Text,
                X = Width,
                Y = Height,
            };
            uisize.SetInitSize(this);

        }
        bool RefreshingFlag=false;
        private void ConfigDictionary_Load(object sender, EventArgs e)
        {
            RefreshingFlag = false;
            dataGridView1.Columns.Add("Key", "Key");
            dataGridView1.Columns.Add("Value", "Value");
            dataGridView1.Columns.Add("Type", "Type");
            dataGridView1.Columns[0].Width = Convert.ToInt32(150);
            dataGridView1.Columns[1].Width = Convert.ToInt32(400);
            dataGridView1.Columns[2].Width = Convert.ToInt32(400);
            foreach (var item in Config)
            {
                string Value = item.Value == null ? "Null" : item.Value.ToString();
                string Type = item.Value == null ? "Null" : item.GetType().ToString();
                dataGridView1.Rows.Add(new string[] { item.Key, Value, Type });
            }

            RefreshingFlag=true;
        }

        private void ConfigDictionary_Resize(object sender, EventArgs e)
        {
            if (!RefreshingFlag) return;
            var newX = Width;
            var newY = Height;

            uisize.UpdateSize(Width, Height, this);
            foreach (DataGridViewTextBoxColumn item in dataGridView1.Columns)
            {
                item.Width = Convert.ToInt16((item.Width * newX) / uisize.X);
            }
            foreach (DataGridViewTextBoxColumn item in dataGridView1.Columns)
            {
                item.Width = Convert.ToInt16((item.Width * newX) / uisize.X);
            }
            uisize.X = newX;
            uisize.Y = newY;
        }
    }
}
