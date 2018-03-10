using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrukerInstrument.Types
{
    public class Tango : InstrumentTypeBase
    {
        /// <summary>
        /// 获取激光波数
        /// </summary>
        /// <returns></returns>
        public override double? GetLaserWavelength( )
        {
            lock (thisLock)
            {
                //OpusCMD334.ReadParameter read = new OpusCMD334.ReadParameter();
                //read.ReadParm("LWN");
                //string htmlstr = DownloadWebPage(url);
                //if(htmlstr==null)return null;
                //HtmlAgilityPack.HtmlDocument dom = new HtmlAgilityPack.HtmlDocument();
                //dom.LoadHtml(htmlstr);
                ////HtmlAgilityPack.HtmlNode temperT = dom.GetElementbyId("TD");
                //HtmlAgilityPack.HtmlNodeCollection nodes = dom.DocumentNode.SelectNodes(@"//tbody//tr");
                //foreach (HtmlAgilityPack.HtmlNode node in nodes) 
                //{ 
                //}
                //return 1;
                lock (thisLock)
                {
                    Web web = new Web(IpAddress, instrumentModel);
                    web.ShowDialog();
                    if (web.laserWave == -1)
                    {
                        ErrorString = "Get Laser WaveNumber Error,Please try again!";
                        return null;
                    }
                    else
                    {
                        return web.laserWave;
                    }
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
                Web web = new Web(IpAddress, instrumentModel);
                web.ShowDialog();
                if (web.Temperature == -1)
                {
                    ErrorString = "Get Temperature Error,Please try again!";
                    return null;
                }
                else
                {
                    return targetPeak += web.Temperature * 0.0107 - 0.7;
                }
            }
        }

        /// <summary>
        /// 移动转轮
        /// </summary>
        /// <param name="position">0 = 打开, 1 = NG4滤光玻璃, 2 = 聚苯乙烯, 3 = 过滤网</param>
        /// <returns></returns>
        public override bool? MoveWheel(int position, string iniFilePath)
        {
            lock (thisLock)
            {
                if (!System.IO.File.Exists(iniFilePath))
                {
                    ErrorString = "file: \r\n" + iniFilePath + "\r\n is not exist!";
                    return null;
                }
                if (!LoadXPM(iniFilePath))
                {
                    return null;
                }

                OpusCMD334.WriteParameter writePara = new OpusCMD334.WriteParameter();
                //writePara.WriteParm("OPF", "Open", iniFilePath, false);
                bool result = false;
                switch (position)
                {
                    case 0: result = writePara.WriteParm("OPF", "Open", iniFilePath, false); break;
                    case 1: result = writePara.WriteParm("OPF", "Filter NG9", iniFilePath, false); break;
                    case 2: result = writePara.WriteParm("OPF", "Filter BRM2065", iniFilePath, false); break;
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
            }
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
            //if (!LoadXPM(scanMethodFile) || !System.IO.Path.HasExtension(File))
            //{
            //    return null;
            //}
            //if (IsScanBack)
            //{
            //    MoveWheel(0, scanMethodFile);
            //}
            //string specPath = File + ".0";
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
            //Ai.Hong.CommonLibrary.SpecFileFormatDouble spcData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            //if (IsScanBack)
            //{
            //    if (!command.Command("MeasureReference ({NSR = " + scanCount + "})", true))
            //    {
            //        ErrorString = command.ErrorDesc;
            //        return null;
            //    }
            //    OpusCMD334.SaveReference saveBack = new OpusCMD334.SaveReference();
            //    if (!saveBack.SaveReference(System.IO.Path.GetFileName(File), System.IO.Path.GetDirectoryName(File)))
            //    {
            //        ErrorString = saveBack.ErrorDesc;
            //        return null;
            //    }
            //    if (!unLoad.Unload(specPath))
            //    {
            //        ErrorString = unLoad.ErrorDesc;
            //        return null;
            //    }
            //    if (!spcData.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.Background))
            //    {
            //        ErrorString = spcData.ErrorString;
            //        return null;
            //    }
            //    Ai.Hong.CommonLibrary.SpecFileFormatDouble backData = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            //    backData.Parameter = spcData.Parameter;
            //    int indexX = Ai.Hong.SpectrumAlgorithm.SpectrumAlgorithm.FindNearestPosition(spcData.XDatas, 0, spcData.XDatas.Length - 1, 4000);
            //    int indexY = Ai.Hong.SpectrumAlgorithm.SpectrumAlgorithm.FindNearestPosition(spcData.XDatas, 0, spcData.XDatas.Length - 1, 12000);

            //    if (indexX != -1 && indexY != -1)
            //    {
            //        float[] tun = new float[Math.Abs(indexX - indexY) + 1];
            //        backData.XDatas = new double[tun.Length];
            //        backData.YDatas = new double[tun.Length];
            //        for (int i = indexX; i < indexY + 1; i++)
            //        {
            //            tun[i - indexX] = (float)spcData.XDatas[i];
            //            backData.XDatas[i - indexX] = spcData.XDatas[i];
            //            backData.YDatas[i - indexX] = spcData.YDatas[i];
            //        }
            //        backData.Parameter.dataCount = (uint)tun.Length;
            //        if (tun.Length > 0)
            //        {
            //            backData.Parameter.firstX = tun[0];
            //            backData.Parameter.lastX = tun[tun.Length - 1];
            //        }
            //        backData.Parameter.maxYValue = (from p in tun select p).ToList().Max();
            //        backData.Parameter.maxYValue = (from p in tun select p).ToList().Min();
            //        spcData = backData;
            //    }
            //}
            //else
            //{
            //    if (!command.Command("MeasureSample ({NSS = " + scanCount + "," + "PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
            //        "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'})", true))
            //    {
            //        ErrorString = command.ErrorDesc;
            //        return null;
            //    }
            //    specPath = specPath.Replace(".spc", string.Empty);
            //    if (!unLoad.Unload(specPath))
            //    {
            //        ErrorString = unLoad.ErrorDesc;
            //        return null;
            //    }
            //    if (!spcData.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.SingleBeam))
            //    {
            //        ErrorString = spcData.ErrorString;
            //        return null;
            //    }
            //}
            //float[] tr = new float[spcData.YDatas.Count()];
            //for (int i = 0; i < spcData.YDatas.Count(); i++)
            //{
            //    tr[i] = (float)spcData.YDatas[i];
            //}
            //Ai.Hong.CommonLibrary.SPCFile.SaveFile(File, tr, spcData.Parameter);

            ////System.IO.File.Move(specPath, File);
            //if (System.IO.File.Exists(specPath) && !string.Equals(specPath, File))
            //{
            //    unLoad.Unload(specPath);
            //    System.IO.File.Delete(specPath);
            //}
            //return File;
            if (!LoadXPM(scanMethodFile) || !System.IO.Path.HasExtension(File))
            {
                return null;
            }
            if (IsScanBack)
            {
                MoveWheel(0, scanMethodFile);
            }
            string specPath = File.Replace(".spc", ".0");
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

            Ai.Hong.CommonLibrary.SpecFileFormatDouble data = new Ai.Hong.CommonLibrary.SpecFileFormatDouble();
            if (IsScanBack)
            {
                string commandString = "MeasureReference ({PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
                    "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'" + addPara + " })";
                bool retcode = command.Command(commandString, true);
                string[] retstrs = command.CommandResult.Split('\n');
                if (retcode == false || retstrs.Length < 1 || retstrs[0] != "OK")
                {
                    ErrorString = command.ErrorDesc;
                    return null;
                }
                if (addPara != null)
                    addPara = addPara.Replace("NSR", "NSS");
                commandString = "MeasureSample ({PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
                    "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'" + addPara + "})";
                retcode = command.Command(commandString, true);
                retstrs = command.CommandResult.Split('\n');
                if (retcode == false || retstrs.Length < 4 || retstrs[0] != "OK")
                {
                    ErrorString = command.ErrorDesc;
                    return null;
                }
                UnloadFileByCommand(retstrs[3]);
                //if (!unLoad.Unload(specPath))
                //{
                //    ErrorString = unLoad.ErrorDesc;
                //    return null;
                //}
                if (!data.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.Background))
                {
                    ErrorString = data.ErrorString;
                    return null;
                }

            }
            else
            {
                string cmdstr = "MeasureSample ({PTH='" + System.IO.Path.GetDirectoryName(File) + "'," +
                    "NAM='" + System.IO.Path.GetFileNameWithoutExtension(File) + "'" + addPara + "})";
                if (!command.Command(cmdstr, true))
                {
                    ErrorString = command.ErrorDesc;
                    return null;
                }
                if (!unLoad.Unload(specPath))
                {
                    ErrorString = unLoad.ErrorDesc;
                    return null;
                }
                if (!data.ReadFile(specPath, Ai.Hong.CommonLibrary.SpecFileFormat.specType.SingleBeam))
                {
                    ErrorString = data.ErrorString;
                    return null;
                }
            }
            float[] tr = new float[data.YDatas.Count()];
            for (int i = 0; i < data.YDatas.Count(); i++)
            {
                tr[i] = (float)data.YDatas[i];
            }
            Ai.Hong.CommonLibrary.SPCFile.SaveFile(File, tr, data.Parameter);


            if (System.IO.File.Exists(specPath) && !string.Equals(specPath, File))
            {
                unLoad.Unload(specPath);
                System.IO.File.Delete(specPath);
            }
            return File;
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

        public override string SpecProcess(string filename, double firstX = 6900, double lastX = 7500)
        {
            if (filename == null || !System.IO.File.Exists(filename))
                return null;

            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();

            //加载文件
            string opusfile = LoadFileByCommand(filename);
            if (opusfile == null)
                return null;

            //剪切
            string cmdstr = string.Format("Cut ([{0}:Spec], {{CFX={1}, CLX={2}}});", opusfile, firstX, lastX);
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
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

            if (SaveFileByCommand(opusfile) == false || UnloadFileByCommand(opusfile) == false)
                return null;

            return filename;
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
            if (!command.Command("SendCommand(0, {UNI='xwn=" + write.ToString() + "@ ITC' })"))
            {
                ErrorString = command.ErrorDesc;
                return null;
            }
            return true;
        }

        /// <summary>
        /// 用Command保存OPUS中的文件
        /// </summary>
        /// <param name="opusFile"></param>
        /// <returns></returns>
        private  bool SaveFileByCommand(string opusFile)
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
        private  bool UnloadFileByCommand(string opusFile)
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
            bool IsNeedChange = false;
            if (sourceData.YDatas.Count() < destData.YDatas.Count())
            {
                IsNeedChange = true;
                string temp = sourceFile;
                sourceFile = destFile;
                destFile = temp;
            }
            string opusSrcFile = LoadFileByCommand(sourceFile);
            string opusDstFile = LoadFileByCommand(destFile);
            if (opusSrcFile == null || opusDstFile == null)
            {
                if (opusSrcFile != null)
                    UnloadFileByCommand(opusSrcFile);

                if (opusDstFile != null)
                    UnloadFileByCommand(opusDstFile);
                return null;
            }

            if (ChangeDataBlockType(opusSrcFile, "Spec", "AB") == false)
                return null;

            if (ChangeDataBlockType(opusDstFile, "Spec", "AB") == false)
                return null;

            //谱图兼容
            OpusCMD334.OpusCommand cmd = new OpusCMD334.OpusCommand();
            string cmdstr = string.Format("MakeCompatible ([{0}:Spec], [{1}:Spec], {{}});", opusDstFile, opusSrcFile);
            bool cmdcode = cmd.Command(cmdstr);
            string[] cmdrets = cmd.CommandResult.Split('\n');
            if (cmdcode == false || cmdrets.Length < 4 || cmdrets[0] != "OK")
                return null;



            //SaveAsSPC(opusSrcFile, sourceFile);
            //SaveAsSPC(opusDstFile, destFile);

            //
            if (SaveFileByCommand(opusSrcFile) == false ||
                SaveFileByCommand(opusDstFile) == false ||
                UnloadFileByCommand(opusSrcFile) == false ||
                UnloadFileByCommand(opusDstFile) == false)
                return null;
            if (IsNeedChange)
            {
                string temp = sourceFile;
                sourceFile = destFile;
                destFile = temp;
            }
            return sourceFile;
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
