using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;
using System.Xml;

namespace BrukerInstrument
{
    public class BrukerInstrument:FTNirInterface.InstrumentInterface
    {

        #region

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        static Types.InstrumentTypeBase baseObj = null;

        /// <summary>
        /// 返回当前执行的错误
        /// </summary>
        /// <returns></returns>
        public override string GetError()
        {
            return Types.InstrumentTypeBase.GetError();
        }
        public class para
        {
            public string resolution { get; set; }
            public string count { get; set; }
        }

        public void Test()
        {
           // para pa = new para();
           // pa.resolution = "4";
           // pa.count = "18";
           // ModifyIniFile(pa, @"e:\AccuracyPQ.xpm");
           // LoadXPM(@"e:\AccuracyPQ.xpm");
           // GetLaserWavelength();
           // double target=0;
           // double? curTarget=0;
           // SetLaserWavelength(7181.53, 7181.68, ref target);

           //int i = 0;
           // Connect();
           // while (Math.Abs((double)curTarget - 7181.68) > 0.1)
           // {
           //     if (System.IO.File.Exists("e:\\" + i.ToString() + ".0"))
           //         System.IO.File.Delete("e:\\" + i.ToString() + ".0");
           //     string testSpc = ScanBackground(@"E:\AccuracyPQ.xpm", 3, "e:\\" + i.ToString() + ".0");//(@"C:\Users\Public\Documents\Bruker\OPUS_7.5.18\XPM\ATR_Di.XPM", 22, @"E:\123.0");
           //     Ai.Hong.CommonLibrary.SpecFileFormatDouble sp = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
           //     if (!sp.ReadFile(testSpc, Ai.Hong.CommonLibrary.SpecFileFormat.specType.Background))//@"E:\234.0"
           //     {
           //         string temp = sp.ErrorString;
           //     }
           //     double tempy;
           //     target = Ai.Hong.SpectrumAlgorithm.SpectrumAlgorithm.PickPeak(sp.XDatas, sp.YDatas, 7181.68, 4, false, out tempy);
           //     double cur = 0;
           //     SetLaserWavelength(target, 7181.68, ref cur);
           //     curTarget = GetLaserWavelength();
           //     i++;
           // }
           // ReadScanPara(@"E:\复烤厂-在线近红外分析系统\泸州\XPM备份\样品扫描.XPM");
            //OpusCMD334.ReadParameter read = new OpusCMD334.ReadParameter();
            //read.ReadParm("TWK");
            //string ss=read.Value;
            

            //ReadScanPara(@"E:\OPUS参数\（MPA）测试样品.XPM");
            //string ser = GetSerialNumber();
            //ser += "";
            //OpusCMD334.ReadParameter read = new OpusCMD334.ReadParameter();
            //if(!read.ReadParm("RES"))
            //{
            //    ErrorString = read.ErrorDesc;
            //}
            //ser += read.Value;
            //LoadFile("123.0", @"E:\复烤厂-在线近红外分析系统\泸州\光谱文件");
            //UnLoadFile(@"E:\复烤厂-在线近红外分析系统\泸州\光谱文件\123.0");
        }

        /// <summary>
        /// 设置仪器的IP地址（需要在最开始调用)
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <returns>True = Bruker仪器</returns>
        public bool SetIPAddress(string ipAddress)
        {
            bool result = Types.InstrumentTypeBase.SetIPAddress(ipAddress);
            baseObj = Types.InstrumentTypeBase.baseObj;
            return result;
        }

        public override bool Connect()
        {
            return Types.InstrumentTypeBase.Connect();
            //if (IpAddress == null)
            //    return false;

            //启动OPUS
            //return StartOPUS();
            

            ////获取OPUS配置文件路径
            //string DocPath = GetOpusSettingPath();
            //if (DocPath == null)
            //    return false;

            //检查是否联机
            //OpusCMD334.Matrix matrix = new OpusCMD334.Matrix();
            //if (matrix == null)
            //{
            //    ErrorString = "Create 'GetOpusPath' Error!";
            //    return false;
            //}
            //if (!matrix.GetIP(DocPath))
            //{
            //    ErrorString = matrix.ErrorDesc;
            //    //return false;
            //}

            //读取并Ping IP地址 确定仪器是否联机
            //if (!Ping(DocPath))
            //    return false;

            //return IsConnected = true;
            //}
        }


