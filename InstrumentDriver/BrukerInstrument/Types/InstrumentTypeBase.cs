using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;
using System.Xml;
using System.Diagnostics;

namespace BrukerInstrument.Types
{
    public class InstrumentTypeBase
    {
        #region Win32 Method
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern long SetWindowLong(int n, int nlndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern long GetWindowLong(IntPtr hWnd, int index);
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        #endregion

        #region Public Property
        /// <summary>
        /// 
        /// </summary>
        public static Types.InstrumentTypeBase baseObj = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = string.Empty;

        /// <summary>
        /// matrix 仪器IP地址
        /// </summary>
        public static string IpAddress = null; // string.Empty;

        /// <summary>
        /// 是否连接仪器
        /// </summary>
        public static bool IsConnected = false;

        /// <summary>
        /// 仪器型号
        /// </summary>
        public static string instrumentModel = null;

        /// <summary>
        /// 仪器序列号
        /// </summary>
        public static string serialNumber = null;

        /// <summary>
        /// 互斥对象，防止同时操作
        /// </summary>
        public object thisLock = new object();
        #endregion
        

        #region Public Method

        /// <summary>
        /// 返回当前执行的错误
        /// </summary>
        /// <returns></returns>
        public static string GetError() { return ErrorString; }

        /// <summary>
        /// 连接仪器
        /// </summary>
        /// <returns></returns>
        public static  bool Connect()
        {
            if (IpAddress == null)
                return false;
            OpusCMD334.CheckOpus checkOpus = new OpusCMD334.CheckOpus();
            if (!checkOpus.IsOpusRunning("opus.exe"))
            {
                //Disconnect();
                //找到 OPUS安装路径
                string OpusPath = string.Empty;
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Classes\opus.文件\protocol\StdFileEditing\server", false);
                if (key != null)//判断键值存在
                {
                    string[] ttp = key.GetValueNames();
                    if (ttp == null || ttp.Count() == 0)
                    {
                        return false;
                    }
                    OpusPath = key.GetValue(ttp[0]).ToString();
                }
                if(string.IsNullOrEmpty(OpusPath))
                {
                    key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@".0\shell\Open\command", false);
                    if (key != null)
                    {
                        string[] ttp = key.GetValueNames();
                        if (ttp == null || ttp.Count() == 0)
                        {
                            return false;
                        }
                        OpusPath = key.GetValue(ttp[0]).ToString();
                        int i = OpusPath.IndexOf("opus.exe");
                        if(i==-1)
                        {
                            OpusPath = key.GetValue("").ToString();
                            i = OpusPath.IndexOf("opus.exe");
                        }
                        OpusPath = OpusPath.Substring(1, i + "opus.exe".Length - 1);
                    }
                }
                // 启动OPUS
                OpusCMD334.StartOpus startOpus = new OpusCMD334.StartOpus();
                if (startOpus == null)
                {
                    ErrorString = "Create 'StartOpus' Error!";
                    return false;
                }
                //"full_access.ows"
                startOpus.StartOpus(OpusPath, "OPUS", null, null, "CHINESE", false);
                System.Diagnostics.Process[] tp = System.Diagnostics.Process.GetProcessesByName("opus");

                if (tp.Count() > 0)
                {
                    int ptr = (tp[0].MainWindowHandle).ToInt32();
                    ShowWindow(ptr, SW_HIDE);
                }
            }
            return true;

        }

        /// <summary>
        /// 断开仪器连接
        /// </summary>
        /// <returns></returns>
        public static bool Disconnect()
        {
            OpusCMD334.CheckOpus checkOpus = new OpusCMD334.CheckOpus();
            if (checkOpus == null)
            {
                ErrorString = "Create 'CheckOpus' Error!";
                return false;
            }
            //OpusCMD334.CloseOpus closeOpus = new OpusCMD334.CloseOpus();
            //if(!closeOpus.CloseOpus())
            //{
            //    ErrorString = "Close OPUS Failed!";
            //    return false;
            //}
            string opus = "opus.exe";
            if (checkOpus.IsOpusRunning(ref opus))
            {
                OpusCMD334.CloseOpus closeOpus = new OpusCMD334.CloseOpus();
                if (!closeOpus.CloseOpus())
                {
                    ErrorString = closeOpus.ErrorDesc;
                    //return false;
                }
            }

            Process[] processes = Process.GetProcessesByName("OPUS");
            if (processes.Length > 0)   //强制关闭OPUS
            {
                for (int i = 0; i < processes.Length; i++)
                    processes[i].Kill();
            }
            return true;
        }

