using MerryTest.Entity;
using MerryTest.Froms;
using MerryTest.SingleTestAPI.API;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest
{
    public partial class SingleTest : Form
    {
        public SingleTest()
        {
            InitializeComponent();
        }
        public string[] GetDllInfo()
        {
            string dllname = $"程序名称       ：{Text}";
            string dllfunction = "程序功能说明 ：MECH测试平台";
            string dllHistoryVersion = "历史程序版本：MEVN V23.10.23.0";
            string dllVersion = "当前程序版本：MEVN V23.12.1.0";
            string dllChangeInfo = "程序改动信息： ";
            string dllChangeInfo1 = "V22.1.8.1：优化启动速度，及版本管理";
            string dllChangeInfo2 = "V22.7.13.0：修复刷新功能";
            string dllChangeInfo3 = "V22.7.18.0：刷修功能测试项重复问题修复";
            string dllChangeInfo4 = "V22.8.8.0：更改标准品条码TE_BZP被禁用，更改标准品字符";
            string dllChangeInfo5 = "V22.8.17.0：修复测试完毕后马上刷新的bug";
            string dllChangeInfo6 = "V22.11.14.0：更改右下角测试 Pass 和 Fail 和 Tesing 和 Await";
            string dllChangeInfo7 = "V22.12.16.0：新增主动更新模板和机型的dll,方法编号 3&6 指定单元格灰色";
            string dllChangeInfo8 = "MEVN V23.4.02.0：增加自动化模式控制继电器";
            string dllChangeInfo9 = "MEVN V23.4.21.0：增加自动校准RF程序的功能";
            string dllChangeInfo10 = "MEVN V23.4.21.2：RF自动化调整线程及Config[SN]";
            string dllChangeInfo11 = "MEVN V23.4.21.3：如果工作站（Station）包含机型（Name）就只显示工作站（Station）";
            string dllChangeInfo12 = "MEVN V23.7.06.1：autotest方法修改，增加获取Config[PID,VID]";
            string dllChangeInfo13 = "MEVN V23.8.14.1：修改模板的Logo";
            string dllChangeInfo14 = "MEVN V23.10.23.0：增加管控补偿值的Limit";

            string[] info = { dllname,
                dllfunction,
                dllHistoryVersion,
                dllVersion,
                dllChangeInfo
                ,dllChangeInfo1,dllChangeInfo2,dllChangeInfo3,dllChangeInfo4,dllChangeInfo5,dllChangeInfo6,dllChangeInfo7,dllChangeInfo8,dllChangeInfo9,
                dllChangeInfo10,dllChangeInfo11,dllChangeInfo12,dllChangeInfo13,dllChangeInfo14
            };
            return info;
        }
        /// <summary>
        /// 整个程序变量
        /// </summary>
        public static Dictionary<string, object> Config = new Dictionary<string, object>();
        /// <summary>
        /// 所有Dll储存器
        /// </summary>
        public static AllDLL _AllDll = new AllDLL(Config);
        /// <summary>
        /// 循环测试线程
        /// </summary>
        Thread loopThread;
        /// <summary>
        /// 窗体大小存储器
        /// </summary>
        UIAdaptiveSize uias;
        /// <summary>
        /// 自动测试线程
        /// </summary>
        Thread TestModeThread;
        /// <summary>
        /// 加载中标志位
        /// </summary>
        bool Loadflag;
        bool RefreshFlag;
        /// <summary>
        /// 正在测试标志位
        /// </summary>
        bool Testflag;
        bool TE_BZPFlag = false;
        /// <summary>
        /// 锁
        /// </summary>
        object lock_obj = new object();


        StringBuilder OnceTestLogging = new StringBuilder();//测试日记记录
        public void Form1_Load(object sender, EventArgs e)
        {
            
            //MessageBox.Show(Environment.UserName);
            if (Testflag || RefreshFlag) return;
            //清除旧数据
            RefreshData();
            Task.Run(() => new ProgressBars(Config).ShowDialog());
            /*加载机型DLL*/
            RunEvent((List<string[]>)Config["LoadEvent"]);
           
            //自动测试模式
            if (!_AllDll.Start(new object[] { _AllDll.FormsData, Handle }))
            {
                MessageBox.Show("机型：Start方法异常 程序即将关闭");
                MerryTest_FormClosed(null, null);
            }
            //加载需要Load的DLL           /*获取测试项目*/
            LoadTestModel();
            /*测试项目渲染至界面*/
            RenderingInterface();
            //设置窗体大小 
            SetWinFormSize();
            //检查运行AutoTest模式
            CheckAutoTest();
        }
        public async void tb_BarCodeSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            Config["SN"] = _AllDll.FormsData[0] = tb_BarCodeSN.Text.Trim().ToUpper();
            Thread TestThread = new Thread(() => 
            {
                Invoke(new Action(() =>
                {
                    Test();
                }));
                while (Testflag) { }
            });
            Testflag = true;//正在测试标志位
            TestThread.Start();
        }

        string value = "";
        async void Test()
        {
            Testflag = true;//正在测试标志位
            int Barcode = (int)Config["Barcode"];
            int MesFlag = (int)Config["MesFlag"];
            bool EnsureVisibleFlag = true;
            DateTime StartTestTime = DateTime.Now;//记录时间
            try
            {
                //Config["SN"] = _AllDll.FormsData[0] = tb_BarCodeSN.Text.Trim().ToUpper();
                if(tb_BarCodeSN.Text.Length != 0)
                {
                    Config["SN"] = _AllDll.FormsData[0] = tb_BarCodeSN.Text.Trim().ToUpper();
                }
                Config["TestResultFlag"] = true;//重置测试结果标志
                tb_BarCodeSN.Enabled = false;//条码框锁住
                if (Config.TE_BZP())
                {
                    System.Windows.Forms.MessageBox.Show(this, $"TE_BZP条码已被禁用/ Mã TE_BZP đã bị chặn", "提示", MessageBoxButtons.OK);
                    return;
                }
                if (tb_BarCodeSN.Text.Length != Barcode && !Config.TE_BZP1() && !Config.TE_BZP2() && !Config.TE_BZP3() && !Config.TE_BZP4())
                {
                    System.Windows.Forms.MessageBox.Show(this, $"条码长度不正确条码长度/Độ dài mã SN không chính xác：{tb_BarCodeSN.Text.Length}：限制长度/Độ dài đã thiết lập：{Barcode}", "提示", MessageBoxButtons.OK);
                    return;
                }
                if (Config.TE_BZP1())
                {
                    TE_BZPFlag = true;
                    if ((int)Config["TE_BZPCount"] >= (int)Config["TE_BZPLimit"])
                    {
                        MessageBox.Show($"标准品测试次数已达到上限 / Số lần test của mã test hàng mẫu đã đạt đến giới hạn{(int)Config["TE_BZPCount"]}");
                        return;
                    }
                }
                if (Config.TE_BZP1() || Config.TE_BZP2() || Config.TE_BZP3()|| Config.TE_BZP4())
                    Config["SN"] = _AllDll.FormsData[0] = $"TE_BZP_{Config["SN"]}";
                if (!WireCount()) return;
                //检测SN写在事件里面
                RunEvent((List<string[]>)Config["StartTestEvent"]);
                await Task.Run(() => Thread.Sleep(((int)Config["StartDelay"]) <= 500 ? 500 : (int)Config["StartDelay"]));//测试前延时
                lb_TestsResult.Text = "Testing";
                Application.DoEvents();
                if (!(bool)Config["TestResultFlag"]) return;
                //测试开始事件
                /*开始功能测试*/
                listBoxMessage("");
                listBoxMessage("#--------------------------------------------------          分割线          ------------------------------------------------------#");
                listBoxMessage("#########开始测试###########");
                listBoxMessage($"SN：{Config["SN"]}，标题：{lb_title.Text}，测试模式：{lb_TestMode.Text}");
                if (!_AllDll.StartRun())
                {
                    listBoxMessage("StartRun执行失败");
                    return;
                }
                string TestValue = "";//测试值
                value = "";
                bool flag = false;
                bool Percent_FROM=false;
                foreach (var item in TestitemEntity.ItemData)
                {
                    Config["TestName"] = item.测试项目;//建立给655 660RF自动校准使用
                    if (!(bool)Config["TestResultFlag"] && !item.测试项目.Contains("+")) continue;//即使测试失败任然执行的测试项+
                    listBoxMessage("###########################");
                    listBoxMessage($"开始测试：{item.测试项目}，耳机指令为：{item.耳机指令}，调用方法序号为：{item.MethodId}");
                    //测试项
                    await Task.Run(() =>
                    {
                        if (item.测试项目.Contains("|"))
                        {
                            var arr = item.测试项目.Split('|');
                            if (arr.Length == 3) _AllDll.Run("Switch", "Run", new object[] { new object[] { $@"dllname=Switch&跳转继电器分={arr[2]}" } });
                            Thread.Sleep(int.Parse(arr[1]));//项目前延时
                        }
                        //
                        if (item.测试项目.Contains("RF_POWER"))
                        {
                            if (item.MethodId == 6) item.MethodId = 5;
                            if (item.MethodId == 3) item.MethodId = 2;
                        }
                        if (item.测试项目.Contains("RF_POWER") && Config["SN"].strContains(Config["TE_BZP4"].ToString()))
                        {
                            if (item.MethodId == 5) item.MethodId = 6;//条码包含RF_JZ就不对比
                            if (item.MethodId == 2) item.MethodId = 3;
                        }
                        if (item.测试项目.Contains("PERCENT"))
                        {
                            if (item.MethodId == 6) item.MethodId = 5;
                            if (item.MethodId == 3) item.MethodId = 2;
                        }
                        if (item.测试项目.Contains("PERCENT") && Config["SN"].strContains(Config["TE_BZP4"].ToString()))
                        {
                            if (item.MethodId == 5) item.MethodId = 6;//条码包含RF_JZ就不对比
                            if (item.MethodId == 2) item.MethodId = 3;
                        }

                        if (!TestMethod.Test(item, out TestValue)) Config["TestResultFlag"] = false;
                    });
                   
                    //AddRF.Add(TestValue);
                    string a = Config["SN"].ToString();
                    string b = Config["TE_BZP4"].ToString();
                    if (item.测试项目.Contains("RF_POWER") && (Config["SN"].strContains(Config["TE_BZP4"].ToString())))
                    {
                        //获取需要校准的测试项目（测试名称，测试结果，指令）；循环运行完之后，如果Flag=True就开始进行校准。
                        flag = true;
                        AddRF.Add(item.测试项目 + "*" + TestValue+"*"+item.耳机指令);
                    }
                    if (item.测试项目.Contains("PERCENT") && (Config["SN"].strContains(Config["TE_BZP4"].ToString())))
                    {
                        //获取需要校准的测试项目（测试名称，测试结果，指令）；循环运行完之后，如果Flag=True就开始进行校准。
                        Percent_FROM = true;
                        AddRF.Add(item.测试项目 + "*" + TestValue + "*" + item.耳机指令);
                    }
                    listBoxMessage($"测试值：{TestValue}，测试结果：{Config["TestResultFlag"]}");
                    //界面回馈结果
                    if (!item.测试项目.Contains("-"))//需要不显示在界面的测试项-
                    {
                        bool TestResultFlag = (bool)Config["TestResultFlag"];
                        int Row = item.编号 - 1;//界面测试项
                        lv_TestItem.Items[Row].SubItems[5].Text = TestValue;
                        lv_TestItem.Items[Row].SubItems[5].BackColor = TestResultFlag ? Color.Green : Color.Red;
                        lv_TestItem.Items[Row].SubItems[6].Text = TestResultFlag.ToString();
                        lv_TestItem.Items[Row].SubItems[6].BackColor = TestResultFlag ? Color.Green : Color.Red;
                        if (EnsureVisibleFlag)
                        {
                            lv_TestItem.EnsureVisible(Row);//保存项目可见
                            Application.DoEvents();
                            EnsureVisibleFlag = TestResultFlag;
                        }
                    }
                }
                if (Percent_FROM)
                {
                    //打开校准窗口，开始进行校准
                    PercentCalibration Percent = new PercentCalibration(AddRF);
                    if (Percent.ShowDialog() != DialogResult.Yes)
                    {
                        //如果没有校准就显示在补偿的值
                        //补偿工具事件ToolStripMenuItem_Click(null, null);
                        Config["TestResultFlag"] = false;
                    }
                    //已经校准完成
                    AddRF = new List<string>();//把需要校准的项目净空掉
                }
                if (flag)
                {
                    //打开校准窗口，开始进行校准
                    RFCalibration rf = new RFCalibration(AddRF);
                    if (rf.ShowDialog() != DialogResult.Yes)
                    {
                        //如果没有校准就显示在补偿的值
                        //补偿工具事件ToolStripMenuItem_Click(null, null);
                        Config["TestResultFlag"] = false;
                    }
                    //已经校准完成
                    AddRF = new List<string>();//把需要校准的项目净空掉
                }
                //获取所有测试项目的数据
                listBoxMessage("########################################");
                Config["TestTime"] = DateTime.Now - StartTestTime;
                Config["OnceTestLogging"] = OnceTestLogging.ToString();
                Config["TestTable"] = lv_TestItem;
                OnceTestLogging.Clear();
                //测试结束事件
                RunEvent((List<string[]>)Config["TestEndEvent"]);
                //保存测试数据
                listBoxMessage($"#####测试结束，最终测试结果为:【{Config["TestResultFlag"]}】######");
                //最终结果渲染
                if ((bool)Config["TestResultFlag"])
                {
                    int TestPassCount;
                    lb_TestsResult.BackColor = Color.Green;
                    lb_TestsResult.Text = "Pass";
                    Config["TestPassCount"] = TestPassCount = (int)Config["TestPassCount"] + 1;
                    lb_TestPassCount.Text = $"测试成功次数：   {TestPassCount}";
                    IniHelpe.SetValue("Program", "TestPassCount", $"{TestPassCount}");
                    if (TE_BZPFlag)
                    {
                        TE_BZPFlag = false;
                        int TE_BZPCount = (int)Config["TE_BZPCount"] + 1;
                        IniHelpe.SetValue("Program", "TE_BZPCount", $"{TE_BZPCount}");
                        Config["TE_BZPCount"] = TE_BZPCount;
                    }
                }
                else
                {
                    lb_TestsResult.BackColor = Color.Red;
                    lb_TestsResult.Text = "Fail";
                }
                //
                //Config["MoveFile"] = true;


                Application.DoEvents();
            }
            finally
            {
                Config["StartTest"] = false;
                Testflag = false;
                new Thread(Init).Start();
                tb_BarCodeSN.Enabled = true;
                tb_BarCodeSN.Clear();
                tb_BarCodeSN.Focus();
                
                Thread.Sleep(50);
                WindowsAPI.SetForegroundWindow(Handle);
            }
        }
        List<string> AddRF = new List<string>();
       

        #region 自定义方法区
        public void RefreshData()
        {
            Loadflag = false;
            RefreshFlag = true;
            Config.Clear();
            _AllDll.Dispose();
            lv_TestItem.Items.Clear();
            TestitemEntity.ItemData.Clear();
            tb_ProgramInfo.Clear();
            Config.Add("admin", this);
            Config.Add("adminPath", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Config.Add("LoadMessagebox", "程序启动");
            Config.Add("LoadMessageboxFlag", false);
            //获取MES部分的Config信息
            _AllDll.Run("Config_ini", "MesConfig", null);
        }
        public void RunEvent(List<string[]> method_parameter)
        {
            foreach (var item in method_parameter)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                bool isResult = false;
                string error = "";
                try
                {
                    //没有参数情况执行
                    if (item.Length == 2)
                    {
                        Config["LoadMessagebox"] = $"触发模块：{item[0]}，触发方法：{item[1]}";
                        _AllDll.EventRun(item[0], item[1], null, out isResult, out error);

                    }
                    //有参数的情况执行
                    else if (item.Length > 2)
                    {
                        List<object> parameter = new List<object>();
                        for (int i = 2; i < item.Length; i++) parameter.Add(item[i]);
                        Config["LoadMessagebox"] = $"启动{item[0]}";
                        _AllDll.EventRun(item[0], item[1], parameter.ToArray(), out isResult, out error);
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText($@"{Config["adminPath"]}\Log\错误信息{DateTime.Now:MM_dd}.txt", $"{DateTime.Now}\r\n{ex}\r\n\r\n", Encoding.UTF8);
                    MessageBox.Show(this, $"{item[0]}类：{item[1]}方法:加载失败\r\n{ex}");
                }
                if (isResult) MessageBox.Show(this, $"${item[0]}类：{item[1]}加载失败\r\n{error}");
                Console.WriteLine($"{item[0]}类：{item[1]}方法:运行时间：{stopwatch.ElapsedMilliseconds}");
                //File.AppendAllText($@"{Config["adminPath"]}\Log\LogTest{DateTime.Now:MM_dd}.txt", $"{item[0]}类：{item[1]}方法:运行时间：{stopwatch.ElapsedMilliseconds}", Encoding.UTF8);
                stopwatch.Stop();
            }
            return;
        }

        /// <summary>
        /// 窗体大小设置
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
                };
                uias.SetInitSize(this);
            }
            if ((bool)Config["TopMostFlag"])
            {
                this.TopMost = true;
                CMS_TopMostFlag.Text = "取消窗体置顶层";
            }
            else
            {
                this.TopMost = false;
                CMS_TopMostFlag.Text = "窗体置顶层";
            }
            if ((bool)Config["Maximized"])
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if ((bool)Config["MinimumSize"])
            {
                this.WindowState = FormWindowState.Normal;
                Thread.Sleep(100);
                窗体缩小ToolStripMenuItem_Click(null, null);
            }
        }

        #endregion


        /// <summary>
        /// 自动测试线程标志位
        /// </summary>
        public void LoadTestModel()
        {
            if (!(bool)Config["CheckDevice"] && !(bool)Config["TestModelFlag"])
            {
                ProgramOpen();
                return;
            }
            //清理线程
            if (TestModeThread != null)
            {
                if (TestModeThread.IsAlive) TestModeThread.Abort();
                Thread.Sleep(500);
                TestModeThread = null;
            }
            TestModeThread = new Thread(new ParameterizedThreadStart(autoTest));
            /*测试模式*/
            if ((bool)Config["TestModelFlag"])
            {
                Config["Barcode"] = 0;
                tb_BarCodeSN.Visible = false;
                TestModeThread.Start(true);
            }
            else
            {
                ProgramOpen();
                TestModeThread.Start(false);
            }
        }


        public void CheckAutoTest()
        {
            if (!(Convert.ToInt32(Config["AutoTest"]) < 1))
            {
                try
                {
                    _AllDll.Run("AutoTest", "AutoTest", null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
               
                Thread t = new Thread(AutoTest);
                t.Start();
            }
        }
        //Begin
        public void AutoTest()
        {
          
            Config["StartTest"] = false;
            Config["MoveFile"] = false;
            while (true)
            {
                //tb_BarCodeSN.Enabled = false;
                if ((bool)Config["StartTest"]&&Testflag==false)
                {
                    Test();
                   
                  
                }
                Thread.Sleep(100);
            }
        }
        //End
        /// <summary>
        /// 自动测试方法体
        /// </summary>
        /// <param name="flag_On"></param>
        public void autoTest(object flag_On)
        {
            bool flag_on = (bool)flag_On;
            string Pid = (string)Config["Pid"];
            string Vid = (string)Config["Vid"];
            string _Pid = (string)Config["_Pid"];
            string _Vid = (string)Config["_Vid"];
            bool flag = false;
            if (flag_on)
            {
                while (true)
                {
                    Pid = (string)Config["Pid"];
                    Vid = (string)Config["Vid"];
                    _Pid = (string)Config["_Pid"];
                    _Vid = (string)Config["_Vid"];
                    GetHandle.Path = "";
                    GetHandle.getPath(Pid, Vid, "");
                    if (GetHandle.Path != "")
                    {
                        if (!flag)
                        {
                            CheckDevice(!flag);
                            Thread.Sleep(50);
                            //跨线程使用UI控件
                            Invoke(new Action<object, KeyEventArgs>(tb_BarCodeSN_KeyDown), new object[] { tb_BarCodeSN, new KeyEventArgs(Keys.Enter) });
                            while (Testflag) Thread.Sleep(1);
                            flag = true;
                        }
                    }
                    else
                    {
                        if (flag)
                        {
                            CheckDevice(!flag);
                            flag = false;
                        }
                    }
                    Thread.Sleep(250);
                }

            }
            else
            {
                while (true)
                {
                    GetHandle.headsetpath[0] = "";
                    GetHandle.donglepath[0] = "";
                    GetHandle.GetHidDevicePath(Pid, Vid, _Pid, _Vid);
                    Invoke(new Action(() =>
                    {
                        lb_Device_H.BackColor = GetHandle.headsetpath[0] != "" ? Color.Chartreuse : SystemColors.ControlLight;
                        lb_Device_D.BackColor = GetHandle.donglepath[0] != "" ? Color.Chartreuse : SystemColors.ControlLight;
                    }));
                    Thread.Sleep(250);
                }
            }
        }

        /// <summary>
        /// 检测Device方法体
        /// </summary>
        /// <param name="deviceFlag"></param>
        public void CheckDevice(bool deviceFlag)
        {
            Invoke(new Action(() =>
            {
                lb_Device_D.BackColor = deviceFlag ? Color.Chartreuse : System.Drawing.SystemColors.ControlLight;
                lb_Device_H.BackColor = deviceFlag ? Color.Chartreuse : System.Drawing.SystemColors.ControlLight;
            }));

        }

        /// <summary>
        /// 启用程式
        /// </summary>
        public void ProgramOpen()
        {
            tb_BarCodeSN.Enabled = true;
            tb_BarCodeSN.Visible = true;
            tb_BarCodeSN.Clear();
            tb_BarCodeSN.Focus();
            lb_TestsResult.Text = "Await";
            lb_TestsResult.BackColor = SystemColors.Control;
            Testflag = false;
        }

        void RenderingInterface()
        {
            string[] Dllname = new string[0];
            //程序界面渲染
            RenderingTsetTable(ref Dllname);
            //执行所需要加载信息的Dll
            LoadDllName(Dllname);
            //设置版本信息弹框
            SetProgrameInfo();

            //设置头部标题
            SetWinformTitle();
            //显示料号里面的信息
            ShowProgramInformation();
            //加载完毕标识
            RefreshFlag = false;
            Loadflag = true;
            Config["LoadMessageboxFlag"] = true;



        }
        public void RenderingTsetTable(ref string[] LoadDllName)
        {
            List<string> loadDllName = new List<string>();
            int number = 0;
            foreach (string[] item in (List<string[]>)Config["TestItem"])
            {
                number = item[1].Contains("-") ? number : number + 1;
                TestitemEntity t1 = new TestitemEntity
                {
                    编号 = item[1].Contains("-") ? 0 : number,
                    测试项目 = item[1],
                    单位 = item[2],
                    数值下限 = item[3],
                    数值上限 = item[4],
                    MethodId = int.Parse(item[5]),
                    耳机指令 = item[6]
                };
                if (t1.MethodId >= 4) loadDllName.Add(t1.耳机指令);
                /*赋值给测试实体*/
                TestitemEntity.ItemData.Add(t1);
                if (item[1].Contains("|")) item[1] = item[1].Split('|')[0];
                if (item[1].Contains("-")) continue;
                string[] str = { number.ToString(), item[1], item[2], item[3], item[4], "", "" };
                lv_TestItem.Items.Add(new ListViewItem(str));
                int i = number - 1;
                lv_TestItem.Items[i].UseItemStyleForSubItems = false;
                if (t1.MethodId == 3 || t1.MethodId == 6)
                {
                    lv_TestItem.Items[i].SubItems[2].BackColor = Color.Gainsboro;
                    lv_TestItem.Items[i].SubItems[3].BackColor = Color.Gainsboro;
                    lv_TestItem.Items[i].SubItems[4].BackColor = Color.Gainsboro;
                }
            }
            LoadDllName = loadDllName.ToArray();
        }
        public void LoadDllName(string[] LoadDllName)
        {
            if (lv_TestItem.Items.Count == 0) MessageBox.Show($@"机型：{(string)Config["Name"]} 站别：{(string)Config["Station"]} 无测试项目");
            //加载程序所需运行信息
            foreach (var item in LoadDllName)
            {
                try
                {
                    string name = item.Split('&')[0].Split('=')[1];
                    _AllDll.LoadDll(name, $@"{Config["adminPath"]}\AllDLL\{name}\{name}.dll");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"指令出错：{item}\r\n{ex.ToString()}");
                }
            }
        }
        public void SetProgrameInfo()
        {
            string computerName = Environment.MachineName;
            List<string> ipv4 = new List<string>();
            foreach (var ipa in Dns.GetHostAddresses(computerName)) if (ipa.AddressFamily == AddressFamily.InterNetwork) ipv4.Add(ipa.ToString());
            tb_ProgramInfo.Text += $"作业信息=>\r\n";
            tb_ProgramInfo.Text += $"工单：{Config["Works"]}\r\n";
            tb_ProgramInfo.Text += $"料号：{Config["OrderNumberInformation"]}\r\n";
            tb_ProgramInfo.Text += $"电脑名称：{computerName}\r\n";
            ipv4.ForEach(ip => tb_ProgramInfo.Text += $"Ipv4：{ip}\r\n");
            tb_ProgramInfo.Text += $"\r\n";
            tb_ProgramInfo.Text += $"程序信息=>\r\n";
            foreach (var item in _AllDll.dllInfo[$"{Config["Name"]}"])
            {
                if (!item.Contains("当前")) continue;
                tb_ProgramInfo.Text += $"{Config["Name"]}： 版本  ：{item.Split('：')[1]}\r\n";
                break;
            }
            foreach (string item in GetDllInfo())
            {
                if (!item.Contains("当前")) continue;
                tb_ProgramInfo.Text += $"{this.Text}： 版本  ：{item.Split('：')[1]}\r\n";
                break;
            }
            foreach (var keys in _AllDll.dllInfo)
            {
                if (Config["Name"].strContains(keys.Key)) continue;
                foreach (var item in keys.Value)
                {
                    if (!item.Contains("当前")) continue;
                    tb_ProgramInfo.Text += $"{keys.Key}： 版本  ：{item.Split('：')[1]}\r\n";
                    break;
                }
            }
        }
        public void SetWinformTitle()
        {
            string lableText = "单机模式";
            string SystematicName = (string)Config["SystematicName"];

            if ((int)Config["MesFlag"] > 0)
            {
                lableText = $"MES模式:{SystematicName}";
            }
            else if ((bool)Config["GetBDFlag"])
            {
                lableText = $"BD模式:{SystematicName}";
            }
            string EngineerMode = (bool)Config["EngineerMode"] ? "工程" : "量产";
            lableText = $"{EngineerMode}{lableText}";
            lb_TestMode.Text = $@"{lableText}";



            lb_WireRod.Text = $"线材使用次数：   {Config["WireCount"]}";
            lb_TestPassCount.Text = $"测试成功次数：   {Config["TestPassCount"]}";
            //标题
            //如果Station包含Name就把Name隐藏
            if (((string)Config["Station"]).strContains((string)Config["Name"]))
            {
                lb_title.Text = $"{Config["Station"]}";
            }
            else
            {
                lb_title.Text = $"{Config["Name"]}  {Config["Station"]}";
            }

            CMS_TopMostFlag.Text = (this.TopMost = (bool)Config["TopMostFlag"])
     ? "取消窗体置顶层"
     : "窗体置顶层";
        }
        public void ShowProgramInformation()
        {
            ProgramInformation infoForms = ProgramInformation.ShowPicture(Config);
            infoForms.Show();
            infoForms.ShowEvent();
        }
        public bool WireCount()
        {
            int WireCount = (int)Config["WireCount"];
            int WireCountEnd = (int)Config["WireCountEnd"];
            int WireIncrease = (int)Config["WireIncrease"];
            WireCount += WireIncrease;
            if (WireCount >= WireCountEnd)
            {
                if (MessageBox.Show(this, "线材使用次数已经达到上限，请更换线材再进行测试/Số lần sử dụng của dây cáp test đã đạt đến giới hạn, vui lòng đổi dây cáp test rồi tiến hành kiểm tra", "线材提示", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) != DialogResult.Yes)
                {
                    return false;
                }
                IniHelpe.SetValue("Program", "WireCount", "0");
                WireCount = 0;
            }
            IniHelpe.SetValue("Program", "WireCount", WireCount.ToString());
            Config["WireCount"] = WireCount;
            lb_WireRod.Text = $@"线材使用次数：   {Config["WireCount"]}";
            return true;
        }
        public void Init()
        {
            int z = 100;
            int StopDelay = (int)Config["StopDelay"];
            while (!Testflag && z < StopDelay)
            {
                Thread.Sleep(100);
                z += 100;
            }
            Invoke(new Action(() =>
            {

                lb_TestsResult.Text = "Await";
                lb_TestsResult.BackColor = Color.Transparent;
                for (int i = 0; i < lv_TestItem.Items.Count; i++)
                {
                    lv_TestItem.Items[i].SubItems[5].BackColor = Color.Transparent;
                    lv_TestItem.Items[i].SubItems[6].BackColor = Color.Transparent;
                    lv_TestItem.Items[i].SubItems[5].Text = "";
                    lv_TestItem.Items[i].SubItems[6].Text = "";
                }
            }));
            /*界面结果初始化*/

            Application.DoEvents();
        }
        public string listBoxMessage(string text)
        {
            string Msg = $"{DateTime.Now}: {text}";

            OnceTestLogging.Append($"{Msg}\r\n");
            Invoke(new Action(() =>
            {
                tb_MsgeInfo.Items.Add(Msg);
                if (tb_MsgeInfo.Items.Count > 0)
                {
                    tb_MsgeInfo.SelectedIndex = tb_MsgeInfo.Items.Count - 1;
                }
            }));

            return Msg;
        }


        public void btn_Loop_Click(object sender, EventArgs e)
        {
            if (loopThread == null)
            {
                if (Testflag) return;
                btn_Loop.Text = "Stop Loop";
                int TestNumbers = Convert.ToInt32(Config["TestNumbers"]);
                string TXTSN = tb_BarCodeSN.Text;
                bool IsTXTSN = (int)Config["Barcode"] == TXTSN.Length;
                loopThread = new Thread(() =>
                {
                    for (int i = 0; i < TestNumbers; i++)
                    {
                        Invoke(new Action(() =>
                        {
                            string SN = IsTXTSN ? TXTSN : $"EN_TEST_{(i).ToString().PadLeft((int)Config["Barcode"], '0')}";
                            tb_BarCodeSN.Text = SN;
                            tb_BarCodeSN_KeyDown(tb_BarCodeSN, new KeyEventArgs(Keys.Enter));
                        }));
                        while (Testflag) { };
                    }
                });
                loopThread.Start();
            }
            else if (loopThread != null)
            {
                btn_Loop.Text = "Loop";
                loopThread.Abort();
                loopThread = null;
            }
        }

        private void MerryTest_Resize(object sender, EventArgs e)
        {
            if (!Loadflag) return;
            var newx = Width;
            uias.UpdateSize(Width, Height, this);
            foreach (ColumnHeader item in lv_TestItem.Columns)
            {
                item.Width = Convert.ToInt16((item.Width * newx) / uias.X);
            }
            uias.X = newx;
        }

        public void MerryTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            TestModeThread?.Abort();
            Thread.Sleep(50);
            #region 记录窗体最大化
            IniHelpe.SetValue("Program", "Maximized", this.WindowState == FormWindowState.Maximized ? "1" : "0");
            #endregion
            #region 测试logging
            RunEvent((List<string[]>)Config["ClosedEvent"]);
            #endregion
            Thread.Sleep(50);
            Process.GetCurrentProcess().Kill();
        }


        #region 右键菜单栏事件

        public void 窗体缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            TopMost = true;
            BringToFront();
            int x = System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Size.Width / 2;
            int y = System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Size.Height / 2;
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Location = (Point)new Size(x, y);         //窗体的起始位置为(x,y)
            this.Width = 300;
            this.Height = 300;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            tb_BarCodeSN.BringToFront();
            tb_BarCodeSN.Location = (Point)new Size(1, 1);
            tb_BarCodeSN.Size = new Size(this.Width - 5, Convert.ToInt32(this.Height * 0.1));
            tb_BarCodeSN.Font = new Font("新細明體", 25);
            lb_TestsResult.BringToFront();
            lb_TestsResult.Location = (Point)new Size(1, Convert.ToInt32(this.Height * 0.15));
            lb_TestsResult.Size = new Size(this.Width - 10, this.Height - 50);
            lb_TestsResult.Font = new Font("新細明體", 50);
            lb_TestsResult.Text = "Await";
            IniHelpe.SetValue("Program", "MinimumSize", "1");
        }

        public void 窗体放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = System.Windows.Forms.SystemInformation.WorkingArea.Width / 8;
            int y = System.Windows.Forms.SystemInformation.WorkingArea.Height / 7;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Height = uias.Height;
            this.Width = uias.Width;
            this.Location = (Point)new Size(x, y);
            this.Text = uias.FormsName;
            this.TopMost = false;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            IniHelpe.SetValue("Program", "MinimumSize", "0");
        }


        private void CMS_TopMostFlag_Click(object sender, EventArgs e)
        {
            if (!this.TopMost)
            {
                this.TopMost = true;
                CMS_TopMostFlag.Text = "取消窗体置顶层";
                IniHelpe.SetValue("Program", "TopMostFlag", "1");
            }
            else if (this.TopMost)
            {
                this.TopMost = false;
                CMS_TopMostFlag.Text = "窗体置顶层";
                IniHelpe.SetValue("Program", "TopMostFlag", "0");
            }
        }


        #endregion



        #region 菜单栏事件
        public void 登录ToolStripMenuItem_Click(object sender, EventArgs e)
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
            Process.Start(path);
        }
        public void 插件ToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (!Loadflag) return;
            List<ToolStripMenuItem> list = new List<ToolStripMenuItem>();

            #region 测试模块信息加载
            关于DllModeToolStripMenuItem.DropDownItems.Clear();

            #region 机型模块信息加载
            string TypePath = $"{Config["adminPath"]}\\TestItem\\{Config["Name"]}";
            string TypeName = Path.GetFileName(TypePath);
            ToolStripMenuItem tool = new ToolStripMenuItem();
            tool.Text = TypeName;
            tool.Name = TypeName;
            tool.Font = new System.Drawing.Font("黑体", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tool.Size = new System.Drawing.Size(224, 26);
            tool.Click += new System.EventHandler(this.关于DllModule_Click);
            list.Add(tool);
            #endregion
            #region  接口插件模块加载
            foreach (var path in _AllDll.dllInfo)
            {
                string Name = Path.GetFileName(path.Key);
                if (!File.Exists($@"{Config["adminPath"]}\AllDLL\{Name}\{Name}.dll")) continue;

                tool = new ToolStripMenuItem();
                tool.Text = Name;
                tool.Name = Name;
                tool.Font = new System.Drawing.Font("黑体", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                tool.Size = new System.Drawing.Size(224, 26);
                tool.Click += new System.EventHandler(this.关于DllModule_Click);
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
        List<ToolStripMenuItem> 文档获取(string path, List<ToolStripMenuItem> tools)
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


        /// <summary>
        /// 循环测试次数设定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void 循环测试次数StripMenuItem2_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tool = (ToolStripMenuItem)sender;
            string numbers = tool.Text == "N+" ? Interaction.InputBox("输入循环测试次数", "提示", "10", -1, -1) : tool.Text;
            Config["TestNumbers"] = int.Parse(numbers);
            Config["LoopFlag"] = btn_Loop.Visible = btn_Loop.Enabled = true;
        }

        public void 文档点击事件(object sender, EventArgs e)
        {
            string dllName = ((ToolStripMenuItem)sender).Tag.ToString();
            using (Process.Start(dllName)) { }

        }
        public void 关于DllModule_Click(object sender, EventArgs e)
        {
            string dllName = ((ToolStripMenuItem)sender).Name;

            new Heip(_AllDll.dllInfo[dllName], this.Location, this.Size).Show();
        }

        public void 年费VID事件(object sender, EventArgs e)
        {
            if (!Loadflag) return;

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

        private void Soundcheck路径设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tool = (ToolStripMenuItem)sender;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = $"所有文件(*.{tool.Text})|*.{tool.Text}";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] path = tool.Tag.ToString().Split('=');
                Config[path[1]] = dialog.FileName;
                IniHelpe.SetValue(tool.Tag.ToString().Split('=')[0], tool.Tag.ToString().Split('=')[1], dialog.FileName);
            }

        }

        private void 打印机ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void 条码长度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Barcode = Interaction.InputBox("输入条码框条码长度", "提示", $"{Config["Barcode"]}", -1, -1);
            if (Barcode.Trim().Length < 1) return;
            Config["Barcode"] = int.TryParse(Barcode, out int result) ? result : Config["Barcode"];
            IniHelpe.SetValue("Config", "Barcode", result.ToString());
        }

        private void 调试ToolStripMenuItem_Click(object sender, EventArgs e) => Station.GetStation().Show();
        private void 补偿工具事件ToolStripMenuItem_Click(object sender, EventArgs e) => new MerryTest.SingleTestAPI.SingleTestAddTool.AddTool().Run(Config);


        private void 机型配置ToolStripMenuItem_Click(object sender, EventArgs e) => _AllDll.Run("TypeNameConfig", $@"{Config["adminPath"]}\AllDLL\MenuStrip\TypeNameConfig\TypeNameConfig.dll", "Run", null);

        private void 程序配置ToolStripMenuItem_Click(object sender, EventArgs e) => _AllDll.Run("Configuration", $@".\AllDLL\MenuStrip\Configuration\Configuration.dll", "Run", null);
        private void 关于MerryTestToolStripMenuItem_Click(object sender, EventArgs e) => new Heip(GetDllInfo(), this.Location, this.Size).Show();
        public void 切换机型事件(object sender, EventArgs e)
        {
            ToolStripMenuItem tool = (ToolStripMenuItem)sender;
            string[] strs = tool.Text.Split('/');
            IniHelpe.SetValue("Config", "NAME", strs[0]);
            IniHelpe.SetValue("Config", "Station", strs[1].Substring(0, strs[1].Length - 4));
            Form1_Load(null, null);
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

        private void cOM设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string RT550Port = Interaction.InputBox("RT550串口设置", "提示", $"{Config["RT550Port"]}", -1, -1);
            if (RT550Port.Trim().Length > 0)
            {
                Config["RT550Port"] = RT550Port;
                IniHelpe.SetValue("Visa", "RT550Port", $"{Config["RT550Port"]}");
            }
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1_Load(null, null);
        }
        private void 切换到连扳ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IniHelpe.SetValue("Flag", "MoreTestMode", "1");
            Process ps = new Process();
            ps.StartInfo.FileName = Application.ExecutablePath;
            ps.Start();
            Process.GetCurrentProcess().Kill();
        }


        private void 显示字典的值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ConfigDictionary(Config).Show();
        }
        #endregion





        private void 主动更新模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Task.Run(() => new ProgressBars(Config).ShowDialog());
            _AllDll.Run("Config_ini", $@"{Config["adminPath"]}\AllDLL\Config_ini\Config_ini.dll", "InitiativeUpdataAllDll", null);

        }

        private void 主动下载机型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => new ProgressBars(Config).ShowDialog());
            _AllDll.Run("Config_ini", $@"{Config["adminPath"]}\AllDLL\Config_ini\Config_ini.dll", "InitiativeUpdataTestItem", null);
        }

        private void 主动更新模板ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Config["EngineerMode"] = false;
            Task.Run(() => new ProgressBars(Config).ShowDialog());
            _AllDll.Run("Config_ini", $@"{Config["adminPath"]}\AllDLL\Config_ini\Config_ini.dll", "CheckMerryTest", null);

        }

    }
}
