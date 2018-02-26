using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;

namespace Ai.Hong.FileFormat
{
    unsafe class SPCFileFormat
    {
        #region SPCHeaderDefine
        [StructLayout(LayoutKind.Explicit, CharSet=CharSet.Ansi, Pack=1)]
        struct SPCHeaderaaa    //SPC文件头
        {
            [FieldOffset(0x00)]
            public byte ftflgs;	/* Flag bits defined below */  //文件格式
            [FieldOffset(0x01)]
            public byte fversn;	/* 0x4B=> new LSB 1st, 0x4C=> new MSB 1st, 0x4D=> old format */ //固定：4B
            [FieldOffset(0x02)]
            public byte fexper;	/* Instrument technique code (see below) 光谱种类*/ //固定：0
            [FieldOffset(0x03)]
            public byte fexp; 	/* Fraction scaling exponent integer (80h=>float) Y数值放大系数 2^fexp , 80H表示直接float*/  //固定：80H float
            [FieldOffset(0x04)]
            public UInt32 fnpts;	/* Integer number of points (or TXYXYS directory position) 设置TXYXYS表示开始节的位置*/	//数据点数量
            [FieldOffset(0x08)]
            public double ffirst;	/* Floating X coordinate of first point */		//开始X
            [FieldOffset(0x10)]
            public double flast;	/* Floating X coordinate of last point */		//结束X
            [FieldOffset(0x18)]
            public UInt32 fnsub;	/* Integer number of subfiles (1 if not TMULTI) */	//SubFile: 固定1
            [FieldOffset(0x1C)]
            public byte fxtype;	/* Type of X axis units (see definitions below) */	//X值类型
            [FieldOffset(0x1D)]
            public byte fytype;	/* Type of Y axis units (see definitions below) */	//Y值类型
            [FieldOffset(0x1E)]
            public byte fztype;	/* Type of Z axis units (see definitions below) */	//Z值类型
            [FieldOffset(0x1F)]
            public byte fpost;	/* Posting disposition (see GRAMSDDE.H) */		//? 02
            [FieldOffset(0x20)]
            public UInt32 fdate;	/* Date/Time LSB: min=6b,hour=5b,day=5b,month=4b,year=12b */  	//日期时间：
            [FieldOffset(0x24)]
            public fixed byte fres[9];	/* Resolution description text (null terminated) */	//	//分辨率描述
            [FieldOffset(0x2D)]
            public fixed byte fsource[9];	/* Source instrument description text (null terminated) */	//测量仪器描述
            [FieldOffset(0x36)]
            public UInt16 fpeakpt;	/* Peak point number for interferograms (0=not known) */	//干涉图峰位
            [FieldOffset(0x38)]
            public fixed float fspare[8];	/* Used for Array Basic storage*/			//?  0
            [FieldOffset(0x58)]
            public fixed byte fcmnt[130];	/* Null terminated comment ASCII text string */		//说明文字
            [FieldOffset(0xDA)]
            public fixed byte fcatxt[30];	/* X,Y,Z axis label strings if ftflgs=TALABS */		//X,y,z坐标轴文字(TABLABS)
            [FieldOffset(0xF8)]
            public UInt32 flogoff;	/* File offset to log block or 0 (see above) */			//log block的文件指针
            [FieldOffset(0xFC)]
            public UInt32 fmods;	/* File Modification Flags (see below: 1=A,2=B,4=C,8=D..) */	//文件修改标志
            [FieldOffset(0x100)]
            public byte fprocs;	/* Processing code (see GRAMSDDE.H) 与DDE有关*/				//? 0
            [FieldOffset(0x101)]
            public byte flevel;	/* Calibration level plus one (1 = not calibration data) 与DDE有关*/	//? 0
            [FieldOffset(0x102)]
            public UInt16 fsampin;	/* Sub-method sample injection number (1 = first or only )  与DDE有关*/	//? 0
            [FieldOffset(0x104)]
            public float ffactor;	/* Floating data multiplier concentration factor (IEEE-32) 与DDE有关*/	//放大系数
            [FieldOffset(0x108)]
            public fixed byte fmethod[48];	/* Method/program/data filename w/extensions comma list 与DDE有关*/	//? 0
            [FieldOffset(0x138)]
            public float fzinc;	/* Z subfile increment (0 = use 1st subnext-subfirst) */		//? 0
            [FieldOffset(0x13C)]
            public UInt32 fwplanes;	/* Number of planes for 4D with W dimension (0=normal) */	//? 0
            [FieldOffset(0x140)]
            public float fwinc;	/* W plane increment (only if fwplanes is not 0) */			//? 0
            [FieldOffset(0x144)]
            public byte fwtype;	/* Type of W axis units (see definitions below) */			//? 0
            [FieldOffset(0x145)]
            public fixed byte freserv[187]; /* Reserved (must be set to zero) */				//? 0

