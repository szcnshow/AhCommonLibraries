using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Windows;
using Ai.Hong.Driver;
using Ai.Hong.FileFormat;
using Ai.Hong.Common;

// Instrument test
namespace Ai.Hong.Driver.IT
{
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
        Scanning=1,
        /// <summary>
        /// 完成一次检测
        /// </summary>
        oneFinished=2,
        /// <summary>
        /// 完成全部重复（扫描结束）
        /// </summary>
        repeateFinished=3,
        /// <summary>
        /// 参数错误
        /// </summary>
        parameterError=4,
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
        userAbort=7,
        /// <summary>
        /// 需要扫描背景
        /// </summary>
        backgroundError = 8,
    }

    ///// <summary>
    ///// 扫描通知消息参数
    ///// </summary>
    //public class ScanNotifyArgs:System.Windows.RoutedEventArgs
    //{
    //    /// <summary>
    //    /// 扫描状态
    //    /// </summary>
    //    public EnumScanNotifyState state { get; set; }
    //    /// <summary>
    //    /// 是否取消扫描
    //    /// </summary>
    //    public bool abortScan { get; set; }
    //    /// <summary>
    //    /// 错误信息
    //    /// </summary>
    //    public string errorString { get; set; }

    //    /// <summary>
    //    /// Scan notify argreements
    //    /// </summary>
    //    public ScanNotifyArgs():base()
    //    {
    //        this.abortScan = false;
    //    }

    //    /// <summary>
    //    /// Scan notify argreements
    //    /// </summary>
    //    /// <param name="routedEvent"></param>
    //    /// <param name="state"></param>
    //    /// <param name="errorString"></param>
    //    public ScanNotifyArgs(System.Windows.RoutedEvent routedEvent, EnumScanNotifyState state, string errorString):base(routedEvent)
    //    {
    //        this.abortScan = false;
    //        this.state = state;
    //        this.errorString = errorString;
    //    }
    //}

    /// <summary>
    /// 仪器自检基本类
    /// </summary>
    public class BaseSelfTestInfo:INotifyPropertyChanged
    {
        #region notifyevent
        /// <summary>
        /// 属性变更消息
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Property change event
        /// </summary>
        /// <param name="propertyName"></param>
        protected void DoPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlIgnore]
        public string ErrorString = null;

        /// <summary>
        /// 测试结果错误
        /// </summary>
        [XmlIgnore]
        protected const string invalidResultMsg = "TEST FAILED";

        private string _chineseName;
        /// <summary>
        /// 本测试的中文名称
        /// </summary>
        [XmlAttribute]
        public string ChineseName { get { return _chineseName; } set { _chineseName = value; DoPropertyChanged("ChineseName"); } }

        private string _englishName;
        /// <summary>
        /// 本测试的英文名称
        /// </summary>
        [XmlAttribute]
        public string EnglishName { get { return _englishName; } set { _englishName = value; DoPropertyChanged("EnglishName"); } }

        private bool _isSelected=true;
        /// <summary>
        /// 是否选中
        /// </summary>
        [XmlAttribute]
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; DoPropertyChanged("IsSelected"); } }

        private ScanParameter _measurePara;
        /// <summary>
        /// 数据采集参数
        /// </summary>
        [XmlIgnore]
        public ScanParameter MeasurePara { get { return _measurePara; } set { _measurePara = value; DoPropertyChanged("MeasurePara"); } }

        private double _firstX=4000;
        /// <summary>
        /// 计算的X起始值
        /// </summary>
        [XmlAttribute]
        public double firstX { get { return _firstX; } set { _firstX = value; DoPropertyChanged("firstX"); } }

        private double _lastX=10000;
        /// <summary>
        /// 计算的X结束值
        /// </summary>
        [XmlAttribute]
        public double lastX { get { return _lastX; } set { _lastX = value; DoPropertyChanged("lastX"); } }

        private double _targetResult;
        /// <summary>
        /// 计算的目标值
        /// </summary>
        [XmlAttribute]
        public double TargetResult { get { return _targetResult; } set { _targetResult = value; DoPropertyChanged("TargetResult"); } }

        /// <summary>
        /// 结果的单位
        /// </summary>
        [XmlAttribute]
        public string ResultUnit { get; set; }

        private double _lessThresold=0.1;
        /// <summary>
        /// 小于目标值的范围
        /// </summary>
        [XmlAttribute]
        public double LessThresold { get { return _lessThresold; } set { _lessThresold = value; DoPropertyChanged("LessThresold"); } }

        private double _greatThresold=0.1;
        /// <summary>
        /// 大于目标值的范围
        /// </summary>
        [XmlAttribute]
        public double GreatThresold { get { return _greatThresold; } set { _greatThresold = value; DoPropertyChanged("GreatThresold"); } }

        private int _backgroundCount=1;
        /// <summary>
        /// 背景扫描重复次数
        /// </summary>
        [XmlAttribute]
        public int BackgroundCount { get { return _backgroundCount; } set { _backgroundCount = value; DoPropertyChanged("BackgroundCount"); } }

        private int _sampleCount=0;
        /// <summary>
        /// 样品扫描重复次数
        /// </summary>
        [XmlAttribute]
        public int SampleCount { get { return _sampleCount; } set { _sampleCount = value; DoPropertyChanged("SampleCount"); } }

        private string _referenceFile = null;
        /// <summary>
        /// 参考图
        /// </summary>
        [XmlElement]
        public string ReferenceFile { get { return _referenceFile; } set { _referenceFile = value; DoPropertyChanged("ReferenceFile"); } }

        /// <summary>
        /// 最终计算结果
        /// </summary>
        [XmlIgnore]
        public double FinalResult { get; set; }

        /// <summary>
        /// 计算结果列表
        /// </summary>
        [XmlIgnore]
        public List<double> results = new List<double>();

        /// <summary>
        /// 测试中采集的光谱数据
        /// </summary>
        [XmlIgnore]
        public List<FileFormat.FileFormat> SpectraDatas = new List<FileFormat.FileFormat>();

        private int _resolution = 8;
        /// <summary>
        /// 分辨率
        /// </summary>
        [XmlAttribute]
        public int Resolution { get { return _resolution; } set { _resolution = value; DoPropertyChanged("Resolution"); } }

        private int _scanCount = 64;
        /// <summary>
        /// 扫描次数
        /// </summary>
        [XmlAttribute]
        public int ScanCount { get { return _scanCount; } set { _scanCount = value; DoPropertyChanged("ScanCount"); } }

        private int _zeroFilling = 1;
        /// <summary>
        /// 填零系数
        /// </summary>
        [XmlAttribute]
        public int ZeroFilling { get { return _zeroFilling; } set { _zeroFilling = value; DoPropertyChanged("ZeroFilling"); } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseSelfTestInfo()
        {
        }

        /// <summary>
        /// 构造函数(默认增益Gain 1, 默认相位校正Mertz，默认截趾函数Blackman_Harris_3_Term)
        /// </summary>
        /// <param name="chineseName">中文名称</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="firstX">起始计算波数</param>
        /// <param name="lastX">结束计算波数</param>
        /// <param name="resolution">分辨率</param>
        /// <param name="zeroFilling">填零系数</param>
        /// <param name="target">测试目标值</param>
        /// <param name="LessThresold">负偏差</param>
        /// <param name="GreatThresold">正偏差</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundCount">背景重复次数</param>
        /// <param name="sampleCount">样品重复次数</param>
        /// <param name="ResultUnit">测量结果单位</param>
        public BaseSelfTestInfo(string chineseName, string englishName, double firstX, double lastX, int resolution, int zeroFilling, double target, double LessThresold, double GreatThresold, int scanCount, int backgroundCount, int sampleCount, string ResultUnit)
        {
            this._chineseName = chineseName;
            this._englishName = englishName;
            this._firstX = firstX;
            this._lastX = lastX;
            this._lessThresold = LessThresold;
            this._greatThresold = GreatThresold;
            this._backgroundCount = backgroundCount;
            this._sampleCount = sampleCount;
            this._targetResult = target;

            this._resolution = resolution;
            this._scanCount = scanCount;
            this._zeroFilling = zeroFilling;
            this.ResultUnit = ResultUnit;
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public virtual ScanParameter GetScanParameter()
        {
            if (this._measurePara == null)
            {
                this._measurePara = new ScanParameter();
            }

            this._measurePara.StartWavelength = 4000.0f;
            this._measurePara.EndWavelength = 10000.0f;
            this._measurePara.Resolution = (EnumDeviceResolutions)Resolution;
            this._measurePara.ScanCount = ScanCount;
            this._measurePara.ZeroFilling =  EnumFTZeroFilling.Filling_1;

            //默认增益Gain 1, 默认相位校正Mertz，默认截趾函数Blackman_Harris_3_Term
            this._measurePara.BackGain = EnumDeviceGain.Gain_1;
            this._measurePara.SampleGain =EnumDeviceGain.Gain_1;
            this._measurePara.PhaseCorrect = EnumFTPhaseCorrect.Mertz;
            this._measurePara.Apodization= EnumFTApodization.Blackman_Harris_3_Term;
            this._measurePara.PhaseResolution = EnumFTPhaseResolution.Res_32;

            return _measurePara;
        }

        /// <summary>
        /// 计算测试结果，结果保存在FinalResult中，由各继承类自己实现
        /// </summary>
        /// <param name="calcuParameter">计算结果附加参数，继承类自解释</param>
        /// <returns>正确或者错误</returns>
        public virtual bool CalculateResult(dynamic calcuParameter = null)
        {
            return false;
        }     

        /// <summary>
        /// 
        /// </summary>
        public void SpectraScan()
        {

        }

        /// <summary>
        /// 是否在目标测试数据的区域内
        /// </summary>
        /// <returns></returns>
        public bool IsValidResult()
        {
            return FinalResult >= TargetResult - LessThresold && FinalResult <= TargetResult + GreatThresold;
        }

        /// <summary>
        /// 是否在目标测试数据的区域内
        /// </summary>
        /// <param name="value">测试结果</param>
        /// <param name="target">比较的目标</param>
        /// <param name="LessThresold">下偏差</param>
        /// <param name="GreatThresold">上偏差</param>
        /// <returns></returns>
        protected bool IsValidResult(double value, double target, double LessThresold, double GreatThresold)
        {
            return value >= target - LessThresold && value <= target + GreatThresold;
        }

        /// <summary>
        /// 是否是测试结果不满足条件的错误，还是其它的未知错误，如果是未知错误，需要查看ErrorString
        /// </summary>
        public bool JustTestFailed()
        {
            return ErrorString == invalidResultMsg;
        }

        /// <summary>
        /// 创建光谱保存文件名（序列号\测试名称\测试类型\日期\测试类型日期时间.spc
        /// </summary>
        /// <typeparam name="T">测试类型</typeparam>
        /// <param name="serialNo">仪器序列号</param>
        /// <param name="testName">测试名称：LWN, PQ, OQ</param>
        /// <param name="testInfo">测试类型</param>
        /// <returns>仪器测试光谱文件名</returns>
        public static string GenerateSaveFile<T>(string serialNo, string testName, T testInfo)
        {
            string typename = typeof(T).Name;
            typename = typename.Replace("TestInfo", "");    //去掉TestInfo
            string[] infos = new string[] { serialNo, testName, typename, DateTime.Now.ToString("yyyy-MM-dd"), typename + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".spc" };

            return System.IO.Path.Combine(infos);
        }

        /// <summary>
        /// 测试的显示名称
        /// </summary>
        /// <param name="language">当前所用语言</param>
        /// <returns></returns>
        public string DisplayName(EnumLanguage language)
        {
            return language == EnumLanguage.Chinese ? ChineseName : EnglishName;
        }
        
        /// <summary>
        /// 查找峰位
        /// </summary>
        /// <param name="xDatas">X轴数据</param>
        /// <param name="yDatas">Y轴数据</param>
        /// <param name="targetPeaks">目标峰位</param>
        /// <param name="upPeak">True=吸收峰位，False=透射峰位</param>
        /// <returns></returns>
        public double[] PickPeaks(double[] xDatas, double[] yDatas, double[] targetPeaks, bool upPeak)
        {
            double[] retPeaks = new double[targetPeaks.Length];
            double newy;

            for (int i = 0; i < targetPeaks.Length; i++ )
                retPeaks[i] = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, targetPeaks[i], 4, out newy, upPeak);

            return retPeaks;
        }

        /// <summary>
        /// 查找最高最低的峰位
        /// </summary>
        /// <param name="xDatas"></param>
        /// <param name="yDatas"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        protected void PickMaxMinPeak(double[] xDatas, double[] yDatas, out double minX, out double minY, out double maxX, out double maxY)
        {
            //查找最大值和最小值及其位置
            minX = xDatas[0];
            minY = double.MaxValue;
            maxX = xDatas[0];
            maxY = double.MinValue;
            
            for (int index = 0; index < xDatas.Length; index++)
            {
                if (yDatas[index] > maxY)
                {
                    maxY = yDatas[index];
                    maxX = xDatas[index];
                }
                if (yDatas[index] < minY)
                {
                    minY= yDatas[index];
                    minX = xDatas[index];
                }
            }

            //标注最高最低峰位并记录
            maxX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, maxX, 4, out maxY, true);
            minX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, minX, 4, out minY, false);
        }
    }

    /// <summary>
    /// 激光峰位校准
    /// </summary>
    public class LaserWavelengthTestInfo:BaseSelfTestInfo
    {
        /// <summary>
        /// 验证峰位
        /// </summary>
        [XmlElement]
        public List<double> verifyPeaks { get; set; }

        /// <summary>
        /// 验证峰位阈值
        /// </summary>
        [XmlAttribute]
        public double verifyPeakThreshold { get; set; }

        /// <summary>
        /// 设备激光波数
        /// </summary>
        [XmlAttribute]
        public double laserLength { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public LaserWavelengthTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="createNew">是否新建</param>
        public LaserWavelengthTestInfo(bool createNew) :
            base("激光波数校准", "Laser Wavelength Correct", 5200, 5500, 4, 8, 5230.4987, 0.1, 0.1, 64, 3, 0, "cm-1")
        {
            this.laserLength = 15798.620000;
            this.verifyPeaks = new List<double>() { 5254.3411, 5474.9066 };
            this.verifyPeakThreshold = 1.0;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();

            try
            {
                Trace.Assert(SpectraDatas != null && SpectraDatas.Count != 0 && verifyPeaks != null && verifyPeaks.Count == 2, "Invalid SpectraDatas or verifyPeaks");

                double picked;

                //判断目标峰位7181.68和验证峰位7232.29, 7242.77是否在阈值0.1内
                FinalResult = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, TargetResult, 4, out picked, false);
                var verResult0 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, verifyPeaks[0], 4, out picked, false);
                var verResult1 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, verifyPeaks[1], 4, out picked, false);
                results.Add(FinalResult);
                results.Add(verResult0);
                results.Add(verResult1);

                if (!IsValidResult() ||
                    !IsValidResult(verResult0, verifyPeaks[0], verifyPeakThreshold, verifyPeakThreshold) ||     //验证峰位阈值为1.0cm-1
                    !IsValidResult(verResult1, verifyPeaks[1], verifyPeakThreshold, verifyPeakThreshold))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 调整激光频率
        /// </summary>
        public void AdjustLaserWavelength()
        {
            //FinalResult = laserLength * TargetResult / results[0];

            //Modify laser wavelength
        }
    }

    /// <summary>
    /// 100%线噪声测试(100% Line Noise Test)
    /// </summary>
    public class LineNoiseTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 单通道谱图
        /// </summary>
        [XmlIgnore]
        public List<Ai.Hong.FileFormat.FileFormat> singleBeams = new List<Ai.Hong.FileFormat.FileFormat>();

        /// <summary>
        /// 干涉图
        /// </summary>
        [XmlIgnore]
        public List<Ai.Hong.FileFormat.FileFormat> interfers = new List<Ai.Hong.FileFormat.FileFormat>();

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public LineNoiseTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public LineNoiseTestInfo(bool createNew)
            : base("100%线噪声测试", "100% Line Noise Test", 5900, 6100, 4, 0, 0, double.MaxValue, 0.1, 8, 10, 10, "%")
        {
            
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();

            double minX, minY, maxX, maxY;
            foreach (var data in SpectraDatas)
            {
                //pp峰值  取透射谱数据
                var rangeDatas = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>(){data.xDatas, data.yDatas}, firstX, lastX);

                //标注最大最小峰位并记录
                PickMaxMinPeak(rangeDatas[0],rangeDatas[1], out minX, out minY, out maxX, out maxY);

                results.Add(Math.Abs(maxY - minY));
            }

            //计算平均APP, 计算最大的 max(PP-APP)
            FinalResult = results.Average();

            return IsValidResult();
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public override ScanParameter GetScanParameter()
        {
            //采集透射谱，保存单通道谱和干涉谱
            var para = base.GetScanParameter();
            para.ResultSpectrum = EnumResultSpectrum.Transmittance;
            para.SaveSingleBeam = true;
            para.SaveInterfere = true;
            return para;
        }
    }

    /// <summary>
    /// 100%线斜率测试(100% Line Deviation Test)
    /// </summary>
    public class DeviationTestInfo:BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public DeviationTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public DeviationTestInfo(bool createNew)
            : base("100%线偏离测试", "100% Line Deviation Test", 4500, 10000, 4, 0, 0, double.MaxValue , 0.5, 8, 10, 10, "%")
        {
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();
            foreach(var spcdata in SpectraDatas)
            {
                var rangeDatas  = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>(){spcdata.xDatas, spcdata.yDatas}, firstX, lastX);
                var yDatas = rangeDatas[1];

                //偏差计算（YDatas)
                double max = Math.Abs( yDatas.Max() - 100);
                double min = Math.Abs(yDatas.Min() - 100);

                results.Add(max > min ? max : min);
            }

            FinalResult = results.Average();

            return IsValidResult();
        }
    }

    /// <summary>
    /// 干涉图峰位振幅测试（Long Term Stability Test, Interferogram Peak Amplitude Test）
    /// </summary>
    public class InterferPeakTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public InterferPeakTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数(TargetResult = 参考光谱=100% Thresold>70%
        /// </summary>
        public InterferPeakTestInfo(bool createNew)
            : base("干涉图振幅测试", "Interferogram Peak Amplitude Test", 4500, 10000, 4, 0, 100.0, 30.0, double.MaxValue, 8, 1, 1, "%")
        {
            ReferenceFile = "Reference_BgInterferogram.spc";
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public override ScanParameter GetScanParameter()
        {
            var para = base.GetScanParameter();
            para.SaveInterfere = true;

            return para;
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="calcuParameter">参考干涉图</param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter)
        {
            results = new List<double>();

            double minX, minY, maxX, maxY;

            //参考干涉图的振幅
            FileFormat.FileFormat referenceData = new FileFormat.FileFormat(calcuParameter  as string);
            if(referenceData.xDatas == null)
            {
                ErrorString = "Cannot find reference file:" + ReferenceFile;
                return false;
            }
            PickMaxMinPeak(referenceData.xDatas, referenceData.yDatas, out minX, out minY, out maxX, out maxY);
            var refValue = Math.Abs(maxY - minY);

            foreach (var data in SpectraDatas)
            {
                //标注最大最小峰位并记录
                PickMaxMinPeak(data.xDatas, data.yDatas, out minX, out minY, out maxX, out maxY);

                results.Add(Math.Abs(maxY - minY) * 100 / refValue);    //变成%
            }
            FinalResult = results.Average();
            
            return IsValidResult();
        }
    }

    /// <summary>
    /// 能量测试（Energy Test）
    /// </summary>
    public class EnergyTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public EnergyTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold 《 30%
        /// </summary>
        public EnergyTestInfo(bool createNew)
            : base("光源能量衰减测试", "Source Energy Attenuation Test", 4000, 10000, 4, 0, 0, double.MaxValue, 30, 8, 10, 10, "%")
        {
            //参考背景单通道图
            ReferenceFile = "Reference_BgSinglebeam.spc";
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public override ScanParameter GetScanParameter()
        {
            var para = base.GetScanParameter();
            para.SaveSingleBeam = true;

            return para;
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="calcuParameter">参考干涉图</param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter)
        {
            results = new List<double>();

            //参考单通道背景图的积分
            FileFormat.FileFormat referenceData = new FileFormat.FileFormat(calcuParameter as string);
            if (referenceData.xDatas == null)
            {
                ErrorString = "Cannot find reference file:" + ReferenceFile;
                return false;
            }
            var refValue = Ai.Hong.Algorithm.CommonAlgorithm.Integrate(referenceData.xDatas, referenceData.yDatas, firstX, lastX);

            //当前测量得到的单通道背景图的积分，计算与原有参考光谱积分的差
            foreach(var data in SpectraDatas)
            {
                var scanvalue = Ai.Hong.Algorithm.CommonAlgorithm.Integrate(data.xDatas, data.yDatas, firstX, lastX);
                results.Add(Math.Abs(refValue - scanvalue) / refValue);
            }

            FinalResult = results.Average() * 100;

            //平均差小于30%为合格
            return IsValidResult();
        }
    }

    /// <summary>
    /// 水蒸气峰位精度（Wavenumber Accuracy Test）
    /// </summary>
    public class VaporAccuracyTestInfo : BaseSelfTestInfo
    {
        private List<double> _verifyPeaks = null;
        /// <summary>
        /// 验证峰位
        /// </summary>
        [XmlElement]
        public List<double> verifyPeaks { get { return _verifyPeaks; } set { _verifyPeaks = value; DoPropertyChanged("verifyPeaks"); } }

        private double _laserLength { get; set; }
        /// <summary>
        /// 设备激光波数
        /// </summary>
        [XmlAttribute]
        public double laserLength { get { return _laserLength; } set { _laserLength = value; DoPropertyChanged("layerLength"); } }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public VaporAccuracyTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数Thresold ±0.1
        /// </summary>
        /// <param name="createNew">是否为新建</param>
        public VaporAccuracyTestInfo(bool createNew) :
            base("水蒸气峰位精度", "Vapor Wavenumber Test", 7100, 7250, 4, 8, 7181.68, 0.1, 0.1, 20, 1, 0, "cm-1")
        {
            this._laserLength = 15798.620000;
            this._verifyPeaks = new List<double>() { 7232.29, 7242.77 };
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();

            try
            {
                Trace.Assert(SpectraDatas != null && SpectraDatas.Count != 0 && verifyPeaks != null && verifyPeaks.Count == 2, "Invalid SpectraDatas or verifyPeaks");

                double picked;
                double verifyThresold = 0.5;

                //判断目标峰位7181.68和验证峰位7232.29, 7242.77是否在阈值0.1内
                FinalResult = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, TargetResult, 4, out picked, false);
                var verResult0 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, verifyPeaks[0], 4, out picked, false);
                var verResult1 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, verifyPeaks[1], 4, out picked, false);
                if (!IsValidResult() ||
                    !IsValidResult(verResult0, verifyPeaks[0], verifyThresold, verifyThresold) ||
                    !IsValidResult(verResult1, verifyPeaks[1], verifyThresold, verifyThresold))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 调整激光频率
        /// </summary>
        public void AdjustLaserWavelength()
        {
            double newvalue = laserLength * results[0] / TargetResult;

            //Modify laser wavelength
        }
    }

    /// <summary>
    /// 聚苯乙烯峰位测试，先扫描背景光谱，然后将IVU转换到聚苯乙烯，扫描带有聚苯乙烯的背景光谱，计算聚苯乙烯的吸收谱
    /// </summary>
    public class PolyAccuracyTestInfo : BaseSelfTestInfo
    {        
        private Driver.EnumDeviceIVU _IVUFilter = Driver.EnumDeviceIVU.Polystyrene;
        /// <summary>
        /// 验证轮位置
        /// </summary>
        [XmlAttribute]
        public Driver.EnumDeviceIVU IVUFilter { get { return _IVUFilter; } set { _IVUFilter = value; } }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public PolyAccuracyTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数Thresold±0.5
        /// </summary>
        public PolyAccuracyTestInfo(bool createNew)
            : base("聚苯乙烯峰位测试", "Polystyrene Wavelength Test", 4560, 4580, 4, 0, 4571.0, 0.5, 0.5, 64, 1, 0, "cm-1")
        {
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public override ScanParameter GetScanParameter()
        {
            var para = base.GetScanParameter();
            para.IVUFilter = IVUFilter;

            return para;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();

            double yvalue;

            //获得仪器温度
            //var tempstr = curInstrument.HardwareGetProperty(Driver.EnumHardware.Device, Driver.EnumHardwareProperties.Temperature);
            //float temperature = float.Parse(tempstr);
            float temperature = 35.0f;

            FinalResult = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, TargetResult, 4, out yvalue, false);

            //T: instrument internal temperature in degree C
            //a. Fiber system, Report Peak Position = Measured Peak Position + 0.0107*T - 0.7
            //b. Integrating sphere system, Report Peak Position = (4571.0 * Measured Peak Position) / (-0.0205 * T + 4571.575)

            FinalResult = (4571 * FinalResult) / (4571.575 - 0.0205 * temperature);

            //临时屏蔽
            //if (curInstrument.deviceModel == Driver.EnumDeviceModel.SphereIntegrate)
            //{
            //    FinalResult = (4571 * FinalResult) / (4571.575 - 0.0205 * temperature);
            //}
            //else if (curInstrument.deviceModel == Driver.EnumDeviceModel.Fiber)
            //{
            //    FinalResult = FinalResult + temperature * 0.0107 - 0.7;
            //}

            return IsValidResult();
        }
    }

    /// <summary>
    /// 分光器稳定性测试(Photometric Accuracy Test)
    /// </summary>
    public class PhotometricTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public PhotometricTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public PhotometricTestInfo(bool createNew)
            : base("光度精确度测试", "Photometric Accuracy Test", 4500, 10000, 8, 8, 0, double.MaxValue, 0.8, 50, 1, 0, "%")
        {
            //参考Class Filter 透射图
            ReferenceFile = "Reference_GlassTransmission.spc";
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter">参考干涉图</param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter)
        {
            results = new List<double>();

            //加载参考透射光谱
            FileFormat.FileFormat referenceData = new FileFormat.FileFormat(calcuParameter as string);
            if (referenceData.xDatas == null)
            {
                ErrorString = "Cannot find reference file:" + ReferenceFile;
                return false;
            }            
            var oldData = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { referenceData.xDatas, referenceData.yDatas }, firstX, lastX)[1];

            var newData = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { SpectraDatas[0].xDatas, SpectraDatas[0].yDatas }, firstX, lastX)[1];

            double total = 0;
            for (int i = 0; i < oldData.Length; i++)
                total += newData[i] - oldData[i];
            FinalResult = Math.Abs(total / oldData.Length);

            return IsValidResult();
        }
    }

    /// <summary>
    /// 测试组合
    /// </summary>
    public class SelTestGroup
    {
        /// <summary>
        /// 组合的内部名称
        /// </summary>
        [XmlAttribute]
        public string innerName { get; set; }

        /// <summary>
        /// 组合的中文名称
        /// </summary>
        [XmlAttribute]
        public string chineseName { get; set; }

        /// <summary>
        /// 组合的英文名称
        /// </summary>
        [XmlAttribute]
        public string englishName { get; set; }

        /// <summary>
        /// 测试的内容
        /// </summary>
        [XmlArray("testItems")]
        [XmlArrayItem("testItem")]
        public List<BaseSelfTestInfo> testItems { get; set; }

        /// <summary>
        /// 测试的显示名称
        /// </summary>
        /// <param name="language">当前所用语言</param>
        /// <returns></returns>
        public string DisplayName(EnumLanguage language)
        {
            return language == EnumLanguage.Chinese ? chineseName : englishName;
        }
    }

    /// <summary>
    /// PQ测试参数信息
    /// </summary>
    [XmlInclude(typeof(LaserWavelengthTestInfo))]
    [XmlInclude(typeof(LineNoiseTestInfo))]
    [XmlInclude(typeof(DeviationTestInfo))]
    [XmlInclude(typeof(InterferPeakTestInfo))]
    [XmlInclude(typeof(EnergyTestInfo))]
    [XmlInclude(typeof(VaporAccuracyTestInfo))]
    [XmlInclude(typeof(PolyAccuracyTestInfo))]
    [XmlInclude(typeof(PhotometricTestInfo))]
    public class PQTestInfo
    {
        private int _PQTestHours = 24;
        /// <summary>
        /// 需要PQ测试的时间间隔
        /// </summary>
        [XmlAttribute]
        public int PQTestHours { get { return _PQTestHours; } set { _PQTestHours = value; } }

        private DateTime _lastPQTestTime = new DateTime(2018, 1, 1, 1, 1, 1);
        /// <summary>
        /// 上一次PQ测试时间
        /// </summary>
        [XmlAttribute]
        public DateTime lastPQTestTime { get; set; }

        /// <summary>
        /// 激光波数校准
        /// </summary>
        [XmlElement]
        public SelTestGroup laserTest { get; set; }

        /// <summary>
        /// 能量测试
        /// </summary>
        [XmlElement]
        public SelTestGroup energyTest { get; set; }

        /// <summary>
        /// 波数精度测试
        /// </summary>
        [XmlElement]
        public SelTestGroup wavelengthTest { get; set; }

        /// <summary>
        /// 吸收重复性测试
        /// </summary>
        [XmlElement]
        public SelTestGroup absorbTest { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PQTestInfo()
        {

        }

        /// <summary>
        /// 创建新的PQ测试参数
        /// </summary>
        /// <param name="createNew"></param>
        public PQTestInfo(bool createNew)
        {
            laserTest = new SelTestGroup()
            {
                innerName = "laserCalibrate",
                chineseName = "激光校准",
                englishName = "Laser Calibrate",
                testItems = new List<BaseSelfTestInfo>() { new LaserWavelengthTestInfo(true) }
            };

            energyTest = new SelTestGroup()
            {
                innerName = "energyTest",
                chineseName = "能量测试",
                englishName = "Energy Testing",
                testItems = new List<BaseSelfTestInfo>() { new LineNoiseTestInfo(true), new DeviationTestInfo(true), new InterferPeakTestInfo(true), new EnergyTestInfo(true) }
            };

            wavelengthTest = new SelTestGroup()
            {
                innerName = "waveAccuracyTest",
                chineseName = "波数精度测试",
                englishName = "Wavelength Accuracy Testing",
                testItems = new List<BaseSelfTestInfo>() { new VaporAccuracyTestInfo(true), new PolyAccuracyTestInfo(true) }
            };

            absorbTest = new SelTestGroup()
            {
                innerName = "photometricTest",
                chineseName = "光学精度测试",
                englishName = "Photometric Accuracy Test",
                testItems = new List<BaseSelfTestInfo>() { new PhotometricTestInfo(true)}
            };
        }

        /// <summary>
        /// 获取所有的测试项目
        /// </summary>
        /// <returns></returns>
        public List<SelTestGroup> GetAllTestGroups()
        {
            var retDatas = new List<SelTestGroup>(){
                laserTest,
                energyTest,
                wavelengthTest,
                absorbTest,
            };

            //移除没有选中的测试项目
            foreach(var item in retDatas)
                item.testItems.RemoveAll(p => p.IsSelected == false);
            retDatas.RemoveAll(p => p.testItems.Count == 0);

            return retDatas;
        }
    }


    /// <summary>
    /// 分辨率测试
    /// </summary>
    public class ResolutionTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 计算分辨率的峰位
        /// </summary>
        [XmlElement]
        public double resolutionPeak { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public ResolutionTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="createNew"></param>
        public ResolutionTestInfo(bool createNew) :
            base("分辨率测试", "Resolution Test", 4000, 10000, 4, 16, 4.0, double.MaxValue, 0, 20, 1, 0, "cm-1")
        {
            this.resolutionPeak = 7294.12;
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public override ScanParameter GetScanParameter()
        {
            var para = base.GetScanParameter();
            para.Apodization = EnumFTApodization.BoxCar;

            return para;
        }

        /// <summary>
        /// 计算中点
        /// </summary>
        /// <param name="firstX"></param>
        /// <param name="firstY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <returns></returns>
        private string CalMidPoint(double firstX, double firstY, double endX, double endY, double targetX, double targetY)
        {

            if (firstY == endY)
            {
                return ((firstX + endX) / 2).ToString() + "+" + ((targetY + firstY) / 2).ToString();
            }
            else
            {
                double temp = targetX * (firstX - endX) * (firstX - endX) - (endY * targetY - firstY * targetY) * (firstX - endX) + (firstX * endY - firstY * endX) * (endY - firstY);
                double x = temp / ((endY - firstY) * (endY - firstY) + (endX - firstX) * (endX - firstX));
                double y = (firstX * x - endX * x + endX * targetX - firstX * targetX - firstY * targetY + endY * targetY) / (endY - firstY);
                return ((x + targetX) / 2).ToString() + "+" + ((y + targetY) / 2).ToString();
            }
        }

        //private double CalMidPoint(double x1, double y1, double x2, double y2, double peakX, double peakY)
        //{
        //    double midx, midy;

        //    if (y1 == y2)
        //    {
        //        midx = x1 + (x2 - x1) / 2;
        //        midy = y1 + (peakY - y1) / 2;
        //    }
        //    else
        //    {
        //        double temp = targetX * (firstX - endX) * (firstX - endX) - (endY * targetY - firstY * targetY) * (firstX - endX) + (firstX * endY - firstY * endX) * (endY - firstY);
        //        double x = temp / ((endY - firstY) * (endY - firstY) + (endX - firstX) * (endX - firstX));
        //        double y = (firstX * x - endX * x + endX * targetX - firstX * targetX - firstY * targetY + endY * targetY) / (endY - firstY);
        //        return ((x + targetX) / 2).ToString() + "+" + ((y + targetY) / 2).ToString();
        //    }
        //}

        /// <summary>
        /// 分辨率计算
        /// </summary>
        /// <param name="firstX"></param>
        /// <param name="firstY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="midPointX"></param>
        /// <param name="midPointY"></param>
        /// <param name="DataX"></param>
        /// <param name="DataY"></param>
        /// <param name="IsUpPeak">True=向上的峰，False=向下的峰</param>
        /// <returns></returns>
        private double CalResolution(double firstX, double firstY, double endX, double endY, double midPointX, double midPointY, double[] DataX, double[] DataY, bool IsUpPeak)
        {
            double Resolution = Ai.Hong.Algorithm.CommonAlgorithm.Integrate(DataX, DataY, firstX, endX, IsUpPeak); 
            int tar = Ai.Hong.Algorithm.CommonMethod.FindNearestPosition(DataX, 0, DataX.Length - 1, resolutionPeak);
            double tary;
            double tarx = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(DataX, DataY, resolutionPeak, 4, out tary, true);
            string hi = CalMidPoint(firstX, firstY, endX, endY, tarx, tary);
            string[] tt = hi.Split('+');
            double xMid = Convert.ToDouble(tt[0]);
            double yMid = Convert.ToDouble(tt[1]);
            double tem = (xMid - tarx) * (xMid - tarx) + (yMid - tary) * (yMid - tary);
            //峰高
            double PickHeight = 2 * Math.Sqrt(tem);
            Resolution = Resolution / PickHeight;
            return Resolution;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter">bool, True=向上的峰，False=向下的峰</param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter)
        {
            var xDatas = SpectraDatas[0].xDatas;
            var yDatas = SpectraDatas[0].yDatas;

            //计算 - log
            //double[] yDatas = new double[SpectraDatas[0].yDatas.Length];
            //for (int i = 0; i < yDatas.Length; i++)
            //    yDatas[i] = -Math.Log10(SpectraDatas[0].yDatas[i]);

            //标定峰位（向上的峰位）
            bool IsUpPeak = (bool)calcuParameter;
            double newYValue;
            double targetPeak = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, resolutionPeak, 4, out newYValue, IsUpPeak);
            double newStartY;
            double newStartX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, resolutionPeak - 2, 4, out newStartY, !IsUpPeak);
            double newEndY;
            double newEndX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, resolutionPeak + 2, 4, out newEndY, !IsUpPeak);

            string midPoint = CalMidPoint(newStartX, newStartY, newEndX, newEndY, targetPeak, newYValue);
            string[] mid = midPoint.Split('+');
            double midPointX = Convert.ToDouble(mid[0]);
            double midPointY = Convert.ToDouble(mid[1]);

            int midX = Ai.Hong.Algorithm.CommonMethod.FindNearestPosition(xDatas, 0, xDatas.Length - 1, midPointX);
            FinalResult = CalResolution(newStartX, newStartY, newEndX, newEndY, midPointX, midPointY, xDatas, yDatas, IsUpPeak);

            return IsValidResult();
        }

    }

    /// <summary>
    /// 能量分布测试(Energy Distribution Test)
    /// </summary>
    public class EnergyDistributeTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 计算比例的X位置
        /// </summary>
        [XmlElement]
        public double targetX { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public EnergyDistributeTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public EnergyDistributeTestInfo(bool createNew)
            : base("能量分布测试", "Energy Distribution Test", 4000, 10000, 8, 8, 0, double.MaxValue, 10, 8, 1, 0, "%")
        {
            targetX = 10000.0;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            var xDatas = SpectraDatas[0].xDatas;
            var yDatas = SpectraDatas[0].yDatas;

            //找到最大Y值
            var maxY = yDatas.Max();

            //找到10000处的Y值
            int x = Ai.Hong.Algorithm.CommonMethod.FindNearestPosition(xDatas, 0, xDatas.Length - 1, targetX);
            double curY = x == -1 ? yDatas[yDatas.Length - 1] : yDatas[x];

            FinalResult = (curY / maxY) * 100;

            return IsValidResult();
        }
    }

    /// <summary>
    /// 100%线斜率测试(100% Line Slope Test)
    /// </summary>
    public class LineSlopeTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 计算的X位置
        /// </summary>
        [XmlElement]
        public List<System.Windows.Point> slopeX { get; set; }

        /// <summary>
        /// 计算的阈值范围
        /// </summary>
        [XmlElement]
        public List<System.Windows.Point> slopeThresold { get; set; }

        /// <summary>
        /// 检测到的结果
        /// </summary>
        [XmlIgnore]
        public List<System.Windows.Point> slopeResult { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public LineSlopeTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public LineSlopeTestInfo(bool createNew)
            : base("100%线斜率测试", "100% Line Slope Test", 4000, 10000, 8, 8, 0, double.MaxValue, 10, 8, 1, 1, "%")
        {
            slopeX = new List<System.Windows.Point>() 
            { 
                new System.Windows.Point(4500, 4900),
                new System.Windows.Point(6000, 6400),
                new System.Windows.Point(7500, 7900),
                new System.Windows.Point(9000, 9400),
            };
            slopeThresold = new List<System.Windows.Point>()
            {
                new System.Windows.Point(98.0, 102.0),
                new System.Windows.Point(99.0, 101.0),
                new System.Windows.Point(99.0, 101.0),
                new System.Windows.Point(98.0, 102.0),
            };
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public override ScanParameter GetScanParameter()
        {
            var para = base.GetScanParameter();
            para.ResultSpectrum = EnumResultSpectrum.Transmittance;

            return para;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            var xDatas = SpectraDatas[0].xDatas;
            var yDatas = SpectraDatas[0].yDatas;

            slopeResult = new List<System.Windows.Point>();
            results = new List<double>();

            for (int i = 0; i < slopeX.Count; i++ )
            {
                var rangeDatas = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { xDatas, yDatas }, slopeX[i].X, slopeX[i].Y);
                double max = rangeDatas[1].Max();
                double min = rangeDatas[1].Min();

                slopeResult.Add(new System.Windows.Point(min, max));
            }

            return IsValidResult();
        }

        /// <summary>
        /// 特别的结果判断
        /// </summary>
        /// <returns></returns>
        public new bool IsValidResult()
        {
            if (slopeResult == null || slopeResult.Count == 0 || slopeThresold == null ||slopeThresold.Count != slopeResult.Count)
                return false;

            //计算各区间的最小最大Y值
            bool result = true;

            for (int i = 0; i < slopeX.Count; i++)
            {
                if (slopeResult[i].X < slopeThresold[i].X || slopeResult[i].Y > slopeThresold[i].Y)
                    result = false;
            }

            return result;
        }
    }

    /// <summary>
    /// 透射重复性测试(Transmittance Reproducibility)
    /// </summary>
    public class TransmitReproductTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public TransmitReproductTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public TransmitReproductTestInfo(bool createNew)
            : base("透射重复性测试", "Transmittance Reproducibility Test", 6500, 10000, 8, 1, 0, double.MaxValue, 2, 8, 1, 8, "%")
        {
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <returns></returns>
        public override ScanParameter GetScanParameter()
        {
            var para = base.GetScanParameter();
            para.BackGain = Driver.EnumDeviceGain.Gain_1;
            para.ResultSpectrum = EnumResultSpectrum.Transmittance;

            return para;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();
            foreach (var spcdata in SpectraDatas)
            {
                var rangeDatas = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { spcdata.xDatas, spcdata.yDatas }, firstX, lastX);
                var yDatas = rangeDatas[1];

                //偏差计算（YDatas)
                double max = Math.Abs(yDatas.Max() - 100);
                double min = Math.Abs(yDatas.Min() - 100);

                results.Add(max > min ? max : min);
            }

            FinalResult = results.Max();

            return IsValidResult();
        }
    }

    /// <summary>
    /// 波数重现性测试（Wavenumber Reproducibility Test）
    /// </summary>
    public class WavenumberReproductTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 验证峰位
        /// </summary>
        [XmlElement]
        public double verifyPeak { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public WavenumberReproductTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数Thresold ±0.1
        /// </summary>
        /// <param name="createNew"></param>
        public WavenumberReproductTestInfo(bool createNew) :
            base("波数重现性测试", "Wavenumber Reproducibility Test", 7100, 7250, 4, 8, 0, 0.05, 0.05, 20, 6, 0, "cm-1")
        {
            verifyPeak = 7181.68;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();

            foreach (var data in SpectraDatas)
            {
                double picked;

                //判断目标峰位7181.68
                var curpeak = Algorithm.CommonAlgorithm.PickPeak(data.xDatas, data.yDatas, verifyPeak, 4, out picked, false);
                results.Add(curpeak);
            }

            //获取最大偏离值
            FinalResult = results.Max() - results.Min();

            return IsValidResult();
        }
    }

    /// <summary>
    /// PQ测试参数信息
    /// </summary>
    [XmlInclude(typeof(LaserWavelengthTestInfo))]
    [XmlInclude(typeof(ResolutionTestInfo))]
    [XmlInclude(typeof(LineNoiseTestInfo))]
    [XmlInclude(typeof(LineSlopeTestInfo))]
    [XmlInclude(typeof(EnergyDistributeTestInfo))]
    [XmlInclude(typeof(TransmitReproductTestInfo))]
    [XmlInclude(typeof(VaporAccuracyTestInfo))]
    [XmlInclude(typeof(WavenumberReproductTestInfo))]
    [XmlInclude(typeof(InterferPeakTestInfo))]
    [XmlInclude(typeof(EnergyTestInfo))]
    [XmlInclude(typeof(PhotometricTestInfo))]
    public class OQTestInfo
    {
        #region properties
        private int _OQTestDays = 365;
        /// <summary>
        /// 需要OQ测试的时间间隔
        /// </summary>
        [XmlAttribute]
        public int OQTestDays { get { return _OQTestDays; } set { _OQTestDays = value; } }

        private DateTime lastOQTestDays = new DateTime(2018, 1, 1, 1, 1, 1);
        /// <summary>
        /// 上一次PQ测试时间
        /// </summary>
        [XmlAttribute]
        public DateTime lastOQTestTime { get; set; }

        /// <summary>
        /// 激光波数校准
        /// </summary>
        [XmlElement]
        public SelTestGroup laserTest { get; set; }

        /// <summary>
        /// 分辨率测试
        /// </summary>
        [XmlElement]
        public SelTestGroup resolutionTest { get; set; }

        /// <summary>
        /// 光路稳定性测试
        /// </summary>
        [XmlElement]
        public SelTestGroup photometricTest { get; set; }

        /// <summary>
        /// 波数精度测试
        /// </summary>
        [XmlElement]
        public SelTestGroup wavelengthTest { get; set; }

        /// <summary>
        /// 采集仪器的参考光谱
        /// </summary>
        [XmlElement]
        public SelTestGroup scanReferenceData { get; set; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public OQTestInfo()
        {

        }

        /// <summary>
        /// 创建新的PQ测试参数
        /// </summary>
        /// <param name="createNew"></param>
        public OQTestInfo(bool createNew)
        {
            laserTest = new SelTestGroup()
            {
                innerName = "laserCalibrate",
                chineseName = "激光校准",
                englishName = "Laser Calibrate",
                testItems = new List<BaseSelfTestInfo>() { new LaserWavelengthTestInfo(true) }
            };

            resolutionTest = new SelTestGroup()
            {
                innerName = "resolutionTest",
                chineseName = "分辨率测试",
                englishName = "Resolution Testing",
                testItems = new List<BaseSelfTestInfo>() { new ResolutionTestInfo(true) }
            };

            photometricTest = new SelTestGroup()
            {
                innerName = "photometricTest",
                chineseName = "光路稳定性测试",
                englishName = "Photometric Accuracy Testing",
                testItems = new List<BaseSelfTestInfo>() { new LineNoiseTestInfo(true), new LineSlopeTestInfo(true), new EnergyDistributeTestInfo(true), new TransmitReproductTestInfo(true) }
            };

            wavelengthTest = new SelTestGroup()
            {
                innerName = "waveAccuracyTest",
                chineseName = "波数精度测试",
                englishName = "Wavelength Accuracy Testing",
                testItems = new List<BaseSelfTestInfo>() { new VaporAccuracyTestInfo(true), new PolyAccuracyTestInfo(true), new WavenumberReproductTestInfo(true) }
            };

            scanReferenceData = new SelTestGroup()
            {
                innerName = "scanRefereceData",
                chineseName = "采集和保存仪器参考谱图",
                englishName = "Scan & Save Reference Spectra",
                testItems = new List<BaseSelfTestInfo>() { new InterferPeakTestInfo(true), new EnergyTestInfo(true), new PhotometricTestInfo(true)}
            };
        }

        /// <summary>
        /// 获取所有的测试项目
        /// </summary>
        /// <returns></returns>
        public List<SelTestGroup> GetAllTestGroups()
        {
            var retDatas = new List<SelTestGroup>(){
                laserTest,
                resolutionTest,
                photometricTest,
                wavelengthTest,
            };

            //移除没有选中的测试项目
            foreach (var item in retDatas)
                item.testItems.RemoveAll(p => p.IsSelected == false);
            retDatas.RemoveAll(p => p.testItems.Count == 0);

            return retDatas;
        }
    }

    /// <summary>
    /// 创建XPS报告
    /// </summary>
    public class XPSReport
    {
        private Border totalBorder = null;
        private Border detailBorder = null;

        private static EnumLanguage language = EnumLanguage.Chinese;

        const double DPCM = 96 / 2.54;      //1cm中的像素点数量
        const double headerFontSize = 20;
        const double textFontSize = 13;
        private static System.Windows.Media.SolidColorBrush headerBackground = System.Windows.Media.Brushes.LightSteelBlue;
        private static System.Windows.Media.SolidColorBrush headerForeground = System.Windows.Media.Brushes.Black;
        private static System.Windows.Media.SolidColorBrush textBackground = System.Windows.Media.Brushes.White;
        private static System.Windows.Media.SolidColorBrush textForeground = System.Windows.Media.Brushes.Black;

        private FTDriver scanner = null;

        private string UserName;
        private string UnitName;
        private EnumLanguage Language = EnumLanguage.Chinese;
        /// <summary>
        /// AHCommonResources 资源目录
        /// </summary>
        private ResourceDictionary resourceDir;

        /// <summary>
        /// 构造函数
        /// </summary>
        public XPSReport()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scanner">测试仪器</param>
        /// <param name="totalTemplate">报告总模板</param>
        /// <param name="detailTemplate">详细报告模板</param>
        /// <param name="UserName">测量人员名称</param>
        /// <param name="UnitName">测量单位名称</param>
        /// <param name="resourceDir">结果图形路径</param>
        /// <param name="language">语言</param>
        public XPSReport(FTDriver scanner, string totalTemplate, string detailTemplate, string UserName, string UnitName, 
            ResourceDictionary resourceDir,
            EnumLanguage language= EnumLanguage.Chinese)
        {
            totalBorder = GetRootBorder(totalTemplate);
            detailBorder = GetRootBorder(detailTemplate);
            this.scanner = scanner;
            this.UserName = UserName;
            this.UnitName = UnitName;
            this.Language = language;
            this.resourceDir = resourceDir;
        }       
        
        /// <summary>
        /// 获取模板的rootBorder
        /// </summary>
        /// <param name="templateName">模板名称</param>
        /// <returns></returns>
        private Border GetRootBorder(string templateName)
        {
            var assemb = System.Reflection.Assembly.GetExecutingAssembly();
            var templateDoc = ResourceOperator.EmbededResourceElement(assemb, templateName) as FlowDocument;
            var blockUI = Ai.Hong.Controls.Common.XPSReportTemplate.GetBlcokUIContainer(templateDoc);
            Border rootBorder = blockUI.Child as Border;
            
            return rootBorder;
        }    

        /// <summary>
        /// 添加Textblock到Grid中
        /// </summary>
        /// <param name="paraGrid"></param>
        /// <param name="textstr">内容</param>
        /// <param name="index">参数Index</param>
        /// <param name="isLabel">是否Label</param>
        private void AddTextToGrid(Grid paraGrid, string textstr, int index, bool isLabel)
        {
            int row = index / 2 + 1;
            int col = (index % 2) * 2;
            if (!isLabel)
                col++;

            if (index / 2 + 1 >= paraGrid.RowDefinitions.Count)
            {
                RowDefinition rowctrl = new RowDefinition();
                rowctrl.Height = new GridLength(1, GridUnitType.Auto);
                paraGrid.RowDefinitions.Add(rowctrl);
            }

            TextBlock txtctrl = new TextBlock();
            txtctrl.HorizontalAlignment = isLabel ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            txtctrl.Text = textstr;
            txtctrl.Margin = new Thickness(4);

            Grid.SetRow(txtctrl, row);
            Grid.SetColumn(txtctrl, col);
            paraGrid.Children.Add(txtctrl);
        }

        /// <summary>
        /// 创建检测结果Grid
        /// </summary>
        /// <param name="rowCount">总共多少行</param>
        /// <param name="titleTextBox">返回标题TextBlock</param>
        /// <param name="imageBorder">返回标题图像的Border</param>
        /// <returns></returns>
        private Grid GenerateOneGroupGrid(int rowCount, out TextBlock titleTextBox, out Border imageBorder)
        {
            Grid rootGrid = new Grid();
            //rootGrid.Margin = new System.Windows.Thickness(0, 10, 0, 10);

            //创建列, 0-6列是内容，7，8是结果的图标
            for (int i = 0; i < 9; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
                rootGrid.ColumnDefinitions.Add(col);
            }

            //0, 1, 3, 5, 7, 8是Auto，2, 4, 6列是1*
            int[] allwidth = new int[] { 2, 4, 6 };
            foreach (var i in allwidth)
                rootGrid.ColumnDefinitions[i].Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);

            //创建行
            for (int i = 0; i < rowCount; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
                rootGrid.RowDefinitions.Add(row);
            }

            //创建Border Title
            Border titleBoarder = new Border();
            titleBoarder.Background = headerBackground;
            titleBoarder.BorderThickness = new System.Windows.Thickness(0);
            Grid.SetRow(titleBoarder, 0);
            Grid.SetColumn(titleBoarder, 0);
            Grid.SetColumnSpan(titleBoarder, rootGrid.ColumnDefinitions.Count);
            rootGrid.Children.Add(titleBoarder);

            //创建Title TextBox
            titleTextBox = new TextBlock();
            titleTextBox.FontSize = headerFontSize;
            titleTextBox.Foreground = headerForeground;
            titleTextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            titleTextBox.Margin = new System.Windows.Thickness(0, 5, 0, 5);
            titleBoarder.Child = titleTextBox;

            //总结果显示图标(跨2列)
            imageBorder = new Border();
            imageBorder.Background = headerBackground;
            Grid.SetRow(imageBorder, 0);
            Grid.SetColumn(imageBorder, 7);
            Grid.SetColumnSpan(imageBorder, 2);
            rootGrid.Children.Add(imageBorder);

            return rootGrid;
        }

        /// <summary>
        /// 添加一个TextBlock到Grid中
        /// </summary>
        /// <param name="rootGrid">Grid</param>
        /// <param name="textstr">内容</param>
        /// <param name="row">Textblock在Grid中的行</param>
        /// <param name="col">Textblock在Grid中的列</param>
        /// <param name="isLabel">是否为标题Label</param>
        private void AddTextBlockToGrid(Grid rootGrid, string textstr, int row, int col, bool isLabel)
        {
            TextBlock txtctrl = new TextBlock();
            txtctrl.HorizontalAlignment = isLabel ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Left;
            txtctrl.Text = textstr;
            txtctrl.FontSize = textFontSize;
            txtctrl.Foreground = textForeground;
            txtctrl.Background = textBackground;
            txtctrl.Margin = new System.Windows.Thickness(4);   //Margin 4
            Grid.SetRow(txtctrl, row);
            Grid.SetColumn(txtctrl, col);
            rootGrid.Children.Add(txtctrl);
        }

        /// <summary>
        /// 添加一个测试结果到Grid中
        /// </summary>
        /// <param name="rootGrid">Grid</param>
        /// <param name="testingInfo">测试结果</param>
        /// <param name="rowIndex">结果在Grid中的行</param>
        /// <param name="showName">是否显示测试的名称</param>
        private void AddOneTestingToGrid(Grid rootGrid, BaseSelfTestInfo testingInfo, int rowIndex, bool showName)
        {
            if(showName)
                AddTextBlockToGrid(rootGrid, testingInfo.DisplayName(language), rowIndex, 0, false);

            AddTextBlockToGrid(rootGrid, "光谱区间:", rowIndex, 1, true);
            AddTextBlockToGrid(rootGrid, testingInfo.firstX.ToString() + " - " + testingInfo.lastX.ToString() + "cm-1", rowIndex, 2, false);

            AddTextBlockToGrid(rootGrid, "测量值:", rowIndex, 3, true);
            double tempresult = Math.Round(testingInfo.FinalResult * 1000) / 1000;
            AddTextBlockToGrid(rootGrid, tempresult.ToString()+testingInfo.ResultUnit, rowIndex, 4, false);

            AddTextBlockToGrid(rootGrid, "阈值:", rowIndex, 5, true);
            string thresoldstr = null;
            if (testingInfo.LessThresold == double.MaxValue)
                thresoldstr = "< " + (testingInfo.TargetResult + testingInfo.GreatThresold).ToString();
            else if (testingInfo.GreatThresold == double.MaxValue)
                thresoldstr = "> " + (testingInfo.TargetResult - testingInfo.LessThresold).ToString();
            else if (testingInfo.LessThresold == testingInfo.GreatThresold)
                thresoldstr = testingInfo.TargetResult.ToString() + "±" + testingInfo.GreatThresold.ToString();
            else
                thresoldstr = (testingInfo.TargetResult - testingInfo.LessThresold).ToString() + " - " + (testingInfo.TargetResult + testingInfo.GreatThresold).ToString();
            AddTextBlockToGrid(rootGrid, thresoldstr + testingInfo.ResultUnit, rowIndex, 6, false);

            var image = CreateResultImage(testingInfo.IsValidResult());
            Grid.SetRow(image, rowIndex);
            Grid.SetColumn(image, 7);
            rootGrid.Children.Add(image);
        }


        /// <summary>
        /// 添加一个测试结果到Grid中
        /// </summary>
        /// <param name="rootGrid">Grid</param>
        /// <param name="testingInfo">测试结果</param>
        /// <param name="rowIndex">结果在Grid中的行</param>
        /// <param name="showName">是否显示测试的名称</param>
        private int AddLineSlopTestingToGrid(Grid rootGrid, LineSlopeTestInfo testingInfo, int rowIndex, bool showName)
        {
            if (showName)
                AddTextBlockToGrid(rootGrid, testingInfo.DisplayName(language), rowIndex, 0, false);

            for (int i = 0; i < testingInfo.slopeX.Count; i++)
            {
                AddTextBlockToGrid(rootGrid, "光谱区间:", rowIndex, 1, true);
                AddTextBlockToGrid(rootGrid, testingInfo.slopeX[i].X.ToString() + " - " + testingInfo.slopeX[i].Y.ToString() + "cm-1", rowIndex, 2, false);

                AddTextBlockToGrid(rootGrid, "测量值:", rowIndex, 3, true);
                double tempmin = Math.Round(testingInfo.slopeResult[i].X  * 1000) / 1000;
                double tempmax = Math.Round(testingInfo.slopeResult[i].Y * 1000) / 1000;
                AddTextBlockToGrid(rootGrid, tempmin.ToString()+"-"+tempmax.ToString() + testingInfo.ResultUnit, rowIndex, 4, false);

                AddTextBlockToGrid(rootGrid, "阈值:", rowIndex, 5, true);
                AddTextBlockToGrid(rootGrid, testingInfo.slopeThresold[i].X.ToString() + " - " + testingInfo.slopeThresold[i].Y.ToString() + testingInfo.ResultUnit, rowIndex, 6, false);

                //测量值在阈值区间内
                var image = CreateResultImage(testingInfo.slopeResult[i].X > testingInfo.slopeThresold[i].X && testingInfo.slopeResult[i].Y < testingInfo.slopeThresold[i].Y);
                Grid.SetRow(image, rowIndex);
                Grid.SetColumn(image, 7);
                rootGrid.Children.Add(image);

                rowIndex++;
            }

            return rowIndex;
        }

        /// <summary>
        /// 显示一个测量组合
        /// </summary>
        /// <param name="group">测试组合</param>
        /// <param name="showName">是否显示测量名称</param>
        /// <returns></returns>
        private Grid GenerateOneGroupResult(SelTestGroup group, bool showName)
        {
            TextBlock titleText = null;
            Border imageBorder = null;

            //计算总共有多少行(LineSlopeTestInfo需要特殊处理)
            int rowIndex = rowIndex = group.testItems.Count * 2;
            var slope = group.testItems.FirstOrDefault(p => p is LineSlopeTestInfo) as LineSlopeTestInfo;
            if (slope != null)
                rowIndex += slope.slopeX.Count;
            var rootGrid = GenerateOneGroupGrid(rowIndex, out titleText, out imageBorder);

            //标题
            titleText.Text = group.DisplayName(language);

            //标题结果图标, 只要有一个没通过就算错误
            var titleImage = CreateResultImage(group.testItems.FirstOrDefault(p => p.IsValidResult() == false) == null ? true : false);
            titleImage.Width = titleImage.Height = 20;
            imageBorder.Child = titleImage;

            rowIndex = 1;   //跳过标题行
            for(int i=0; i<group.testItems.Count; i++)
            {
                if (group.testItems[i] is LineSlopeTestInfo)  //结果有多行, 特殊处理
                    rowIndex = AddLineSlopTestingToGrid(rootGrid, group.testItems[i] as LineSlopeTestInfo, rowIndex, showName);
                else
                    AddOneTestingToGrid(rootGrid, group.testItems[i], rowIndex, showName);
                rowIndex++;

                //添加测试之间的分隔线
                if (i < group.testItems.Count - 1)
                {
                    GridSplitter sp = new GridSplitter();
                    sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    sp.Height = 1;
                    sp.Background = System.Windows.Media.Brushes.Gray;
                    sp.Margin = new System.Windows.Thickness(5, 0, 5, 0);
                    Grid.SetColumnSpan(sp, rootGrid.ColumnDefinitions.Count);
                    Grid.SetRow(sp, rowIndex);
                    rootGrid.Children.Add(sp);

                    rowIndex++;
                }
            }

            return rootGrid;
        }

        /// <summary>
        /// 创建grid的外框
        /// </summary>
        /// <param name="contentGrid"></param>
        /// <returns></returns>
        private Border GenerateOuterBorder(Grid contentGrid)
        {
            Border border = new Border();
            border.BorderThickness = new System.Windows.Thickness(1);
            border.BorderBrush = System.Windows.Media.Brushes.Black;
            border.Child = contentGrid;
            border.Margin = new System.Windows.Thickness(0, 10, 0, 10);

            return border;
        }

        /// <summary>
        /// 创建测试的基本信息
        /// </summary>
        /// <returns></returns>
        private Grid GenerageReportTestBasenfo()
        {
            TextBlock titleText;
            Border imageBorder;
            Grid contentGrid = GenerateOneGroupGrid(3, out titleText, out imageBorder);

            titleText.Text = "测试信息";
            
            AddTextBlockToGrid(contentGrid, "测试单位", 1, 0, true);
            AddTextBlockToGrid(contentGrid, UnitName , 1, 1, true);

            AddTextBlockToGrid(contentGrid, "测试人员", 1, 2, true);
            AddTextBlockToGrid(contentGrid, UserName, 1, 3, true);

            AddTextBlockToGrid(contentGrid, "测试时间", 1, 4, true);
            AddTextBlockToGrid(contentGrid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1, 5, true);

            AddTextBlockToGrid(contentGrid, "仪器类型", 2, 0, true);
            AddTextBlockToGrid(contentGrid, scanner == null ? null : Common.Extenstion.EnumExtensions.GetEnumDescription(scanner.ConnectedDevice.Type, Language), 2, 1, true);

            AddTextBlockToGrid(contentGrid, "仪器型号", 2, 2, true);
            AddTextBlockToGrid(contentGrid, scanner == null ? null : Common.Extenstion.EnumExtensions.GetEnumDescription(scanner.ConnectedDevice.Model, Language), 2, 3, true);

            AddTextBlockToGrid(contentGrid, "序列号", 2, 4, true);
            AddTextBlockToGrid(contentGrid, scanner == null ? null : scanner.GetSerialNumber(), 2, 5, true);

            return contentGrid;
        }

        /// <summary>
        /// 设置最终测试结果信息
        /// </summary>
        /// <param name="groups">测量类别</param>
        /// <param name="rootBorder">根元素</param>
        private void SetFinalResult(List<SelTestGroup> groups, Border rootBorder)
        {
            bool successed = true;
            foreach(var group in groups)
            {
                foreach(var item in group.testItems)
                {
                    if (item.IsValidResult() == false)
                        successed = false;
                }
            }

            //结果
            var txtctrl = Ai.Hong.Controls.Common.XPSReportTemplate.GetElement<TextBlock>(rootBorder, "txtAllResult");
            if(successed)
            {
                txtctrl.Text ="测试结果 = 通过";
                txtctrl.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                txtctrl.Text ="测试结果 = 未通过";
                txtctrl.Foreground = System.Windows.Media.Brushes.Red;
            }

            //Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(detailBorder, "txtAllResult", "测试结果=" + (successed ? "通过" : "未通过"));

            //结果图标
            var imgBorder = Ai.Hong.Controls.Common.XPSReportTemplate.GetElement<Border>(rootBorder, "resultImageBorder");
            var image = CreateResultImage(successed);
            image.Width = image.Height = 20;
            imgBorder.Child = image;

            //日期
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "testingDate", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 创建总报告
        /// </summary>
        /// <param name="groups">测试组合的列表</param>
        /// <param name="reportTitle">报告标题</param>
        /// <returns></returns>
        public Border GenerateTotalPage(List<SelTestGroup> groups, string reportTitle)
        {
            Border rootBorder = Ai.Hong.Controls.Common.XPSReportTemplate.CloneObject(totalBorder);
            Grid rootGrid = rootBorder.Child as Grid;

            //报告名称
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "txtReportTitle", reportTitle);

            //报告内容
            for (int i = -1; i < groups.Count; i++ )
            {
                Grid contentGrid = null;
                if(i == -1)
                    contentGrid = GenerageReportTestBasenfo();    //测试的基本信息
                else
                    contentGrid = GenerateOneGroupResult(groups[i], true);  //具体测试信息

                var border = GenerateOuterBorder(contentGrid);
                Grid.SetRow(border, i + 2);    //row 0是report Title, 1=base info
                rootGrid.Children.Add(border);
            }

            SetFinalResult(groups, rootBorder);

            ////报告结尾
            //bool totalResult = true;
            //foreach(var group in groups)
            //{
            //    foreach (var item in group.testItems)
            //    {
            //        if (item.IsValidResult() == false)
            //            totalResult = false;
            //    }
            //}
            //Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "txtAllResult", "测试结果 = " + (totalResult ? "通过" : "未通过"));
            //Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "testingDate", DateTime.Now.ToString("yyyy-MM-dd"));

            return rootBorder;
        }

        /// <summary>
        /// 创建一页详细测试结果报告
        /// </summary>
        /// <param name="testingInfo">测试结果</param>
        /// <returns></returns>
        public Border GenerateDetailPage(BaseSelfTestInfo testingInfo)
        {
            Border rootBorder = Ai.Hong.Controls.Common.XPSReportTemplate.CloneObject(detailBorder);
            Grid rootGrid = rootBorder.Child as Grid;

            //测试名称
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "testingTitle", testingInfo.DisplayName(language));

            TextBlock titleText = null;
            Border imageBorder = null;

            //创建结果Grid
            int rowCount = 2;
            if (testingInfo is LineSlopeTestInfo)    //结果有多行, 特殊处理
                rowCount += (testingInfo as LineSlopeTestInfo).slopeX.Count;
            var resultGrid = GenerateOneGroupGrid(rowCount, out  titleText, out imageBorder);
            resultGrid.Margin = new System.Windows.Thickness(0, 10, 0, 10);

            titleText.Text = "测试结果";
            if (testingInfo is LineSlopeTestInfo)    //结果有多行, 特殊处理
                AddLineSlopTestingToGrid(resultGrid, testingInfo as LineSlopeTestInfo, 1, false);
            else
                AddOneTestingToGrid(resultGrid, testingInfo, 1, false);
            Grid.SetRow(resultGrid, 1);
            rootGrid.Children.Add(resultGrid);

            //光谱图
            var bmpfile = System.IO.Path.GetTempFileName();
            Ai.Hong.Charts.SpectrumChart chart = new Ai.Hong.Charts.SpectrumChart();

            string spctype = FileFormat.FileFormat.GetYAxisTypeName(testingInfo.SpectraDatas[0].dataInfo.dataType, false);
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "txtChartType", spctype);

            //显示光谱图
            foreach (var item in testingInfo.SpectraDatas)
                chart.AddChart(item.xDatas, item.yDatas, System.Windows.Media.Brushes.Black, Guid.NewGuid());
            chart.SaveToBitmapFile(bmpfile, 1000, 500, System.Windows.Media.Brushes.White);

            System.Windows.Controls.Image spcChartImage = new Image();
            Ai.Hong.Common.CommonMethod.SetImageSource(spcChartImage, bmpfile, null);
            //System.IO.File.Delete(bmpfile);
            Border border = rootBorder.FindName("borderGraphic") as Border;
            border.Child = spcChartImage;

            //光谱文件列表
            string files = null;
            foreach (var item in testingInfo.SpectraDatas)
                files += item.fileInfo.filename + "\r\n";
            //border = rootBorder.FindName("borderGraphicPath") as Border;
            for (int i = 0; i < 2; i++)
                files += files;
            Ai.Hong.Controls.Common.XPSReportTemplate.FillTextData(rootBorder, "filePaths", files);

            //测量参数
            Grid paraGrid = rootBorder.FindName("gridParameters") as Grid;
            var scanpara = testingInfo.GetScanParameter();
            var parasEnglish = new string[] { "Resolution", "Count", "BackGain", "ZeroFilling", "PhaseCorrect", "Apodization" };
            var parasChinese = new string[] { "分辨率", "扫描次数", "背景增益", "填零系数", "截趾函数", "相位校正方法", "相位分辨率" };
            var paraValues = new string[] { scanpara.Resolution.ToString(), scanpara.ScanCount.ToString(), scanpara.BackGain.ToString(), scanpara.ZeroFilling.ToString(), scanpara.Apodization.ToString(), scanpara.PhaseCorrect.ToString(), scanpara.PhaseResolution.ToString() };

            for (int index = 0; index < paraValues.Length; index++)
            {
                if(Language == EnumLanguage.Chinese)
                    AddTextToGrid(paraGrid, parasChinese[index] + ":", index, true);
                else
                    AddTextToGrid(paraGrid, parasEnglish[index] + ":", index, true);
                AddTextToGrid(paraGrid, paraValues[index], index, false);
            }

            return rootBorder;
        }

        /// <summary>
        /// 创建结果图标
        /// </summary>
        /// <param name="result">结果，NULL=Unknown</param>
        /// <returns></returns>
        private Ai.Hong.Controls.VectorImage CreateResultImage(bool? result)
        {
            Ai.Hong.Controls.VectorImage image = new Ai.Hong.Controls.VectorImage();
            string key = null;
            System.Windows.Media.SolidColorBrush brush;
            if (result == null)
            {
                key = "InfoWithCircelGeometry";
                brush = System.Windows.Media.Brushes.Goldenrod;
            }
            else if (result == true)
            {
                key = "SingleRightGeometry";
                brush = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                key = "SingleWrongGeometry";
                brush = System.Windows.Media.Brushes.IndianRed;
            }

            image.VectorSource = resourceDir[key] as System.Windows.Media.DrawingGroup;
            image.Height = 12;
            image.Width = 12;
            image.DrawColor = brush;
            image.Margin = new System.Windows.Thickness(0, 0, 4, 0);
            return image;
        }

        /// <summary>
        /// 创建并保存测试报告
        /// </summary>
        /// <param name="filename">XPS文件名</param>
        /// <param name="groups">测试组合的列表</param>
        /// <param name="reportTitle">报告的标题</param>
        /// <returns></returns>
        public bool CreateAndSaveXPSFile(string filename, List<SelTestGroup> groups, string reportTitle)
        {
            FixedDocument fixedDoc = new FixedDocument();

            //总报告
            Border border = GenerateTotalPage(groups, reportTitle);
            var page = Ai.Hong.Controls.Common.XPSReportTemplate.CreatePageContent(border);
            fixedDoc.Pages.Add(page);

            //详细报告
            foreach (var group in groups)
            {
                foreach (var item in group.testItems)
                {
                    border = GenerateDetailPage(item);
                    page = Ai.Hong.Controls.Common.XPSReportTemplate.CreatePageContent(border);
                    fixedDoc.Pages.Add(page);
                }
            }

            return Ai.Hong.Controls.Common.XPSReportTemplate.SaveToXpsFile(filename, fixedDoc);
        }
    }
}
