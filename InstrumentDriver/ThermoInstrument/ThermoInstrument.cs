using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmTalk;
using System.Runtime.InteropServices;

namespace FTNirInterface.ThermoInstrument
{
    /// <summary>
    /// Thermo仪器操作
    /// </summary>
    public class ThermoInstrument:FTNirInterface.InstrumentInterface
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

        /// <summary>
        /// omnic操作对象
        /// </summary>
        private OmTalkClass omTalkObject = null;

        /// <summary>
        /// 错误消息
        /// </summary>
        private string ErrorMessage = null;

        /// <summary>
        /// 保存当前激光波数
        /// </summary>
        public double laserWaveNum = 0.63291848068006051;

        /// <summary>
        /// 错误代码
        /// </summary>
        private short ErrorCode = 0;

        /// <summary>
        /// 最大X值
        /// </summary>
        static int maxXWavelength;

        /// <summary>
        /// 最小X值
        /// </summary>
        static int minXWavelength;
        
        #endregion

        private UInt32 authorValue = 0;

        /// <summary>
        /// 测试Ominic命令
        /// </summary>
        /// <param name="command">命令</param>
        public void test(string command)
        {
            ExecuteCommandOMNIC(command);
        }

        private string GetAuthorBenchString(string parameter)
        {
            //从授权文件里面读出的是DWORD，对应为：Benc
            string retstr = null;
            for (int i = 0; i < 4; i++)
            {
                retstr += (char)((authorValue >> i * 8) & 0xFF);
            }
            return "Bench " + parameter;
            return retstr + "h " + parameter;
        }
        
