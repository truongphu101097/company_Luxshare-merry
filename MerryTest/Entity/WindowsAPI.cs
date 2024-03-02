using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerryTest.Entity
{
    internal static class WindowsAPI
    {
        /// <summary>
        /// 将该指针的窗口设为活动窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 读取INI文件 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="INIPath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string INIPath);

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 释放hFile路径资源
        /// </summary>
        /// <param name="hFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hFile);

        /// <summary>
        /// 获取电脑设备的GUID
        /// </summary>
        /// <param name="hidGuid"></param>
        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(ref Guid hidGuid);

        /// <summary>
        /// 获取通过电脑上的GUID过滤出所需的设备的GUID
        /// </summary>
        /// <param name="gClass">电脑设备的UID</param>
        /// <param name="strEnumerator">NULL</param>
        /// <param name="hParent">intPtr.zero</param>
        /// <param name="nFlags">选择所需条件</param>
        /// <returns>返回该GUID指针</returns>
        [DllImport("setupapi.dll", SetLastError = true)]//过滤设备，获取需要的设备
        public static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, Digcf nFlags);

        /// <summary>
        /// 用于过滤HID设备的参数
        /// </summary>
        public enum Digcf  //3
        {
            /// <summary>
            /// 返回与系统默认设备相关的设备
            /// </summary>
            DigcfDefault = 0x1,
            /// <summary>
            /// 返回当前存在的设备
            /// </summary>
            DigcfPresent = 0x2,
            /// <summary>
            /// 返回所有安装的设备
            /// </summary>
            DigcfAllclasses = 0x4,
            /// <summary>
            /// 只返回当前硬件配置文件的设备
            /// </summary>
            DigcfProfile = 0x8,
            /// <summary>
            /// 返回所有支持的设备
            /// </summary>
            DigcfDeviceinterface = 0x10
        }

        /// <summary>
        /// 根据路径创建句柄
        /// </summary>
        /// <param name="strName">根据GUID获取的Path</param>
        /// <param name="nAccess">0x40000000|0x80000000</param>
        /// <param name="nShareMode">0x1|0x2</param>
        /// <param name="lpSecurity">IntPtr.zreo</param>
        /// <param name="nCreationFlags">3</param>
        /// <param name="nAttributes">0</param>
        /// <param name="lpTemplate">IntPtr.zreo</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref SpDeviceInterfaceData oInterfaceData, ref SpDeviceInterfaceDetailData oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        /// <summary>
        /// 通过GUID指针  获取设备信息
        /// </summary>
        /// <param name="hDevInfo">GUID指针</param>
        /// <param name="devInfo">0</param>
        /// <param name="interfaceClassGuid">IntPtr.zero</param>
        /// <param name="memberIndex">从0索引开始有多少设备就有多少个</param>
        /// <param name="deviceInterfaceData">获取的信息</param>
        /// <returns>返回该索引信息是否成功</returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, uint devInfo, ref Guid interfaceClassGuid, uint memberIndex, ref SpDeviceInterfaceData deviceInterfaceData);
        /// <summary>
        /// 获取装置路径及ClassGUID
        /// </summary>
        /// <param name="deviceInfoSet"></param>
        /// <param name="deviceInterfaceData"></param>
        /// <param name="deviceInterfaceDetailData"></param>
        /// <param name="deviceInterfaceDetailDataSize"></param>
        /// <param name="requiredSize"></param>
        /// <param name="deviceInfoData"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SpDeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData,
                                                                 uint deviceInterfaceDetailDataSize, ref uint requiredSize, IntPtr deviceInfoData);
        /// <summary>
        /// 销毁设备信息列表
        /// </summary>
        /// <param name="deviceInfoSet"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);


        public struct SpDeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 2)]//2
        public struct SpDeviceInterfaceDetailData
        {
            internal int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string DevicePath;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref SpDeviceInterfaceData DeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        public unsafe static extern bool SetupDiGetDevicePropertyW(
                                IntPtr DeviceInfoSet,
                                ref SpDeviceInterfaceData DeviceInfoData,
                                ref DEVPKEY_Device_RemovalRelations PropertyKey,
                                ref uint PropertyType,
                                byte* PropertyBuffer,
                                byte[] PropertyBufferSize,
                                ref int RequiredSize,
                                int Flags
                                  );
        public struct DEVPKEY_Device_RemovalRelations
        {
            public Guid fmtid;
            public uint pid;

        }
        public static DEVPKEY_Device_RemovalRelations DEVPKEY_Device_BusRelations = new DEVPKEY_Device_RemovalRelations()
        {
            fmtid = new Guid("4340A6C5-93FA-4706-972C-7B648008A5A7"),
            pid = 7
        };
        public static DEVPKEY_Device_RemovalRelations DEVPKEY_Device_LocationInfo = new DEVPKEY_Device_RemovalRelations()
        {
            fmtid = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"),
            pid = 15
        };



























        /// <summary>
        /// 转换成String使用contains
        /// </summary>
        /// <param name="varObject"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool strContains(this object varObject, string v) => Convert.ToString(varObject).Contains(v);
        public static DialogResult MessageBoxShow_MerryTest(string MEG, MessageBoxButtons Buttons = MessageBoxButtons.OK)
            => MessageBox.Show(MEG, "MerryTest提示", Buttons, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
        public static DialogResult MessageBoxShow_MoreTest(string MEG, MessageBoxButtons Buttons = MessageBoxButtons.OK)
            => MessageBox.Show(MEG, "MoreTest提示", Buttons, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);

        /// <summary>
        /// 判断是否是标准品
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool TE_BZP(this Dictionary<string, object> keys) => keys["SN"].strContains("TE_BZP");
        public static bool TE_BZP1(this Dictionary<string, object> keys) => keys["SN"].strContains((string)keys["TE_BZP1"]);
        public static bool TE_BZP2(this Dictionary<string, object> keys) => keys["SN"].strContains((string)keys["TE_BZP2"]);
        public static bool TE_BZP3(this Dictionary<string, object> keys) => keys["SN"].strContains((string)keys["TE_BZP3"]);
        public static bool TE_BZP4(this Dictionary<string, object> keys) => keys["SN"].strContains((string)keys["TE_BZP4"]);



    }
}
