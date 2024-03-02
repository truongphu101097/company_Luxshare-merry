### 模块名称

CRY574

### 使用前提

1. CRY574.dll 目录下存在[\CRY574Pro(20190213-1.0.25)|CRY574PRO.exe]()
2. 接入了 CRY574 蓝牙适配仪
4. 设置了虚拟串口（ COM200 -> COM201、COM200 -> COM201 ）
5. CRY574PRO.exe 的串口设置为 COM200
6. SN 框长度设置为12位

### 支持的测试项目

#### Connect

指定 MAC 地址与 CRY574 进行连接。
运行逻辑：断开连接 -> 等待5秒 -> 连接，最多重试两次。
标准品条码不做操作。

**参数**

| 名称              | 作用              | 备注                                                         |
| ----------------- | ----------------- | ------------------------------------------------------------ |
| getMacAddressFrom | 获取Mac地址的方式 | 可选项：SN、BitAddress<br />返回值等于 Config[getMacAddressFrom] |

#### EnterA2DP_Mode

切换到 A2DP 模式

#### EnterHFP_Mode

切换到 HFP 模式

#### Disconnect

与 CRY574 断开连接