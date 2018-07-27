using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Ai.Hong.Common.Extenstion;

namespace Ai.Hong.Driver
{
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

        #region const properties
        /// <summary>
        /// 所有预定义的硬件属性
        /// </summary>
        private static List<HardwarePropertyInfo> constHardwareProperties = new List<HardwarePropertyInfo>()
        {
            //光学部件
            new HardwarePropertyInfo(EnumHardware.Device, EnumHardwareProperties.ScanChannel, EnumPropCategory.optical, "BackChannel","背景通道", "Back Channel", typeof(EnumDeviceChannels), ((int)EnumDeviceChannels.SphereBackground).ToString(), false),
            new HardwarePropertyInfo(EnumHardware.Device, EnumHardwareProperties.ScanChannel, EnumPropCategory.optical, "SampleChannel", "样品通道", "Sample Channel", typeof(EnumDeviceChannels), ((int)EnumDeviceChannels.SphereSample).ToString(), false),
            new HardwarePropertyInfo(EnumHardware.Source, EnumHardwareProperties.Source, EnumPropCategory.optical, "Source", "光源", "Source", typeof(EnumDeviceSource),((int)EnumDeviceSource.Source1).ToString(), false),
            new HardwarePropertyInfo(EnumHardware.Laser, EnumHardwareProperties.Wavelength, EnumPropCategory.optical, nameof(LaserWavelength), "激光波数", nameof(LaserWavelength), typeof(float),"15798.65", false),

            //电子部件

            //采样部件
            new HardwarePropertyInfo(EnumHardware.Device, EnumHardwareProperties.Resolution, EnumPropCategory.acquire, nameof(Resolution), "分辨率", "Resolution", typeof(EnumDeviceResolutions), ((int)EnumDeviceResolutions.res_8).ToString(), false),
        };

        /// <summary>
        /// 预定义的所有FT转换参数
        /// </summary>
        private static List<BasePropertyInfo> constFTProperties = new List<BasePropertyInfo>()
        {
            new BasePropertyInfo(nameof(PhaseResolution), "相位分辨率", "Phase Resolution", typeof(EnumFTPhaseResolution), ((int)EnumFTPhaseResolution.Res_32).ToString(), 
                false, EnumExtensions.TypeDictionaryToDynamic(EnumExtensions.EnumTypeToDescriptionList<EnumFTPhaseResolution>(Language))),
            new BasePropertyInfo(nameof(PhaseCorrect), "相位校正方法", "Phase Correct", typeof(EnumFTPhaseCorrect), ((int)EnumFTPhaseCorrect.Power_Spectrum).ToString(),
                false, EnumExtensions.TypeDictionaryToDynamic(EnumExtensions.EnumTypeToDescriptionList<EnumFTPhaseCorrect>(Language))),
            new BasePropertyInfo(nameof(Apodization), "截趾函数", "Apodization", typeof(EnumFTApodization), ((int)EnumFTApodization.BoxCar).ToString(),
                false, EnumExtensions.TypeDictionaryToDynamic(EnumExtensions.EnumTypeToDescriptionList<EnumFTApodization>(Language))),
            new BasePropertyInfo(nameof(ZeroFilling), "填零因子", "Zero Filling", typeof(EnumFTZeroFilling), ((int)EnumFTZeroFilling.Filling_1).ToString(), 
                false, EnumExtensions.TypeDictionaryToDynamic(EnumExtensions.EnumTypeToDescriptionList<EnumFTZeroFilling>(Language))),
        };

