using MerryTest.Entity;
using MerryTest.Froms;
using MerryTest.MoreTestAPI;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MerryTest.MoreTestAPI.MoreProperty;

namespace MerryTest
{
    public partial class MoreTest : Form
    {
        public MoreTest()
        {
            InitializeComponent();
            this.Text = $"MoreTest-22.10.28.0";

        }

        //###################################################     加载部分       ！！！！！！！！！！！！！！！！！！！！！！！！！！
        #region 加载部分
        private void MoreTest_Load(object sender, EventArgs e)
        {
            if (TestingFlag || RefreshingFlag) return;
            RefreshData();
            ShowProgess();
            if (!ReviewMoreProperty()) return;
            RunEvent(MoreTestControl.First().Key, (List<string[]>)Config["LoadEvent"]);
            LoadAllDll();
            LoadTestType();
            LoadTestItem();
            RenderingTheForm();

        }

        /// <summary>
        /// 清除数据重新加载
        /// </summary>
        public void RefreshData()
        {
            RefreshingFlag = true;
            Config.Clear();
            _AllDll.Dispose();
            lv_TestTable.Items.Clear();
            lv_TestTable.Clear();

            testItem.Clear();
            tb_ProgramInfo.Clear();
            _AllDll.Config = Config;
            Config.Add("admin", this);
            Config.Add("adminHandle", Handle);
            Config.Add("adminPath", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Config.Add("LoadMessagebox", "程序启动");
            Config.Add("LoadMessageboxFlag", false);

            //获取MES部分的Config信息
            _AllDll.AllDllRun(0, "Config_ini", "MesConfig", null);
            MoreTestControl = (Dictionary<int, Dictionary<string, string>>)Config["TestControl"];
        }
        /// <summary>
        /// 提示程序是否需要重启
        /// </summary>
        /// <returns></returns>
        public bool ReviewMoreProperty()
        {
            if (MoreTestControl.Count <= 0)
            {
                if (MessageboxShow(this, "连扳模板加载TestControl异常了，请检查配置参数，程序是否需要重启") == DialogResult.Yes)
                {
                    RefreshingFlag = false;
                    MoreTest_Load(null, null);
                    return false;
                }
                MoreTest_FormClosed(null, null);
            }
            return true;
        }
        /// <summary>
        /// 渲染界面
        /// </summary>
        public void RenderingTheForm()
        {
            Rendering_dgv_TestResult();
            Rendering_dgv_TestTable();
            RendEring_Widget();
            RefreshingFlag = false;
            SetWinFormSize();
            ShowProgramInformation();
            //移除已经存在的事件
            KeyDownEventClass.KeyDownEvent -= KeyDownEventClass_KeyDownEvent;
            //新增检测回车的事件
            KeyDownEventClass.KeyDownEvent += KeyDownEventClass_KeyDownEvent;
            LoadingFlag = true;
            Config["LoadMessageboxFlag"] = true;
            SeltctDGV_ResultTable();
        }
        /// <summary>
        /// 渲染测试结果表格
        /// </summary>
        public void Rendering_dgv_TestResult()
        {
            dgv_TestResult.Rows.Clear();
            dgv_TestResult.Columns.Clear();
            dgv_TestResult.Columns.Add("Col0", "Col0");
            //添加一行
            dgv_TestResult.Rows.Add(new object[] { "\\", });
            //设置高度
            dgv_TestResult.Rows[0].Height = 30;
            //设置选中后字体颜色
            dgv_TestResult.Rows[0].DefaultCellStyle.SelectionForeColor = Color.Black;
            //设置设置字体大小
            dgv_TestResult.Rows[0].DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //设置选中后背景色
            dgv_TestResult.Rows[0].DefaultCellStyle.SelectionBackColor = Color.White;

            //将行设定为只读
            dgv_TestResult.Rows[0].ReadOnly = true;
            dgv_TestResult.Rows.Add(new object[] { "SN" });
            dgv_TestResult.Rows[1].Height = 45;
            dgv_TestResult.Rows[1].DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv_TestResult.Rows[1].DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 224, 224);
            dgv_TestResult.Rows[1].DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dgv_TestResult.Rows[1].ReadOnly = false;
            dgv_TestResult.Rows.Add(new object[] { "测试项目" });
            dgv_TestResult.Rows[2].DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv_TestResult.Rows[2].DefaultCellStyle.SelectionBackColor = Color.White;
            dgv_TestResult.Rows[2].DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dgv_TestResult.Rows[2].Height = 45;
            dgv_TestResult.Rows[2].ReadOnly = true;
            //设置第一列，列宽
            dgv_TestResult.Columns[0].Width = 117;
            //程序设定默认有8列
            for (int i = 1; i <= 8; i++)
            {
                string index = $"Col{i}";
                //添加列
                dgv_TestResult.Columns.Add(index, index);
                //设定列字体居中
                dgv_TestResult.Columns[index].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                //设定列的字体
                dgv_TestResult.Columns[index].DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                //设定宽度
                dgv_TestResult.Columns[index].Width = 117;

            }
            int ColIndex = 1;
            //将所有线程需要渲染的编号设定
            foreach (var item in MoreTestControl)
            {
                string index = $"{item.Key}";
                dgv_TestResult.Columns[ColIndex].HeaderText = index;
                dgv_TestResult.Columns[ColIndex].Name = index;
                dgv_TestResult[$"{item.Key}", 0].Value = index;
                dgv_TestResult[$"{item.Key}", 1].Value = "";
                dgv_TestResult[$"{item.Key}", 2].Value = "";

                ColIndex++;
            }
            //将用户不需要编辑的列都锁定只读
            for (int i = 0; i < dgv_TestResult.Columns.Count; i++)
            {
                if (dgv_TestResult.Columns[i].HeaderText.Contains("Col"))
                    dgv_TestResult.Columns[i].ReadOnly = true;
            }
        }

