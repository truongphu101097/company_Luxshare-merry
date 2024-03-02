using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerryTest.MoreTestAPI.MoreTestAddTool
{
    public class AddTool : INIOperationClass
    {

        #region 主程序调用接口

        public void Run()
        {
            if (!File.Exists(_path))
            {
                using (File.Create(_path))
                {

                }
                Thread.Sleep(1000);
            }
            List<int> id = new List<int>();
            foreach (var item in (Dictionary<int, Dictionary<string, string>>)MerryTest.MoreTestAPI.MoreProperty.Config["TestControl"])
                id.Add(item.Key);

            List<string> clearSection = new List<string>();
            foreach (var item in id)
            {
                clearSection.Add($"{item}#N9320B");
                clearSection.Add($"{item}#RT550");
                clearSection.Add($"{item}#_2883");
                clearSection.Add($"{item}#DM5D");
            }
            foreach (var TestControl in id)
            {
                string SectionN9320b = $"{TestControl}#N9320B";
                string SectionRT550 = $"{TestControl}#RT550";
                string SectionUSB2883 = $"{TestControl}#_2883";
                string SectionDM5D = $"{TestControl}#DM5D";
                string TestIten_SectionN9320b = $"N9320B";
                string TestIten_RT550 = $"RT550";
                string TestIten_USB2883 = $"_2883";
                string TestIten_DM5D = $"DM5D";

                Dictionary<string, string> addvalue = new Dictionary<string, string>();
                List<string> key = new List<string>(INIGetAllItemKeys(_path, SectionN9320b));
                key.AddRange(INIGetAllItemKeys(_path, SectionRT550));
                key.AddRange(INIGetAllItemKeys(_path, SectionUSB2883));
                key.AddRange(INIGetAllItemKeys(_path, SectionDM5D));

                List<string> writekey = new List<string>();
                List<string[]> testitem = (List<string[]>)MerryTest.MoreTestAPI.MoreProperty.Config["TestItem"];

                //根据测试项目读取已经存在的值跟键
                foreach (var item in testitem)
                {
                    if (item[6].Contains(TestIten_SectionN9320b))
                    {
                        writekey.Add($"{item[1]}&{SectionN9320b}");
                        if (!key.Contains(item[1])) continue;

                        addvalue.Add($"{item[1]}&{SectionN9320b}", INIGetStringValue(_path, SectionN9320b, item[1], ""));
                    }
                    if (item[6].Contains(TestIten_RT550))
                    {
                        writekey.Add($"{item[1]}&{SectionRT550}");
                        if (!key.Contains(item[1])) continue;
                        addvalue.Add($"{item[1]}&{SectionRT550}", INIGetStringValue(_path, SectionRT550, item[1], ""));
                    }

                    if (item[6].Contains(TestIten_USB2883))
                    {
                        writekey.Add($"{item[1]}&{SectionUSB2883}");
                        if (!key.Contains(item[1])) continue;

                        addvalue.Add($"{item[1]}&{SectionUSB2883}", INIGetStringValue(_path, SectionUSB2883, item[1], ""));
                    }
                    if (item[6].Contains(TestIten_DM5D))
                    {
                        writekey.Add($"{item[1]}&{SectionDM5D}");
                        if (!key.Contains(item[1])) continue;

                        addvalue.Add($"{item[1]}&{SectionDM5D}", INIGetStringValue(_path, SectionDM5D, item[1], ""));
                    }


                }
                string[] a = INIGetAllSectionNames(_path);
                //清楚多余字段，值，键
                foreach (var item in INIGetAllSectionNames(_path))
                {
                    if (!clearSection.Contains(item)) INIEmptySection(_path, item);
                    if (!clearSection.Contains(item)) INIDeleteSection(_path, item);
                }
                //写入字段，值，键
                foreach (var item in writekey)
                {
                    if (addvalue.ContainsKey(item))
                    {
                        if (item.Contains(SectionRT550)) INIWriteValue(_path, SectionRT550, item.Split('&')[0], addvalue[item]);
                        if (item.Contains(SectionN9320b)) INIWriteValue(_path, SectionN9320b, item.Split('&')[0], addvalue[item]);
                        if (item.Contains(SectionDM5D)) INIWriteValue(_path, SectionDM5D, item.Split('&')[0], addvalue[item]);
                        if (item.Contains(SectionUSB2883)) INIWriteValue(_path, SectionUSB2883, item.Split('&')[0], addvalue[item]);

                        continue;
                    }
                    if (item.Contains(SectionRT550)) INIWriteValue(_path, SectionRT550, item.Split('&')[0], "0");
                    if (item.Contains(SectionN9320b)) INIWriteValue(_path, SectionN9320b, item.Split('&')[0], "0");
                    if (item.Contains(SectionDM5D)) INIWriteValue(_path, SectionDM5D, item.Split('&')[0], "0");
                    if (item.Contains(SectionUSB2883)) INIWriteValue(_path, SectionUSB2883, item.Split('&')[0], "0");

                }


            }
            AddToolForms.GetForm1(_path, "").Show();
            return;
        }
        #endregion

        string _path = ".\\AllDLL\\MenuStrip\\MoreAddDeploy.ini";
        [DllImport("kernel32.dll")]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder value, int size, string INIpath);
        [DllImport("kernel32.dll")]
        static extern int WritePrivateProfileString(string section, string key, string val, string path);
        public string GetValue(string section, string key)
        {
            StringBuilder var = new StringBuilder(512);
            GetPrivateProfileString(section, key, "null", var, 512, _path);
            return var.ToString().Trim();
        }
        public long SetValue(string section, string Key, string value) => WritePrivateProfileString(section, Key, value, _path);
    }
}