        /// <summary>
        /// 预定义的所有采样参数
        /// </summary>
        private static List<BasePropertyInfo> constAcquireProperties = new List<BasePropertyInfo>()
        {
            //new BasePropertyInfo("resolution", "分辨率", "Resolution", typeof(int), "8"),
            new BasePropertyInfo(nameof(StartWavelength), "起始波数", "Start Wavelength", typeof(float), "4000"),
            new BasePropertyInfo(nameof(EndWavelength), "结束波数", "End Wavelength", typeof(float), "10000"),
            new BasePropertyInfo(nameof(ScanCount), "扫描次数", "Count", typeof(EnumScanCount),"32", true, 
                new Dictionary<dynamic, string>(){{1, "1" }, {2, "2" }, {4, "4" }, {8, "8" }, {16, "16" }, {32, "32" }, {64, "64" } }),
            new BasePropertyInfo(nameof(RepeatCount), "重复次数", "Repeat", typeof(EnumRepeatCount), "0", true, 
                new Dictionary<dynamic, string>(){ { 0, "0" }, { 1, "1" }, {2, "2" }, {3, "3" }, {4, "4" } }),
            //new BasePropertyInfo("minX", "最小X值", "Min Wavelength", typeof(int), "4000.0", true, true),
            //new BasePropertyInfo("maxX", "最大X值", "Max Wagelength", typeof(int), "10000.0", true, true),
            //new BasePropertyInfo("xStandardize", "X轴标准化", "X Standardize", typeof(int), "0"), 以后再用
            //new BasePropertyInfo("xStep", "X轴步长", "X Step", typeof(int), "2"),
            new BasePropertyInfo(nameof(ResultSpectrum), "结果谱图", "Result Spectrum", typeof(EnumResultSpectrum), ((int)EnumResultSpectrum.Absorbance).ToString(),
                false, EnumExtensions.TypeDictionaryToDynamic(EnumExtensions.EnumTypeToDescriptionList<EnumResultSpectrum>(Language))),
            new BasePropertyInfo(nameof(SaveSingleBeam), "保存单通道图", "Save SingleBeam", typeof(EnumYesNo), ((int)EnumYesNo.No).ToString(),
                false, new Dictionary<dynamic, string>(){{false, "否" }, {true, "是" } }),
            new BasePropertyInfo(nameof(SaveInterfere), "保存干涉图", "Save Interfere", typeof(EnumYesNo), ((int)EnumYesNo.No).ToString(), //, EnumToDescriptionList<EnumYesNo>(language)),
                false, new Dictionary<dynamic, string>(){{false, "否" }, {true, "是" } }),
            new BasePropertyInfo(nameof(BackgroundDuration), "背景有限期", "Background Duration", typeof(EnumBackgroundDuration), ((int)EnumBackgroundDuration.Duration_60).ToString(),
                false, EnumExtensions.TypeDictionaryToDynamic(EnumExtensions.EnumTypeToDescriptionList<EnumBackgroundDuration>(Language))),
            new BasePropertyInfo(nameof(SaveFileType), "文件格式", "File Format", typeof(EnumSaveFileType), ((int)EnumSaveFileType.SPC).ToString(),
                false, EnumExtensions.TypeDictionaryToDynamic(EnumExtensions.EnumTypeToDescriptionList<EnumSaveFileType>(Language))),
        };

