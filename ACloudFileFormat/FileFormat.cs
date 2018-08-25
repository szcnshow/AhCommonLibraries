using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;

namespace Ai.Hong.FileFormat
{
    /// <summary>
    /// 光谱文件格式
    /// </summary>
    public class FileFormat
    {
        #region C++通用函数

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern IntPtr GetErrorMessage32();

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern IntPtr GetErrorMessage64();

        /// <summary>
        /// 获取DLL错误消息
        /// </summary>
        /// <returns></returns>
        public static string GetDLLErrorMessage()
        {
            IntPtr retptr = IntPtr.Zero;

            if (CommonMethod.Is64BitVersion())
                retptr = GetErrorMessage64();
            else
                retptr = GetErrorMessage32();

            return CommonMethod.CopyStringFromIntptrAndFree(ref retptr, Encoding.ASCII);
        }
        #endregion

        /// <summary>
        /// SPCFilter
        /// </summary>
        public const string SPCFilter = "GRAMS SPC|*.spc";
        /// <summary>
        /// SPAFilter
        /// </summary>
        public const string SPAFilter = "OMNIC|*.spa";
        /// <summary>
        /// OPUSFilter
        /// </summary>
        public const string OPUSFilter = "BURKER OPUS(*.n)|*.0*;*.1*;*.2*;*.3*;*.4*;*.5*;*.6*;*.7*;*.8*;*.9*";
        /// <summary>
        /// DXFilter
        /// </summary>
        public const string DXFilter = "JCAMP-DX|*.dx;*.jdx;*.jcm";
        /// <summary>
        /// FOSSFilter
        /// </summary>
        public const string FOSSFilter = "FOSS|*.cal;*.nir";
        /// <summary>
        /// MATLABFilter
        /// </summary>
        public const string MATLABFilter = "MATLAB|*.mat";
        /// <summary>
        /// EXCELFilter
        /// </summary>
        public const string EXCELFilter = "EXCEL|*.xls;*xlsx";
        /// <summary>
        /// CSVFilter
        /// </summary>
        public const string CSVFilter = "CSV FILE|*.csv";
        /// <summary>
        /// TXTFilter
        /// </summary>
        public const string TXTFilter = "TEXT|*.txt";

        /// <summary>
        /// 所有光谱格式
        /// </summary>
        public const string allFilters = SPCFilter + "|" + SPAFilter + "|" + OPUSFilter + "|" + DXFilter + "|" + FOSSFilter + "|" + MATLABFilter + "|" + EXCELFilter + "|" + CSVFilter + "|" + TXTFilter;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorString { get; set; }

        /// <summary>
        /// OPUS:Bruker格式，SPC:EssentialIR格式，JCAMP:JCAMP-DX格式, DPT:Data Table格式, SPA:Thermo格式，SP:PE格式
        /// </summary>
        public enum EnumFileType
        {
            /// <summary>
            /// SPC格式
            /// </summary>
            SPC,
            /// <summary>
            /// OPUS格式
            /// </summary>
            OPUS,
            /// <summary>
            /// SPA格式
            /// </summary>
            SPA,
            /// <summary>
            /// SP格式
            /// </summary>
            SP,
            /// <summary>
            /// JCAMP DX格式
            /// </summary>
            JCAMP,
            /// <summary>
            /// Matlab格式
            /// </summary>
            FOSS,
            /// <summary>
            /// Matlab格式
            /// </summary>
            MAT,
            /// <summary>
            /// EXCEL格式
            /// </summary>
            EXCEL,
            /// <summary>
            /// CSV格式
            /// </summary>
            CSV,
            /// <summary>
            /// TXT格式
            /// </summary>
            TXT,
            /// <summary>
            /// 未知格式
            /// </summary>
            UNKNOWN
        }

        /// <summary>
        /// 文件信息
        /// </summary>
        public class FileInfo
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string filename { get; set; }

            /// <summary>
            /// 原始文件类型(SPC, OPUS, DX, SPA, MAT...)
            /// </summary>
            public EnumFileType fileType { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime createTime { get; set; }

            /// <summary>
            /// 分辨率
            /// </summary>
            public double resolution { get; set; }

            /// <summary>
            /// X轴坐标类型(1/CM...)
            /// </summary>
            public XAXISTYPE xType { get; set; }

            /// <summary>
            /// 光谱类型（近红外、中红外、拉曼...)
            /// </summary>
            public SPECTYPE specType { get; set; }

            /// <summary>
            /// 光谱点数量
            /// </summary>
            public int dataCount { get; set; }

            /// <summary>
            /// 文件描述
            /// </summary>
            public string fileMemo { get; set; }

            /// <summary>
            /// 仪器描述
            /// </summary>
            public string instDescription { get; set; }

            /// <summary>
            /// 文件修改标志
            /// </summary>
            public UInt32 modifyFlag { get; set; }

            /// <summary>
            /// Z轴类型
            /// </summary>
            public ZAXISTYPE zType { get; set; }

            /// <summary>
            /// Z轴步长
            /// </summary>
            public float fzinc { get; set; }

            /// <summary>
            /// 光谱处理参数（主要用于Raman Shift系数）
            /// </summary>
            public float[] fspare { get; set; }

            /// <summary>
            /// 克隆FileInfo
            /// </summary>
            /// <returns></returns>
            public FileInfo Clone()
            {
                FileInfo retdata = this.MemberwiseClone() as FileInfo;
                if (fspare != null)
                {
                    retdata.fspare = new float[fspare.Length];
                    Array.Copy(fspare, retdata.fspare, fspare.Length);
                }

                return retdata;
            }
        }

        /// <summary>
        /// 数据信息
        /// </summary>
        public class DataInfo
        {
            /// <summary>
            /// 开始波数
            /// </summary>
            public double firstX;

            /// <summary>
            /// 结束波数
            /// </summary>
            public double lastX;

            /// <summary>
            /// 最大Y值
            /// </summary>
            public double maxYValue;

            /// <summary>
            /// 最小Y值
            /// </summary>
            public double minYValue;

            /// <summary>
            /// 数据类型(吸收谱, 干涉图...)
            /// </summary>
            public YAXISTYPE dataType;

            /// <summary>
            /// 数据名称
            /// </summary>
            public string dataTitle;

            /// <summary>
            /// 克隆DataInfo
            /// </summary>
            /// <returns></returns>
            public DataInfo Clone()
            {
                return (DataInfo)this.MemberwiseClone();
            }
        };

