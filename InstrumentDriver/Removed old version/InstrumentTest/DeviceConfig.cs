using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ai.Hong.Driver;
using Ai.Hong.Driver.IT;
using Ai.Hong.Common;
using System.IO;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 应用程序运行配置文件
    /// </summary>
    public class DeviceConfig
    {
        /// <summary>
        /// 系统语言
        /// </summary>
        [XmlAttribute]
        public EnumLanguage Language { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [XmlAttribute]
        public string UserName { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [XmlAttribute]
        public string UnitName { get; set; }

        /// <summary>
        /// 当前采集参数文件名
        /// </summary>
        [XmlElement]
        public string MeasureConfigFile { get; set; }

        /// <summary>
        /// 当前采集参数属性
        /// </summary>
        [XmlIgnore]
        public ScanParameter MeasureParameter { get; set; }

        /// <summary>
        /// PQ测试参数
        /// </summary>
        [XmlIgnore]
        public PerformanceTestGroupInfo PQTestParameter { get; set; }

        /// <summary>
        /// OQ测试参数
        /// </summary>
        [XmlIgnore]
        public PerformanceTestGroupInfo OQTestParameter { get; set; }

        /// <summary>
        /// 上一次连接的设备信息
        /// </summary>
        [XmlElement]
        public DeviceInfo LastDeviceInfo { get; set; }

        /// <summary>
        /// 当前连接的仪器
        /// </summary>
        [XmlIgnore]
        public FTDriver CurInstrument { get; set; }

        /// <summary>
        /// 当前硬件
        /// </summary>
        [XmlIgnore]
        public DeviceHardware CurHardware { get; set; }

        /// <summary>
        /// PQ config filename
        /// </summary>
        [XmlIgnore]
        public const string PQConfigFilename = "PQTestConfig.config";

        /// <summary>
        /// OQ config filename
        /// </summary>
        [XmlIgnore]
        public const string OQConfigFilename = "OQTestConfig.config";

        /// <summary>
        /// measurement filename
        /// </summary>
        [XmlIgnore]
        private const string defaultMeasureName = "MesureConfig.config";

        /// <summary>
        /// 配置文件路径（反序列化后立即设置）
        /// </summary>
        [XmlIgnore]
        public string ConfigFilename = null;

        /// <summary>
        /// 配置文件路径（反序列化后立即设置）
        /// </summary>
        [XmlIgnore]
        public string ConfigFilepath { get { return ConfigFilename==null? null: System.IO.Path.GetDirectoryName(ConfigFilename); } }

        /// <summary>
        /// 构造函数（用于反序列化）
        /// </summary>
        public DeviceConfig()
        {

        }

        /// <summary>
        /// 创建配置参数
        /// </summary>
        /// <param name="scanner">当前连接的仪器</param>
        /// <param name="configFile">配置参数文件名</param>
        /// <param name="userName">操作员名称</param>
        /// <param name="unitName">操作单位名称</param>
        /// <param name="language">使用的语言</param>
        public DeviceConfig(FTDriver scanner, string configFile, string userName="Unknown", string unitName="Unknown", EnumLanguage language = EnumLanguage.Chinese)
        {
            try
            {
                this.CurInstrument = scanner;
                this.Language = language;
                this.UserName = userName;
                this.UnitName = unitName;

                if (string.IsNullOrWhiteSpace(configFile))
                {
                    var tmppath = System.Reflection.Assembly.GetEntryAssembly().Location;
                    ConfigFilename = Path.Combine(Path.GetDirectoryName(tmppath), "Config", "Application.config");
                }
                else
                    ConfigFilename = configFile;

                if (!Directory.Exists(ConfigFilepath))
                    Directory.CreateDirectory(ConfigFilepath);

                if(Init(scanner) == true)
                    CommonMethod.SerializeToFile(this, configFile);

                //保存
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 初始化AppConfigInfo
        /// </summary>
        /// <param name="scanner">当前连接的仪器</param>
        /// <returns>True=需要保存配置文件</returns>
        public bool Init(FTDriver scanner)
        {
            try
            {
                
                bool needsave = false;

                CurHardware = scanner.deviceHardware;
                if (CurHardware == null)
                    throw new Exception("Invalid scanner hardware");

                if (MeasureConfigFile == null)
                    MeasureConfigFile = System.IO.Path.Combine(ConfigFilepath, defaultMeasureName);

                //初始化采集参数
                MeasureParameter = CommonMethod.DeserializeFromFile<ScanParameter>(MeasureConfigFile);
                if (MeasureParameter == null)   //需要创建新的采集参数
                {
                    MeasureParameter = new ScanParameter(CurHardware, Language);
                    MeasureParameter.Filename = MeasureConfigFile;
                    CommonMethod.SerializeToFile(MeasureParameter, MeasureParameter.Filename);

                    needsave = true;
                }
                else
                {
                    MeasureParameter.Filename = MeasureConfigFile;
                }

                //初始化与硬件相关的扫描参数属性
                CurHardware.InitScanParameter(MeasureParameter);

                //初始化PQ,OQ参数(参数文件肯定在Config目录中)
                PQTestParameter = CommonMethod.DeserializeFromFile<PerformanceTestGroupInfo>(System.IO.Path.Combine(ConfigFilepath, PQConfigFilename));
                if (PQTestParameter == null)
                {
                    PQTestParameter = CreatePQTest();
                    CommonMethod.SerializeToFile(PQTestParameter, System.IO.Path.Combine(ConfigFilepath, PQConfigFilename));

                    needsave = true;
                }

                //初始化PQ,OQ参数(参数文件肯定在Config目录中)
                OQTestParameter = CommonMethod.DeserializeFromFile<PerformanceTestGroupInfo>(System.IO.Path.Combine(ConfigFilepath, OQConfigFilename));
                if (OQTestParameter == null)
                {
                    OQTestParameter = CreateOQTest();
                    CommonMethod.SerializeToFile(OQTestParameter, System.IO.Path.Combine(ConfigFilepath, OQConfigFilename));

                    needsave = true;
                }

                return needsave;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 创建PQ测试项目（每种仪器自己创建）
        /// </summary>
        /// <returns></returns>
        protected virtual PerformanceTestGroupInfo CreatePQTest()
        {
            PerformanceTestGroupInfo info = new PerformanceTestGroupInfo();
            return info;
        }

        /// <summary>
        /// 创建OQ测试项目（每种仪器自己创建）
        /// </summary>
        /// <returns></returns>
        protected virtual PerformanceTestGroupInfo CreateOQTest()
        {
            PerformanceTestGroupInfo info = new PerformanceTestGroupInfo();
            return info;
        }
    }


}