        /// <summary>
        /// 设置仪器的IP地址（需要在最开始调用)
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <returns>True = Bruker仪器</returns>
        public static bool SetIPAddress(string ipAddress)
        {
            //地址格式错误
            System.Net.IPAddress address = null;
            if (System.Net.IPAddress.TryParse(ipAddress, out address) == false)
                return false;

            //地址错误
            if (Ai.Hong.CommonMethod.PingDevice(ipAddress, 2000) == false)
                return false;

            if (IsBurkerInstrument(ipAddress, out serialNumber, out instrumentModel) == false)
                return false;

            IpAddress = ipAddress;

            return true;
        }

        /// <summary>
        /// 允许不安全的文件头解析
        /// </summary>
        public static  bool SetAllowUnsafeHeaderParsing()
        {
            //Get the assembly that contains the internal class
            System.Reflection.Assembly aNetAssembly = System.Reflection.Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
            if (aNetAssembly != null)
            {
                //Use the assembly in order to get the internal type for 
                // the internal class
                Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (aSettingsType != null)
                {
                    //Use the internal static property to get an instance 
                    // of the internal settings class. If the static instance 
                    // isn't created allready the property will create it for us.
                    object anInstance = aSettingsType.InvokeMember("Section",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty
                    | System.Reflection.BindingFlags.NonPublic, null, null, new object[] { });
                    if (anInstance != null)
                    {
                        //Locate the private bool field that tells the 
                        // framework is unsafe header parsing should be 
                        // allowed or not
                        System.Reflection.FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField(
                        "useUnsafeHeaderParsing",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (aUseUnsafeHeaderParsing != null)
                        {
                            aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 通过网页判断是不是Burker仪器，并获取仪器的序列号
        /// </summary>
        /// <param name="commPort">仪器IP地址</param>
        /// <param name="serialNo">输出：序列号</param>
        /// <returns>True:是Bruker仪器</returns>
        public static  bool IsBurkerInstrument(string ipAddress, out string serialNo, out string instrumentModel)
        {
            serialNo = null;
            instrumentModel = null;
            string url = "http://" + ipAddress + "/";
            string htmlstr = DownloadWebPage(url);
            if (htmlstr == null)
                return false;

            //仪器格式为：<h2>Alpha 2 00981 - Home</h2>
            //<h2>Matrix-F SFDA SN_0329 Home</h2>

            string h2Mark = "<h2>";
            int h2IndexBegin = htmlstr.IndexOf(h2Mark, StringComparison.OrdinalIgnoreCase);
            if (h2IndexBegin == -1)
                return false;
            h2IndexBegin += h2Mark.Length;

            //取结尾
            string h2MarkEnd = "</h2>";
            int h2IndexEnd = htmlstr.IndexOf(h2MarkEnd, h2IndexBegin, StringComparison.OrdinalIgnoreCase);
            if (h2IndexEnd < 0)
                return false;

            string header = htmlstr.Substring(h2IndexBegin, h2IndexEnd - h2IndexBegin);
            if (string.IsNullOrWhiteSpace(header))
                return false;

            //去掉'Home' 和 '- Home'
            header = header.Replace("- Home", "");
            header = header.Replace("Home", "");
            header = header.Trim();

            if (string.IsNullOrWhiteSpace(header))
                return false;

            //取第一个空格全面为仪器型号
            h2IndexBegin = header.IndexOf(' ');
            if (h2IndexBegin < 1)
                return false;

            //第一个是仪器类型
            instrumentModel = header.Substring(0, h2IndexBegin);
            bool result = InitInstrumentObject(instrumentModel);
            if (!result) return false;

            //后面的是仪器序列号
            serialNo = header.Substring(h2IndexBegin + 1);
            if (serialNo != null)
                serialNo = serialNo.Trim();

            return !string.IsNullOrWhiteSpace(serialNo);
        }

        /// <summary>
        /// 下载网页为String
        /// </summary>
        /// <param name="Url">网址</param>
        public static  string DownloadWebPage(string Url)
        {
            SetAllowUnsafeHeaderParsing();

            int testtime = Environment.TickCount;
            try
            {
                // Open a connection
                System.Net.HttpWebRequest WebRequestObject = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(Url);

                // You can also specify additional header values like 
                // the user agent or the referer:
                WebRequestObject.UserAgent = ".NET Framework/2.0";
                WebRequestObject.Referer = "http://www.example.com/";
                // Request response:
                System.Net.WebResponse Response = WebRequestObject.GetResponse();

                // Open data stream:
                System.IO.Stream WebStream = Response.GetResponseStream();

                // Create reader object:
                System.IO.StreamReader Reader = new System.IO.StreamReader(WebStream);

                // Read the entire stream content:
                string PageContent = Reader.ReadToEnd();

                // Cleanup
                Reader.Close();
                WebStream.Close();
                Response.Close();

                testtime = Environment.TickCount - testtime;

                return PageContent;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 初始化仪器操作接口
        /// </summary>
        /// <param name="ins"></param>
        private static bool InitInstrumentObject(string ins)
        {
            switch (ins)
            {
                case "Tango":
                    baseObj = new Types.Tango(); return true;
                case "MPA":
                    baseObj = new Types.MPA(); return true;
                case "Matrix-I":
                    baseObj = new Types.Matrix_I(); return true;
                default:
                    ErrorString = "There is not the COM Object of this Instument type : " + ins + "\r\n Please contact the Developer !";
                    return false;
            }
        }

        /// <summary>
        /// 获取仪器序列号
        /// </summary>
        /// <returns></returns>
        public static FTNirInterface.InstrumentInterface.InstrumentInfo GetSerialNumber()
        {
            FTNirInterface.InstrumentInterface.InstrumentInfo info = new FTNirInterface.InstrumentInterface.InstrumentInfo();
            info.serialNumber = serialNumber;
            switch(instrumentModel)
            {
                case "MPA":info.instrumentType=FTNirInterface.enumInstrumentType.MPA;break;
                case "Tango":info.instrumentType=FTNirInterface.enumInstrumentType.Tango;break;
                case "Matrix-I": info.instrumentType = FTNirInterface.enumInstrumentType.Matrix_I; break;
                default:break;
            }
            info.instrumentFactor = FTNirInterface.enumInstrumentFactor.Bruker;
            return info;
        }

        /// <summary>
        /// 计算吸收谱
        /// </summary>
        /// <param name="backFile">背景光谱</param>
        /// <param name="sampleFile">样品光谱</param>
        /// <returns></returns>
        public string CalculateAbs(string backFile, string sampleFile)
        {
            if (!System.IO.File.Exists(backFile) || !System.IO.File.Exists(sampleFile))
            {
                ErrorString = "Spectrum nos Exist!";
                return null;
            }
            Ai.Hong.CommonLibrary.SpecFileFormatDouble back = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            Ai.Hong.CommonLibrary.SpecFileFormatDouble sample = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (!back.ReadFile(backFile))
            {
                ErrorString = "Read Spectrum Error \r\n" + backFile;
                return null;
            }
            if (!sample.ReadFile(sampleFile))
            {
                ErrorString = "Read Spectrum Error \r\n" + sample;
                return null;
            }
            if (back.YDatas.Count() != sample.YDatas.Count())
            {
                ErrorString = "The points of back and sample not equal!";
                return null;
            }
            int count = back.YDatas.Count() > sample.YDatas.Count() ? sample.YDatas.Count() : back.YDatas.Count();
            for (int i = 0; i < count; i++)
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
            fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sampleFile), fileName + "_abs.spc");
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
        public string CalculateTrans(string backFile, string sampleFile)
        {
            if (!System.IO.File.Exists(backFile) || !System.IO.File.Exists(sampleFile))
            {
                ErrorString = "Spectrum nos Exist!";
                return null;
            }
            Ai.Hong.CommonLibrary.SpecFileFormatDouble back = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            Ai.Hong.CommonLibrary.SpecFileFormatDouble sample = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (!back.ReadFile(backFile, Ai.Hong.CommonLibrary.SpecFileFormat.specType.Background))
            {
                ErrorString = "Read Spectrum Error \r\n" + backFile;
                return null;
            }
            if (!sample.ReadFile(sampleFile, Ai.Hong.CommonLibrary.SpecFileFormat.specType.SingleBeam))
            {
                ErrorString = "Read Spectrum Error \r\n" + sample;
                return null;
            }
            if (back.YDatas.Count() != sample.YDatas.Count())
            {
                ErrorString = "The points of back and sample not equal!";
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
            fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sampleFile), fileName + "_tr.spc");
            //存储样品吸光度光谱
            Ai.Hong.CommonLibrary.SPCFile.SaveFile(fileName, trData, sample.Parameter);
            return fileName;
        }

        /// <summary>
        /// 加载光谱文件到OPUS
        /// </summary>
        /// <param name="fileName">光谱文件名</param>
        /// <param name="fileDirectory">光谱文件夹</param>
        /// <returns></returns>
        private bool LoadFile(string fileName)
        {
            lock (thisLock)
            {
                if (!System.IO.File.Exists(fileName))
                {
                    ErrorString = "File " + fileName + "\r\n is not exist!";
                    return false;
                }
                OpusCMD334.LoadFile loadFile = new OpusCMD334.LoadFile();
                if (!loadFile.Load(System.IO.Path.GetFileName(fileName), System.IO.Path.GetDirectoryName(fileName)))
                {
                    ErrorString = loadFile.ErrorDesc;
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 从OPUS卸载光谱文件
        /// </summary>
        /// <param name="path">光谱路径</param>
        /// <returns></returns>
        private bool UnLoadFile(string path)
        {
            lock (thisLock)
            {
                if (!System.IO.File.Exists(path))
                {
                    ErrorString = "File " + path + "\r\n is not exist!";
                    return false;
                }
                OpusCMD334.UnloadFile unloadFile = new OpusCMD334.UnloadFile();
                if (!unloadFile.Unload(path))
                {
                    ErrorString = unloadFile.ErrorDesc;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 加载配置文件XPM
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns></returns>
        public bool LoadXPM(string path)
        {
            if (path == null)
            {
                return true;
            }
            if (!System.IO.File.Exists(path))
            {
                ErrorString = "XPM File is not Exist!";
                return false;
            }
            lock (thisLock)
            {
                //检查XPM合法性
                OpusCMD334.CheckXPM checkXpm = new OpusCMD334.CheckXPM();
                if (!checkXpm.CheckXPM(path))
                {
                    ErrorString = checkXpm.ErrorDesc;
                    return false;
                }
                //加载XPM文件到OPUS
                OpusCMD334.LoadXPM loadXpm = new OpusCMD334.LoadXPM();
                if (!loadXpm.LoadXPM(path))
                {
                    ErrorString = loadXpm.ErrorDesc;
                    return false;
                }
                return true;
            }
        }

        #endregion


        #region Virtual Method
        /// <summary>
        /// 获取激光波数
        /// </summary>
        /// <returns></returns>
        public virtual double? GetLaserWavelength() { return null; }

        /// <summary>
        /// 写入激光波数到仪器
        /// </summary>
        /// <param name="waveNum"></param>
        /// <returns></returns>
        public virtual bool? SetLaserWavelength(double curPeak, double targetPeak, ref double curLaser) { return null; }

        /// <summary>
        /// 波数精度-聚苯乙烯-温度校正
        /// </summary>
        /// <param name="targetPeak"></param>
        /// <returns></returns>
        public virtual double? TemperatureCalibrate(double targetPeak) { return null; }
        /// <summary>
        /// 下载网页为String
        /// </summary>
        /// <param name="Url">网址</param>
        //public virtual string DownloadWebPage(string Url)
        //{
        //    return null;
        //}

        /// <summary>
        /// 移动转轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <returns></returns>
        public virtual bool? MoveWheel(int position, string iniFilePath) { return null; }

        

        /// <summary>
        /// 扫描背景光谱
        /// </summary>
        /// <param name="scanMethodFile">XPM文件路径</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">背景光谱存储路径</param>
        /// <returns></returns>
        public virtual string ScanBackground(string scanMethodFile, int scanCount, string backgroundFile, string addPara = null)
        {
            return null;
        }

        /// <summary>
        /// 扫描样品光谱
        /// </summary>
        /// <param name="scanMethodFile">XPM文件路径</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="sampleFile">样品光谱存储路径</param>
        /// <returns></returns>
        public virtual string ScanSample(string scanMethodFile, int scanCount, string sampleFile, string addPara = "")
        {
            return null;
        }

        /// <summary>
        /// 光谱兼容处理
        /// </summary>
        /// <param name="sourceFile">需要处理的光谱</param>
        /// <param name="destFile">标准光谱</param>
        /// <returns></returns>
        public virtual string MakeCompatiable(string sourceFile, string destFile) { return null; }

        public virtual string  SpecProcess(string filename, double firstX = 6900, double lastX = 7500)
        {
            return filename;
        }

        /// <summary>
        /// 修改配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scPara"></param>
        /// <param name="iniFilePath"></param>
        /// <returns></returns>
        public virtual bool? ModifyIniFile<T>(T scPara, string iniFilePath) { return null; }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <param name="iniPath">配置文件路径</param>
        public virtual Dictionary<string, string> ReadScanPara(string xpmPath) { return null; }
        #endregion


        #region 暂未使用

        /// <summary>
        /// 获取激光矫正光谱
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="firstX">校正范围</param>
        /// <param name="lastX">校正范围</param>
        /// <returns></returns>
        public static string GetLaserWavelengthDataaaa(string filename, double firstX = 6900, double lastX = 7500)
        {
            if (filename == null || !System.IO.File.Exists(filename))
                return null;

            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();

            //加载文件
            string cmdstr = string.Format(" Load (, {{COF=0, DAP='{0}', DAF='{1}'}});", System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileName(filename));
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 3 || cmdrets[0] != "OK")
                return null;

            //剪切
            string opusfile = cmdrets[3];
            cmdstr = string.Format("Cut ([{0}:Spec], {{CFX={1}, CLX={2}}});", opusfile, firstX, lastX);
            cmdcode = cmd.Command(cmdstr);
            cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return null;

            //基线校正
            cmdstr = string.Format("Baseline ([{0}:Spec], {{BME=3}});", opusfile);
            cmdcode = cmd.Command(cmdstr);
            cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return null;

            //最大最小归一化
            cmdstr = string.Format("Normalize ([{0}:Spec], {{NME=1, NFX=4000.000000, NLX=400.000000, NWR=1}});", opusfile);
            cmdcode = cmd.Command(cmdstr);
            cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return null;

            //改变数据块类型
            cmdstr = string.Format("ChangeDataBlockType ([{0}:Spec], {{CBT='AB'}});", opusfile);
            cmdcode = cmd.Command(cmdstr);
            cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return null;

            //另存文件
            cmdstr = string.Format("Save ([{0}:Spec], {{}});", opusfile);
            cmdcode = cmd.Command(cmdstr);
            cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 3 || cmdrets[0] != "OK")
                return null;

            //卸载文件
            cmdstr = string.Format("Unload ([{0}:Spec], {{}});", opusfile);
            cmdcode = cmd.Command(cmdstr);
            cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return null;
            return filename;
        }

        /// <summary>
        /// 获取OPUS配置文件路径
        /// </summary>
        /// <returns></returns>
        private string GetOpusSettingPath()
        {
            //找到opus配置文件路径
            OpusCMD334.GetOpusPath path = new OpusCMD334.GetOpusPath();
            if (path == null || !path.GetOpusPath())
            {
                ErrorString = "Create 'GetOpusPath' Error!";
                return null;
            }

            if (!System.IO.Directory.Exists(path.Opus_Path))
            {
                ErrorString = "Can't find OPUS document path,Please check opus is installed or not!";
                return null;
            }

            string temp = System.IO.Path.Combine(path.Opus_Path, "matrix.nti");
            if (!System.IO.File.Exists(temp))
            {
                ErrorString = "matrix.nti Path is Not Exist!\r\n" + temp;
                return null;
            }
            return temp;
        }

        /// <summary>
        /// 获取并Ping IP地址  确定仪器是否连接
        /// </summary>
        /// <returns></returns>
        private bool Ping(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);    //加载Xml文件  
            XmlElement rootElem = doc.DocumentElement;   //获取根节点  
            XmlNodeList personNodes = rootElem.GetElementsByTagName("INSTRUMENT"); //获取person子节点集合  
            foreach (XmlElement node in personNodes)
            {
                foreach (XmlElement child in node.ChildNodes)
                {
                    //读取IP地址 
                    if (string.Equals(child.Name, "URL"))
                    {
                        //判断IP是否正确
                        if (System.Text.RegularExpressions.Regex.IsMatch(child.InnerText, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                        {
                            IpAddress = child.InnerText;
                            break;
                        }
                        else
                        {
                            ErrorString = "File Error!:\r\n" + path;
                            return false;
                        }
                    }

                }
            }

            //Ping 仪器
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            if (ping.Send(IpAddress).Status != System.Net.NetworkInformation.IPStatus.Success)
            {
                ErrorString = "Can't find Instrument!";
                return false;
            }
            return true;
        }

        private static bool ChangeDataBlockType(string opusFile, string srcType, string dstType)
        {
            //改变数据块类型
            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();
            string cmdstr = string.Format("ChangeDataBlockType ([{0}:{1}], {{CBT='{2}'}});", opusFile, srcType, dstType);
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return false;

            return true;

        }

        private static bool SaveAsSPC(string opusFile, string spcFile)
        {
            //SaveAs ([<$ResultFile 1>:Spec], {DAP='E:\Project\VspecNirCloud\CloudManager\bin\Release\Data\IT_Tango\PQ\2015_11_30\14_39_59\LineNoiseAndDevTest', OEX='1', SAN='LineNoiseAndDevTest_2015-11-30 14-39-59.0.spc', COF=256, DPA=5, DPO=5, SEP=',', YON='0', ADP='1', X64='1'});
            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();
            string cmdstr = string.Format("SaveAs ([{0}:Spec], {{DAP='{1}', OEX='1', SAN='{2}', COF=256, DPA=5, DPO=5, SEP=',', YON='0', ADP='1', X64='1'}});", opusFile, System.IO.Path.GetDirectoryName(spcFile), System.IO.Path.GetFileName(spcFile));
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return false;

            return true;
        }

        public static string ChangeWaterRangeToAB(string spectrumFile, string saveFile)
        {
            try
            {
                OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();

                //加载文件
                string cmdstr = "Load (, {COF=0, DAP='" + System.IO.Path.GetDirectoryName(spectrumFile) + "', DAF='" + System.IO.Path.GetFileName(spectrumFile) + "'});";
                if (cmd.Command(cmdstr) == false)
                    throw new Exception("Load Error:" + cmd.ErrorDesc);

                string[] rets = cmd.CommandResult.Split('\n');
                string retname = rets[3];
                cmdstr = "Calculator ([" + retname + ":Spec], {FOR='[" + retname + ":Spec]'});";
                if (cmd.Command(cmdstr) == false)
                    throw new Exception("Load Error:" + cmd.ErrorDesc);

                cmdstr = "Cut ([" + retname + ":Spec], {{CFX=6900.000000, CLX=7500.000000});";
                if (cmd.Command(cmdstr) == false)
                    throw new Exception("Load Error:" + cmd.ErrorDesc);

                cmdstr = "Baseline ([" + retname + ":Spec], {BME=3});";
                if (cmd.Command(cmdstr) == false)
                    throw new Exception("Load Error:" + cmd.ErrorDesc);

                cmdstr = "ABTR ([" + retname + ":Spec], {CCM=3});";
                if (cmd.Command(cmdstr) == false)
                    throw new Exception("Load Error:" + cmd.ErrorDesc);

                //Normalize ([<$ResultFile 4>:AB], {NME=1, NFX=4000.000000, NLX=400.000000, NWR=1});
                cmdstr = "Normalize ([" + retname + ":Spec], {NME=1, NFX=4000.000000, NLX=400.000000, NWR=1});";
                if (cmd.Command(cmdstr) == false)
                    throw new Exception("Load Error:" + cmd.ErrorDesc);

                //SaveAs ([<$ResultFile 4>], {DAP='D:', OEX='1', SAN='AccurancyResult.0', YON='0'});
                cmdstr = "SaveAs ([" + retname + "], {DAP='" + System.IO.Path.GetDirectoryName(saveFile) + "', OEX='1', SAN='" + System.IO.Path.GetFileName(saveFile) + "', YON='0'});";
                if (cmd.Command(cmdstr) == false)
                    throw new Exception("Load Error:" + cmd.ErrorDesc);

                return saveFile;
            }
            catch (Exception ex)
            {
                ErrorString = ex.ToString();
                return null;
            }

        }



        #endregion
    }
}