        /// <summary>
        /// 采集信息
        /// </summary>
        public class AcquisitionInfo
        {
            /// <summary>
            /// Number of co-added scans
            /// </summary>
            public int SCANS { get; set; }    //Value, Number of co-added scans
            /// <summary>
            /// Number of co-added scans in background file
            /// </summary>
            public int SCANSBG { get; set; }  //Value, Number of co-added scans in background file
            /// <summary>
            /// Background spectrum filename
            /// </summary>
            public string NAMEBG { get; set; }   //Text, Background spectrum filename
            /// <summary>
            /// Detector Gain Factor
            /// </summary>
            public double GAIN { get; set; }     //Value, Detector Gain Factor
            /// <summary>
            /// Scan Speed or Velocity text
            /// </summary>
            public string SPEED { get; set; }    //Text, Scan Speed or Velocity text (may be text like "Slow")
            /// <summary>
            ///  Scan Speed or Velocity number
            /// </summary>
            public int VELOCITY { get; set; } //Value, Scan Speed or Velocity number (-1 = automatic or unknown)
            /// <summary>
            /// Laser Wavenumber
            /// </summary>
            public double LWN { get; set; }      //Value, Laser Wavenumber (usually ~15799.7 cm-1)
            /// <summary>
            /// Raman Laser Frequency
            /// </summary>
            public double RAMANFREQ { get; set; }    //Value, Raman Laser Frequency in Wavenumbers
            /// <summary>
            /// Raman Laser Power
            /// </summary>
            public double RAMANPWR { get; set; } //Value, Raman Laser Power
            /// <summary>
            /// J-Stop aperture width
            /// </summary>
            public double JSTOP { get; set; }    //Value, J-Stop aperture width
            /// <summary>
            /// B-Stop aperture width
            /// </summary>
            public double BSTOP { get; set; }    //Value, B-Stop aperture width
            /// <summary>
            /// Detector type name
            /// </summary>
            public string DET { get; set; }      //Text, Detector type name
            /// <summary>
            /// Source type name
            /// </summary>
            public string SRC { get; set; }      //Text, Source type name
            /// <summary>
            /// Beam Splitter name
            /// </summary>
            public string BSPLITTER { get; set; }    //Text, Beam Splitter name
            /// <summary>
            /// Purge Delay before scanning
            /// </summary>
            public double PURGE { get; set; }    //Value, Purge Delay before scanning
            /// <summary>
            /// Zero Filling Factor
            /// </summary>
            public int ZFF { get; set; }             //Value, Zero Filling Factor (0=none, 1=2x, 2=4x, etc.)
            /// <summary>
            /// FTIR Laser Sampling Frequency and Raman Mode 2 = Mid-IR mode 1 = Near-IR mode -2 = Raman-shift
            /// </summary>
            public enumIRMODE IRMODE { get; set; }          //Value, FTIR Laser Sampling Frequency and Raman Mode 2 = Mid-IR mode 1 = Near-IR mode -2 = Raman-shift (negative => Raman)
            /// <summary>
            /// Apodization function type name， "None" (boxcar apodization) "Triangular" "Weak" Norton-Beer "Medium" Norton-Beer "Strong" Norton-Beer "Happ-Genzel" "Bessel" "Cosine" "Filler" "Kaiser" Bessel "Gaussian" "Tradezoidal"
            /// </summary>
            public string APOD { get; set; }         //Text, Apodization function type name: "None" (boxcar apodization) "Triangular" "Weak" Norton-Beer "Medium" Norton-Beer "Strong" Norton-Beer "Happ-Genzel" "Bessel" "Cosine" "Filler" "Kaiser" Bessel "Gaussian" "Tradezoidal"
            /// <summary>
            /// Phase correction type name，"Self" "Magnitude" "Background" "Stored Phase" "None"
            /// </summary>
            public string PHASE { get; set; }        //Text, Phase correction type name: "Self" "Magnitude" "Background" "Stored Phase" "None"
            /// <summary>
            /// Number of interferogram points used for phase correction
            /// </summary>
            public int PHASEPTS { get; set; }        //Value, Number of interferogram points used for phase correction
            /// <summary>
            /// nterferogram mode name: "Double" sided "Single" sided "Left" single sided "Right" single sided
            /// </summary>
            public string IGRAMSIDE { get; set; }    //Text, Interferogram mode name: "Double" sided "Single" sided "Left" single sided "Right" single sided
            /// <summary>
            /// Interferogram collection direction name: "Bi" interferometer scans in both direction "Uni" interferometer scans in one direction "Forward" uni-directional "Reverse" uni-directional
            /// </summary>
            public string IGRAMDIR { get; set; }     //Text, Interferogram collection direction name: "Bi" interferometer scans in both direction "Uni" interferometer scans in one direction "Forward" uni-directional "Reverse" uni-directional
            /// <summary>
            /// Polarizer Angle
            /// </summary>
            public double POLARIZER { get; set; }    //Value, Polarizer Angle
            /// <summary>
            /// Low Pass Filter Cutoff
            /// </summary>
            public double LOWPASS { get; set; }      //Value, Low Pass Filter Cutoff
            /// <summary>
            /// High Pass Filter Cutoff
            /// </summary>
            public double HIGHPASS { get; set; }     //Value, High Pass Filter Cutoff
            /// <summary>
            /// Optical Filter Name
            /// </summary>
            public string FILTER { get; set; }       //Text, Optical Filter Name
            /// <summary>
            /// Number of averaged readings per data point
            /// </summary>
            public int SMOOTH { get; set; }          //Value, Number of averaged readings per data point
            /// <summary>
            /// Single beam mode name: "Sample" (Does not replace existing background). “Background" for future ratioing with sample.
            /// </summary>
            public string BEAM { get; set; }         //Text, Single beam mode name: "Sample" (Does not replace existing background). “Background" for future ratioing with sample.
            /// <summary>
            /// Time for A/D averaging of each point reading (msec). If the A/D has a fixed time and point readings are averaged, then this time should be divided by the approximate A/D time to get the number of readings.
            /// </summary>
            public double AVGTIME { get; set; }      //Value, Time for A/D averaging of each point reading (msec). If the A/D has a fixed time and point readings are averaged, then this time should be divided by the approximate A/D time to get the number of readings.
            /// <summary>
            /// Begin delay in seconds
            /// </summary>
            public double BDELAY { get; set; }       //Value, Begin delay in seconds
            /// <summary>
            /// Scan delay in seconds
            /// </summary>
            public double SDELAY { get; set; }       //Value, Scan delay in seconds
            /// <summary>
            /// Detector or beam path channel used for acquisition
            /// </summary>
            public int CHANNEL { get; set; }       //Value, Detector or beam path channel used for acquisition

            /// <summary>
            /// 克隆
            /// </summary>
            /// <returns></returns>
            public AcquisitionInfo Clone()
            {
                return MemberwiseClone() as AcquisitionInfo;
            }

            /// <summary>
            /// 从SPC 注释中读取采集参数
            /// </summary>
            /// <param name="logData">注释</param>
            /// <returns></returns>
            public static AcquisitionInfo FromSPCLogData(string logData)
            {
                if (string.IsNullOrWhiteSpace(logData))
                    return null;

                AcquisitionInfo retinfo = new AcquisitionInfo();
                logData = logData.Replace("\n", "");
                logData = logData.Replace("\0", "");
                string[] logs = logData.Split('\r');

                var props = typeof(AcquisitionInfo).GetProperties();
                int tempint;
                double tempdouble;
                foreach (var item in props)
                {
                    //按参数名称在Log中查找，例如:SCANS = 4
                    string valuestr = logs.FirstOrDefault(p => p.IndexOf(item.Name + " = ") == 0);
                    if (valuestr != null)
                    {
                        valuestr = valuestr.Substring((item.Name + " = ").Length);    //去掉前面的名称和' = ', 只保留值
                        if (item.PropertyType == typeof(int))
                        {
                            if (int.TryParse(valuestr, out tempint))
                                item.SetValue(retinfo, tempint, null);
                        }
                        else if (item.PropertyType == typeof(double))
                        {
                            if (double.TryParse(valuestr, out tempdouble))
                                item.SetValue(retinfo, tempdouble, null);
                        }
                        else if (item.PropertyType == typeof(string))
                        {
                            item.SetValue(retinfo, valuestr, null);
                        }
                        else if (item.PropertyType == typeof(enumIRMODE))
                        {
                            if (int.TryParse(valuestr, out tempint))
                                item.SetValue(retinfo, tempint, null);
                            item.SetValue(retinfo, (enumIRMODE)tempint, null);
                        }
                    }
                }

                return retinfo;
            }

            /// <summary>
            /// 生成Acquisition Log Data
            /// </summary>
            /// <returns></returns>
            public string ToSPCLogData()
            {
                var props = typeof(AcquisitionInfo).GetProperties();
                string retdata = null;
                int tempint;
                double tempdouble;
                foreach (var item in props)
                {
                    string curstr = null;
                    if (item.PropertyType == typeof(int))
                    {
                        tempint = (int)item.GetValue(this, null);
                        if (tempint != 0)
                            curstr = tempint.ToString();
                    }
                    else if (item.PropertyType == typeof(double))
                    {
                        tempdouble = (double)item.GetValue(this, null);
                        if (tempdouble != 0)
                            curstr = tempdouble.ToString();
                    }
                    else if (item.PropertyType == typeof(string))
                    {
                        curstr = (string)item.GetValue(this, null);
                    }
                    else if (item.PropertyType == typeof(enumIRMODE))
                    {
                        curstr = ((int)((enumIRMODE)item.GetValue(this, null))).ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(curstr))
                    {
                        curstr = item.Name + " = " + curstr;
                        retdata += curstr + "\r\n";
                    }

                }

                return retdata;
            }
        };

        /// <summary>
        /// 光谱类型
        /// </summary>
        public enum enumIRMODE
        {
            /// <summary>
            /// 近红外
            /// </summary>
            NearIR = 1,
            /// <summary>
            /// 中红外
            /// </summary>
            MidIR = 2,
            /// <summary>
            /// 拉曼
            /// </summary>
            RamanShift = -1
        }

