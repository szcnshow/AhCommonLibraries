using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace Ai.Hong.Driver
{
    /// <summary>
    /// Driver class
    /// Communicate with USB driver, set or get device properties, acquire data
    /// </summary>
    public class Driver:IDisposable
    {
        /// <summary>
        /// 模拟，没有接USB
        /// </summary>
        private const bool isSimulate = true;

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
            OK = 0,
            /// <summary>
            /// NotFound
            /// </summary>
            NotFound = 1,
            /// <summary>
            /// Warning
            /// </summary>
            Warning = 2,
            /// <summary>
            /// Busy
            /// </summary>
            Busy = 3,
            /// <summary>
            /// Fault
            /// </summary>
            Fault = 4,
            /// <summary>
            /// NotReady
            /// </summary>
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
            Automatic = 1,
            /// <summary>
            /// Gain level 1
            /// </summary>
            [Description("一级增益,Gain level 1")]
            Gain_1 = 2,
            /// <summary>
            /// Gain level 2
            /// </summary>
            [Description("二级增益,Gain level 2")]
            Gain_2 = 3,
            /// <summary>
            /// Gain level 3
            /// </summary>
            [Description("三级增益,Gain level 3")]
            Gain_3 = 4,
            /// <summary>
            /// Gain level 4
            /// </summary>
            [Description("四级增益,Gain level 4")]
            Gain_4 = 5,
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

        #endregion

        #region USB enum defines

        /// <summary>
        /// 自定义USB命令
        /// </summary>
        private enum EnumUsbCommand
        {
            /// <summary>
            /// GetProperty
            /// </summary>
            [Description("获取属性")]
            GetProperty=1,
            /// <summary>
            /// SetProperty
            /// </summary>
            [Description("设置属性")]
            SetProperty = 2,
            /// <summary>
            /// GetForamt
            /// </summary>
            [Description("获取属性取值范围")]
            GetForamt = 3,
            /// <summary>
            /// StartScan
            /// </summary>
            [Description("开始扫描")]
            StartScan = 4,
            /// <summary>
            /// AbortScan
            /// </summary>
            [Description("结束扫描")]
            AbortScan = 5,
            /// <summary>
            /// GetError
            /// </summary>
            [Description("获取错误代码")]
            GetError = 6
        }

        /// <summary>
        /// 设备的GUID
        /// </summary>
        public Guid deviceGUID = new Guid("{43998C4C-755C-4302-A736-929341459F62}");

        /// <summary>
        /// 输入输出
        /// </summary>
        private enum EnumInputOutput
        {
            /// <summary>
            /// Input
            /// </summary>
            Input = 0x00,   //设置
            /// <summary>
            /// Output
            /// </summary>
            Output = 0x80,    //获取
        };

        /// <summary>
        /// 控制命令发送的目标
        /// </summary>
        private enum EnumCommandTarget
        {
            /// <summary>
            /// Device
            /// </summary>
            Device = 0x00,       //设备
            /// <summary>
            /// Interface
            /// </summary>
            Interface = 0x01,    //接口
            /// <summary>
            /// Port
            /// </summary>
            Port = 0x02          //端口
        };

        /// <summary>
        /// 控制命令发送的目标
        /// </summary>
        private enum EnumCommandType
        {
            /// <summary>
            /// Standard
            /// </summary>
            Standard = 0x00,       //标准
            /// <summary>
            /// Class
            /// </summary>
            Class = 0x20,          //类型
            /// <summary>
            /// Custom
            /// </summary>
            Custom = 0x40          //用户
        };


        #endregion

        #region Hardware properties

        /// <summary>
        /// 硬件属性信息
        /// </summary>
        public class HardwarePropertyInfo
        {
            /// <summary>
            /// 部件ID
            /// </summary>
            public EnumHardware hardwareID;

            /// <summary>
            /// 属性ID
            /// </summary>
            public EnumHardwareProperties propID;

            /// <summary>
            /// 属性类型
            /// </summary>
            public Type valueType;

            /// <summary>
            /// 是否只读
            /// </summary>
            public bool isReadonly;

            /// <summary>
            /// 是否为列表项
            /// </summary>
            public bool isSelection;

            /// <summary>
            /// 属性最小值
            /// </summary>
            public float minValue;

            /// <summary>
            /// 属性最大值
            /// </summary>
            public float maxValue;

            /// <summary>
            /// 属性选择列表
            /// </summary>
            public List<int> selections;

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
                this.propID = propID;
                this.valueType = valueType ?? typeof(int);
                this.isReadonly = isReadonly;
                this.isSelection = isSelection;
            }

            /// <summary>
            /// Clone当前类的值
            /// </summary>
            /// <returns></returns>
            public HardwarePropertyInfo Clone()
            {
                HardwarePropertyInfo retData = this.MemberwiseClone() as HardwarePropertyInfo;
                if (selections != null)
                {
                    retData.selections = new List<int>();
                    retData.selections.AddRange(selections);
                }

                return retData;
            }
        }


        #endregion

        #region properties

        /// <summary>
        /// 所有部件及其所有属性的列表
        /// </summary>
        private List<HardwarePropertyInfo> allHardwarePropertyInfos = new List<HardwarePropertyInfo>();

        /// <summary>
        /// 错误代码
        /// </summary>
        private EnumHardwareError _errorCode = 0;

        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrorString { get { return _errorCode.ToString(); } }

        /// <summary>
        /// 本实例是否已经销毁了
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 扫描过程中的回调函数（通知调用进程刷新扫描进度）
        /// </summary>
        /// <param name="maxValue">最大扫描次数</param>
        /// <param name="curValue">当前扫描次数</param>
        /// <param name="errorCode">错误代码, 0=OK</param>
        /// <returns>True=继续扫描，False=终止扫描</returns>
        public delegate bool ScanProcessingCallback(EnumHardwareError errorCode, int maxValue, int curValue);

        /// <summary>
        /// Usb Pnp 消息回调函数
        /// </summary>
        /// <param name="plugIn">True=PlugIn, False=PlugOut</param>
        /// <param name="serialNumber">设备序列号</param>
        public delegate void UsbPnpCallback(bool plugIn, string serialNumber);

        #endregion

        #region constructor & deconstructor
        /// <summary>
        /// Disposes the UsbDevice including all unmanaged WinUSB handles. This function
        /// should be called when the UsbDevice object is no longer in use, otherwise
        /// unmanaged handles will remain open until the garbage collector finalizes the
        /// object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        /// <param name="disposing">Indicates whether Dispose was called manually (true) or by
        /// the garbage collector (false) via the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && usbCurrentDevice != null)
                usbDeviceClose();

            _disposed = true;
        }

        /// <summary>
        /// Finalizer for the UsbDevice. Disposes all unmanaged handles.
        /// </summary>
        ~Driver()
        {
            Dispose(false);
        }

        #endregion

        #region USB communication functions

        /// <summary>
        /// 所有接入的设备列表, Key=DevicePath, Value=Serial Number
        /// </summary>
        private Dictionary<string, string> usbConnectedDevices = new Dictionary<string, string>(){
            {"temp", "temp"}
        };

        /// <summary>
        /// 设备句柄
        /// </summary>
        private USBDevice usbCurrentDevice = null;

        /// <summary>
        /// 当前扫描次数
        /// </summary>
        private int currentScanCount = 0;

        /// <summary>
        /// 获取所有属于本GUID的连接设备的序列号,并记录PathName, SerialNumber到connectedDevice中
        /// </summary>
        /// <param name="deviceGuid">设备的GUID</param>
        /// <returns></returns>
        private List<string> usbAllConnectedDevices(Guid deviceGuid)
        {
            usbConnectedDevices.Clear();

            //列出所有本GUID的USB设备
            var devices = MadWizard.WinUSBNet.USBDevice.GetDevices(deviceGuid);
            foreach (var item in devices)
            {
                //获取设备的序列号
                using(var usb = new MadWizard.WinUSBNet.USBDevice(item.DevicePath))
                {
                    if (usb.Descriptor != null && string.IsNullOrEmpty(usb.Descriptor.SerialNumber) == false)
                        usbConnectedDevices.Add(usb.Descriptor.PathName, usb.Descriptor.SerialNumber);
                }
            }

            return usbConnectedDevices.Values.ToList();
        }

        /// <summary>
        /// 连接指定的设备
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private bool usbDeviceConnect(Guid deviceGuid, string serialNumber)
        {
            //找到Serial Number对应的PathName
            string pathname = null;
            if (serialNumber != null) //serialNumber != NULL, Find this device
            {
                if (!usbConnectedDevices.ContainsValue(serialNumber))
                    return false;

                pathname = usbConnectedDevices.FirstOrDefault(p => p.Value == serialNumber).Key;
            }
            else    //Serial number == NULL, find the first device
            {
                if(usbConnectedDevices.Count == 0 )
                {
                    _errorCode = EnumHardwareError.NotFound;
                    return false;
                }
                pathname = usbConnectedDevices.First().Key;
            }

            if(usbCurrentDevice != null)
                usbCurrentDevice.Dispose();

            usbCurrentDevice = new USBDevice(pathname);

            return true;
        }

        /// <summary>
        /// 关闭USB设备
        /// </summary>
        /// <returns></returns>
        private void usbDeviceClose()
        {
            if (usbCurrentDevice != null)
                usbCurrentDevice.Dispose();

            usbCurrentDevice = null;
        }

        /// <summary>
        /// 如果没有连接设备，将引发一个Exception
        /// </summary>
        private void usbCheckConnected()
        {
            if (usbConnectedDevices == null)
            {
                _errorCode = EnumHardwareError.NotConnect;
                throw new Exception(_errorCode.ToString());
            }
        }

        /// <summary>
        /// 获取USB设备错误代码
        /// </summary>
        /// <returns></returns>
        public EnumHardwareError usbErrorCode()
        {
            usbCheckConnected();

            var retBuffer = usbCurrentDevice.ControlIn((int)EnumInputOutput.Input, (int)EnumUsbCommand.GetError, 0, 0, 4);
            if (retBuffer == null || retBuffer.Length == 0)
            {
                return EnumHardwareError.Fault;
            }

            //将返回值转换为ErrorCode
            return (EnumHardwareError)System.BitConverter.ToInt32(retBuffer, 0);
        }


        private bool usbControlOutput<T>(EnumHardware hardwareID, EnumHardwareProperties propertyID, T value)
        {
            usbCheckConnected();

            byte[] outBuffer;

            //通过类型转换来输出
            if (typeof(T) == typeof(int))
                outBuffer = System.BitConverter.GetBytes((int)System.Convert.ChangeType(value, typeof(int)));
            else if (typeof(T) == typeof(float))
                outBuffer = System.BitConverter.GetBytes((float)System.Convert.ChangeType(value, typeof(float)));
            else
                outBuffer = Encoding.UTF8.GetBytes((string)System.Convert.ChangeType(value, typeof(string)));
            
            usbCurrentDevice.ControlOut((int)EnumInputOutput.Output, (int)EnumUsbCommand.GetProperty, (int)hardwareID >> 16 | (int)propertyID, 0, outBuffer);
            _errorCode = usbErrorCode();

            return _errorCode == EnumHardwareError.OK;
        }

        /// <summary>
        /// 获取USB设备中部件的属性
        /// </summary>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <returns>状态的值, Int, float都转为string返回，由调用程序解读, NULL表示错误</returns>
        public T usbControlInput<T>(EnumHardware hardwareID, EnumHardwareProperties propertyID)
        {
            usbCheckConnected();

            int valueLen = 0;
            if (typeof(T) == typeof(int) || typeof(T) == typeof(float))
                valueLen = 4;
            else    //string
                valueLen = 64;

            var retBuffer = usbCurrentDevice.ControlIn((int)EnumInputOutput.Input, (int)EnumUsbCommand.GetProperty, (int)hardwareID >> 16 | (int)propertyID, 0, valueLen);
            if (retBuffer == null || retBuffer.Length == 0)
            {
                _errorCode = usbErrorCode();
                return default(T);
            }

            if (typeof(T) == typeof(int))
            {
                return (T)System.Convert.ChangeType(System.BitConverter.ToInt32(retBuffer, 0), typeof(T));
            }
            else if (typeof(T) == typeof(float))
                return (T)System.Convert.ChangeType(System.BitConverter.ToSingle(retBuffer, 0), typeof(T));
            else
                return (T)System.Convert.ChangeType(Encoding.UTF8.GetString(retBuffer), typeof(T));      //肯定返回的是ASCII
        }

        /// <summary>
        /// 发送设备采集命令
        /// </summary>
        /// <param name="totalScanCount">扫描次数</param>
        /// <returns></returns>
        private bool usbStartAcquire(int totalScanCount)
        {
            usbCheckConnected();

            //设置扫描次数
            var buffer = System.BitConverter.GetBytes(totalScanCount);

            usbCurrentDevice.ControlOut((int)EnumInputOutput.Output, (int)EnumUsbCommand.StartScan, 0, 0, buffer);
            _errorCode = usbErrorCode();

            //将当前扫描次数设置为0
            if(_errorCode == EnumHardwareError.OK)
                currentScanCount = 0;

            return _errorCode == EnumHardwareError.OK;
        }

        /// <summary>
        /// 发送终止采集命令
        /// </summary>
        /// <returns></returns>
        private bool usbAbortAcquire()
        {
            usbCheckConnected();

            usbCurrentDevice.ControlOut((int)EnumInputOutput.Output, (int)EnumUsbCommand.AbortScan, 0, 0);
            _errorCode = usbErrorCode();

            //将当前扫描次数设置为0
            if (_errorCode == EnumHardwareError.OK)
                currentScanCount = 0;

            return _errorCode == EnumHardwareError.OK;
        }

        /// <summary>
        /// 通过读取Interrupt端口, 检查是否有新数据
        /// </summary>
        /// <param name="totalScanCount">总扫描次数</param>
        /// <param name="curScanCount">当前扫描次数</param>
        /// <param name="dataLength">当前数据长度</param>
        /// <returns></returns>
        private bool usbCheckNewData(out int totalScanCount, out int curScanCount, out int dataLength)
        {
            usbCheckConnected();

            totalScanCount = curScanCount = dataLength = 0;

            //设备必须支持中断端口
            var pipe = usbCurrentDevice.Interfaces[0].Pipes.First(p => p.IsInterrupt);
            if(pipe == null)
            {
                throw new Exception(EnumHardwareError.NotFound.ToString());
            }

            byte[] buffer = new byte[12];
            if(pipe.Read(buffer) != buffer.Length)
            {
                _errorCode = EnumHardwareError.Fault;
                return false;
            }

            totalScanCount = BitConverter.ToInt32(buffer, 0);
            curScanCount = BitConverter.ToInt32(buffer, 4);
            dataLength = BitConverter.ToInt32(buffer, 8);

            if (totalScanCount == 0)        //没有总扫描次数, 没有扫描任务
            {
                _errorCode = EnumHardwareError.OK;
                return false;
            }
            else if (curScanCount == 0)     //当前扫描次数为0, 扫描还没有开始
            {
                _errorCode = EnumHardwareError.OK;
                return false;
            }
            else if (curScanCount > currentScanCount && dataLength == 0)     //当前扫描次数不为0，数据长度=0，有错误
            {
                _errorCode = usbErrorCode();
                return false;
            }
            else
            {
                _errorCode = EnumHardwareError.OK;
                return true;
            }
        }

        /// <summary>
        /// 通过Bulk In端口读取采集的数据
        /// </summary>
        /// <param name="dataLength">要读取的数据长度</param>
        /// <returns></returns>
        private float[] usbReadNewData(int dataLength)
        {
            usbCheckConnected();

            //设备必须支持Bulk In 端口
            var pipe = usbCurrentDevice.Interfaces[0].Pipes.First(p => p.IsIn);
            if (pipe == null)
            {
                throw new Exception(EnumHardwareError.NotFound.ToString());
            }

            if (dataLength <= 0)
            {
                _errorCode = EnumHardwareError.Fault;
                return null;
            }

            byte[] buffer = new byte[dataLength];
            if (pipe.Read(buffer) != buffer.Length)
            {
                _errorCode = EnumHardwareError.Fault;
                return null;
            }

            //转换为float类型
            float[] newdata = new float[dataLength / 4];
            for (int i = 0; i < newdata.Length; i++ )
            {
                newdata[i] = System.BitConverter.ToSingle(buffer, i * 4);
            }
            return newdata;
        }

        #endregion

        #region public device functions

        /// <summary>
        /// 当前设备类型
        /// </summary>
        public EnumDeviceCategory deviceCategory { get; private set; }

        /// <summary>
        /// 当前设备型号
        /// </summary>
        public EnumDeviceModel deviceModel { get; private set; }

        /// <summary>
        /// 扫描进度回调函数
        /// </summary>
        private ScanProcessingCallback processCallback = null;

        /// <summary>
        /// 扫描数据读取线程
        /// </summary>
        private System.Threading.Thread scanThread = null;

        /// <summary>
        /// 扫描数据列表
        /// </summary>
        private List<float[]> scannedDatas = new List<float[]>();

        /// <summary>
        /// 查询仪器列表
        /// </summary>
        /// <returns>系统连接仪器的序列号列表</returns>
        public List<string> DeviceQuery()
        {
            return usbConnectedDevices.Values.ToList();
        }

        /// <summary>
        /// 连接指定的仪器
        /// </summary>
        /// <param name="serialNo">仪器的序列号，如果为NLL，表示连接第一个找到的仪器</param>
        /// <returns>是否连接成功</returns>
        public bool DeviceConnect(string serialNo = null)
        {
            return usbDeviceConnect(deviceGUID, serialNo);
        }

        /// <summary>
        /// 断开当前仪器连接
        /// </summary>
        /// <returns></returns>
        public void DeviceDisconnect()
        {
            usbDeviceClose();
        }

        /// <summary>
        /// 获取当前连接仪器的序列号
        /// </summary>
        /// <returns></returns>
        public string DeviceSerialNumber()
        {
            if (usbCurrentDevice == null || usbCurrentDevice.Descriptor == null)
                return null;

            return usbCurrentDevice.Descriptor.SerialNumber;
        }

        /// <summary>
        /// 获取仪器工作状态
        /// </summary>
        /// <returns></returns>
        public EnumHardwareStatus DeviceStatus()
        {
            if(usbCurrentDevice == null)
            {
                _errorCode = EnumHardwareError.NotFound;
                return EnumHardwareStatus.NotFound;
            }

            int status = usbControlInput<int>(EnumHardware.Device, EnumHardwareProperties.Status);
            return (EnumHardwareStatus)status;
        }

        /// <summary>
        /// 设置采集参数，包括分辨率、波数范围、扫描次数等
        /// </summary>
        /// <param name="scanParameter">扫描参数,参数名称和值：{res=”4.0”, count=”32”, r_start=”4000”, r_end =”10000”}</param>
        /// <returns></returns>
        public bool DeviceSetScanParameter(string scanParameter)
        {
            return false;
        }

        private void DeviceCheckNewData()
        {
            int totalCount, curCount, dataLength;

            while(true)
            {
                if(usbCheckNewData(out totalCount, out curCount, out dataLength) == false)
                {
                    if (_errorCode != EnumHardwareError.OK)     //出现了错误
                        return;
                    else
                        System.Threading.Thread.Sleep(100);     //等待100ms
                }

                var newdata = usbReadNewData(dataLength);
                if(newdata == null)
                    return;     //出现错误
                else
                    scannedDatas.Add(newdata);

                if (processCallback != null)    //通知调用方
                {
                    if(processCallback(_errorCode, totalCount, curCount) == false)  //调用方需要停止扫描
                    {
                        DeviceAbortScan();
                        _errorCode = usbErrorCode();

                        return;
                    }
                }

                if (totalCount == curCount)     //扫描结束
                    return;
            }
        }

        /// <summary>
        /// 开始采集光谱
        /// </summary>
        /// <param name="totalScanCount">总扫描次数</param>
        /// <param name="processCallback">扫描进度回调函数</param>
        /// <returns></returns>
        public bool DeviceStartScan(int totalScanCount, ScanProcessingCallback processCallback = null)
        {
            //是否正在扫描
            if (DeviceIsScanning())
            {
                _errorCode = EnumHardwareError.Busy;
                return false;
            }

            scannedDatas.Clear();
            this.processCallback = processCallback;
            if (usbStartAcquire(totalScanCount) == false)
                return false;

            //创建读取扫描数据的线程
            scanThread = new System.Threading.Thread(DeviceCheckNewData);
            scanThread.Start();

            return true;
        }

        /// <summary>
        /// 是否正在扫描
        /// </summary>
        /// <returns></returns>
        public bool DeviceIsScanning()
        {
            return scanThread != null && scanThread.IsAlive;
        }

        /// <summary>
        /// 强制终止采集光谱
        /// </summary>
        /// <returns></returns>
        public bool DeviceAbortScan()
        {
            if (DeviceIsScanning() == false)
                return true;

            if (usbAbortAcquire() == false)
                return false;

            if (scanThread != null)
            {
                scanThread.Abort();
                scanThread = null;
            }

            scannedDatas.Clear();

            return true;
        }

        /// <summary>
        /// 获取扫描结果数据
        /// </summary>
        /// <returns>一次或者多次扫描结果的列表</returns>
        public List<float[]> DeviceGetResult()
        {
            return scannedDatas;
        }

        #endregion

        #region Static Hardware functions

        /// <summary>
        /// 设备类型和型号对应的信息
        /// </summary>
        private class DeviceCategoryProperties
        {
            /// <summary>
            /// 设备类型
            /// </summary>
            public EnumDeviceCategory category;

            /// <summary>
            /// 设备型号
            /// </summary>
            public EnumDeviceModel model;

            /// <summary>
            /// 设备列表及其包含的属性信息
            /// </summary>
            public Dictionary<EnumHardware, List<EnumHardwareProperties>> hardwares;

            /// <summary>
            /// 设备属性列表
            /// </summary>
            public List<HardwarePropertyInfo> properties;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="category">设备类型</param>
            /// <param name="model">设备型号</param>
            /// <param name="hardwares">设备列表及其包含的属性信息</param>
            public DeviceCategoryProperties(EnumDeviceCategory category, EnumDeviceModel model, Dictionary<EnumHardware, List<EnumHardwareProperties>> hardwares)
            {
                this.category = category;
                this.model = model;
                this.hardwares = hardwares;
            }
        }

        /// <summary>
        /// 硬件属性的值类型和是否可读列表(const)
        /// </summary>
        private static List<HardwarePropertyInfo> constPropertyInfoList = new List<HardwarePropertyInfo>()
        {
            new HardwarePropertyInfo(EnumHardwareProperties.Status, typeof(int), true, true),
            new HardwarePropertyInfo(EnumHardwareProperties.SerialNo, typeof(string), true, false),
            new HardwarePropertyInfo(EnumHardwareProperties.Temperature, typeof(float), true, false),
            new HardwarePropertyInfo(EnumHardwareProperties.ProduceDate, typeof(string), true, false),
            new HardwarePropertyInfo(EnumHardwareProperties.DeviceCategory, typeof(int), false, true),  //主要是给用户提供选择
            new HardwarePropertyInfo(EnumHardwareProperties.DeviceModel, typeof(int), false, true),       //主要是给用户提供选择
            new HardwarePropertyInfo(EnumHardwareProperties.Resolution, typeof(int), false, true),
            new HardwarePropertyInfo(EnumHardwareProperties.MaxFrequency, typeof(float), false, false),
            new HardwarePropertyInfo(EnumHardwareProperties.MinFrequency, typeof(float), false, false),

            new HardwarePropertyInfo(EnumHardwareProperties.Source, typeof(int), true, true),
            new HardwarePropertyInfo(EnumHardwareProperties.Voltage, typeof(float), true, false),
            new HardwarePropertyInfo(EnumHardwareProperties.Current, typeof(float), true, false),
            new HardwarePropertyInfo(EnumHardwareProperties.Humidity, typeof(float), true, false),
            new HardwarePropertyInfo(EnumHardwareProperties.Intensity, typeof(float), true, false),
            new HardwarePropertyInfo(EnumHardwareProperties.Wavelength, typeof(float), false, false),
            new HardwarePropertyInfo(EnumHardwareProperties.DetectorType, typeof(int), true, true),
            new HardwarePropertyInfo(EnumHardwareProperties.Position, typeof(int), false, true),
            new HardwarePropertyInfo(EnumHardwareProperties.ScanMode, typeof(int), false, true),
            new HardwarePropertyInfo(EnumHardwareProperties.ScanChannel, typeof(int), false, true),
        };

        /// <summary>
        /// FTNIR硬件所包含的属性列表(const)
        /// </summary>
        private static Dictionary<EnumHardware, List<EnumHardwareProperties>> FTNIRHardwareHasProperties = new Dictionary<EnumHardware, List<EnumHardwareProperties>>()
        {
            {EnumHardware.Device, new List<EnumHardwareProperties>(){EnumHardwareProperties.DeviceCategory, EnumHardwareProperties.DeviceModel, EnumHardwareProperties.Resolution, EnumHardwareProperties.MaxFrequency, EnumHardwareProperties.MinFrequency, EnumHardwareProperties.Humidity, EnumHardwareProperties.ScanMode, EnumHardwareProperties.ScanChannel}},
            {EnumHardware.Source, new List<EnumHardwareProperties>(){EnumHardwareProperties.Source,EnumHardwareProperties.Voltage, EnumHardwareProperties.Current, EnumHardwareProperties.Intensity}},
            {EnumHardware.Board, new List<EnumHardwareProperties>(){EnumHardwareProperties.Voltage, EnumHardwareProperties.Current}},
            {EnumHardware.Power, new List<EnumHardwareProperties>(){EnumHardwareProperties.Voltage, EnumHardwareProperties.Current}},
            {EnumHardware.Drier, new List<EnumHardwareProperties>(){EnumHardwareProperties.Humidity}},
            {EnumHardware.Laser, new List<EnumHardwareProperties>(){EnumHardwareProperties.Intensity, EnumHardwareProperties.Wavelength}},
            {EnumHardware.Detector, new List<EnumHardwareProperties>(){EnumHardwareProperties.DetectorType}},
            {EnumHardware.Background, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.Aperture, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.Interferometer, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.IVU, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.HighFilter, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.LowFilter, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.BackGain, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.SampleGain, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.SampleSwitcher, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.Rotator, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
            {EnumHardware.OpticalSwitcher, new List<EnumHardwareProperties>(){EnumHardwareProperties.Position}},
        };

        private static List<DeviceCategoryProperties> deviceCategoryProperties = new List<DeviceCategoryProperties>()
        {
            new DeviceCategoryProperties(EnumDeviceCategory.FTNIR, EnumDeviceModel.SphereIntegrate, FTNIRHardwareHasProperties),
            new DeviceCategoryProperties(EnumDeviceCategory.FTNIR, EnumDeviceModel.Fiber, FTNIRHardwareHasProperties),
            new DeviceCategoryProperties(EnumDeviceCategory.FTNIR, EnumDeviceModel.Cell, FTNIRHardwareHasProperties),
            new DeviceCategoryProperties(EnumDeviceCategory.FTNIR, EnumDeviceModel.GasCell, FTNIRHardwareHasProperties),
            new DeviceCategoryProperties(EnumDeviceCategory.FTNIR, EnumDeviceModel.Online, FTNIRHardwareHasProperties),
        };


        /// <summary>
        /// 获取仪器包含的所有部件
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <returns>设备部件列表</returns>
        public static List<EnumHardware> HardwareList(EnumDeviceCategory category, EnumDeviceModel model)
        {
            var device = deviceCategoryProperties.FirstOrDefault(p => p.category == category && p.model == model);
            Trace.Assert(device != null, "Device not found");

            return (from p in device.hardwares select p.Key).ToList();
        }

        /// <summary>
        /// 获取部件可用的属性列表
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <param name="hardwareID">硬件ID</param>
        /// <returns></returns>
        public static List<EnumHardwareProperties> HardwarePropertyList(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware hardwareID)
        {
            var device = deviceCategoryProperties.FirstOrDefault(p => p.category == category && p.model == model);
            if (device == null)
                return null;

            if (!device.hardwares.ContainsKey(hardwareID))
                return null;

            return device.hardwares[hardwareID];
        }

        /// <summary>
        /// USB设备连接后调用(临时使用 public)
        /// 获取所有的部件，部件的所有属性，以及属性的取值范围或选项列表
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        private static void HardwareGetAllProperties(EnumDeviceCategory category, EnumDeviceModel model)
        {
            var device = deviceCategoryProperties.FirstOrDefault(p => p.category == category && p.model == model);
            Trace.Assert(device != null, "Device not found");

            var deviceprops = device.properties;
            if (deviceprops != null && deviceprops.Count > 0)
                return;

            device.properties = new List<HardwarePropertyInfo>();
            deviceprops = device.properties;

            //通用属性，每个硬件都有
            var commprop = new List<EnumHardwareProperties>() { EnumHardwareProperties.Status, EnumHardwareProperties.SerialNo, 
                                                            EnumHardwareProperties.Temperature, EnumHardwareProperties.ProduceDate};

            foreach (var hardware in device.hardwares)
            {
                //加入每个设备都有的通用属性,肯定是只读的
                foreach (var prop in commprop)
                {
                    //获取预定义属性的基本信息
                    var info = constPropertyInfoList.FirstOrDefault(p => p.propID == prop);
                    Trace.Assert(info != null, "Property not found");
                        
                    //Clone属性，准备加入部件属性列表中
                    var newinfo = info.Clone();
                    newinfo.hardwareID = hardware.Key;
                    deviceprops.Add(newinfo);
                }

                //查看设备的专用属性是否存在
                foreach (var prop in hardware.Value)
                {
                    //获取预定义属性的基本信息
                    var info = constPropertyInfoList.FirstOrDefault(p => p.propID == prop);
                    Trace.Assert(info != null, "Property not found");

                    //Clone属性，准备加入部件属性列表中
                    var newinfo = info.Clone();
                    newinfo.hardwareID = hardware.Key;

                    //只读属性,本属性没有取值范围或者可选列表
                    if (info.isReadonly == false)   //读写属性，表示本属性有取值范围或者可选列表
                    {
                        //属性是整数,表示有列表选择
                        if (info.valueType == typeof(int))
                        {
                            var sels = innerHardwarePropertySelections(category, model, hardware.Key, prop);
                            Trace.Assert(sels != null, "Get Property Error");
                            newinfo.selections = sels;
                        }
                        //属性是小数,表示有取值范围
                        else if (info.valueType == typeof(float))
                        {
                            float min, max;
                            Trace.Assert(innerHardwareGetPropertyRange(category, model, hardware.Key, prop, out min, out max), "Get Property Error");
                            newinfo.minValue = min;
                            newinfo.maxValue = max;
                        }
                    }
                    deviceprops.Add(newinfo);
                }
            }
        }

        /// <summary>
        /// 获取设备属性的选择项列表
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <param name="deviceID">设备ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <returns>Null=出现错误</returns>
        private static List<int> innerHardwarePropertySelections(EnumDeviceCategory category, EnumDeviceModel model, Driver.EnumHardware deviceID, Driver.EnumHardwareProperties propertyID)
        {
            switch (propertyID)
            {
                case EnumHardwareProperties.DeviceCategory:
                    return new List<int> { (int)EnumDeviceCategory.FTNIR };
                case EnumHardwareProperties.DeviceModel:
                    return new List<int> { (int)EnumDeviceModel.SphereIntegrate, (int)EnumDeviceModel.Fiber, (int)EnumDeviceModel.Cell, (int)EnumDeviceModel.GasCell };
                case EnumHardwareProperties.Resolution:
                    return new List<int> { 4, 8, 16, 32 };
                case EnumHardwareProperties.Source:
                    return new List<int> { (int)EnumDeviceSource.Source1 };
                case EnumHardwareProperties.DetectorType:
                    return new List<int> { (int)EnumDeviceDetector.InGaAs };
                case EnumHardwareProperties.Position:
                    switch (deviceID)
                    {
                        case EnumHardware.Interferometer:
                            return new List<int> { (int)EnumDeviceSwitcher.Off, (int)EnumDeviceSwitcher.On };
                        case EnumHardware.Background:
                            return new List<int> { (int)EnumDeviceSwitcher.Off, (int)EnumDeviceSwitcher.On };
                        case EnumHardware.OpticalSwitcher:
                            switch (model)
                            {
                                case EnumDeviceModel.SphereIntegrate:
                                    return new List<int> { (int)EnumDeviceOpticalSwitcher.Closed, (int)EnumDeviceOpticalSwitcher.SphereIntegrate };
                                case EnumDeviceModel.Fiber:
                                    return new List<int> { (int)EnumDeviceOpticalSwitcher.Closed, (int)EnumDeviceOpticalSwitcher.Fibler };
                                case EnumDeviceModel.Cell:
                                    return new List<int> { (int)EnumDeviceOpticalSwitcher.Closed, (int)EnumDeviceOpticalSwitcher.Cell };
                                case EnumDeviceModel.GasCell:
                                    return new List<int> { (int)EnumDeviceOpticalSwitcher.Closed, (int)EnumDeviceOpticalSwitcher.GasCell };
                                case EnumDeviceModel.Online:
                                    return new List<int> { (int)EnumDeviceOpticalSwitcher.Closed, (int)EnumDeviceOpticalSwitcher.Online };
                                default:
                                    break;
                            }
                            break;
                        case EnumHardware.SampleSwitcher:
                            return new List<int> { (int)EnumDeviceSwitcher.Off, (int)EnumDeviceSwitcher.On };
                        case EnumHardware.BackGain:
                            return new List<int> { (int)EnumDeviceGain.Disabled, (int)EnumDeviceGain.Automatic, (int)EnumDeviceGain.Gain_1, (int)EnumDeviceGain.Gain_2, (int)EnumDeviceGain.Gain_3, (int)EnumDeviceGain.Gain_4 };
                        case EnumHardware.SampleGain:
                            return new List<int> { (int)EnumDeviceGain.Disabled, (int)EnumDeviceGain.Automatic, (int)EnumDeviceGain.Gain_1, (int)EnumDeviceGain.Gain_2, (int)EnumDeviceGain.Gain_3, (int)EnumDeviceGain.Gain_4 };
                        case EnumHardware.Aperture:
                            return new List<int> { (int)EnumDeviceAperture.Open, (int)EnumDeviceAperture.Three_Quater, (int)EnumDeviceAperture.Half, (int)EnumDeviceAperture.Quarter, (int)EnumDeviceAperture.Off };
                        case EnumHardware.IVU:
                            return new List<int> { (int)EnumDeviceIVU.Empty, (int)EnumDeviceIVU.Glass, (int)EnumDeviceIVU.Polystyrene };
                        case EnumHardware.HighFilter:
                            return new List<int> { (int)EnumDeviceHighFiler.Disabled, (int)EnumDeviceHighFiler.Enabled };
                        case EnumHardware.LowFilter:
                            return new List<int> { (int)EnumDeviceLowFiler.Disabled, (int)EnumDeviceLowFiler.Automatic, (int)EnumDeviceLowFiler.Khz_10 };
                        case EnumHardware.Rotator:
                            return new List<int> { (int)EnumDeviceSwitcher.Off, (int)EnumDeviceSwitcher.On };
                        default:
                            return null; ;
                    }
                    break;
                case EnumHardwareProperties.ScanMode:
                    return new List<int> { (int)EnumDeviceScanMode.DoubleSidedFB, (int)EnumDeviceScanMode.SingleSidedFB, (int)EnumDeviceScanMode.DoubleSided, (int)EnumDeviceScanMode.SingleSided };
                case EnumHardwareProperties.ScanChannel:
                    switch (model)
                    {
                        case EnumDeviceModel.SphereIntegrate:
                            return new List<int> { (int)EnumDeviceChannels.SphereBackground, (int)EnumDeviceChannels.SphereSample, };
                        case EnumDeviceModel.Fiber:
                            return new List<int> { (int)EnumDeviceChannels.FiberBackground, (int)EnumDeviceChannels.FiberSample, };
                        case EnumDeviceModel.Cell:
                            return new List<int> { (int)EnumDeviceChannels.CellBackground, (int)EnumDeviceChannels.CellSample, };
                        case EnumDeviceModel.GasCell:
                            return new List<int> { (int)EnumDeviceChannels.CellBackground, (int)EnumDeviceChannels.CellSample, };
                        case EnumDeviceModel.Online:
                            return new List<int> { (int)EnumDeviceChannels.OutBackground, (int)EnumDeviceChannels.OutSample };
                        default:
                            break;
                    }
                    break;
                default:
                    return null;
            }
            return null;
        }

        /// <summary>
        /// 获取浮点属性的取值范围
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        private static bool innerHardwareGetPropertyRange(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware hardwareID, EnumHardwareProperties propertyID, out float min, out float max)
        {
            min = 0;
            max = 0;
            switch (category)
            {
                case EnumDeviceCategory.FTNIR:
                    if (propertyID == EnumHardwareProperties.MaxFrequency || propertyID == EnumHardwareProperties.MinFrequency)
                    {
                        min = 0;
                        max = 15000f;
                    }
                    else if (propertyID == EnumHardwareProperties.Wavelength)
                    {
                        min = 4000.0f;
                        max = 10000.0f;
                    }
                    else
                    {
                        min = max = 0f;
                    }
                    return true;
                case EnumDeviceCategory.FTIR:
                    return false;
            }

            return false;
        }

        /// <summary>
        /// 检查部件是否包含了指定属性，并获取本属性的类型，只读信息
        /// 使用deviceHasProperties检查PropertyID是否在DeviceID中
        /// 用propertyInfoList来获取PropertyID的类型，只读信息
        /// </summary>
        /// <param name="category">设备类型FTNIR, FTIR...</param>
        /// <param name="model">设备型号Sephere, Fiber...</param>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <param name="forRead">True=获取属性, False=设置属性</param>
        /// <returns></returns>
        public static HardwarePropertyInfo HardwarePropertyDetail(EnumDeviceCategory category, EnumDeviceModel model, EnumHardware hardwareID, EnumHardwareProperties propertyID, bool forRead = true)
        {
            //确保仪器类型正确
            var device = deviceCategoryProperties.FirstOrDefault(p => p.category == category && p.model == model);
            Trace.Assert(device != null, "Device not found");

            //刷新属性
            HardwareGetAllProperties(category, model);

            switch (category)
            {
                case EnumDeviceCategory.FTNIR:
                case EnumDeviceCategory.FTIR:
                    var info = device.properties.FirstOrDefault(p => p.hardwareID == hardwareID && p.propID == propertyID);
                    if (info.isReadonly == true && forRead == false)
                        return null;

                    return info;
            }
            return null;
        }
        
        #endregion

        #region hardware functions

        /// <summary>
        /// 获取部件的属性
        /// </summary>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <returns>状态的值, Int, float都转为string返回，由调用程序解读, NULL表示错误</returns>
        public string HardwareGetProperty(EnumHardware hardwareID, EnumHardwareProperties propertyID)
        {
            var propInfo = Driver.HardwarePropertyDetail(deviceCategory, deviceModel, hardwareID, propertyID);
            if (propInfo == null)
                return null;

            if (propInfo.valueType == typeof(int))
                return usbControlInput<int>(hardwareID, propertyID).ToString();
            else if (propInfo.valueType == typeof(float))
                return usbControlInput<float>(hardwareID, propertyID).ToString();
            else if (propInfo.valueType == typeof(string))
                return usbControlInput<string>(hardwareID, propertyID).ToString();
            else
                return null;
        }

        /// <summary>
        /// 设置部件的属性
        /// </summary>
        /// <param name="hardwareID">部件ID</param>
        /// <param name="propertyID">属性ID</param>
        /// <param name="propertyValue">属性的值</param>
        /// <returns>错误信息</returns>
        public EnumHardwareError HardwareSetProperty(EnumHardware hardwareID, EnumHardwareProperties propertyID, string propertyValue)
        {
            var propInfo = Driver.HardwarePropertyDetail(deviceCategory, deviceModel, hardwareID, propertyID);
            if (propInfo == null)
                return _errorCode;

            _errorCode = EnumHardwareError.OK;
            if (propInfo.valueType == typeof(int))
                usbControlOutput<int>(hardwareID, propertyID, int.Parse(propertyValue));
            else if (propInfo.valueType == typeof(float))
                usbControlOutput<float>(hardwareID, propertyID, float.Parse(propertyValue));
            else if (propInfo.valueType == typeof(string))
                usbControlOutput<string>(hardwareID, propertyID, propertyValue);
            else
                _errorCode = EnumHardwareError.Format;

            return _errorCode;
        }

        #endregion
    }
}
