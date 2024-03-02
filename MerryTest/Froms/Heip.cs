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
    public partial class Heip : Form
    {
        public Heip(string[] text, Point point, Size size)
        {

            InitializeComponent();
            //this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            //int X = point.X + Convert.ToInt32(Convert.ToDouble(size.Width) / 10);
            //int Y = point.Y + size.Height / 10;
            //Location = new Point(X, Y);
            listBox1.DataSource = text;
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void Heip_Load(object sender, EventArgs e)
        {
            listBox1.HorizontalScrollbar = true;
        }
    }
}