        /// <summary>
        /// fexper 光谱类型(红外，近红外，拉曼...)
        /// </summary>
        public enum SPECTYPE
        {
            /// <summary>
            /// General SPC
            /// </summary>
            [Description("General")]
            SPCGEN = 0,	    /* General SPC (could be anything) */
            /// <summary>
            /// 气相色谱图
            /// </summary>
            [Description("Gas Chromatogram")]
            SPCGC = 1,	    /* 气相色谱图 Gas Chromatogram */
            /// <summary>
            /// 通用色谱图
            /// </summary>
            [Description("General Chromatogram")]
            SPCCGM = 2,	    /* 通用色谱图 General Chromatogram (same as SPCGEN with TCGRAM) */
            /// <summary>
            /// HPLC色谱图
            /// </summary>
            [Description("HPLCHPLC")]
            SPCHPLC = 3,	/* HPLC色谱图 HPLCHPLC Chromatogram */
            /// <summary>
            /// 傅立叶变换中红外，近红外，拉曼FT-IR, FT-NIR, FT-Raman
            /// </summary>
            [Description("FT-IR/NIR/RAMAN")]
            SPCFTIR = 4,	    /* 傅立叶变换中红外，近红外，拉曼FT-IR, FT-NIR, FT-Raman Spectrum or Igram (Can also be used for scanning IR.) */
            /// <summary>
            /// 近红外 NIR Spectrum
            /// </summary>
            [Description("NIR")]
            SPCNIR = 5,	    /* 近红外 NIR Spectrum (Usually multi-spectral data sets for calibration.) */
            /// <summary>
            /// UV-VIS Spectrum
            /// </summary>
            [Description("UV-VIS")]
            SPCUV = 7,	    /* UV-VIS Spectrum (Can be used for single scanning UV-VIS-NIR.) */
            /// <summary>
            /// X-射线
            /// </summary>
            [Description("X-ray")]
            SPCXRY = 8,	    /* X-射线 X-ray Diffraction Spectrum */
            /// <summary>
            /// 多种光谱 Mass Spectrum
            /// </summary>
            [Description("Mass")]
            SPCMS = 9,	    /* 多种光谱 Mass Spectrum  (Can be single, GC-MS, Continuum, Centroid or TOF.) */
            /// <summary>
            /// 核磁
            /// </summary>
            [Description("NMR")]
            SPCNMR = 10,	/* 核磁 NMR Spectrum or FID */
            /// <summary>
            /// 拉曼谱 Raman Spectrum
            /// </summary>
            [Description("Raman")]
            SPCRMN = 11,	/* 拉曼谱 Raman Spectrum (Usually Diode Array, CCD, etc. use SPCFTIR for FT-Raman.) */
            /// <summary>
            /// 荧光 Fluorescence Spectrum
            /// </summary>
            [Description("Fluorescence")]
            SPCFLR = 12,	/* 荧光 Fluorescence Spectrum */
            /// <summary>
            /// 原子 Atomic Spectrum
            /// </summary>
            [Description("Atomic")]
            SPCATM = 13,	/* 原子 Atomic Spectrum */
            /// <summary>
            /// 色谱 Chromatography
            /// </summary>
            [Description("Chromatography")]
            SPCDAD = 14,	    /* 色谱 Chromatography Diode Array Spectra */
            /// <summary>
            /// 中红外谱
            /// </summary>
            [Description("MIR")]
            SPCMIR = 30	    /* 中红外谱*/
        }

        /// <summary>
        /// fxtype X坐标类型
        /// </summary>
        public enum XAXISTYPE
        {
            /// <summary>
            /// Arbitrary
            /// </summary>
            [Description("Arbitrary")]
            XARB = 0,	/* Arbitrary */
            /// <summary>
            /// Wavenumber
            /// </summary>
            [Description("Wavenumber")]
            XWAVEN = 1,	/* Wavenumber (cm-1) */
            /// <summary>
            /// Micrometers
            /// </summary>
            [Description("Micrometers")]
            XUMETR = 2,	/* Micrometers (um) */
            /// <summary>
            /// Nanometers
            /// </summary>
            [Description("Nanometers")]
            XNMETR = 3,	/* Nanometers (nm) */
            /// <summary>
            /// Seconds
            /// </summary>
            [Description("Seconds")]
            XSECS = 4,	/* Seconds */
            /// <summary>
            /// Minutes
            /// </summary>
            [Description("Minutes")]
            XMINUTS = 5,	/* Minutes */
            /// <summary>
            /// Hertz
            /// </summary>
            [Description("Hertz")]
            XHERTZ = 6,	/* Hertz (Hz) */
            /// <summary>
            /// Kilohertz
            /// </summary>
            [Description("Kilohertz")]
            XKHERTZ = 7,	/* Kilohertz (KHz) */
            /// <summary>
            /// Megahertz
            /// </summary>
            [Description("Megahertz")]
            XMHERTZ = 8,	/* Megahertz (MHz) */
            /// <summary>
            /// Mass
            /// </summary>
            [Description("Mass")]
            XMUNITS = 9,	/* Mass (M/z) */
            /// <summary>
            /// PPM
            /// </summary>
            [Description("PPM")]
            XPPM = 10,	/* Parts per million (PPM) */
            /// <summary>
            /// Days
            /// </summary>
            [Description("Days")]
            XDAYS = 11,	/* Days */
            /// <summary>
            /// Years
            /// </summary>
            [Description("Years")]
            XYEARS = 12,	/* Years */
            /// <summary>
            /// Raman Shift
            /// </summary>
            [Description("Raman Shift")]
            XRAMANS = 13	    /* Raman Shift (cm-1) */
        }

        /// <summary>
        /// fytype Y坐标类型
        /// </summary>
        public enum YAXISTYPE
        {
            /// <summary>
            /// Arbitrary Intensity
            /// </summary>
            [Description("Arbitrary Intensity")]
            YARB = 0,	/* Arbitrary Intensity */
            /// <summary>
            /// Interferogram
            /// </summary>
            [Description("Interferogram")]
            YIGRAM = 1,	/* Interferogram */
            /// <summary>
            /// Absorbance
            /// </summary>
            [Description("Absorbance")]
            YABSRB = 2,	/* Absorbance */
            /// <summary>
            /// Kubelka-Monk
            /// </summary>
            [Description("Kubelka-Monk")]
            YKMONK = 3,	/* Kubelka-Monk */
            /// <summary>
            /// Counts
            /// </summary>
            [Description("Counts")]
            YCOUNT = 4,	/* Counts */
            /// <summary>
            /// Volts
            /// </summary>
            [Description("Volts")]
            YVOLTS = 5,	/* Volts */
            /// <summary>
            /// Degrees
            /// </summary>
            [Description("Degrees")]
            YDEGRS = 6,	/* Degrees */
            /// <summary>
            /// Milliamps
            /// </summary>
            [Description("Milliamps")]
            YAMPS = 7,	/* Milliamps */
            /// <summary>
            /// Millimeters
            /// </summary>
            [Description("Millimeters")]
            YMETERS = 8,	/* Millimeters */
            /// <summary>
            /// Millivolts
            /// </summary>
            [Description("Millivolts")]
            YMVOLTS = 9,	/* Millivolts */
            /// <summary>
            /// Log(1/R)
            /// </summary>
            [Description("Log(1/R)")]
            YLOGDR = 10,	/* Log(1/R) */
            /// <summary>
            /// Percent
            /// </summary>
            [Description("Percent")]
            YPERCNT = 11,	/* Percent */
            /// <summary>
            /// Intensity
            /// </summary>
            [Description("Intensity")]
            YINTENS = 12,	/* Intensity */
            /// <summary>
            /// Relative Intensity
            /// </summary>
            [Description("Relative Intensity")]
            YRELINT = 13,	/* Relative Intensity */
            /// <summary>
            /// Energy
            /// </summary>
            [Description("Energy")]
            YENERGY = 14,	/* Energy */
            /// <summary>
            /// Decibel
            /// </summary>
            [Description("Decibel")]
            YDECBL = 16,	/* Decibel */
            /// <summary>
            /// Temperature (F)
            /// </summary>
            [Description("Temperature (F)")]
            YDEGRF = 19,	/* Temperature (F) */
            /// <summary>
            /// Temperature (C)
            /// </summary>
            [Description("Temperature (C)")]
            YDEGRC = 20,	/* Temperature (C) */
            /// <summary>
            /// Temperature (K)
            /// </summary>
            [Description("Temperature (K)")]
            YDEGRK = 21,	/* Temperature (K) */
            /// <summary>
            /// Index of Refraction [N]
            /// </summary>
            [Description("Index of Refraction [N]")]
            YINDRF = 22,	/* Index of Refraction [N] */
            /// <summary>
            /// Extinction Coeff. [K]
            /// </summary>
            [Description("Extinction Coeff. [K]")]
            YEXTCF = 23,	/* Extinction Coeff. [K] */
            /// <summary>
            /// Real
            /// </summary>
            [Description("Real")]
            YREAL = 24,	/* Real */
            /// <summary>
            /// Imaginary
            /// </summary>
            [Description("Imaginary")]
            YIMAG = 25,	/* Imaginary */
            /// <summary>
            /// Complex
            /// </summary>
            [Description("Complex")]
            YCMPLX = 26,	/* Complex */
            /// <summary>
            /// Transmission
            /// </summary>
            [Description("Transmission")]
            YTRANS = 128,	/* Transmission (ALL HIGHER MUST HAVE VALLEYS!) */
            /// <summary>
            /// Reflectance
            /// </summary>
            [Description("Reflectance")]
            YREFLEC = 129,	/* Reflectance */
            /// <summary>
            /// Single Beam with Valley Peaks
            /// </summary>
            [Description("Single Beam with Valley Peaks")]
            YVALLEY = 130,	/* Arbitrary or Single Beam with Valley Peaks */
            /// <summary>
            /// Emission
            /// </summary>
            [Description("Emission")]
            YEMISN = 131,	/* Emission */
            /// <summary>
            /// OPUS ATR
            /// </summary>
            [Description("ATR")]
            YATR = 140,	/* Emission */
            /// <summary>
            /// Background single beam
            /// </summary>
            [Description("SCRF")]
            YSCRF = 141,	/* Emission */
            /// <summary>
            /// Sample single beam
            /// </summary>
            [Description("SCSM")]
            YSCSM = 142,	/* Emission */
            /// <summary>
            /// Background interfergoram
            /// </summary>
            [Description("IGRF")]
            YIGRF = 143,	/* Emission */
            /// <summary>
            /// Sample interfergoram
            /// </summary>
            [Description("IGSM")]
            YIGSM = 144,	/* Emission */
            /// <summary>
            /// RAMAN PIXEL
            /// </summary>
            [Description("RPXL")]
            YRPXL = 145	/* Emission */

        }