        /// <summary>
        /// 断开仪器连接
        /// </summary>
        /// <returns></returns>
        public override bool Disconnect()
        {
            return Types.InstrumentTypeBase.Disconnect();
        }

        /// <summary>
        /// 获取仪器序列号
        /// </summary>
        /// <returns></returns>
        public override InstrumentInfo GetInstrumentInfo()
        {
            return Types.InstrumentTypeBase.GetSerialNumber();
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <param name="iniPath">配置文件路径</param>
        public override Dictionary<string, string> ReadScanPara(string xpmPath)
        {
            return baseObj.ReadScanPara(xpmPath);
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
            return baseObj.ModifyIniFile<T>(scPara, iniFilePath);
        }

        /// <summary>
        /// 加载配置文件XPM
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns></returns>
        private bool LoadXPM(string path)
        {
            return baseObj.LoadXPM(path);
        }

        /// <summary>
        /// 移动转轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <returns></returns>
        public override bool? MoveWheel(int position, string iniFilePath)
        {
            return baseObj.MoveWheel(position, iniFilePath);
        }


        /// <summary>
        /// 扫描背景光谱
        /// </summary>
        /// <param name="scanMethodFile">XPM文件路径</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">背景光谱存储路径</param>
        /// <returns></returns>
        public override string ScanBackground(string scanMethodFile, int scanCount, string backgroundFile, string addPara = null)
        {
            return baseObj.ScanBackground(scanMethodFile, scanCount, backgroundFile, addPara);
        }

        public string SpecProcess(string path)
        {
            return baseObj.SpecProcess(path);
        }

        /// <summary>
        /// 扫描样品光谱
        /// </summary>
        /// <param name="scanMethodFile">XPM文件路径</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="sampleFile">样品光谱存储路径</param>
        /// <returns></returns>
        public override string ScanSample(string scanMethodFile, int scanCount, string sampleFile, string addPara = "")
        {
            return baseObj.ScanSample(scanMethodFile, scanCount, sampleFile, addPara);
            /*
            if (!LoadXPM(scanMethodFile) || !System.IO.Path.HasExtension(sampleFile))
            {
                return null;
            }
            string specPath = sampleFile.Replace(".spc", ".0");
            if (System.IO.File.Exists(specPath))
            {
                OpusCMD334.UnloadFile unLoadFile = new OpusCMD334.UnloadFile();
                unLoadFile.Unload(specPath);
                System.IO.File.Delete(specPath);
            }
            if (System.IO.File.Exists(sampleFile))
            {
                System.IO.File.Delete(sampleFile);
            }
            OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            if (!command.Command("MeasureSample ({EXP='" + System.IO.Path.GetFileName(scanMethodFile) + "'," +
                "XPP='" + System.IO.Path.GetDirectoryName(scanMethodFile) + "'," +
                "PTH='" + System.IO.Path.GetDirectoryName(sampleFile) + "'," +
                "NAM='" + System.IO.Path.GetFileNameWithoutExtension(sampleFile) + "'," +
                "NSS = " + scanCount + "})", true))
            {
                ErrorString = command.ErrorDesc;
                return null;
            }
            //OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            //if (!command.Command("{NSS=" + scanCount.ToString() + "}", true))
            //{
            //    ErrorString = command.ErrorDesc;
            //    return null;
            //}
            //OpusCMD334.TakeSample takeSample = new OpusCMD334.TakeSample();
            //if (!takeSample.TakeSample(System.IO.Path.GetFileName(scanMethodFile),System.IO.Path.GetDirectoryName(scanMethodFile),System.IO.Path.GetFileName(sampleFile),System.IO.Path.GetDirectoryName(sampleFile),true))
            //{
            //    ErrorString = takeSample.ErrorDesc;
            //    return null;
            //}
            OpusCMD334.UnloadFile unLoad = new OpusCMD334.UnloadFile();
            if (!unLoad.Unload(specPath))
            {
                ErrorString = unLoad.ErrorDesc;
                return null;
            }
            System.IO.File.Move(specPath, sampleFile);
            System.IO.File.Delete(specPath);
            return sampleFile;
             * */
        }

        /// <summary>
        /// 获取激光波数
        /// </summary>
        /// <returns></returns>
        public override double? GetLaserWavelength()
        {
            return baseObj.GetLaserWavelength();
            

            //string testPath="E:\\test.0";
            //if (System.IO.File.Exists(testPath))
            //    System.IO.File.Delete(testPath);
            //string testSpc = ScanBackground(@"C:\Users\Public\Documents\Bruker\OPUS_7.5.18\XPM\ATR_Di.XPM", 3, testPath);//(@"C:\Users\Public\Documents\Bruker\OPUS_7.5.18\XPM\ATR_Di.XPM", 22, @"E:\123.0");
            
            //LoadFile(System.IO.Path.GetFileName(testSpc), System.IO.Path.GetDirectoryName(testSpc));
            ////LoadFile("1.4", @"C:\Users\xu.chuan\Documents\Bruker\OPUS_7.5.18\Data\MEAS");
            //OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            //if (!command.Command(@"READ_FROM_FILE" + testSpc))// C:\Users\xu.chuan\Documents\Bruker\OPUS_7.5.18\Data\MEAS\1.4"))
            //{
            //    ErrorString = "DDE Error!";
            //    return null;
            //}
            //if (!command.Command("READ_FROM_BLOCK INSTRUMENT PARAMETER"))
            //{
            //    ErrorString = command.CommandResult;
            //    return null;
            //}
            //if (!command.Command("READ_PARAMETER LWN"))
            //{
            //    ErrorString = command.CommandResult;

            //    return null;
            //}
            //基础激光波数
            //OpusCMD334.ReadParameter readPara = new OpusCMD334.ReadParameter();
            //if (!readPara.ReadParm("LWN"))
            //{
            //    ErrorString = readPara.ErrorDesc;
            //    return null;
            //}
            //double value = 0;
            //if (!double.TryParse(readPara.Value, out value))
            //    return null;
            ////string str = command.CommandResult.Replace("OK", string.Empty).Replace("\n", string.Empty);
            ////UnLoadFile(testSpc);
            ////return Convert.ToDouble(str.Replace("\0", string.Empty));
            //return 0;
            //校正激光波数
            //retstr = ExecuteGetOMNIC("Bench LaserAdjust");
            //double laserAdjust = 0;
            //if (double.TryParse(retstr, out laserAdjust) == false)
            //    return null;

            //return laserFred + laserAdjust;
        }

        /// <summary>
        /// 写入激光波数到仪器
        /// </summary>
        /// <param name="waveNum"></param>
        /// <returns></returns>
        public override bool? SetLaserWavelength(double curPeak, double targetPeak, ref double curLaser)
        {
            return baseObj.SetLaserWavelength(curPeak, targetPeak,ref curLaser);
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
        /// 通过网页判断是不是Burker仪器，并获取仪器的序列号
        /// </summary>
        /// <param name="commPort">仪器IP地址</param>
        /// <param name="serialNo">输出：序列号</param>
        /// <returns>True:是Bruker仪器</returns>
        public static bool IsBurkerInstrument(string ipAddress, out string serialNo, out string instrumentModel)
        {
            bool result= Types.InstrumentTypeBase.IsBurkerInstrument(ipAddress, out serialNo, out instrumentModel);
           // baseObj = Types.InstrumentTypeBase.baseObj;
            return result;
        }


        /// <summary>
        /// 光谱兼容处理
        /// </summary>
        /// <param name="sourceFile">需要处理的光谱</param>
        /// <param name="destFile">标准光谱</param>
        /// <returns></returns>
        public static string MakeCompatiable(string sourceFile, string destFile)
        {
            return baseObj.MakeCompatiable(sourceFile, destFile);
        }

    }
}