        /// <summary>
        /// 仪器连接
        /// </summary>
        public override bool Connect()
        {
            //if (GetAuthorityData(ACloudSecurity.enumDataType.NIR, (int)ACloudSecurity.enumFactory.THERMO, ref authorValue) == false)
            //{
            //    ErrorMessage = "Not Authorized";
            //    return false;
            //}
            //Disconnect();

            if (InitOmnicTalk() == false)
                return false;

            //最大的X值
            string retstr = ExecuteGetOMNIC(GetAuthorBenchString("HighCutoff"));
            double retvalue = 0;
            if (double.TryParse(retstr, out retvalue) == false)
                return false;
            maxXWavelength = (int)retvalue;

            //最小的X值
            retstr = ExecuteGetOMNIC(GetAuthorBenchString("LowCutoff"));
            retvalue = 0;
            if (double.TryParse(retstr, out retvalue) == false)
                return false;
            minXWavelength = (int)retvalue;

            System.Diagnostics.Process[] tp = (System.Diagnostics.Process.GetProcessesByName("omnic32"));

            if(tp.Count()>0)
            {
                int ptr=(tp[0].MainWindowHandle).ToInt32();
                ShowWindow(ptr, SW_HIDE);
            }

            return true;
        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <param name="iniPath">配置文件路径</param>
        public override Dictionary<string, string> ReadScanPara(string iniPath)
        {
            if(!System.IO.File.Exists(iniPath))
            {
                return null;
            }
            //加载扫描参数文件
            string command = "LoadParameters \"" + iniPath + "\" Collect " + GetAuthorBenchString("");
            if (ExecuteCommandOMNIC(command) == false)
                return null;
            try
            {
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("resolution", ExecuteGetOMNIC("Collect Resolution"));
                para.Add("scans", ExecuteGetOMNIC("Collect NumScans"));
                para.Add("gain", ExecuteGetOMNIC(GetAuthorBenchString("Gain")));
                para.Add("zeroFill", ExecuteGetOMNIC("Collect ZeroFill"));
                para.Add("phaseCorrection", ExecuteGetOMNIC("Collect PhaseCor"));
                para.Add("apodization", ExecuteGetOMNIC("Collect ApodizationFunction"));
                para.Add("velocity", ExecuteGetOMNIC(GetAuthorBenchString("Velocity")));
                return para;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 加载Omnic
        /// </summary>
        /// <param name="forceReload"></param>
        /// <returns></returns>
        private bool InitOmnicTalk(bool forceReload = false)
        {
            try
            {
                //if (forceReload && omTalkObject != null)
                //{
                //    omTalkObject.EndOMNIC();
                //    omTalkObject = null;
                //}

                if (omTalkObject == null)
                {
                    //System.Windows.Forms.MessageBox.Show("Init");
                    omTalkObject = new OmTalk.OmTalkClass();
                    omTalkObject.LoadOmTalk();
                    //System.Windows.Forms.MessageBox.Show("Start");
                    //Minimized with out focus
                    var result = omTalkObject.StartOMNIC(7, null);
                    //System.Windows.Forms.MessageBox.Show("StartEnd "+result);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                System.Windows.Forms.MessageBox.Show(ErrorMessage);
                return false;
            }
        }

        /// <summary>
        /// 断开仪器连接
        /// </summary>
        public override bool Disconnect()
        {
            if (omTalkObject != null)
            {
                omTalkObject.EndOMNIC();
                omTalkObject = null;
            }
            System.Diagnostics.Process[] tp = (System.Diagnostics.Process.GetProcessesByName("omnic32"));
            if (tp.Count() > 0)
            {
                for (int i = 0; i < tp.Length; i++)
                    tp[i].Kill();
            }
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("OmTalk");
            if (processes.Length > 0 )
            {
                for (int i = 0; i < processes.Length; i++)
                    processes[i].Kill();
            }
            return true;
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public override string GetError()
        {
            return ErrorMessage;
        }

        private bool ResultToValue<T>(string omnicReturn, out T retvalue) where T : IComparable, IFormattable
        {
            retvalue = default(T);
            if ((ErrorCode = omTalkObject.ErrOMNIC()) != 0 || string.IsNullOrWhiteSpace(omnicReturn))
                return false;
            try
            {
                retvalue = (T)Convert.ChangeType(omnicReturn, typeof(T));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 执行一条GETOMNIC指令
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private string ExecuteGetOMNIC(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter) || omTalkObject == null)
                return null;
            string retStr = omTalkObject.GetOMNIC(parameter);
            ErrorCode = omTalkObject.ErrOMNIC();
            if (ErrorCode != 0)
                return null;

            return retStr;
        }

        /// <summary>
        /// 波数精度-聚苯乙烯-温度校正
        /// </summary>
        /// <param name="targetPeak"></param>
        /// <returns></returns>
        public override double? TemperatureCalibrate(double targetPeak)
        {
            if (targetPeak < 0 )
            {
                return null;
            }
            string tem = omTalkObject.GetOMNIC("BenchStatus BoardTemp");
            ErrorCode = omTalkObject.ErrOMNIC();
            if (ErrorCode != 0)
                return null;
            double curTem = 0;
            if (double.TryParse(tem, out curTem) == false)
                return null;
            return targetPeak += curTem * 0.0107 - 0.7;
        }

        /// <summary>
        /// 执行一条GETOMNIC指令
        /// </summary>
        /// <param name="parameter">命令</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        private bool ExecuteSetOMNIC(string parameter, string value)
        {
            if (string.IsNullOrWhiteSpace(parameter) || string.IsNullOrWhiteSpace(value) || omTalkObject == null)
                return false;

            omTalkObject.SetOMNIC(parameter,value);
            ErrorCode = omTalkObject.ErrOMNIC();
            if (ErrorCode != 0)
                return false;
            
            return true;
        }

        /// <summary>
        /// 执行OMNIC命令
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool ExecuteCommandOMNIC(string parameter)
        {
            omTalkObject.ExecuteOMNIC(parameter);
            ErrorCode = omTalkObject.ErrOMNIC();
            if (ErrorCode != 0)
            {
                omTalkObject.ErrMsgBox();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取激光波数
        /// </summary>
        /// <returns></returns>
        public override double? GetLaserWavelength()
        {
            //基础激光波数
            string retstr = ExecuteGetOMNIC(GetAuthorBenchString("LaserFreq"));
            double laserFred = 0;
            if (double.TryParse(retstr, out laserFred) == false)
                return null;

            //校正激光波数
            retstr = ExecuteGetOMNIC(GetAuthorBenchString("LaserAdjust"));
            double laserAdjust = 0;
            if (double.TryParse(retstr, out laserAdjust) == false)
                return null;

            return laserFred + laserAdjust;
        }

        /// <summary>
        /// 写入激光波数到仪器
        /// </summary>
        /// <param name="curPeak">当前峰位</param>
        /// <param name="targetPeak">目标峰位</param>
        /// <param name="curLaser">返回激光波数</param>
        /// <returns></returns>
        public override bool? SetLaserWavelength(double curPeak,double targetPeak,ref double curLaser)
        {
            //设置当前激光波数 在扫描光谱Load文件后  写入omnic
            //if (curPeak == double.MaxValue && targetPeak == double.MaxValue)
            //{
            //    laserWaveNum = curLaser;
            //    return true;
            //}
            
            //获取基础激光波数
            string retstr = ExecuteGetOMNIC(GetAuthorBenchString("LaserFreq"));
            double laserFred = 0;
            if (double.TryParse(retstr, out laserFred) == false)
                return null;

            retstr = ExecuteGetOMNIC(GetAuthorBenchString("LaserAdjust"));
            double laserAdjust = 0;
            if (double.TryParse(retstr, out laserAdjust) == false)
                return null;
            //修改校正激光波数
            double tempFred = 0;
            if (curLaser == double.MaxValue)
            {
                tempFred = 10000 / (laserFred + laserAdjust);
            }
            else
            {
                tempFred = curLaser;
            }
            double writeValue = (10000 * targetPeak) / (tempFred * curPeak);
            curLaser = 10000 / writeValue;
            laserWaveNum = curLaser;
            tempFred = writeValue - laserFred;
            return ExecuteSetOMNIC(GetAuthorBenchString("LaserAdjust"), tempFred.ToString());
        }

        /// <summary>
        /// 设置扫描参数
        /// </summary>
        /// <param name="resolution">分辨率</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="Gain">增益</param>
        /// <param name="zeroFilling">填0因子</param>
        /// <param name="phaseCorrection">相位校正</param>
        /// <param name="apodization">截至函数</param>
        /// <returns></returns>
        public override bool SetScanParameter(double resolution, double firstX, double lastX, int scanCount, int Gain = 1, int zeroFilling = 0, enumPhaseCorrection phaseCorrection = enumPhaseCorrection.magnitude, enumApodization apodization = enumApodization.Blackman_Harris3Term)
        {
            if (firstX < minXWavelength)
                firstX = minXWavelength;
            if (lastX > maxXWavelength)
                lastX = maxXWavelength;

            //分辨率
            if (ExecuteSetOMNIC("Collect Resolution", resolution.ToString()) == false)
                return false;

            //扫描次数
            if (ExecuteSetOMNIC("Collect NumScans", scanCount.ToString()) == false)
                return false;

            //扫描范围以后再设定

            return true;
        }

        /// <summary>
        /// 保存光谱
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool SaveSpectrumData(string filename)
        {
            ACloudFileFormat.FileFormat specData = new ACloudFileFormat.FileFormat();

            //0, 0 表示所有光谱数据
            float firstx=0, lastx=0;
            omTalkObject.GetSpecData(ref firstx, ref lastx);
            if((ErrorCode = omTalkObject.ErrOMNIC()) != 0)
                return false;

            double resolution = 0;
            //分辨率
            string retstr = ExecuteGetOMNIC("Collect Resolution");
            if (ResultToValue(retstr, out resolution) == false)
                return false;
            specData.fileInfo.resolution = resolution;

            //数据数量
            int datacount;
            retstr = omTalkObject.GetSpecNum();
            if (ResultToValue(retstr, out datacount) == false)
                return false;
            specData.fileInfo.dataCount = datacount;

            //起始波数
            retstr = omTalkObject.GetSpecFirstX();
            if (ResultToValue(retstr, out firstx) == false)
                return false;

            //结束波数
            retstr = omTalkObject.GetSpecLastX();
            if (ResultToValue(retstr, out lastx) == false)
                return false;

            //X轴步长
            float xstep = 0;
            retstr = omTalkObject.GetSpecIncrement();
            if (ResultToValue(retstr, out xstep) == false)
                return false;

            //获取光谱数据
            double[] YDatas = new double[datacount];
            for (int i = 0; i < datacount; i++)
            {
                YDatas[i] = omTalkObject.GetSpecDataSingle(i+1);
            }

            specData.AddData(firstx, lastx, YDatas, ACloudFileFormat.FileFormat.SPECTYPE.SPCNIR, ACloudFileFormat.FileFormat.XAXISTYPE.XWAVEN, ACloudFileFormat.FileFormat.YAXISTYPE.YABSRB);
            
            //保存文件
            if (specData.SaveFile(filename, ACloudFileFormat.FileFormat.EnumFileType.SPC) == false)
                return false;

            return true;
        }

        /// <summary>
        /// 扫描光谱
        /// </summary>
        /// <param name="scanMethodFile">扫描设置</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="filename">保存的文件</param>
        /// <param name="isBackground">True=背景, False=样品</param>
        /// <returns></returns>
        private string ScanSpectrum(string scanMethodFile, int scanCount, string filename, bool isBackground)
        {
            System.Diagnostics.Process[] tp = (System.Diagnostics.Process.GetProcessesByName("omnic32"));

            if (tp.Count() > 0)
            {
                int ptr = (tp[0].MainWindowHandle).ToInt32();
                ShowWindow(ptr, SW_HIDE);
            }


            //参数错误
            if (string.IsNullOrWhiteSpace(scanMethodFile) || !System.IO.File.Exists(scanMethodFile) ||
                !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filename)))
            {
                ErrorMessage = "Invalid Parameters";
                return null;
            }

            //加载扫描参数文件
            string command = "LoadParameters \"" + scanMethodFile + "\" Collect " + GetAuthorBenchString("");
            if (ExecuteCommandOMNIC(command) == false)
                return null;

            if (laserWaveNum != double.MaxValue)
            {
                //写入当前激光波数到omnic
                double laserFred = 0;
                if (double.TryParse(ExecuteGetOMNIC(GetAuthorBenchString("LaserFreq")), out laserFred) == false)
                    return null;
                if (laserWaveNum == 0)
                {
                    ErrorMessage = "laserWaveNum=0";
                    return null;
                }
                double write = (10000 / laserWaveNum) - laserFred;
                omTalkObject.SetOMNIC(GetAuthorBenchString("LaserAdjust"), write.ToString());
                if (omTalkObject.ErrOMNIC() != 0)
                    return null;
            }

            //设置扫描次数
            if (scanCount != 0 && ExecuteSetOMNIC("Collect NumScans", scanCount.ToString()) == false)
                return null;

            //扫描背景，需要删除以前的背景
            //if (isBackground)
            //{
            //    if (ExecuteCommandOMNIC("Select All"))
            //        ExecuteCommandOMNIC("DeleteSpectrum");
            //}

            //执行背景，样品扫描
            command = isBackground ? "Invoke CollectBackground Background Auto" : "Invoke CollectSample Sample Auto";
            if (ExecuteCommandOMNIC(command) == false)
                return null;

            //显示刚刚扫描的光谱
            command = isBackground ? "DisplayBackground" : "Display";
            if (ExecuteCommandOMNIC(command) == false)
                return null;
            if (tp.Count() > 0)
            {
                int ptr = (tp[0].MainWindowHandle).ToInt32();
                ShowWindow(ptr, SW_HIDE);
            }

            double? laser = GetLaserWavelength();
            if (laser == null)
                return null;

            //if (ExecuteCommandOMNIC("LaserAdjustment "+ ((double)laser).ToString()) == false)
            //    return null;

            try
            {
                //保存扫描的文件
                if (SaveSpectrumData(filename) == false)
                    return null;
            }
            catch (Exception ex) { ErrorMessage = ex.ToString(); }
            //删除Omnic中刚刚扫描的光谱
            //if (!isBackground)
            //{
            //    if (ExecuteCommandOMNIC(@"Select ""Background"""))
            //        ExecuteCommandOMNIC("DeleteSpectrum");
            //}
            try
            {
                if (ExecuteCommandOMNIC("Select All"))
                    ExecuteCommandOMNIC("DeleteSpectrum");
            }
            catch (Exception ex) { ErrorMessage = ex.ToString(); }
            if (tp.Count() > 0)
            {
                int ptr = (tp[0].MainWindowHandle).ToInt32();
                ShowWindow(ptr, SW_HIDE);
            }
            return filename;
        }

        /// <summary>
        /// 扫描背景
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">背景保存文件</param>
        /// <param name="addPara">附件参数</param>
        public override string ScanBackground(string scanMethodFile, int scanCount, string backgroundFile, string addPara = null)
        {
            return ScanSpectrum(scanMethodFile, scanCount, backgroundFile, true);
        }

        /// <summary>
        /// 扫描样品
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="sampleFile">样品保存文件</param>
        /// <param name="addPara">附加参数</param>
        public override string ScanSample(string scanMethodFile, int scanCount, string sampleFile, string addPara = null)
        {
            return ScanSpectrum(scanMethodFile, scanCount, sampleFile, false);
        }

        /// <summary>
        /// 获取仪器序列号
        /// </summary>
        /// <returns></returns>
        public override InstrumentInfo GetInstrumentInfo()
        {
            FTNirInterface.InstrumentInterface.InstrumentInfo info = new FTNirInterface.InstrumentInterface.InstrumentInfo();
            info.serialNumber=ExecuteGetOMNIC(GetAuthorBenchString("SerialNum"));

            return info;
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
            Type type = scPara.GetType();
            if (type == null || !System.IO.File.Exists(iniFilePath))
            {
                return false;
            }
            //加载扫描参数文件
            string command = "LoadParameters \"" + iniFilePath + "\" Collect " + GetAuthorBenchString("");
            if (ExecuteCommandOMNIC(command) == false)
                return false;
            foreach (var p in type.GetProperties())
            {
                switch (p.Name)
                {
                    case "resolution":
                        ExecuteSetOMNIC("Collect Resolution", p.GetValue(scPara, null).ToString());
                        break;
                    case "count":
                        ExecuteSetOMNIC("Collect NumScans", p.GetValue(scPara, null).ToString());
                        break;
                    case "xRegion":
                        var region=p.GetValue(scPara,null);
                        if(region!=null)
                        {
                            Type typeRegion=region.GetType();
                            foreach(var item in typeRegion.GetProperties())
                            {
                                switch(item.Name)
                                {
                                    case "minumum":
                                        ExecuteSetOMNIC(GetAuthorBenchString("LowCutoff"), p.GetValue(scPara, null).ToString());
                                        break;
                                    case "maxumum":
                                        ExecuteSetOMNIC(GetAuthorBenchString("HighCutoff"), p.GetValue(scPara, null).ToString());
                                        break;
                                    default:break;
                                }
                            }
                        }
                        break;
                    default: break;
                }
            }
            string str = "SaveParameters \"" + iniFilePath + "\" Collect " + GetAuthorBenchString("");
            if(!ExecuteCommandOMNIC(str))
            {
                return false;
            }
            return true;
            
        }
        
        /// <summary>
        /// 读取传感器信息
        /// </summary>
        /// <returns></returns>
        public override string ReadSensors()
        {
            return base.ReadSensors();
        }
    }
}