        /// <summary>
        /// Z坐标类型
        /// </summary>
        public enum ZAXISTYPE
        {
            /// <summary>
            /// Arbitrary
            /// </summary>
            [Description("Arbitrary")]
            XARB = 0,	/* Arbitrary */
            /// <summary>
            /// Wavenumber
            /// </summary>
            [Description("Wavenumber")]
            XWAVEN = 1,	/* Wavenumber (cm-1) */
            /// <summary>
            /// Micrometers
            /// </summary>
            [Description("Micrometers")]
            XUMETR = 2,	/* Micrometers (um) */
            /// <summary>
            /// Nanometers
            /// </summary>
            [Description("Nanometers")]
            XNMETR = 3,	/* Nanometers (nm) */
            /// <summary>
            /// Seconds
            /// </summary>
            [Description("Seconds")]
            XSECS = 4,	/* Seconds */
            /// <summary>
            /// Minutes
            /// </summary>
            [Description("Minutes")]
            XMINUTS = 5,	/* Minutes */
            /// <summary>
            /// Hertz
            /// </summary>
            [Description("Hertz")]
            XHERTZ = 6,	/* Hertz (Hz) */
            /// <summary>
            /// Kilohertz
            /// </summary>
            [Description("Kilohertz")]
            XKHERTZ = 7,	/* Kilohertz (KHz) */
            /// <summary>
            /// Megahertz
            /// </summary>
            [Description("Megahertz")]
            XMHERTZ = 8,	/* Megahertz (MHz) */
            /// <summary>
            /// Mass
            /// </summary>
            [Description("Mass")]
            XMUNITS = 9,	/* Mass (M/z) */
            /// <summary>
            /// PPM
            /// </summary>
            [Description("PPM")]
            XPPM = 10,	/* Parts per million (PPM) */
            /// <summary>
            /// Days
            /// </summary>
            [Description("Days")]
            XDAYS = 11,	/* Days */
            /// <summary>
            /// Years
            /// </summary>
            [Description("Years")]
            XYEARS = 12,	/* Years */
            /// <summary>
            /// Raman Shift
            /// </summary>
            [Description("Raman Shift")]
            XRAMANS = 13,	/* Raman Shift (cm-1) */
            /// <summary>
            /// eV
            /// </summary>
            [Description("eV")]
            XEV = 14,	/* eV */
            /// <summary>
            /// XYZ text
            /// </summary>
            [Description("XYZ text")]
            ZTEXTL = 15,	/* XYZ text labels in fcatxt (old 0x4D version only) */
            /// <summary>
            /// Diode Number
            /// </summary>
            [Description("Diode Number")]
            XDIODE = 16,	/* Diode Number */
            /// <summary>
            /// Channel
            /// </summary>
            [Description("Channel")]
            XCHANL = 17,	/* Channel */
            /// <summary>
            /// Degrees
            /// </summary>
            [Description("Degrees")]
            XDEGRS = 18,	/* Degrees */
            /// <summary>
            /// Temperature (F)
            /// </summary>
            [Description("Temperature (F)")]
            XDEGRF = 19,	/* Temperature (F) */
            /// <summary>
            /// Temperature (C)
            /// </summary>
            [Description("Temperature (C)")]
            XDEGRC = 20,	/* Temperature (C) */
            /// <summary>
            /// Temperature (K)
            /// </summary>
            [Description("Temperature (K)")]
            XDEGRK = 21,	/* Temperature (K) */
            /// <summary>
            /// Data Points
            /// </summary>
            [Description("Data Points")]
            XPOINT = 22,	/* Data Points */
            /// <summary>
            /// Milliseconds
            /// </summary>
            [Description("Milliseconds")]
            XMSEC = 23,	/* Milliseconds (mSec) */
            /// <summary>
            /// Microseconds
            /// </summary>
            [Description("Microseconds")]
            XUSEC = 24,	/* Microseconds (uSec) */
            /// <summary>
            /// Nanoseconds
            /// </summary>
            [Description("Nanoseconds")]
            XNSEC = 25,	/* Nanoseconds (nSec) */
            /// <summary>
            /// Gigahertz
            /// </summary>
            [Description("Gigahertz")]
            XGHERTZ = 26,	/* Gigahertz (GHz) */
            /// <summary>
            /// Centimeters
            /// </summary>
            [Description("Centimeters")]
            XCM = 27,	/* Centimeters (cm) */
            /// <summary>
            /// Meters
            /// </summary>
            [Description("Meters")]
            XMETERS = 28,	/* Meters (m) */
            /// <summary>
            /// Millimeters
            /// </summary>
            [Description("Millimeters")]
            XMMETR = 29,	/* Millimeters (mm) */
            /// <summary>
            /// Hours
            /// </summary>
            [Description("Hours")]
            XHOURS = 30,	/* Hours */
            /// <summary>
            /// Double interferogram
            /// </summary>
            [Description("Double interferogram")]
            XDBLIGM = 255	/* Double interferogram (no display labels) */
        }

        /// <summary>
        /// 数据参数
        /// </summary>
        public List<DataInfo> dataInfoList { get; set; }

