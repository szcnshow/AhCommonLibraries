using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VspecNIRTypeLib;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;

namespace VspecInstrument
{
    public class VspecInstrument : FTNirInterface.InstrumentInterface
    {
       
        static Types.InstrumentTypeBase baseObj = null;

        //获取错误信息
        public override string GetError()
        {
            return Types.InstrumentTypeBase.GetError();
        }

       
        /// <summary>
        /// 连接光谱仪
        /// </summary>
        public override bool Connect()
        {
            bool result = Types.InstrumentTypeBase.Connect();
            baseObj = Types.InstrumentTypeBase.baseObj;
            return result;
        }

        /// <summary>
        /// 断开仪器连接
        /// </summary>
        public override bool Disconnect()
        {
            return Types.InstrumentTypeBase.Disconnect();
        }


        /// <summary>
        /// 把激光波数写入仪器
        /// </summary>
        /// <param name="curPeak">当前峰位</param>
        /// <param name="targetPeak">目标峰位</param>
        /// <returns></returns>
        public override bool? SetLaserWavelength(double curPeak, double targetPeak, ref double curLaser)
        {
            return baseObj.SetLaserWavelength(curPeak, targetPeak,ref curLaser);
        }
        /// <summary>
        /// 获得仪器参数 
        /// </summary>
        /// <returns></returns>
        public override string GetParametersTable()
        {
            return Types.InstrumentTypeBase.GetParametersTable();
        }

        /// <summary>
        /// 灵敏度测试  
        /// </summary>
        /// <returns></returns>
        public override string ReadSensors()
        {
            return baseObj.ReadSensors();
        }

        /// <summary>
        /// 获取仪器序列号
        /// </summary>
        /// <returns></returns>
        public override InstrumentInfo GetInstrumentInfo()
        {
            return baseObj.GetInstrumentInfo();
        }

        /// <summary>
        /// 移动转轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <returns></returns>
        public override bool? MoveWheel(int position,string iniFilePath)
        {
            return baseObj.MoveWheel(position, iniFilePath);
        }

        /// <summary>
        /// 波数精度-聚苯乙烯-温度校正
        /// </summary>
        /// <param name="targetPeak"></param>
        /// <returns></returns>
        public override double? TemperatureCalibrate(double targetPeak)
        {
            return baseObj.TemperatureCalibrate(targetPeak);
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <param name="iniPath">配置文件路径</param>
        public override Dictionary<string, string> ReadScanPara(string iniPath)
        {
            return baseObj.ReadScanPara(iniPath);
        }

        

        /// <summary>
        /// 移动积分球位置 0 for sample.1 for background
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override bool? MoveFlag(int position)
        {
            return baseObj.MoveFlag(position);
        }

        /// <summary>
        /// 移动积分球位置 0 for off.1 for on
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override bool? SampleSpinner(int position)
        {
            return baseObj.SampleSpinner(position);
        }

        
        public override bool?  IsTransmissionCellEmpty()
        {
            return baseObj.IsTransmissionCellEmpty();
        }

        /// <summary>
        /// 扫描背景
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">背景保存文件</param>
        public override string ScanBackground(string scanMethodFile, int scanCount, string backgroundFile, string addPara = null)
        {
            return baseObj.ScanBackground(scanMethodFile, scanCount, backgroundFile);
        }

        /// <summary>
        /// 扫描样品
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">样品保存文件</param>
        public override string ScanSample(string scanMethodFile, int scanCount, string sampleFile, string addPara = null)
        {
            return baseObj.ScanSample(scanMethodFile, scanCount, sampleFile);
        }


        /// <summary>
        /// 计算吸收谱
        /// </summary>
        /// <param name="backFile">背景光谱</param>
        /// <param name="sampleFile">样品光谱</param>
        /// <returns></returns>
        public override string CalculateAbs(string backFile, string sampleFile)
        {
            return baseObj.CalculateAbs(backFile, sampleFile);
        }

        /// <summary>
        /// 计算透射谱
        /// </summary>
        /// <param name="backFile">背景光谱</param>
        /// <param name="sampleFile">样品光谱</param>
        /// <returns></returns>
        public override string CalculateTrans(string backFile, string sampleFile)
        {
            return baseObj.CalculateTrans(backFile, sampleFile);
        }

        /// <summary>
        /// 获取仪器激光波长
        /// </summary>
        /// <returns></returns>
        public override double? GetLaserWavelength()
        {
            return base.GetLaserWavelength();
        }

        /// <summary>
        /// 修改配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scPara"></param>
        /// <param name="iniFilePath"></param>
        /// <returns></returns>
        public override bool? ModifyIniFile<T>(T scPara, string iniFilePath)
        {
            return baseObj.ModifyIniFile(scPara, iniFilePath);
        }
    }

    /// <summary>
    /// JsonString 操作
    /// </summary>
    public class JsonString
    {
        public static string ErrorString;

        /// <summary>
        /// 序列化对象的到Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJsonString(object obj)
        {
            try
            {
                return new JavaScriptSerializer().Serialize(obj);
            }
            catch (Exception ex)
            {
                ErrorString = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// 反序列化得到json字符串对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToObj<T>(string json) where T : class
        {
            try
            {
                T obj = Activator.CreateInstance<T>();
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer serializer;
                    serializer = new DataContractJsonSerializer(obj.GetType());
                    return (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                ErrorString = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// 反序列化Json到List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonStr"></param>
        /// <returns></returns>
        public static List<T> JsonToList<T>(string JsonStr)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<T> objs;
            try
            {
                objs = Serializer.Deserialize<List<T>>(JsonStr);
                return objs;
            }
            catch (Exception ex)
            {
                ErrorString = ex.ToString();
                return null;
            }
        }
        /// <summary>
        /// 仪器类型、序列号等
        /// </summary>
        public class ParametersTable
        {
            /// <summary>
            /// 仪器类型 1：光纤 2：积分球  3：积分球+透射 4：光纤+积分球+透射
            /// </summary>
            public int systemType { get; set; }

            /// <summary>
            /// 仪器型号
            /// </summary>
            public string serialNum { get; set; }

            /// <summary>
            /// 防火墙版本
            /// </summary>
            public int firmwareVer { get; set; }

            /// <summary>
            /// 激光波数(635 ~ 645)
            /// </summary>
            public double laserWavelen { get; set; }

            /// <summary>
            /// 扫描速度
            /// </summary>
            public int[] velocities { get; set; }

            /// <summary>
            /// 分辨率
            /// </summary>
            public int[] resolutions { get; set; }

            public int retVal { get; set; }
        }

        /// <summary>
        /// 仪器温度等
        /// </summary>
        public class Sensors
        {
            public int id { get; set; }

            public double val { get; set; }
        }

        /// <summary>
        /// ReadSensos To Obj
        /// </summary>
        public class GetSensors
        {
            public List<Sensors> sensors { get; set; }

            public int retVal { get; set; }

            public GetSensors()
            {
                sensors = new List<Sensors>();
            }
        }
    }
}
