using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Ai.Hong.Driver
{
    #region device enum defines

    /// <summary>
    /// 错误代码
    /// </summary>
    public enum EnumHardwareError
    {
        /// <summary>
        /// OK
        /// </summary>
        [Description("正确,OK")]
        OK = 0, //正确
                /// <summary>
                /// Driver Error
                /// </summary>
        [Description("设备驱动错误,Driver Error")]
        Driver = 01,
        /// <summary>
        /// Communication Port Error
        /// </summary>
        [Description("通信端口错误,Communication Port Error")]
        CommPort = 02,    //通信端口错误
                          /// <summary>
                          /// xxx Not Found
                          /// </summary>
        [Description("没找到, Not Found")]
        NotFound = 03,    //没找到XXX
                          /// <summary>
                          /// Device Not Connected
                          /// </summary>
        [Description("仪器未连接,Device Not Connected")]
        NotConnect = 04,  //仪器未连接
                          /// <summary>
                          /// xxx Not Ready
                          /// </summary>
        [Description("未准备好, Not Ready")]
        NotReady = 05,    //XXX未准备好
                          /// <summary>
                          /// xxx Warning
                          /// </summary>
        [Description("报警, Warning")]
        Warning = 06,     //XXX报警
                          /// <summary>
                          /// xxx Fault
                          /// </summary>
        [Description("故障, Fault")]
        Fault = 07,       //XXX故障
                          /// <summary>
                          /// xxx Busy
                          /// </summary>
        [Description("被占用, Busy")]
        Busy = 08,        //XXX被占用
                          /// <summary>
                          /// xxx Is Read Only
                          /// </summary>
        [Description("参数不能修改, Is Read Only")]
        ReadOnly = 09,    //参数不能修改
                          /// <summary>
                          /// xxx Range Error
                          /// </summary>
        [Description("参数范围错误, Range Error")]
        Range = 10,       //参数范围错误
                          /// <summary>
                          /// xxx Format Error
                          /// </summary>
        [Description("参数格式错误, Format Error")]
        Format = 11,      //参数格式错误
    };

    /// <summary>
    /// 仪器包含的所有硬件
    /// </summary>
    public enum EnumHardware
    {
        /// <summary>
        /// Device
        /// </summary>
        [Description("整机,Device")]
        Device = 00,
        /// <summary>
        /// Main board
        /// </summary>
        [Description("主板,Board")]
        Board = 01,
        /// <summary>
        /// Light Source
        /// </summary>
        [Description("光源,Source")]
        Source = 02,
        /// <summary>
        /// Laser
        /// </summary>
        [Description("激光器,Laser")]
        Laser = 03,
        /// <summary>
        /// Interferometer
        /// </summary>
        [Description("干涉仪,Interferometer")]
        Interferometer = 04,
        /// <summary>
        /// Power
        /// </summary>
        [Description("电源,Power")]
        Power = 05,
        /// <summary>
        /// Detector
        /// </summary>
        [Description("检测器,Detector")]
        Detector = 06,
        /// <summary>
        /// Inner Background
        /// </summary>
        [Description("内置背景,Inner Background")]
        Background = 07,
        /// <summary>
        /// Cooler
        /// </summary>
        [Description("冷却器,Cooler")]
        Cooler = 08,
        /// <summary>
        /// Drier
        /// </summary>
        [Description("干燥器,Drier")]
        Drier = 09,

        /// <summary>
        /// Optical Switcher
        /// </summary>
        [Description("光路开关,Optical Switcher")]
        OpticalSwitcher = 12,
        /// <summary>
        /// Sample Switcher
        /// </summary>
        [Description("样品开关,Sample Switcher")]
        SampleSwitcher = 13,
        /// <summary>
        /// Background Gain
        /// </summary>
        [Description("背景增益,Background Gain")]
        BackGain = 14,
        /// <summary>
        /// Sample Gain
        /// </summary>
        [Description("样品增益,Sample Gain")]
        SampleGain = 15,
        /// <summary>
        /// Aperture
        /// </summary>
        [Description("光阑,Aperture")]
        Aperture = 16,
        /// <summary>
        /// Inner verify IVU
        /// </summary>
        [Description("IVU内部验证单元,IVU Filter")]
        IVU = 17,
        /// <summary>
        /// High Filter
        /// </summary>
        [Description("高通滤波,High Filter")]
        HighFilter = 18,
        /// <summary>
        /// Low Filter
        /// </summary>
        [Description("低通滤波,Low Filter")]
        LowFilter = 19,
        /// <summary>
        /// Sphere integrator
        /// </summary>
        [Description("积分球,Sphere Integrator")]
        Sphere = 22,
        /// <summary>
        /// Fiber
        /// </summary>
        [Description("光纤,Fiber")]
        Fiber = 23,
        /// <summary>
        /// Liquid Cell
        /// </summary>
        [Description("液体池,Liqude Cell")]
        Cell = 24,
        /// <summary>
        /// Gas Cell
        /// </summary>
        [Description("气体池,Gas Cell")]
        GasCell = 25,
        /// <summary>
        /// Sample Rotator
        /// </summary>
        [Description("旋转台,Rotator")]
        Rotator = 26,
    };

    /// <summary>
    /// 硬件所有的属性
    /// </summary>
    public enum EnumHardwareProperties
    {
        /// <summary>
        /// Status
        /// </summary>
        [Description("状态（所有部件）")]
        Status = 00,
        /// <summary>
        /// Serial Number
        /// </summary>
        [Description("序列号（所有部件）")]
        SerialNo = 01,
        /// <summary>
        /// Temperature
        /// </summary>
        [Description("温度（所有部件）")]
        Temperature = 02,
        /// <summary>
        /// ProduceDate
        /// </summary>
        [Description("生产日期（所有部件）")]
        ProduceDate = 03,

        /// <summary>
        /// DeviceType
        /// </summary>
        [Description("仪器类型（整机，01=NIR，02=IR...）")]
        DeviceCategory = 06,
        /// <summary>
        /// DeviceModel
        /// </summary>
        [Description("仪器型号（整机01=Sphere, 02=Fiber, 03=Cell... ）")]
        DeviceModel = 07,
        /// <summary>
        /// Resolution
        /// </summary>
        [Description("分辨率（整机）")]
        Resolution = 08,
        /// <summary>
        /// MaxFrequency
        /// </summary>
        [Description("最高采样频率（整机）")]
        MaxFrequency = 11,
        /// <summary>
        /// MinFrequency
        /// </summary>
        [Description("最低采样频率（整机）")]
        MinFrequency = 12,

        /// <summary>
        /// Source
        /// </summary>
        [Description("光源")]
        Source = 15,
        /// <summary>
        /// Voltage
        /// </summary>
        [Description("电压（主板，电源，光源）")]
        Voltage = 16,
        /// <summary>
        /// Current
        /// </summary>
        [Description("电流（主板，电源，光源）")]
        Current = 17,
        /// <summary>
        /// Humidity
        /// </summary>
        [Description("湿度（整机，干燥器），")]
        Humidity = 18,
        /// <summary>
        /// Intensity
        /// </summary>
        [Description("强度（光源，激光器，检测器）")]
        Intensity = 19,
        /// <summary>
        /// Wavelength
        /// </summary>
        [Description("激光波数")]
        Wavelength = 20,
        /// <summary>
        /// DetectorType
        /// </summary>
        [Description("检测器类型（0=InGaAs，1=PbS...）")]
        DetectorType = 21,
        /// <summary>
        /// Position
        /// </summary>
        [Description("背景（0=光路外，1=光路中），光澜（00=开=光全部通过，01=3/4开，02=1/2开，03=1/4开，04=关）,干涉仪（0=停止，1=转动），滤光片（0=空，1=玻璃，2=聚苯乙烯...），高通滤波（00=启用，01=禁用），低通滤波（00=自动，01=10KHZ，02=禁用），增益（00=Auto，01=一级增益，02=二级增益，03=三级增益）")]
        Position = 22,
        /// <summary>
        /// ScanMode
        /// </summary>
        [Description("采样方式（整机，01=DoubleSidedFB，02=SingleSidedFB，03=DoubleSided，04=SingleSided）")]
        ScanMode = 23,
        /// <summary>
        /// ScanChannel
        /// </summary>
        [Description("采样通道")]
        ScanChannel = 24,
    };

    /// <summary>
    /// 硬件状态
    /// </summary>
    public enum EnumHardwareStatus
    {
        /// <summary>
        /// OK
        /// </summary>
        [Description("正常,OK")]
        OK = 0,
        /// <summary>
        /// NotFound
        /// </summary>
        [Description("未找到,NotFound")]
        NotFound = 1,
        /// <summary>
        /// Warning
        /// </summary>
        [Description("警告,Warning")]
        Warning = 2,
        /// <summary>
        /// Busy
        /// </summary>
        [Description("忙,Busy")]
        Busy = 3,
        /// <summary>
        /// Fault
        /// </summary>
        [Description("错误,Fault")]
        Fault = 4,
        /// <summary>
        /// NotReady
        /// </summary>
        [Description("未就绪,NotReady")]
        NotReady = 5,
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public enum EnumDeviceCategory
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("未知,Unknown")]
        Unknown = 0,
        /// <summary>
        /// FTNIR
        /// </summary>
        [Description("近红外,FTNIR")]
        FTNIR = 1,
        /// <summary>
        /// FTIR
        /// </summary>
        [Description("中红外,FTIR")]
        FTIR = 2,
    }

    /// <summary>
    /// 设备型号（可以组合）
    /// </summary>
    public enum EnumDeviceModel
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("未知,Unknown")]
        Unknown = 0,
        /// <summary>
        /// SphereIntegrate
        /// </summary>
        [Description("积分球,Sphere Integrator")]
        SphereIntegrate = 1,
        /// <summary>
        /// Fiber
        /// </summary>
        [Description("光纤,Fiber")]
        Fiber = 2,
        /// <summary>
        /// Liquid Cell
        /// </summary>
        [Description("液体池, Liquid Cell")]
        Cell = 4,
        /// <summary>
        /// Gas Cell
        /// </summary>
        [Description("气体池,Gas Cell")]
        GasCell = 8,
        /// <summary>
        /// Solid Online
        /// </summary>
        [Description("在线,Online")]
        Online = 16
    }

    /// <summary>
    /// 检测器类型（可以组合）
    /// </summary>
    public enum EnumDeviceDetector
    {
        /// <summary>
        /// Unkown
        /// </summary>
        [Description("未知,Unkown")]
        Unkown = 0,
        /// <summary>
        /// InGaAs
        /// </summary>
        [Description("铟镓砷检测器,InGaAs")]
        InGaAs = 1,
        /// <summary>
        /// PbS
        /// </summary>
        [Description("硫化铅检测器,PbS")]
        PbS = 2,
        /// <summary>
        /// DTGS
        /// </summary>
        [Description("DTGS检测器,DTGS")]
        DTGS = 4,
        /// <summary>
        /// MCT
        /// </summary>
        [Description("MCT检测器,MCT")]
        MCT = 8,
        /// <summary>
        /// CCD
        /// </summary>
        [Description("CCD检测器,CCD")]
        CCD = 16,
    }

    /// <summary>
    /// 光阑开关状态
    /// </summary>
    public enum EnumDeviceAperture
    {
        /// <summary>
        /// Open
        /// </summary>
        [Description("打开,Open")]
        Open = 0,
        /// <summary>
        /// Three_Quater
        /// </summary>
        [Description("3/4打开,Three Quater Open")]
        Three_Quater = 1,
        /// <summary>
        /// Half
        /// </summary>
        [Description("1/2打开,Half Open")]
        Half = 2,
        /// <summary>
        /// Quarter
        /// </summary>
        [Description("1/4打开,Quarter Open")]
        Quarter = 3,
        /// <summary>
        /// Off
        /// </summary>
        [Description("关闭,Off")]
        Off = 4,
    }

    /// <summary>
    /// 转轮状态
    /// </summary>
    public enum EnumDeviceIVU
    {
        /// <summary>
        /// Empty
        /// </summary>
        [Description("空,Empty")]
        Empty = 0,
        /// <summary>
        /// Glass
        /// </summary>
        [Description("玻璃,Glass")]
        Glass = 1,
        /// <summary>
        /// Polystyrene
        /// </summary>
        [Description("聚苯乙烯,Polystyrene")]
        Polystyrene = 2,
    }

    /// <summary>
    /// 高通滤波状态
    /// </summary>
    public enum EnumDeviceHighFiler
    {
        /// <summary>
        /// Disabled
        /// </summary>
        [Description("禁用,Disabled")]
        Disabled = 0,
        /// <summary>
        /// Enabled
        /// </summary>
        [Description("启用,Enabled")]
        Enabled = 1,
    }

    /// <summary>
    /// 低通滤波状态
    /// </summary>
    public enum EnumDeviceLowFiler
    {
        /// <summary>
        /// Disabled
        /// </summary>
        [Description("禁用,Disabled")]
        Disabled = 0,
        /// <summary>
        /// Automatic
        /// </summary>
        [Description("自动,Automatic")]
        Automatic = 1,
        /// <summary>
        /// Khz_10
        /// </summary>
        [Description("10KHZ,10 Khz")]
        Khz_10 = 2,
    }

    /// <summary>
    /// 增益
    /// </summary>
    public enum EnumDeviceGain
    {
        /// <summary>
        /// Disabled
        /// </summary>
        [Description("禁用,Disabled")]
        Disabled = 0,
        /// <summary>
        /// Automatic
        /// </summary>
        [Description("自动,Automatic")]
        Automatic = -1,
        /// <summary>
        /// Gain level 1
        /// </summary>
        [Description("一级增益,Gain level 1")]
        Gain_1 = 1,
        /// <summary>
        /// Gain level 2
        /// </summary>
        [Description("二级增益,Gain level 2")]
        Gain_2 = 2,
        /// <summary>
        /// Gain level 3
        /// </summary>
        [Description("三级增益,Gain level 3")]
        Gain_3 = 3,
        /// <summary>
        /// Gain level 4
        /// </summary>
        [Description("四级增益,Gain level 4")]
        Gain_4 = 4,
        /// <summary>
        /// Gain level 4
        /// </summary>
        [Description("五级增益,Gain level 5")]
        Gain_5 = 5,
        /// <summary>
        /// Gain level 4
        /// </summary>
        [Description("六级增益,Gain level 6")]
        Gain_6 = 6,
        /// <summary>
        /// Gain level 4
        /// </summary>
        [Description("七级增益,Gain level 7")]
        Gain_7 = 7,
        /// <summary>
        /// Gain level 4
        /// </summary>
        [Description("八级增益,Gain level 8")]
        Gain_8 = 8,
    }

    /// <summary>
    /// 分光器位置（EnumHardware.SampleSwitcher）
    /// </summary>
    public enum EnumDeviceOpticalSwitcher
    {
        /// <summary>
        /// Closed
        /// </summary>
        [Description("关闭,Closed")]
        Closed = 0,
        /// <summary>
        /// SpherIntegrate
        /// </summary>
        [Description("积分球,Sphere Integrator")]
        SphereIntegrate = 1,
        /// <summary>
        /// Fibler
        /// </summary>
        [Description("光纤,Fibler")]
        Fibler = 2,
        /// <summary>
        /// Cell
        /// </summary>
        [Description("液体池,Liquid Cell")]
        Cell = 3,
        /// <summary>
        /// GasCell
        /// </summary>
        [Description("气体池,Gas Cell")]
        GasCell = 4,
        /// <summary>
        /// GasCell
        /// </summary>
        [Description("在线,On Line")]
        Online = 5
    }

    /// <summary>
    /// 采样模式
    /// </summary>
    public enum EnumDeviceScanMode
    {
        /// <summary>
        /// Double Sided Forward Backward
        /// </summary>
        [Description("双边往返采样,Double Sided FB")]
        DoubleSidedFB = 1,
        /// <summary>
        /// Single Sided Forward Backward
        /// </summary>
        [Description("单边往返采样,Single Sided FB")]
        SingleSidedFB = 2,
        /// <summary>
        /// Double Sided
        /// </summary>
        [Description("双边单次采样,Double Sided")]
        DoubleSided = 3,
        /// <summary>
        /// Single Sided
        /// </summary>
        [Description("单边单次采样,Single Sided")]
        SingleSided = 4
    }

    /// <summary>
    /// 开关状态（背景，旋转台，干涉仪，样品开关）
    /// </summary>
    public enum EnumDeviceSwitcher
    {
        /// <summary>
        /// Off
        /// </summary>
        [Description("关闭,Off")]
        Off = 0,
        /// <summary>
        /// On
        /// </summary>
        [Description("开启,On")]
        On = 1
    }

    /// <summary>
    /// 分辨率列表
    /// </summary>
    public enum EnumDeviceResolutions
    {
        /// <summary>
        /// 1 cm-1
        /// </summary>
        [Description("1 cm-1,1 cm-1")]
        res_1 = 1,
        /// <summary>
        /// 2 cm-1
        /// </summary>
        [Description("2 cm-1,2 cm-1")]
        res_2 = 2,
        /// <summary>
        /// 4 cm-1
        /// </summary>
        [Description("4 cm-1,4 cm-1")]
        res_4 = 4,
        /// <summary>
        /// 8 cm-1
        /// </summary>
        [Description("8 cm-1,8 cm-1")]
        res_8 = 8,
        /// <summary>
        /// 16 cm-1
        /// </summary>
        [Description("16 cm-1,16 cm-1")]
        res_16 = 16,
        /// <summary>
        /// 32 cm-1
        /// </summary>
        [Description("32 cm-1,32 cm-1")]
        res_32 = 32,
        /// <summary>
        /// 64 cm-1
        /// </summary>
        [Description("64 cm-1,64 cm-1")]
        res_64 = 64,
        /// <summary>
        /// 128 cm-1
        /// </summary>
        [Description("128 cm-1,128 cm-1")]
        res_128 = 128,
    }

    /// <summary>
    /// 仪器测量通道
    /// </summary>
    public enum EnumDeviceChannels
    {
        /// <summary>
        /// Sphere Background
        /// </summary>
        [Description("积分球背景通道,Sphere Back Channel")]
        SphereBackground = 1,
        /// <summary>
        /// Sphere Sample
        /// </summary>
        [Description("积分球样品通道,Sphere Sample Channel")]
        SphereSample = 2,
        /// <summary>
        /// Fiber Background
        /// </summary>
        [Description("光纤背景通道,Fiber Back Channel")]
        FiberBackground = 3,
        /// <summary>
        /// Fiber Sample
        /// </summary>
        [Description("光纤样品通道,Fiber Sample Channel")]
        FiberSample = 4,
        /// <summary>
        /// Cell Background
        /// </summary>
        [Description("液体池背景通道,Cell Back Channel")]
        CellBackground = 5,
        /// <summary>
        /// Cell Sample
        /// </summary>
        [Description("液体池样品通道,Cell Sample Channel")]
        CellSample = 6,
        /// <summary>
        /// Out Background
        /// </summary>
        [Description("外置背景通道,Outer Back Channel")]
        OutBackground = 7,
        /// <summary>
        /// Out Sample
        /// </summary>
        [Description("外置样品通道,Outer Sample Channel")]
        OutSample = 8
    }

    /// <summary>
    /// 光源(可以组合)
    /// </summary>
    public enum EnumDeviceSource
    {
        /// <summary>
        /// Off
        /// </summary>
        [Description("无,Off")]
        Off = 0,
        /// <summary>
        /// Source 1
        /// </summary>
        [Description("光源1,Source 1")]
        Source1 = 1,
        /// <summary>
        /// Source 2
        /// </summary>
        [Description("光源2,Source 1")]
        Source2 = 2,
        /// <summary>
        /// Source 3
        /// </summary>
        [Description("光源3,Source 1")]
        Source3 = 4,
        /// <summary>
        /// Source 4
        /// </summary>
        [Description("光源4,Source 1")]
        Source4 = 8,
    }

    /// <summary>
    /// Instrument model
    /// </summary>
    public enum EnumDeviceBrand
    {
        /// <summary>
        /// Default
        /// </summary>
        [Description("缺省,Default")]
        Default,
        /// <summary>
        /// Bruer MPA
        /// </summary>
        [Description("布鲁克MPA,Bruer MPA")]
        Bruer_MPA,
        /// <summary>
        /// Bruer Tango
        /// </summary>
        [Description("布鲁克Tango,Bruer Tango")]
        Bruer_Tango,
        /// <summary>
        /// Bruer Matrix_E
        /// </summary>
        [Description("布鲁克Matrix-E,Bruer Matrix-E")]
        Bruer_Matrix_E,
        /// <summary>
        /// Bruer Matrix_F
        /// </summary>
        [Description("布鲁克Matrix-F,Bruer Matrix-F")]
        Bruer_Matrix_F,
        /// <summary>
        /// Bruer Matrix_I
        /// </summary>
        [Description("布鲁克Matrix-I,Bruer Matrix-I")]
        Bruer_Matrix_I,
        /// <summary>
        /// Catron IIS100
        /// </summary>
        [Description("开创 IIS100,Catron IIS100")]
        Catron_IIS100,
        /// <summary>
        /// CFTIR GAS
        /// </summary>
        [Description("CFTIR GAS,CFTIR GAS")]
        CFTIR_GAS,
        /// <summary>
        /// Long light integrate sphere
        /// </summary>
        [Description("九光 积分球,LongLight Integrate")]
        LongLight_Integrate,
        /// <summary>
        /// Thermo AntarisII
        /// </summary>
        [Description("赛默飞 AntarisII,Thermo AntarisII")]
        Thermo_AntarisII,
        /// <summary>
        /// Vspec Fiber
        /// </summary>
        [Description("威斯派克 2000,Vspec 2000")]
        Vspec_2000,
        /// <summary>
        /// Vspec Integrating Sphere
        /// </summary>
        [Description("威斯派克 3000,Vspec 3000")]
        Vspec_3000,
        /// <summary>
        /// Vspec QuasIR
        /// </summary>
        [Description("威斯派克 4000,Vspec 4000")]
        Vspec_4000,
    }

    /// <summary>
    /// Instrument factor
    /// </summary>
    public enum EnumDeviceFactor
    {
        /// <summary>
        /// Bruker
        /// </summary>
        [Description("布鲁克,Bruker")]
        Bruker,
        /// <summary>
        /// Catron
        /// </summary>
        [Description("开创,Catron")]
        Catron,
        /// <summary>
        /// LongLight
        /// </summary>
        [Description("九光,LongLight")]
        LongLight,
        /// <summary>
        /// Thermo
        /// </summary>
        [Description("赛默飞,Thermo")]
        Thermo,
        /// <summary>
        /// Vspec
        /// </summary>
        [Description("威斯派克,Vspec")]
        Vspec,
    }

    /// <summary>
    /// 设备采集命令
    /// </summary>
    public enum EnumAcquireCommand
    {
        /// <summary>
        /// Start
        /// </summary>
        [Description("开始,Start")]
        Start = 0,
        /// <summary>
        /// Stop acquire, Wait for device 
        /// </summary>
        [Description("停止,Stop")]
        Stop = 1,
    }

    /// <summary>
    /// 设备采集的数据格式
    /// </summary>
    public enum EnumAcquireDataType
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        [Description("原始数据,Raw Data")]
        Raw = 0,
        /// <summary>
        /// 背景干涉谱
        /// </summary>
        [Description("背景干涉谱,Back Interfergoram")]
        BackInterfer = 1,
        /// <summary>
        /// 样品干涉谱
        /// </summary>
        [Description("样品干涉谱Sample Interfergoram")]
        SampleInterfer = 2,
        /// <summary>
        /// 背景单通道谱
        /// </summary>
        [Description("背景单通道谱,Back Single Beam")]
        BackBeam = 3,
        /// <summary>
        /// 样品单通道谱
        /// </summary>
        [Description("样品单通道谱,Sample Single Beam")]
        SampleBeam = 4,
        /// <summary>
        /// 吸收谱
        /// </summary>
        [Description("吸收谱,Absorption")]
        Absorption = 5,
        /// <summary>
        /// 透射谱
        /// </summary>
        [Description("透射谱,Transmission")]
        Transmission = 6,
        /// <summary>
        /// 发射谱
        /// </summary>
        [Description("发射谱,Emission")]
        Emission = 7,
        /// <summary>
        /// 反射谱
        /// </summary>
        [Description("反射谱,Reflection")]
        Reflection = 8,
    }

    /// <summary>
    /// 扫描通知状态枚举
    /// </summary>
    public enum EnumScanNotifyState
    {
        /// <summary>
        /// 还没有开始扫描
        /// </summary>
        Idel = 0,
        /// <summary>
        /// 正在扫描
        /// </summary>
        Scanning = 1,
        /// <summary>
        /// 完成一次检测
        /// </summary>
        oneFinished = 2,
        /// <summary>
        /// 完成全部重复（扫描结束）
        /// </summary>
        repeateFinished = 3,
        /// <summary>
        /// 参数错误
        /// </summary>
        parameterError = 4,
        /// <summary>
        /// 设备错误
        /// </summary>
        deviceError = 5,
        /// <summary>
        /// 文件错误
        /// </summary>
        fileError = 6,
        /// <summary>
        /// 用户取消
        /// </summary>
        userAbort = 7,
        /// <summary>
        /// 需要扫描背景
        /// </summary>
        backgroundError = 8,
    }

    #endregion

    #region properties value defines

    ///// <summary>
    ///// 驱动使用的语言类型枚举
    ///// </summary>
    //public enum EnumLanguage
    //{
    //    /// <summary>
    //    /// Chinese
    //    /// </summary>
    //    Chinese = 0,
    //    /// <summary>
    //    /// English
    //    /// </summary>
    //    English = 1,
    //}

    /// <summary>
    /// 结果谱图类型
    /// </summary>
    public enum EnumResultSpectrum
    {
        /// <summary>
        /// Absorbance
        /// </summary>
        [Description("吸收谱,Absorbance")]
        Absorbance = 0,
        /// <summary>
        /// Transmittance
        /// </summary>
        [Description("透过谱,Transmittance")]
        Transmittance = 1,
        /// <summary>
        /// Kubelka_Munk
        /// </summary>
        [Description("Kubelka Munk谱,Kubelka Munk")]
        Kubelka_Munk = 2,
        /// <summary>
        /// Reflectance
        /// </summary>
        [Description("反射谱,Reflectance")]
        Reflectance = 3,
        /// <summary>
        /// Log Reflectance
        /// </summary>
        [Description("Log 反射谱,Log Reflectance")]
        Log_Reflectance = 4,
    }

    /// <summary>
    /// 属性所属的类型
    /// </summary>
    public enum EnumPropCategory
    {
        /// <summary>
        /// 光学参数
        /// </summary>
        optical = 0,
        /// <summary>
        /// 电子参数
        /// </summary>
        electric = 1,
        /// <summary>
        /// 采集参数
        /// </summary>
        acquire = 2,
    }

    /// <summary>
    /// 相位分辨率
    /// </summary>
    public enum EnumFTPhaseResolution
    {
        /// <summary>
        /// 8
        /// </summary>
        [Description("8")]
        Res_8 = 8,
        /// <summary>
        /// 16
        /// </summary>
        [Description("16")]
        Res_16 = 16,
        /// <summary>
        /// 32
        /// </summary>
        [Description("32")]
        Res_32 = 32,
        /// <summary>
        /// 64
        /// </summary>
        [Description("64")]
        Res_64 = 64,
        /// <summary>
        /// 128
        /// </summary>
        [Description("128")]
        Res_128 = 128,
        /// <summary>
        /// 256
        /// </summary>
        [Description("256")]
        Res_256 = 256,
    }

    /// <summary>
    /// FT相位校正方法
    /// </summary>
    public enum EnumFTPhaseCorrect
    {
        /// <summary>
        /// Mertz
        /// </summary>
        [Description("Mertz")]
        Mertz = 1,
        /// <summary>
        /// 
        /// </summary>
        [Description("Mertz Signed")]
        MertzSigned = 2,
        /// <summary>
        /// 
        /// </summary>
        [Description("Power Spectrum")]
        PowerSpectrum = 3,
        /// <summary>
        /// Mertz / No Peak Searched
        /// </summary>
        [Description("Mertz / No Peak Searched")]
        Mertz_NoPeakSearched = 4,
        /// <summary>
        /// Mertz Signed / No Peak Searched
        /// </summary>
        [Description("Mertz Signed / No Peak Searched")]
        MertzSigned_NoPeakSearched = 5,
        /// <summary>
        /// Power / No Peak Searched
        /// </summary>
        [Description("Power / No Peak Searched")]
        Power_NoPeakSearched = 6,
        /// <summary>
        /// Mertz / Stored Phase
        /// </summary>
        [Description("Mertz / Stored Phase")]
        Mertz_StoredPhase = 7,
        /// <summary>
        /// No / Save Complex Data
        /// </summary>
        [Description("No / Save Complex Data")]
        No_SaveComplexData = 8,
        /// <summary>
        /// Forman
        /// </summary>
        [Description("Forman")]
        Forman = 9,
        /// <summary>
        /// Forman / Stored Phase
        /// </summary>
        [Description("Forman / Stored Phase")]
        Forman_StoredPhase = 10,
        /// <summary>
        /// Forman / Preapodized
        /// </summary>
        [Description("Forman / Preapodized")]
        Forman_Preapodized = 11,
        /// <summary>
        /// Double Phase
        /// </summary>
        [Description("Double Phase")]
        DoublePhase = 12,
        /// <summary>
        /// Mertz / Full Range Peak Search
        /// </summary>
        [Description("Mertz / Full Range Peak Search")]
        Mertz_FullRangePeakSearch = 13,
    }

    /// <summary>
    /// FT截趾函数
    /// </summary>
    public enum EnumFTApodization
    {
        /// <summary>
        /// BoxCar
        /// </summary>
        [Description("BoxCar")]
        BoxCar = 1,
        /// <summary>
        /// Triangular
        /// </summary>
        [Description("Triangular")]
        Triangular = 2,
        /// <summary>
        /// 
        /// </summary>
        [Description("FourPoint")]
        FourPoint = 3,
        /// <summary>
        /// Happ Genzel
        /// </summary>
        [Description("Happ Genzel")]
        Happ_Genzel = 4,
        /// <summary>
        /// Blackman Harris 3 Term
        /// </summary>
        [Description("Blackman Harris 3 Term")]
        Blackman_Harris_3_Term = 5,
        /// <summary>
        /// Blackman Harris 4 Term
        /// </summary>
        [Description("Blackman Harris 4 Term")]
        Blackman_Harris_4_Term = 6,
        /// <summary>
        /// Norton Beer Weak
        /// </summary>
        [Description("Norton Beer Weak")]
        Norton_Beer_Weak = 7,
        /// <summary>
        /// Norton Beer Medium
        /// </summary>
        [Description("Norton Beer Medium")]
        Norton_Beer_Medium = 8,
        /// <summary>
        /// Norton Beer Strong
        /// </summary>
        [Description("Norton Beer Strong")]
        Norton_Beer_Strong = 9,
        /// <summary>
        /// User One
        /// </summary>
        [Description("User One")]
        UserOne = 10,
        /// <summary>
        /// User Two
        /// </summary>
        [Description("User Two")]
        UserTwo = 11
    }

    /// <summary>
    /// FT填零
    /// </summary>
    public enum EnumFTZeroFilling
    {
        /// <summary>
        /// Filling 0
        /// </summary>
        [Description("Factor 0")]
        Filling_0 = 0,
        /// <summary>
        /// Filling 1
        /// </summary>
        [Description("Factor 1")]
        Filling_1 = 1,
        /// <summary>
        /// Filling 2
        /// </summary>
        [Description("Factor 2")]
        Filling_2 = 2,
        /// <summary>
        /// Filling 4
        /// </summary>
        [Description("Factor 4")]
        Filling_4 = 4,
        /// <summary>
        /// Filling 8
        /// </summary>
        [Description("Factor 8")]
        Filling_8 = 8,
        /// <summary>
        /// Filling 16
        /// </summary>
        [Description("Factor 16")]
        Filling_16 = 16,
        /// <summary>
        /// Filling 32
        /// </summary>
        [Description("Factor 32")]
        Filling_32 = 32,
        /// <summary>
        /// Filling 64
        /// </summary>
        [Description("Factor 64")]
        Filling_64 = 64,
        /// <summary>
        /// Filling 128
        /// </summary>
        [Description("Factor 128")]
        Filling_128 = 128,
        /// <summary>
        /// Filling 0
        /// </summary>
        [Description("Factor 256")]
        Filling_256 = 256,
    }

    /// <summary>
    /// 扫描次数
    /// </summary>
    public enum EnumScanCount
    {
        /// <summary>
        /// ScanCount 1
        /// </summary>
        [Description("1")]
        ScanCount_1 = 1,
        /// <summary>
        /// ScanCount 2
        /// </summary>
        [Description("2")]
        ScanCount_2 = 2,
        /// <summary>
        /// ScanCount 4
        /// </summary>
        [Description("4")]
        ScanCount_4 = 4,
        /// <summary>
        /// ScanCount 8
        /// </summary>
        [Description("8")]
        ScanCount_8 = 8,
        /// <summary>
        /// ScanCount 16
        /// </summary>
        [Description("16")]
        ScanCount_16 = 16,
        /// <summary>
        /// ScanCount 32
        /// </summary>
        [Description("32")]
        ScanCount_32 = 32,
        /// <summary>
        /// ScanCount 64
        /// </summary>
        [Description("64")]
        ScanCount_64 = 64,
        /// <summary>
        /// ScanCount 128
        /// </summary>
        [Description("128")]
        ScanCount_128 = 128,
        /// <summary>
        /// ScanCount 256
        /// </summary>
        [Description("256")]
        ScanCount_256 = 256,
    }

    /// <summary>
    /// 重复扫描次数
    /// </summary>
    public enum EnumRepeatCount
    {
        /// <summary>
        /// Repeat 0
        /// </summary>
        [Description("0")]
        Repeat_0 = 0,
        /// <summary>
        /// Repeat 1
        /// </summary>
        [Description("1")]
        Repeat_1 = 1,
        /// <summary>
        /// Repeat 2
        /// </summary>
        [Description("2")]
        Repeat_2 = 2,
        /// <summary>
        /// Repeat 3
        /// </summary>
        [Description("3")]
        Repeat_3 = 3,
        /// <summary>
        /// Repeat 4
        /// </summary>
        [Description("4")]
        Repeat_4 = 4,
        /// <summary>
        /// Repeat 5
        /// </summary>
        [Description("5")]
        Repeat_5 = 5,
        /// <summary>
        /// Repeat 6
        /// </summary>
        [Description("6")]
        Repeat_6 = 6,
        /// <summary>
        /// Repeat 7
        /// </summary>
        [Description("7")]
        Repeat_7 = 7,
        /// <summary>
        /// Repeat 8
        /// </summary>
        [Description("8")]
        Repeat_8 = 8,
        /// <summary>
        /// Repeat 9
        /// </summary>
        [Description("9")]
        Repeat_9 = 9,
    }

    /// <summary>
    /// 背景光谱的有效期
    /// </summary>
    public enum EnumBackgroundDuration
    {
        /// <summary>
        /// 15 minutes
        /// </summary>
        [Description("5分钟,5 minutes")]
        Duration_5 = 5,
        /// <summary>
        /// 15 minutes
        /// </summary>
        [Description("15分钟,15 minutes")]
        Duration_15 = 15,
        /// <summary>
        /// 30 minutes
        /// </summary>
        [Description("30分钟,30 minutes")]
        Duration_30 = 30,
        /// <summary>
        /// 60 minutes
        /// </summary>
        [Description("1小时,1 Hour")]
        Duration_60 = 60,
        /// <summary>
        /// 120 minutes
        /// </summary>
        [Description("2小时,2 Hours")]
        Duration_120 = 120,
        /// <summary>
        /// 240 minutes
        /// </summary>
        [Description("4小时,4 Hours")]
        Duration_240 = 240,
        /// <summary>
        /// 480 minutes
        /// </summary>
        [Description("8小时,8 Hours")]
        Duration_480 = 480,
        /// <summary>
        /// 480 minutes
        /// </summary>
        [Description("1天,1 Day")]
        Duration_1440 = 1440,
        /// <summary>
        /// 480 minutes
        /// </summary>
        [Description("长期,Always")]
        Duration_Never = Int16.MaxValue,
    }

    /// <summary>
    /// Yes No枚举
    /// </summary>
    public enum EnumYesNo
    {
        /// <summary>
        /// No
        /// </summary>
        [Description("否,No")]
        No = 0,
        /// <summary>
        /// Yes
        /// </summary>
        [Description("是,Yes")]
        Yes = 1,
    }

    /// <summary>
    /// 扫描背景、样品的枚举，可以组合
    /// </summary>
    public enum EnumBackgroundSample
    {
        /// <summary>
        /// Background
        /// </summary>
        [Description("背景,Background")]
        Background = 0x01,
        /// <summary>
        /// Sample
        /// </summary>
        [Description("样品,Sample")]
        Sample = 0x02,
    }

    #endregion


    #region class definition

    /// <summary>
    /// 仪器信息类
    /// </summary>
    public class DeviceInfo : INotifyPropertyChanged
    {
        #region notifyevent
        /// <summary>
        /// 属性变更消息
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变更消息
        /// </summary>
        /// <param name="propertyName"></param>
        protected void DoPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// 仪器厂商
        /// </summary>
        public EnumDeviceFactor Factor { get; set; }

        /// <summary>
        /// 仪器种类
        /// </summary>
        public EnumDeviceCategory Type { get; set; }

        /// <summary>
        /// 仪器型号
        /// </summary>
        public EnumDeviceModel Model { get; set; }

        /// <summary>
        /// 仪器商标
        /// </summary>
        public EnumDeviceBrand Brand { get; set; }

        /// <summary>
        /// 仪器序列号
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 设备采集到数据的消息
        /// </summary>
        public Action<int> OnDataReceived { get; set; }

        /// <summary>
        /// 设备是否已经连接
        /// </summary>
        public bool IsConnected { get; set; } = false;

        /// <summary>
        /// 是否模拟采集
        /// </summary>
        public bool IsSimulate { get; set; } = false;

        /// <summary>
        /// 设备附加信息
        /// </summary>
        public object Tag { get; set; } = null;

        /// <summary>
        /// 所有部件及其所有属性的列表
        /// </summary>
        public List<HardwarePropertyInfo> AllHardwarePropertyInfos = null;

        private EnumHardwareStatus _status = EnumHardwareStatus.NotFound;
        /// <summary>
        /// 设备状态
        /// </summary>
        public EnumHardwareStatus Status { get { return _status; } set { value = _status; DoPropertyChange("Status"); } }
    }

    /// <summary>
    /// 信息的基础类
    /// </summary>
    public class BasePropertyInfo : INotifyPropertyChanged
    {
        #region notifyevent
        /// <summary>
        /// 属性变更消息
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// DoPropertyChange
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        public void DoPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region properties
        /// <summary>
        /// 内部名称
        /// </summary>
        [XmlAttribute]
        public string InnerName { get; set; }

        private string _value;
        /// <summary>
        /// 属性的值
        /// </summary>
        [XmlAttribute]
        public string Value { get { return _value; } set { _value = value; DoPropertyChange("Value"); } }

        /// <summary>
        /// 属性值类型
        /// </summary>
        [XmlIgnore]
        public Type ValueType { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        [XmlIgnore]
        public string ChineseName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        [XmlIgnore]
        public string EnglishName { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [XmlIgnore]
        public bool IsValid { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        [XmlIgnore]
        public List<dynamic> Selections { get; set; }

        /// <summary>
        /// 是否可以录入
        /// </summary>
        [XmlIgnore]
        public bool Inputable { get; set; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public BasePropertyInfo()
        {

        }

        /// <summary>
        /// 构造函数(用于定义固定的属性)
        /// </summary>
        /// <param name="innerName">内部名称</param>
        /// <param name="chineseName">中文名称</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="valueType">值得类型</param>
        /// <param name="value">当前值</param>
        /// <param name="isValid">是否有效</param>
        /// <param name="inputable">是否允许用户录入</param>
        /// <param name="selections">选项列表</param>
        public BasePropertyInfo(string innerName, string chineseName, string englishName, Type valueType, string value = null, bool isValid = true, bool inputable = false, List<dynamic> selections = null)
        {
            this.InnerName = innerName;
            this.ChineseName = chineseName;
            this.EnglishName = englishName;
            this.ValueType = valueType;
            this.Value = value;
            this.IsValid = isValid;
            this.Inputable = inputable;
            this.Selections = selections;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public BasePropertyInfo Clone()
        {
            return this.MemberwiseClone() as BasePropertyInfo;
        }

        /// <summary>
        /// 获取属性的显示名称
        /// </summary>
        /// <param name="language">使用的语言</param>
        /// <returns></returns>
        public string PropertyDispalayName(Common.EnumLanguage language)
        {
            return language == Common.EnumLanguage.Chinese ? ChineseName : EnglishName;
        }
    };

    /// <summary>
    /// 硬件属性信息
    /// </summary>
    public class HardwarePropertyInfo:BasePropertyInfo
    {
        /// <summary>
        /// 属性所属的类型
        /// </summary>
        [XmlIgnore]
        public EnumPropCategory Category { get; set; }

        /// <summary>
        /// 部件ID
        /// </summary>
        public EnumHardware HardwareID;

        /// <summary>
        /// 属性ID
        /// </summary>
        public EnumHardwareProperties PropertyID;

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadonly;

        /// <summary>
        /// 是否为列表项
        /// </summary>
        public bool IsSelection;

        /// <summary>
        /// 属性最小值
        /// </summary>
        public float MinValue;

        /// <summary>
        /// 属性最大值
        /// </summary>
        public float MaxValue;

        /// <summary>
        /// 从硬件获取的属性
        /// </summary>
        [XmlIgnore]
        public HardwarePropertyInfo HardwarePropInfo = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HardwarePropertyInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propID">属性ID</param>
        /// <param name="valueType">属性值类型</param>
        /// <param name="isReadonly">是否只读</param>
        /// <param name="isSelection">是否为列表项</param>
        public HardwarePropertyInfo(EnumHardwareProperties propID, Type valueType = null, bool isReadonly = true, bool isSelection = false)
        {
            this.PropertyID = propID;
            this.ValueType = valueType ?? typeof(int);
            this.IsReadonly = isReadonly;
            this.IsSelection = isSelection;
        }

        /// <summary>
        /// 构造函数(用于定义固定的属性)
        /// </summary>
        /// <param name="innerName">内部名称</param>
        /// <param name="chineseName">中文名称</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="valueType">值得类型</param>
        /// <param name="value">当前值</param>
        /// <param name="category">所属种类</param>
        /// <param name="hardwareID">硬件ID</param>
        /// <param name="propID">属性ID</param>
        /// <param name="inputable">是否允许用户录入</param>
        public HardwarePropertyInfo(string innerName, string chineseName, string englishName, Type valueType, string value,
            EnumPropCategory category, EnumHardware hardwareID, EnumHardwareProperties propID, bool inputable = false) :
            base(innerName, chineseName, englishName, valueType, value, true, inputable)
        {
            this.Category = category;
            this.HardwareID = hardwareID;
            this.PropertyID = propID;
        }

        /// <summary>
        /// Clone当前类的值
        /// </summary>
        /// <returns></returns>
        public new HardwarePropertyInfo Clone()
        {
            HardwarePropertyInfo retData = this.MemberwiseClone() as HardwarePropertyInfo;

            return retData;
        }
    }

    /// <summary>
    /// 录入信息字段属性
    /// </summary>
    public class SampleFieldInfo : BasePropertyInfo
    {
        private bool _forFilename;
        /// <summary>
        /// 是否作为文件名
        /// </summary>
        [XmlAttribute]
        public bool ForFilename { get { return _forFilename; } set { _forFilename = value; DoPropertyChange("forFilename"); } }

        /// <summary>
        /// 是否需要预定义属性
        /// </summary>
        [XmlAttribute]
        public bool HasPreDefines { get; set; }

        /// <summary>
        /// 预定义的值
        /// </summary>
        [XmlArray("preDefines")]
        [XmlArrayItem("preDefine")]
        public List<string> PreDefines { get; set; }

        private bool _isSelected = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        [XmlAttribute]
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; DoPropertyChange("isSelected"); } }

        private bool _selectOnly = false;
        /// <summary>
        /// 是否只能选择
        /// </summary>
        [XmlAttribute]
        public bool SelectOnly { get { return _selectOnly; } set { _selectOnly = value; DoPropertyChange("selectOnly"); } }

        /// <summary>
        /// 系统自动创建的信息
        /// </summary>
        [XmlAttribute]
        public bool AutoGenerate { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SampleFieldInfo()
        {

        }

        /// <summary>
        /// 构造函数(用于定义固定的属性)
        /// </summary>
        /// <param name="innerName">内部名称</param>
        /// <param name="chineseName">中文名称</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="valueType">值得类型</param>
        /// <param name="hasPreDefines">是否可以预定义</param>
        /// <param name="isValid">是否选中</param>
        /// <param name="forFilename">是否作为文件名</param>
        /// <param name="autoGenerate">是否系统自动生成</param>
        public SampleFieldInfo(string innerName, string chineseName, string englishName, Type valueType, bool hasPreDefines = true, bool isValid = false, bool forFilename = false, bool autoGenerate = false) :
            base(innerName, chineseName, englishName, valueType, null, isValid, true)
        {
            this.ForFilename = forFilename;
            this.HasPreDefines = hasPreDefines;
            this.AutoGenerate = autoGenerate;
        }

        /// <summary>
        /// Clone this class
        /// </summary>
        /// <returns></returns>
        public new SampleFieldInfo Clone()
        {
            var retData = this.MemberwiseClone() as SampleFieldInfo;
            retData.PreDefines = new List<string>();
            if (this.PreDefines == null)
                this.PreDefines = new List<string>();

            retData.PreDefines.AddRange(this.PreDefines);
            return retData;
        }
    }

    /// <summary>
    /// 仪器扫描参数
    /// </summary>
    [Serializable]
    public class ScanParameter : INotifyPropertyChanged
    {
        #region notify
        /// <summary>
        /// 属性变更消息
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Property changed event
        /// </summary>
        /// <param name="propertyName"></param>
        public void DoPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region properties
        /// <summary>
        /// 参数文件名
        /// </summary>
        [XmlElement]
        public string Filename { get; set; }

        private string _savePath;
        /// <summary>
        /// 光谱保存路径
        /// </summary>
        [XmlElement]
        public string SavePath { get { return _savePath; } set { _savePath = value; DoPropertyChange("savePath"); } }

        private bool _createFolder;
        /// <summary>
        /// 按照样品信息自动创建文件夹
        /// </summary>
        [XmlAttribute]
        public bool CreateFolder { get { return _createFolder; } set { _createFolder = value; DoPropertyChange("createFolder"); } }

        private EnumDeviceCategory _deviceCategory = EnumDeviceCategory.FTNIR;
        /// <summary>
        /// 设备种类
        /// </summary>
        [XmlAttribute]
        public EnumDeviceCategory DeviceCategory { get { return _deviceCategory; } set { _deviceCategory = value; DoPropertyChange("deviceCategory"); } }

        private EnumDeviceModel _deviceModel = EnumDeviceModel.SphereIntegrate;
        /// <summary>
        /// 设备类型
        /// </summary>
        [XmlAttribute]
        public EnumDeviceModel DeviceModel { get { return _deviceModel; } set { _deviceModel = value; DoPropertyChange("deviceModel"); } }

        /// <summary>
        /// 设备属性
        /// </summary>
        [XmlArray("HardwareProps")]
        [XmlArrayItem("HardwareProp")]
        public List<HardwarePropertyInfo> HardwareProps { get; set; }

        /// <summary>
        /// FT转换属性
        /// </summary>
        [XmlArray("FtTransProps")]
        [XmlArrayItem("FtTransProp")]
        public List<BasePropertyInfo> FtTransProps { get; set; }

        /// <summary>
        /// 采集参数
        /// </summary>
        [XmlArray("AcquireProps")]
        [XmlArrayItem("AcquireProp")]
        public List<BasePropertyInfo> AcquireProps { get; set; }

        /// <summary>
        /// 样品信息
        /// </summary>
        [XmlArray("SampleProps")]
        [XmlArrayItem("SampleProp")]
        public List<SampleFieldInfo> SampleProps { get; set; }

        /// <summary>
        /// 背景参考光谱
        /// </summary>
        [XmlIgnore]
        public FileFormat.FileFormat ReferenceFile;

        /// <summary>
        /// 背景光谱
        /// </summary>
        [XmlIgnore]
        public FileFormat.FileFormat BackgroundSpectrum { get; set; }

        /// <summary>
        /// 背景光谱扫描时间
        /// </summary>
        [XmlIgnore]
        public DateTime BackgroundTime { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ScanParameter()
        {
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public ScanParameter Clone()
        {
            return MemberwiseClone() as ScanParameter;
        }

        /// <summary>
        /// 比较两个扫描参数是否相同
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public bool IsSameParameter(ScanParameter para)
        {
            if (Resolution != para.Resolution || 
                ScanCount != para.ScanCount || RepeatCount != para.RepeatCount ||
                XStep > para.XStep + 0.01 || XStep < para.XStep - 0.01 ||
                StartWavelength > para.StartWavelength + 0.1 || StartWavelength < para.StartWavelength - 0.1 ||
                EndWavelength > para.EndWavelength + 0.1 || EndWavelength < para.EndWavelength - 0.1)
                return false;

            return true;
        }

        /// <summary>
        /// 设置特殊属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public virtual bool SetPropertyValue(string propertyName, dynamic propertyValue)
        {
            throw new NotImplementedException();
        }

        #region alias properties

        /// <summary>
        /// 通过属性名称获取属性实例
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        private BasePropertyInfo FindPropertyByName(string propertyName)
        {
            System.Diagnostics.Trace.Assert(propertyName != null, "Invalid parameters");

            var fieldinfo = HardwareProps.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            fieldinfo = FtTransProps.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            fieldinfo = AcquireProps.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            fieldinfo = SampleProps.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            return null;
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public T GetPropertyValue<T>(string propertyName)
        {
            var fieldinfo = FindPropertyByName(propertyName);
            System.Diagnostics.Trace.Assert(fieldinfo != null, "Invalid Property Name");

            if (typeof(T) == typeof(int) || typeof(T).IsEnum)
                return (T)(object)(int.Parse(fieldinfo.Value));
            else if (typeof(T) == typeof(float))
                return (T)(object)(float.Parse(fieldinfo.Value));
            else if (typeof(T) == typeof(string))
                return (T)(object)fieldinfo.Value;
            else if (typeof(T) == typeof(bool))
                return (T)(object)(fieldinfo.Value == "1");

            return default(T);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public void SetPropertyValue<T>(string propertyName, T propertyValue)
        {
            var fieldinfo = FindPropertyByName(propertyName);
            System.Diagnostics.Trace.Assert(fieldinfo != null, "Invalid Property Name");

            if (typeof(T) == typeof(int) || typeof(T) == typeof(float))
                fieldinfo.Value = propertyValue.ToString();
            else if (typeof(T).IsEnum)
                fieldinfo.Value = ((int)(object)propertyValue).ToString();
            else if (typeof(T) == typeof(string))
                fieldinfo.Value = (string)((object)propertyValue);
            else if (typeof(T) == typeof(bool))
                fieldinfo.Value = (bool)(object)propertyValue == true ? "1" : "0";
        }

        /// <summary>
        /// 扫描次数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public int ScanCount { get { return GetPropertyValue<int>(nameof(ScanCount)); } set { SetPropertyValue<int>(nameof(ScanCount), value); } }

        /// <summary>
        /// 重复次数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public int RepeatCount { get { return GetPropertyValue<int>(nameof(RepeatCount)); } set { SetPropertyValue(nameof(RepeatCount), value); } }

        /// <summary>
        /// 是否需要保存SingleBeam
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public bool SaveSingleBeam { get { return GetPropertyValue<bool>(nameof(SaveSingleBeam)); } set { SetPropertyValue(nameof(SaveSingleBeam), value); } }

        /// <summary>
        /// 是否需要保存SingleBeam
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public bool SaveInterfere { get { return GetPropertyValue<bool>(nameof(SaveInterfere)); } set { SetPropertyValue(nameof(SaveInterfere), value); } }

        /// <summary>
        /// 是否需要保存Interfere
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public bool NeedInterfere { get { return GetPropertyValue<bool>(nameof(NeedInterfere)); } set { SetPropertyValue(nameof(NeedInterfere), value); } }

        /// <summary>
        /// 起始波数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public float StartWavelength { get { return GetPropertyValue<float>(nameof(StartWavelength)); } set { SetPropertyValue(nameof(StartWavelength), value); } }

        /// <summary>
        /// 结束波数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public float EndWavelength { get { return GetPropertyValue<float>(nameof(EndWavelength)); } set { SetPropertyValue(nameof(EndWavelength), value); } }

        /// <summary>
        /// 分辨率
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceResolutions Resolution { get { return GetPropertyValue<EnumDeviceResolutions>(nameof(Resolution)); } set { SetPropertyValue(nameof(Resolution), value); } }

        /// <summary>
        /// 结果谱图
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumResultSpectrum ResultSpectrum { get { return GetPropertyValue<EnumResultSpectrum>(nameof(ResultSpectrum)); } set { SetPropertyValue(nameof(ResultSpectrum), value); } }

        /// <summary>
        /// 背景增益
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceGain BackGain { get { return GetPropertyValue<EnumDeviceGain>(nameof(BackGain)); } set { SetPropertyValue(nameof(BackGain), value); } }

        /// <summary>
        /// 样品增益
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceGain SampleGain { get { return GetPropertyValue<EnumDeviceGain>(nameof(SampleGain)); } set { SetPropertyValue(nameof(SampleGain), value); } }

        /// <summary>
        /// 相位校准方法
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumFTPhaseCorrect PhaseCorrect { get { return GetPropertyValue<EnumFTPhaseCorrect>(nameof(PhaseCorrect)); } set { SetPropertyValue(nameof(PhaseCorrect), value); } }

        /// <summary>
        /// 相位分辨率
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumFTPhaseResolution PhaseResolution { get { return GetPropertyValue<EnumFTPhaseResolution>(nameof(PhaseResolution)); } set { SetPropertyValue(nameof(PhaseResolution), value); } }
        
        /// <summary>
        /// 截止函数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumFTApodization Apodization { get { return GetPropertyValue<EnumFTApodization>(nameof(Apodization)); } set { SetPropertyValue(nameof(Apodization), value); } }

        /// <summary>
        /// 填零系数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public int ZeroFilling { get { return GetPropertyValue<int>(nameof(ZeroFilling)); } set { SetPropertyValue<int>(nameof(ZeroFilling), value); } }

        /// <summary>
        /// IVU滤光片
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceIVU IVUFilter { get { return GetPropertyValue<EnumDeviceIVU>(nameof(IVUFilter)); } set { SetPropertyValue<EnumDeviceIVU>(nameof(IVUFilter), value); } }

        /// <summary>
        /// 背景光谱有效期
        /// </summary>
        [XmlIgnore]
        public EnumBackgroundDuration BackgroundDuration { get { return GetPropertyValue<EnumBackgroundDuration>(nameof(BackgroundDuration)); } set { SetPropertyValue(nameof(BackgroundDuration), value); } }

        /// <summary>
        /// X轴插值后的步长, 4cm-1 = 1, 8cm-1 = 2, 16cm-1=4;
        /// </summary>
        [XmlIgnore]
        public double XStep { get { return GetPropertyValue<double>(nameof(XStep)); } set { SetPropertyValue(nameof(XStep), value); } }

        /// <summary>
        /// 是否需要扫描背景
        /// </summary>
        [XmlIgnore]
        public bool NeedScanBackground
        {
            get
            {
                if (BackgroundSpectrum == null)
                    return true;

                int duration = GetPropertyValue<int>("bgDuration");
                var diff = DateTime.Now - BackgroundTime;
                return diff.TotalMinutes >= duration;
            }
        }

        #endregion

    }

    /// <summary>
    /// 扫描通知消息参数
    /// </summary>
    public class ScanNotifyArgs : System.Windows.RoutedEventArgs
    {
        /// <summary>
        /// 扫描状态
        /// </summary>
        public EnumScanNotifyState state { get; set; }
        /// <summary>
        /// 是否取消扫描
        /// </summary>
        public bool abortScan { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errorString { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ScanNotifyArgs() : base()
        {
            this.abortScan = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="state"></param>
        /// <param name="errorString"></param>
        public ScanNotifyArgs(System.Windows.RoutedEvent routedEvent, EnumScanNotifyState state, string errorString) : base(routedEvent)
        {
            this.abortScan = false;
            this.state = state;
            this.errorString = errorString;
        }
    }
    #endregion
}
