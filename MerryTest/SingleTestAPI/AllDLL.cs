using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.SingleTestAPI.API
{
    public class AllDLL
    {
        public AllDLL(Dictionary<string, object> config) => Config = config;
        Dictionary<string, object> Config;
        Dictionary<string, Type> DllType = new Dictionary<string, Type>();
        Dictionary<string, object> MagicClassObject = new Dictionary<string, object>();
        public Dictionary<string, string[]> dllInfo = new Dictionary<string, string[]>();
        static string TestSN = "";
        static string LicenseKey = "";
        static string BDAT = "";
        static string SN_BCCode = "";
        static string TestValue = "";
        /// <summary>
        /// 需要传入Dll的数据( 索引  
        /// [0]=SN，
        /// [1]=LicenseKey，
        /// [2]=BDAT，
        /// [3]=SN_BCCode ,
        /// [3]=TestValue 
        /// )
        /// </summary>
        public List<string> FormsData = new List<string>
        {
            TestSN,
            LicenseKey,
            BDAT,
            SN_BCCode,
            TestValue
        };
        /// <summary>
        ///  运行设备DLL
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object Run(string name, string methods, object[] parameter)
        {

            string path = $@"{Config["adminPath"]}\AllDLL\{name}\{name}.dll";
            return Run(name, path, methods, parameter);
        }
        public object Run(string name, string path, string methods, object[] parameter)
        {
            LoadDll(name, path);
            return CallMethod(name, methods, parameter);
        }
        public object EventRun(string name, string methods, object[] parameter, out bool isResult, out string error)
        {
            string path = $@"{Config["adminPath"]}\AllDLL\{name}\{name}.dll";
            LoadDll(name, path);
            return CallMethod(name, methods, parameter, out isResult, out error);

        }
        /// <summary>
        /// 连接DLL并调用Start方法
        /// </summary>
        /// <param name="name">机型名</param>
        /// <param name="parameter">参数</param>
        /// <returns>Start方法回传值</returns>
        public bool Start(object[] parameter)
        {

            string Name = (string)Config["Name"];
            Config["LoadMessagebox"] = $"启动{Name}";
            if (!LoadDll(Name, $@"{Config["adminPath"]}\TestItem\{Name}\{Name}.dll"))
            {
                return false;
            }
            return (bool)CallMethod(Name, "Start", parameter);

        }
        /// <summary>
        /// 运行StartRun方法
        /// </summary>
        /// <param name="parameter">方法参数(object[]方式)，没有参数则为null</param>
        /// <returns>Run方法回传值</returns>
        public bool StartRun() => (bool)CallMethod((string)Config["Name"], "StartRun", null);
        /// <summary>
        /// 运行机型模块Run方法
        /// </summary>
        /// <param name="parameter">方法参数(object[]方式)，没有参数则为null</param>
        /// <returns>Run方法回传值</returns>
        public string TypeNameRun(object[] parameter)
        {
            string result = CallMethod((string)Config["Name"], "Run", parameter, out bool errorResult, out string error).ToString();
            if (errorResult) return error;
            return result;
        }
        public void Dispose()
        {
            DllType.Clear();
            MagicClassObject.Clear();
            dllInfo.Clear();
        }
        public bool LoadDll(string TKey, string path)
        {
            try
            {
                if (DllType.ContainsKey(TKey)) return true;
                string _namespace = "MerryDllFramework";
                string _class = "MerryDll";
                //根据路径读取Dll
                var ass = Assembly.LoadFrom(path);
                //根据抓的dll执行该命名空间及类
                DllType.Add(TKey, ass.GetType($"{_namespace}.{_class}"));
                //抓取该类构造函数并且抓取改方法
                MagicClassObject.Add(TKey, DllType[TKey].GetConstructor(Type.EmptyTypes).Invoke(new object[] { }));
                object dllinfo = CallMethod(TKey, "GetDllInfo", null);
                if (dllinfo.GetType() == typeof(string[]))
                {
                    dllInfo.Add(TKey, (string[])dllinfo);
                }
                isCallMethod(TKey);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dll加载失败：{path} \r\n{ex}");
                return false;
            }

        }
        public object CallMethod(string TKey, string method, object[] parameter)
        {
            return CallMethod(TKey, method, parameter, out _, out _);
        }
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public object CallMethod(string TKey, string method, object[] parameter, out bool isError, out string error)
        {
            error = "";
            isError = false;
            try
            {

                MethodInfo mi = DllType[TKey].GetMethod(method);
                if (mi == null)
                {
                    MessageBox.Show(error = $"{TKey}模块{method}方法无法索引，执行失败");
                    return null;
                }

                return mi.Invoke(MagicClassObject[TKey], parameter);

            }
            catch (Exception ex)
            {
                File.AppendAllText($@"{SingleTest.Config["adminPath"]}\Log\错误信息{DateTime.Now:MM_dd}.txt", $"{DateTime.Now}\r\n{ex}\r\n{TKey}|{method}\r\n", Encoding.UTF8);

                error = $"{ex.Message} False";
                isError = true;
                return false;
            }
        }

        public bool isCallMethod(string TKey)
        {
            MethodInfo mi = DllType[TKey].GetMethod("Interface");
            if (mi != null)
            {
                mi.Invoke(MagicClassObject[TKey], new object[] { Config });//方法有参数时，需要把null替换为参数的集合
                return true;
            }
            return false;
        }

    }
}