        /// <summary>
        /// 渲染测试表格
        /// </summary>
        public void Rendering_dgv_TestTable()
        {
            lv_TestTable.Columns.Add("编号", "编号");
            lv_TestTable.Columns.Add("测试项目", "测试项目");
            lv_TestTable.Columns.Add("单位", "单位");
            lv_TestTable.Columns.Add("数值下限", "数值下限");
            lv_TestTable.Columns.Add("数值上限", "数值上限");
            lv_TestTable.Columns[0].Width = 60;
            lv_TestTable.Columns[1].Width = 140;
            lv_TestTable.Columns[2].Width = 60;
            lv_TestTable.Columns[3].Width = 90;
            lv_TestTable.Columns[4].Width = 90;


            //渲染测试项目，单位，上限，下限
            List<string[]> testItemList = (List<string[]>)Config["TestItem"];
            int number = 0;
            foreach (var ItemLise in testItemList)
            {

                string TestName = ItemLise[1].Contains('|') ? ItemLise[1].Split('|')[0] : ItemLise[1];
                if (TestName.Contains("-")) continue;
                lv_TestTable.Items.Add(new ListViewItem(new String[] { ItemLise[0], TestName, ItemLise[2], ItemLise[3], ItemLise[4] }));
                int MethodId = int.Parse(ItemLise[5]);
                lv_TestTable.Items[number].UseItemStyleForSubItems = false;
                if (MethodId == 3 || MethodId == 6)
                {

                    lv_TestTable.Items[number].SubItems[2].BackColor = Color.Gainsboro;
                    lv_TestTable.Items[number].SubItems[3].BackColor = Color.Gainsboro;
                    lv_TestTable.Items[number].SubItems[4].BackColor = Color.Gainsboro;
                }
                number++;


            }

            //根据线程渲染测试增加测试结果列
            foreach (var item in MoreTestControl)
            {
                TestFlags[item.Key] = 2;
                string index = $"{item.Key}";

                int col;
                col = this.lv_TestTable.Columns.Add(new ColumnHeader
                {
                    Width = 120,
                    Name = $"{item.Key}",
                    ImageKey = $"{item.Key}",
                    Text = $"{item.Key}号",
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                });
                for (int i = 0; i < lv_TestTable.Items.Count; i++)
                {
                    lv_TestTable.Items[i].SubItems.Add("");
                    lv_TestTable.Items[i].SubItems[col].Name = $"{item.Key}";
                    lv_TestTable.Items[i].SubItems[$"{item.Key}"].Text = "";

                }
            }


        }
        /// <summary>
        /// 渲染程序控件
        /// </summary>
        public void RendEring_Widget()
        {
            lb_Title.Text = $"{Config["Name"]}  {Config["Station"]}";
            string lableText = "单机模式";
            if ((int)Config["MesFlag"] > 0)
            {
                lableText = $"MES模式:{Config["SystematicName"]}";
            }
            else if ((bool)Config["GetBDFlag"])
            {
                lableText = $"BD模式:{Config["SystematicName"]}";
            }
            string EngineerMode = (bool)Config["EngineerMode"] ? "工程" : "量产";

            lb_TestMode.Text = $@"{EngineerMode}{lableText}";
            lb_ScanHint.Text = "请扫描[1]号";

            string computerName = Environment.MachineName;
            // List<IPAddress> ipv4 = Dns.GetHostAddresses(computerName).Where(item => item.AddressFamily == AddressFamily.InterNetwork).ToList();
            tb_ProgramInfo.Text += $"作业信息=>\r\n";
            tb_ProgramInfo.Text += $"工号：{Config["UserID"]}\r\n";
            tb_ProgramInfo.Text += $"工单：{Config["Works"]}\r\n";
            tb_ProgramInfo.Text += $"料号：{Config["OrderNumberInformation"]}\r\n";
            tb_ProgramInfo.Text += $"电脑名称：{computerName}\r\n";
            //ipv4.ForEach(ip => tb_ProgramInfo.Text += $"Ipv4：{ip}\r\n");
            tb_ProgramInfo.Text += $"\r\n";
            tb_ProgramInfo.Text += $"程序信息=>\r\n";
            if (isLoadMydll)
            {
                foreach (var keys in myDll.dllInfo)
                    foreach (var item in keys.Value)
                    {
                        if (!item.Contains("当前")) continue;
                        tb_ProgramInfo.Text += $"{Config["Name"]}： 版本  ：{item.Split('：')[1]}\r\n";
                        break;
                    }
            }
            else
            {
                tb_ProgramInfo.Text += $"{Config["Name"]}： 版本  ：0.0.0.0\r\n";
            }


            foreach (var keys in _AllDll.dllInfo)
                foreach (var item in keys.Value)
                {
                    if (!item.Contains("当前")) continue;
                    tb_ProgramInfo.Text += $"{keys.Key}： 版本  ：{item.Split('：')[1]}\r\n";
                    break;
                }
        }
        /// <summary>
        /// 记录窗体大小，用于程序自适应
        /// </summary>
        public void SetWinFormSize()
        {
            if (uias == null)
            {
                uias = new UIAdaptiveSize
                {
                    Width = Width,
                    Height = Height,
                    FormsName = this.Text,
                    X = Width,
                    Y = Height,

                };
                uias.SetInitSize(this);
            }
            if ((bool)Config["TopMostFlag"])
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
            if ((bool)Config["Maximized"])
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if ((bool)Config["MinimumSize"])
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
        /// <summary>
        /// 显示窗体料号数据数据
        /// </summary>
        public void ShowProgramInformation()
        {
            ProgramInformation infoForms = ProgramInformation.ShowPicture(Config);
            infoForms.Show();
            infoForms.ShowEvent();
        }
        /// <summary>
        /// 程序启动将程序焦点选中到可以输入的条码框内
        /// </summary>
        public async void SeltctDGV_ResultTable()
        {
            await Task.Run(() => Thread.Sleep(300));
            WindowsAPI.SetForegroundWindow(Handle);
            for (int i = 0; i < dgv_TestResult.Columns.Count; i++)
                if (!dgv_TestResult[i, 1].ReadOnly)
                {
                    dgv_TestResult[i, 1].Selected = true;
                    dgv_TestResult.CurrentCell = dgv_TestResult[i, 1];
                    break;
                }

        }
        #endregion

        //###################################################     开始测试部分     ！！！！！！！！！！！！！！！！！！！！！！！！！！
        #region 开始测试部分
        /// <summary>
        /// 进入编辑后变色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_TestResult_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int Col = e.ColumnIndex;
            int Row = e.RowIndex;
            if (dgv_TestResult[Col, Row].ReadOnly)
                return;
            dgv_TestResult[Col, Row].Style.BackColor = Color.White;
            lb_ScanHint.Text = $"请扫描[{dgv_TestResult.Columns[Col].Name}]号";
        }
        bool EnterFlag = false;
        /// <summary>
        /// 监听按下回车键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void KeyDownEventClass_KeyDownEvent(object sender, KeyEventArgs e)
        {

            //     监听电脑的回车按键     判断焦点在哪       判断  电脑选中程序的焦点指针
            if (e.KeyCode == Keys.Enter && this.Handle == WindowsAPI.GetForegroundWindow())
            {
                try
                {
                    if (EnterFlag)
                        return;
                    EnterFlag = true;
                    int Col = this.dgv_TestResult.CurrentCell.ColumnIndex;
                    int Row = this.dgv_TestResult.CurrentCell.RowIndex;
                    if (dgv_TestResult[Col, Row].ReadOnly)
                        return;
                    EnterKeyDownFlag = true;
                    //进入格子的事件后等待格子使用完毕
                    await Task.Run(() =>
                    {
                        Thread.Sleep(50);
                        while (RenderCellFlag)
                        { }

                    });
                    EnterKeyDownFlag = false;
                    if (!TestFlags.ContainsValue(2))
                    {
                        this.dgv_TestResult[0, 0].Selected = true;
                        this.dgv_TestResult.CurrentCell = this.dgv_TestResult[0, 0];
                        return;
                    }
                    _ = Task.Run(() =>
                    {
                        //因为焦点的问题一直没有好的办法解决所以干脆设定1.5秒内将系统的焦点指定在程序
                        for (int i = 0; i < 5; i++)
                        {
                            WindowsAPI.SetForegroundWindow(Handle);
                            Thread.Sleep(300);
                        }
                    });
                    int ColName = Convert.ToInt16(dgv_TestResult.Columns[Col].Name);
                    //控制扫描完毕后跳转到哪个格子输入
                    if (SkipCellFlag)
                    {
                        int maxTestID = MoreTestControl.Keys.Max();
                        if (maxTestID <= ColName)
                        {
                            for (int i = 0; i < dgv_TestResult.ColumnCount; i++)
                                if (!dgv_TestResult[i, 1].ReadOnly)
                                {
                                    Col = i;
                                    break;
                                }
                        }
                        else
                        {
                            bool SkipFlag = true;
                            for (int i = Col; i < dgv_TestResult.ColumnCount; i++)
                                if (!dgv_TestResult[i, 1].ReadOnly)
                                {
                                    Col = i;
                                    SkipFlag = false;
                                    break;
                                }
                            if (SkipFlag)
                                for (int i = 0; i < dgv_TestResult.ColumnCount; i++)
                                    if (!dgv_TestResult[i, 1].ReadOnly)
                                    {
                                        Col = i;
                                        break;
                                    }
                        }
                    }
                    //将格子内容清空
                    this.dgv_TestResult[Col, Row].Value = "";
                    //将格子设置为焦点
                    this.dgv_TestResult[Col, Row].Selected = true;
                    //将格子进入编辑状态
                    this.dgv_TestResult.CurrentCell = this.dgv_TestResult[Col, Row];

                }
                finally
                {
                    EnterFlag = false;
                }
            }

        }
        private void dgv_TestResult_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            RenderCellFlag = true;
            try
            {

                if (!EnterKeyDownFlag)
                    return;
                int Col = e.ColumnIndex;
                int Row = e.RowIndex;
                SkipCellFlag = StartTestInit(int.Parse(dgv_TestResult.Columns[Col].Name), Row);
            }
            finally
            {
                RenderCellFlag = false;
                EnterKeyDownFlag = false;
            }

        }
        public bool StartTestInit(int Col, int Row)
        {

            Config["TestResultFlag"] = true;
            Thread thread;
            Config["SN"] = dgv_TestResult[$"{Col}", Row].Value.ToString().Trim().ToUpper();
            int TestControlFlag = (int)Config["TestControlFlag"];
            int StartDelay = (int)Config["StartDelay"];
            if (!ReviewSN(Col, (string)Config["SN"], TestControlFlag))
                return false;
            RunEvent(Col, (List<string[]>)Config["StartTestEvent"]);
            if (!(bool)Config["TestResultFlag"])
                return false;
            switch (TestControlFlag)
            {

                case 4:     //扫一测一
                    snList[Col] = (string)Config["SN"];
                    TestFlags[Col] = 1;
                    StartTestInit(Col);
                    thread = new Thread(new ParameterizedThreadStart(StartTest));
                    thread.Start(new object[] { Col, (string)Config["SN"], (int)Config["StartDelay"] });
                    break;
                case 3:     //不输入条码全部线程一起跑
                    foreach (var item in MoreTestControl)
                    {
                        Thread.Sleep(5);
                        Config["SN"] = $"{DateTime.Now.ToString("HH:mm:ss:ffff")}";
                        dgv_TestResult[$"{item.Key}", Row].Value = Config["SN"].ToString();
                        snList[item.Key] = (string)Config["SN"];
                        TestFlags[item.Key] = 1;
                        StartTestInit(item.Key);
                        thread = new Thread(new ParameterizedThreadStart(StartTest));
                        thread.Start(new object[] { item.Key, (string)Config["SN"], (int)Config["StartDelay"] });
                    }
                    break;

                case 0:     //扫描后存着一起跑
                default:
                    snList[Col] = (string)Config["SN"];
                    TestFlags[Col] = 1;
                    StartTestInit(Col);

                    if (TestFlags.ContainsValue(2))
                        break;
                    foreach (var item in MoreTestControl)
                    {
                        thread = new Thread(new ParameterizedThreadStart(StartTest));
                        thread.Start(new object[] { item.Key, snList[item.Key], (int)Config["StartDelay"] });
                    }
                    break;
            }
            return true;

        }
        public void StartTestInit(int Col)
        {
            for (int i = 0; i < lv_TestTable.Items.Count; i++)
            {
                lv_TestTable.Items[i].SubItems[$"{Col}"].Text = "";
                lv_TestTable.Items[i].SubItems[$"{Col}"].BackColor = Color.White;
            }
            for (int i = 1; i < dgv_TestResult.RowCount; i++)
                this.dgv_TestResult[$"{Col}", i].Style.BackColor = Color.Yellow;
            this.dgv_TestResult[$"{Col}", 1].ReadOnly = true;
            this.dgv_TestResult[$"{Col}", 2].Value = "await";
        }
        public void StartTest(object obj)
        {
            TestingFlag = true;//正在测试标志位
            int TestID = (int)((object[])obj)[0];
            string SN = (string)((object[])obj)[1];
            int StartDelay = (int)((object[])obj)[2];
            bool flag = true;//测试结果标志
            string TestValue = "";//测试值
            DateTime StartTestTime = DateTime.Now;
            StringBuilder OnceTestLogging = new StringBuilder();//测试日记记录
            try
            {
                List<string> HidInfo = new List<string>();
                if (MoreTestControl[TestID].ContainsKey("Location"))
                {
                    new MerryTest.MoreTestAPI.GetHandle().SelectLocation(MoreTestControl[TestID]["Location"], out HidInfo);
                }
                Dictionary<string, object> OnceConfig = new Dictionary<string, object>() {
                    {"SN", SN },
                    {"BindingSN","" },//barcode SN
                    {"BitAddressByBindingSN","" },//通过通过barcodeSN获取的SN
                    {"BitAddress","" },//通过SN获取的BD号
                    {"LincenseKey","" },//通过SN获取的LincenseKey号
                    {"CustomerSN","" },//通过SN获取的客户SN号
                    {"SN_BCCode","" },//通过SN获取的Poly客户SN号
                    {"TestResultFlag", flag },
                    {"TestID",TestID },
                    {"HidInfo",HidInfo.ToArray()},
                    {"FreqTrim","" }
            };
                foreach (var item in MoreTestControl[TestID])
                    OnceConfig[item.Key] = item.Value;
                TestMethod tm = new TestMethod(OnceConfig, TestID);
                Thread.Sleep(StartDelay);
                if (isLoadMydll)
                    flag = myDll.StartTest(TestID, OnceConfig);
                OnceTestLogging.Append(RecordTheLog(TestID, $"测试线程：{TestID}，StartTest方法执行结果：{flag}"));
                ///循环开始测试
                foreach (var item in testItem)
                {
                    if (!flag && !item.测试项目.Contains("+")) continue;//即使测试失败任然执行的测试项+
                    if (flag) Invoke(new Action(() => dgv_TestResult[$"{TestID}", 2].Value = item.测试项目));//将测试项目渲染到测试界面表格
                    OnceTestLogging.Append(RecordTheLog(TestID, $"测试线程：{TestID}，{item.测试项目}，耳机指令为：{item.耳机指令}，调用方法序号为：{item.MethodId}"));
                    //测试项
                    bool testResult = false;
                    //项目名称包含*就按照*的个数多测几次
                    for (int j = 0; j < item.测试项目.Split('*').Length; j++)
                    {
                        if (item.测试项目.Contains("|"))
                        {
                            string[] arr = item.测试项目.Split('|');

                            if (arr.Length == 3)
                                _AllDll.AllDllRun(TestID, "Switch", "Run", new object[] { new object[] { $@"dllname=Switch&跳转继电器分={arr[2]}", OnceConfig } });
                            Thread.Sleep(int.Parse(arr[1]));//项目前延时
                        }
                        testResult = tm.Test(item, out TestValue);
                        if (testResult) break;
                        if (item.测试项目.Contains("*")) Thread.Sleep(1500);
                    }
                    if (!testResult)
                        OnceConfig["TestResultFlag"] = flag = false;
                    OnceTestLogging.Append(RecordTheLog(TestID, $"测试线程：{TestID}，测试值：{TestValue}，测试结果：{flag}"));
                    //界面回馈结果
                    if (!item.测试项目.Contains("-"))//需要不显示在界面的测试项-
                    {
                        Invoke(new Action(() =>
                        {
                            var i = item.编号 - 1;//界面测试项
                            lv_TestTable.Items[i].SubItems[$"{TestID}"].Text = TestValue;
                            lv_TestTable.Items[i].SubItems[$"{TestID}"].BackColor = flag ? Color.Green : Color.Red;
                            if (flag)
                            {
                                lv_TestTable.EnsureVisible(i);//保存项目可见
                                lv_TestTable.Items[i].EnsureVisible();
                                Application.DoEvents();
                            }
                        }));
                    }
                }
                OnceTestLogging.Append(RecordTheLog(TestID, $"##############################################################"));
                OnceTestLogging.Append(RecordTheLog(TestID, $"测试线程：{TestID}， ###############################测试结束，最终测试结果为:【{Config["TestResultFlag"]}】###############################"));
                OnceTestLogging.Append(RecordTheLog(TestID, $"开始执行TestEndEvent"));
                OnceTestLogging.Append(RecordTheLog(TestID, $"##############################################################"));
                OnceTestLogging.Append(RecordTheLog(TestID, ""));
                OnceTestLogging.Append(RecordTheLog(TestID, ""));
                OnceConfig["TestTime"] = DateTime.Now - StartTestTime;
                OnceConfig["OnceTestLogging"] = OnceTestLogging.ToString();
                OnceConfig["TestResultFlag"] = flag;
                OnceConfig["TestTable"] = dataGridViewTableToDataTable(TestID);
                RunEvent(TestID, (List<string[]>)Config["TestEndEvent"], OnceConfig);
                flag = (bool)OnceConfig["TestResultFlag"];
                //最终结果渲染
                Invoke(new Action(() =>
                {
                    if (flag)
                        dgv_TestResult[$"{TestID}", 2].Value = $"Pass";
                    for (int i = 1; i < dgv_TestResult.Rows.Count; i++)
                        dgv_TestResult[$"{TestID}", i].Style.BackColor = flag ? Color.Green : Color.Red;
                    Application.DoEvents();
                }));
            }
            finally
            {
                lock (snList)
                {
                    Thread.Sleep(50);
                    WindowsAPI.SetForegroundWindow(Handle);
                    snList[TestID] = "";
                    TestFlags[TestID] = 2;
                    TestEndInit(TestID);
                }

            }


        }
        private void dgv_TestResult_Leave(object sender, EventArgs e)
        {
            this.dgv_TestResult.CurrentCell.Selected = false;
        }

