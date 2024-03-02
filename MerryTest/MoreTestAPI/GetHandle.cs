using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MerryTest.Entity.WindowsAPI;
namespace MerryTest.MoreTestAPI
{
    /// <summary>
    /// 获取句柄类
    /// </summary>
    public class GetHandle
    {
        #region 参数及引用区

        #endregion
        public string Path = "";

        #region 将路径转换成句柄
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint FILE_SHARE_READ = 0x1;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);
        private static IntPtr GetHidDeviceHandle(string HidDevicePath)
        {
            IntPtr _HIDWriteHandle = IntPtr.Zero;
            if (!String.IsNullOrEmpty(HidDevicePath))
            {

                _HIDWriteHandle = CreateFile(HidDevicePath, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            }
            return _HIDWriteHandle;
        }

        #endregion

        #region 获取装置路径1



        public bool getPath(string PID, string VID, string col, string Number)
        {
            Path = "";
            Guid hidGuid = Guid.Empty;
            Regex TXregPid = new Regex(PID.ToLower());
            Regex TXregVid = new Regex(VID.ToLower());
            Regex ExternAgs = new Regex(col.ToLower());
            Regex rNumber = new Regex(Number.ToLower());
            bool result = true;
            IntPtr hidHandle = IntPtr.Zero;
            UInt32 deviceSerialNumber = 0;
            HidD_GetHidGuid(ref hidGuid);

            IntPtr hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfDeviceinterface);//过滤设备，获取需要的设备
            try
            {
                SpDeviceInterfaceData deviceInterfaceData = new SpDeviceInterfaceData();
                deviceInterfaceData.Size = Marshal.SizeOf(deviceInterfaceData);
                while (result)
                {

                    result = SetupDiEnumDeviceInterfaces(hDevInfo, 0, ref hidGuid, deviceSerialNumber, ref deviceInterfaceData);//获取设备，true获取到                   
                    if (result)
                    {
                        uint nRequiredSize = 0;
                        SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero);

                        SpDeviceInterfaceDetailData detailData = new SpDeviceInterfaceDetailData();
                        detailData.Size = 5;	// hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it, mail me! Thx
                        result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, ref detailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero);//获取接口的详细信息，必须调用两次，一次返回长度，二次获取数据

                        if (result)
                        {
                            if (detailData.DevicePath != null)
                            {
                                Match MatchPid = TXregPid.Match(detailData.DevicePath);
                                Match MatchVID = TXregVid.Match(detailData.DevicePath);
                                Match mathExternAgs = ExternAgs.Match(detailData.DevicePath);
                                Match mNumber = rNumber.Match(detailData.DevicePath);

                                if (MatchPid.Success && MatchVID.Success && mathExternAgs.Success && mNumber.Success)
                                {
                                    Path = detailData.DevicePath;
                                    return true;
                                }
                            }
                            deviceSerialNumber++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            return false;
        }
        #endregion


        #region 参数区


        #endregion
        #region Location
        unsafe public void SelectLocation(string Location, out List<string> HidInfo)
        {
            HidInfo = new List<string>();
            bool Flag = false;

            Guid guid = Guid.Empty;
            //获取HID的GUID
            HidD_GetHidGuid(ref guid);
            //          系统装置类指针                                选择USB                          返回当前存在且 所安装的设备
            IntPtr DeviceInfoSet = SetupDiGetClassDevs(ref guid, "USB", IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfAllclasses);
            //指针==0的时候说明获取失败或者没有
            if (DeviceInfoSet == IntPtr.Zero) return;
            for (int i = 0; ; i++)
            {
                //系统定义装置结构
                SpDeviceInterfaceData DeviceInterfaceData = new SpDeviceInterfaceData();
                //获取非托管变量的大小
                DeviceInterfaceData.Size = Marshal.SizeOf(DeviceInterfaceData);
                //系统装置会有个列表排列装置    指针         我们不知道第几个需要是哪个设备只能每个遍历（i）
                if (!SetupDiEnumDeviceInfo(DeviceInfoSet, i, ref DeviceInterfaceData)) break;

                int RequiredSize = 0;
                uint PropertyType = 0;
                //确保变量有足够的容量储存返回信息
                int byteLength = 2048; //1024;
                byte[] PathBuffer = new byte[byteLength];
                if (PathBuffer.Length < byteLength) Array.Resize(ref PathBuffer, byteLength);
                byte[] LocationBuffer = new byte[byteLength];
                if (LocationBuffer.Length < byteLength) Array.Resize(ref LocationBuffer, byteLength);

                //C#使用指针的方式
                fixed (byte* sTransData = &PathBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                                     DeviceInfoSet,
                                     ref DeviceInterfaceData,
                                     ref DEVPKEY_Device_BusRelations,//系统变量存储路径的GUID和PID的编号
                                     ref PropertyType,
                                    sTransData,                      //储存容量的指针
                                    (PathBuffer),                    //返回的内容是ASCII
                                     ref RequiredSize,
                                     0
                                     );
                }
                fixed (byte* sTransData = &LocationBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                               DeviceInfoSet,
                               ref DeviceInterfaceData,
                               ref DEVPKEY_Device_LocationInfo,
                               ref PropertyType,
                              sTransData,
                                (LocationBuffer),
                               ref RequiredSize,
                               0
                               );
                }
                //转换格式就不用解释
                List<byte> Pathbyte = new List<byte>();
                List<byte> LocationByte = new List<byte>();
                foreach (var item in PathBuffer)
                    if (item > 0) Pathbyte.Add(item);
                string Pathstr = Encoding.ASCII.GetString(Pathbyte.ToArray()).ToLower();

                foreach (var item in LocationBuffer)
                    if (item > 0) LocationByte.Add(item);
                string locationstr = Encoding.ASCII.GetString(LocationByte.ToArray());
                Console.WriteLine(Pathstr);
                Console.WriteLine(locationstr);

                //选择包含的位置              //包含HID字眼用于下指令，因为装置会有很多 USB Path
                if (locationstr.Contains(Location) & Pathstr.Contains("hid"))
                {
                    foreach (var item in Pathstr.Split('h'))
                    {
                        if (item.Trim().Length <= 0) continue;
                        HidInfo.Add($"h{item}".Replace("\\", "#"));
                    }
                }

            }
        }
        unsafe public bool SelectLocation(out List<string> HidInfos)
        {

            HidInfos = new List<string>();
            Guid guid = Guid.Empty;
            //获取HID的GUID
            HidD_GetHidGuid(ref guid);
            //          系统装置类指针                                选择USB                          返回当前存在且 所安装的设备
            IntPtr DeviceInfoSet = SetupDiGetClassDevs(ref guid, "USB", IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfAllclasses);
            //指针==0的时候说明获取失败或者没有
            if (DeviceInfoSet == IntPtr.Zero) return false;
            for (int i = 0; ; i++)
            {
                //系统定义装置结构
                SpDeviceInterfaceData DeviceInterfaceData = new SpDeviceInterfaceData();
                //获取非托管变量的大小
                DeviceInterfaceData.Size = Marshal.SizeOf(DeviceInterfaceData);
                //系统装置会有个列表排列装置    指针         我们不知道第几个需要是哪个设备只能每个遍历（i）
                if (!SetupDiEnumDeviceInfo(DeviceInfoSet, i, ref DeviceInterfaceData)) break;

                int RequiredSize = 0;
                uint PropertyType = 0;
                //确保变量有足够的容量储存返回信息
                byte[] PathBuffer = new byte[2048];
                if (PathBuffer.Length < 2048) Array.Resize(ref PathBuffer, 2048);
                byte[] LocationBuffer = new byte[2048];
                if (LocationBuffer.Length < 2048) Array.Resize(ref LocationBuffer, 2048);

                //C#使用指针的方式
                fixed (byte* sTransData = &PathBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                                     DeviceInfoSet,
                                     ref DeviceInterfaceData,
                                     ref DEVPKEY_Device_BusRelations,//系统变量存储路径的GUID和PID的编号
                                     ref PropertyType,
                                    sTransData,                      //储存容量的指针
                                    (PathBuffer),                    //返回的内容是ASCII
                                     ref RequiredSize,
                                     0
                                     );
                }
                fixed (byte* sTransData = &LocationBuffer[0])
                {
                    SetupDiGetDevicePropertyW(
                               DeviceInfoSet,
                               ref DeviceInterfaceData,
                               ref DEVPKEY_Device_LocationInfo,
                               ref PropertyType,
                              sTransData,
                                (LocationBuffer),
                               ref RequiredSize,
                               0
                               );
                }
                //转换格式就不用解释
                List<byte> Pathbyte = new List<byte>();
                List<byte> LocationByte = new List<byte>();
                foreach (var item in PathBuffer)
                    if (item > 0) Pathbyte.Add(item);
                foreach (var item in LocationBuffer)
                    if (item > 0) LocationByte.Add(item);
                string locationstr = Encoding.ASCII.GetString(LocationByte.ToArray());
                string Pathstr = Encoding.ASCII.GetString(Pathbyte.ToArray()).ToLower();
                //选择包含的位置              //包含HID字眼用于下指令，因为装置会有很多 USB Path
                if (Pathstr.Contains("hid"))
                {
                    foreach (var item in Pathstr.Split('h'))
                    {
                        HidInfos.Add($"{locationstr}={$"h{item}".Replace("\\", "#")}");
                    }
                }

            }
            return true;
        }
        #endregion
    }
}
