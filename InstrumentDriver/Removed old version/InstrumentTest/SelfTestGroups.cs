using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ai.Hong.Common;

namespace Ai.Hong.Driver.IT
{
    #region SelfTestGroup
    /// <summary>
    /// 测试组合
    /// </summary>
    public class SelTestGroup
    {
        /// <summary>
        /// 组合的内部名称
        /// </summary>
        [XmlAttribute]
        public string InnerName { get; set; }

        /// <summary>
        /// 组合的中文名称
        /// </summary>
        [XmlAttribute]
        public string ChineseName { get; set; }

        /// <summary>
        /// 组合的英文名称
        /// </summary>
        [XmlAttribute]
        public string EnglishName { get; set; }

        /// <summary>
        /// 测试的内容
        /// </summary>
        [XmlArray("TestItems")]
        [XmlArrayItem("testItem")]
        public List<BaseSelfTestInfo> TestItems { get; set; }

        /// <summary>
        /// 测试的显示名称
        /// </summary>
        /// <param name="language">当前所用语言</param>
        /// <returns></returns>
        public string DisplayName(EnumLanguage language)
        {
            return language == EnumLanguage.Chinese ? ChineseName : EnglishName;
        }
    }

    #endregion

    #region Test Group
    /// <summary>
    /// 性能测试参数信息,每个仪器都包含PQ/OQ测试参数，需要自己定义
    /// </summary>
    [XmlInclude(typeof(LaserWavelengthTestInfo))]
    [XmlInclude(typeof(LineNoiseTestInfo))]
    [XmlInclude(typeof(DeviationTestInfo))]
    [XmlInclude(typeof(InterferPeakTestInfo))]
    [XmlInclude(typeof(EnergyTestInfo))]
    [XmlInclude(typeof(VaporAccuracyTestInfo))]
    [XmlInclude(typeof(PolyAccuracyTestInfo))]
    [XmlInclude(typeof(PhotometricTestInfo))]
    [XmlInclude(typeof(ResolutionTestInfo))]
    [XmlInclude(typeof(LineSlopeTestInfo))]
    [XmlInclude(typeof(EnergyDistributeTestInfo))]
    [XmlInclude(typeof(TransmitReproductTestInfo))]
    [XmlInclude(typeof(WavenumberReproductTestInfo))]
    public class PerformanceTestGroupInfo
    {
        /// <summary>
        /// PQ 激光波数校准
        /// </summary>
        [XmlIgnore]
        public const string PQLaserCalibrate = "PQLaserCalibrate";
        /// <summary>
        /// PQ 能量测试
        /// </summary>
        [XmlIgnore]
        public const string PQEnergyTest = "PQEnergyTest";
        /// <summary>
        /// PQ 波数准确度测试
        /// </summary>
        [XmlIgnore]
        public const string PQWaveAccuracyTest = "PQWaveAccuracyTest";
        /// <summary>
        /// PQ 光路精度测试
        /// </summary>
        [XmlIgnore]
        public const string PQPhotometricTest = "PQPhotometricTest";
        /// <summary>
        /// OQ 激光波数校准
        /// </summary>
        [XmlIgnore]
        public const string OQLaserCalibrate = "OQLaserCalibrate";
        /// <summary>
        /// OQ 分辨率测试
        /// </summary>
        [XmlIgnore]
        public const string OQResolutionTest = "OQResolutionTest";
        /// <summary>
        /// OQ 光路精度测试
        /// </summary>
        [XmlIgnore]
        public const string OQPhotometricTest = "OQPhotometricTest";
        /// <summary>
        /// OQ 波数准确度测试
        /// </summary>
        [XmlIgnore]
        public const string OQWaveAccuracyTest = "OQWaveAccuracyTest";
        /// <summary>
        /// OQ 扫描参考光谱
        /// </summary>
        [XmlIgnore]
        public const string OQScanRefereceData = "OQScanRefereceData";

