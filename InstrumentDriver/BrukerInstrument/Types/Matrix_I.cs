using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTNirInterface.WaitDialog;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace BrukerInstrument.Types
{
    public class Matrix_I : InstrumentTypeBase
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        static Window GetTopWindow()
        {
            var hwnd = GetForegroundWindow();
            if (hwnd == null)
                return null;
            return (Window)HwndSource.FromHwnd(hwnd).RootVisual;

        }
        /// <summary>
        /// 获取激光波数
        /// </summary>
        /// <returns></returns>
        public override double? GetLaserWavelength()
        {
            lock (thisLock)
            {
                try
                {
                    double lwn = 0;
                    //return lwn;
                    string htmlstr = DownloadWebPage("http://" + IpAddress + "/config/report.htm");
                    if (htmlstr == null) return null;
                    HtmlAgilityPack.HtmlDocument dom = new HtmlAgilityPack.HtmlDocument();
                    dom.LoadHtml(htmlstr);
                    //HtmlAgilityPack.HtmlNode temperT = dom.GetElementbyId("TD");
                    HtmlAgilityPack.HtmlNodeCollection nodes = dom.DocumentNode.SelectNodes(@"//tbody//tr");

                    HtmlAgilityPack.HtmlNodeCollection nodeCollection = dom.DocumentNode.SelectNodes(@"//body//fieldset//table");
                    if (nodeCollection != null)
                    {
                        var item = nodeCollection.FirstOrDefault(p => p.InnerText.Contains("Additional parameters for Cmds"));// && p.InnerText.Contains("ITC*"));
                        if (item != null)
                        {
                            var ip = item.ParentNode.ChildNodes.Last().ChildNodes.FirstOrDefault(p => p.InnerText.Contains("ITC*"));
                            if (ip != null)
                            {
                                var ite = ip.InnerText.Trim().Split('\n').FirstOrDefault(p => p.Contains("ITC"));
                                if (ite != null)
                                {
                                    var regex = new System.Text.RegularExpressions.Regex(@"\d*\.\d*|0\.\d*[1-9]\d*$");
                                    //string[] result = new string[] { regex.Match(ite).Value, regex.Replace(ite, "")};
                                    if (!double.TryParse(regex.Match(ite).Value, out lwn))
                                    {
                                        ErrorString = "Read LWN Error";
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                    return lwn;

                    //HtmlAgilityPack.HtmlEntity

                }
                catch
                {
                    ErrorString = "Read LWN Error";
                    return null;
                }
            }
        }

        /// <summary>
        /// 波数精度-聚苯乙烯-温度校正
        /// </summary>
        /// <param name="targetPeak"></param>
        /// <returns></returns>
        public override double? TemperatureCalibrate(double targetPeak)
        {
            lock (thisLock)
            {
                if (targetPeak < 0)
                {
                    return null;
                }
                string webString = DownloadWebPage("http://" + IpAddress + "/config/report.htm");
                if (webString == null)
                {
                    ErrorString = "Get Web Page Error,Please try again !";
                    return null;
                }
                bool result = false;
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(webString);
                HtmlAgilityPack.HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes(@"//body//fieldset//tr");
                var nodes = (from p in nodeCollection where p.InnerText.Contains("Scannerblock Temperature") select p).FirstOrDefault();
                if (nodes == null)
                {
                    result = true;
                }
                string temper = nodes.ChildNodes[1].InnerText;// nodeCollection[nodeCollection.IndexOf(nodes) + 1].InnerText;
                double temperature = 0;
                try
                {
                    temperature = Convert.ToDouble(temper);
                }
                catch
                {
                    result = true;
                }
                if (result)
                {
                    ErrorString = "Get Temperature Error,Please try again!";
                    return null;
                }
                else
                {
                    return targetPeak += temperature * 0.0107 - 0.7;
                }
            }
        }

        /// <summary>
        /// 写入激光波数到仪器
        /// </summary>
        /// <param name="waveNum"></param>
        /// <returns></returns>
        public override bool? SetLaserWavelength(double curPeak, double targetPeak, ref double curLaser)
        {
            double? curLwn = GetLaserWavelength();
            if (!curLwn.HasValue)
                return null;
            curLaser = 10000 / (double)curLwn * curPeak / targetPeak;
            double write = 10000 / curLaser;
            OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            if (!command.Command("SendCommand(0, {UNI='xwn=" + write.ToString() + "@\"' })"))
            {
                ErrorString = command.ErrorDesc;
                return null;
            }
            bool? result = null;
            //重启仪器
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                new WebBrowser().Navigate("http://" + IpAddress + "/config/resetpage.htm?Sub=RESET+INSTRUMENT");
                Thread.Sleep(3000);
                //等待仪器重启
                ProcessWaitDialog waitDialog = new ProcessWaitDialog((ProcessWaitDialog.SetProcessAndMsgDeletage callBack) =>
                {
                    int i = 0;
                    string str = "";
                    while (true)
                    {
                        i++;
                        for (int j = 0; j <= i % 6; j++) str += ".";
                        callBack("Please Wait", int.MaxValue, int.MaxValue, "Reset Instrument Now" + str, false);
                        var html = DownloadWebPage("http://" + IpAddress + "/config/report.htm");
                        if (!string.IsNullOrEmpty(html)) break;
                        System.Threading.Thread.Sleep(500);
                        str = "";
                    }
                    return true;
                });

                if (Application.Current.Windows.Count > 1)
                {
                    waitDialog.Owner = Application.Current.Windows[Application.Current.Windows.Count - 2];
                }
                else
                {
                    waitDialog.Owner = System.Windows.Application.Current.MainWindow;
                }
                result = waitDialog.ShowDialog() == true;
            }));
            return result;
        }

        /// <summary>
        /// 移动转轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <returns></returns>
        public override bool? MoveWheel(int position, string iniFilePath)
        {
            //lock (thisLock)
            //{
            if (!System.IO.File.Exists(iniFilePath))
            {
                ErrorString = "file: \r\n" + iniFilePath + "\r\n is not exist!";
                return null;
            }
            if (!LoadXPM(iniFilePath))
            {
                return null;
            }
            OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            OpusCMD334.WriteParameter writePara = new OpusCMD334.WriteParameter();

            bool result = false;
            // string para = "apt", value = "3";

            switch (position)
            {
                //APT= 1(BRM2065) 2(Filter NG9) 3(Filter NG4) 4(Filter NG11） 5(Reference Spectralon) 10(Open)
                case 0:
                    //result=command.Command("SendCommand (0, {UNI='apt=10'}");
                    result = writePara.WriteParm("APT", "Open", iniFilePath, false);
                    //result = command.Command("SendCommand (0, {UNI='apt=10'}");
                    break;//OPEN
                case 1:
                    //result = command.Command("SendCommand (0, {UNI='apt=3'}");
                    result = writePara.WriteParm("APT", "Filter NG4", iniFilePath, false);
                    //result = command.Command("SendCommand (0, {UNI='apt=3'}");
                    break;//Filter NG4
                case 2:
                    //result = command.Command("SendCommand (0, {UNI='apt=1'}");
                    result = writePara.WriteParm("APT", "Filter BRM2065", iniFilePath, false);
                    //result = command.Command("SendCommand (0, {UNI='apt=1'}");
                    break;//BRM2065
                default: result = true; break;
            }
            if (!result)
            {
                ErrorString = writePara.ErrorDesc;
            }
            if (!LoadXPM(iniFilePath))
            {
                return null;
            }
            return result;
            //}
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
            return ScanSpec(scanMethodFile, scanCount, backgroundFile, true, addPara);
        }

        private string ScanSpec(string scanMethodFile, int scanCount, string File, bool IsScanBack, string addPara = "")
        {
            if (!LoadXPM(scanMethodFile) || !System.IO.Path.HasExtension(File))
            {
                return null;
            }
            if (IsScanBack)
            {
                MoveWheel(0, scanMethodFile);
            }
            string specPath = File + ".0";
            if (System.IO.File.Exists(specPath))
            {
                OpusCMD334.UnloadFile unLoadFile = new OpusCMD334.UnloadFile();
                unLoadFile.Unload(specPath);
                System.IO.File.Delete(specPath);
            }
            if (System.IO.File.Exists(File))
            {
                System.IO.File.Delete(File);
            }

            OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            OpusCMD334.UnloadFile unLoad = new OpusCMD334.UnloadFile();
            Ai.Hong.CommonLibrary.SpecFileFormatDouble spcData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (IsScanBack)
            {
                string para = addPara;
                if (addPara == null || (addPara != null && !addPara.Contains("HFQ") && !addPara.Contains("LFQ")))
                {
                    //OpusCMD334.ReadParameter read = new OpusCMD334.ReadParameter();
                    //if (!read.ReadParm("HFQ", scanMethodFile))
                    //{
                    //    ErrorString = "Read HFQ Error! Please Restart System";
                    //    return null;
                    //}
                    //para += ",HFQ=" + read.Value;
                    //if (!read.ReadParm("LFQ", scanMethodFile))
                    //{
                    //    ErrorString = "Read LFQ Error! Please Restart System";
                    //    return null;
                    //}
                    //para += ",LFQ=" + read.Value;
                    para += ",HFQ=4000,LFQ=12000";//默认为4000-12000

                }
                spcData = ScanningBack(scanCount, specPath, File, para);
            }
            else
            {
                spcData = ScanningSample(scanCount, specPath, File, addPara);
                //if (spcData == null)
                //{
                //    ScanningBack(scanCount, specPath, File);
                //    spcData = ScanningSample(scanCount, specPath, File, addPara);
                //}

            }
            if (spcData == null)
            {
                ErrorString = "Scan Error";
                return null;
            }
            float[] tr = new float[spcData.YDatas.Count()];
            for (int i = 0; i < spcData.YDatas.Count(); i++)
            {
                tr[i] = (float)spcData.YDatas[i];
            }
            Ai.Hong.CommonLibrary.SPCFile.SaveFile(File, tr, spcData.Parameter);

            //System.IO.File.Move(specPath, File);
            if (System.IO.File.Exists(specPath) && !string.Equals(specPath, File))
            {
                unLoad.Unload(specPath);
                System.IO.File.Delete(specPath);
            }
            return File;
            //if (!LoadXPM(scanMethodFile) || !System.IO.Path.HasExtension(File))
            //{
            //    return null;
            //}
            //if (IsScanBack)
            //{
            //    MoveWheel(0, scanMethodFile);
            //}
            //string specPath = File.Replace(".spc", ".0");
            //if (System.IO.File.Exists(specPath))
            //{
            //    OpusCMD334.UnloadFile unLoadFile = new OpusCMD334.UnloadFile();
            //    unLoadFile.Unload(specPath);
            //    System.IO.File.Delete(specPath);
            //}
            //if (System.IO.File.Exists(File))
            //{
            //    System.IO.File.Delete(File);
            //}

            //OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            //OpusCMD334.UnloadFile unLoad = new OpusCMD334.UnloadFile();

            //Ai.Hong.CommonLibrary.SpecFileFormatDouble data = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            //if (IsScanBack)
            //{
            //    string commandString = "MeasureReference ({PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
            //        "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'" + addPara + " })";
            //    bool retcode = command.Command(commandString, true);
            //    string[] retstrs = command.CommandResult.Split('\n');
            //    if (retcode == false || retstrs.Length < 1 || retstrs[0] != "OK")
            //    {
            //        ErrorString = command.ErrorDesc;
            //        return null;
            //    }
            //    if (addPara != null)
            //        addPara = addPara.Replace("NSR", "NSS");
            //    commandString = "MeasureSample ({PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
            //        "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'" + addPara + "})";
            //    retcode = command.Command(commandString, true);
            //    retstrs = command.CommandResult.Split('\n');
            //    if (retcode == false || retstrs.Length < 4 || retstrs[0] != "OK")
            //    {
            //        ErrorString = command.ErrorDesc;
            //        return null;
            //    }
            //    UnloadFileByCommand(retstrs[3]);
            //    //if (!unLoad.Unload(specPath))
            //    //{
            //    //    ErrorString = unLoad.ErrorDesc;
            //    //    return null;
            //    //}
            //    if (!data.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.Background))
            //    {
            //        ErrorString = data.ErrorString;
            //        return null;
            //    }

            //}
            //else
            //{
            //    string cmdstr = "MeasureSample ({PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
            //        "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'" + addPara + "})";
            //    if (!command.Command(cmdstr, true))
            //    {
            //        ErrorString = command.ErrorDesc;
            //        return null;
            //    }
            //    if (!unLoad.Unload(specPath))
            //    {
            //        ErrorString = unLoad.ErrorDesc;
            //        return null;
            //    }
            //    if (!data.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.SingleBeam))
            //    {
            //        ErrorString = data.ErrorString;
            //        return null;
            //    }
            //}
            //float[] tr = new float[data.YDatas.Count()];
            //for (int i = 0; i < data.YDatas.Count(); i++)
            //{
            //    tr[i] = (float)data.YDatas[i];
            //}
            //Ai.Hong.CommonLibrary.SPCFile.SaveFile(File, tr, data.Parameter);


            //if (System.IO.File.Exists(specPath) && !string.Equals(specPath, File))
            //{
            //    unLoad.Unload(specPath);
            //    System.IO.File.Delete(specPath);
            //}
            //return File;
        }

        private Ai.Hong.CommonLibrary.SpecFileFormatDouble ScanningBack(int scanCount, string specPath, string File, string addPara)
        {
            OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            OpusCMD334.UnloadFile unLoad = new OpusCMD334.UnloadFile();
            Ai.Hong.CommonLibrary.SpecFileFormatDouble spcData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();

            string commandStr = "MeasureReference ({NSR = " + scanCount + ",APT='Open' " + addPara + "})";
            if (scanCount == 0)
            {
                commandStr = commandStr.Replace("NSR = " + scanCount + ",", string.Empty);
            }
            if (!command.Command(commandStr, true))
            {
                ErrorString = command.ErrorDesc;
                return null;
            }
            OpusCMD334.SaveReference saveBack = new OpusCMD334.SaveReference();
            if (!saveBack.SaveReference(System.IO.Path.GetFileName(File), System.IO.Path.GetDirectoryName(File)))
            {
                ErrorString = saveBack.ErrorDesc;
                return null;
            }
            if (!unLoad.Unload(specPath))
            {
                ErrorString = unLoad.ErrorDesc;
                return null;
            }
            if (!spcData.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.Background))
            {
                ErrorString = spcData.ErrorString;
                return null;
            }
            if (addPara != null && addPara.Contains("HFQ") && addPara.Contains("LFQ"))
            {
                try
                {
                    string[] temp = addPara.Split(',');
                    string fxString = (from p in temp where p.Contains("HFQ") select p).First().Trim().Replace("HFQ=", string.Empty);
                    string lxString = (from p in temp where p.Contains("LFQ") select p).First().Trim().Replace("LFQ=", string.Empty);
                    Ai.Hong.CommonLibrary.SpecFileFormatDouble backData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
                    backData.Parameter = spcData.Parameter;
                    int indexX = Ai.Hong.SpectrumAlgorithm.SpectrumAlgorithm.FindNearestPosition(spcData.XDatas, 0, spcData.XDatas.Length - 1, Convert.ToInt32(fxString));
                    int indexY = Ai.Hong.SpectrumAlgorithm.SpectrumAlgorithm.FindNearestPosition(spcData.XDatas, 0, spcData.XDatas.Length - 1, Convert.ToInt32(lxString));

                    if (indexX != -1 && indexY != -1)
                    {
                        float[] tun = new float[Math.Abs(indexX - indexY) + 1];
                        backData.XDatas = new double[tun.Length];
                        backData.YDatas = new double[tun.Length];
                        for (int i = indexX; i < indexY + 1; i++)
                        {
                            tun[i - indexX] = (float)spcData.XDatas[i];
                            backData.XDatas[i - indexX] = spcData.XDatas[i];
                            backData.YDatas[i - indexX] = spcData.YDatas[i];
                        }
                        backData.Parameter.dataCount = (uint)tun.Length;
                        if (tun.Length > 0)
                        {
                            backData.Parameter.firstX = tun[0];
                            backData.Parameter.lastX = tun[tun.Length - 1];
                        }
                        backData.Parameter.maxYValue = (from p in tun select p).ToList().Max();
                        backData.Parameter.maxYValue = (from p in tun select p).ToList().Min();
                        spcData = backData;
                    }
                }
                catch { }
            }
            //Ai.Hong.CommonLibrary.SpecFileFormatDouble backData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            //backData.Parameter = spcData.Parameter;
            //int indexX = Ai.Hong.SpectrumAlgorithm.SpectrumAlgorithm.FindNearestPosition(spcData.XDatas, 0, spcData.XDatas.Length - 1, 4000);
            //int indexY = Ai.Hong.SpectrumAlgorithm.SpectrumAlgorithm.FindNearestPosition(spcData.XDatas, 0, spcData.XDatas.Length - 1, 12000);

            //if (indexX != -1 && indexY != -1)
            //{
            //    float[] tun = new float[Math.Abs(indexX - indexY) + 1];
            //    backData.XDatas = new double[tun.Length];
            //    backData.YDatas = new double[tun.Length];
            //    for (int i = indexX; i < indexY + 1; i++)
            //    {
            //        tun[i - indexX] = (float)spcData.XDatas[i];
            //        backData.XDatas[i - indexX] = spcData.XDatas[i];
            //        backData.YDatas[i - indexX] = spcData.YDatas[i];
            //    }
            //    backData.Parameter.dataCount = (uint)tun.Length;
            //    if (tun.Length > 0)
            //    {
            //        backData.Parameter.firstX = tun[0];
            //        backData.Parameter.lastX = tun[tun.Length - 1];
            //    }
            //    backData.Parameter.maxYValue = (from p in tun select p).ToList().Max();
            //    backData.Parameter.maxYValue = (from p in tun select p).ToList().Min();
            //    spcData = backData;
            //}
            return spcData;
        }

        private Ai.Hong.CommonLibrary.SpecFileFormatDouble ScanningSample(int scanCount, string specPath, string File, string addPara)
        {
            OpusCMD334.OpusCommand command = new OpusCMD334.OpusCommand();
            OpusCMD334.UnloadFile unLoad = new OpusCMD334.UnloadFile();
            Ai.Hong.CommonLibrary.SpecFileFormatDouble spcData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();

            string commandStr = "MeasureSample ({NSS = " + scanCount + "," + "PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
                "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'" + addPara + "})";

            if (scanCount == 0)
            {
                commandStr = commandStr.Replace("NSS = " + scanCount + ",", string.Empty);
            }

            if (!command.Command(commandStr, true))
            {
                ErrorString = command.ErrorDesc;
                return null;
            }
            specPath = specPath.Replace(".spc", string.Empty);
            if (!unLoad.Unload(specPath))
            {
                ErrorString = unLoad.ErrorDesc;
                return null;
            }
            if (!spcData.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.SingleBeam))
            {
                ErrorString = spcData.ErrorString;
                return null;
            }
            // System.Windows.MessageBox.Show(spcData.Parameter.)
            return spcData;
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
            return ScanSpec(scanMethodFile, scanCount, sampleFile, false, addPara);
        }



        /// <summary>
        /// 用Command保存OPUS中的文件
        /// </summary>
        /// <param name="opusFile"></param>
        /// <returns></returns>
        private bool SaveFileByCommand(string opusFile)
        {
            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();

            string cmdstr = string.Format("Save ([{0}:Spec], {{}});", opusFile);
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 3 || cmdrets[0] != "OK")
                return false;

            return true;
        }

        /// <summary>
        /// 通过Command加载文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>加载到OPUS中的文件名</returns>
        private static string LoadFileByCommand(string filename)
        {
            if (filename == null || !System.IO.File.Exists(filename))
                return null;

            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();
            //加载文件
            string cmdstr = string.Format(" Load (, {{COF=0, DAP='{0}', DAF='{1}'}});", System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileName(filename));
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 4 || cmdrets[0] != "OK")
                return null;

            return cmdrets[3];
        }

        /// <summary>
        /// 通过OPUS Command卸载文件
        /// </summary>
        /// <param name="opusFile"></param>
        /// <returns></returns>
        private bool UnloadFileByCommand(string opusFile)
        {
            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();

            //卸载文件
            string cmdstr = string.Format("Unload ([{0}:Spec], {{}});", opusFile);
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 1 || cmdrets[0] != "OK")
                return false;

            return true;
        }

        /// <summary>
        /// 光谱兼容处理
        /// </summary>
        /// <param name="sourceFile">需要处理的光谱</param>
        /// <param name="destFile">标准光谱</param>
        /// <returns></returns>
        public override string MakeCompatiable(string sourceFile, string destFile)
        {
            Ai.Hong.CommonLibrary.SpecFileFormatDouble sourceData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (!sourceData.ReadFile(sourceFile))
            {
                return null;
            }
            Ai.Hong.CommonLibrary.SpecFileFormatDouble destData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (!destData.ReadFile(destFile))
            {
                return null;
            }
            try
            {
                if (sourceData.YDatas.Count() == destData.YDatas.Count()) return sourceFile;
                double step = destData.XDatas[1] - destData.XDatas[0];
                double[] newXDatas = null;

                double orgfirstx = destData.Parameter.firstX;
                double orglastx = destData.Parameter.lastX;
                //以DestFile为标准拟合dest
                destData.YDatas = SpectrumStandardizingFit(destData, orgfirstx, orglastx, step, out newXDatas);
                destData.XDatas = newXDatas;
                destData.Parameter.firstX = newXDatas.Min();
                destData.Parameter.lastX = newXDatas.Max();
                destData.Parameter.minYValue = destData.YDatas.Min();
                destData.Parameter.maxYValue = destData.YDatas.Max();
                destData.Parameter.dataCount = (uint)destData.YDatas.Count();
                //以DestFile为标准拟合source
                sourceData.YDatas = SpectrumStandardizingFit(sourceData, orgfirstx, orglastx, step, out newXDatas);
                sourceData.XDatas = newXDatas;
                sourceData.Parameter.firstX = newXDatas.Min();
                sourceData.Parameter.lastX = newXDatas.Max();
                sourceData.Parameter.minYValue = sourceData.YDatas.Min();
                sourceData.Parameter.maxYValue = sourceData.YDatas.Max();
                sourceData.Parameter.dataCount = (uint)sourceData.YDatas.Count();

                float[] floatData = new float[sourceData.YDatas.Count()];
                for (int i = 0; i < destData.YDatas.Count(); i++)
                {
                    floatData[i] = (float)destData.YDatas[i];
                }
                Ai.Hong.CommonLibrary.SPCFile.SaveFile(destFile, floatData, destData.Parameter);

                for (int i = 0; i < sourceData.YDatas.Count(); i++)
                {
                    floatData[i] = (float)sourceData.YDatas[i];
                }
                Ai.Hong.CommonLibrary.SPCFile.SaveFile(sourceFile, floatData, sourceData.Parameter);
            }
            catch { }
            return sourceFile;
        }

        /// <summary>
        /// 用新的参数拟合光谱
        /// </summary>
        /// <param name="fileData">原有光谱文件数据</param>
        /// <param name="xDatas">拟合后的X轴数据</param>
        /// <returns>拟合后的Y轴数据</returns>
        public static double[] SpectrumStandardizingFit(Ai.Hong.CommonLibrary.SpecFileFormatDouble fileData, double firstx, double lastx, double stepx, out double[] xDatas)
        {
            try
            {
                if (fileData == null || fileData.XDatas == null || fileData.YDatas == null ||
                    fileData.XDatas.Length == 0 || fileData.XDatas.Length != fileData.YDatas.Length)
                    throw new Exception("原始光谱读取错误");

                if ((stepx > 0 && lastx < firstx + stepx) || (stepx < 0 && lastx > firstx + stepx))
                    throw new Exception("拟合参数错误");

                //对转换为波数的nirstR.spc进行三次样条曲线拟合
                alglib.spline1dinterpolant c;
                alglib.spline1dbuildcubic(fileData.XDatas, fileData.YDatas, out c);

                //按照当前X轴校正后的坐标对Y轴重新插值
                int datacount = (int)((lastx - firstx) / stepx);
                xDatas = new double[datacount];

                double[] yDatas = new double[datacount];
                for (int i = 0; i < datacount; i++)
                {
                    xDatas[i] = firstx + i * stepx;
                    yDatas[i] = alglib.spline1dcalc(c, xDatas[i]);
                }

                return yDatas;

            }
            catch (Exception ex)
            {
                xDatas = null;
                ErrorString = ex.Message;
                return null;
            }
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
            //OpusCMD334.ReadParameter read = new OpusCMD334.ReadParameter();
            //read.ReadParm("FXV");
            //read.ReadParm("LXV");
            //加载XPM扫描参数文件
            if (LoadXPM(iniFilePath) == false)
                return false;
            OpusCMD334.WriteParameter writePara = new OpusCMD334.WriteParameter();
            foreach (var p in type.GetProperties())
            {
                if (string.Equals(p.Name, "xRegion"))
                {
                    ModifyIniFile(p.GetValue(scPara, null), iniFilePath);
                }
                switch (p.Name)
                {
                    case "resolution":
                        if (!writePara.WriteParm("RES", p.GetValue(scPara, null).ToString(), iniFilePath, false))
                        {
                            ErrorString = writePara.ErrorDesc;
                            return false;
                        }
                        break;
                    case "count":
                        if (!writePara.WriteParm("NSS", p.GetValue(scPara, null).ToString(), iniFilePath, false))
                        {
                            ErrorString = writePara.ErrorDesc;
                            return false;
                        }
                        if (!writePara.WriteParm("NSR", p.GetValue(scPara, null).ToString(), iniFilePath, false))
                        {
                            ErrorString = writePara.ErrorDesc;
                            return false;
                        }
                        break;
                    case "minumum":
                        if (!writePara.WriteParm("HFQ", p.GetValue(scPara, null).ToString(), iniFilePath, false))
                        {
                            ErrorString = writePara.ErrorDesc;
                            return false;
                        }
                        break;
                    case "maxumum":
                        if (!writePara.WriteParm("LFQ", p.GetValue(scPara, null).ToString(), iniFilePath, false))
                        {
                            ErrorString = writePara.ErrorDesc;
                            return false;
                        }
                        break;
                    default: break;
                }
            }
            return true;

        }

        /// <summary>
        /// 获取扫描参数
        /// </summary>
        /// <param name="iniPath">配置文件路径</param>
        public override Dictionary<string, string> ReadScanPara(string xpmPath)
        {
            lock (thisLock)
            {
                //加载配置文件
                if (!LoadXPM(xpmPath))
                {
                    return null;
                }
                Dictionary<string, string> para = new Dictionary<string, string>();
                OpusCMD334.ReadParameter readPara = new OpusCMD334.ReadParameter();
                if (readPara.ReadParm("RES"))
                {
                    para.Add("resolution", readPara.Value);
                }
                if (readPara.ReadParm("NSS"))//样品扫描次数
                {
                    para.Add("scans", readPara.Value);
                }
                if (readPara.ReadParm("ASG"))//PGN
                {
                    int temp;
                    if (int.TryParse(readPara.Value, out temp))
                    {
                        if (temp == -1)
                        {
                            para.Add("gain", "Automatic");
                        }
                        else
                        {
                            para.Add("gain", Math.Pow(2, temp).ToString());
                        }
                    }
                }
                if (readPara.ReadParm("ZFF"))
                {
                    para.Add("zeroFill", readPara.Value);
                }
                if (readPara.ReadParm("PHZ"))
                {
                    para.Add("phaseCorrection", readPara.Value);
                }
                if (readPara.ReadParm("FTA"))
                {
                    para.Add("apodization", readPara.Value);
                }
                if (readPara.ReadParm("VEL"))
                {
                    para.Add("velocity", readPara.Value);
                }
                return para;
            }
        }
    }
}
