using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static MerryTest.SingleTestAPI.SingleTestAddTool.INIOperationClass;

namespace MerryTest
{
    public partial class RFCalibration : Form
    {
        public string TestValue = "";
        public bool flag = false;
        List<string> AddRF = new List<string>();
     

        string SectionN9320b = "N9320B";
        string SectionRT550 = "RT550";
        string HanOpticSens = "HanOpticSens";
        string MT8852B = "MT8852B";
        string RFTEST = "RFTEST";
        string RF_JZ = "RF_JZ";
        public RFCalibration(List<string> addRF)
        {
            InitializeComponent();
            //TestValue = Test;
            AddRF = addRF;
        }
        string Compensation_Limit = "Compensation_Limit";
        private void RFCalibration_Load(object sender, EventArgs e)
        {
            string path = ".\\AllDLL\\MenuStrip\\Standard.ini";//把标准值放在此路径
            string path2 = ".\\AllDLL\\MenuStrip\\RealValue.ini";//测试实际值放在此路径

            CreatPath(path);
            CreatPath(path2);
            //rtbTestValue.Text = File.ReadAllText(path, Encoding.UTF8);
            try
            {
                txbLimitCompensation.Text = (Convert.ToDouble(INIGetStringValue(path2, RF_JZ, Compensation_Limit, "50"))).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            WriteResult();//计算测试实际值=测试显示值 - 补偿值；然后把结果写入到 RealValue.ini文件里面去
            LoadResult();//显示测试实际值
         
            Compensation(); //根据设定的校准值自动计算准备设定的补偿值
            rtbCalibrValue.Text = File.ReadAllText(path, Encoding.UTF8);//显示之前已设定的标准值
            //WriteStandard();//保存校准值
            timer1.Enabled = true;
        }
        private void CreatPath(string path)
        {
            if (!File.Exists(path))
            {
                using (File.Create(path))
                {

                };
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 显示测试实际值
        /// </summary>
        private void LoadResult()
        {
            string path = ".\\AllDLL\\MenuStrip\\RealValue.ini";
            rtbTestValue.Text = File.ReadAllText(path, Encoding.UTF8);
        }

        /// <summary>
        /// 保存校准值
        /// </summary>
        private void WriteStandard()
        {
            string path = ".\\AllDLL\\MenuStrip\\Standard.ini";
            File.WriteAllText(path, rtbCalibrValue.Text);
        }
        private void btnYes_Click(object sender, EventArgs e)
        {
            StartCalibration();//开始进行校准
            //this.DialogResult = DialogResult.Yes;
            timer1.Enabled = false;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            timer1.Enabled = false;
            this.Close();
        }

        private void RFCalibration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnYes_Click(null, null);
            }
            else if (e.KeyCode == Keys.N)
            {
                btnNo_Click(null, null);
            }
        }

        Dictionary<string, string> RealValue = new Dictionary<string, string>();
        /// <summary>
        /// 计算测试实际值，然后保存到路径的响应文件
        /// </summary>
        public void WriteResult()
        {
            string path = ".\\AllDLL\\MenuStrip\\AddDeploy.ini";
            string _path = ".\\AllDLL\\MenuStrip\\RealValue.ini";//测试实际值放在此路径
            List<string> writekey = new List<string>();
            //用于储存已经存在在值
            List<string> key = new List<string>();
            key.AddRange(INIGetAllItemKeys(_path, RF_JZ));
            //ADDRF=测试项目里面需要叫你
            try
            {
                foreach (var item in AddRF)
                {
                    string[] add = item.Split('*');//add[0]:测试项目名称   add[1]:测试显示值     add[2]测试指令
                                                   // addvalue.Add($"{add[0]}&{RF_JZ}", add[1]);
                                                   //测试实际值 = 测试显示值 - 补偿值
                    if (add[2].Contains(SectionN9320b))
                    {
                        var test1 = Convert.ToDouble(INIGetStringValue(path, SectionN9320b, add[0], "0"));
                        RealValue[$"{add[0]}&{RF_JZ}"] = (Convert.ToDouble(add[1]) - Convert.ToDouble(INIGetStringValue(path, SectionN9320b, add[0], "0"))).ToString();
                    }
                    if (add[2].Contains(SectionRT550))
                    {
                        RealValue[$"{add[0]}&{RF_JZ}"] = (Convert.ToDouble(add[1]) - Convert.ToDouble(INIGetStringValue(path, SectionRT550, add[0], "0"))).ToString();
                    }
                    if (add[2].Contains(MT8852B))
                    {
                        RealValue[$"{add[0]}&{RF_JZ}"] = (Convert.ToDouble(add[1]) - Convert.ToDouble(INIGetStringValue(path, MT8852B, add[0], "0"))).ToString();
                    }
                    if (add[2].Contains(RFTEST))
                    {
                        RealValue[$"{add[0]}&{RF_JZ}"] = (Convert.ToDouble(add[1]) - Convert.ToDouble(INIGetStringValue(path, RFTEST, add[0], "0"))).ToString();
                    }
                    if (add[2].Contains(HanOpticSens))
                    {
                        RealValue[$"{add[0]}&{RF_JZ}"] = (Convert.ToDouble(add[1]) - Convert.ToDouble(INIGetStringValue(path, HanOpticSens, add[0], "0"))).ToString();
                    }

                    writekey.Add($"{add[0]}&{RF_JZ}");
                }
                RealValue["Compensation_Limit"] = (Convert.ToDouble(INIGetStringValue(path, RF_JZ, Compensation_Limit, "50"))).ToString();
                writekey.Add($"{Compensation_Limit}&{RF_JZ}");
            }
            catch
            {

            }

            //清楚多余字段，值，键
            foreach (var item in INIGetAllSectionNames(_path))
            {
                INIEmptySection(_path, item);
                //if (item != RF_JZ) INIDeleteSection(_path, item);
                INIDeleteSection(_path, item);
            }
            //写入字段，值，键
            foreach (var item in writekey)
            {

                if (RealValue.ContainsKey(item))
                {
                    if (item.Contains(RF_JZ)) INIWriteValue(_path, RF_JZ, item.Split('&')[0], RealValue[item]);
                    continue;
                }
                if (item.Contains(RF_JZ)) INIWriteValue(_path, RF_JZ, item.Split('&')[0], "0");
            }
        }

        Dictionary<string, string> CalibrationValue = new Dictionary<string, string>();

        /// <summary>
        /// 保存校准值到此路径的文件
        /// </summary>
        public void StartCalibration()
        {
            string path = ".\\AllDLL\\MenuStrip\\AddDeploy.ini";
            string path2 = ".\\AllDLL\\MenuStrip\\Standard.ini";
            string path3 = ".\\AllDLL\\MenuStrip\\RealValue.ini";

            List<string> writekey = new List<string>();
            List<string> key = new List<string>();
            key.AddRange(INIGetAllItemKeys(path, SectionN9320b));
            key.AddRange(INIGetAllItemKeys(path, MT8852B));
            key.AddRange(INIGetAllItemKeys(path, SectionRT550));
            key.AddRange(INIGetAllItemKeys(path, HanOpticSens));
            key.AddRange(INIGetAllItemKeys(path, RFTEST));
            key.AddRange(INIGetAllItemKeys(path2, RF_JZ));
            try
            {
                foreach (var item1 in AddRF)
                {
                    string[] item = item1.Split('*');//item[0]:测试项目名称   item[1]:测试显示值     item[2]测试指令
                                                     //writekey.Add($"{item[0]}&{RF_JZ}");
                                                     //如果测试命令包含（N9320B，MT8852B, RT550, HanOpticSens, RFTEST）
                    if (item[2].Contains(SectionN9320b) || item[2].Contains(MT8852B) || item[2].Contains(SectionRT550) || item[2].Contains(HanOpticSens) || item[2].Contains(RFTEST))
                    {
                        writekey.Add($"{item[0]}&{RF_JZ}");
                        if (!key.Contains(item[0])) continue;
                        //addvalue.Add($"{item[0]}&{RF_JZ}", INIGetStringValue(path, RF_JZ, item[0], ""));
                        CalibrationValue[$"{item[0]}&{RF_JZ}"] = INIGetStringValue(path2, RF_JZ, item[0], "");
                    }
                    if (item[2].Contains(SectionN9320b))
                    {
                        writekey.Add($"{item[0]}&{SectionN9320b}");
                        if (!key.Contains(item[0])) continue;
                        CalibrationValue[$"{item[0]}&{SectionN9320b}"] = (Convert.ToDouble(INIGetStringValue(path2, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, SectionN9320b, item[0], "0"))).ToString();
                        //addvalue.Add($"{item[0]}&{SectionN9320b}", (Convert.ToDouble(INIGetStringValue(path, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, SectionN9320b, item[0], "0"))).ToString());
                    }

                    if (item[2].Contains(MT8852B))
                    {
                        writekey.Add($"{item[0]}&{MT8852B}");
                        if (!key.Contains(item[0])) continue;
                        CalibrationValue[$"{item[0]}&{MT8852B}"] = (Convert.ToDouble(INIGetStringValue(path2, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, MT8852B, item[0], "0"))).ToString();
                        // addvalue.Add($"{item[0]}&{MT8852B}", (Convert.ToDouble(INIGetStringValue(path, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, MT8852B, item[0], "0"))).ToString());

                    }

                    if (item[2].Contains(SectionRT550))
                    {
                        writekey.Add($"{item[0]}&{SectionRT550}");
                        if (!key.Contains(item[0])) continue;
                        CalibrationValue[$"{item[0]}&{SectionRT550}"] = (Convert.ToDouble(INIGetStringValue(path2, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, SectionRT550, item[0], "0"))).ToString();
                        //addvalue.Add($"{item[0]}&{SectionRT550}", (Convert.ToDouble(INIGetStringValue(path, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, SectionRT550, item[0], "0"))).ToString());
                    }

                    if (item[2].Contains(HanOpticSens))
                    {
                        writekey.Add($"{item[0]}&{HanOpticSens}");
                        if (!key.Contains(item[0])) continue;
                        CalibrationValue[$"{item[0]}&{HanOpticSens}"] = (Convert.ToDouble(INIGetStringValue(path2, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, HanOpticSens, item[0], "0"))).ToString();
                        //addvalue.Add($"{item[0]}&{HanOpticSens}", (Convert.ToDouble(INIGetStringValue(path, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, HanOpticSens, item[0], "0"))).ToString());
                    }
                    if (item[2].Contains(RFTEST))
                    {
                        writekey.Add($"{item[0]}&{RFTEST}");
                        if (!key.Contains(item[0])) continue;
                        CalibrationValue[$"{item[0]}&{RFTEST}"] = (Convert.ToDouble(INIGetStringValue(path2, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, RFTEST, item[0], "0"))).ToString();
                        //addvalue.Add($"{item[0]}&{HanOpticSens}", (Convert.ToDouble(INIGetStringValue(path, RF_JZ, item[0], "0")) - Convert.ToDouble(item[1]) + Convert.ToDouble(INIGetStringValue(path, HanOpticSens, item[0], "0"))).ToString());
                    }

                }
                //检查补偿值是否在Limit内
                bool flag_jz = true;
                foreach (var item in writekey)
                {
                    if (!item.Contains(RF_JZ))
                    {
                        if (Math.Abs(double.Parse(CalibrationValue[item])) > double.Parse(txbLimitCompensation.Text))
                        {
                            MessageBox.Show($"校准失败，补偿值超过Limit了、\nHiệu chuẩn thất bại, Giá trị bù đã vượt quá giới hạn thiết lập! \n|{CalibrationValue[item]}| > |{txbLimitCompensation.Text}|");
                            flag_jz = false;
                            break;
                        }
                    }
                }
                if (!flag_jz)
                {
                    this.DialogResult = DialogResult.No;
                    return;
                }

                //清楚多余字段，值，键
                foreach (var item in INIGetAllSectionNames(path))
                {
                    INIEmptySection(path, item);
                    if (item != SectionN9320b && item != SectionRT550 && item != HanOpticSens && item != MT8852B && item != RFTEST) INIDeleteSection(path, item);
                    INIEmptySection(path2, item);
                    //if (item != RF_JZ) 
                    INIDeleteSection(path2, item);
                }
                foreach (var item in INIGetAllSectionNames(path2))
                {
                    INIEmptySection(path2, item);
                    if (item != RF_JZ)
                        INIDeleteSection(path2, item);
                }
                //写入字段，值，键
                foreach (var item in writekey)
                {
                    if (CalibrationValue.ContainsKey(item))
                    {
                        if (item.Contains(RF_JZ)) INIWriteValue(path2, RF_JZ, item.Split('&')[0], CalibrationValue[item]);
                        if (item.Contains(SectionRT550)) INIWriteValue(path, SectionRT550, item.Split('&')[0], CalibrationValue[item]);
                        if (item.Contains(SectionN9320b)) INIWriteValue(path, SectionN9320b, item.Split('&')[0], CalibrationValue[item]);
                        if (item.Contains(MT8852B)) INIWriteValue(path, MT8852B, item.Split('&')[0], CalibrationValue[item]);
                        if (item.Contains(HanOpticSens)) INIWriteValue(path, HanOpticSens, item.Split('&')[0], CalibrationValue[item]);
                        if (item.Contains(RFTEST)) INIWriteValue(path, RFTEST, item.Split('&')[0], CalibrationValue[item]);

                        continue;
                    }
                    if (item.Contains(RF_JZ)) INIWriteValue(path2, RF_JZ, item.Split('&')[0], "0");
                    if (item.Contains(SectionRT550)) INIWriteValue(path, SectionRT550, item.Split('&')[0], "0");
                    if (item.Contains(SectionN9320b)) INIWriteValue(path, SectionN9320b, item.Split('&')[0], "0");
                    if (item.Contains(MT8852B)) INIWriteValue(path, MT8852B, item.Split('&')[0], "0");
                    if (item.Contains(HanOpticSens)) INIWriteValue(path, HanOpticSens, item.Split('&')[0], "0");
                    if (item.Contains(RFTEST)) INIWriteValue(path, RFTEST, item.Split('&')[0], "0");
                }
                
                this.DialogResult = DialogResult.Yes;
                
            }
            catch
            {

            }
            finally
            {
                INIWriteValue(path3, RF_JZ, Compensation_Limit, txbLimitCompensation.Text);
            }
            
            
        }

        /// <summary>
        /// 根据设定的校准值自动计算补偿值
        /// </summary>
        public void Compensation()
        {
            string path = ".\\AllDLL\\MenuStrip\\RealValue.ini";
            string path2 = ".\\AllDLL\\MenuStrip\\Standard.ini";
            string ListCompensationValue = null;
            foreach (var item1 in AddRF)
            {
                string[] item = item1.Split('*');
                double value = 0;
                try
                {
                    value = Convert.ToDouble(INIGetStringValue(path2, RF_JZ, item[0], "0")) - Convert.ToDouble(INIGetStringValue(path, RF_JZ, item[0], "0"));
                }
                catch (Exception ex)
                {
                    value = 0;
                }
               
                ListCompensationValue +=($"{ item[0]} = { value}\n");

            }
            rtbCompensation.Text = ListCompensationValue;
        }

        string _path = ".\\AllDLL\\MenuStrip\\RealValue.ini";//测试实际值放在此路径
        [DllImport("kernel32.dll")]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder value, int size, string INIpath);
        [DllImport("kernel32.dll")]
        static extern int WritePrivateProfileString(string section, string key, string val, string path);
        public string GetValue(string section, string key)
        {
            try
            {
                StringBuilder var = new StringBuilder(512);
                GetPrivateProfileString(section, key, "null", var, 512, _path);
                return var.ToString().Trim();
            }
            catch
            {
                return "0";
            }

        }
        public long SetValue(string section, string Key, string value) => WritePrivateProfileString(section, Key, value, _path);

       

        private void rtbCalibrValue_TextChanged(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string path = ".\\AllDLL\\MenuStrip\\Standard.ini";
            Compensation();
            //rtbCalibrValue.Text = File.ReadAllText(path, Encoding.UTF8);
            WriteStandard();
        }
    }
}
