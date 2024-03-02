using System.IO;
using System.Reflection;
using System.Text;
using static MerryTest.Entity.WindowsAPI;

namespace MerryTest.Entity
{
    internal class IniHelpe
    {
        #region 对象的引用，值得定义，类的实例化
        static string path = $@"{ Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Config\CONFIG.INI";
        #endregion
        /// <summary>
        /// 指定section和key读值
        /// </summary>
        /// <param name="section">【类别】</param>
        /// <param name="key">键</param>
        /// <returns>value值</returns>
        public static string GetValue(string section, string key)
        {
            StringBuilder var = new StringBuilder(512);
            int length = GetPrivateProfileString(section, key, "", var, 512, path);
            if (length <= 0) SetValue(section, key, "");
            return var.ToString().Trim();

        }
        /// <summary>
        /// 异步修改INI文本
        /// </summary>
        /// <param name="section">【类别】</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetValue(string section, string key, string value) =>
            WritePrivateProfileString(section, key, value, path);
        public static void SetValue(string section, string key, string value,string Path) =>
           WritePrivateProfileString(section, key, value, Path);
    }
}