        /// <summary>
        /// 需要测试的时间间隔（小时）
        /// </summary>
        [XmlAttribute]
        public int TestDuration { get; set; }

        /// <summary>
        /// 上一次PQ测试时间
        /// </summary>
        [XmlAttribute]
        public DateTime LastTestTime { get; set; } = new DateTime(2018, 1, 1, 1, 1, 1);

        /// <summary>
        /// 测试组合
        /// </summary>
        [XmlElement]
        public List<SelTestGroup> TestGroups { get; set; } = new List<SelTestGroup>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public PerformanceTestGroupInfo()
        {

        }

        /// <summary>
        /// 获取所有的测试项目
        /// </summary>
        /// <returns></returns>
        public List<BaseSelfTestInfo> GetAllTestItems()
        {
            List<BaseSelfTestInfo> retDatas = new List<BaseSelfTestInfo>();

            //不返回没有选中的测试项目
            foreach (var item in TestGroups)
                retDatas.AddRange(item.TestItems.Where(p=>p.IsSelected));
            return retDatas;
        }

        /// <summary>
        /// 根据类型获取检测项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetTestItem<T>() where T : class
        {
            var allItems = GetAllTestItems();
            foreach (var item in allItems)
            {
                if (item is T)
                    return item as T;
            }

            return null;
        }
    }
    #endregion

    #region OQ test group
    /// <summary>
    /// OQ测试参数信息
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
    public class OQTestInfoaaa
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
        public OQTestInfoaaa()
        {

        }

        /// <summary>
        /// 创建新的PQ测试参数
        /// </summary>
        /// <param name="createNew">仪器型号</param>
        public OQTestInfoaaa(bool createNew)
        {
            laserTest = new SelTestGroup()
            {
                InnerName = "laserCalibrate",
                ChineseName = "激光校准",
                EnglishName = "Laser Calibrate",
                TestItems = new List<BaseSelfTestInfo>() { new LaserWavelengthTestInfo(createNew) }
            };

            resolutionTest = new SelTestGroup()
            {
                InnerName = "resolutionTest",
                ChineseName = "分辨率测试",
                EnglishName = "Resolution Testing",
                TestItems = new List<BaseSelfTestInfo>() { new ResolutionTestInfo(true) }
            };

            photometricTest = new SelTestGroup()
            {
                InnerName = "photometricTest",
                ChineseName = "光路稳定性测试",
                EnglishName = "Photometric Accuracy Testing",
                TestItems = new List<BaseSelfTestInfo>() { new LineNoiseTestInfo(createNew), new LineSlopeTestInfo(true), new EnergyDistributeTestInfo(true), new TransmitReproductTestInfo(true) }
            };

            wavelengthTest = new SelTestGroup()
            {
                InnerName = "waveAccuracyTest",
                ChineseName = "波数精度测试",
                EnglishName = "Wavelength Accuracy Testing",
                TestItems = new List<BaseSelfTestInfo>() { new VaporAccuracyTestInfo(true), new PolyAccuracyTestInfo(true), new WavenumberReproductTestInfo(true) }
            };

            scanReferenceData = new SelTestGroup()
            {
                InnerName = "scanRefereceData",
                ChineseName = "采集和保存仪器参考谱图",
                EnglishName = "Scan & Save Reference Spectra",
                TestItems = new List<BaseSelfTestInfo>() { new InterferPeakTestInfo(true), new EnergyTestInfo(true), new PhotometricTestInfo(true) }
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
                item.TestItems.RemoveAll(p => p.IsSelected == false);
            retDatas.RemoveAll(p => p.TestItems.Count == 0);

            return retDatas;
        }

        /// <summary>
        /// 根据类型获取检测项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetTestItem<T>() where T:class
        {
            var allItems = GetAllTestGroups();
            foreach(var item in allItems)
            {
                if (item is T)
                    return item as T;
            }

            return null;
        }
    }
    #endregion
}
