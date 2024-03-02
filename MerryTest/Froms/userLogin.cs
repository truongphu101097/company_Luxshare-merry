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
    public partial class userLogin : Form
    {
        public userLogin()
        {
            InitializeComponent();
        }
        private void btn_Enter_Click(object sender, EventArgs e)
        {
            string UserName = cb_UserName.Text;
            string Password = tb_Password.Text;
            if (UserName == "Admin")
            {
                if (Password == "merryTE")
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    return;
                }

            }
            this.Close();
        }

        private void tb_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            btn_Enter_Click(null, null);
        }

        private void userLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
