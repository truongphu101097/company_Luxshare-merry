using MerryTest.Entity;
using MerryTest.Froms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.MoreTestAPI
{
    internal static class MoreProperty
    {

        public static Thread loopThread;
        /// <summary>
        /// 自己写的按键监控事件
        /// </summary>
        public static KeyboardHook KeyDownEventClass = new KeyboardHook();
        /// <summary>
        /// 正在加载标志位
        /// </summary>
        public static bool LoadingFlag;
        /// <summary>
        /// 正在刷新标志位
        /// </summary>
        public static bool RefreshingFlag;
        /// <summary>
        /// 正在测试标志位
        /// </summary>
        public static bool TestingFlag;
        /// <summary>
        /// 标识单元格是否可以渲染
        /// </summary>
        public static bool RenderCellFlag = false;
        /// <summary>
        /// 标识单元格是否需要跳转
        /// </summary>
        public static bool SkipCellFlag = false;
        /// <summary>
        /// 锁
        /// </summary>
        public static object lock_obj = new object();
        /// <summary>
        /// 窗体大小存储器
        /// </summary>
        public static UIAdaptiveSize uias;
        /// <summary>
        /// 整个程序变量
        /// </summary>
        public static Dictionary<string, object> Config = new Dictionary<string, object>();
        /// <summary>
        /// 所有Dll的控制对象
        /// </summary>
        public static AllDLLs _AllDll = new AllDLLs();
        /// <summary>
        /// 所有所有测试项目（包括隐藏项目）
        /// </summary>
        public static List<TestitemEntity> testItem = TestitemEntity.ItemData;
        /// <summary>
        /// 连扳模板特有的多线程参数
        /// </summary>
        public static Dictionary<int, Dictionary<string, string>> MoreTestControl;
        /// <summary>
        /// 线程是否正在测试的状态 1代表正常测试，2代表没有在测试
        /// </summary>
        public static Dictionary<int, int> TestFlags = new Dictionary<int, int>();
        /// <summary>
        /// SN的集合
        /// </summary>
        public static Dictionary<int, string> snList = new Dictionary<int, string>();
        /// <summary>
        /// 机型Dll的类
        /// </summary>
        public static TestTypeDll myDll = new TestTypeDll();
        /// <summary>
        /// 成功按键Enter标志位
        /// </summary>
        public static bool EnterKeyDownFlag = false;
        /// <summary>
        /// 加载机型Dll标识符号
        /// </summary>
        public static bool isLoadMydll = false;
        /// <summary>
        /// Log的记录
        /// </summary>
        public static Dictionary<int, List<string>> LogList = new Dictionary<int, List<string>>();










        /// <summary>
        /// 记录测试日记
        /// </summary>
        /// <param name="TestID"></param>
        /// <param name="MSG"></param>
        /// <returns></returns>
        public static string RecordTheLog(int TestID, string MSG)
        {
            lock (LogList)
            {
                string msg = $"{DateTime.Now}: {MSG}\r\n";
                LogList[TestID].Add(msg);
                return msg;
            }

        }
        /// <summary>
        /// 加载AllDLL事件处理器
        /// </summary>
        /// <param name="TestID"></param>
        /// <param name="method_parameter"></param>
        public static void RunEvent(int TestID, List<string[]> method_parameter, Dictionary<string, object> onceConfig = null)
        {
            foreach (var item in method_parameter)
            {
                string error = null;
                bool isResult = false;
                Exception exc = new Exception();
                Config["LoadMessagebox"] = $"触发模块：{item[0]}，触发方法：{item[1]}";
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                try
                {
                    //没有参数情况执行
                    if (item.Length == 2)
                    {
                        if (onceConfig != null)
                            _AllDll.EnventAllDllRun(TestID, item[0], "OnceConfigInterface", new object[] { onceConfig }, out _, out _);
                        _AllDll.EnventAllDllRun(TestID, item[0], item[1], null, out isResult, out exc);
                    }
                    //有参数的情况执行
                    else if (item.Length > 2)
                    {
                        List<object> parameter = new List<object>();
                        if (onceConfig != null)
                            _AllDll.EnventAllDllRun(TestID, item[0], "OnceConfigInterface", new object[] { onceConfig }, out _, out _);
                        for (int i = 2; i < item.Length; i++) parameter.Add(item[i]);
                        _AllDll.EnventAllDllRun(TestID, item[0], item[1], parameter.ToArray(), out isResult, out exc);
                    }
                }
                catch (Exception ex)
                {
                    exc = ex;
                    isResult = true;
                }
                if (isResult) MessageboxShow($"$线程：{TestID}--dll：{item[0]}--方法：{item[1]}：加载失败\r\n{error}");
                stopwatch.Stop();
            }
            return;
        }
        /// <summary>
        /// 根据测试项目加载测试所需要的dll
        /// </summary>
        public static void LoadAllDll()
        {
            List<string[]> testitem = (List<string[]>)Config["TestItem"];
            foreach (var Item in testitem)
            {
                if (int.Parse(Item[5]) >= 4)
                {
                    string dllName = Item[6].Split('&')[0].Split('=')[1];
                    foreach (var AllThread in MoreTestControl)
                    {
                        string Result = _AllDll.LoadDll(AllThread.Key, dllName, $@"{Config["adminPath"]}\AllDLL\{dllName}\{dllName}.dll");
                        if (Result.Contains("False")) MessageboxShow($"指令出错：{Item[6]}\r\n{Result}");
                    }
                }

            }

        }
        /// <summary>
        /// 加载机型dll
        /// </summary>
        /// <returns></returns>
        public static bool LoadTestType()
        {
            isLoadMydll = false;
            List<string[]> testitem = (List<string[]>)Config["TestItem"];
            foreach (var item in testitem)
                if (int.Parse(item[5]) <= 3)
                {
                    isLoadMydll = true;
                    break;
                }
            if (!isLoadMydll) return true;
            myDll.dllName = (string)Config["Name"];
            myDll.Config = Config;
            foreach (var item in MoreTestControl)
                if (!myDll.Start(item.Key))
                {
                    MessageboxShow("启动机型Start方法返回False，禁止启动,程序即将关闭");
                    Kill();
                }
            return true;
        }
        /// <summary>
        /// 读取测试计划的txt
        /// </summary>
        public static void LoadTestItem()
        {
            List<string[]> testitem = (List<string[]>)Config["TestItem"];
            int number = 0;
            foreach (var item in testitem)
            {
                number = item[1].Contains("-") ? number : number + 1;
                testItem.Add(new TestitemEntity()
                {
                    编号 = item[1].Contains("-") ? 0 : number,
                    测试项目 = item[1],
                    单位 = item[2],
                    数值下限 = item[3],
                    数值上限 = item[4],
                    MethodId = int.Parse(item[5]),
                    耳机指令 = item[6]
                });
            }
            foreach (var item in MoreTestControl)
            {
                if (!LogList.ContainsKey(item.Key))
                    LogList.Add(item.Key, new List<string>() { "", "" });
            }


        }
        /// <summary>
        /// 弹窗
        /// </summary>
        /// <param name="MSG"></param>
        /// <returns></returns>
        public static DialogResult MessageboxShow(string MSG)
            => MessageBox.Show($"{MSG}", "连扳程序提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

        /// <summary>
        /// 检测SN
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public static bool ReviewSN(int TestID, string SN, int TestControlFlag)
        {
            if (TestControlFlag == 3)
                return true;
            lock (snList)
            {
                int Barcode = (int)Config["Barcode"];
                if (Barcode == 0 && SN.Length == 0)
                    return true;
                if (Config.TE_BZP())
                {
                    MessageboxShow($"TE_BZP条码已被禁用");
                    return false;
                }
                if (SN.Length != Barcode && !Config.TE_BZP1() && !Config.TE_BZP2() && !Config.TE_BZP3())
                {
                    MessageboxShow($"条码长度不正确条码长度：{SN.Length}：限制长度：{Barcode}");
                    return false;
                }
                if (Config.TE_BZP1())
                {
                    if ((int)Config["TE_BZPCount"] >= (int)Config["TE_BZPLimit"])
                    {
                        MessageboxShow($"标准品测试次数已达到上限{(int)Config["TE_BZPCount"]}");
                        return false;
                    }
                }
                if (Config.TE_BZP1() || Config.TE_BZP2() || Config.TE_BZP3())
                    Config["SN"] =  SN = $"TE_BZP_{SN}";
                if (snList.ContainsValue(SN))
                {

                    MessageboxShow($"线程{TestID}提示：{SN}正在{snList.Where(item => item.Value == SN).First().Key}号测试");

                    return false;
                }
                return true;
            }

        }




        /// <summary>
        /// 弹窗
        /// </summary>
        /// <param name="MSG"></param>
        /// <param name="Buttons"></param>
        /// <returns></returns>
        public static DialogResult MessageboxShow(IWin32Window owner, string MSG, string Title = "连扳程序提示", MessageBoxButtons Buttons = MessageBoxButtons.YesNo)
            => MessageBox.Show(owner, $"{MSG}", Title, Buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        /// <summary>
        /// 显示程序启动提示框
        /// </summary>
        public static void ShowProgess()
            => Task.Run(() => new ProgressBars(Config).ShowDialog());
        /// <summary>
        /// 结束进程
        /// </summary>
        public static void Kill()
            => Process.GetCurrentProcess().Kill();
    }
}
