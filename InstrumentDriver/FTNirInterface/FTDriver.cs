using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ai.Hong.Driver
{
    /// <summary>
    /// 仪器信息类
    /// </summary>
    public class FTDriver : INotifyPropertyChanged, IDisposable
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// 系统连接的所有设备
        /// </summary>
        public List<DeviceInfo> AllDevices { get; set; }

        private DeviceInfo _connectedDevice = null;
        /// <summary>
        /// 当前已经连接的仪器
        /// </summary>
        public DeviceInfo ConnectedDevice { get { return _connectedDevice; } set { _connectedDevice = value; DoPropertyChange("DeviceConnected"); } }

        /// <summary>
        /// 设备是否连接
        /// </summary>
        public bool DeviceConnected { get { return ConnectedDevice == null ? false : ConnectedDevice.IsConnected; } }

        private string _operateMessage = null;
        /// <summary>
        /// Device operate message
        /// </summary>
        public string OperateMessage { get { return _operateMessage; } set { _operateMessage = value; DoPropertyChange("OperateMessage"); } }

        /// <summary>
        /// 设备采集到数据的消息
        /// </summary>
        public Action<int> OnDataReceived { get; set; }

        /// <summary>
        /// 当前设备类型
        /// </summary>
        public EnumDeviceCategory DeviceCategory { get { return ConnectedDevice != null ? ConnectedDevice.Category : EnumDeviceCategory.Unknown; } }

        /// <summary>
        /// 当前设备型号
        /// </summary>
        public EnumDeviceModel DeviceModel { get { return ConnectedDevice != null ? ConnectedDevice.Model : EnumDeviceModel.Unknown; } }

        private EnumHardwareStatus _deviceStatus = EnumHardwareStatus.NotFound;
        /// <summary>
        /// 设备状态
        /// </summary>
        public EnumHardwareStatus DeviceStatus { get { return _deviceStatus; } set { _deviceStatus = value; DoPropertyChange("DeviceStatus"); } }
        
        /// <summary>
        /// 错误代码
        /// </summary>
        protected EnumHardwareError ErrorCode = 0;

        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrorString { get { return ErrorCode.ToString(); } }

        /// <summary>
        /// 数据存取同步锁定
        /// </summary>
        protected object DataLock = new object();

        /// <summary>
        /// 设备操作同步锁定
        /// </summary>
        protected object DeviceLock = new object();

        private EnumAcquireCommand _acquireCommand = EnumAcquireCommand.Stop;
        /// <summary>
        /// 当前扫描命令状态
        /// </summary>
        public EnumAcquireCommand AcquireCommand { get { return _acquireCommand; } set { _acquireCommand = value; DoPropertyChange(nameof(AcquireCommand)); } }

        /// <summary>
        /// 本实例是否已经销毁了
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 扫描过程中的回调函数（通知调用进程刷新扫描进度）
        /// </summary>
        /// <param name="maxValue">最大扫描次数</param>
        /// <param name="curValue">当前扫描次数</param>
        /// <param name="status">设备状态, 正在扫描=Busy, 空闲=Idel</param>
        /// <returns>True=继续扫描，False=终止扫描</returns>
        public delegate bool ScanProcessingCallback(EnumScanNotifyState status, int maxValue, int curValue);

        /// <summary>
        /// 当前扫描进度
        /// </summary>
        protected int ScanProgress = 0;

        /// <summary>
        /// 扫描进度回调函数
        /// </summary>
        public ScanProcessingCallback ProcessCallback = null;

        /// <summary>
        /// 是否正在扫描的标志
        /// </summary>
        public bool IsScanning = false;

        /// <summary>
        /// 扫描线程
        /// </summary>
        protected System.Threading.Thread ScanThread = null;

        /// <summary>
        /// 实时数据列表
        /// </summary>
        protected List<FileFormat.FileFormat> LiveDatas = null;

        /// <summary>
        /// 扫描数据列表
        /// </summary>
        protected List<FileFormat.FileFormat> ScannedDatas = null;

        /// <summary>
        /// 当前扫描参数
        /// </summary>
        public ScanParameter ScanParameter = null;

        /// <summary>
        /// 设备包含的硬件属性操作
        /// </summary>
        public DeviceHardware deviceHardware;

        #region constructor & deconstructor
        /// <summary>
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

            if (disposing == false)
                Disconnect();

            _disposed = true;
        }

        /// <summary>
        /// Finalizer for the UsbDevice. Disposes all unmanaged handles.
        /// </summary>
        ~FTDriver()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// 枚举系统连接的设备
        /// </summary>
        /// <param name="parameter">枚举参数</param>
        /// <returns></returns>
        public virtual List<DeviceInfo> EnumerateDevices(dynamic parameter) { throw new NotImplementedException(); }

        /// <summary>
        /// 连接仪器
        /// </summary>
        /// <returns></returns>
        public virtual bool Connect(DeviceInfo device) { throw new NotImplementedException(); }

        /// <summary>
        /// 断开仪器
        /// </summary>
        /// <returns></returns>
        public virtual bool Disconnect() { throw new NotImplementedException(); }

        /// <summary>
        /// 列出设备所有硬件
        /// </summary>
        /// <returns></returns>
        public List<EnumHardware> ListHardwares()
        {
            return deviceHardware?.HardwareList();
        }

        /// <summary>
        /// 列出硬件属性
        /// </summary>
        /// <param name="hardwareID">硬件ID编号</param>
        /// <returns></returns>
        public List<EnumHardwareProperties> ListHardwareProperties(EnumHardware hardwareID)
        {
            return deviceHardware?.HardwarePropertyList(hardwareID);
        }

        /// <summary>
        /// 列出硬件属性详细信息
        /// </summary>
        /// <param name="hardwareID">硬件ID编号</param>
        /// <param name="propertyID">硬件属性编号</param>
        /// <returns></returns>
        public HardwarePropertyInfo GetHardwarePropertyInfo(EnumHardware hardwareID, EnumHardwareProperties propertyID)
        {
            return deviceHardware?.GetHardwarePropertyInfo(hardwareID, propertyID);
        }

        /// <summary>
        /// 获取底层设备
        /// </summary>
        /// <returns>底层设备，继承类负责解释</returns>
        public virtual dynamic GetLowLayerDevice() { throw new NotImplementedException(); }

        /// <summary>
        /// 获取当前连接设备的状态
        /// </summary>
        /// <returns></returns>
        public EnumHardwareStatus GetDeviceStatus()
        {
            return (EnumHardwareStatus)GetHardwareProperty(EnumHardware.Device, EnumHardwareProperties.Status);
        }

        /// <summary>
        /// 设置设备硬件属性
        /// </summary>
        /// <param name="hardwareID">硬件类型</param>
        /// <param name="propertyID">硬件属性</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        public bool SetHardwareProperty(EnumHardware hardwareID, EnumHardwareProperties propertyID, dynamic value)
        {
            return deviceHardware?.HardwareSetProperty(GetLowLayerDevice(), hardwareID, propertyID, value);
        }

        /// <summary>
        /// 获取设备硬件属性
        /// </summary>
        /// <param name="hardwareID">硬件类型</param>
        /// <param name="propertyID">硬件属性</param>
        /// <returns></returns>
        public dynamic GetHardwareProperty(EnumHardware hardwareID, EnumHardwareProperties propertyID)
        {
            return deviceHardware?.HardwareGetProperty(GetLowLayerDevice(), hardwareID, propertyID);
        }

        /// <summary>
        /// 发送采集命令（开始或者停止采集）
        /// </summary>
        /// <param name="command">Start or Stop acquire</param>
        /// <param name="callback">Scan processing callback</param>
        /// <returns></returns>
        public virtual bool SendAcquireCommand(EnumAcquireCommand command, ScanProcessingCallback callback) { throw new NotImplementedException(); }

        /// <summary>
        /// 设置采集配置文件
        /// </summary>
        /// <param name="paraFile"></param>
        /// <returns></returns>
        public virtual bool SetExperimentFile(string paraFile) { throw new NotImplementedException(); }

        /// <summary>
        /// 设置采集配置文件
        /// </summary>
        /// <param name="paramter">Scan Parameters</param>
        /// <returns></returns>
        public virtual bool SetExperimentParemter(ScanParameter paramter) { this.ScanParameter = paramter; return true; }

        /// <summary>
        /// 读取采集配置文件
        /// </summary>
        /// <param name="paraFile">Parameter file</param>
        /// <returns></returns>
        public virtual ScanParameter LoadExperimentFile(string paraFile) { throw new NotImplementedException(); }

        /// <summary>
        /// 保存采集配置文件
        /// </summary>
        /// <param name="paraData">Scan Parameters</param>
        /// <param name="saveFile">保存</param>
        /// <returns></returns>
        public virtual bool SaveExperimentFile(ScanParameter paraData, string saveFile) { throw new NotImplementedException(); }

        /// <summary>
        /// 获取设备信号强度
        /// </summary>
        /// <returns></returns>
        public virtual double GetIntensity() { throw new NotImplementedException(); }

        /// <summary>
        /// 刷新设备状态
        /// </summary>
        /// <returns></returns>
        public virtual bool RefreshDeviceStatus() { throw new NotImplementedException(); }

        /// <summary>
        /// 刷新扫描状态
        /// </summary>
        /// <returns></returns>
        public virtual bool RefreshScanStatus() { throw new NotImplementedException(); }

        /// <summary>
        /// 获取实时扫描数据
        /// </summary>
        /// <returns></returns>
        public List<FileFormat.FileFormat> GetLiveDatas()
        {
            lock (DataLock) {
                if (LiveDatas == null)
                    return null;

                var retDatas = new List<FileFormat.FileFormat>();
                retDatas.AddRange(LiveDatas);
                return retDatas;
            }
        }

        /// <summary>
        /// 获取扫描数据
        /// </summary>
        /// <returns></returns>
        public List<FileFormat.FileFormat> GetScanedDatas()
        {
            lock (DataLock)
            {
                if (ScannedDatas == null)
                    return null;

                var retDatas = new List<FileFormat.FileFormat>();
                retDatas.AddRange(ScannedDatas);
                return retDatas;
            }
        }

        /// <summary>
        /// 计算吸收谱
        /// </summary>
        /// <param name="backFile">背景光谱</param>
        /// <param name="sampleFile">样品光谱</param>
        /// <returns></returns>
        public virtual string CalculateAbs(string backFile, string sampleFile)
        {
            //return null;
            if (!System.IO.File.Exists(backFile) || !System.IO.File.Exists(sampleFile))
            {
                return null;
            }

            Ai.Hong.FileFormat.FileFormat back = new Ai.Hong.FileFormat.FileFormat();
            Ai.Hong.FileFormat.FileFormat sample = new Ai.Hong.FileFormat.FileFormat();
            Ai.Hong.FileFormat.FileFormat saveData = new Ai.Hong.FileFormat.FileFormat();
            if (!back.ReadFile(backFile))
            {
                return null;
            }
            if (!sample.ReadFile(sampleFile))
            {
                return null;
            }
            if (back.yDatas.Count() != sample.yDatas.Count())
            {
                return null;
            }
            for (int i = 0; i < back.yDatas.Count(); i++)
            {
                if (back.yDatas[i] == 0)
                    back.yDatas[i] = 1;
                else
                    back.yDatas[i] = Math.Abs(sample.yDatas[i] / back.yDatas[i]);
            }
            //得到吸收光谱
            for (int i = 0; i < back.yDatas.Count(); i++)
            {
                back.yDatas[i] = Math.Log10(1 / back.yDatas[i]);
            }
            double[] trData = new double[back.yDatas.Length];
            for (int index = 0; index < trData.Length; index++)
                trData[index] = (double)back.yDatas[index];
            string fileName = System.IO.Path.GetFileNameWithoutExtension(sampleFile);
            fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sampleFile), fileName + "_abs.spc");
            //存储样品吸光度光谱
            saveData.fileInfo.resolution = sample.fileInfo.resolution;
            saveData.fileInfo.dataCount = sample.fileInfo.dataCount;
            saveData.AddData(sample.dataInfo.firstX, sample.dataInfo.lastX, trData, Ai.Hong.FileFormat.FileFormat.SPECTYPE.SPCNIR, Ai.Hong.FileFormat.FileFormat.XAXISTYPE.XWAVEN, Ai.Hong.FileFormat.FileFormat.YAXISTYPE.YABSRB);
            if (saveData.SaveFile(fileName, Ai.Hong.FileFormat.FileFormat.EnumFileType.SPC) == false)
                return null;
            return fileName;
        }

        /// <summary>
        /// 计算透射谱
        /// </summary>
        /// <param name="backFile">背景光谱</param>
        /// <param name="sampleFile">样品光谱</param>
        /// <returns></returns>
        public virtual string CalculateTrans(string backFile, string sampleFile)
        {
            if (!System.IO.File.Exists(backFile) || !System.IO.File.Exists(sampleFile))
            {
                return null;
            }
            Ai.Hong.FileFormat.FileFormat back = new Ai.Hong.FileFormat.FileFormat();
            Ai.Hong.FileFormat.FileFormat sample = new Ai.Hong.FileFormat.FileFormat();
            Ai.Hong.FileFormat.FileFormat saveData = new Ai.Hong.FileFormat.FileFormat();
            if (!back.ReadFile(backFile))
            {
                return null;
            }
            if (!sample.ReadFile(sampleFile))
            {
                return null;
            }
            if (back.yDatas.Count() != sample.yDatas.Count())
            {
                return null;
            }
            //得到透射图
            for (int i = 0; i < back.yDatas.Count(); i++)
            {
                if (back.yDatas[i] == 0)
                    back.yDatas[i] = 1;
                else
                    back.yDatas[i] = Math.Abs(sample.yDatas[i] / back.yDatas[i]);
            }
            double[] trData = new double[back.yDatas.Length];
            for (int index = 0; index < trData.Length; index++)
                trData[index] = (float)back.yDatas[index];
            string fileName = System.IO.Path.GetFileNameWithoutExtension(sampleFile);
            fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sampleFile), fileName + "_tr.spc");

            //存储样品吸光度光谱
            saveData.fileInfo.resolution = sample.fileInfo.resolution;
            saveData.fileInfo.dataCount = sample.fileInfo.dataCount;
            saveData.AddData(sample.dataInfo.firstX, sample.dataInfo.lastX, trData, Ai.Hong.FileFormat.FileFormat.SPECTYPE.SPCNIR, Ai.Hong.FileFormat.FileFormat.XAXISTYPE.XWAVEN, Ai.Hong.FileFormat.FileFormat.YAXISTYPE.YABSRB);
            if (saveData.SaveFile(fileName, Ai.Hong.FileFormat.FileFormat.EnumFileType.SPC) == false)
                return null;
            return fileName;
        }

        #region alias
        /// <summary>
        /// 写入激光波数到仪器
        /// </summary>
        /// <param name="waveLength">激光波数</param>
        /// <returns></returns>
        public bool SetLaserWavelength(double waveLength)
        {
            return SetHardwareProperty(EnumHardware.Laser, EnumHardwareProperties.Wavelength, waveLength);
        }

        /// <summary>
        /// 读取设备温度
        /// </summary>
        /// <returns></returns>
        public double GetTemperature() { return GetHardwareProperty(EnumHardware.Device, EnumHardwareProperties.Temperature); }

        /// <summary>
        /// 移动验证轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <returns></returns>
        public virtual bool MoveIVU(int position) { throw new NotImplementedException(); }

        /// <summary>
        /// 获取仪器序列号
        /// </summary>
        /// <returns></returns>
        public string GetSerialNumber() { return GetHardwareProperty(EnumHardware.Device, EnumHardwareProperties.SerialNo) as string; }

        /// <summary>
        /// 透射池是否空
        /// </summary>
        /// <returns></returns>
        public virtual bool? IsTransmissionCellEmpty() { return null; }

        #endregion

        /// <summary>
        /// 扫描参考光谱
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="referenceFile">参考光谱保存文件</param>
        /// <returns></returns>
        public string ScanReference(string scanMethodFile, int scanCount, string referenceFile) { return null; }

        /// <summary>
        /// 获取当前激光波数
        /// </summary>
        /// <returns></returns>
        public virtual double? GetLaserWavelength() { return null; }

        /// <summary>
        /// 修改参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scPara"></param>
        /// <param name="iniFilePath"></param>
        /// <returns></returns>
        public virtual bool? ModifyIniFile<T>(T scPara, string iniFilePath) where T : class
        {
            return true;
        }

        /// <summary>
        /// 结果光谱类型到光谱文件Y轴类型的转换
        /// </summary>
        /// <param name="spectrumType"></param>
        /// <returns></returns>
        public static FileFormat.FileFormat.YAXISTYPE YDataTypeFromResultSepctrum(EnumResultSpectrum spectrumType)
        {
            switch (spectrumType)
            {
                case EnumResultSpectrum.Interfer:
                    return FileFormat.FileFormat.YAXISTYPE.YIGRAM;
                case EnumResultSpectrum.BackSingleBeam:
                    return FileFormat.FileFormat.YAXISTYPE.YIGRF;
                case EnumResultSpectrum.SampleSingleBeam:
                    return FileFormat.FileFormat.YAXISTYPE.YIGSM;
                case EnumResultSpectrum.Absorbance:
                    return FileFormat.FileFormat.YAXISTYPE.YABSRB;
                case EnumResultSpectrum.Transmittance:
                    return FileFormat.FileFormat.YAXISTYPE.YTRANS;
                case EnumResultSpectrum.Kubelka_Munk:
                    return FileFormat.FileFormat.YAXISTYPE.YKMONK;
                case EnumResultSpectrum.Reflectance:
                    return FileFormat.FileFormat.YAXISTYPE.YREFLEC;
                case EnumResultSpectrum.Emission:
                    return FileFormat.FileFormat.YAXISTYPE.YEMISN;
                case EnumResultSpectrum.Log_Reflectance:
                    return FileFormat.FileFormat.YAXISTYPE.YLOGDR;
                default:
                    return FileFormat.FileFormat.YAXISTYPE.YARB;
            }
        }

        /// <summary>
        /// 光谱仪器类型到光谱文件类型的转换
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static FileFormat.FileFormat.SPECTYPE SpecTypeFromDeviceCategory(EnumDeviceCategory category)
        {
            switch (category)
            {
                case EnumDeviceCategory.FTNIR:
                    return FileFormat.FileFormat.SPECTYPE.SPCNIR;
                case EnumDeviceCategory.FTIR:
                    return FileFormat.FileFormat.SPECTYPE.SPCFTIR;
                default:
                    return FileFormat.FileFormat.SPECTYPE.SPCGEN;
            }
        }
    }
}
