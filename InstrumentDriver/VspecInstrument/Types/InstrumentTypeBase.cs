using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VspecNIRTypeLib;
using FTNirInterface;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;

namespace VspecInstrument.Types
{
    public class InstrumentTypeBase
    {
        [DllImport("kernel32")]
        private static extern int WritePrivateProfileString(string section, string key, string writeVal, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 仪器操作对象
        /// </summary>
        public static InstrumentTypeBase baseObj = null;

        /// <summary>
        /// 仪器接口
        /// </summary>
        public static VspecNIRTypeLib.VspecNIRObject instrumentObject = null;

        /// <summary>
        /// 错误代码
        /// </summary>
        public static int errorCode;

        /// <summary>
        /// 仪器是否联机
        /// </summary>
        public static bool isConnected;

        /// <summary>
        /// 仪器信息
        /// </summary>
        public static FTNirInterface.InstrumentInterface.InstrumentInfo info;

        #region Static Method

        //获取错误信息
        public static string GetError()
        {
            switch (errorCode)
            {
                case -2:
                    return "没有找到加密卡";
                case -3:
                    return "加载扫描配置文件错误";
                case -4:
                    return "COM接口错误";
                case -5:
                    return "仪器扫描光谱出现错误";
                case -6:
                    return "光谱文件保存错误";
                case -10:
                    return "创建接口对象错误";
                case -11:
                    return "没有连接仪器";
                case -12:
                    return "没有生成光谱,请检查扫描配置文件";
                case -13:
                    return "重命名光谱文件错误";
                case -14:
                    return "LOCK FAULT";
                case -15:
                    return "激光波数超出范围";
                case -16:
                    return "光谱文件不存在";
                case -17:
                    return "读取光谱出错";
                case -18:
                    return "光谱分辨率不一致";
                case -19:
                    return "There is not the COM Object of this Instument type, Please contact the Developer !";
                case -20:
                    return "Serialize Json to Obj Error!";
                default:
                    return "未知错误 from scanner";
            }
        }

        /// <summary>
        /// 返回当前执行的错误
        /// </summary>
        /// <returns></returns>
        //public static string GetError() { return ErrorString; }

        /// <summary>
        /// 连接仪器
        /// </summary>
        /// <returns></returns>
        public static bool Connect()
        {
            if (instrumentObject == null)
                instrumentObject = new VspecNIRObject();

            if (instrumentObject == null)
            {
                errorCode = -10;
                return false;
            }
            errorCode = instrumentObject.Connect();
            isConnected = (errorCode == 0);
            //判断仪器是否是积分球类型
            string jsonString = "";
            errorCode = instrumentObject.GetParametersTable(ref jsonString);
            if (errorCode == 0 && jsonString != "")
            {
                JsonString.ParametersTable par = JsonString.JsonToObj<JsonString.ParametersTable>(jsonString);
                InitInstrumentObject(par.systemType);
            }
            return isConnected;
        }

        /// <summary>
        /// 断开仪器连接
        /// </summary>
        /// <returns></returns>
        public static bool Disconnect()
        {

            if (instrumentObject == null)
                return true;

            if (!isConnected)
                return true;

            errorCode = instrumentObject.Disconnect();
            isConnected = false;
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.Start();//启动CMD
            p.StandardInput.WriteLine("taskkill /im eftirPLS.exe /f");
            p.StandardInput.WriteLine("taskkill /im vspecNIR.exe /f");
            System.Threading.Thread.Sleep(100);
            //p.StandardInput.AutoFlush = true;
            // p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            instrumentObject = null;
            return errorCode == 0;

        }

        /// <summary>
        /// 初始化仪器操作接口
        /// </summary>
        /// <param name="ins">1,光纤,2:积分球,6:积分球+透射</param>
        private static bool InitInstrumentObject(int type)
        {
            info = new InstrumentInterface.InstrumentInfo();
            info.instrumentFactor = FTNirInterface.enumInstrumentFactor.Vspec;
             //判断仪器是否是积分球类型
            string jsonString = GetParametersTable();
            if (jsonString != null)
            {
                JsonString.ParametersTable par = JsonString.JsonToObj<JsonString.ParametersTable>(jsonString);
                info.serialNumber = par.serialNum;
            }
            switch (type)
            {
                case 1:
                    baseObj = new Types.Fiber();
                    info.instrumentType = FTNirInterface.enumInstrumentType.Fiber;
                    return true;
                case 2:
                    baseObj = new Types.IntegratingSphere(); 
                    info.instrumentType = FTNirInterface.enumInstrumentType.IntegratingSphere;
                    return true;
                case 6:
                    baseObj = new Types.IntegratingSphereTrans(); 
                    info.instrumentType = FTNirInterface.enumInstrumentType.QuasIR;
                    return true;
                default:
                    errorCode = -19; return false;
            }
        }

        /// <summary>
        /// 获得仪器参数 格式:{"systemType":1,"serialNum":"VS1003","firmwareVer":3,"laserWavelen":"637.947265","velocities":[15],"resolutions":[32,16,8,4],"retVal":0}
        /// </summary>
        /// <returns></returns>
        public static  string GetParametersTable()
        {

            if (!isConnected)
            {
                errorCode = -11;
                return null;
            }
            string para = "";
            errorCode = instrumentObject.GetParametersTable(ref para);
            return errorCode == 0 ? para : null;

        }

        /// <summary>
        /// 灵敏度测试  格式：{"sensors":[{"id":1,"val":20.00000},{"id":2,"val":22.00000},{"id":3,"val":55.00000},{"id":4,"val":15.10000},{"id":5,"val":-40.00000}],"retVal":0}，ID2是温度
        /// </summary>
        /// <returns></returns>
        public  string ReadSensors()
        {

            if (!isConnected)
            {
                errorCode = -1;
                return null;
            }
            string para = "";
            errorCode = instrumentObject.ReadSensors(ref para);
            return errorCode == 0 ? para : null;

        }

        /// <summary>
        /// 把激光波数写入仪器
        /// </summary>
        /// <param name="curPeak">当前峰位</param>
        /// <param name="targetPeak">目标峰位</param>
        /// <returns></returns>
        public bool? SetLaserWavelength(double curPeak, double targetPeak, ref double curLaser)
        {
            if (!isConnected)
            {
                errorCode = -11;
                return false;
            }
            string paraString = GetParametersTable();
            if (paraString == null)
            {
                return false;
            }
            JsonString.ParametersTable senser = JsonString.JsonToObj<JsonString.ParametersTable>(paraString);
            string tempString = string.Empty;
            try
            {
                tempString = (senser.laserWavelen * curPeak / targetPeak).ToString();
            }
            catch
            {
                return false;
            }
            double tempValue = Convert.ToDouble(tempString.Substring(0, tempString.Length < 17 ? tempString.Length : 17));
            if (tempValue < 645 && tempValue > 635)
                errorCode = instrumentObject.SetLaserWavelength(tempValue.ToString());
            else
                errorCode = -15;
            return errorCode == 0;
        }

        /// <summary>
        /// 获取仪器序列号
        /// </summary>
        /// <returns></returns>
        public  FTNirInterface.InstrumentInterface.InstrumentInfo GetInstrumentInfo()
        {
            return info;
            //FTNirInterface.InstrumentInterface.InstrumentInfo info = new FTNirInterface.InstrumentInterface.InstrumentInfo();
            //if (!isConnected)
            //{
            //    errorCode = -11;
            //    return info;
            //}
            ////判断仪器是否是积分球类型
            //string jsonString = GetParametersTable();

            //if (jsonString != null)
            //{
            //    JsonString.ParametersTable par = JsonString.JsonToObj<JsonString.ParametersTable>(jsonString);
            //    info.serialNumber = par.serialNum;
            //}
            //return info;

        }

        /// <summary>
        /// 移动转轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <returns></returns>
        public bool? MoveWheel(int position, string iniFilePath)
        {

            if (!isConnected)
            {
                errorCode = -11;
                return false;
            }
            errorCode = instrumentObject.MoveWheel(position);
            return errorCode == 0;

        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <param name="iniPath">配置文件路径</param>
        public  Dictionary<string, string> ReadScanPara(string iniPath)
        {
            if (!System.IO.File.Exists(iniPath))
            {
                return null;
            }
            try
            {
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("resolution", Ai.Hong.CommonMethod.ReadIniFile(iniPath, "Collection", "resolution"));
                para.Add("scans", Ai.Hong.CommonMethod.ReadIniFile(iniPath, "Collection", "sampleScans"));
                para.Add("gain", Ai.Hong.CommonMethod.ReadIniFile(iniPath, "Collection", "gain"));
                para.Add("zeroFill", Ai.Hong.CommonMethod.ReadIniFile(iniPath, "Process", "zeroFill"));
                para.Add("phaseCorrection", Ai.Hong.CommonMethod.ReadIniFile(iniPath, "Process", "phaseCorrection"));
                para.Add("apodization", Ai.Hong.CommonMethod.ReadIniFile(iniPath, "Process", "apodization"));
                para.Add("velocity", Ai.Hong.CommonMethod.ReadIniFile(iniPath, "Collection", "velocity"));
                return para;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 移动转轮位置 0 for off.1 for on
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool? SampleSpinner(int position)
        {

            if (!isConnected)
            {
                errorCode = -11;
                return false;
            }

            errorCode = instrumentObject.SampleSpinner(position);
            return errorCode == 0;

        }

        public void DeleteExtentedFile(string dstFile, string ext)
        {
            try
            {
                string tempstr = dstFile.ToLower().Replace(".spc", ext);
                if (System.IO.File.Exists(tempstr))
                    System.IO.File.Delete(tempstr);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 计算吸收谱
        /// </summary>
        /// <param name="backFile">背景光谱</param>
        /// <param name="sampleFile">样品光谱</param>
        /// <returns></returns>
        public  string CalculateAbs(string backFile, string sampleFile)
        {
            if (!System.IO.File.Exists(backFile) || !System.IO.File.Exists(sampleFile))
            {
                errorCode = -16;
                return null;
            }
            Ai.Hong.CommonLibrary.SpecFileFormatDouble back = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            Ai.Hong.CommonLibrary.SpecFileFormatDouble sample = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (!back.ReadFile(backFile))
            {
                errorCode = -17;
                return null;
            }
            if (!sample.ReadFile(sampleFile))
            {
                errorCode = -17;
                return null;
            }
            if (back.YDatas.Count() != sample.YDatas.Count())
            {
                errorCode = -18;
                return null;
            }
            for (int i = 0; i < back.YDatas.Count(); i++)
            {
                if (back.YDatas[i] == 0)
                    back.YDatas[i] = 1;
                else
                    back.YDatas[i] = Math.Abs(sample.YDatas[i] / back.YDatas[i]);
            }
            //得到吸收光谱
            for (int i = 0; i < back.YDatas.Count(); i++)
            {
                back.YDatas[i] = Math.Log10(1 / back.YDatas[i]);
            }
            float[] trData = new float[back.YDatas.Length];
            for (int index = 0; index < trData.Length; index++)
                trData[index] = (float)back.YDatas[index];
            string fileName = System.IO.Path.GetFileNameWithoutExtension(sampleFile);
            fileName = Path.Combine(System.IO.Path.GetDirectoryName(sampleFile), fileName + "_abs.spc");
            //存储样品吸光度光谱
            Ai.Hong.CommonLibrary.SPCFile.SaveFile(fileName, trData, sample.Parameter);
            return fileName;
        }

        /// <summary>
        /// 计算透射谱
        /// </summary>
        /// <param name="backFile">背景光谱</param>
        /// <param name="sampleFile">样品光谱</param>
        /// <returns></returns>
        public  string CalculateTrans(string backFile, string sampleFile)
        {
            if (!System.IO.File.Exists(backFile) || !System.IO.File.Exists(sampleFile))
            {
                errorCode = -16;
                return null;
            }
            Ai.Hong.CommonLibrary.SpecFileFormatDouble back = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            Ai.Hong.CommonLibrary.SpecFileFormatDouble sample = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (!back.ReadFile(backFile))
            {
                errorCode = -17;
                return null;
            }
            if (!sample.ReadFile(sampleFile))
            {
                errorCode = -17;
                return null;
            }
            if (back.YDatas.Count() != sample.YDatas.Count())
            {
                errorCode = -18;
                return null;
            }
            //得到透射图
            for (int i = 0; i < back.YDatas.Count(); i++)
            {
                if (back.YDatas[i] == 0)
                    back.YDatas[i] = 1;
                else
                    back.YDatas[i] = Math.Abs(sample.YDatas[i] / back.YDatas[i]);
            }
            float[] trData = new float[back.YDatas.Length];
            for (int index = 0; index < trData.Length; index++)
                trData[index] = (float)back.YDatas[index];
            string fileName = System.IO.Path.GetFileNameWithoutExtension(sampleFile);
            fileName = Path.Combine(System.IO.Path.GetDirectoryName(sampleFile), fileName + "_tr.spc");
            //存储样品吸光度光谱
            Ai.Hong.CommonLibrary.SPCFile.SaveFile(fileName, trData, sample.Parameter);
            return fileName;
        }

        /// <summary>
        /// 修改配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scPara"></param>
        /// <param name="iniFilePath"></param>
        /// <returns></returns>
        public  bool? ModifyIniFile<T>(T scPara, string iniFilePath)
        {
            Type type = scPara.GetType();
            if (type == null || !System.IO.File.Exists(iniFilePath))
            {
                return false;
            }
            foreach (var p in type.GetProperties())
            {
                if (string.Equals(p.Name, "xRegion"))
                {
                    ModifyIniFile(p.GetValue(scPara, null), iniFilePath);
                }
                switch (p.Name)
                {
                    case "resolution":
                        WritePrivateProfileString("Collection", "resolution", p.GetValue(scPara, null).ToString(), iniFilePath);
                        break;
                    case "count":
                        WritePrivateProfileString("Collection", "backgroundScans", p.GetValue(scPara, null).ToString(), iniFilePath);
                        WritePrivateProfileString("Collection", "sampleScans", p.GetValue(scPara, null).ToString(), iniFilePath);
                        break;
                    case "minumum":
                        WritePrivateProfileString("Process", "firstX", p.GetValue(scPara, null).ToString(), iniFilePath);
                        break;
                    case "maxumum":
                        WritePrivateProfileString("Process", "lastX", p.GetValue(scPara, null).ToString(), iniFilePath);
                        break;
                    default: break;
                }
            }
            return true;
        }
        

        #endregion


        #region Virtual Method

        /// <summary>
        /// 波数精度-聚苯乙烯-温度校正
        /// </summary>
        /// <param name="targetPeak"></param>
        /// <returns></returns>
        public virtual double? TemperatureCalibrate(double targetPeak) { return null; }

        /// <summary>
        /// 移动积分球位置 0 for sample.1 for background
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual bool? MoveFlag(int position) { return null; }

        /// <summary>
        /// 判断不控温样品池(透射) 是否放有样品
        /// </summary>
        /// <returns></returns>
        public virtual bool? IsTransmissionCellEmpty() { return null; }

        /// <summary>
        /// 扫描背景
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">背景保存文件</param>
        public virtual string ScanBackground(string scanMethodFile, int scanCount, string backgroundFile, string addPara = null){return null;}

        /// <summary>
        /// 扫描样品
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">样品保存文件</param>
        public virtual string ScanSample(string scanMethodFile, int scanCount, string sampleFile, string addPara = null){ return null;}



        #endregion
    }
}