        private void TestEndInit(int TestID)
        {
            int TestControlFlag = (int)Config["TestControlFlag"];
            Invoke(new Action(() =>
            {
                switch (TestControlFlag)
                {
                    case 4:

                        dgv_TestResult[$"{TestID}", 1].ReadOnly = false;
                        if (dgv_TestResult.IsCurrentCellInEditMode)
                        {
                            break;
                        }
                        dgv_TestResult[$"{TestID}", 1].Selected = true;
                        dgv_TestResult[$"{TestID}", 1].Value = "";
                        dgv_TestResult.CurrentCell = dgv_TestResult[$"{TestID}", 1];
                        break;
                    case 3:
                    case 0:
                    default:
                        if (TestFlags.ContainsValue(1))
                            return;
                        myDll.TestsEnd(TestID);
                        for (int i = 0; i < dgv_TestResult.Columns.Count; i++)
                        {
                            if (dgv_TestResult.Columns[i].HeaderText.Contains("Col"))
                                continue;
                            dgv_TestResult[i, 1].ReadOnly = false;
                        }
                        dgv_TestResult[1, 1].Selected = true;
                        dgv_TestResult[1, 1].Value = "";
                        dgv_TestResult.CurrentCell = dgv_TestResult[1, 1];
                        break;
                }
            }));
            TestingFlag = TestFlags.ContainsValue(1);

        }
        private DataTable dataGridViewTableToDataTable(int TestID)
        {
            DataTable table = new DataTable();
            table.Columns.Add("编号");
            table.Columns.Add("测试项目");
            table.Columns.Add("单位");
            table.Columns.Add("数值下限");
            table.Columns.Add("数值上限");
            table.Columns.Add("测试值");
            table.Columns.Add("测试结果");
            Invoke(new Action(() =>
            {
                for (int i = 0; i < lv_TestTable.Items.Count; i++) 
                {
                    table.Rows.Add();
                    table.Rows[i][0] = lv_TestTable.Items[i].SubItems[0].Text;
                    table.Rows[i][1] = lv_TestTable.Items[i].SubItems[1].Text;
                    table.Rows[i][2] = lv_TestTable.Items[i].SubItems[2].Text;
                    table.Rows[i][3] = lv_TestTable.Items[i].SubItems[3].Text;
                    table.Rows[i][4] = lv_TestTable.Items[i].SubItems[4].Text;
                    table.Rows[i][5] = lv_TestTable.Items[i].SubItems[$"{TestID}"].Text;
                    table.Rows[i][6] = lv_TestTable.Items[i].SubItems[$"{TestID}"].BackColor == Color.Green;

                }
            }));
            return table;
        }



