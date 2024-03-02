using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.MoreTestAPI
{
    public class AllDLLs : IDisposable
    {

        public Dictionary<string, object> Config;
        public Dictionary<string, string[]> dllInfo
        {
            get
            {
                return alldll.LastOrDefault().Value.dllInfo;
            }
        }

        Dictionary<int, AllDLL> alldll = new Dictionary<int, AllDLL>();
        public string LoadDll(int TestID, string dllName, string path)
        {
            if (!alldll.ContainsKey(TestID)) alldll[TestID] = new AllDLL(this.Config);
            return alldll[TestID].LoadDll(dllName, path);
        }
        public object CallMethod(int TestID, string dllName, string method, object[] parameter, out bool isError, out Exception error)
            => alldll[TestID].CallMethod(dllName, method, parameter, out isError, out error);
        public object AllDllRun(int TestID, string dllName, object[] parameter)
        {
            string Result = alldll[TestID].CallMethod(dllName, "Run", parameter, out bool isError, out Exception error).ToString();
            if (isError)
                return $"Error {error.Message} False";
            return Result;
        }
        public object AllDllRun(int TestID, string dllName, string method, object[] parameter)
            => EnventAllDllRun(TestID, dllName, method, parameter, out _, out _);
        public object EnventAllDllRun(int TestID, string dllName, string method, object[] parameter, out bool isError, out Exception error)
        {
            string path = $@"{Config["adminPath"]}\AllDLL\{dllName}\{dllName}.dll";
            string LoadResult = LoadDll(TestID, dllName, path);
            isError = true;
            error = null;
            if (LoadResult.Contains("False"))
                return LoadResult;
            return CallMethod(TestID, dllName, method, parameter, out isError, out error);
        }
        public void Dispose()
        {
            alldll.Clear();
            alldll = null;
            alldll = new Dictionary<int, AllDLL>();
            GC.Collect();
        }

    }
    class AllDLL
    {
        public AllDLL(Dictionary<string, object> Config)
            => this.Config = Config;
        Dictionary<string, object> Config;
        Dictionary<string, Type> DllType = new Dictionary<string, Type>();
        Dictionary<string, object> MagicClassObject = new Dictionary<string, object>();
        public Dictionary<string, string[]> dllInfo = new Dictionary<string, string[]>();

        /// <summary>
        /// 加载Dll
        /// </summary>
        /// <param name="TKey">dll名称</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public string LoadDll(string dllName, string path)
        {
            bool loadDllFlag = false;
            try
            {
                if (DllType.ContainsKey(dllName)) return $"{loadDllFlag = true}";
                string _namespace = "MerryDllFramework";
                string _class = "MerryDll";
                //根据路径读取Dll
                if (!File.Exists(path))
                    return MessageboxShow($"{path} Not found File False\r\n找不到要加载的Dll");
                var ass = Assembly.LoadFrom(path);
                //根据抓的dll执行该命名空间及类
                Type dllType = ass.GetType($"{_namespace}.{_class}");
                if (dllType == null)
                    return MessageboxShow($"{dllName} Not found namespace class False\r\n加载Dll的类异常");
                DllType[dllName] = dllType;
                //抓取该类构造函数并且抓取改方法
                object ClassObject = DllType[dllName].GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                if (ClassObject == null)
                    return MessageboxShow($"{dllName} Not found Constructor Init False\r\n加载Dll的构造函数出现异常");
                MagicClassObject[dllName] = ClassObject;
                dllInfo[dllName] = GetDllInfo(dllName);
                CallMethod(dllName, "Interface", new object[] { Config }, out _, out _);
                loadDllFlag = true;
                return $"{true}";
            }
            catch (Exception ex)
            {
                Config["BugLog"] = ex.ToString();
                MessageBox.Show($"Dll加载失败：{path} \r\n{ex}\r\n");
                return $"{ex.Message} False";
            }
            finally
            {
                if (!loadDllFlag)
                {
                    DllType.Remove(dllName);
                    MagicClassObject.Remove(dllName);
                }
            }

        }
        /// <summary>
        /// 呼叫Dll的方法
        /// </summary>
        /// <param name="TKey">dll名称</param>
        /// <param name="method">方法名称</param>
        /// <param name="parameter">传入的参数</param>
        /// <param name="isError">是否报错</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public object CallMethod(string dllName, string method, object[] parameter, out bool isError, out Exception error)
        {
            error = null;
            isError = false;
            try
            {
                if (!DllType.ContainsKey(dllName))
                {
                    MessageboxShow($"{dllName} 并未加载，无法执行方法");
                }
                MethodInfo mi = DllType[dllName].GetMethod(method);
                if (mi == null)
                    return $"Null {false}";
                return mi.Invoke(MagicClassObject[dllName], parameter);
            }
            catch (Exception ex)
            {
                error = ex;
                Config["BugLog"] = ex.ToString();
                isError = true;
                return $"{false}";
            }
        }
        string[] GetDllInfo(string dllName)
        {
            object result = CallMethod(dllName, "GetDllInfo", null, out _, out _);
            if (result.ToString().Contains("False"))
            {
                MessageboxShow("加载Dll版本信息报错了，请停止测试");
                return null;
            }
            if (result.GetType() == typeof(string[]))
            {
                return (string[])result;
            }
            return null;

        }
        public static string MessageboxShow(string MSG, string Title = "连扳程序提示")
        {
            MessageBox.Show($"{MSG}", Title, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            return MSG;
        }

        ~AllDLL()
        {
            DllType.Clear();
            MagicClassObject.Clear();
            dllInfo.Clear();
            Console.WriteLine("AllDll的资源都释放了");
        }


    }
}