        /// <summary>
        /// 预定义的所有样品信息
        /// </summary>
        private static List<SampleFieldInfo> constSampleProps = new List<SampleFieldInfo>()
            {
                new SampleFieldInfo("index", "检品号", "index", typeof(string), true, true),
                new SampleFieldInfo("kind", "品种", "kind", typeof(string), true, true),
                new SampleFieldInfo("name", "名称", "name", typeof(string), true, true),
                new SampleFieldInfo("origin", "来源", "origin", typeof(string)),
                new SampleFieldInfo("specific", "规格", "specific", typeof(string)),
                new SampleFieldInfo("grade", "等级", "grade", typeof(string)),
                new SampleFieldInfo("form", "形态", "form", typeof(string)),
                new SampleFieldInfo("batch", "批次", "batch", typeof(string)),
                new SampleFieldInfo("package", "包装", "package", typeof(string)),
                new SampleFieldInfo("operator", "检测人员", "operator", typeof(string),false,false,false,true),
                new SampleFieldInfo("unit", "检测单位", "unit", typeof(string),false,false,false,true),
                new SampleFieldInfo("address", "测样地点", "address", typeof(string)),
                new SampleFieldInfo("date", "测样日期", "date", typeof(DateTime),false,false,false,true),
                new SampleFieldInfo("time", "测样时间", "time", typeof(DateTime),false,false,false,true),
                new SampleFieldInfo("Custom_1", "自定义1", "Customer 1", typeof(string)),
                new SampleFieldInfo("Custom_2", "自定义2", "Customer 2", typeof(string)),
                new SampleFieldInfo("Custom_3", "自定义3", "Customer 3", typeof(string)),
            };

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
        /// 附加信息，由各设备自己解释
        /// </summary>
        [XmlElement]
        public Dictionary<string,  string> AddtionalData { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        [XmlAttribute]
        public static Common.EnumLanguage Language { get; set; }

        /// <summary>
        /// 设备属性
        /// </summary>
        [XmlArray("HardwareProps")]
        [XmlArrayItem("HardwareProp")]
        public List<BasePropertyInfo> HardwareProps { get; set; }

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
        /// 按照设备型号创建扫描参数
        /// </summary>
        /// <param name="hardware"></param>
        /// <param name="language"></param>
        public ScanParameter(DeviceHardware hardware, Common.EnumLanguage language)
        {
            //HardwareProps = constHardwareProperties;
            FtTransProps = constFTProperties;
            AcquireProps = constAcquireProperties;
            SampleProps = constSampleProps;
            Language = language;

            hardware.InitScanParameter(this);
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
        /// 通过属性名称获取属性实例
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        private BasePropertyInfo FindPropertyByName(string propertyName)
        {
            System.Diagnostics.Trace.Assert(propertyName != null, "Invalid parameters");

            var fieldinfo = HardwareProps?.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            fieldinfo = FtTransProps?.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            fieldinfo = AcquireProps?.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            fieldinfo = SampleProps?.FirstOrDefault(p => p.InnerName == propertyName) as BasePropertyInfo;
            if (fieldinfo != null)
                return fieldinfo;

            return null;
        }

        #region 操作属性

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public T GetPropertyValue<T>(string propertyName)
        {
            var fieldinfo = FindPropertyByName(propertyName);
            if (fieldinfo == null)
                return default(T);

            if (typeof(T) == typeof(int) || typeof(T).IsEnum)
                return (T)(object)(int.Parse(fieldinfo.Value));
            else if (typeof(T) == typeof(float))
                return (T)(object)(float.Parse(fieldinfo.Value));
            else if (typeof(T) == typeof(double))
                return (T)(object)(double.Parse(fieldinfo.Value));
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
            if (fieldinfo == null)
                return;

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
        /// 设置属性列表选项
        /// </summary>
        /// <param name="innerName">属性内部名称</param>
        /// <param name="selections">选项</param>
        public void SetPropertySelections(string innerName, Dictionary<dynamic, string> selections)
        {
            var propInfo = FindPropertyByName(innerName);
            if(propInfo != null)
                propInfo.Selections = selections;
        }

        /// <summary>
        /// 获取属性列表选项
        /// </summary>
        /// <param name="innerName">属性内部名称</param>
        public Dictionary<dynamic, string> GetPropertySelections(string innerName)
        {
            var propInfo = FindPropertyByName(innerName);
            return propInfo != null ? propInfo.Selections : null;
        }

        /// <summary>
        /// 设置属性最大最小值
        /// </summary>
        /// <param name="innerName"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public void SetPropertyMaxMin(string innerName, float max, float min)
        {
            var propInfo = FindPropertyByName(innerName);
            if (propInfo != null)
            {
                propInfo.MaxValue = max;
                propInfo.MinValue = min;
            }
        }

        /// <summary>
        /// 获取属性最大最小值
        /// </summary>
        /// <param name="innerName"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns>true=找到属性</returns>
        public bool GetPropertyMaxMin(string innerName, out float max, out float min)
        {
            max = float.MaxValue;
            min = float.MinValue;
            var propInfo = FindPropertyByName(innerName);
            if (propInfo != null)
            {
                max = propInfo.MaxValue;
                min = propInfo.MinValue;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 设置附加属性
        /// </summary>
        /// <param name="key">属性名称</param>
        /// <param name="value">属性值</param>
        public void SetAddtionalData(string key, string value)
        {
            if (AddtionalData == null)
                AddtionalData = new Dictionary<string, string>();

            if (AddtionalData.ContainsKey(key))
                AddtionalData[key] = value;
            else
                AddtionalData.Add(key, value);
        }

        /// <summary>
        /// 获取附加属性值
        /// </summary>
        /// <param name="key">属性名称</param>
        /// <returns></returns>
        public string GetAddtionalData(string key)
        {
            return AddtionalData == null ? null : AddtionalData[key];
        }

        #endregion

        #region alias properties

        /// <summary>
        /// 扫描次数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public int ScanCount { get { return GetPropertyValue<int>(nameof(ScanCount)); } set { SetPropertyValue<int>(nameof(ScanCount), value); DoPropertyChange(nameof(ScanCount)); } }

        /// <summary>
        /// 重复次数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public int RepeatCount { get { return GetPropertyValue<int>(nameof(RepeatCount)); } set { SetPropertyValue(nameof(RepeatCount), value); DoPropertyChange(nameof(RepeatCount)); } }

        /// <summary>
        /// 是否需要保存SingleBeam
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public bool SaveSingleBeam { get { return GetPropertyValue<bool>(nameof(SaveSingleBeam)); } set { SetPropertyValue(nameof(SaveSingleBeam), value); DoPropertyChange(nameof(SaveSingleBeam)); } }

        /// <summary>
        /// 是否需要保存SingleBeam
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public bool SaveInterfere { get { return GetPropertyValue<bool>(nameof(SaveInterfere)); } set { SetPropertyValue(nameof(SaveInterfere), value); DoPropertyChange(nameof(SaveInterfere)); } }

        /// <summary>
        /// 是否需要保存Interfere
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public bool NeedInterfere { get { return GetPropertyValue<bool>(nameof(NeedInterfere)); } set { SetPropertyValue(nameof(NeedInterfere), value); DoPropertyChange(nameof(NeedInterfere)); } }

        /// <summary>
        /// 起始波数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public float StartWavelength { get { return GetPropertyValue<float>(nameof(StartWavelength)); } set { SetPropertyValue(nameof(StartWavelength), value); DoPropertyChange(nameof(StartWavelength)); } }

        /// <summary>
        /// 结束波数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public float EndWavelength { get { return GetPropertyValue<float>(nameof(EndWavelength)); } set { SetPropertyValue(nameof(EndWavelength), value); DoPropertyChange(nameof(EndWavelength)); } }

        /// <summary>
        /// 分辨率
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceResolutions Resolution { get { return GetPropertyValue<EnumDeviceResolutions>(nameof(Resolution)); } set { SetPropertyValue(nameof(Resolution), value); DoPropertyChange(nameof(Resolution)); } }

        /// <summary>
        /// 结果谱图
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumResultSpectrum ResultSpectrum { get { return GetPropertyValue<EnumResultSpectrum>(nameof(ResultSpectrum)); } set { SetPropertyValue(nameof(ResultSpectrum), value); DoPropertyChange(nameof(ResultSpectrum)); } }

        /// <summary>
        /// 背景增益
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceGain BackGain { get { return GetPropertyValue<EnumDeviceGain>(nameof(BackGain)); } set { SetPropertyValue(nameof(BackGain), value); DoPropertyChange(nameof(BackGain)); } }

        /// <summary>
        /// 样品增益
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceGain SampleGain { get { return GetPropertyValue<EnumDeviceGain>(nameof(SampleGain)); } set { SetPropertyValue(nameof(SampleGain), value); DoPropertyChange(nameof(SampleGain)); } }

        /// <summary>
        /// 相位校准方法
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumFTPhaseCorrect PhaseCorrect { get { return GetPropertyValue<EnumFTPhaseCorrect>(nameof(PhaseCorrect)); } set { SetPropertyValue(nameof(PhaseCorrect), value); DoPropertyChange(nameof(PhaseCorrect)); } }

        /// <summary>
        /// 相位分辨率
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumFTPhaseResolution PhaseResolution { get { return GetPropertyValue<EnumFTPhaseResolution>(nameof(PhaseResolution)); } set { SetPropertyValue(nameof(PhaseResolution), value); DoPropertyChange(nameof(PhaseResolution)); } }

        /// <summary>
        /// 截止函数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumFTApodization Apodization { get { return GetPropertyValue<EnumFTApodization>(nameof(Apodization)); } set { SetPropertyValue(nameof(Apodization), value); DoPropertyChange(nameof(Apodization)); } }

        /// <summary>
        /// 填零系数
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumFTZeroFilling ZeroFilling { get { return GetPropertyValue<EnumFTZeroFilling>(nameof(ZeroFilling)); } set { SetPropertyValue(nameof(ZeroFilling), value); DoPropertyChange(nameof(ZeroFilling)); } }

        /// <summary>
        /// IVU滤光片
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public EnumDeviceIVU IVUFilter { get { return GetPropertyValue<EnumDeviceIVU>(nameof(IVUFilter)); } set { SetPropertyValue(nameof(IVUFilter), value); DoPropertyChange(nameof(IVUFilter)); } }

        /// <summary>
        /// 背景光谱有效期
        /// </summary>
        [XmlIgnore]
        public EnumBackgroundDuration BackgroundDuration { get { return GetPropertyValue<EnumBackgroundDuration>(nameof(BackgroundDuration)); } set { SetPropertyValue(nameof(BackgroundDuration), value); DoPropertyChange(nameof(BackgroundDuration)); } }

        /// <summary>
        /// X轴插值后的步长, 4cm-1 = 1, 8cm-1 = 2, 16cm-1=4;
        /// </summary>
        [XmlIgnore]
        public float XStep { get { return GetPropertyValue<float>(nameof(XStep)); } set { SetPropertyValue(nameof(XStep), value); DoPropertyChange(nameof(XStep)); } }

        /// <summary>
        /// 保存的光谱文件格式
        /// </summary>
        [XmlIgnore]
        public EnumSaveFileType SaveFileType { get { return GetPropertyValue<EnumSaveFileType>(nameof(SaveFileType)); } set { SetPropertyValue(nameof(SaveFileType), value); DoPropertyChange(nameof(SaveFileType)); } }

        /// <summary>
        /// 激光器波数
        /// </summary>
        [XmlIgnore]
        public float LaserWavelength { get { return GetPropertyValue<float>(nameof(LaserWavelength)); } set { SetPropertyValue(nameof(LaserWavelength), value); DoPropertyChange(nameof(LaserWavelength)); } }

        /// <summary>
        /// 是否需要扫描背景
        /// </summary>
        [XmlIgnore]
        public bool NeedScanBackground { get { return BackgroundSpectrum == null || (DateTime.Now - BackgroundTime).TotalMinutes >= (int)BackgroundDuration; } }

        #endregion

    }
}
