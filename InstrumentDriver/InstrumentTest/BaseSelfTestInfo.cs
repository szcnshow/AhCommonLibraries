using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Ai.Hong.Common;
using Ai.Hong.FileFormat;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 仪器自检基本类
    /// </summary>
    public class BaseSelfTestInfo : INotifyPropertyChanged
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private bool _isSelected = true;
        /// <summary>
        /// 是否选中
        /// </summary>
        [XmlAttribute]
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; DoPropertyChanged("IsSelected"); } }

        private double _firstX = 4000;
        /// <summary>
        /// 计算的X起始值
        /// </summary>
        [XmlAttribute]
        public double firstX { get { return _firstX; } set { _firstX = value; DoPropertyChanged("firstX"); } }

        private double _lastX = 10000;
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

        private double _lessThresold = 0.1;
        /// <summary>
        /// 小于目标值的范围
        /// </summary>
        [XmlAttribute]
        public double LessThresold { get { return _lessThresold; } set { _lessThresold = value; DoPropertyChanged("LessThresold"); } }

        private double _greatThresold = 0.1;
        /// <summary>
        /// 大于目标值的范围
        /// </summary>
        [XmlAttribute]
        public double GreatThresold { get { return _greatThresold; } set { _greatThresold = value; DoPropertyChanged("GreatThresold"); } }

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

        /// <summary>
        /// 采集参数
        /// </summary>
        [XmlElement]
        public ScanParameter AcquireParameter { get; set; }

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
        /// <param name="resultUnit">测量结果单位</param>
        public BaseSelfTestInfo(string chineseName, string englishName, string resultUnit)
        {
            this._chineseName = chineseName;
            this._englishName = englishName;
            this.ResultUnit = resultUnit;
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

            for (int i = 0; i < targetPeaks.Length; i++)
                retPeaks[i] = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, targetPeaks[i], 4, out double newy, upPeak);

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
                    minY = yDatas[index];
                    minX = xDatas[index];
                }
            }

            //标注最高最低峰位并记录
            maxX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, maxX, 4, out maxY, true);
            minX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, minX, 4, out minY, false);
        }
    }
}
