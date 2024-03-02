using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using MerryTest.Entity;

namespace MerryTest.MoreTestAPI.Froms
{
    public partial class MoreTestStation : Form
    {
        private MoreTestStation()
        {
            InitializeComponent();
            x = this.Width;
            y = this.Height;
            X = this.Width;
            setTag(this);
        }
        #region 控件大小随窗体大小等比例缩放
        private float X;
        private float x;//定义当前窗体的宽度
        private float y;//定义当前窗体的高度
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ";" + con.Height + ";" + con.Left + ";" + con.Top + ";" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    setTag(con);
                }
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            //遍历窗体中的控件，重新设置控件的值
            foreach (Control con in cons.Controls)
            {
                //获取控件的Tag属性值，并分割后存储字符串数组
                if (con.Tag != null)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ';' });
                    //根据窗体缩放的比例确定控件的值
                    con.Width = Convert.ToInt32(System.Convert.ToSingle(mytag[0]) * newx);//宽度
                    con.Height = Convert.ToInt32(System.Convert.ToSingle(mytag[1]) * newy);//高度
                    con.Left = Convert.ToInt32(System.Convert.ToSingle(mytag[2]) * newx);//左边距
                    con.Top = Convert.ToInt32(System.Convert.ToSingle(mytag[3]) * newy);//顶边距
                    Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字体大小
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                    {
                        setControls(newx, newy, con);
                    }
                }
            }
        }
        private void Station_Resize(object sender, EventArgs e)
        {
            if (!loadflag) return;
            float newx = (this.Width) / x;
            float newy = (this.Height) / y;
            float newX = (this.Width) / X;
            setControls(newx, newy, this);
            foreach (ColumnHeader item in listView1.Columns)
            {
                item.Width = Convert.ToInt16((item.Width * newX));
            }
            int width = this.dataGridView1.Width - 18;
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].Width = Convert.ToInt32(90);
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            this.dataGridView1.Columns[1].Width = Convert.ToInt32(150);
            this.dataGridView1.Columns[6].Width = Convert.ToInt32(900);
            loadflag = true;
            X = this.Width;

        }
        #endregion


        #region 参数区  
        private static MoreTestStation station = null;
        DataTable table = new DataTable();
        DataTable tableRun = new DataTable();

        TestitemEntity testitem = new TestitemEntity();
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static MoreTestStation GetStation()
        {
            if (station == null || station.IsDisposed)
            {
                station = new MoreTestStation();
            }
            return station;
        }
        /// <summary>
        /// 处理序号问题
        /// </summary>
        private void NumberIssue()
        {
            int number = 1;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i]["编号"].ToString() == "0") continue;
                table.Rows[i]["编号"] = number;
                number++;
            }
        }
        private bool iscontent()
        {
            #region 判断字符部分

            if (tb_ProjectName.Text.Contains(",")
                || tb_Sleep.Text.Contains(",")
                || tb_switch.Text.Contains(",")
                || tb_unit.Text.Contains(",")
                || tb_LowerLimit.Text.Contains(",")
                || tb_UpperLimit.Text.Contains(",")
                || tb_Order.Text.Contains(",")
                || tb_way.Text.Contains(","))
            {
                MessageBox.Show("编号，测试项目，延时，继电器，单位，下限，上限，指令，方法 内容不能含有此符号：  ,  ");

                return false;

            }
            return true;
            #endregion
        }
        #endregion


        bool loadflag = false;
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_Load(object sender, EventArgs e)
        {
            table.Columns.Add("编号");
            table.Columns.Add("测试项目");
            table.Columns.Add("单位");
            table.Columns.Add("数值下限");
            table.Columns.Add("数值上限");
            table.Columns.Add("方法");
            table.Columns.Add("指令");
            table.Rows.Clear();
            lb_testliem.Items.Clear();
            MoreProperty.testItem.ForEach(item =>
            {
                lb_testliem.Items.Add(item.测试项目);
                string[] arr = { item.编号.ToString(), item.测试项目, item.单位, item.数值下限, item.数值上限, item.MethodId.ToString(), item.耳机指令 };
                table.Rows.Add(arr);
            });
            tableRun = table.Copy();
            tableRun.Rows.Clear();
            dataGridView1.AllowUserToResizeRows = true;
            dataGridView1.DataSource = table;
            dataGridView1.ReadOnly = false;
            dataGridView1.RowHeadersVisible = false;
            int width = this.dataGridView1.Width - 18;
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].Width = Convert.ToInt32(75);
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            this.dataGridView1.Columns[1].Width = Convert.ToInt32(120);
            this.dataGridView1.Columns[6].Width = Convert.ToInt32(600);
            loadflag = true;
            foreach (var item in (Dictionary<int, Dictionary<string, string>>)MoreProperty.Config["TestControl"])
            {
                cb_Select_ID.Items.Add(item.Key);
            }
            cb_Select_ID.SelectedIndex = 0;
        }
        #region 上下左右按键


        /// <summary>
        /// 更新焦点问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lb_testliem.SelectedIndex;
            if (index < 0) return;
            #region  光标问题
            foreach (DataGridViewRow dgvr in dataGridView1.Rows) dgvr.Selected = false;
            dataGridView1.Rows[index].Selected = true;//设置整行焦点
            dataGridView1.FirstDisplayedScrollingRowIndex = index;//滚动条移到指定位置
            //  dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells[2];//设置单元格焦点
            #endregion
            string[] str = table.Rows[index]["测试项目"].ToString().Split('|');

            if (str[0].Contains("+"))
            {
                str[0] = str[0].Replace("+", "");
                cb_execute.Checked = true;
            }
            else { cb_execute.Checked = false; }
            if (str[0].Contains("-"))
            {
                str[0] = str[0].Replace("-", "");
                cb_Display.Checked = true;
            }
            else
            {
                cb_Display.Checked = false;
            }
            tb_Sleep.Text = "";
            tb_switch.Text = "";
            switch (str.Length)
            {
                case 1:
                    tb_ProjectName.Text = str[0];
                    break;
                case 2:
                    tb_ProjectName.Text = str[0];
                    tb_Sleep.Text = str[1];
                    break;
                case 3:
                    tb_ProjectName.Text = str[0];
                    tb_Sleep.Text = str[1];
                    tb_switch.Text = str[2];
                    break;
            }
            tb_unit.Text = table.Rows[index]["单位"].ToString();
            tb_LowerLimit.Text = table.Rows[index]["数值下限"].ToString();
            tb_UpperLimit.Text = table.Rows[index]["数值上限"].ToString();
            tb_Order.Text = table.Rows[index]["指令"].ToString();
            tb_way.Text = table.Rows[index]["方法"].ToString();
        }
        /// <summary>
        /// 上调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Up_Click(object sender, EventArgs e)
        {
            int index = lb_testliem.SelectedIndex;
            if (index <= 0) return;
            string str = lb_testliem.Items[index].ToString();
            lb_testliem.Items[index] = lb_testliem.Items[index - 1].ToString();
            lb_testliem.Items[index - 1] = str;
            List<string> strlist = new List<string>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                strlist.Add(table.Rows[index][i].ToString());
                table.Rows[index][i] = table.Rows[index - 1][i].ToString();
                table.Rows[index - 1][i] = strlist[i];
            }
            lb_testliem.SelectedIndex = index - 1;
        }
        /// <summary>
        /// 下调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Lower_Click(object sender, EventArgs e)
        {
            int index = lb_testliem.SelectedIndex;
            if (index >= lb_testliem.Items.Count - 1) return;
            string str = lb_testliem.Items[index].ToString();
            lb_testliem.Items[index] = lb_testliem.Items[index + 1].ToString();
            lb_testliem.Items[index + 1] = str;
            List<string> strlist = new List<string>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                strlist.Add(table.Rows[index][i].ToString());
                table.Rows[index][i] = table.Rows[index + 1][i].ToString();
                table.Rows[index + 1][i] = strlist[i];
            }
            lb_testliem.SelectedIndex = index + 1;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Dilite_Click(object sender, EventArgs e)
        {
            int index = lb_testliem.SelectedIndex;
            if (index < 0) return;
            lb_testliem.Items.RemoveAt(index);
            table.Rows[index].Delete();
            lb_testliem.SelectedIndex = index - 1;
            NumberIssue();
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Inset_Click(object sender, EventArgs e)
        {
            {
                if (!iscontent()) return;
                string name = tb_ProjectName.Text;
                string number = "1";
                if (tb_switch.Text.Trim().Length != 0)
                {
                    if (tb_Sleep.Text.Trim().Length == 0)
                    {
                        name += $"|1|{tb_switch.Text}";
                    }
                    else
                    {
                        name += $"|{tb_Sleep.Text}|{tb_switch.Text}";
                    }
                }
                else
                {
                    if (tb_Sleep.Text.Trim().Length != 0)
                    {
                        name += $"|{tb_Sleep.Text}";
                    }
                }
                if (name.Contains("-"))
                {
                    number = "0";
                }
                string[] arr = { number, name, tb_unit.Text, tb_LowerLimit.Text, tb_UpperLimit.Text, tb_way.Text, tb_Order.Text, };
                table.Rows.Add(arr);
                NumberIssue();
                lb_testliem.Items.Add(name);
                lb_testliem.SelectedIndex = lb_testliem.Items.Count - 1;
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Colse_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        /// <summary>
        ///     更新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Updata_KeyDown(object sender, KeyEventArgs e)
        {
            int index = lb_testliem.SelectedIndex;
            int indexx = -1;
            if (index < 0) return;
            if (sender != null) indexx = ((System.Windows.Forms.TextBox)sender).SelectionStart;
            if (!iscontent()) return;
            string name = tb_ProjectName.Text.Replace("-", "").Replace("+", "");
            string number = "1";
            if (cb_Display.Checked) name += "-";
            if (cb_execute.Checked) name += "+";
            if (tb_switch.Text.Trim().Length != 0)
            {
                if (tb_Sleep.Text.Trim().Length == 0)
                {
                    name += $"|1|{tb_switch.Text}";
                }
                else
                {
                    name += $"|{tb_Sleep.Text}|{tb_switch.Text}";
                }
            }
            else
            {
                if (tb_Sleep.Text.Trim().Length != 0)
                {
                    name += $"|{tb_Sleep.Text}";
                }

            }
            if (name.Contains("-"))
            {
                number = "0";
            }
            table.Rows[index]["编号"] = number;
            table.Rows[index]["测试项目"] = name;
            table.Rows[index]["单位"] = tb_unit.Text;
            table.Rows[index]["数值下限"] = tb_LowerLimit.Text;
            table.Rows[index]["数值上限"] = tb_UpperLimit.Text;
            table.Rows[index]["指令"] = tb_Order.Text;
            table.Rows[index]["方法"] = tb_way.Text;
            lb_testliem.Items[index] = name;
            NumberIssue();

            if (indexx != -1) ((System.Windows.Forms.TextBox)sender).SelectionStart = indexx;


        }
        /// <summary>
        /// 独立执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StartRun_Click(object sender, EventArgs e)
        {
            int TestID = int.Parse(cb_Select_ID.Text);
            List<string> HidInfo = new List<string>();
            Dictionary<string, string> MoreTestControl = ((Dictionary<int, Dictionary<string, string>>)MoreProperty.Config["TestControl"])[(TestID)];
            if (MoreTestControl.ContainsKey("Location"))
            {
                new MerryTest.MoreTestAPI.GetHandle().SelectLocation(MoreTestControl["Location"], out HidInfo);
            }
            Dictionary<string, object> OnceConfig = new Dictionary<string, object>();
            OnceConfig["SN"] = MoreProperty.Config["SN"];
            OnceConfig["BindingSN"] = MoreProperty.Config["BindingSN"];
            OnceConfig["BitAddressByBindingSN"] = MoreProperty.Config["BitAddressByBindingSN"];
            OnceConfig["BitAddress"] = MoreProperty.Config["BitAddress"];
            OnceConfig["LincenseKey"] = MoreProperty.Config["LincenseKey"];
            OnceConfig["CustomerSN"] = MoreProperty.Config["CustomerSN"];
            OnceConfig["SN_BCCode"] = MoreProperty.Config["SN_BCCode"];
            OnceConfig["TestResultFlag"] = MoreProperty.Config["TestResultFlag"];
            OnceConfig["HidInfo"] = HidInfo.ToArray();
            OnceConfig["TestTable"] = "TestTable";
            OnceConfig["TestID"] = cb_Select_ID.Text;
            if (MoreProperty.isLoadMydll)
                MoreProperty.myDll.StartTest(TestID, OnceConfig);
            try
            {
                foreach (var item in ((Dictionary<int, Dictionary<string, string>>)MoreProperty.Config["TestControl"])[(TestID)])
                {
                    OnceConfig[item.Key] = item.Value;

                }
            }
            catch (Exception)
            {

                throw;
            }

            TestMethod tm = new TestMethod(OnceConfig, TestID);
            switch (((Button)sender).Name)
            {
                case "btn_StartRun":
                    int index = lb_testliem.SelectedIndex;
                    if (index < 0) return;
                    btn_StartRun.Enabled = false;
                    tb_result.BackColor = System.Drawing.Color.White;
                    tb_result.Text = "";
                    Task.Run(() =>
                    {
                        testitem = text(index);
                        if (testitem.测试项目.Contains("|"))
                        {
                            var arr = testitem.测试项目.Split('|');

                            if (arr.Length == 3) MoreProperty._AllDll.AllDllRun(TestID, "Switch", new object[] { new object[] { $@"dllname=Switch&跳转继电器分={arr[2]}", OnceConfig } });


                            Thread.Sleep(Convert.ToInt32(arr[1]));//项目前延时
                        }
                        bool flag = tm.Test(testitem, out string testvalue);
                        Invoke(new Action(() =>
                        {
                            tb_result.BackColor = flag ? System.Drawing.Color.Chartreuse : System.Drawing.Color.LightCoral;
                            tb_result.Text = testvalue;
                            btn_StartRun.Enabled = true;
                        }));
                    }); break;
                case "btn_ListStartRun":
                    if (listView1.Items.Count <= 0) return;
                    btn_ListStartRun.Enabled = false;
                    TestitemEntity[] testitems = new TestitemEntity[listView1.Items.Count];
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        testitems[i] = text(listView1.Items[i].SubItems[0].Text, listView1.Items[i].SubItems[1].Text);
                        listView1.Items[i].SubItems[2].BackColor = Color.White;
                        listView1.Items[i].SubItems[2].Text = "";
                    }

                    Task.Run(() =>
                    {
                        for (int i = 0; i < testitems.Length; i++)
                        {
                            testitem = testitems[i];
                            if (testitem.测试项目.Contains("|"))
                            {
                                var arr = testitem.测试项目.Split('|');

                                if (arr.Length == 3) MoreProperty._AllDll.AllDllRun(TestID, "Switch", new object[] { new object[] { $@"dllname=Switch&跳转继电器分={arr[2]}", OnceConfig } });

                                Thread.Sleep(Convert.ToInt32(arr[1]));//项目前延时
                            }
                            bool flag = tm.Test(testitem, out string testvalue);
                            Invoke(new Action(() =>
                            {
                                //flag ?        System.Drawing.Color.Chartreuse                  : System.Drawing.Color.LightCoral;
                                listView1.Items[i].SubItems[2].Text = testvalue;
                                listView1.Items[i].UseItemStyleForSubItems = false;
                                listView1.Items[i].SubItems[2].BackColor = flag ? Color.Green : Color.Red;
                                btn_ListStartRun.Enabled = true;
                            }));
                        }
                    });
                    break;
            }
        }
        /// <summary>
        /// 隐藏项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Display_Click(object sender, EventArgs e)
        {
            Updata_KeyDown(null, null);
        }
        private void tb_ProjectName_TextChanged_1(object sender, EventArgs e)
        {
            Updata_KeyDown(null, null);
        }
        private TestitemEntity text(int index)
        {
            List<TestitemEntity> list = new List<TestitemEntity>();
            list.Add(new TestitemEntity
            {
                测试项目 = table.Rows[index]["测试项目"].ToString(),
                单位 = table.Rows[index]["单位"].ToString(),
                数值上限 = table.Rows[index]["数值上限"].ToString(),
                数值下限 = table.Rows[index]["数值下限"].ToString(),
                耳机指令 = table.Rows[index]["指令"].ToString(),
                MethodId = Convert.ToInt32(table.Rows[index]["方法"])
            });
            return list.ToArray()[0];
        }
        private TestitemEntity text(string number, string name)
        {
            TestitemEntity item = new TestitemEntity();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (number == table.Rows[i]["编号"].ToString() && table.Rows[i]["测试项目"].ToString() == name)
                {
                    item.测试项目 = table.Rows[i]["测试项目"].ToString();
                    item.单位 = table.Rows[i]["单位"].ToString();
                    item.数值上限 = table.Rows[i]["数值上限"].ToString();
                    item.数值下限 = table.Rows[i]["数值下限"].ToString();
                    item.耳机指令 = table.Rows[i]["指令"].ToString();
                    item.MethodId = Convert.ToInt32(table.Rows[i]["方法"]);
                    break;
                }
            }
            return item;
        }

        private void btn_insetlist_Click_1(object sender, EventArgs e)
        {
            int index = lb_testliem.SelectedIndex;
            if (index < 0) return;
            string[] str = { table.Rows[index][0].ToString(), table.Rows[index][1].ToString(), "" };
            listView1.Items.Add(new ListViewItem(str));

        }

        private void btn_diletelist_Click(object sender, EventArgs e)
        {
            try
            {
                int index = listView1.SelectedIndices[0];
                listView1.Items[index].Remove();
                listView1.Items[index - 1].Selected = true;
            }
            catch (Exception)
            {
            }
        }

        private void Savs_Click(object sender, EventArgs e)
        {
            string path = $@".\TestItem\{MoreProperty.Config["Name"]}";
            using (FileStream fs = new FileStream($"{path}\\{MoreProperty.Config["Station"]}.txt", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string str = $"{table.Rows[i]["编号"]},{table.Rows[i]["测试项目"]},{table.Rows[i]["单位"]},{table.Rows[i]["数值下限"]},{table.Rows[i]["数值上限"]},{table.Rows[i]["方法"]},{table.Rows[i]["指令"]}";
                        string[] isstr = str.Split(',');
                        foreach (string item in isstr)
                        {
                            if (item.Length == 0)
                            {
                                MessageBox.Show($"该测试项目：[    {isstr[1]}    ]，\r除了延时和继电器，其他参数不能为空请重新检测参数");
                                sw.Close();
                                fs.Close();
                                return;
                            }
                        }
                        sw.WriteLine(str);
                    }
                    sw.Close();
                    fs.Close();
                }
            }
            this.Close();
        }
    }
}