        /// <summary>
        /// 缺省的光谱数据信息
        /// </summary>
        public DataInfo dataInfo
        {
            get
            {
                if (dataInfoList != null && dataInfoList.Count > 0)
                    return dataInfoList[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// 文件参数
        /// </summary>
        public FileInfo fileInfo { get; set; }

        /// <summary>
        /// 采集参数
        /// </summary>
        public AcquisitionInfo acquisitionInfo { get; set; }

        /// <summary>
        /// X轴数据,如果xDataList.Count > 1，那么必须=yDataList.Count
        /// </summary>
        public List<double[]> xDataList { get; set; }

        /// <summary>
        /// 缺省的X轴数据
        /// </summary>
        public double[] xDatas
        {
            get
            {
                if (xDataList != null && xDataList.Count > 0)
                    return xDataList[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Y轴数据，如果xDataList.count==1，那么所有yDatas的数据点数量必须相同
        /// </summary>
        public List<double[]> yDataList { get; set; }

        /// <summary>
        /// 缺省的Y轴数据
        /// </summary>
        public double[] yDatas
        {
            get
            {
                if (yDataList != null && yDataList.Count > 0)
                    return yDataList[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// 文件附加数据，比如样品信息，结果信息，由调用程序来解析
        /// </summary>
        public byte[] additionalData { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileFormat()
        {
            fileInfo = new FileInfo();
            acquisitionInfo = new AcquisitionInfo();
            dataInfoList = new List<DataInfo>();
            xDataList = new List<double[]>();
            yDataList = new List<double[]>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">光谱文件名</param>
        public FileFormat(string filename)
        {
            ReadFile(filename);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileData">光谱数据</param>
        public FileFormat(byte[] fileData)
        {
            ReadFile(fileData);
        }

        /// <summary>
        /// 克隆文件数据
        /// </summary>
        /// <param name="withDatas">True=包含光谱, False=不包含光谱，只有文件信息</param>
        /// <returns></returns>
        public FileFormat Clone(bool withDatas = true)
        {
            var retData = this.MemberwiseClone() as FileFormat;

            //采集信息
            if (this.acquisitionInfo != null)
                retData.acquisitionInfo = this.acquisitionInfo.Clone();
            else
                retData.acquisitionInfo = new AcquisitionInfo();

            //附加信息
            if (this.additionalData != null)
            {
                retData.additionalData = new byte[this.additionalData.Length];
                Array.Copy(this.additionalData, retData.additionalData, additionalData.Length);
            }

            //文件信息
            if (this.fileInfo != null)
                retData.fileInfo = this.fileInfo.Clone();
            else
                retData.fileInfo = new FileInfo();

            //需要数据的时候才能Clone DataInfoList
            if (withDatas && this.dataInfoList != null)
            {
                retData.dataInfoList = new List<DataInfo>();
                foreach (var item in this.dataInfoList)
                {
                    retData.dataInfoList.Add(item.Clone());
                }
            }
            else
                retData.dataInfoList = new List<DataInfo>();

            //X轴数据
            if (withDatas && this.xDataList != null)
            {
                retData.xDataList = new List<double[]>();
                foreach (var item in this.xDataList)
                {
                    var data = new double[item.Length];
                    Array.Copy(item, data, item.Length);
                    retData.xDataList.Add(data);
                }
            }
            else
                retData.xDataList = new List<double[]>();

            //Y轴数据
            if (withDatas && this.yDataList != null)
            {
                retData.yDataList = new List<double[]>();
                foreach (var item in this.yDataList)
                {
                    var data = new double[item.Length];
                    Array.Copy(item, data, item.Length);
                    retData.yDataList.Add(data);
                }
            }
            else
                retData.yDataList = new List<double[]>();

            return retData;
        }

        /// <summary>
        /// 读取光谱文件
        /// </summary>
        /// <param name="filename">光谱文件名</param>
        /// <returns></returns>
        public bool ReadFile(string filename)
        {
            byte[] fileData = CommonMethod.ReadBinaryFile(filename);
            if(fileData == null)
            {
                ErrorString = CommonMethod.ErrorString;
                return false;
            }

            return ReadFile(fileData);
        }

        /// <summary>
        /// 读取光谱文件
        /// </summary>
        /// <param name="fileData">光谱文件数据</param>
        /// <returns></returns>
        public bool ReadFile(byte[] fileData)
        {
            if (fileData == null)
                return false;

            if (SPCFileFormat.HasAuthority() && SPCFileFormat.IsSPCFile(fileData, fileData.Length))
            {
                SPCFileFormat.ReadFile(fileData, fileData.Length, this);
                if (fileInfo != null)
                    fileInfo.fileType = EnumFileType.SPC;
            }
            else if (SPAFileFormat.HasAuthority() && SPAFileFormat.IsSPAFile(fileData, fileData.Length))
            {
                SPAFileFormat.ReadFile(fileData, fileData.Length, this);
                if (fileInfo != null)
                    fileInfo.fileType = EnumFileType.SPA;
            }
            else if (OPUSFileFormat.HasAuthority() && OPUSFileFormat.IsOPUSFile(fileData, fileData.Length))
            {
                OPUSFileFormat.ReadFile(fileData, fileData.Length, this);
                if (fileInfo != null)
                    fileInfo.fileType = EnumFileType.OPUS;
            }
            else if (JCAMPFileFormat.IsJCAMPFile(fileData))
            {
                JCAMPFileFormat.ReadFile(fileData, this);
                if (fileInfo != null)
                    fileInfo.fileType = EnumFileType.JCAMP;
            }
            else if (FossFileFormat.IsFossFile(fileData))
            {
                FossFileFormat.ReadFile(fileData, this);
                if (fileInfo != null)
                    fileInfo.fileType = EnumFileType.FOSS;
            }
            else
            {
                ErrorString = "Invalid file format";
                return false;
            }

            if(xDatas == null || yDatas == null)
            {
                ErrorString = GetDLLErrorMessage();
                return false;
            }
            else  //X轴按照从小到大顺序排序
            {
                if (xDataList.Count == 1)    //只有一个X轴
                {
                    if (dataInfo.firstX > dataInfo.lastX)    //需要反转数据，确保firstX小于lastX
                    {
                        double temp = dataInfo.firstX;
                        dataInfo.firstX = dataInfo.lastX;
                        dataInfo.lastX = temp;

                        Array.Reverse(xDatas);

                        foreach (var item in yDataList)
                            Array.Reverse(item);
                    }
                }
                else
                {
                    //多个x轴,肯定有相同个数的Y轴,逐个比较X轴的波数范围，逐个反转
                    for (int i = 0; i < dataInfoList.Count; i++)
                    {
                        if (dataInfoList[i].firstX > dataInfoList[i].lastX)
                        {
                            double temp = dataInfoList[i].firstX;
                            dataInfoList[i].firstX = dataInfoList[i].lastX;
                            dataInfoList[i].lastX = temp;

                            Array.Reverse(xDataList[i]);
                            Array.Reverse(yDataList[i]);
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 将FileFormat保存到文件
        /// </summary>
        /// <param name="filename">需要保存的文件名</param>
        /// <param name="filetype">光谱文件格式(缺省SPC)</param>
        /// <returns></returns>
        public bool SaveFile(string filename, EnumFileType filetype = EnumFileType.SPC)
        {
            byte[] fileData = SaveFile(filetype);
            if (fileData == null)
                return false;

            return CommonMethod.SaveBinaryFile(filename, fileData);
        }

        /// <summary>
        /// 将FileFormat保存到数组
        /// </summary>
        /// <param name="filetype">光谱文件格式(缺省SPC)</param>
        /// <returns></returns>
        public byte[] SaveFile(EnumFileType filetype = EnumFileType.SPC)
        {
            if (filetype == EnumFileType.SPC)
                return SPCFileFormat.SaveFile(this);
            else if (filetype == EnumFileType.OPUS)
                return OPUSFileFormat.SaveFile(this);
            else if (filetype == EnumFileType.SPA)
                return SPAFileFormat.SaveFile(this);
            else if (filetype == EnumFileType.FOSS)
                return FossFileFormat.SaveFile(this);
            else if (filetype == EnumFileType.JCAMP)
                return JCAMPFileFormat.SaveFile(this);
            else
                ErrorString = "Invalid file format";

            return null;
        }

        /// <summary>
        /// 将FileFormat连通附加信息保存到数组
        /// Foss的附加信息=（仪器编号,组分名称数组,组分数组,文件时间数组）
        /// </summary>
        /// <param name="appendDatas">附加信息</param>
        /// <param name="filetype">光谱文件格式(缺省SPC)</param>
        /// <returns></returns>
        public byte[] SaveFileWithAddtional(object[] appendDatas, EnumFileType filetype = EnumFileType.SPC)
        {
            if (filetype == EnumFileType.SPC)
                return SPCFileFormat.SaveFile(this);
            else if (filetype == EnumFileType.OPUS)
                return OPUSFileFormat.SaveFile(this);
            else if (filetype == EnumFileType.SPA)
                return SPAFileFormat.SaveFile(this);
            else if (filetype == EnumFileType.FOSS)
            {
                if (appendDatas == null || appendDatas.Length != 4 ||
                    (appendDatas[0] as string) == null ||
                    (appendDatas[1] as string[])== null ||
                    (appendDatas[2] as double[]) == null ||
                    (appendDatas[3] as DateTime[]) == null)
                {
                    ErrorString = "Invalid parameters, addtionalDatas[string serialNo, string[] compNames, double[] compDatas, DataTime[] fileTime ]";
                    return null;
                }
                return FossFileFormat.SaveFile(this, (string)appendDatas[0], (string[])appendDatas[1], (double[])appendDatas[2], (DateTime[])appendDatas[3]);
            }
            else if (filetype == EnumFileType.JCAMP)
                return JCAMPFileFormat.SaveFile(this);
            else
                ErrorString = "Invalid file format";

            return null;
        }

        /// <summary>
        /// 添加X轴和Y轴数据
        /// </summary>
        /// <param name="newXDatas">X轴数据</param>
        /// <param name="newYDatas">Y轴数据</param>
        /// <param name="specType">光谱文件格式</param>
        /// <param name="xType">X轴数据类型</param>
        /// <param name="dataType">Y轴数据类型</param>
        public bool AddData(double[] newXDatas, double[] newYDatas, SPECTYPE specType, XAXISTYPE xType, YAXISTYPE dataType)
        {
            if (newXDatas.Length != newYDatas.Length || newXDatas.Length < 2)
                return false;

            if (fileInfo == null)
                fileInfo = new FileInfo();
            if (xDataList == null)
                xDataList = new List<double[]>();
            if (yDataList == null)
                yDataList = new List<double[]>();
            if (dataInfoList == null)
                dataInfoList = new List<DataInfo>();

            if (xDatas == null)     //还没有添加过数据，设置为第一个数据信息
            {
                fileInfo.xType = xType;
                fileInfo.dataCount = newYDatas.Length;
                fileInfo.specType = specType;
            }

            //添加X轴
            if (xDatas == null)
                xDataList.Add(newXDatas);
            else if (xDataList.Count > 1)   //有两个以上的X轴，一定是多X轴了
                xDataList.Add(newXDatas);
            else    //检查是否与原有X轴数据相同
            {
                if (xDatas.Length != newXDatas.Length)      //数据点数量不一样，肯定是不同的X轴
                    xDataList.Add(newXDatas);
                else
                {
                    bool issame = true;
                    double stepx = (xDatas[xDatas.Length - 1] - xDatas[0]) / (xDatas.Length - 1);
                    stepx = Math.Abs(stepx / 1000);   //设定一个最小误差
                    for (int i = 0; i < xDatas.Length; i++)
                    {
                        if (Math.Abs(newXDatas[i] - xDatas[i]) > stepx)     //如果超出最小误差，就认为不同的X轴
                        {
                            issame = false;
                            break;
                        }
                    }
                    if (!issame)    //X轴不一样
                    {
                        if (xDataList.Count != yDataList.Count)  //以前有相同的X轴共享不同的Y轴，需要按Y轴数量复制X轴
                        {
                            while (xDataList.Count < yDataList.Count)
                                xDataList.Add(xDatas);
                        }
                        xDataList.Add(newXDatas);
                    }
                }
            }

            yDataList.Add(newYDatas);

            //创建光谱数据信息
            DataInfo info = new DataInfo();
            info.dataType = dataType;
            info.firstX = newXDatas[0];
            info.lastX = newXDatas[newXDatas.Length - 1];
            info.maxYValue = newYDatas.Max();
            info.minYValue = newYDatas.Min();

            dataInfoList.Add(info);

            return true;
        }

        /// <summary>
        /// 添加统一格式的Y轴数据（已经添加了其它数据）
        /// </summary>
        /// <param name="newYDatas">Y轴数据</param>
        /// <param name="dataType">Y轴数据类型</param>
        public bool AddData(double[] newYDatas, YAXISTYPE dataType)
        {
            if (xDatas == null || yDatas == null || dataInfo == null || xDatas.Length != newYDatas.Length || xDataList.Count > 1)
                return false;

            yDataList.Add(newYDatas);

            //创建光谱数据信息
            DataInfo info = new DataInfo();
            info.dataType = dataType;
            info.firstX = dataInfo.firstX;
            info.lastX = dataInfo.lastX;
            info.maxYValue = newYDatas.Max();
            info.minYValue = newYDatas.Min();

            dataInfoList.Add(info);

            return true;
        }

        /// <summary>
        /// 添加均匀X轴的Y轴数据（没有添加了其它数据）
        /// </summary>
        public bool AddData(double firstX, double lastX, double[] newYDatas, SPECTYPE specType, XAXISTYPE xType, YAXISTYPE dataType)
        {
            if (xDatas != null || yDatas != null || dataInfo != null || newYDatas == null || newYDatas.Length < 2)
                return false;

            if (fileInfo == null)
                fileInfo = new FileInfo();
            if (xDataList == null)
                xDataList = new List<double[]>();
            if (yDataList == null)
                yDataList = new List<double[]>();
            if (dataInfoList == null)
                dataInfoList = new List<DataInfo>();

            fileInfo.xType = xType;
            fileInfo.dataCount = newYDatas.Length;
            fileInfo.specType = specType;

            double[] newXDatas = new double[newYDatas.Length];
            double stepx = (lastX - firstX) / (newYDatas.Length - 1);
            for (int i = 0; i < newXDatas.Length; i++)
                newXDatas[i] = i * stepx;
            xDataList.Add(newXDatas);
            yDataList.Add(newYDatas);

            //创建光谱数据信息
            DataInfo info = new DataInfo();
            info.dataType = dataType;
            info.firstX = firstX;
            info.lastX = lastX;
            info.maxYValue = newYDatas.Max();
            info.minYValue = newYDatas.Min();

            dataInfoList.Add(info);

            return true;
        }

        /// <summary>
        /// 简单创建光谱文件
        /// </summary>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="resolution">分辨率</param>
        /// <param name="yDatas">Y值</param>
        /// <param name="filetype">文件格式</param>
        /// <param name="spectype">光谱类型</param>
        /// <param name="ytype">Y轴类型</param>
        /// <param name="xtype">X轴类型</param>
        /// <returns>创建的文件数据</returns>
        public static byte[] SaveFile(double firstX, double lastX, double resolution, double[] yDatas, EnumFileType filetype = EnumFileType.SPC, SPECTYPE spectype = SPECTYPE.SPCNIR, YAXISTYPE ytype = YAXISTYPE.YABSRB, XAXISTYPE xtype = XAXISTYPE.XWAVEN)
        {
            if (yDatas == null || yDatas.Length == 0)
                return null;

            FileFormat fmt = new FileFormat();
            fmt.AddData(firstX, lastX, yDatas, spectype, xtype, ytype);
            fmt.fileInfo.resolution = resolution;
            fmt.fileInfo.createTime = DateTime.Now;
            fmt.fileInfo.dataCount = yDatas.Length;
            fmt.fileInfo.fileType = filetype;

            return fmt.SaveFile(filetype);
        }

        /// <summary>
        /// 获取支持文件的OpenFileDialog Filter
        /// </summary>
        public static string GetSupportFormatFilter()
        {
            string retstr = SPCFileFormat.HasAuthority() ? SPCFilter : null;
            if (OPUSFileFormat.HasAuthority())
            {
                retstr += retstr == null ? OPUSFilter : "|" + OPUSFilter;
            }
            if (SPAFileFormat.HasAuthority())
            {
                retstr += retstr == null ? SPAFilter : "|" + SPAFilter;
            }
            if (FossFileFormat.HasAuthority())
            {
                retstr += retstr == null ? FOSSFilter : "|" + FOSSFilter;
            }

            //始终支持JCAMP格式
            retstr += retstr == null ? DXFilter : "|" + DXFilter;

            return retstr;
        }

        /// <summary>
        /// 返回文件名对应的光谱格式类型
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件类型</returns>
        public static EnumFileType FileNameToSpecType(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return EnumFileType.UNKNOWN;

            string extstr = System.IO.Path.GetExtension(filename).ToUpper();
            if (string.IsNullOrWhiteSpace(extstr) || extstr.Length < 2)     //后缀包含'.'
                return EnumFileType.UNKNOWN;
            extstr = extstr.Substring(1);

            if (extstr == "SPC")
                return EnumFileType.SPC;
            else if (extstr == "SP")
                return EnumFileType.SP;
            else if (extstr == "SPA")
                return EnumFileType.SPA;
            else if (extstr == "NIR" || extstr == "CAL")
                return EnumFileType.FOSS;
            else if (extstr == "DX" || extstr == "JDX" || extstr == "JCM")
                return EnumFileType.JCAMP;
            else if (extstr == "MAT")
                return EnumFileType.MAT;
            else if (extstr == "XLS" || extstr == "XLSX")
                return EnumFileType.EXCEL;
            else if (extstr == "CSV")
                return EnumFileType.CSV;
            else if (extstr == "TXT")
                return EnumFileType.TXT;
            else
            {
                //OPUS需要判断最后是不是数字结尾
                extstr = extstr.Substring(1);
                int tempint = 0;
                if (int.TryParse(extstr, out tempint))
                    return EnumFileType.OPUS;
            }

            return EnumFileType.UNKNOWN;

        }

        /// <summary>
        /// 返回光谱格式对应的文件后缀
        /// </summary>
        /// <param name="sepcType">光谱格式</param>
        /// <returns>文件后缀</returns>
        public static string SpecTypeToFileExtension(EnumFileType sepcType)
        {
            if (sepcType == EnumFileType.UNKNOWN)
                return null;

            switch (sepcType)
            {
                case EnumFileType.SPC:
                    return ".SPC";
                case EnumFileType.OPUS:
                    return ".0";
                case EnumFileType.SPA:
                    return ".SPA";
                case EnumFileType.SP:
                    return ".SP";
                case EnumFileType.FOSS:
                    return ".NIR";
                case EnumFileType.JCAMP:
                    return ".DX";
                case EnumFileType.MAT:
                    return ".MAT";
                case EnumFileType.EXCEL:
                    return ".XLS";
                case EnumFileType.CSV:
                    return ".CSV";
                case EnumFileType.TXT:
                    return ".TXT";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取授权光谱格式列表
        /// </summary>
        /// <returns>授权光谱格式列表</returns>
        public static List<EnumFileType> GetAuthorityFormats()
        {
            List<EnumFileType> retList = new List<EnumFileType>();
            if (SPCFileFormat.HasAuthority())
                retList.Add(EnumFileType.SPC);
            if (OPUSFileFormat.HasAuthority())
                retList.Add(EnumFileType.OPUS);
            if (SPAFileFormat.HasAuthority())
                retList.Add(EnumFileType.SPA);
            if (FossFileFormat.HasAuthority())
                retList.Add(EnumFileType.FOSS);

            retList.Add(EnumFileType.JCAMP);

            return retList;
        }

        /// <summary>
        /// 获取光谱类型(红外，近红外，拉曼...)名称
        /// </summary>
        /// <param name="specType">光谱类型</param>
        /// <param name="chineseName">True：中文，False：英文</param>
        public static string GetSpecTypeName(SPECTYPE specType, bool chineseName)
        {
            switch (specType)
            {
                case SPECTYPE.SPCGEN:	    /* General SPC (could be anything) */
                    return chineseName ? "通用" : "General";
                case SPECTYPE.SPCGC:	    /* 气相色谱图 Gas Chromatogram */
                    return chineseName ? "气相色谱" : "Gas CG";
                case SPECTYPE.SPCCGM:	    /* 通用色谱图 General Chromatogram (same as SPCGEN with TCGRAM) */
                    return chineseName ? "通用色谱" : "General CG";
                case SPECTYPE.SPCHPLC:	/* HPLC色谱图 HPLCHPLC Chromatogram */
                    return chineseName ? "HPLC色谱" : "HPLCHPLC";
                case SPECTYPE.SPCFTIR:	    /* 傅立叶变换中红外，近红外，拉曼FT-IR, FT-NIR, FT-Raman Spectrum or Igram (Can also be used for scanning IR.) */
                    return chineseName ? "傅立叶变换" : "FT-IR/NIR/RAMAN";
                case SPECTYPE.SPCNIR:	    /* 近红外 NIR Spectrum (Usually multi-spectral data sets for calibration.) */
                    return chineseName ? "近红外" : "NIR";
                case SPECTYPE.SPCUV:	    /* UV-VIS Spectrum (Can be used for single scanning UV-VIS-NIR.) */
                    return chineseName ? "紫外" : "UV-VIS";
                case SPECTYPE.SPCXRY:	    /* X-射线 X-ray Diffraction Spectrum */
                    return chineseName ? "X-射线" : "X-ray";
                case SPECTYPE.SPCMS:	    /* 多种光谱 Mass Spectrum  (Can be single, GC-MS, Continuum, Centroid or TOF.) */
                    return chineseName ? "质谱" : "Mass";
                case SPECTYPE.SPCNMR:	/* 核磁 NMR Spectrum or FID */
                    return chineseName ? "核磁" : "NMR";
                case SPECTYPE.SPCRMN:	/* 拉曼谱 Raman Spectrum (Usually Diode Array, CCD, etc. use SPCFTIR for FT-Raman.) */
                    return chineseName ? "拉曼" : "Raman";
                case SPECTYPE.SPCFLR:	/* 荧光 Fluorescence Spectrum */
                    return chineseName ? "荧光" : "Fluorescence";
                case SPECTYPE.SPCATM:	/* 原子 Atomic Spectrum */
                    return chineseName ? "原子" : "Atomic";
                case SPECTYPE.SPCDAD:	    /* 色谱 Chromatography Diode Array Spectra */
                    return chineseName ? "色谱" : "CG";
                case SPECTYPE.SPCMIR:	    /* 中红外谱*/
                    return chineseName ? "中红外" : "MIR";
                default:
                    return chineseName ? "未知" : "Unknown";
            }
        }

        /// <summary>
        /// 获取X坐标名称
        /// </summary>
        /// <param name="xAxisType">X轴类型</param>
        /// <param name="chineseName">True：中文，False：英文</param>
        public static string GetXAxisTypeName(XAXISTYPE xAxisType, bool chineseName)
        {
            switch (xAxisType)
            {
                case XAXISTYPE.XARB:	/* Arbitrary */
                    return chineseName ? "未知" : "Arbitrary";
                case XAXISTYPE.XWAVEN:	/* Wavenumber (cm-1) */
                    return chineseName ? "波数" : "Wavenumber";
                case XAXISTYPE.XUMETR:	/* Micrometers (um) */
                    return chineseName ? "毫米" : "Micrometers";
                case XAXISTYPE.XNMETR:	/* Nanometers (nm) */
                    return chineseName ? "纳米" : "Nanometers";
                case XAXISTYPE.XSECS:	/* Seconds */
                    return chineseName ? "秒钟" : "Seconds";
                case XAXISTYPE.XMINUTS:	/* Minutes */
                    return chineseName ? "分钟" : "Minutes";
                case XAXISTYPE.XHERTZ:	/* Hertz (Hz) */
                    return chineseName ? "赫兹" : "Hertz";
                case XAXISTYPE.XKHERTZ:	/* Kilohertz (KHz) */
                    return chineseName ? "千赫兹" : "KHz";
                case XAXISTYPE.XMHERTZ:	/* Megahertz (MHz) */
                    return chineseName ? "兆赫兹" : "MHz";
                case XAXISTYPE.XMUNITS:	/* Mass (M/z) */
                    return chineseName ? "M/z" : "M/z";
                case XAXISTYPE.XPPM:	/* Parts per million (PPM) */
                    return chineseName ? "PPM" : "PPM";
                case XAXISTYPE.XDAYS:	/* Days */
                    return chineseName ? "天" : "Days";
                case XAXISTYPE.XYEARS:	/* Years */
                    return chineseName ? "年" : "Years";
                case XAXISTYPE.XRAMANS:	    /* Raman Shift (cm-1) */
                    return chineseName ? "拉曼位移" : "Raman Shift";
                default:
                    return chineseName ? "未知" : "Unknown";
            }
        }

        /// <summary>
        /// 获取X坐标名称
        /// </summary>
        /// <param name="yAxisType">X轴类型</param>
        /// <param name="chineseName">True：中文，False：英文</param>
        public static string GetYAxisTypeName(YAXISTYPE yAxisType, bool chineseName)
        {
            switch (yAxisType)
            {
                case YAXISTYPE.YARB:	/* Arbitrary Intensity */
                    return chineseName ? "未知" : "Arbitrary";
                case YAXISTYPE.YIGRAM:	/* Interferogram */
                    return chineseName ? "干涉谱" : "Interferogram";
                case YAXISTYPE.YABSRB:	/* Absorbance */
                    return chineseName ? "吸收谱" : "Absorbance";
                case YAXISTYPE.YKMONK:	/* Kubelka-Monk */
                    return chineseName ? "K-M谱" : "Kubelka-Monk";
                case YAXISTYPE.YCOUNT:	/* Counts */
                    return chineseName ? "数量" : "Counts";
                case YAXISTYPE.YVOLTS:	/* Volts */
                    return chineseName ? "伏" : "Volts";
                case YAXISTYPE.YDEGRS:	/* Degrees */
                    return chineseName ? "度" : "Degrees";
                case YAXISTYPE.YAMPS:	/* Milliamps */
                    return chineseName ? "毫安" : "Milliamps";
                case YAXISTYPE.YMETERS:	/* Millimeters */
                    return chineseName ? "毫米" : "Millimeters";
                case YAXISTYPE.YMVOLTS:	/* Millivolts */
                    return chineseName ? "毫伏" : "Millivolts";
                case YAXISTYPE.YLOGDR:	/* Log(1/R) */
                    return chineseName ? "Log(1/R)" : "Log(1/R)";
                case YAXISTYPE.YPERCNT:	/* Percent */
                    return chineseName ? "百分比" : "Percent";
                case YAXISTYPE.YINTENS:	/* Intensity */
                    return chineseName ? "强度" : "Intensity";
                case YAXISTYPE.YRELINT:	/* Relative Intensity */
                    return chineseName ? "相对强度" : "Relative Intensity";
                case YAXISTYPE.YENERGY:	/* Energy */
                    return chineseName ? "能量" : "Energy";
                case YAXISTYPE.YDECBL:	/* Decibel */
                    return chineseName ? "分贝" : "Decibel";
                case YAXISTYPE.YDEGRF:	/* Temperature (F) */
                    return chineseName ? "华氏温度" : "Temperature(F)";
                case YAXISTYPE.YDEGRC:	/* Temperature (C) */
                    return chineseName ? "摄氏温度" : "Temperature(C)";
                case YAXISTYPE.YDEGRK:	/* Temperature (K) */
                    return chineseName ? "开氏温度" : "Temperature(K)";
                case YAXISTYPE.YINDRF:	/* Index of Refraction [N] */
                    return chineseName ? "折射" : "Index of Refraction [N]";
                case YAXISTYPE.YEXTCF:	/* Extinction Coeff. [K] */
                    return chineseName ? "消失系数" : "Extinction Coeff. [K]";
                case YAXISTYPE.YREAL:	/* Real */
                    return chineseName ? "实数" : "Real";
                case YAXISTYPE.YIMAG:	/* Imaginary */
                    return chineseName ? "虚数" : "Imaginary";
                case YAXISTYPE.YCMPLX:	/* Complex */
                    return chineseName ? "复数" : "Complex";
                case YAXISTYPE.YTRANS:	/* Transmission (ALL HIGHER MUST HAVE VALLEYS!) */
                    return chineseName ? "透射" : "Transmission";
                case YAXISTYPE.YREFLEC:	/* Reflectance */
                    return chineseName ? "反射" : "Reflectance";
                case YAXISTYPE.YVALLEY:	/* Arbitrary or Single Beam with Valley Peaks */
                    return chineseName ? "单通道" : "Single Beam";
                case YAXISTYPE.YEMISN:	/* Emission */
                    return chineseName ? "发射" : "Emission";
                case YAXISTYPE.YATR:	/* Emission */
                    return chineseName ? "ATR" : "OPUS ATR";
                case YAXISTYPE.YSCRF:	/* Emission */
                    return chineseName ? "单通道背景" : "OPUS SCRF";
                case YAXISTYPE.YSCSM:	/* Emission */
                    return chineseName ? "单通道样品" : "OPUS SCSM";
                case YAXISTYPE.YIGRF:	/* Emission */
                    return chineseName ? "背景干涉图" : "OPUS IGRF";
                case YAXISTYPE.YIGSM:	/* Emission */
                    return chineseName ? "样品干涉图" : "OPUS IGSM";
                case YAXISTYPE.YRPXL:	/* Emission */
                    return chineseName ? "拉曼像素" : "RAMAN PIXEL";
                default:
                    return chineseName ? "未知" : "Unknown";
            }
        }


        /// <summary>
        /// 获取红外光谱的名称
        /// </summary>
        /// <param name="irMode">光谱类型</param>
        /// <param name="chineseName">True：中文，False：英文</param>
        public static string GeIrModeName(enumIRMODE irMode, bool chineseName)
        {
            switch (irMode)
            {
                case enumIRMODE.NearIR:
                    return chineseName ? "近红外" : "NearIR";
                case enumIRMODE.MidIR:
                    return chineseName ? "中红外" : "MidIR";
                case enumIRMODE.RamanShift:
                    return chineseName ? "拉曼" : "RamanShift";
                default:
                    return chineseName ? "未知" : "Unknown";
            }
        }

        /// <summary>
        /// 读取文件中组分列表
        /// <param name="fileData">文件内容</param>
        /// </summary>
        public static string[] GetComponentName(byte[] fileData)
        {
            return FossFileFormat.GetComponentName(fileData);
        }

        /// <summary>
        /// 读取浓度数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="specIndex">光谱序号</param>
        /// <returns>返回光谱浓度数据</returns>
        public static double[] GetComponentData(byte[] fileData, int specIndex)
        {
            return FossFileFormat.GetComponentData(fileData, specIndex);
        }

        /// <summary>
        /// 获取指定Y轴格式的数据索引（吸收谱，干涉谱...)
        /// </summary>
        /// <param name="dataType">Y轴数据类型（吸收谱，干涉谱...)</param>
        /// <returns>在dataInfoList中的索引，找不到返回-1</returns>
        public int GetDataTypeIndex(YAXISTYPE dataType)
        {
            var info = dataInfoList.FirstOrDefault(p => p.dataType == dataType);
            if (info == null)
                return -1;
            else
                return dataInfoList.IndexOf(info);
        }

        /// <summary>
        /// 获取指定格式的X轴数据（吸收谱，干涉谱...)
        /// </summary>
        /// <param name="dataType">数据类型（吸收谱，干涉谱...)</param>
        /// <returns>X轴数据</returns>
        public double[] GetXDatasByType(YAXISTYPE dataType)
        {
            int index = GetDataTypeIndex(dataType);
            if (index == -1)
                return null;
            else
                return index >= xDataList.Count ? xDatas : xDataList[index];
        }

        /// <summary>
        /// 获取指定格式的Y轴数据（吸收谱，干涉谱...)
        /// </summary>
        /// <param name="dataType">数据类型（吸收谱，干涉谱...)</param>
        /// <returns>Y轴数据</returns>
        public double[] GetYDatasByType(YAXISTYPE dataType)
        {
            int index = GetDataTypeIndex(dataType);
            if (index == -1)
                return null;
            else
                return index >= yDataList.Count ? null: yDataList[index];
        }

        /// <summary>
        /// 获取指定格式的数据信息（吸收谱，干涉谱...)
        /// </summary>
        /// <param name="dataType">数据类型（吸收谱，干涉谱...)</param>
        /// <returns>数据信息</returns>
        public DataInfo GetDataInfoByType(YAXISTYPE dataType)
        {
            int index = GetDataTypeIndex(dataType);
            if (index == -1)
                return null;
            else
                return dataInfoList[index];
        }

        /// <summary>
        /// 仅仅保留某个类型的Y轴数据
        /// </summary>
        /// <param name="dataType">Y轴数据类型</param>
        public bool OnlyKeepData(YAXISTYPE dataType)
        {
            var xdata = GetXDatasByType(dataType);
            var ydata = GetYDatasByType(dataType);
            var datainfo = GetDataInfoByType(dataType);
            if (xdata == null || ydata == null || datainfo == null)
                return false;

            xDataList.Clear();
            xDataList.Add(xdata);

            yDataList.Clear();
            yDataList.Add(ydata);

            dataInfoList.Clear();
            dataInfoList.Add(datainfo);

            return true;
        }
    }

    /// <summary>
    /// 定量模型文件信息
    /// </summary>
    public class QuantModelFormat
    {
        /// <summary>
        /// Vspec模型
        /// </summary>
        public const string VSpecFilter = "VModel Model|*.vpls";
        /// <summary>
        /// Thermo模型
        /// </summary>
        public const string ThermoFilter = "Thermo Model|*.qnt";
        /// <summary>
        /// Bruker模型
        /// </summary>
        public const string BRUKERFilter = "BURKER Mode|*.q2";

        #region C++通用函数

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern IntPtr GetErrorMessage32();

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern IntPtr GetErrorMessage64();

        /// <summary>
        /// 获取DLL错误消息
        /// </summary>
        /// <returns></returns>
        public static string GetDLLErrorMessage()
        {
            IntPtr retptr = IntPtr.Zero;

            if (CommonMethod.Is64BitVersion())
                retptr = GetErrorMessage64();
            else
                retptr = GetErrorMessage32();

            return CommonMethod.CopyStringFromIntptrAndFree(ref retptr, Encoding.ASCII);
        }
        #endregion

        /// <summary>
        /// 模型参数
        /// </summary>
        public class QuantModelParameter
        {
            /// <summary>
            /// 全部光谱数量
            /// </summary>
            public int allFileCount;
            /// <summary>
            /// 建模光谱数量
            /// </summary>
            public int calFileCount;
            /// <summary>
            /// 内部验证光谱数量
            /// </summary>
            public int innerTestFileCount;
            /// <summary>
            /// 外部验证光谱数量
            /// </summary>
            public int outerTestFileCount;
            /// <summary>
            /// 光谱文件X轴起始波数
            /// </summary>
            public double fileFirstX;		//原始光谱X轴起始波数
            /// <summary>
            /// 光谱文件X轴结束波数
            /// </summary>
            public double fileLastX;		//原始光谱X轴结束波数
            /// <summary>
            /// 光谱文件数据点数
            /// </summary>
            public double fileSpecCols;
            /// <summary>
            /// 光谱文件X轴数据间隔
            /// </summary>
            public double fileXStep;		//X轴数据间隔
            /// <summary>
            /// 模型X轴起始波数
            /// </summary>
            public double modelFirstX;		//建模X轴起始波数
            /// <summary>
            /// 模型X轴结束波数
            /// </summary>
            public double modelLastX;		//建模X轴结束波数
            /// <summary>
            ///模型数据点数量
            /// </summary>
            public int modelSpecCols;
            /// <summary>
            /// 模型X轴数据间隔
            /// </summary>
            public double modelStepX;		//X轴数据间隔
        };

        /// <summary>
        /// 组分信息
        /// </summary>
        public class QuantComponentInfo
        {
            /// <summary>
            /// 组分名称
            /// </summary>
            public string name;
            /// <summary>
            /// 组分单位
            /// </summary>
            public string unit;
            /// <summary>
            /// 平均马氏距离
            /// </summary>
            public double mahalDistance;
            /// <summary>
            /// 最小浓度
            /// </summary>
            public double lowestConcentration;
            /// <summary>
            /// 最大浓度
            /// </summary>
            public double highestConcentration;
        };

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorString { get; set; }

        /// <summary>
        /// 模型参数
        /// </summary>
        public QuantModelParameter modelParameter { get; private set; }

        /// <summary>
        /// 模型组分列表
        /// </summary>
        public List<QuantComponentInfo> modelComponents { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileData">模型数据</param>
        public QuantModelFormat(byte[] fileData)
        {
            GetModelInfo(fileData);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">模型文件名</param>
        public QuantModelFormat(string filename)
        {
            var fileData = CommonMethod.ReadBinaryFile(filename);            
            GetModelInfo(fileData);
        }

        /// <summary>
        /// 获取模型参数和组分列表
        /// </summary>
        /// <param name="fileData">模型数据</param>
        private void GetModelInfo(byte[] fileData)
        {
            if (fileData == null || fileData.Length == 0)
            {
                ErrorString = "Invalid parameter";
                return;
            }

            modelParameter = GetModelParameter(fileData);
            if (modelParameter != null)
                modelComponents = GetComponentList(fileData);
        }

        /// <summary>
        /// 获取模型参数
        /// </summary>
        /// <param name="fileData">模型数据</param>
        /// <returns>模型参数信息</returns>
        private QuantModelParameter GetModelParameter(byte[] fileData)
        {
            if (OPUSFileFormat.HasAuthority() && OPUSFileFormat.IsOPUSFile(fileData, fileData.Length))
            {
                return OPUSFileFormat.GetModelParameter(fileData);
            }
            else if (SPAFileFormat.HasAuthority() && SPAFileFormat.IsSPAFile(fileData, fileData.Length))
            {
                return SPAFileFormat.GetModelParameter(fileData);
            }

            return null;
        }

        /// <summary>
        /// 获取模型组分列表
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        private List<QuantComponentInfo> GetComponentList(byte[] fileData)
        {
            if (OPUSFileFormat.HasAuthority() && OPUSFileFormat.IsOPUSFile(fileData, fileData.Length))
            {
                return OPUSFileFormat.GetComponentList(fileData);
            }
            else if (SPAFileFormat.HasAuthority() && SPAFileFormat.IsSPAFile(fileData, fileData.Length))
            {
                return SPAFileFormat.GetComponentList(fileData);
            }

            return null;
        }

    }
}
