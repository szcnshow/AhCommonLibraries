using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ai.Hong.FileFormat
{
    class JCAMPFileFormat
    {
        public static string ErrorString { get; set; }

        //格式：##RESOLUTION=16
        /// <summary>
        /// 填充数据格式
        /// </summary>
        /// <param name="Para">数据格式</param>
        /// <param name="property">内容</param>
        private static bool FillParameter(FileFormat.DataInfo Para, string[] property)
        {
            switch (property[0])
            {
                case "##FIRSTX":
                    double.TryParse(property[1], out Para.firstX);
                    break;
                case "##LASTX":
                    double.TryParse(property[1], out Para.lastX);
                    break;
                case "##MAXY":
                    double.TryParse(property[1], out Para.maxYValue);
                    break;
                case "##MINY":
                    double.TryParse(property[1], out Para.minYValue);
                    break;
                case "##YUNITS":        //谱图类型(吸收谱，干涉谱...)
                    {
                        switch(property[1])
                        {
                            case "TRANSMITTANCE":
                                Para.dataType = FileFormat.YAXISTYPE.YTRANS;
                                break;
                            case "REFLECTANCE":
                                Para.dataType = FileFormat.YAXISTYPE.YREFLEC;
                                break;
                            case "ABSORBANCE":
                                Para.dataType = FileFormat.YAXISTYPE.YABSRB;
                                break;
                            case  "KUBELKA-MONK":
                                Para.dataType = FileFormat.YAXISTYPE.YKMONK;
                                break;
                            case "ARBITRARY UNITS":
                                Para.dataType = FileFormat.YAXISTYPE.YARB;
                                break;
                            default:
                                Para.dataType = FileFormat.YAXISTYPE.YARB;
                                break;
                        }
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 读取Y轴数据
        /// </summary>
        /// <param name="reader">文件流</param>
        /// <param name="xyType">数据格式</param>
        /// <param name="dataCount">数量数量</param>
        /// <param name="yScale">Y轴放大系数</param>
        /// <param name="yDatas">out Y轴值</param>
        /// <returns>返回最后读取的字符串(##END=)，错误返回null</returns>
        private static string ReadXYData(StreamReader reader, string xyType, int dataCount, double yScale, out double[] yDatas)
        {
            yDatas = null;

            try
            {
                if (dataCount == 0)
                    throw new Exception("Invalid format");

                yDatas = new double[dataCount];

                //分析数据是按空格还是+分隔
                string curLine = reader.ReadLine();
                char splitChar;
                if (curLine.IndexOf('+') > 0)
                    splitChar = '+';
                else if (curLine.IndexOf(' ') > 0)
                    splitChar = ' ';
                else
                    throw new Exception("Invalid format");

                int dataIndex = 0;
                while (curLine != null)
                {
                    if (curLine.Length < 1 || curLine[0] == '#')    //通常会是##END
                        break;

                    curLine = curLine.Trim();
                    string[] datas = curLine.Split(splitChar);
                    switch (xyType)
                    {
                        case "(X++(Y..Y))":  //一个X后面跟多个Y
                            //X数据不用处理

                            for (int i = 1; i < datas.Length; i++)
                            {
                                if (dataIndex >= yDatas.Length)
                                    break;

                                if (double.TryParse(datas[i], out yDatas[dataIndex]) == false)
                                    throw new Exception("Invalid format");
                                dataIndex++;
                            }
                            break;
                        case "(XY..XY)":    //一个X后面跟一个Y,数据肯定是双数
                            if ((datas.Length % 2) != 0)
                                throw new Exception("Invalid format");

                            for (int i = 1; i < datas.Length / 2; i++)
                            {
                                if (dataIndex >= yDatas.Length)
                                    break;

                                if (double.TryParse(datas[i * 2 + 1], out yDatas[dataIndex]) == false)
                                    throw new Exception("Invalid format");

                                dataIndex++;
                            }
                            break;
                        default:
                            throw new Exception("Invalid format");
                    }
                    curLine = reader.ReadLine();    //读下一行
                }

                //按照放大系数转换YData
                for (int i = 0; i < yDatas.Length; i++)
                {
                    yDatas[i] *= yScale;
                }

                return curLine;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 读取一个数据节
        /// </summary>
        /// <param name="dxFile">文件流</param>
        /// <param name="fileData">光谱格式数据</param>
        /// <param name="dataInfo">out 数据信息</param>
        /// <param name="yDatas">out Y轴数据</param>
        /// <returns>返回最后一个字符串，错误返回null</returns>
        private static string ReadOneBlock(System.IO.StreamReader dxFile, FileFormat fileData, out FileFormat.DataInfo dataInfo, out double[] yDatas)
        {
            dataInfo = new FileFormat.DataInfo();
            yDatas = null;

            if (fileData == null)
                return null;

            if (fileData.fileInfo == null)
                fileData.fileInfo = new FileFormat.FileInfo();
            if (fileData.acquisitionInfo == null)
                fileData.acquisitionInfo = new FileFormat.AcquisitionInfo();

            var fileInfo = fileData.fileInfo;
            var acqInfo = fileData.acquisitionInfo;

            string curline = null;
            try
            {
                curline = dxFile.ReadLine();
                if (curline == null)
                    throw new Exception("Invalid file format");

                //一个Block的第二行肯定是##JCAMP_DX
                curline = curline.ToUpper();
                if (curline.IndexOf("##JCAMP_DX") != 0 && curline.IndexOf("##JCAMP-DX") != 0)
                    throw new Exception("Invalid file format");

                curline = dxFile.ReadLine();
                if (curline == null)
                    throw new Exception("Invalid file format");
                curline = curline.ToUpper();

                DateTime filetime;
                int dataCount = 0;
                double tempdouble;
                double yfactor = 1;
                double xfactor = 1;
                while (curline != null && curline.IndexOf("##END=") != 0 && curline.IndexOf("##TITLE=") != 0)
                {
                    string[] property = curline.Split('=');
                    if (property.Length == 2)   //必须有值才处理
                    {
                        property[0] = property[0].Trim();
                        property[1] = property[1].Trim();
                        switch (property[0])
                        {
                            case "##FIRSTX":
                            case "##LASTX":
                            case "##MAXY":
                            case "##MINY":
                            case "##YUNITS":        //谱图类型(吸收谱，干涉谱...)
                                if (FillParameter(dataInfo, property) == false)
                                    throw new Exception("Invalid file format");
                                break;
                            case "##DATE":  //标准格式是YY/MM/DD, 但OPUS存的是DD/MM/YYYY
                                if (DateTime.TryParse(property[1], out filetime))
                                {
                                    fileInfo.createTime = filetime;
                                }
                                break;
                            case "##TIME":
                                if (DateTime.TryParse(property[1], out filetime))
                                {
                                    fileInfo.createTime = fileInfo.createTime.AddHours(filetime.Hour);
                                    fileInfo.createTime = fileInfo.createTime.AddMinutes(filetime.Minute);
                                    fileInfo.createTime = fileInfo.createTime.AddSeconds(filetime.Second);
                                }
                                break;
                            case "##DATA TYPE":     //光谱的类型(红外，拉曼...)
                                switch (property[1])
                                {
                                    case "INFRARED SPECTRUM":
                                        fileInfo.specType = FileFormat.SPECTYPE.SPCNIR;
                                        break;
                                    case "RAMAN SPECTRUM":
                                        fileInfo.specType = FileFormat.SPECTYPE.SPCRMN;
                                        break;
                                }
                                break;
                            case "##NPOINTS":
                                int.TryParse(property[1], out dataCount);
                                break;
                            case "##XFACTOR":   //X数据放大系数
                                double.TryParse(property[1], out xfactor);
                                break;
                            case "##YFACTOR":   //Y数据放大系数
                                double.TryParse(property[1], out yfactor);
                                break;
                            case "##RESOLUTION":
                                if (double.TryParse(property[1], out tempdouble))
                                    fileInfo.resolution = tempdouble;
                                break;
                            case "##ORIGIN":
                            case "##OWNER":
                            case "##CLASS":
                            case "##SAMPLE DESCRIPTION":
                            case "##SPECTROMETER/DATA SYSTEM":
                            case "##INSTRUMENTAL PARAMETERS":
                            case "##DATA PROCESSING":
                                break;
                            case "##XUNITS":
                                switch (property[1])
                                {
                                    case "NANOMETERS":
                                        fileInfo.xType = FileFormat.XAXISTYPE.XNMETR;
                                        break;
                                    case "1/CM":
                                        fileInfo.xType = FileFormat.XAXISTYPE.XWAVEN;
                                        break;
                                    case "MICROMETERS":
                                        fileInfo.xType = FileFormat.XAXISTYPE.XUMETR;
                                        break;
                                    case "SECONDS":
                                        fileInfo.xType = FileFormat.XAXISTYPE.XSECS;
                                        break;
                                    case "ARBITRARY UNITS":
                                        fileInfo.xType = FileFormat.XAXISTYPE.XARB;
                                        break;
                                }
                                break;
                            case "##XYDATA":
                                curline = ReadXYData(dxFile, property[1], dataCount, yfactor, out yDatas);  //获得最后读取的字符串
                                continue;
                        }
                    }

                    //读下一个行
                    curline = dxFile.ReadLine();
                    if (curline != null)
                        curline = curline.ToUpper();
                }

                return curline;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// 比较两个double是否相同
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool IsSameDouble(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= 0.0000001;
        }

        /// <summary>
        /// 创建等距离的X轴数据
        /// </summary>
        /// <param name="firstX">起始X</param>
        /// <param name="lastX">结束X</param>
        /// <param name="dataCount">数据数量</param>
        /// <returns>X轴数据</returns>
        private static double[] CreateStepXDatas(double firstX, double lastX, int dataCount)
        {
            double[] xDatas = new double[dataCount];
            double xstep = (lastX - firstX) / (xDatas.Length - 1);
            for (int i = 0; i < xDatas.Length; i++)
                xDatas[i] = firstX + i * xstep;

            return xDatas;
        }

        public static bool ReadFile(byte[] fileData, FileFormat fileFormat)
        {
            if (fileData == null || fileFormat == null)
            {
                ErrorString = "Invalid parameters";
                return false;
            }

            fileFormat.dataInfoList = null;
            fileFormat.acquisitionInfo = null;
            fileFormat.xDataList = null;
            fileFormat.fileInfo = null;
            fileFormat.yDataList = null;
            
            if (!IsJCAMPFile(fileData))
            {
                ErrorString = "Invalid file format";
                return false;
            }

            System.IO.MemoryStream memStream = new System.IO.MemoryStream(fileData);
            System.IO.StreamReader dxFile = new System.IO.StreamReader(memStream);
            try
            {
                //查找数据位置
                string curline = dxFile.ReadLine();
                while (curline != null)
                {
                    curline = curline.ToUpper();

                    if (curline.IndexOf("##TITLE") == 0)    //一个Block开始
                    {
                        //保留数据的标题
                        string oldtitle = curline;

                        FileFormat.DataInfo dataInfo = null;
                        double[] yDatas = null;
                        curline = ReadOneBlock(dxFile, fileFormat, out dataInfo, out yDatas);   //从ReadOneBlock返回最后读取的字符串
                        if (curline == null)
                            throw new Exception(ErrorString);
                        else                        
                        {
                            //文件头没有返回dataInfo和yDatas
                            if (dataInfo != null && yDatas != null)
                            {
                                if (fileFormat.dataInfoList == null)
                                    fileFormat.dataInfoList = new List<FileFormat.DataInfo>();
                                fileFormat.dataInfoList.Add(dataInfo);
                                
                                if (fileFormat.yDataList == null)
                                    fileFormat.yDataList = new List<double[]>();
                                fileFormat.yDataList.Add(yDatas);

                                //处理X轴,判断X轴是否与第一个相同
                                if (fileFormat.xDatas == null)
                                {
                                    fileFormat.xDataList = new List<double[]>() { CreateStepXDatas(dataInfo.firstX, dataInfo.lastX, yDatas.Length) };
                                }
                                else
                                {
                                    double[] fXDatas = fileFormat.xDatas;
                                    //如果X轴数据不一样，需要创建新的X轴
                                    if (!IsSameDouble(fXDatas[0], dataInfo.firstX) ||
                                        !IsSameDouble(fXDatas[fXDatas.Length - 1], dataInfo.lastX) ||
                                        fXDatas.Length != yDatas.Length)
                                    {
                                        fileFormat.xDataList.Add(CreateStepXDatas(dataInfo.firstX, dataInfo.lastX, yDatas.Length));
                                    }
                                }

                                //添加数据的标题
                                string[] titleinfos = oldtitle.Split('=');
                                dataInfo.dataTitle = titleinfos.Length > 1 ? titleinfos[1].Trim() : null;
                            }

                            //处理当前字符串
                            continue;
                        }
                    }
                    curline = dxFile.ReadLine();
                }

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (dxFile != null)
                    dxFile.Close();
            }
        }

        /// <summary>
        /// 写入一个数据BLock
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="dataInfo">数据块信息</param>
        /// <param name="xDatas">X数据</param>
        /// <param name="yDatas">Y数据</param>
        /// <returns></returns>
        private static bool WriteOneBlock(StreamWriter writer, FileFormat.FileInfo fileInfo,  FileFormat.DataInfo dataInfo, double[] xDatas, double[] yDatas)
        {
            if (writer == null || fileInfo==null || dataInfo == null || xDatas == null || yDatas == null)
            {
                ErrorString = "Invalid parameters";
                return false;
            }

            //文件头,Title可以自定义名称
            if(string.IsNullOrWhiteSpace(dataInfo.dataTitle))
                writer.WriteLine("##TITLE=" + dataInfo.dataType.ToString());
            else
                writer.WriteLine("##TITLE=" + dataInfo.dataTitle);

            writer.WriteLine("##JCAMP-DX=5.01");

            //光谱类型
            if(fileInfo.specType == FileFormat.SPECTYPE.SPCNIR)
                writer.WriteLine("##DATA TYPE=" + "INFRARED SPECTRUM");  //INFRARED SPECTRUM                
            else if(fileInfo.specType == FileFormat.SPECTYPE.SPCRMN)
                writer.WriteLine("##DATA TYPE=" + "RAMAN SPECTRUM");  //RAMAN SPECTRUM
            else
                writer.WriteLine("##DATA TYPE=" + "UNKNOWN");       //未知

            writer.WriteLine("##DATE=" + fileInfo.createTime.ToString("dd") + "/" + fileInfo.createTime.ToString("MM") + "/" + fileInfo.createTime.ToString("yyyy"));
            writer.WriteLine("##TIME=" + fileInfo.createTime.ToString("HH") + "/" + fileInfo.createTime.ToString("mm") + "/" + fileInfo.createTime.ToString("ss"));
            writer.WriteLine("##ORIGIN=VSPEC");
            writer.WriteLine("##OWNER=VSPEC");

            //X轴格式
            if (fileInfo.xType == FileFormat.XAXISTYPE.XNMETR)
                writer.WriteLine("##XUNITS=" + "NANOMETERS");
            else if (fileInfo.xType == FileFormat.XAXISTYPE.XWAVEN)
                writer.WriteLine("##XUNITS=" + "1/CM");
            else if (fileInfo.xType == FileFormat.XAXISTYPE.XUMETR)
                writer.WriteLine("##XUNITS=" + "MICROMETERS");
            else if (fileInfo.xType == FileFormat.XAXISTYPE.XSECS)
                writer.WriteLine("##XUNITS=" + "SECONDS");
            else
                writer.WriteLine("##XUNITS=" + "ARBITRARY UNITS");

            //Y轴格式
            if (dataInfo.dataType == FileFormat.YAXISTYPE.YTRANS)
                writer.WriteLine("##YUNITS=" + "TRANSMITTANCE");
            else if (dataInfo.dataType == FileFormat.YAXISTYPE.YREFLEC)
                writer.WriteLine("##YUNITS=" + "REFLECTANCE");
            else if (dataInfo.dataType == FileFormat.YAXISTYPE.YABSRB)
                writer.WriteLine("##YUNITS=" + "ABSORBANCE");
            else if (dataInfo.dataType == FileFormat.YAXISTYPE.YKMONK)
                writer.WriteLine("##YUNITS=" + "KUBELKA-MONK");
            else
                writer.WriteLine("##YUNITS=" + "ARBITRARY UNITS");

            writer.WriteLine("##RESOLUTION=" + fileInfo.resolution);
            writer.WriteLine("##FIRSTX=" + dataInfo.firstX);
            writer.WriteLine("##LASTX=" + dataInfo.lastX);
            writer.WriteLine("##MAXY=" + dataInfo.maxYValue);
            writer.WriteLine("##MINY=" + dataInfo.minYValue);

            //保留6位有效数字
            double xfactor = (int)(Math.Truncate(Math.Log10(Math.Max(dataInfo.firstX, dataInfo.lastX)))) - 6;
            writer.WriteLine("##XFACTOR=1e" + xfactor);

            double yfactor = (int)(Math.Truncate(Math.Log10(dataInfo.maxYValue))) - 6;
            writer.WriteLine("##YFACTOR=1e" + yfactor.ToString());
            writer.WriteLine("##NPOINTS=" + yDatas.Length);
            writer.WriteLine("##FIRSTY=" + yDatas[0]);
            writer.WriteLine("##XYDATA=(X++(Y..Y))");

            yfactor = Math.Pow(10, yfactor);
            xfactor = Math.Pow(10, xfactor);

            //数据
            int count = 10;
            double stepx = (dataInfo.lastX - dataInfo.firstX) / (yDatas.Length - 1);
            for (int line = 0; line < ((yDatas.Length - 1) / count) + 1; line++)  //10个一行
            {
                //xValue
                string writestr = ((int)(dataInfo.firstX / xfactor + line * count * stepx / xfactor)).ToString();
                for (int col = 0; col < count; col++)
                {
                    if (line * count + col >= yDatas.Length)
                        break;

                    writestr += " " + ((int)(yDatas[line * count + col] / yfactor)).ToString();
                }
                writer.WriteLine(writestr);
            }
            writer.WriteLine("##END=");

            return true;
        }

        //将文件保存为JCAMP_DX格式
        public static byte[] SaveFile(FileFormat fileFormat)
        {
            StreamWriter stream = null;
            if (fileFormat == null)
                return null;

            //临时文件
            string tempSaveFile = Path.GetTempFileName();
            try
            {
                stream = new StreamWriter(tempSaveFile, false, Encoding.UTF8);

                //文件头
                DateTime filetime = File.GetCreationTime(tempSaveFile);

                //如果有多个数据块，需要一个总的文件头
                if(fileFormat.yDataList.Count > 1)
                {
                    stream.WriteLine("##TITLE=ACloud Export File");
                    stream.WriteLine("##JCAMP-DX=5.01");
                    stream.WriteLine("##DATA TYPE= LINK");
                    stream.WriteLine("##BLOCKS= "+fileFormat.yDataList.Count);
                }

                for (int i = 0; i < fileFormat.yDataList.Count; i++)
                {
                    //如果多个X轴，取对应的X，否则取第一个X轴
                    double[] xDatas = fileFormat.xDataList.Count > i ? fileFormat.xDataList[i] : fileFormat.xDatas;
                    if (WriteOneBlock(stream, fileFormat.fileInfo, fileFormat.dataInfoList[i], xDatas, fileFormat.yDataList[i]) == false)
                        throw new Exception(ErrorString);
                }
                stream.Close();
                stream = null;

                //将全部文件读出
                byte[] retData = CommonMethod.ReadBinaryFile(tempSaveFile);
                if (retData == null)
                    throw new Exception(CommonMethod.ErrorString);

                return retData;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
            finally
            {
                if (stream != null)
                    stream.Close();

                if (File.Exists(tempSaveFile))
                    File.Delete(tempSaveFile);
            }
        }

        /// <summary>
        /// 是否是JCAMP文件
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <returns></returns>
        public static bool IsJCAMPFile(byte[] fileData)
        {
            try
            {
                System.IO.MemoryStream memStream = new System.IO.MemoryStream(fileData);
                System.IO.StreamReader reader = new System.IO.StreamReader(memStream);
                string title = reader.ReadLine();
                reader.Close();
                memStream.Close();

                return title.IndexOf("##TITLE=") == 0;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
