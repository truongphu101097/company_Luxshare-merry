using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MerryTest.Entity.WindowsAPI;

namespace MerryTest.SingleTestAPI.API
{
    /// <summary>
    /// 获取句柄类
    /// </summary>
    public static class GetHandle
    {
        #region 参数及引用区
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint FILE_SHARE_READ = 0x1;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        public static IntPtr Handle = IntPtr.Zero;
        public static string Path = "";
        #endregion
        #region 获取装置路径
        public static bool GetHidDevicePath(string headsetPID, string headsetVID, string donglePID, string dongleVID)
        {
            var hidGuid = Guid.Empty;
            var TXregPid = new Regex(donglePID.ToLower());
            var TXregVid = new Regex(dongleVID.ToLower());
            var RXregPid = new Regex(headsetPID.ToLower());
            var RXregVid = new Regex(headsetVID.ToLower());
            Regex[] ExternAgs = { new Regex("") };
            var result = true;
            var resultflag = false;
            var hidHandle = IntPtr.Zero;
            uint deviceSerialNumber = 0;
            HidD_GetHidGuid(ref hidGuid);

            var hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfDeviceinterface);//过滤设备，获取需要的设备
            try
            {
                var deviceInterfaceData = new SpDeviceInterfaceData();
                deviceInterfaceData.Size = Marshal.SizeOf(deviceInterfaceData);
                while (result)
                {
                    result = SetupDiEnumDeviceInterfaces(hDevInfo, 0, ref hidGuid, deviceSerialNumber, ref deviceInterfaceData);//获取设备，true获取到                   
                    if (result)
                    {
                        uint nRequiredSize = 0;
                        SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero);

                        var detailData = new SpDeviceInterfaceDetailData();
                        detailData.Size = 5;	// hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it, mail me! Thx
                        result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, ref detailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero);//获取接口的详细信息，必须调用两次，一次返回长度，二次获取数据
                        if (result)
                        {
                            if (detailData.DevicePath != null)
                            {
                                Match MatchTXPid = TXregPid.Match(detailData.DevicePath);
                                Match MatchRxPid = RXregPid.Match(detailData.DevicePath);
                                Match MatchTXVID = TXregVid.Match(detailData.DevicePath);
                                Match MatchRXVID = RXregVid.Match(detailData.DevicePath);
                                Match[] mathExternAgs = new Match[1];
                                for (int i = 0; i < 1; i++)
                                {
                                    mathExternAgs[i] = ExternAgs[i].Match(detailData.DevicePath);
                                }
                                if (MatchTXPid.Success && MatchTXVID.Success)
                                {
                                    for (int i = 0; i < 1; i++)
                                    {
                                        if (mathExternAgs[i].Success)
                                        {
                                            donglepath[i] = detailData.DevicePath;
                                            resultflag = true;
                                        }
                                    }
                                }
                                else if (MatchRxPid.Success && MatchRXVID.Success)
                                {
                                    for (int i = 0; i < 1; i++)
                                    {
                                        if (mathExternAgs[i].Success)
                                        {
                                            headsetpath[i] = detailData.DevicePath;
                                            resultflag = true;
                                        }
                                    }
                                }
                            }
                            deviceSerialNumber++;
                        }
                    }
                }
            }
            catch
            {

                resultflag = false;
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            return resultflag;
        }
        #endregion
        #region 将路径转换成句柄
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
        private static bool GetHidDevicePath(string PID, string VID, string col)
        {
            Guid hidGuid = Guid.Empty;
            Regex TXregPid = new Regex(PID.ToLower());
            Regex TXregVid = new Regex(VID.ToLower());
            Regex ExternAgs = new Regex(col.ToLower());
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

                        SpDeviceInterfaceDetailData detailData = new SpDeviceInterfaceDetailData
                        {
                            Size = 5    // hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it, mail me! Thx
                        };
                        result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, ref detailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero);//获取接口的详细信息，必须调用两次，一次返回长度，二次获取数据

                        if (result)
                        {
                            if (detailData.DevicePath != null)
                            {
                                Match MatchPid = TXregPid.Match(detailData.DevicePath);
                                Match MatchVID = TXregVid.Match(detailData.DevicePath);
                                Match mathExternAgs = ExternAgs.Match(detailData.DevicePath);

                                if (MatchPid.Success && MatchVID.Success && mathExternAgs.Success)
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
            catch
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

        #region 对外方法
        /// <summary>
        /// Headset句柄对象数组
        /// </summary>
        public static IntPtr[] headsethandle = new IntPtr[6];
        /// <summary>
        /// Dongle句柄对象数组
        /// </summary>
        public static IntPtr[] donglehandle = new IntPtr[6];
        /// <summary>
        /// Headset句柄地址
        /// </summary>
        public static string[] headsetpath = new string[6];
        /// <summary>
        /// Dongle句柄地址
        /// </summary>
        public static string[] donglepath = new string[6];
        /// <summary>
        /// 抓取句柄 donglePIDVID不能为NULL
        /// </summary>
        /// <param name="headsetPID">headsetPID</param>
        /// <param name="headsetVID">headsetVID</param>
        /// <param name="donglePID">donglePID</param>
        /// <param name="dongleVID">dongleVID</param>
        /// <returns>抓取是否成功</returns>
        public static bool gethandle(string headsetPID, string headsetVID, string donglePID, string dongleVID)
        {
            bool flag = true;
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    headsetpath[i] = "";
                    donglepath[i] = "";
                }
                flag = GetHidDevicePath(headsetPID, headsetVID, donglePID, dongleVID);
                for (int i = 0; i < 6; i++)
                {
                    headsethandle[i] = GetHidDeviceHandle(headsetpath[i]);
                    donglehandle[i] = GetHidDeviceHandle(donglepath[i]);
                }

            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        public static bool getPath(string PID, string VID, string col)
        {
            bool flag = true;
            try
            {
                flag = GetHidDevicePath(PID, VID, col);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        #endregion
        #region 释放句柄
        /// <summary>
        /// 释放句柄
        /// </summary>
        public static void Dispose()
        {
            try
            {
                foreach (IntPtr handle in headsethandle)
                {

                    if (handle != IntPtr.Zero) CloseHandle(handle);
                }
                foreach (IntPtr handle in donglehandle)
                {

                    if (handle != IntPtr.Zero) CloseHandle(handle);
                }
                for (int i = 0; i < 6; i++)
                {
                    donglepath[i] = "";
                    headsetpath[i] = "";
                }
            }
            catch
            {

            }
        }





        #endregion
    }
}
