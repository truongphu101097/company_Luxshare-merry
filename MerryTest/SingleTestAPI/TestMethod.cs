using MerryTest.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerryTest.SingleTestAPI.API
{
    internal class TestMethod
    {
        static string _TestValue;
        static int TestItemNumber = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">项目实体</param>
        /// <param name="TestValue"></param>
        /// <returns></returns>
        public static bool Test(TestitemEntity item, out string TestValue)
        {
            bool result;
            SingleTest.Config["Command"] = item.耳机指令;
            //if (item.编号 != 0)
            //{
            //    SingleTest.Config["TestItemNumber"] = item.编号;
            //    TestItemNumber = item.编号;
            //}
            //else
            //{
            //    SingleTest.Config["TestItemNumber"] = TestItemNumber + 1;
            //}
            try
            {
                _TestValue = "False";
                switch (item.MethodId)
                {
                    case 1://下指令并且读取指令返回值，如果为True或者False，直接判断结果。如果是值，则返回与数值上下限的比对结果
                        result = GetValueContrast(item.数值上限, item.数值下限, item.耳机指令);
                        break;
                    case 2://下指令并且读取指令返回值，然后拿返回值与数值上下限做比对
                        result = GetValuesSectionCompare(item.数值上限, item.数值下限, item.耳机指令);
                        break;
                    case 3://获取返回值
                        result = GetValues(item.耳机指令);
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
                        TestValue = _TestValue = "方法序号错误 False";
                        break;
                }
            }
            catch (Exception ex)
            {
                _TestValue = $"{ex.Message} False";
                result = false;
            }
            //将单项测试结果存入字典
            SingleTest.Config["_TestValue"] = TestValue = SingleTest._AllDll.FormsData[4] = _TestValue;
            return result;
        }
        /// <summary>
        /// 下指令并且读取指令返回值，如果为True或者False，直接判断结果。如果是值，则返回与数值上下限的比对结果
        /// </summary>
        static bool GetValueContrast(string UpperLimit, string LowerLimit, string Command)
        {
            _TestValue = SingleTest._AllDll.TypeNameRun(new object[] { Command });
            if (_TestValue == "False")
                return false;
            return _TestValue == "True" || (_TestValue == LowerLimit && _TestValue == UpperLimit);
        }
        /// <summary>
        /// 下指令并且读取指令返回值，然后拿返回值与数值上下限做比对
        /// </summary>
        static bool GetValuesSectionCompare(string UpperValue, string LowerValue, string Command)
        {
            _TestValue = SingleTest._AllDll.TypeNameRun(new object[] { Command });
            //由于_TestValue需要在本函数外使用，不能进行Replace，所以另外创建对象操作
            var TestValue = _TestValue;
            if (_TestValue == "False") return false;
            if (TestValue.Contains("%"))
            {
                TestValue = TestValue.Replace("%", "");
            }
            if (!double.TryParse(TestValue, out double val)) return false;
            double value = Convert.ToDouble(TestValue);
            double Lo = LowerValue == "<" ? double.MinValue : double.Parse(LowerValue);
            double Up = UpperValue == ">" ? double.MaxValue : double.Parse(UpperValue);
            return Lo <= value && value <= Up;
        }
        /// <summary>
        /// 下指令并且读取指令返回值，如果不是False则显示值
        /// </summary>
        static bool GetValues(string Command)
        {
            _TestValue = SingleTest._AllDll.TypeNameRun(new object[] { Command });
            return _TestValue.Contains("False") ? false : true;
        }
        static bool GetDeviceValueEquals(TestitemEntity item)
        {
            string UpperLimit = item.数值上限;
            string LowerLimit = item.数值下限;
            string Command = item.耳机指令;
            _TestValue = SingleTest._AllDll.Run(Command.Split('&')[0].Split('=')[1], "Run", new object[] { new object[] { Command, SingleTest._AllDll.FormsData, item } }).ToString();
            if (_TestValue == "False") return false;
            return _TestValue == "True" || (UpperLimit == _TestValue && LowerLimit == _TestValue);
        }
        static bool GetDeviceValuesSectionEquals(TestitemEntity item)
        {
            string UpperLimit = item.数值上限;
            string LowerLimit = item.数值下限;
            string Command = item.耳机指令;
            _TestValue = SingleTest._AllDll.Run(Command.Split('&')[0].Split('=')[1], "Run", new object[] { new object[] { Command, SingleTest._AllDll.FormsData, item } }).ToString();
            if (!double.TryParse(_TestValue, out double val)) return false;
            double value = double.Parse(_TestValue);
            double Lo = LowerLimit == "<" ? double.MinValue : double.Parse(LowerLimit);
            double Up = UpperLimit == ">" ? double.MaxValue : double.Parse(UpperLimit);
            return Lo <= value && value <= Up;
        }
        static bool GetDeviceValue(TestitemEntity item)
        {
            string Command = item.耳机指令;
            _TestValue = SingleTest._AllDll.Run(Command.Split('&')[0].Split('=')[1], "Run", new object[] { new object[] { Command, SingleTest._AllDll.FormsData, item } }).ToString();
            return _TestValue.Contains("False") ? false : true;
        }
    }
}
