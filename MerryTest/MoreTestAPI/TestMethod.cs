using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MerryTest.Entity;

namespace MerryTest.MoreTestAPI
{
    internal class TestMethod
    {
        Dictionary<string, object> OnceConfig;
        int TestID;
        string _TestValue;
        public TestMethod(Dictionary<string, object> MoreTestControl, int TestID)
        {
            this.OnceConfig = MoreTestControl;
            this.TestID = TestID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">项目实体</param>
        /// <param name="TestValue"></param>
        /// <returns></returns>
        public bool Test(TestitemEntity item,   out string TestValue)
        {
            _TestValue = "Test Method False";
            bool result;
            try
            {

                switch (item.MethodId)
                {
                    case 1://下指令并且读取指令返回值，如果为True或者False，直接判断结果。如果是值，则返回与数值上下限的比对结果
                        result = GetValueContrast(item );
                        break;
                    case 2://下指令并且读取指令返回值，然后拿返回值与数值上下限做比对
                        result = GetValuesSectionCompare(item );
                        break;
                    case 3://获取返回值
                        result = GetValues(item.耳机指令 );
                        break;
                    case 4://获取设备的返回值做比对
                        result = GetDeviceValueEquals(item);
                        break;
                    case 5://获取设备的数值做比对
                        result = GetDeviceValuesSectionEquals(item);
                        break;
                    case 6://获取设备的返回值
                        result = GetDeviceValue(item);
                        break;
                    default:
                        result = false;
                        TestValue = _TestValue = "Method ID False";
                        break;
                }
            }
            catch (Exception ex)
            {
                MoreProperty.Config["BugLog"] = $"{ex}";
                _TestValue = $"{ex.Message} False";
                result = false;
            }
            MoreProperty.Config["_TestValue"] = TestValue = _TestValue;//将单项测试结果存入字典
            return result;
        }
        /// <summary>
        /// 下指令并且读取指令返回值，如果为True或者False，直接判断结果。如果是值，则返回与数值上下限的比对结果
        /// </summary>
        bool GetValueContrast(TestitemEntity item)
        {
            _TestValue = MoreProperty.myDll.Run(TestID, item.耳机指令);
            if (_TestValue == "False")
                return false;
            return _TestValue == "True" || (_TestValue == item.数值下限 && _TestValue == item.数值上限);
        }
        /// <summary>
        /// 下指令并且读取指令返回值，然后拿返回值与数值上下限做比对
        /// </summary>
        bool GetValuesSectionCompare(TestitemEntity item)
        {
            string UpperLimit = item.数值上限;
            string LowerLimit = item.数值下限;
            _TestValue = MoreProperty.myDll.Run(TestID, item.耳机指令);
            //由于_TestValue需要在本函数外使用，不能进行Replace，所以另外创建对象操作
            var TestValue = _TestValue;
            if (_TestValue == "False") return false;
            if (TestValue.Contains("%"))
            {
                TestValue = TestValue.Replace("%", "");
            }
            if (!double.TryParse(TestValue, out double val)) return false;
            double value = Convert.ToDouble(TestValue);
            double Lo = LowerLimit == "<" ? double.MinValue : double.Parse(LowerLimit);
            double Up = UpperLimit == ">" ? double.MaxValue : double.Parse(UpperLimit);
            return Lo <= value && value <= Up;
        }
        /// <summary>
        /// 下指令并且读取指令返回值，如果不是False则显示值
        /// </summary>
        bool GetValues(string Command)
        {
            _TestValue = MoreProperty.myDll.Run(TestID, Command);
            return _TestValue.Contains("False") ? false : true;
        }
        bool GetDeviceValueEquals(TestitemEntity item)
        {
            string UpperLimit = item.数值上限;
            string LowerLimit = item.数值下限;
            string Command = item.耳机指令;
            _TestValue = MoreProperty._AllDll.AllDllRun(TestID, Command.Split('&')[0].Split('=')[1], new object[] { new object[] { Command, MoreProperty.myDll.FormsData, item, OnceConfig } }).ToString();
            if (_TestValue == "False") return false;
            return _TestValue == "True" || (UpperLimit == _TestValue && LowerLimit == _TestValue);
        }
        bool GetDeviceValuesSectionEquals(TestitemEntity item)
        {
            string UpperLimit = item.数值上限;
            string LowerLimit = item.数值下限;
            string Command = item.耳机指令;
            _TestValue = MoreProperty._AllDll.AllDllRun(TestID, Command.Split('&')[0].Split('=')[1], new object[] { new object[] { Command, MoreProperty.myDll.FormsData, item, OnceConfig } }).ToString();
            if (!double.TryParse(_TestValue, out double val)) return false;
            double value = Convert.ToDouble(_TestValue);
            double Lo = LowerLimit == "<" ? double.MinValue : double.Parse(LowerLimit);
            double Up = UpperLimit == ">" ? double.MaxValue : double.Parse(UpperLimit);
            return Lo <= value && value <= Up;
        }
        bool GetDeviceValue(TestitemEntity item)
        {
            string Command = item.耳机指令;
            _TestValue = MoreProperty._AllDll.AllDllRun(TestID, Command.Split('&')[0].Split('=')[1], new object[] { new object[] { Command, MoreProperty.myDll.FormsData, item, OnceConfig } }).ToString();
            return _TestValue.Contains("False") ? false : true;
        }
    }
}
