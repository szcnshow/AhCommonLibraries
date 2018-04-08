using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.FileFormat
{
    unsafe class OPUSFileFormat
    {
        private const int ENUMSZ=20;

        public static string ErrorString = null;

        #region OPUSHeaderDefine

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=1)]
        unsafe struct DataPara
        {
            public Int32 DPF;		//1 = REAL32, 2 = INT32
            public Int32 NPT;		//INT32      number of data points
            public double FXV;		//REAL64     first X value
            public double LXV;		//REAL64     last X value
            public double CSF;		//REAL64     common factor for all Y - values
            public double MXY;		//REAL64     maximum Y value
            public double MNY;		//REAL64     minimum Y value
            public UInt32 DER;		//INT32      derivative(0, 1, 2, ..)
            public UInt32 paraType;		//参数类型
            public UInt32 dataType;		//光谱类型
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string DXU;	//ENUM       data x - units(WN=wavenumber cm-1, MI=micron, LGW=log wavenumber, MIN=minutes, PNT=points)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string DYU;	//ENUM       data y - units(SC=single channel,TR=transmission,AB=absorbance,KM=Kubelka-Munk,LA=-log(AB),DR=diffuse reflectance)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string DAT;		//STRING     Date of measurement
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string TIM;		//STRING     Time of measurement
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string XTX;		//STRING     Text describing the X-axis units, optional
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string YTX;		//STRING     Text describing the Y-axis units, optional
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct AcquisitionPara
        {
            public Int32 DLY;			//Delay before mesurement
            public Int32 GSW;			//INT32     Gain switch window
            public double HFW;			//REAL64    High frequency limit
            public double LFW;			//Low frequency limit
            public Int32 NSS;			//Number of sample scans
            public Int32 PGN;		//INT32     Programmed gain(ifs120)
            public double RES;		//Resolution
            public double RLP;		//Raman Laser Power
            public double RLW;		//Raman Laser Wavelength
            public Int32 SGN;		//Main amplifier gain, sample
            public Int32 SNR;		//Wheel position, sample measurement
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string ITF;	//ENUM      Interface type
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string sim;	//ENUM      Simulation mode
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string APT;	//ENUM      Aperture setting
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string AQM;	//Aquisition mode
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string BMS;    //ENUM      Beamsplitter setting
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string COR;	//ENUM      Correlation test mode
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string DTC;	//Detector setting
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string GSG;	//Gain switch gain
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string HPF;	//High pass filter
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string LPF;	//ENUM      Low pass filter
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string OPF;	//ENUM      optical filter setting
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string SCH;	//Sample measurement channel
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string SRC;	//Source setting
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string VEL;	//ENUM      scanner velocity
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct InstrumentPara
        {
            public double LFL;	//Low folding limit
            public double HFL;	//High folding limit
            public double LWN;			//REAL64    Laser wavenumber
            public Int32 ASG;   //Actual signal gain
            public Int32 ALF;	//Actual low - pass filter
            public Int32 AHF;	//Actual high - pass filter
            public Int32 ASS;	//Actual number of sample scans
            public Int32 RSN;	//Running sample number
            public Int32 PKA;	//Peak amplitude
            public Int32 PKL;	//Peak location
            public Int32 ssm;	//Sample spacing multiplicator
            public Int32 SSP;	//Sample spacing divisor
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string INS;	//Instrument type
        };

        [StructLayout(LayoutKind.Sequential)]
        struct FTPara
        {
            public double HFQ;		//REAL64     High frequency cutoff
            public double LFQ;		//REAL64     Low frequency cutoff
            public Int32 PIP;		//INT32      Igram points for phase calc.
            public Int32 PTS;		//INT32      Phase transform size
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string APF;	//ENUM       Apodization function
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string PHZ;	//ENUM       Phase correction mode
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string SPZ;	//ENUM       Stored phase mode
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ENUMSZ)]
            public string ZFF;	//ENUM       Zero filling factor
        };

        /// <summary>
        /// Quant2模型参数信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        struct QuantModelInfo
        {
            Int16 word1;		//0x0900
            /// <summary>
            /// 光谱文件列表的大小
            /// </summary>
            public Int32 fileListSize;		
            Int16 word2;		//0x0100
            Int16 word3;		//0x0700
            Int16 word4;		//0x0000
            /// <summary>
            /// 全部光谱数量
            /// </summary>
            public Int16 spectrumCount;		//0x00C0,全部光谱数量
            public Int16 compCount;			//0x0300,建模组分数量
            Int16 word5;		//0x0000
            /// <summary>
            /// 建模的数据点数量
            /// </summary>
            public Int32 modelSpecCols;		//0x0000081A,建模的数据点数量
            Int32 dword2;		//0000100F
            Int16 word6;		//0000100F
            Int32 dword3;		//00000000
            Int16 word7;		//0x0000
            double double1;		//0x47EFFFFFE0000000
            double double2;		//0x47EFFFFFE0000000
            Int32 dword4;		//0x0000000000
            Int16 word8;		//00000011
            Int32 dword5;		//00000011
            /// <summary>
            /// 光谱文件X轴数据间隔
            /// </summary>
            public double fileStepX;		//X轴数据间隔
            /// <summary>
            /// 模型X轴数据间隔
            /// </summary>
            public double modelStepX;		//X轴数据间隔
            /// <summary>
            /// 原始光谱X轴起始波数
            /// </summary>
            public double specFirstX;		//原始光谱X轴起始波数
            /// <summary>
            /// 原始光谱X轴结束波数
            /// </summary>
            public double specLastX;		//原始光谱X轴结束波数
            /// <summary>
            /// 建模X轴起始波数
            /// </summary>
            public double modelFirstX;		//建模X轴起始波数
            /// <summary>
            /// 建模X轴结束波数
            /// </summary>
            public double modelLastX;		//建模X轴结束波数
            /// <summary>
            /// 建模数据点的间隔
            /// </summary>
            public Int16 dataSplit;		//建模数据点的间隔
            Int16 word9;		//0x0001
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct QuantComponentInfo
        {
            /// <summary>
            /// 组分名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] name;
            /// <summary>
            /// 组分单位
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] unit;
            Int16 word1;    //未知
        };

        // 参数信息类型
        private const UInt32 ABParaType = 0x101F;
        private const UInt32 ScSmParaType = 0x0417;
        private const UInt32 ScRfParaType = 0x041B;
        private const UInt32 IgRfParaType = 0x081B;
        private const UInt32 IgSmParaType = 0x0817;
        private const UInt32 ATRParaType = 0x401F;
        private const UInt32 RAMANParaType = 0x281F;

        //数据类型
        private const UInt32 ABDataType = 0x100F;
        private const UInt32 ScSmDataType = 0x0407;
        private const UInt32 ScRfDataType = 0x040B;
        private const UInt32 IgRfDataType = 0x080B;
        private const UInt32 IgSmDataType = 0x0807;
        private const UInt32 ATRDataType = 0x400F;
        private const UInt32 RAMANDataType = 0x280F;


        #endregion

        #region 32bitDLL
        /// <summary>
        /// 是否授权SPC文件
        /// </summary>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSHasAuthority")]
        private static extern bool OPUSHasAuthority32();

        /// <summary>
        /// 是否为OPUS文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSIsOPUSFile")]
        private static extern bool OPUSIsOPUSFile32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取采集参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>AcquisitionPara</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetAcquisitionParameter")]
        private static extern IntPtr OPUSGetAcquisitionParameter32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取仪器参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>InstrumentPara</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetInstrumentParameter")]
        private static extern IntPtr OPUSGetInstrumentParameter32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取仪器参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>FTPara</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetFTParameter")]
        private static extern IntPtr OPUSGetFTParameter32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取光谱数量
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>光谱数量</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetSpectrumCount")]
        private static extern int OPUSGetSpectrumCount32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取数据参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="dataType">光谱类型(AB, SmSc, RfSc, IgSm, IgRf..)</param>
        /// <returns>DataPara</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetDataParameter")]
        private static extern IntPtr OPUSGetDataParameter32(byte[] fileData, int fileSize, UInt32 dataType);

        /// <summary>
        /// 获取光谱数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="dataType">光谱类型(AB, SmSc, RfSc, IgSm, IgRf..)</param>
        /// <param name="outDataSize">返回数据的数量(double)</param>
        /// <param name="valueType">值的类型(1=float, 2=int)</param>
        /// <param name="yFactor">Y轴放大系数</param>
        /// <returns>double[]</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetYDatas")]
        private static extern IntPtr OPUSGetYDatas32(byte[] fileData, int fileSize, UInt32 dataType, int valueType, double yFactor, ref int outDataSize);

        /// <summary>
        /// 创建OPUS文件
        /// </summary>
        /// <param name="instPara">仪器参数</param>
        /// <param name="acquPara">采集参数</param>
        /// <param name="ftPara">FT变换参数</param>
        /// <param name="specCount">光谱数量</param>
        /// <param name="dataPara">光谱参数(与specCount相等，里面包括每条光谱的参数类型和光谱类型)</param>
        /// <param name="yDatas">(一个或多个)Y轴数据，每组数据数量与对应的dataPara->NPT相等</param>
        /// <param name="logBinary">私有信息</param>
        /// <param name="logBinarySize">私有信息大小</param>
        /// <param name="outDataSize">返回数据大小（BYTE）</param>
        /// <returns>OPUS格式数据BYTE</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSCreateFile")]
        private static extern IntPtr OPUSCreateFile32(ref InstrumentPara instPara, ref AcquisitionPara acquPara, ref FTPara ftPara, int specCount, byte[] dataPara, double[] yDatas, byte[] logBinary, int logBinarySize, ref int outDataSize);

        /// <summary>
        /// 获取模型信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>QuantModelInfo</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetQuantModelInfo")]
        private static extern IntPtr OPUSGetQuantModelInfo32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取模型组分信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="componentCount">组分数量</param>
        /// <returns>多个QuantComponentInfo</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetQuantComponentInfo")]
        private static extern IntPtr OPUSGetQuantComponentInfo32(byte[] fileData, int fileSize, ref int componentCount);

        #endregion

        #region 64bitDLL
        /// <summary>
        /// 是否授权SPC文件
        /// </summary>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSHasAuthority")]
        private static extern bool OPUSHasAuthority64();

        /// <summary>
        /// 是否为OPUS文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSIsOPUSFile")]
        private static extern bool OPUSIsOPUSFile64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取采集参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>AcquisitionPara</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetAcquisitionParameter")]
        private static extern IntPtr OPUSGetAcquisitionParameter64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取仪器参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>InstrumentPara</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetInstrumentParameter")]
        private static extern IntPtr OPUSGetInstrumentParameter64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取仪器参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>FTPara</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetFTParameter")]
        private static extern IntPtr OPUSGetFTParameter64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取光谱数量
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>光谱数量</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetSpectrumCount")]
        private static extern int OPUSGetSpectrumCount64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取数据参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="dataType">光谱类型(AB, SmSc, RfSc, IgSm, IgRf..)</param>
        /// <returns>DataPara</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetDataParameter")]
        private static extern IntPtr OPUSGetDataParameter64(byte[] fileData, int fileSize, UInt32 dataType);

        /// <summary>
        /// 获取光谱数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="dataType">光谱类型(AB, SmSc, RfSc, IgSm, IgRf..)</param>
        /// <param name="outDataSize">返回数据的数量(double)</param>
        /// <param name="valueType">Y轴数据类型</param>
        /// <param name="yFactor">Y轴放大系数</param>
        /// <returns>double[]</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetYDatas")]
        private static extern IntPtr OPUSGetYDatas64(byte[] fileData, int fileSize, UInt32 dataType, int valueType, double yFactor, ref int outDataSize);

        /// <summary>
        /// 创建OPUS文件
        /// </summary>
        /// <param name="instPara">仪器参数</param>
        /// <param name="acquPara">采集参数</param>
        /// <param name="ftPara">FT变换参数</param>
        /// <param name="specCount">光谱数量</param>
        /// <param name="dataPara">光谱参数(与specCount相等，里面包括每条光谱的参数类型和光谱类型)</param>
        /// <param name="yDatas">(一个或多个)Y轴数据，每组数据数量与对应的dataPara->NPT相等</param>
        /// <param name="logBinary">私有信息</param>
        /// <param name="logBinarySize">私有信息大小</param>
        /// <param name="outDataSize">返回数据大小（BYTE）</param>
        /// <returns>OPUS格式数据BYTE</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSCreateFile")]
        private static extern IntPtr OPUSCreateFile64(ref InstrumentPara instPara, ref AcquisitionPara acquPara, ref FTPara ftPara, int specCount, byte[] dataPara, double[] yDatas, byte[] logBinary, int logBinarySize, ref int outDataSize);

        /// <summary>
        /// 获取模型信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>QuantModelInfo</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetQuantModelInfo")]
        private static extern IntPtr OPUSGetQuantModelInfo64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取模型组分信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="componentCount">组分数量</param>
        /// <returns>多个QuantComponentInfo</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPUSGetQuantComponentInfo")]
        private static extern IntPtr OPUSGetQuantComponentInfo64(byte[] fileData, int fileSize, ref int componentCount);

        #endregion

        #region 32/64Bit自适应
        /// <summary>
        /// 是否授权为SPC文件
        /// </summary>
        public static bool HasAuthority()
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return OPUSHasAuthority64();
                else
                    return OPUSHasAuthority32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 是否为OPUS文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        public static bool IsOPUSFile(byte[] fileData, int fileSize)
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return OPUSIsOPUSFile64(fileData, fileSize);
                else
                    return OPUSIsOPUSFile32(fileData, fileSize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取采集参数(DLY=int.MaxValue; 表示错误数据)
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>AcquisitionPara</returns>
        private static AcquisitionPara GetAcquisitionParameter(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = OPUSGetAcquisitionParameter64(fileData, fileSize);
                else
                    retptr = OPUSGetAcquisitionParameter32(fileData, fileSize);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                bool retOK;
                AcquisitionPara retheader = CommonMethod.CopyStructureFromIntptrAndFree<AcquisitionPara>(ref retptr, out retOK);
                if (!retOK)
                    retheader.DLY = int.MaxValue;     //表示错误数据

                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                AcquisitionPara retheader = new AcquisitionPara();
                retheader.DLY = int.MaxValue;     //表示错误数据
                return retheader;
            }

        }

        /// <summary>
        /// 获取仪器参数（RSN = int.MaxValue 表示错误数据）
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>InstrumentPara</returns>
        private static InstrumentPara GetInstrumentParameter(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = OPUSGetInstrumentParameter64(fileData, fileSize);
                else
                    retptr = OPUSGetInstrumentParameter32(fileData, fileSize);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                bool retOK;
                InstrumentPara retheader = CommonMethod.CopyStructureFromIntptrAndFree<InstrumentPara>(ref retptr, out retOK);
                if (!retOK)
                    retheader.RSN = int.MaxValue;     //表示错误数据

                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                InstrumentPara retheader = new InstrumentPara();
                retheader.RSN = int.MaxValue;       //表示错误数据
                return retheader;
            }

        }

        /// <summary>
        /// 获取FT参数（PIP = int.MaxValue表示错误）
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>FTPara</returns>
        private static FTPara GetFTParameter(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = OPUSGetFTParameter64(fileData, fileSize);
                else
                    retptr = OPUSGetFTParameter32(fileData, fileSize);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                bool retOK;
                FTPara retheader = CommonMethod.CopyStructureFromIntptrAndFree<FTPara>(ref retptr, out retOK);
                if (!retOK)
                    retheader.PIP = int.MaxValue;     //表示错误数据

                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                FTPara retheader = new FTPara();
                retheader.PIP = int.MaxValue;       //表示错误数据
                return retheader;
            }
        }

        /// <summary>
        /// 获取光谱数量
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        private static int GetSpectrumCount(byte[] fileData, int fileSize)
        {
            try
            {
            if (CommonMethod.Is64BitVersion())
                return OPUSGetSpectrumCount64(fileData, fileSize);
            else
                return OPUSGetSpectrumCount32(fileData, fileSize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// 获取光谱参数(NPT = int.MaxValue表示数据错误)
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="dataType">光谱类型</param>
        /// <returns>DataPara</returns>
        private static DataPara GetDataParameter(byte[] fileData, int fileSize, UInt32 dataType)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = OPUSGetDataParameter64(fileData, fileSize, dataType);
                else
                    retptr = OPUSGetDataParameter32(fileData, fileSize, dataType);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                bool retOK;
                DataPara retheader = CommonMethod.CopyStructureFromIntptrAndFree<DataPara>(ref retptr, out retOK);
                if (!retOK)
                    retheader.NPT = int.MaxValue;     //表示错误数据

                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                DataPara retheader = new DataPara();
                retheader.NPT = int.MaxValue;       //表示错误数据
                return retheader;
            }
        }

        /// <summary>
        /// 获取光谱参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="dataType">光谱类型</param>
        /// <param name="valueType">值的类型(1=float, 2=int)</param>
        /// <param name="yFactor">Y轴放大系数</param>
        /// <returns>DataPara</returns>
        private static double[] GetYDatas(byte[] fileData, int fileSize, UInt32 dataType, int valueType, double yFactor)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = OPUSGetYDatas64(fileData, fileSize, dataType, valueType, yFactor, ref datasize);
                else
                    retptr = OPUSGetYDatas32(fileData, fileSize, dataType, valueType, yFactor, ref datasize);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        private static byte[] CreateFile(InstrumentPara instPara, AcquisitionPara aqPara, FTPara ftPara, IList<DataPara> dataParas, IList<double[]> yDatas, byte[] addtionDatas)
        {
            try
            {
                if (dataParas == null || yDatas == null || dataParas.Count == 0 || dataParas.Count != yDatas.Count)
                    return null;

                //将所有Y轴数据拷贝到一起
                int ydatacount = (from p in yDatas select p.Length).Sum();
                double[] allYdatas = new double[ydatacount];
                int offset = 0;
                for (int i = 0; i < yDatas.Count; i++)
                {
                    Array.Copy(yDatas[i], 0, allYdatas, offset, yDatas[i].Length);
                    offset += yDatas[i].Length;
                }

                //将所有光谱数据参数拷贝到一起
                byte[] dataParaBytes = new byte[Marshal.SizeOf(typeof(DataPara)) * dataParas.Count];
                fixed (byte* pt = dataParaBytes)
                {
                    for (int i = 0; i < dataParas.Count; i++)
                    {
                        IntPtr ptr = new IntPtr(pt);
                        Marshal.StructureToPtr(dataParas[i], IntPtr.Add(ptr, i * Marshal.SizeOf(typeof(DataPara))), false);
                    }
                }

                IntPtr retPtr = IntPtr.Zero;
                int outDataSize = 0;
                if (CommonMethod.Is64BitVersion())
                    retPtr = OPUSCreateFile64(ref instPara, ref aqPara, ref ftPara, dataParas.Count, dataParaBytes, allYdatas, addtionDatas, addtionDatas == null ? 0 : addtionDatas.Length, ref outDataSize);
                else
                    retPtr = OPUSCreateFile32(ref instPara, ref aqPara, ref ftPara, dataParas.Count, dataParaBytes, allYdatas, addtionDatas, addtionDatas == null ? 0 : addtionDatas.Length, ref outDataSize);

                if (retPtr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                return CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retPtr, outDataSize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取采集参数(DLY=int.MaxValue; 表示错误数据)
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <returns>OPUSQuantModelInfo,fileListSize = int.MaxValue表示错误</returns>
        private static QuantModelInfo GetQuantModelInfo(byte[] fileData)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = OPUSGetQuantModelInfo64(fileData, fileData.Length);
                else
                    retptr = OPUSGetQuantModelInfo32(fileData, fileData.Length);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());
                
                bool retOK;
                QuantModelInfo retData = CommonMethod.CopyStructureFromIntptrAndFree<QuantModelInfo>(ref retptr, out retOK);
                if (!retOK)
                    retData.fileListSize = int.MaxValue;     //表示错误数据

                return retData;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                QuantModelInfo retData = new QuantModelInfo();
                retData.fileListSize = int.MaxValue;     //表示错误数据
                return retData;
            }

        }

        /// <summary>
        /// 获取采集参数(DLY=int.MaxValue; 表示错误数据)
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <returns>OPUSQuantModelInfo,fileListSize = int.MaxValue表示错误</returns>
        private static List<QuantComponentInfo> GetQuantComponentInfo(byte[] fileData)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int componentCount = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = OPUSGetQuantComponentInfo64(fileData, fileData.Length, ref componentCount);
                else
                    retptr = OPUSGetQuantComponentInfo32(fileData, fileData.Length, ref componentCount);
                if (retptr == IntPtr.Zero || componentCount == 0)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                return CommonMethod.CopyStructureListFromIntptrAndFree<QuantComponentInfo>(ref retptr, componentCount);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }

        }
        #endregion

        /// <summary>
        /// 通过firstX和lastX构建xDatas
        /// </summary>
        private static double[] GenerateXDatas(double firstX, double lastX, int dataCount)
        {
            double stepx = (lastX - firstX) / (dataCount - 1);

            double[] xdatas = new double[dataCount];
            for (int j = 0; j < dataCount; j++)
            {
                xdatas[j] = firstX + j * stepx;
            }

            return xdatas;

        }

        public static bool ReadFile(byte[] fileData, int fileSize, FileFormat fileFormat)
        {
            fileFormat.dataInfoList = null;
            fileFormat.acquisitionInfo = null;
            fileFormat.xDataList = null;
            fileFormat.fileInfo = null;
            fileFormat.yDataList = null;

            InstrumentPara instPara = GetInstrumentParameter(fileData, fileSize);
            AcquisitionPara aqPara = GetAcquisitionParameter(fileData, fileSize);
            FTPara ftPara = GetFTParameter(fileData, fileSize);

            if (instPara.RSN == int.MaxValue && aqPara.DLY == int.MaxValue && ftPara.PIP == int.MaxValue)
            {
                instPara.RSN = 0;   //临时使用，支持以前的格式
                //return false;
            }

            //填充光谱文件信息
            fileFormat.fileInfo = new FileFormat.FileInfo();
            if (instPara.RSN != int.MaxValue)
            {
                //fileFormat.fileInfo.fileMemo
                //fileFormat.fileInfo.fspare
                fileFormat.fileInfo.fzinc = 0.5f;
                //fileFormat.fileInfo.modifyFlag
                fileFormat.fileInfo.resolution = aqPara.RES;
                string sourcetpye = aqPara.SRC; //CommonMethod.CovertByteArrayToString(aqPara.SRC, Encoding.ASCII);
                fileFormat.fileInfo.specType = sourcetpye == "MIR" ? FileFormat.SPECTYPE.SPCMIR : FileFormat.SPECTYPE.SPCNIR;
                fileFormat.fileInfo.xType = FileFormat.XAXISTYPE.XWAVEN;
                fileFormat.fileInfo.zType = FileFormat.ZAXISTYPE.XARB;
                fileFormat.fileInfo.instDescription = instPara.INS;
            }

            //填充光谱采集信息
            fileFormat.acquisitionInfo = new FileFormat.AcquisitionInfo();
            if(aqPara.DLY != int.MaxValue && ftPara.PIP != int.MaxValue)
            {
                fileFormat.acquisitionInfo.APOD = ftPara.APF;
                //fileFormat.acquisitionInfo.AVGTIME
                fileFormat.acquisitionInfo.BDELAY = aqPara.DLY;
                //fileFormat.acquisitionInfo.BEAM = 
                fileFormat.acquisitionInfo.BSPLITTER = aqPara.BMS;  // CommonMethod.CovertByteArrayToString(aqPara.BMS, Encoding.ASCII);
                //fileFormat.acquisitionInfo.BSTOP
                //fileFormat.acquisitionInfo.CHANNEL = OPF
                fileFormat.acquisitionInfo.DET = aqPara.DTC;    // CommonMethod.CovertByteArrayToString(aqPara.DTC, Encoding.ASCII);
                fileFormat.acquisitionInfo.FILTER = aqPara.OPF; // CommonMethod.CovertByteArrayToString(aqPara.OPF, Encoding.ASCII);
                fileFormat.acquisitionInfo.GAIN = aqPara.SGN;
                fileFormat.acquisitionInfo.HIGHPASS = aqPara.HFW;
                //fileFormat.acquisitionInfo.IGRAMDIR =
                fileFormat.acquisitionInfo.IGRAMSIDE = aqPara.AQM; //CommonMethod.CovertByteArrayToString(aqPara.AQM, Encoding.ASCII);
                string sourcetpye = aqPara.SRC; // CommonMethod.CovertByteArrayToString(aqPara.SRC, Encoding.ASCII);
                fileFormat.acquisitionInfo.IRMODE = sourcetpye == "MIR" ? FileFormat.enumIRMODE.MidIR : FileFormat.enumIRMODE.NearIR;
                //fileFormat.acquisitionInfo.JSTOP = 
                fileFormat.acquisitionInfo.LOWPASS = aqPara.LFW;
                fileFormat.acquisitionInfo.LWN = instPara.LWN;
                //fileFormat.acquisitionInfo.NAMEBG
                fileFormat.acquisitionInfo.PHASE = ftPara.PHZ;
                fileFormat.acquisitionInfo.PHASEPTS = ftPara.PIP;
                fileFormat.acquisitionInfo.POLARIZER = ftPara.PTS;
                fileFormat.acquisitionInfo.PURGE = aqPara.DLY;
                fileFormat.acquisitionInfo.RAMANFREQ = aqPara.RLW;
                fileFormat.acquisitionInfo.RAMANPWR = aqPara.RLP;
                fileFormat.acquisitionInfo.SCANS = aqPara.NSS;
                //fileFormat.acquisitionInfo.SCANSBG
                fileFormat.acquisitionInfo.SDELAY = aqPara.DLY;
                //fileFormat.acquisitionInfo.SMOOTH
                //fileFormat.acquisitionInfo.SPEED
                fileFormat.acquisitionInfo.SRC = aqPara.SRC;    // CommonMethod.CovertByteArrayToString(aqPara.SRC, Encoding.ASCII);
                //fileFormat.acquisitionInfo.VELOCITY
                int tempint = 0;
                if (int.TryParse(ftPara.ZFF, out tempint))
                    fileFormat.acquisitionInfo.ZFF = tempint;
            }

            //读取光谱数据
            List<UInt32> datatypes = new List<UInt32> { ABDataType, ScSmDataType, ScRfDataType, IgRfDataType, IgSmDataType, ATRDataType, RAMANDataType };
            List<UInt32> paratypes = new List<UInt32> { ABParaType, ScSmParaType, ScRfParaType, IgRfParaType, IgSmParaType, ATRParaType, RAMANParaType };

            fileFormat.xDataList = new List<double[]>();
            fileFormat.yDataList = new List<double[]>();
            fileFormat.dataInfoList = new List<FileFormat.DataInfo>();

            fileFormat.fileInfo.createTime = new DateTime(1970, 1, 1);
            
            for (int i = 0; i < datatypes.Count; i++)
            {
                //光谱数据参数
                DataPara dataPara = GetDataParameter(fileData, fileSize, paratypes[i]);
                if (dataPara.NPT == int.MaxValue)
                    continue;

                double[] ydatas = GetYDatas(fileData, fileSize, datatypes[i], (int)dataPara.DPF, (double)dataPara.CSF);
                if (ydatas == null)
                    continue;

                if (ydatas.Length > dataPara.NPT)    //有可能从OPUS里面读取的数据点数大于实际点数(可能OPUS中数据有对齐的默认)
                    Array.Resize(ref ydatas, dataPara.NPT);
                else if (ydatas.Length < dataPara.NPT)
                    dataPara.NPT = ydatas.Length;
 
                //设置数据格式
                FileFormat.DataInfo info = new FileFormat.DataInfo();
                switch(datatypes[i])    //OPUS和SPC的文件格式不相同
                {
                    case ABDataType:
                        info.dataType = FileFormat.YAXISTYPE.YABSRB;
                        break;
                    case ScSmDataType:
                        info.dataType = FileFormat.YAXISTYPE.YSCSM;
                        break;
                    case ScRfDataType:
                        info.dataType = FileFormat.YAXISTYPE.YSCRF;
                        break;
                    case IgRfDataType:
                        info.dataType = FileFormat.YAXISTYPE.YIGRF;
                        break;
                    case IgSmDataType:
                        info.dataType = FileFormat.YAXISTYPE.YIGSM;
                        break;
                    case ATRDataType:
                        info.dataType = FileFormat.YAXISTYPE.YATR;
                        break;
                    case RAMANDataType:
                        info.dataType = FileFormat.YAXISTYPE.YEMISN;
                        break;
                    default:
                        info.dataType = FileFormat.YAXISTYPE.YARB;
                        break;
                }
                info.firstX = dataPara.FXV;
                info.lastX = dataPara.LXV;
                info.maxYValue = ydatas.Max();
                info.minYValue = ydatas.Min();

                if (fileFormat.fileInfo.dataCount == 0)     //设置为第一个光谱的数据点
                    fileFormat.fileInfo.dataCount = ydatas.Length;

                double[] xdatas = null;

                double stepx = (info.lastX - info.firstX) / (ydatas.Length - 1);

                //第一个X Data，直接添加
                if (fileFormat.xDataList.Count == 0)
                {
                    xdatas = GenerateXDatas(info.firstX, info.lastX, ydatas.Length);
                }
                else
                {
                    if (fileFormat.xDataList.Count > 1)  //已经超过一个X轴了,直接添加
                    {
                        xdatas = GenerateXDatas(info.firstX, info.lastX, ydatas.Length);
                    }
                    else  //判断是否与第一个X轴相同
                    {
                        double curstepx = Math.Abs((info.lastX - info.firstX) / (ydatas.Length - 1)) / 100; //偏差不超过1/100
                        if(fileFormat.yDatas.Length != ydatas.Length ||     //数据点数不同
                            Math.Abs(fileFormat.dataInfo.lastX - info.lastX ) > curstepx ||
                            Math.Abs(fileFormat.dataInfo.firstX - info.firstX ) > curstepx)
                        {
                            while (fileFormat.xDataList.Count < fileFormat.yDataList.Count)     //如果前面的X轴为公用的，必须补齐
                            {
                                double[] tempxdatas = new double[fileFormat.xDatas.Length];
                                Array.Copy(fileFormat.xDatas, tempxdatas, fileFormat.xDatas.Length);
                                fileFormat.xDataList.Add(tempxdatas);
                            }
                            xdatas = GenerateXDatas(info.firstX, info.lastX, ydatas.Length);
                        }
                    }
                }
                if (fileFormat.fileInfo.createTime.Year == 1970)
                {
                    string tempdatestr =  dataPara.DAT; //CommonMethod.CovertByteArrayToString( dataPara.DAT, Encoding.ASCII);
                    string temptimestr = dataPara.TIM; //CommonMethod.CovertByteArrayToString(dataPara.TIM, Encoding.ASCII);
                    if (tempdatestr!=null && tempdatestr.Length>1 && temptimestr!=null && temptimestr.Length>1)
                    {
                        string[] timestrs = temptimestr.Split('.');
                        if (timestrs!= null && timestrs.Length > 1)
                        {
                            tempdatestr += " " + timestrs[0];
                            DateTime tempdate;
                            System.Globalization.CultureInfo curlture = new System.Globalization.CultureInfo("fr-FR");
                            if (DateTime.TryParse(tempdatestr, curlture, System.Globalization.DateTimeStyles.AssumeLocal, out tempdate) == true)
                            {
                                fileFormat.fileInfo.createTime = tempdate;
                            }
                        }
                    }
                }

                fileFormat.yDataList.Add(ydatas);
                fileFormat.dataInfoList.Add(info);
                if (xdatas != null)
                    fileFormat.xDataList.Add(xdatas);
            }

            return fileFormat.xDatas != null && fileFormat.yDatas != null;
        }

        public static byte[] SaveFile(FileFormat fileFormat)
        {
            //仪器信息
            InstrumentPara instPara = new InstrumentPara();
            instPara.AHF = (int)fileFormat.acquisitionInfo.HIGHPASS;
            instPara.ALF = (int)fileFormat.acquisitionInfo.LOWPASS;
            //instPara.ASG
            instPara.ASS = fileFormat.acquisitionInfo.SCANS;
            //instPara.HFL = 
            instPara.INS = fileFormat.fileInfo.instDescription;
            //instPara.LFL
            instPara.LWN = fileFormat.acquisitionInfo.LWN;
            //instPara.PKA
            //instPara.PKL
            //instPara.ssm
            //instPara.SSP

            //采集信息
            AcquisitionPara aqPara = new AcquisitionPara();
            //aqPara.APT
            //aqPara.AQM
            aqPara.BMS = fileFormat.acquisitionInfo.BEAM;  //new byte[ENUMSZ];
            //aqPara.COR
            aqPara.DLY = (int)fileFormat.acquisitionInfo.BDELAY;     //转换为ms
            aqPara.DTC = fileFormat.acquisitionInfo.DET;
            //aqPara.GSG
            //aqPara.GSW
            //aqPara.HFW
            //aqPara.HPF
            //aqPara.ITF
            //aqPara.LFW
            //aqPara.LPF
            aqPara.NSS = fileFormat.acquisitionInfo.SCANS;
            //aqPara.OPF = fileFormat.acquisitionInfo.o
            aqPara.PGN = (int)fileFormat.acquisitionInfo.GAIN;
            aqPara.RES = fileFormat.fileInfo.resolution;
            aqPara.RLP = fileFormat.acquisitionInfo.RAMANPWR;
            aqPara.RLW = fileFormat.acquisitionInfo.RAMANFREQ;
            //aqPara.SCH = 
            aqPara.SGN = (int)fileFormat.acquisitionInfo.GAIN;
            //aqPara.sim
            //aqPara.SNR
            //aqPara.SRC = new byte[ENUMSZ];
            if (fileFormat.fileInfo.specType == FileFormat.SPECTYPE.SPCNIR)
                aqPara.SRC = "NIR"; //CommonMethod.ConvertStringToByteArray("NIR", aqPara.SRC, ENUMSZ, Encoding.ASCII);
            else if (fileFormat.fileInfo.specType == FileFormat.SPECTYPE.SPCMIR)
                aqPara.SRC = "MIR"; //CommonMethod.ConvertStringToByteArray("MIR", aqPara.SRC, ENUMSZ, Encoding.ASCII);
            else if (fileFormat.fileInfo.specType == FileFormat.SPECTYPE.SPCRMN)
                aqPara.SRC = "RAN"; //CommonMethod.ConvertStringToByteArray("RAN", aqPara.SRC, ENUMSZ, Encoding.ASCII);     
            //aqPara.VEL

            //FT信息
            FTPara ftPara = new FTPara();
            ftPara.APF = fileFormat.acquisitionInfo.APOD;
            ftPara.HFQ = fileFormat.acquisitionInfo.HIGHPASS;
            ftPara.LFQ = fileFormat.acquisitionInfo.LOWPASS;
            ftPara.PHZ = fileFormat.acquisitionInfo.PHASE;
            //ftPara.PIP
            //ftPara.PTS
            //ftPara.SPZ
            ftPara.ZFF = fileFormat.acquisitionInfo.ZFF.ToString();

            //光谱数据参数
            List<DataPara> dataParas = new List<DataPara>();
            for (int i = 0; i < fileFormat.dataInfoList.Count; i++)
            {
                DataPara para = new DataPara();

                para.CSF = 1;   //Y轴放大系数

                //光谱数据格式
                switch(fileFormat.dataInfoList[i].dataType)
                {
                    case FileFormat.YAXISTYPE.YABSRB:                
                        para.dataType = ABDataType;
                        para.paraType = ABParaType;
                        break;
                    case FileFormat.YAXISTYPE.YSCSM:
                        para.dataType = ScSmDataType;
                        para.paraType = ScSmParaType;
                        break;
                    case FileFormat.YAXISTYPE.YSCRF:
                        para.dataType = ScRfDataType;
                        para.paraType = ScRfParaType;
                        break;
                    case FileFormat.YAXISTYPE.YIGRF:
                        para.dataType = IgRfDataType;
                        para.paraType = IgRfParaType;
                        break;
                    case FileFormat.YAXISTYPE.YIGSM:
                        para.dataType = IgSmDataType;
                        para.paraType = IgSmParaType;
                        break;
                    case FileFormat.YAXISTYPE.YATR:
                        para.dataType = ATRDataType;
                        para.paraType = ATRParaType;
                        break;
                    case FileFormat.YAXISTYPE.YARB:
                        para.dataType = ABDataType;
                        para.paraType = ABParaType;
                        break;
                    case FileFormat.YAXISTYPE.YEMISN:
                        para.dataType = RAMANDataType;
                        para.paraType = RAMANParaType;
                        break;
                }
                para.DER = 0;
                para.DPF = 1;   //float
                para.DXU = "WN";
                para.FXV = fileFormat.dataInfoList[i].firstX;
                para.LXV = fileFormat.dataInfoList[i].lastX;
                para.MNY = fileFormat.dataInfoList[i].minYValue;
                para.MXY = fileFormat.dataInfoList[i].maxYValue;
                para.NPT = fileFormat.yDataList[i].Length;
                para.DAT  = fileFormat.fileInfo.createTime.ToString("dd/MM/yyyy");
                para.TIM = fileFormat.fileInfo.createTime.ToString("HH:mm:ss");

                dataParas.Add(para);
            }

            return CreateFile(instPara, aqPara, ftPara, dataParas, fileFormat.yDataList, fileFormat.additionalData);
        }

        /// <summary>
        /// 获取模型的参数信息
        /// </summary>
        /// <param name="fileData">模型文件数据</param>
        /// <returns>模型参数</returns>
        public static Ai.Hong.FileFormat.QuantModelFormat.QuantModelParameter GetModelParameter(byte[] fileData)
        {
            QuantModelInfo modelInfo = GetQuantModelInfo(fileData);
            if (modelInfo.fileListSize == int.MaxValue)
                return null;

            Ai.Hong.FileFormat.QuantModelFormat.QuantModelParameter retModelInfo = new QuantModelFormat.QuantModelParameter();
            retModelInfo.allFileCount = modelInfo.spectrumCount;
            retModelInfo.fileFirstX = modelInfo.specFirstX;
            retModelInfo.fileLastX = modelInfo.specLastX;
            retModelInfo.modelFirstX = modelInfo.modelFirstX;
            retModelInfo.modelLastX = modelInfo.modelLastX;
            retModelInfo.modelSpecCols = modelInfo.modelSpecCols;
            retModelInfo.fileXStep = modelInfo.fileStepX;
            retModelInfo.modelStepX = modelInfo.modelStepX;

            return retModelInfo;
        }

        /// <summary>
        /// 获取模型的组分信息
        /// </summary>
        /// <param name="fileData">模型文件数据</param>
        /// <returns>组分信息列表</returns>
        public static List<Ai.Hong.FileFormat.QuantModelFormat.QuantComponentInfo> GetComponentList(byte[] fileData)
        {
            var comps = GetQuantComponentInfo(fileData);
            if (comps == null)
                return null;

            var retDatas = new List<Ai.Hong.FileFormat.QuantModelFormat.QuantComponentInfo>();
            foreach(var item in comps)
            {
                var curInfo = new Ai.Hong.FileFormat.QuantModelFormat.QuantComponentInfo();
                curInfo.name = CommonMethod.CovertByteArrayToString(item.name, Encoding.Default);
                curInfo.unit = CommonMethod.CovertByteArrayToString(item.unit, Encoding.Default);
                int j = curInfo.unit.IndexOf("?%");
                if (j > 0)
                    curInfo.unit = curInfo.unit.Substring(0, j);
                retDatas.Add(curInfo);
            }

            return retDatas;
        }
    }
}
