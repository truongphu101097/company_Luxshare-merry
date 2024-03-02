using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerryTest.MoreTestAPI
{
    internal class TestTypeDll
    {
        public string dllName;
        public Dictionary<string, object> Config;
        Dictionary<int, AllDLL> testTypedll = new Dictionary<int, AllDLL>();
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
            "",
            "",
            "",
            "",
            ""
        };
        public Dictionary<string, string[]> dllInfo
        {
            get
            {
                return testTypedll.First().Value.dllInfo;
            }
        }

        /// <summary>
        /// 程序启动时触发机型dll
        /// </summary>
        /// <param name="TestID"></param>
        /// <returns></returns>
        public bool Start(int TestID)
        {
            string dllPath = $"{Config["adminPath"]}/TestItem/{dllName}/{dllName}.dll";
            AllDLL mydll = new AllDLL(Config);
            string LoadResult = mydll.LoadDll(dllName, dllPath);
            if (LoadResult.Contains("False")) return false;
            object isStart = mydll.CallMethod(dllName, "Start", new object[] { new List<string> { }, (IntPtr)Config["adminHandle"] }, out bool isError, out Exception Errorinfo);
            if (isError) AllDLL.MessageboxShow($"机型Dll使用Start方法报错，报错信息：\r\n{Errorinfo}");
            testTypedll[TestID] = mydll;
            return Convert.ToBoolean(isStart);
        }
        /// <summary>
        /// 开始测试时触发机型dll
        /// </summary>
        /// <param name="TestID"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool StartTest(int TestID, Dictionary<string, object> info)
        {
            return Convert.ToBoolean(testTypedll[TestID].CallMethod(dllName, "StartTest", new object[] { info }, out _, out _));
        }
        /// <summary>
        /// 测试中触发机型dll
        /// </summary>
        /// <param name="TestID"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public string Run(int TestID, string Command)
            => testTypedll[TestID].CallMethod(dllName, "Run", new object[] { Command }, out _, out _).ToString();
        /// <summary>
        /// 所有线程测试结束时触发机型dll
        /// </summary>
        /// <param name="TestID"></param>
        /// <returns></returns>
        public void TestsEnd(int TestID)
            => testTypedll[TestID].CallMethod(dllName, "TestsEnd", new object[] { new object { } }, out _, out _);
        /// <summary>
        /// 程序关闭时触发机型dll
        /// </summary>
        /// <param name="TestID"></param>
        /// <returns></returns>
        public string CloseProgram(int TestID)
            => testTypedll[TestID].CallMethod(dllName, "CloseProgram", new object[] { new object { } }, out _, out _).ToString();
    }
}