            //HeadSize:0x200
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct SPCHeader    //SPC文件头
        {
            public byte ftflgs;	/* Flag bits defined below */  //文件格式
            public byte fversn;	/* 0x4B=> new LSB 1st, 0x4C=> new MSB 1st, 0x4D=> old format */ //固定：4B
            public byte fexper;	/* Instrument technique code (see below) 光谱种类*/ //固定：0
            public byte fexp; 	/* Fraction scaling exponent integer (80h=>float) Y数值放大系数 2^fexp , 80H表示直接float*/  //固定：80H float
            public UInt32 fnpts;	/* Integer number of points (or TXYXYS directory position) 设置TXYXYS表示开始节的位置*/	//数据点数量
            public double ffirst;	/* Floating X coordinate of first point */		//开始X
            public double flast;	/* Floating X coordinate of last point */		//结束X
            public UInt32 fnsub;	/* Integer number of subfiles (1 if not TMULTI) */	//SubFile: 固定1
            public byte fxtype;	/* Type of X axis units (see definitions below) */	//X值类型
            public byte fytype;	/* Type of Y axis units (see definitions below) */	//Y值类型
            public byte fztype;	/* Type of Z axis units (see definitions below) */	//Z值类型
            public byte fpost;	/* Posting disposition (see GRAMSDDE.H) */		//? 02
            public UInt32 fdate;	/* Date/Time LSB: min=6b,hour=5b,day=5b,month=4b,year=12b */  	//日期时间：
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string fres;	/* Resolution description text (null terminated) */	//	//分辨率描述
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string fsource;	/* Source instrument description text (null terminated) */	//测量仪器描述
            public UInt16 fpeakpt;	/* Peak point number for interferograms (0=not known) */	//干涉图峰位
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public float[] fspare;	/* Used for Array Basic storage*/			//?  0
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 130)]
            public string fcmnt;	/* Null terminated comment ASCII text string */		//说明文字
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
            public string fcatxt;	/* X,Y,Z axis label strings if ftflgs=TALABS */		//X,y,z坐标轴文字(TABLABS)
            public UInt32 flogoff;	/* File offset to log block or 0 (see above) */			//log block的文件指针
            public UInt32 fmods;	/* File Modification Flags (see below: 1=A,2=B,4=C,8=D..) */	//文件修改标志
            public byte fprocs;	/* Processing code (see GRAMSDDE.H) 与DDE有关*/				//? 0
            public byte flevel;	/* Calibration level plus one (1 = not calibration data) 与DDE有关*/	//? 0
            public UInt16 fsampin;	/* Sub-method sample injection number (1 = first or only )  与DDE有关*/	//? 0
            public float ffactor;	/* Floating data multiplier concentration factor (IEEE-32) 与DDE有关*/	//放大系数
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string fmethod;	/* Method/program/data filename w/extensions comma list 与DDE有关*/	//? 0
            public float fzinc;	/* Z subfile increment (0 = use 1st subnext-subfirst) */		//? 0
            public UInt32 fwplanes;	/* Number of planes for 4D with W dimension (0=normal) */	//? 0
            public float fwinc;	/* W plane increment (only if fwplanes is not 0) */			//? 0
            public byte fwtype;	/* Type of W axis units (see definitions below) */			//? 0
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 187)]
            public byte[] freserv; /* Reserved (must be set to zero) */				//? 0

            //HeadSize:0x200
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct SPCSubHeader //SPC内容标记
        {
            public byte subflgs;	/* Flags as defined above */
            public byte subexp;	    /* Exponent for sub-file's Y values (80h=>float) Y数值放大系数 2^subexp, 80H表示直接float*/
            public UInt16 subindx;	/* Integer index number of trace subfile (0=first) */
            public float subtime;	/* Floating time for trace (Z axis corrdinate) Z轴(时间轴)的时间值*/
            public float subnext;	/* Floating time for next trace (May be same as beg) 本subFile所代表的采集时间*/
            public float subnois;	/* Floating peak pick noise level if high byte nonzero 噪声值，Galactic软件用*/
            public UInt32 subnpts;	/* Integer number of subfile points for TXYXYS type 本文件的数据点数量*/
            public UInt32 subscan;	/* Integer number of co-added scans or 0 (for collect) 产生subFile数据所扫描的次数*/
            public float subwlevel;	/* Floating W axis value (if fwplanes non-zero) 4D文件时用*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] subresv;	/* Reserved area (must be set to zero) */

            //sub Header Size: 0x20
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct SPCLogHeader		/* SPCHeader.Flogoff表示本Header的位置 log block header format */
        {
            public UInt32 logsizd;	    /* byte size of disk block. SPCLogHeader+LogData的大小*/
            public UInt32 logsizm; 	/* byte size of memory block. logsizd对齐4096后的数据*/
            public UInt32 logtxto;	/* byte offset to text. Log Data的偏移(从本Header开始)*/
            public UInt32 logbins;	/* byte size of binary area (immediately after logstc). 紧接本Head后Binary数据的大小（这里保存我们的IdentifyBlock）*/
            public UInt32 logdsks;	    /* byte size of disk area (immediately after logbins) logbins数据之后，logData数据之前的私有数据大小。*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] logspar;	/* reserved (must be zero) */
        }

        public enum Ftflgs     //Ftflgs 文件组成方式
        {
            TSPREC = 1,	    /* Single precision (16 bit) Y data if set. 16位X或Y数据*/
            TCGRAM = 2,	    /* Enables fexper in older software (CGM if fexper=0) (not used)*/
            TMULTI = 4,	    /* Multiple traces format (set if more than one subfile) 多个subFile*/
            TRANDM = 8,	    /* If TMULTI and TRANDM=1 then arbitrary time (Z) values 随意的Z轴时间顺序*/
            TORDRD = 0x10,	/* If TMULTI and TORDRD=1 then ordered but uneven subtimes 不按顺序的subFile*/
            TALABS = 0x20,	/* Set if should use fcatxt axis labels, not fxtype etc.  使用fcatxt指明的X轴类型，而不是fxtype*/
            TXYXYS = 0x40,	/* If TXVALS and multifile, then each subfile has own X's 每个subFile都有自己的X数据,与TXVALS连用，表示数据不是线条，而是柱状图*/
            TXVALS = 0x80	/* Floating X value array preceeds Y's  (New format only) 在Y数据之前包含Float X 数组*/
        }

        enum SubFlgs
        {
            SUBCHGD = 1,	    /* Subflgs bit if subfile changed 文件有修改*/
            SUBNOPT = 8,	    /* Subflgs bit if peak table file should not be used 没有峰位表*/
            SUBMODF = 128	/* Subflgs bit if subfile modified by arithmetic 由算法修改的文件*/
        }

        #endregion

        #region 32bitDLL
        /// <summary>
        /// 是否授权SPC文件
        /// </summary>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCHasAuthority")]
        private static extern bool SPCHasAuthority32();

        /// <summary>
        /// 是否为SPC文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCIsSPCFile")]
        private static extern bool SPCIsSPCFile32(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取SPC文件头
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetMainHeader")]
        private static extern IntPtr SPCGetMainHeader32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取SPC子数据结构
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="subIndex">子数据序号</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetSubHeader")]
        private static extern IntPtr SPCGetSubHeader32(byte[] fileData, int fileSize, int subIndex, ref int outDataSize);

        /// <summary>
        /// 读取Y轴数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="subIndex">子数据序号</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetYDatas")]
        private static extern IntPtr SPCGetYDatas32(byte[] fileData, int fileSize, int subIndex, ref int outDataSize);

        /// <summary>
        /// 读取X轴数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="subIndex">子数据序号</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetXDatas")]
        private static extern IntPtr SPCGetXDatas32(byte[] fileData, int fileSize, int subIndex, ref int outDataSize);

        /// <summary>
        /// 获取SPC的文本Log数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>Log数据</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetLogText")]
        private static extern IntPtr SPCGetLogText32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 获取SPC的Binary Log数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>Log数据</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetLogBinary")]
        private static extern IntPtr SPCGetLogBinary32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 创建SPC格式数据，包含一个或多个subFile，X轴间距不等，由所有Y轴共享
        /// </summary>
        /// <param name="mainHeader">SPC文件头</param>
        /// <param name="subTypes">子数据类型（一个或者多个）</param>
        /// <param name="dataPoints">子数据的数据点数列表，长度等于mainHeader->fnsub，可以为NULL</param>
        /// <param name="xDatas">X轴数据，可以为NULL</param>
        /// <param name="yDatas">子数据（一个或者多个Y轴数据，每条数据长度=文件头的数据点数）</param>
        /// <param name="logBinary">附加数据</param>
        /// <param name="logBinarySize">附加数据长度</param>
        /// <param name="logText">光谱采集参数</param>
        /// <param name="logTextSize">采集参数长度</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPC格式数据</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCCreateFile")]
        private static extern IntPtr SPCCreateFile32(ref SPCHeader mainHeader, byte[] subTypes, int[] dataPoints, float[] xDatas, float[] yDatas, byte[] logText, int logTextSize, byte[] logBinary, int logBinarySize, ref int outDataSize);

        #endregion

        #region 64bitDLL
        /// <summary>
        /// 是否授权SPC文件
        /// </summary>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCHasAuthority")]
        private static extern bool SPCHasAuthority64();

        /// <summary>
        /// 是否为SPC文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCIsSPCFile")]
        private static extern bool SPCIsSPCFile64(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取SPC文件头
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetMainHeader")]
        private static extern IntPtr SPCGetMainHeader64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取SPC子数据结构
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="subIndex">子数据序号</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetSubHeader")]
        private static extern IntPtr SPCGetSubHeader64(byte[] fileData, int fileSize, int subIndex, ref int outDataSize);

        /// <summary>
        /// 读取Y轴数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="subIndex">子数据序号</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetYDatas")]
        private static extern IntPtr SPCGetYDatas64(byte[] fileData, int fileSize, int subIndex, ref int outDataSize);

        /// <summary>
        /// 读取X轴数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="subIndex">子数据序号</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPCHeader结构</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetXDatas")]
        private static extern IntPtr SPCGetXDatas64(byte[] fileData, int fileSize, int subIndex, ref int outDataSize);

        /// <summary>
        /// 获取SPC的文本Log数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>Log数据</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetLogText")]
        private static extern IntPtr SPCGetLogText64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 获取SPC的Binary Log数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>Log数据</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCGetLogBinary")]
        private static extern IntPtr SPCGetLogBinary64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 创建SPC格式数据，包含一个或多个subFile，X轴间距不等，由所有Y轴共享
        /// </summary>
        /// <param name="mainHeader">SPC文件头</param>
        /// <param name="subTypes">子数据类型（一个或者多个）</param>
        /// <param name="dataPoints">每个光谱的数据点数量</param>
        /// <param name="xDatas">X轴数据，可以为NULL</param>
        /// <param name="yDatas">子数据（一个或者多个Y轴数据，每条数据长度=文件头的数据点数）</param>
        /// <param name="logBinary">附加数据</param>
        /// <param name="logBinarySize">附加数据长度</param>
        /// <param name="logText">光谱采集参数</param>
        /// <param name="logTextSize">采集参数长度</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns>SPC格式数据</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPCCreateFile")]
        private static extern IntPtr SPCCreateFile64(ref SPCHeader mainHeader, byte[] subTypes, int[] dataPoints, float[] xDatas, float[] yDatas, byte[] logText, int logTextSize, byte[] logBinary, int logBinarySize, ref int outDataSize);

        #endregion

        #region 32/64Bit自适应

        /// <summary>
        /// 是否授权为SPC文件
        /// </summary>
        public static bool HasAuthority()
        {
            if (CommonMethod.Is64BitVersion())
                return SPCHasAuthority64();
            else
                return SPCHasAuthority32();
        }

        /// <summary>
        /// 是否为SPC文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        public static bool IsSPCFile(byte[] fileData, int fileSize)
        {
            if (CommonMethod.Is64BitVersion())
                return SPCIsSPCFile64(fileData, fileSize);
            else
                return SPCIsSPCFile32(fileData, fileSize);
        }

        /// <summary>
        /// 读取SPC文件头
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>SPCHeader结构</returns>
        private static SPCHeader GetMainHeader(byte[] fileData, int fileSize)
        {
            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;
            if (CommonMethod.Is64BitVersion())
                retptr = SPCGetMainHeader64(fileData, fileSize, ref datasize);
            else
                retptr = SPCGetMainHeader32(fileData, fileSize, ref datasize);

            bool retOK;
            SPCHeader retheader = CommonMethod.CopyStructureFromIntptrAndFree<SPCHeader>(ref retptr, out retOK);
            if (!retOK)
                retheader.fnpts = int.MaxValue;     //表示错误数据

            return retheader;
        }

        /// <summary>
        /// 读取子数据的结构
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="subIndex">需要获取子结构的序号</param>
        /// <returns>子数据的结构</returns>
        private static SPCSubHeader GetSubHeader(byte[] fileData, int fileSize, int subIndex)
        {
            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;
            if (CommonMethod.Is64BitVersion())
                retptr = SPCGetSubHeader64(fileData, fileSize, subIndex, ref datasize);
            else
                retptr = SPCGetSubHeader32(fileData, fileSize, subIndex, ref datasize);

            bool retOK;
            SPCSubHeader retheader = CommonMethod.CopyStructureFromIntptrAndFree<SPCSubHeader>(ref retptr, out retOK);
            if (!retOK)
                retheader.subnpts = int.MaxValue;     //表示错误数据

            return retheader;
        }

        /// <summary>
        /// 读取Y轴数据
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileSize"></param>
        /// <param name="subIndex"></param>
        /// <returns></returns>
        private static double[] GetYData(byte[] fileData, int fileSize, int subIndex)
        {
            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;
            if (CommonMethod.Is64BitVersion())
                retptr = SPCGetYDatas64(fileData, fileSize, subIndex, ref datasize);
            else
                retptr = SPCGetYDatas32(fileData, fileSize, subIndex, ref datasize);

            float[] tempdatas = CommonMethod.CopyDataArrayFromIntptrAndFree<float>(ref retptr, datasize);
            if (tempdatas == null)
                return null;

            double[] retdatas = new double[tempdatas.Length];
            for (int i = 0; i < retdatas.Length; i++)
                retdatas[i] = tempdatas[i];

            return retdatas;
        }

        /// <summary>
        /// 读取X轴数据
        /// </summary>
        private static double[] GetXData(byte[] fileData, int fileSize, int subIndex)
        {
            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;
            if (CommonMethod.Is64BitVersion())
                retptr = SPCGetXDatas64(fileData, fileSize, subIndex, ref datasize);
            else
                retptr = SPCGetXDatas32(fileData, fileSize, subIndex, ref datasize);

            float[] tempdatas = CommonMethod.CopyDataArrayFromIntptrAndFree<float>(ref retptr, datasize);
            if (tempdatas == null)
                return null;

            double[] retdatas = new double[tempdatas.Length];
            for (int i = 0; i < retdatas.Length; i++)
                retdatas[i] = tempdatas[i];

            return retdatas;
        }

        /// <summary>
        /// 读取文本的LOG数据
        /// </summary>
        public static string GetLogText(byte[] fileData, int fileSize)
        {
            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;
            if (CommonMethod.Is64BitVersion())
                retptr = SPCGetLogText64(fileData, fileSize, ref datasize);
            else
                retptr = SPCGetLogText32(fileData, fileSize, ref datasize);

            byte[] tempdatas = CommonMethod.CopyByteFromIntptrAndFree(ref retptr, datasize);
            if (tempdatas == null)
                return null;

            return Encoding.ASCII.GetString(tempdatas);     //这里假定都是ASCII格式字符串
        }

        /// <summary>
        /// 读取二进制的LOG数据
        /// </summary>
        public static byte[] GetLogBinary(byte[] fileData, int fileSize)
        {
            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;
            if (CommonMethod.Is64BitVersion())
                retptr = SPCGetLogBinary64(fileData, fileSize, ref datasize);
            else
                retptr = SPCGetLogBinary32(fileData, fileSize, ref datasize);

            return CommonMethod.CopyByteFromIntptrAndFree(ref retptr, datasize);
        }

        /// <summary>
        /// 创建SPC文件
        /// </summary>
        /// <param name="mainHeader">文件结构</param>
        /// <param name="subTypes">子数据类型</param>
        /// <param name="dataPoints">各数据的数量</param>
        /// <param name="xDatas">X轴数据，可以为NULL</param>
        /// <param name="yDatas">Y轴数据，一条或者多条，列=mainHeader->fnpts, 行=subTypes.Length</param>
        /// <param name="logText">光谱采集参数</param>
        /// <param name="logBinary">私有数据</param>
        /// <returns>SPC内容</returns>
        private static byte[] CreateCommonXFile(SPCHeader mainHeader, byte[] subTypes, int[] dataPoints, float[] xDatas, float[] yDatas, byte[] logText, byte[] logBinary)
        {
            //xDatas=NULL表示等间距X轴
            if (xDatas == null && mainHeader.fnpts <= 0 || yDatas == null || logText == null || logText.Length == 0)
                return null;

            if (xDatas == null)     //没有X轴数据，就不能有dataPoints
            {
                dataPoints = null;
                if (yDatas.Length != mainHeader.fnpts * mainHeader.fnsub)   //Y轴数据必须与文件数量和每个文件的数据点数相符
                    return null;
            }
            else
            {
                if (dataPoints == null)  //如果没有dataPoints，表示只有一个xDatas，由所有Y轴共享
                {
                    if (xDatas.Length != mainHeader.fnpts || yDatas.Length != mainHeader.fnpts * mainHeader.fnsub)
                        return null;
                }
                else    //每个Y都对应不同的X
                {
                    mainHeader.fnpts = 0;
                    mainHeader.fnsub = (uint)dataPoints.Length;
                    int count = dataPoints.Sum();
                    if (xDatas.Length != count || yDatas.Length != count)
                        return null;
                }
            }

            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;
            if (CommonMethod.Is64BitVersion())
                retptr = SPCCreateFile64(ref mainHeader, subTypes, dataPoints, xDatas, yDatas, logText, logText == null ? 0 : logText.Length, logBinary, logBinary == null ? 0 : logBinary.Length, ref datasize);
            else
                retptr = SPCCreateFile32(ref mainHeader, subTypes, dataPoints, xDatas, yDatas, logText, logText == null ? 0 : logText.Length, logBinary, logBinary == null ? 0 : logBinary.Length, ref datasize);

            return CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
        }

        #endregion

        public static FileFormat.AcquisitionInfo FromSPCLogData(string logData)
        {
            if (string.IsNullOrWhiteSpace(logData))
                return null;

            FileFormat.AcquisitionInfo retinfo = new FileFormat.AcquisitionInfo();
            logData = logData.Replace("\n", "");
            logData = logData.Replace("\0", "");
            string[] logs = logData.Split('\r');

            var props = typeof(FileFormat.AcquisitionInfo).GetProperties();
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
                    else if (item.PropertyType == typeof(FileFormat.enumIRMODE))
                    {
                        if (int.TryParse(valuestr, out tempint))
                            item.SetValue(retinfo, tempint, null);
                        item.SetValue(retinfo, (FileFormat.enumIRMODE)tempint, null);
                    }
                }
            }

            return retinfo;
        }

        /// <summary>
        /// 生成Acquisition Log Data
        /// </summary>
        /// <returns></returns>
        public static string ToSPCLogData(FileFormat.AcquisitionInfo acquistData)
        {
            var props = typeof(FileFormat.AcquisitionInfo).GetProperties();
            string retdata = null;
            int tempint;
            double tempdouble;
            foreach (var item in props)
            {
                string curstr = null;
                if (item.PropertyType == typeof(int))
                {
                    tempint = (int)item.GetValue(acquistData, null);
                    if (tempint != 0)
                        curstr = tempint.ToString();
                }
                else if (item.PropertyType == typeof(double))
                {
                    tempdouble = (double)item.GetValue(acquistData, null);
                    if (tempdouble != 0)
                        curstr = tempdouble.ToString();
                }
                else if (item.PropertyType == typeof(string))
                {
                    curstr = (string)item.GetValue(acquistData, null);
                }
                else if (item.PropertyType == typeof(FileFormat.enumIRMODE))
                {
                    curstr = ((int)((FileFormat.enumIRMODE)item.GetValue(acquistData, null))).ToString();
                }

                if (!string.IsNullOrWhiteSpace(curstr))
                {
                    curstr = item.Name + " = " + curstr;
                    retdata += curstr + "\r\n";
                }

            }

            return retdata;
        }


        public static bool ReadFile(byte[] fileData, int fileSize, FileFormat fileFormat)
        {
            fileFormat.dataInfoList = null;
            fileFormat.acquisitionInfo = null;
            fileFormat.xDataList = null;
            fileFormat.fileInfo = null;
            fileFormat.yDataList = null;

            SPCHeader mainheader = GetMainHeader(fileData, fileSize);
            if (mainheader.fnpts == int.MaxValue)  //读取文件头错误
                return false;

            //填充光谱文件信息
            fileFormat.fileInfo = new FileFormat.FileInfo();
            fileFormat.fileInfo.createTime = CommonMethod.DWordToDateTime(mainheader.fdate);
            fileFormat.fileInfo.dataCount = (int)mainheader.fnpts;
            fileFormat.fileInfo.fileMemo = mainheader.fcmnt; //CommonMethod.ConvertFixedByteToString(mainheader.fcmnt, 130);
            fileFormat.fileInfo.instDescription = mainheader.fsource; //CommonMethod.ConvertFixedByteToString(mainheader.fsource, 9);
            fileFormat.fileInfo.modifyFlag = mainheader.fmods;
            double tempres = 0; //resolution
            string resstr = mainheader.fres;    // CommonMethod.ConvertFixedByteToString(mainheader.fres, 9);
            fileFormat.fileInfo.resolution = double.TryParse(resstr, out tempres) == true ? tempres : 0;

            fileFormat.fileInfo.specType = mainheader.fexper == 0 ? FileFormat.SPECTYPE.SPCNIR : (FileFormat.SPECTYPE)mainheader.fexper;
            fileFormat.fileInfo.xType = (FileFormat.XAXISTYPE)mainheader.fxtype;
            fileFormat.fileInfo.zType = (FileFormat.ZAXISTYPE)mainheader.fztype;
            fileFormat.fileInfo.fzinc = mainheader.fzinc;
            fileFormat.fileInfo.fspare = mainheader.fspare;  //CommonMethod.CopyDataArrayFromFixedPtr<float>(mainheader.fspare, 8);

            //读取X轴数据
            if ((mainheader.ftflgs & (byte)Ftflgs.TXYXYS) == 0)     //统一的X轴
            {
                fileFormat.xDataList = new List<double[]>();
                double[] tempx = GetXData(fileData, fileSize, 0);
                fileFormat.xDataList.Add(tempx);
            }
            else    //多个X轴
            {
                fileFormat.xDataList = new List<double[]>();
                for (int i = 0; i < mainheader.fnsub; i++)
                {
                    double[] tempx = GetXData(fileData, fileSize, i);
                    fileFormat.xDataList.Add(tempx);
                }
            }

            //获取Y数据以及格式信息
            fileFormat.dataInfoList = new List<FileFormat.DataInfo>();
            fileFormat.yDataList = new List<double[]>();
            for (int i = 0; i < mainheader.fnsub; i++)
            {
                SPCSubHeader subheader = GetSubHeader(fileData, fileSize, i);
                if (subheader.subnpts == int.MaxValue)      //读取结构错误
                    return false;

                //读取Y轴数据
                double[] ydata = GetYData(fileData, fileSize, i);
                if (ydata == null || ydata.Length == 0)
                    return false;

                //Y轴数据格式
                FileFormat.DataInfo info = new FileFormat.DataInfo();
                if (subheader.subscan == 0)      //用这个属性来记录子数据的类型（吸收谱，干涉谱等等）
                    info.dataType = mainheader.fytype == 0 ? FileFormat.YAXISTYPE.YABSRB : (FileFormat.YAXISTYPE)mainheader.fytype;
                else
                    info.dataType = (FileFormat.YAXISTYPE)subheader.subscan;
                info.firstX = mainheader.ffirst;
                info.lastX = mainheader.flast;
                info.maxYValue = ydata.Max();
                info.minYValue = ydata.Min();

                fileFormat.dataInfoList.Add(info);
                fileFormat.yDataList.Add(ydata);
            }

            //读取光谱参数
            string parastr = GetLogText(fileData, fileSize);
            if (parastr != null)
            {
                fileFormat.acquisitionInfo = FromSPCLogData(parastr);
            }
            if (fileFormat.acquisitionInfo == null)
                fileFormat.acquisitionInfo = new FileFormat.AcquisitionInfo();

            return true;
        }

        public static byte[] SaveFile(FileFormat fileFormat)
        {
            if (fileFormat.xDatas == null || fileFormat.xDatas.Length == 0)
                return null;
            if (fileFormat.yDataList == null || fileFormat.yDataList.Count == 0)
                return null;
            if (fileFormat.dataInfoList == null || fileFormat.dataInfoList.Count != fileFormat.yDataList.Count)
                return null;
            if (fileFormat.xDataList.Count == 1)     //统一的X轴
            {
                //检查是否所有Y轴数据点数量与X数量相同
                foreach (var item in fileFormat.yDataList)
                {
                    if (item.Length != fileFormat.xDatas.Length)
                        return null;
                }
            }
            else
            {
                //检查是否所有Y轴数据点数量与对应的X轴数据数量相同
                if (fileFormat.xDataList.Count != fileFormat.yDataList.Count)
                    return null;
                for (int i = 0; i < fileFormat.xDataList.Count; i++)
                {
                    if (fileFormat.xDataList[i].Length != fileFormat.yDataList[i].Length)
                        return null;
                }
            }

            //子文件类型
            byte[] subTypes = (from p in fileFormat.dataInfoList select (byte)p.dataType).ToArray();

            //文件主结构
            SPCHeader header = new SPCHeader();
            header.ftflgs = 0;
            header.fexper = (byte)fileFormat.fileInfo.specType;
            header.fexp = 0x80;     //float
            header.fnpts = (uint)fileFormat.xDatas.Length;
            header.ffirst = fileFormat.dataInfo.firstX;
            header.flast = fileFormat.dataInfo.lastX;
            header.fnsub = (uint)fileFormat.dataInfoList.Count;
            header.fxtype = (byte)fileFormat.fileInfo.xType;
            header.fytype = (byte)fileFormat.dataInfo.dataType;
            header.fdate = CommonMethod.DataTimeToDWord(DateTime.Now);
            header.fres = fileFormat.fileInfo.resolution.ToString();    // CommonMethod.ConvertStringToFixedByte(fileFormat.fileInfo.resolution.ToString(), header.fres, 9);
            header.fsource = fileFormat.fileInfo.instDescription;    //CommonMethod.ConvertStringToFixedByte(fileFormat.fileInfo.instDescription, header.fsource, 9);
            header.fcmnt = fileFormat.fileInfo.fileMemo;  //CommonMethod.ConvertStringToFixedByte(fileFormat.fileInfo.fileMemo, header.fcmnt, 130);
            header.fmods = fileFormat.fileInfo.modifyFlag;
            //CommonMethod.CopyDataArrayToFixedPtr<float>(header.fspare, 8, fileFormat.fileInfo.fspare);
            header.fspare = fileFormat.fileInfo.fspare;

            //设置Z轴格式
            if (fileFormat.dataInfoList.Count < 2)
            {
                header.fztype = 0;
                header.fzinc = 0;
            }
            else
            {
                header.fztype = fileFormat.fileInfo.fzinc == 0 ? (byte)FileFormat.ZAXISTYPE.XMSEC : (byte)fileFormat.fileInfo.zType;  //如果没有设置fzinc,缺省设置为单位=秒
                header.fzinc = fileFormat.fileInfo.fzinc == 0 ? 0.5f : fileFormat.fileInfo.fzinc;    //如果没有设置fzinc,缺省设置为0.5s
            }
            string logstr = ToSPCLogData(fileFormat.acquisitionInfo);
            byte[] logText = Encoding.ASCII.GetBytes(logstr);

            int[] dataPoints = null;    //xData的数据点数

            //XDatas转换到float
            float[] floatxdatas = null;

            //判断是否为均匀的X
            double stepx = (fileFormat.dataInfo.lastX - fileFormat.dataInfo.firstX) / (fileFormat.fileInfo.dataCount - 1);
            stepx += stepx / 1000;  //稍微放大一点
            stepx = Math.Abs(stepx);

            bool isevenlyx = true;
            if (fileFormat.xDataList.Count > 1)     //超过1个X数据，一定是多X文件
            {
                isevenlyx = false;
                dataPoints = (from p in fileFormat.xDataList select p.Length).ToArray();    //记录每个X轴的数据点数
            }
            else
            {
                for (int i = 1; i < fileFormat.xDatas.Length; i++)
                {
                    if (Math.Abs(fileFormat.xDatas[i] - fileFormat.xDatas[i - 1]) > stepx)
                    {
                        isevenlyx = false;
                        break;
                    }
                }
            }

            //将所有X轴转换为float，合并在一个Array
            int index = 0;
            if (!isevenlyx && fileFormat.xDatas != null)
            {
                int xcount = (from p in fileFormat.xDataList select p.Length).Sum();     //所有X轴的数据数量
                floatxdatas = new float[xcount];
                foreach (var xdatas in fileFormat.xDataList)
                {
                    for (int i = 0; i < xdatas.Length; i++)
                        floatxdatas[index++] = (float)xdatas[i];
                }
            }

            //所有YDatas转换到float，合并在一个Array
            int ycount = (from p in fileFormat.yDataList select p.Length).Sum();     //所有X轴的数据数量
            float[] floatydatas = new float[ycount];
            index = 0;
            foreach (var ydatas in fileFormat.yDataList)
            {
                for (int i = 0; i < ydatas.Length; i++)
                    floatydatas[index++] = (float)ydatas[i];
            }

            return CreateCommonXFile(header, subTypes, dataPoints, floatxdatas, floatydatas, logText, fileFormat.additionalData);
        }

    }
}
