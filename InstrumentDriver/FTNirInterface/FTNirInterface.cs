using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTNirInterface
{
    /// <summary>
    /// 仪器状态，联机，初始化，空闲，正忙，错误
    /// </summary>
    public enum enumDeviceStatus { Connect,Init, Idle, Busy, Error };

    /// <summary>
    /// 相位校正枚举
    /// </summary>
    public enum enumPhaseCorrection { mertz = 0, magnitude = 1, none=2 };

    /// <summary>
    /// 截趾函数枚举
    /// </summary>
    public enum enumApodization {Triangle, Boxcar, Beer_BortonMed,Beer_BortonWeak, Beer_BortonStrong, Happ_Genzel, Bessel, Cosine, Blackman_Harris3Term,  Blackman_Harris4Term, Cosine3};

    public enum enumInstrumentType { Default, MPA, Tango, Matrix, Matrix_E, Matrix_F, Matrix_I, Fiber, IntegratingSphere, QuasIR }

    public enum enumInstrumentFactor { Bruker, Vspec, Thermo }

    /// <summary>
    /// 样品采集通道
    /// </summary>
    public enum enumAcquireChannel
    {
        /// <summary>
        /// 积分球
        /// </summary>
        Sphere = 0,
        /// <summary>
        /// 液体池
        /// </summary>
        LiquidCell = 1,
        /// <summary>
        /// 光纤
        /// </summary>
        Fibber = 2,
        /// <summary>
        /// 气体池
        /// </summary>
        GasCell=3
    }

    /// <summary>
    /// 仪器接口
    /// </summary>
    public  class InstrumentInterface
    {
        /// <summary>
        /// 仪器信息类
        /// </summary>
        public class InstrumentInfo
        {
            /// <summary>
            /// 仪器厂商
            /// </summary>
            public enumInstrumentFactor instrumentFactor { get; set; }

            /// <summary>
            /// 仪器型号
            /// </summary>
            public enumInstrumentType instrumentType { get; set; }

            /// <summary>
            /// 仪器序列号
            /// </summary>
            public string serialNumber { get; set; }
        }
        /// <summary>
        /// 仪器连接
        /// </summary>
        public virtual bool Connect() {return false; }//{return false;}


        /// <summary>
        /// 断开仪器连接
        /// </summary>
        public virtual bool Disconnect() { return false; }// { return false; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public virtual string GetError() { return null; }

        /// <summary>
        /// 写入激光波数到仪器
        /// </summary>
        /// <param name="laserWavelength">激光波数</param>
        /// <param name="targetPeak">目标峰位</param>
        /// <param name="curLaser">当前激光峰位</param>
        /// <returns></returns>
        public virtual bool? SetLaserWavelength(double laserWavelength, double targetPeak, ref double curLaser) {  return null; }

        /// <summary>
        /// 获得仪器参数 格式:{"systemType":1,"serialNum":"VS1003","firmwareVer":3,"laserWavelen":"637.947265","velocities":[15],"resolutions":[32,16,8,4],"retVal":0}
        /// </summary>
        /// <returns></returns>
        public virtual string GetParametersTable() { return null; }

        /// <summary>
        /// 灵敏度测试  格式：{"sensors":[{"id":1,"val":20.00000},{"id":2,"val":22.00000},{"id":3,"val":55.00000},{"id":4,"val":15.10000},{"id":5,"val":-40.00000}],"retVal":0}，ID2是温度
        /// </summary>
        /// <returns></returns>
        public virtual string ReadSensors() { return null; }

        /// <summary>
        /// 波数精度-聚苯乙烯-温度校正
        /// </summary>
        /// <param name="targetPeak"></param>
        /// <returns></returns>
        public virtual double? TemperatureCalibrate(double targetPeak) { return null; }

        /// <summary>
        /// 移动转轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public virtual bool? MoveWheel(int position, string path = "") { return true; }

        /// <summary>
        /// 是否是积分球类型仪器
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMoveFlagBack() { return true; }

        /// <summary>
        /// 移动积分球位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual bool? MoveFlag(int position) { return true; }

        /// <summary>
        /// 积分球仪器 样品转轮转动
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual bool? SampleSpinner(int position) { return true; }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <param name="iniPath">配置文件路径</param>
        public virtual Dictionary<string,string> ReadScanPara(string iniPath) { return null; }

        /// <summary>
        /// 扫描背景
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">背景保存文件</param>
        /// <param name="addPara">附加参数</param>
        /// <return>返回背景文件名</return>
        public virtual string ScanBackground(string scanMethodFile, int scanCount, string backgroundFile, string addPara = null) { return null; }

        /// <summary>
        /// 扫描样品
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="sampleFile">样品保存文件</param>
        /// <param name="addPara">附加参数</param>
        /// <return>返回样品文件名</return>
        public virtual string ScanSample(string scanMethodFile, int scanCount, string sampleFile, string addPara = null) { return null; }

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

        /// <summary>
        /// 获取仪器序列号
        /// </summary>
        /// <returns></returns>
        public virtual InstrumentInfo GetInstrumentInfo() { return new InstrumentInfo(); }

        /// <summary>
        /// 扫描参考光谱
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="referenceFile">参考光谱保存文件</param>
        /// <returns></returns>
        public virtual string ScanReference(string scanMethodFile, int scanCount, string referenceFile) { return null; }

        /// <summary>
        /// 设置扫描参数
        /// </summary>
        /// <param name="resolution">分辨率</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="apodization">截趾函数</param>
        /// <param name="Gain">增益</param>
        /// <param name="phaseCorrection">相位校正</param>
        /// <param name="zeroFilling">填零因子</param>
        /// <returns></returns>
        public virtual bool SetScanParameter(double resolution, double firstX, double lastX, int scanCount, int Gain=1, int zeroFilling=0, enumPhaseCorrection phaseCorrection= enumPhaseCorrection.magnitude, enumApodization apodization= enumApodization.Blackman_Harris3Term ) { return false; }

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
        /// 透射池是否空
        /// </summary>
        /// <returns></returns>
        public virtual bool? IsTransmissionCellEmpty() { return null; }
    }


}
