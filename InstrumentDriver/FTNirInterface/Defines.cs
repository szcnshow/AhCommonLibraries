using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Firmware
        /// </summary>
        [Description("固件,Firmware")]
        Firmware = 27,
        /// <summary>
        /// Firmware
        /// </summary>
        [Description("累加采样,Coaddtion")]
        Coaddtion = 28,
        /// <summary>
        /// Firmware
        /// </summary>
        [Description("光学计量,Metrology")]
        Metrology = 29,
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
        /// <summary>
        /// Device name
        /// </summary>
        [Description("设备名称")]
        Name = 25,
        /// <summary>
        /// Device decription
        /// </summary>
        [Description("设备描述")]
        Decription = 26,
        /// <summary>
        /// Version
        /// </summary>
        [Description("版本号")]
        Version = 27,
        /// <summary>
        /// Address
        /// </summary>
        [Description("仪器地址")]
        Address = 28,
        /// <summary>
        /// Port（Communication port)
        /// </summary>
        [Description("端口")]
        Port = 29,
        /// <summary>
        /// Pressure
        /// </summary>
        [Description("压力")]
        Pressure = 30,
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
        /// Start live acquire
        /// </summary>
        [Description("实时开始,Live Start")]
        LiveStart = 0,
        /// <summary>
        /// Live Start
        /// </summary>
        [Description("背景开始,Background Start")]
        BackStart = 1,
        /// <summary>
        /// Live Start
        /// </summary>
        [Description("样品开始,Background Start")]
        SampleStart = 2,
        /// <summary>
        /// Stop acquire, Wait for device 
        /// </summary>
        [Description("停止扫描,Stop Acquisition")]
        Stop = 3,
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
        OneFinished = 2,
        /// <summary>
        /// 完成全部重复（扫描结束）
        /// </summary>
        RepeateFinished = 3,
        /// <summary>
        /// 参数错误
        /// </summary>
        ParameterError = 4,
        /// <summary>
        /// 设备错误
        /// </summary>
        DeviceError = 5,
        /// <summary>
        /// 文件错误
        /// </summary>
        FileError = 6,
        /// <summary>
        /// 用户取消
        /// </summary>
        UserAbort = 7,
        /// <summary>
        /// 需要扫描背景
        /// </summary>
        BackgroundError = 8,
    }

    #endregion

    #region properties value defines

    /// <summary>
    /// 结果谱图类型
    /// </summary>
    public enum EnumResultSpectrum
    {
        /// <summary>
        /// Interfergoram
        /// </summary>
        [Description("干涉谱,Interfergoram")]
        Interfer = 0,
        /// <summary>
        /// Interfergoram
        /// </summary>
        [Description("背景单通道谱,Background SingleBeam")]
        BackSingleBeam = 1,
        /// <summary>
        /// Interfergoram
        /// </summary>
        [Description("样品单通道谱,Sample SingleBeam")]
        SampleSingleBeam = 2,
        /// <summary>
        /// Absorbance
        /// </summary>
        [Description("吸收谱,Absorbance")]
        Absorbance = 3,
        /// <summary>
        /// Transmittance
        /// </summary>
        [Description("透过谱,Transmittance")]
        Transmittance = 4,
        /// <summary>
        /// Kubelka_Munk
        /// </summary>
        [Description("Kubelka Munk谱,Kubelka Munk")]
        Kubelka_Munk = 5,
        /// <summary>
        /// Reflectance
        /// </summary>
        [Description("反射谱,Reflectance")]
        Reflectance = 6,
        /// <summary>
        /// Emission
        /// </summary>
        [Description("发射谱,Emission")]
        Emission = 7,
        /// <summary>
        /// Log Reflectance
        /// </summary>
        [Description("Log 反射谱,Log Reflectance")]
        Log_Reflectance = 8,
        /// <summary>
        /// Raw data
        /// </summary>
        [Description("原始数据,Raw data")]
        Raw = 9,
    }

    /// <summary>
    /// 保存的光谱文件格式
    /// </summary>
    public enum EnumSaveFileType
    {
        /// <summary>
        /// SPC
        /// </summary>
        SPC = 0,
        /// <summary>
        /// CSV
        /// </summary>
        CSV = 1,
        /// <summary>
        /// JDX
        /// </summary>
        JDX = 2,
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
        /// Power Spectrum
        /// </summary>
        [Description("Power Spectrum")]
        Power_Spectrum = 2,
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
        /// Happ Genzel
        /// </summary>
        [Description("Happ Genzel")]
        Happ_Genzel = 3,
        /// <summary>
        /// Blackman Harris 3 Term
        /// </summary>
        [Description("Blackman Harris 3 Term")]
        Blackman_Harris_3_Term = 4,
        /// <summary>
        /// Blackman Harris 4 Term
        /// </summary>
        [Description("Blackman Harris 4 Term")]
        Blackman_Harris_4_Term = 5,
        /// <summary>
        /// Blackman Harris 4 Term
        /// </summary>
        [Description("Blackman Harris 7 Term")]
        Blackman_Harris_7_Term = 6,
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
    }

    /// <summary>
    /// FT填零
    /// </summary>
    public enum EnumFTZeroFilling
    {
        ///// <summary>
        ///// Filling 0
        ///// </summary>
        //[Description("Factor 0")]
        //Filling_0 = 0,
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
    /// 扫描通知消息参数
    /// </summary>
    public class ScanNotifyArgs : System.Windows.RoutedEventArgs
    {
        /// <summary>
        /// 扫描状态
        /// </summary>
        public EnumScanNotifyState State { get; set; }
        /// <summary>
        /// 是否取消扫描
        /// </summary>
        public bool AbortScan { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorString { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ScanNotifyArgs() : base()
        {
            this.AbortScan = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="state"></param>
        /// <param name="errorString"></param>
        public ScanNotifyArgs(System.Windows.RoutedEvent routedEvent, EnumScanNotifyState state, string errorString) : base(routedEvent)
        {
            this.AbortScan = false;
            this.State = state;
            this.ErrorString = errorString;
        }
    }
    #endregion
}