        #endregion

        //###################################################     菜单栏事件     ！！！！！！！！！！！！！！！！！！！！！！！！！！
        #region 菜单栏事件

        //###################################################     菜单栏登录事件     ！！！！！！！！！！！！！！！！！！！！！！！！！！

        private void 登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userLogin box = new userLogin()
            {
                MdiParent = this.MdiParent
            };
            box.StartPosition = FormStartPosition.CenterScreen;
            if (box.ShowDialog() == DialogResult.OK)
                foreach (ToolStripMenuItem tool in menuStrip1.Items)
                {
                    foreach (ToolStripMenuItem items in tool.DropDownItems)
                        items.Enabled = true;
                    tool.Enabled = true;
                }
        }
        public void 插件事件(object sender, EventArgs e)
        {
            string Dllname = ((ToolStripMenuItem)sender).Name;
            string path = $@"{Config["adminPath"]}\AllDLL\MenuStrip\Interface\{Dllname}\{Dllname}.exe";
            using (Process.Start(path)) { }
        }
        //###################################################     加载插件和加载文档事件     ！！！！！！！！！！！！！！！！！！！！！！！！！！

        public void 插件ToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (!LoadingFlag) return;
            List<ToolStripMenuItem> list = new List<ToolStripMenuItem>();

            #region 测试模块信息加载
            关于DllModeToolStripMenuItem.DropDownItems.Clear();

            #region 机型模块信息加载
            string TypePath = $"{Config["adminPath"]}\\TestItem\\{Config["Name"].ToString().Split('-')[0]}";
            string TypeName = Path.GetFileName(TypePath);
            ToolStripMenuItem tool = new ToolStripMenuItem();
            tool.Text = TypeName;
            tool.Name = TypeName;
            tool.Font = new System.Drawing.Font("黑体", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tool.Size = new System.Drawing.Size(224, 26);
            list.Add(tool);
            #endregion
            #region  接口插件模块加载
            foreach (string path in Directory.GetDirectories($@"{Config["adminPath"]}\AllDll"))
            {
                string Name = Path.GetFileName(path);
                if (!File.Exists($@"{path}\{Name}.dll")) continue;

                tool = new ToolStripMenuItem();
                tool.Text = Name;
                tool.Name = Name;
                tool.Font = new System.Drawing.Font("黑体", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                tool.Size = new System.Drawing.Size(224, 26);
                list.Add(tool);
            }
            关于DllModeToolStripMenuItem.DropDownItems.AddRange(list.ToArray());
            list.Clear();
            插件ToolStripMenuItem.DropDownItems.Clear();

            foreach (string path in Directory.GetDirectories($@"{Config["adminPath"]}\AllDll\MenuStrip\Interface"))
            {
                string Name = Path.GetFileName(path);
                if (!File.Exists($@"{path}\{Name}.exe")) continue;
                tool = new ToolStripMenuItem();
                tool.Text = Name;
                tool.Name = Name;
                tool.Font = new System.Drawing.Font("黑体", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                tool.Size = new System.Drawing.Size(224, 26);
                tool.Click += new System.EventHandler(this.插件事件);
                list.Add(tool);
            }
            插件ToolStripMenuItem.DropDownItems.AddRange(list.ToArray());
            #endregion
            list.Clear();
            #endregion

            #region 文档说明加载
            关于文档ToolStripMenuItem.DropDownItems.Clear();
            string DocumentPath = $@"{Config["adminPath"]}\AllDLL\MenuStrip\Document";
            文档获取(DocumentPath, list);
            关于文档ToolStripMenuItem.DropDownItems.AddRange(list.ToArray());
            list.Clear();
            #endregion
        }
        public List<ToolStripMenuItem> 文档获取(string path, List<ToolStripMenuItem> tools)
        {

            foreach (var item in Directory.GetFiles(path))
            {
                string FileName = Path.GetFileNameWithoutExtension(item);
                if (FileName.Contains("~$")) continue;
                ToolStripMenuItem tool = new ToolStripMenuItem();
                tool.Text = FileName;
                tool.Name = FileName;
                tool.Tag = item;
                tool.Font = new System.Drawing.Font("黑体", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                tool.Size = new System.Drawing.Size(224, 26);
                tool.Click += new System.EventHandler(this.文档点击事件);
                tools.Add(tool);
            }
            foreach (var item in Directory.GetDirectories(path))
            {
                文档获取(item, tools);
            }

            return tools;
        }
        public void 文档点击事件(object sender, EventArgs e)
        {
            string dllName = ((ToolStripMenuItem)sender).Tag.ToString();
            using (Process.Start(dllName)) { }

        }
        //###################################################     显示字典当前内容     ！！！！！！！！！！！！！！！！！！！！！！！！！！
        private void 显示字典的值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ConfigDictionary(Config).Show();
        }

        private void 连接IP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string RT550_IP = Interaction.InputBox("RT550连接地址", "提示", $"{Config["RT550_IP"]}", -1, -1);
            if (RT550_IP.Trim().Length > 0)
            {
                Config["RT550_IP"] = RT550_IP;
                IniHelpe.SetValue("Visa", "RT550_IP", $"{Config["RT550_IP"]}");
            }
        }


        //###################################################     Loop 设定 事件     ！！！！！！！！！！！！！！！！！！！！！！！！！！

        public void 循环测试次数StripMenuItem2_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tool = (ToolStripMenuItem)sender;
            string numbers = tool.Text == "N+" ? Interaction.InputBox("输入循环测试次数", "提示", "10", -1, -1) : tool.Text;
            Config["TestNumbers"] = int.Parse(numbers);
            Config["LoopFlag"] = btn_Loop.Visible = btn_Loop.Enabled = true;
        }
        private void menuStrip1_MouseEnter(object sender, EventArgs e)
        {
            if (!LoadingFlag) return;
            List<string> str = new List<string>();
            str.Add("CH200001");
            if (str.Contains(Environment.UserName) || (bool)Config["EngineerMode"])
                foreach (ToolStripMenuItem tool in menuStrip1.Items)
                {
                    foreach (ToolStripMenuItem items in tool.DropDownItems)
                        items.Enabled = true;
                    tool.Enabled = true;
                }
        }

        //###################################################     条码长度设定 事件     ！！！！！！！！！！！！！！！！！！！！！！！！！！

        private void 条码长度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Barcode = Interaction.InputBox("输入条码框条码长度", "提示", $"{Config["Barcode"]}", -1, -1);
            if (Barcode.Trim().Length < 1) return;
            Config["Barcode"] = int.TryParse(Barcode, out int result) ? result : Config["Barcode"];
            IniHelpe.SetValue("Config", "Barcode", result.ToString());

        }
        //###################################################     打印机 设定 事件     ！！！！！！！！！！！！！！！！！！！！！！！！！！

        private void 打印机toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            打印机toolStripMenuItem5.DropDownItems.Clear();
            PrintDocument print = new PrintDocument();
            string sDefault = print.PrinterSettings.PrinterName;//默认打印机名
            List<ToolStripMenuItem> printList = new List<ToolStripMenuItem>();
            foreach (string sPrint in PrinterSettings.InstalledPrinters)//获取所有打印机名称
            {
                ToolStripMenuItem tool = new ToolStripMenuItem()
                {
                    Text = sPrint,
                    Font = new System.Drawing.Font("黑体", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Size = new System.Drawing.Size(224, 26),
                };

                tool.Click += new System.EventHandler(this.打印机名称设定事件);
                printList.Add(tool);
            }
            打印机toolStripMenuItem5.DropDownItems.AddRange(printList.ToArray());
        }
        public void 打印机名称设定事件(object sender, EventArgs e)
        {
            ToolStripMenuItem tool = (ToolStripMenuItem)sender;
            Config["Print"] = tool.Text;
            IniHelpe.SetValue("Print", "Print", tool.Text);
        }

        private void addToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MerryTest.MoreTestAPI.MoreTestAddTool.AddTool().Run();
        }


        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoreTest_Load(null, null);
        }

        private void 切换到单板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IniHelpe.SetValue("Flag", "MoreTestMode", "0");
            Process ps = new Process();
            ps.StartInfo.FileName = Application.ExecutablePath;
            ps.Start();
            Process.GetCurrentProcess().Kill();

        }

        private void 切换测试模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tool = (ToolStripMenuItem)sender;

            string path = $"{Config["adminPath"]}/TestItem/{Config["Name"]}/0_TestControl/TestControl.ini";
            string Mode = tool.Text.Split(':')[0];
            Config["TestControlFlag"] = int.Parse(Mode);
            IniHelpe.SetValue("TestModel", "TestControlFlag", Mode, path);


        }


        private void 调试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MerryTest.MoreTestAPI.Froms.MoreTestStation.GetStation().Show();
        }

        #endregion

        //###################################################     窗体事件     ！！！！！！！！！！！！！！！！！！！！！！！！！！
        #region  窗体事件大集合

        private void MoreTest_Resize(object sender, EventArgs e)
        {

            if (RefreshingFlag) return;
            var newX = Width;
            var newY = Height;

            uias.UpdateSize(Width, Height, this);
            foreach (DataGridViewTextBoxColumn item in dgv_TestResult.Columns)
            {
                item.Width = Convert.ToInt16((item.Width * newX) / uias.X);
            }

            //foreach (DataGridViewRow item in dgv_TestResult.Rows)
            //{
            //    item.Height = Convert.ToInt16((item.Height * newY) / uias.Y);
            //}
            foreach (ColumnHeader item in lv_TestTable.Columns)
            {
                item.Width = Convert.ToInt16((item.Width * newX) / uias.X);
            }

            uias.X = newX;
            uias.Y = newY;
        }
        private void MoreTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            #region 记录窗体最大化
            IniHelpe.SetValue("Program", "Maximized", this.WindowState == FormWindowState.Maximized ? "1" : "0");
            #endregion
            #region 测试logging
            RunEvent(0, (List<string[]>)Config["ClosedEvent"]);
            #endregion
            Thread.Sleep(50);
            Kill();
        }
        int Barcode;
        private void bt_Loop_Click(object sender, EventArgs e)
        {

            if (loopThread == null)
            {
                if (TestingFlag) return;
                btn_Loop.Text = "Stop Loop";
                int TestNumbers = (int)Config["TestNumbers"];
                Barcode = (int)Config["Barcode"];


                Config["Barcode"] = 10;
                loopThread = new Thread(() =>
                {
                    for (int i = 0; i < TestNumbers;)
                    {
                        Invoke(new Action(() =>
                        {
                            for (int colCount = 0; colCount < dgv_TestResult.Columns.Count; colCount++)
                            {
                                if (!dgv_TestResult[colCount, 1].ReadOnly)
                                {
                                    i++;
                                    Console.WriteLine(i);
                                    string SN = (i).ToString().PadLeft((int)Config["Barcode"], '0');
                                    dgv_TestResult[colCount, 1].Value = $"EN_TEST_{SN}";
                                    EnterKeyDownFlag = false;
                                    StartTestInit(int.Parse(dgv_TestResult.Columns[colCount].Name), 1);
                                }
                            }
                        }));
                        while (TestingFlag) { };
                        Thread.Sleep(500);

                    }
                });
                loopThread.Start();
            }
            else if (loopThread != null)
            {
                btn_Loop.Text = "Loop";
                loopThread.Abort();
                Config["Barcode"] = Barcode;
                loopThread = null;
            }
        }



        #endregion

        private void 主动更新模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowProgess();
            _AllDll.AllDllRun(0, $@"Config_ini", "InitiativeUpdataAllDll", null);


        }

        private void 主动更新机型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowProgess();
            _AllDll.AllDllRun(0, $@"Config_ini", "InitiativeUpdataTestItem", null);

        }

        private void 主动更新模板ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowProgess();
            Config["EngineerMode"] = false;
            _AllDll.AllDllRun(0, $@"Config_ini", "CheckMerryTest", null);
        }
    }
}
