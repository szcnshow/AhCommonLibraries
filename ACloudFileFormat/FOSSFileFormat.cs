using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.FileFormat
{
    unsafe class FossFileFormat
    {
        public static string ErrorString = null;

        #region FOSSHeaderDefine
	    
        /// <summary>
        /// 组分名称长度
        /// </summary>
        private const int CompNameLength = 0x10;

        /// <summary>
        /// 仪器和序列号分隔符
        /// </summary>
        private const char instSerialSeprator = '\\';

        /// <summary>
        /// 文件描述
        /// </summary>
        private const int MAIN_TITLE = 0;

        /// <summary>
        /// 仪器描述
        /// </summary>
	    private const int MAIN_INSTRUMENT= 1;
        /// <summary>
        /// 仪器序列号
        /// </summary>
	    private const int MAIN_SERIALNO =2;
        /// <summary>
        /// 组分名称
        /// </summary>
        private const int MAIN_COMPONENT = 3;
        /// <summary>
        /// 样品编号长度
        /// </summary>
        private const int SAMPLECODELEN = 13;
        /// <summary>
        /// 样品信息长度
        /// </summary>
        private const int SAMPLEIDLEN = 0x32;
        /// <summary>
	    /// FOSS CAL文件头
	    /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct MainHeader
        {
            public UInt16 fileMark;			//0x0002
            public UInt32 fileCount;		//光谱文件数量 0x0E
            public UInt16 specCols;			//光谱数据点数
            public UInt16 componentCount;	//组分数量
            public UInt16 word4;			//更新日期
            public UInt32 fileTime;			//更新时间
            public UInt16 word3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x47)]
            public byte[] tile;		//文件描述
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x09)]
            public byte[] masterNo;		//master number
            UInt16 word1;				//0x01
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1c)]
            byte[] byte2;
            public UInt16 word2;				//0x1E
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x15)]
            public byte[] instrument;	//仪器描述
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x09)]
            public byte[] serialNo;		//仪器序列号
            public UInt16 rangeCount;		//光谱区间数量
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x07)]
            public UInt16[] rangeCols;		//光谱数据点数数量
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x07)]
            public UInt32[] dword1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x07)]
            public float[] rangeFirstX;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x07)]
            public float[] rangeStepX;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x07)]
            public float[] rangeLastX;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x60)]
            byte[] byte3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x200)]
            public byte[] components;	//组分名称，1行一个名称
        };

        /// <summary>
        /// 光谱头结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct FileHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
            public byte[] sampleNo;		//样品编号
            public UInt16 position;			//位置，0x0100
            byte byte2;				//0x00
            public UInt16 word1;			//检测日期
            public UInt16 productCode;		//0x000A
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public byte[] clientCode;		//
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x96)]
            public byte[] sampleID;	//sampleID
            byte byte3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] username;
            public UInt32 fileTime;			//检测时间
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x26)]
            byte[] byte4;
        };

        #endregion

        #region 32bitDLL
        /// <summary>
        /// 是否授权Foss文件
        /// </summary>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossHasAuthority")]
        private static extern bool FossHasAuthority32();

        /// <summary>
        /// 是否为SPA文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossIsFossFile")]
        private static extern bool FossIsFossFile32(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取文件头信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>XHeader结构</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetMainHeader")]
        private static extern IntPtr FossGetMainHeader32(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取光谱头信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="index">光谱文件索引</param>
        /// <returns>Title（字符串）</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetFileHeader")]
        private static extern IntPtr FossGetFileHeader32(byte[] fileData, int fileSize, int index);

        /// <summary>
        /// 读取光谱数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="index">光谱文件索引</param>
        /// <param name="outDataSize">返回数据大小(double)</param>
        /// <returns>Y轴数据（double）</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetSpectrumData")]
        private static extern IntPtr FossGetSpectrumData32(byte[] fileData, int fileSize, int index, ref int outDataSize);

        /// <summary>
        /// 读取浓度数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="index">光谱文件索引</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>仪器序列号（string）</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetComponentData")]
        private static extern IntPtr FossGetComponentData32(byte[] fileData, int fileSize, int index, ref int outDataSize);

        /// <summary>
        /// 创建一条光谱数据
        /// </summary>
        /// <param name="sampleCode">样品编号(字节,长度=SAMPLECODELEN)</param>
        /// <param name="index">光谱序号</param>
        /// <param name="fileTime">创建时间</param>
        /// <param name="yDatas">光谱数据</param>
        /// <param name="yDataCount">光谱数据数量</param>
        /// <param name="compDatas">浓度数据</param>
        /// <param name="compDataCount">浓度数据数量</param>
        /// <param name="sampleID">样品信息</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossCreateFileData")]
        private static extern IntPtr FossCreateFileData32(byte[] sampleCode, UInt16 index, UInt32 fileTime, double[] yDatas, UInt32 yDataCount, double[] compDatas, UInt32 compDataCount, byte[] sampleID, ref int outDataSize);

        /// <summary>
        /// 创建光谱文件索引数据
        /// </summary>
        /// <param name="sampleNo">样品编号列表</param>
        /// <param name="sampleCount">样品编号列表数量</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossCreateIndexData")]
        private static extern IntPtr FossCreateIndexData32(string[] sampleNo, UInt32 sampleCount, ref int outDataSize);

        /// <summary>
        /// 创建文件头
        /// </summary>
        /// <param name="fileCount">光谱文件数量</param>
        /// <param name="specCols">光谱数据点数量</param>
        /// <param name="fileTime">光谱文件日期</param>
        /// <param name="firstX"></param>
        /// <param name="lastX"></param>
        /// <param name="compCount">组分数量</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossCreateMainHeader")]
        private static extern IntPtr FossCreateMainHeader32(UInt32 fileCount, UInt32 specCols, UInt32 fileTime, double firstX, double lastX, int compCount, ref int outDataSize);

        /// <summary>
        /// 在主文件头中添加附加信息
        /// </summary>
        /// <param name="headerData">主文件头数据</param>
        /// <param name="infoType">信息类型</param>
        /// <param name="infoData">信息内容</param>
        /// <param name="infoSize">信息长度</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossAddMainHeaderInfo")]
        private static extern bool FossAddMainHeaderInfo32(byte[] headerData, int infoType, byte[] infoData, int infoSize);

        /// <summary>
        /// 获取保存后文件大小
        /// </summary>
        /// <param name="fileCount">光谱文件数量</param>
        /// <param name="specCols">光谱数据点数量</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetSaveSize")]
        private static extern int FossGetSaveSize32(int fileCount, int specCols);

        #endregion

        #region 64bitDLL
        /// <summary>
        /// 是否授权Foss文件
        /// </summary>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossHasAuthority")]
        private static extern bool FossHasAuthority64();

        /// <summary>
        /// 是否为SPA文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossIsFossFile")]
        private static extern bool FossIsFossFile64(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取文件头信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>XHeader结构</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetMainHeader")]
        private static extern IntPtr FossGetMainHeader64(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取光谱头信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="index">光谱文件索引</param>
        /// <returns>Title（字符串）</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetFileHeader")]
        private static extern IntPtr FossGetFileHeader64(byte[] fileData, int fileSize, int index);

        /// <summary>
        /// 读取光谱数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="index">光谱文件索引</param>
        /// <param name="outDataSize">返回数据大小(double)</param>
        /// <returns>Y轴数据（double）</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetSpectrumData")]
        private static extern IntPtr FossGetSpectrumData64(byte[] fileData, int fileSize, int index, ref int outDataSize);

        /// <summary>
        /// 读取浓度数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="index">光谱文件索引</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>仪器序列号（string）</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetComponentData")]
        private static extern IntPtr FossGetComponentData64(byte[] fileData, int fileSize, int index, ref int outDataSize);

        /// <summary>
        /// 创建一条光谱数据
        /// </summary>
        /// <param name="sampleCode">样品编号(字节,长度=SAMPLECODELEN)</param>
        /// <param name="index">光谱序号</param>
        /// <param name="fileTime">创建时间</param>
        /// <param name="yDatas">光谱数据</param>
        /// <param name="yDataCount">光谱数据数量</param>
        /// <param name="compDatas">浓度数据</param>
        /// <param name="compDataCount">浓度数据数量</param>
        /// <param name="sampleID">样品信息</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossCreateFileData")]
        private static extern IntPtr FossCreateFileData64(byte[] sampleCode, UInt16 index, UInt32 fileTime, double[] yDatas, UInt32 yDataCount, double[] compDatas, UInt32 compDataCount, byte[] sampleID, ref int outDataSize);

        /// <summary>
        /// 创建光谱文件索引数据
        /// </summary>
        /// <param name="sampleNo">样品编号列表</param>
        /// <param name="sampleCount">样品编号列表数量</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossCreateIndexData")]
        private static extern IntPtr FossCreateIndexData64(string[] sampleNo, UInt32 sampleCount, ref int outDataSize);

        /// <summary>
        /// 创建文件头
        /// </summary>
        /// <param name="fileCount">光谱文件数量</param>
        /// <param name="specCols">光谱数据点数量</param>
        /// <param name="fileTime">光谱文件时间</param>
        /// <param name="firstX"></param>
        /// <param name="lastX"></param>
        /// <param name="compCount">组分数量</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossCreateMainHeader")]
        private static extern IntPtr FossCreateMainHeader64(UInt32 fileCount, UInt32 specCols, UInt32 fileTime, double firstX, double lastX, int compCount, ref int outDataSize);

        /// <summary>
        /// 在主文件头中添加附加信息
        /// </summary>
        /// <param name="headerData">主文件头数据</param>
        /// <param name="infoType">信息类型</param>
        /// <param name="infoData">信息内容</param>
        /// <param name="infoSize">信息长度</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossAddMainHeaderInfo")]
        private static extern bool FossAddMainHeaderInfo64(byte[] headerData, int infoType, byte[] infoData, int infoSize);

        /// <summary>
        /// 获取保存后文件大小
        /// </summary>
        /// <param name="fileCount">光谱文件数量</param>
        /// <param name="specCols">光谱数据点数量</param>
        /// <returns></returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FossGetSaveSize")]
        private static extern int FossGetSaveSize64(int fileCount, int specCols);

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
                    return FossHasAuthority64();
                else
                    return FossHasAuthority32();
            }
            catch(Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 是否为SPC文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        public static bool IsFossFile(byte[] fileData)
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return FossIsFossFile64(fileData, fileData.Length);
                else
                    return FossIsFossFile32(fileData, fileData.Length);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取SPC文件头
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <returns>SPCHeader结构</returns>
        private static MainHeader GetMainHeader(byte[] fileData)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = FossGetMainHeader64(fileData, fileData.Length);
                else
                    retptr = FossGetMainHeader32(fileData, fileData.Length);

                bool retOK;
                MainHeader retheader = CommonMethod.CopyStructureFromIntptrAndFree<MainHeader>(ref retptr, out retOK);
                if (!retOK)
                {
                    retheader.fileMark = UInt16.MaxValue;     //表示错误数据
                    ErrorString = CommonMethod.ErrorString;
                }
                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                MainHeader retheader = new MainHeader();
                retheader.fileMark = UInt16.MaxValue;    //表示错误数据                
                return retheader;
            }
        }

        /// <summary>
        /// 获取光谱头信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="index">光谱序号</param>
        private static FileHeader GetFileHeader(byte[] fileData, int index)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = FossGetFileHeader64(fileData, fileData.Length, index);
                else
                    retptr = FossGetFileHeader32(fileData, fileData.Length, index);

                bool retOK;
                FileHeader retheader = CommonMethod.CopyStructureFromIntptrAndFree<FileHeader>(ref retptr, out retOK);
                if (!retOK)
                {
                    retheader.position = UInt16.MaxValue;     //表示错误数据
                    ErrorString = CommonMethod.ErrorString;
                }
                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                FileHeader retheader = new FileHeader();
                retheader.position = UInt16.MaxValue;    //表示错误数据                
                return retheader;
            }
        }

        /// <summary>
        /// 获取光谱Y轴数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="index">光谱序号</param>
        /// <returns>返回光谱Y轴数据</returns>
        private static double[] GetSpectrumData(byte[] fileData, int index)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = FossGetSpectrumData64(fileData, fileData.Length, index, ref datasize);
                else
                    retptr = FossGetSpectrumData32(fileData, fileData.Length, index, ref datasize);

                double[] retDatas = CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, datasize);
                if (retDatas == null)
                {
                    ErrorString = CommonMethod.ErrorString;
                    return null;
                }

                return retDatas;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 读取浓度数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="index">光谱序号</param>
        /// <returns>返回光谱浓度数据</returns>
        public static double[] GetComponentData(byte[] fileData, int index)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = FossGetComponentData64(fileData, fileData.Length, index, ref datasize);
                else
                    retptr = FossGetComponentData32(fileData, fileData.Length, index, ref datasize);

                double[] retDatas = CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, datasize);
                if (retDatas == null)
                {
                    ErrorString = CommonMethod.ErrorString;
                    return null;
                }

                return retDatas;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 将str转换为固定大小的数组，数组大小为maxLen
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="arrayLen">固定数组大小</param>
        /// <returns></returns>
        private static byte[] StringToBytesArray(string str, int arrayLen)
        {
            byte[] retData = new byte[arrayLen];
            var bytes = Encoding.Default.GetBytes(str);
            Array.Copy(bytes, retData, bytes.Length < arrayLen - 1 ? bytes.Length : arrayLen - 1);

            return retData;
        }

        /// <summary>
        /// 将DateTime转换为1970-1-1开始的秒数
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private static UInt32 CreateFileDateTime(DateTime datetime)
        {
            var diff = datetime.Subtract(new DateTime(1970, 1, 1));
            return (UInt32)diff.TotalSeconds + 5*3600;  //考虑时差
        }

        /// <summary>
        /// 将DateTime转换为Date和Time两部分
        /// </summary>
        /// <param name="time">32位的日期和时间</param>
        /// <returns></returns>
        private static DateTime CreateFileDateTime(UInt32 time)
        {
            DateTime retdate = new DateTime(1970, 1, 1);
            retdate = retdate.AddSeconds(time - 5*3600);    //考虑时差
            
            return retdate;
        }

        /// <summary>
        /// 向主结构中添加信息
        /// </summary>
        /// <param name="headerData">文件头数据</param>
        /// <param name="infoType">信息类型</param>
        /// <param name="infoData">信息数据</param>
        /// <returns>返回是否成功</returns>
        private static bool AddMainHeaderInfo(byte[] headerData, int infoType, byte[] infoData)
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return FossAddMainHeaderInfo64(headerData, infoType, infoData, infoData.Length);
                else
                    return FossAddMainHeaderInfo32(headerData, infoType, infoData, infoData.Length);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 创建光谱文件块
        /// </summary>
        /// <param name="fileCount">文件数量</param>
        /// <param name="fileTime">光谱创建时间</param>
        /// <param name="specCols">光谱数据点数量</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="compNames">组分名称</param>
        /// <param name="title">文件描述</param>
        /// <param name="instrument">仪器名称</param>
        /// <param name="serialNo">仪器序列号</param>
        /// <returns></returns>
        private static byte[] CreateMainHeader(int fileCount, DateTime fileTime, int specCols, double firstX, double lastX, string[] compNames, string title, string instrument, string serialNo)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;

                //创建主结构
                UInt32 time = CreateFileDateTime(fileTime);
                int compCount = compNames == null ? 0 : compNames.Length;   //组分数量

                if (CommonMethod.Is64BitVersion())
                    retptr = FossCreateMainHeader64((UInt32)fileCount, (UInt32)specCols, time, firstX, lastX, compCount, ref datasize);
                else
                    retptr = FossCreateMainHeader32((UInt32)fileCount, (UInt32)specCols, time, firstX, lastX, compCount, ref datasize);

                byte[] headerDatas = CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
                if (headerDatas == null)
                {
                    ErrorString = CommonMethod.ErrorString;
                    return null;
                }

                //文件描述
                if (!string.IsNullOrWhiteSpace(title))
                {
                    if (AddMainHeaderInfo(headerDatas, MAIN_TITLE, Encoding.Default.GetBytes(title)) == false)
                        return null;
                }

                //仪器描述
                if (!string.IsNullOrWhiteSpace(instrument))
                {
                    if (AddMainHeaderInfo(headerDatas, MAIN_INSTRUMENT, Encoding.Default.GetBytes(instrument)) == false)
                        return null;
                }

                //仪器序列号
                if (!string.IsNullOrWhiteSpace(serialNo))
                {
                    if (AddMainHeaderInfo(headerDatas, MAIN_SERIALNO, Encoding.Default.GetBytes(serialNo)) == false)
                        return null;
                }

                //组分信息
                if(compNames != null && compNames.Length != 0)
                {
                    byte[] compBytes = new byte[compNames.Length * CompNameLength];   //一个组分长度为CompNameLength
                    for(int i=0; i<compNames.Length; i++)
                    {
                        var namebytes = StringToBytesArray(compNames[i], CompNameLength);
                        Array.Copy(namebytes, 0, compBytes, i * CompNameLength, CompNameLength);
                    }
                    if (AddMainHeaderInfo(headerDatas, MAIN_COMPONENT, compBytes) == false)
                        return null;
                }

                return headerDatas;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 创建光谱文件块
        /// </summary>
        /// <param name="sampleCode">样品编号</param>
        /// <param name="index">光谱序号</param>
        /// <param name="fileTime">光谱创建时间</param>
        /// <param name="yDatas">y轴数据</param>
        /// <param name="compDatas">浓度数据</param>
        /// <returns></returns>
        private static byte[] CreateFileData(string sampleCode, int index, DateTime fileTime, double[] yDatas, double[] compDatas)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                UInt32 time = CreateFileDateTime(fileTime);

                //处理样品编号
                byte[] codeBytes = StringToBytesArray(sampleCode, SAMPLECODELEN);
                byte[] IDBytes = StringToBytesArray(sampleCode, SAMPLEIDLEN);   //因为样品编号位数比较少，因此在sampleID里面保存一份长的

                int dataSize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = FossCreateFileData64(codeBytes, (UInt16)index, time, yDatas, (UInt32)yDatas.Length, compDatas, compDatas == null ? 0 : (UInt32)compDatas.Length, IDBytes, ref dataSize);
                else
                    retptr = FossCreateFileData32(codeBytes, (UInt16)index, time, yDatas, (UInt32)yDatas.Length, compDatas, compDatas == null ? 0 : (UInt32)compDatas.Length, IDBytes, ref dataSize);

                return CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, dataSize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        //private static byte[] Create
        /// <summary>
        /// 创建光谱文件索引
        /// </summary>
        /// <param name="sampleNo">样品编号列表</param>
        /// <returns></returns>
        private static byte[] CreateIndexData(string[] sampleNo)
        {
            if (sampleNo == null || sampleNo.Length == 0)
                return null;

            int len = 0x10;     //一条的长度是0x10
            int namelen = 0x0c; //名字最大长度为0x0c
            try
            {
                byte[] retData = new byte[sampleNo.Length * len];
                for (int i = 0; i < sampleNo.Length; i++ )
                {
                    var namebyte = Encoding.Default.GetBytes(sampleNo[i]);
                    Array.Copy(namebyte, 0, retData, i * len, namebyte.Length < namelen ? namebyte.Length : namelen);
                    namebyte = BitConverter.GetBytes((UInt16)i);
                    Array.Copy(namebyte, 0, retData, i * len + namelen + 1, namebyte.Length);
                }

                return retData;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取保存后文件大小
        /// </summary>
        /// <param name="fileCount">光谱文件数量</param>
        /// <param name="specCols">光谱数据点数量</param>
        /// <returns></returns>
        private static int GetFileSaveSize(int fileCount, int specCols)
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return FossGetSaveSize64(fileCount, specCols);
                else
                    return FossGetSaveSize32(fileCount, specCols);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// 读取光谱文件
        /// </summary>
        public static bool ReadFile(byte[] fileData, FileFormat fileFormat)
        {
            fileFormat.dataInfoList = null;
            fileFormat.acquisitionInfo = null;
            fileFormat.xDataList = null;
            fileFormat.fileInfo = null;
            fileFormat.yDataList = null;

            MainHeader mainheader = GetMainHeader(fileData);
            if (mainheader.fileMark == UInt16.MaxValue)  //读取文件头错误
                return false;

            //填充光谱文件信息
            fileFormat.fileInfo = new FileFormat.FileInfo();
            fileFormat.fileInfo.createTime = CreateFileDateTime(mainheader.fileTime);
            fileFormat.fileInfo.dataCount = (int)mainheader.specCols;
            fileFormat.fileInfo.fileMemo = CommonMethod.CovertByteArrayToString(mainheader.tile, Encoding.Default);
            //仪器描述 仪器名称-序列号
            fileFormat.fileInfo.instDescription = CommonMethod.CovertByteArrayToString(mainheader.instrument, Encoding.Default) + instSerialSeprator +
                CommonMethod.CovertByteArrayToString(mainheader.serialNo, Encoding.Default); 
            fileFormat.fileInfo.modifyFlag = 0;
            fileFormat.fileInfo.resolution = 16;    //固定值

            fileFormat.fileInfo.specType = FileFormat.SPECTYPE.SPCNIR;      //固定值
            fileFormat.fileInfo.xType = FileFormat.XAXISTYPE.XNMETR;        //mainheader.xType = 1
            fileFormat.fileInfo.zType = FileFormat.ZAXISTYPE.XMSEC;         //固定值
            fileFormat.fileInfo.fzinc = 0.5f;           //固定值
            fileFormat.fileInfo.fspare = new float[8];  //固定值
            
            //读取X轴数据（肯定是均匀的X轴）
            fileFormat.xDataList = new List<double[]>();
            double[] tempx = new double[mainheader.specCols];
            int index = 0;
            for(int i=0; i<mainheader.rangeCount; i++)  //每个光谱段读取
            {
                for(int j=0; j<mainheader.rangeCols[i]; j++)
                {
                    tempx[index] = mainheader.rangeFirstX[i] + j * mainheader.rangeStepX[i];
                    index++;
                }
            }
            fileFormat.xDataList.Add(tempx);

            //获取Y数据以及格式信息
            fileFormat.dataInfoList = new List<FileFormat.DataInfo>();
            fileFormat.yDataList = new List<double[]>();

            for(int i=0; i<mainheader.fileCount; i++)
            {
                FileHeader fileHeader = GetFileHeader(fileData, i);
                if (fileHeader.position == UInt16.MaxValue)
                    return false;

                //读取Y轴数据
                double[] ydata = GetSpectrumData(fileData, i);
                if (ydata == null || ydata.Length == 0)
                    return false;
                fileFormat.yDataList.Add(ydata);

                //Y轴数据格式
                FileFormat.DataInfo info = new FileFormat.DataInfo();
                info.dataTitle = CommonMethod.CovertByteArrayToString(fileHeader.sampleNo, Encoding.Default);
                info.dataType = FileFormat.YAXISTYPE.YABSRB;
                info.firstX = mainheader.rangeFirstX[0];
                info.lastX = mainheader.rangeLastX[mainheader.rangeCount-1];
                info.maxYValue = ydata.Max();
                info.minYValue = ydata.Min();
                fileFormat.dataInfoList.Add(info);
            }
            
            //读取光谱参数
            fileFormat.acquisitionInfo = new FileFormat.AcquisitionInfo();
            fileFormat.acquisitionInfo.GAIN = 0;
            fileFormat.acquisitionInfo.HIGHPASS = 0;
            fileFormat.acquisitionInfo.LOWPASS = 0;
            fileFormat.acquisitionInfo.LWN = 0;
            fileFormat.acquisitionInfo.SPEED = null;
            fileFormat.acquisitionInfo.SCANS = 0;
            fileFormat.acquisitionInfo.SCANSBG = 0;
            fileFormat.acquisitionInfo.IRMODE = FileFormat.enumIRMODE.NearIR;
            
            return true;
        }

        /// <summary>
        /// 读取文件中组分列表
        /// <param name="fileData">文件内容</param>
        /// </summary>
        public static string[] GetComponentName(byte[] fileData)
        {
            MainHeader mainheader = GetMainHeader(fileData);
            if (mainheader.fileMark == UInt16.MaxValue)  //读取文件头错误
                return null;

            string[] retData = new string[mainheader.componentCount];
            byte[] nameArray = new byte[CompNameLength];  //组分名称最长0x10

            for(int i=0; i<mainheader.componentCount; i++)
            {
                Array.Copy(mainheader.components, i * CompNameLength, nameArray, 0, CompNameLength);
                retData[i] = CommonMethod.CovertByteArrayToString(nameArray, Encoding.Default);
            }

            return retData;
        }

        /// <summary>
        /// 保存光谱
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <returns></returns>
        public static byte[] SaveFile(FileFormat fileFormat)
        {
            return SaveFile(fileFormat, null, null, null, null);
        }  

        /// <summary>
        /// 保存光谱
        /// </summary>
        /// <param name="fileFormat">光谱数据</param>
        /// <param name="serialNo">仪器序列号</param>
        /// <param name="compNames">组分名称</param>
        /// <param name="compDatas">组分浓度</param>
        /// <param name="fileTimes">文件时间</param>
        /// <returns></returns>
        public static byte[] SaveFile(FileFormat fileFormat, string serialNo, string[] compNames, double[] compDatas, DateTime[] fileTimes)
        {
            //必须相同的X轴才能保存
            for (int i = 1; i < fileFormat.xDataList.Count; i++)
            {
                if (CommonMethod.IsSameXDatas(fileFormat.xDataList[i], fileFormat.xDataList[0]) == false)
                {
                    ErrorString = "X datas is different";
                    return null;
                }
            }

            //Y轴数据点数必须相同
            for(int i=1; i<fileFormat.yDataList.Count; i++)
            {
                if(fileFormat.yDataList[i].Length != fileFormat.yDataList[0].Length)
                {
                    ErrorString = "Y datas is different";
                    return null;
                }
            }

            //如果提供了组分名称，必须提供相应的组分浓度数据
            if(compNames != null)
            {
                if(compDatas == null || compNames.Length * fileFormat.yDataList.Count !=  compDatas.Length )
                {
                    ErrorString = "Invalid component datas";
                    return null;
                }
            }

            if(fileTimes != null && fileTimes.Length != fileFormat.yDataList.Count)
            {
                ErrorString = "Invalid file times";
                return null;
            }

            //获取保存文件的大小
            int saveSize = GetFileSaveSize(fileFormat.dataInfoList.Count, fileFormat.fileInfo.dataCount);
            if (saveSize == 0)
                return null;
            byte[] retDatas = new byte[saveSize];

            //主结构
            byte[] mainHeader = CreateMainHeader(fileFormat.dataInfoList.Count, fileFormat.fileInfo.createTime, fileFormat.fileInfo.dataCount, fileFormat.dataInfo.firstX, fileFormat.dataInfo.lastX, compNames, fileFormat.fileInfo.fileMemo, fileFormat.fileInfo.instDescription, serialNo);
            if (mainHeader == null)
                return null;
            Array.Copy(mainHeader, 0, retDatas, 0, mainHeader.Length);

            int copyOffset = mainHeader.Length;

            //创建样品序号
            string[] sampleNo = new string[fileFormat.yDataList.Count];
            for (int i = 0; i < fileFormat.yDataList.Count; i++)
            {
                sampleNo[i] = string.IsNullOrWhiteSpace(fileFormat.dataInfoList[i].dataTitle) ? i.ToString() : fileFormat.dataInfoList[i].dataTitle;
            }
            
            //逐条增加光谱数据
            for (int i = 0; i < fileFormat.yDataList.Count; i++ )
            {
                //拷贝浓度数据
                double[] curCompData = null;
                if(compNames != null)
                {
                    curCompData = new double[compNames.Length];
                    Array.Copy(compDatas, i * compNames.Length, curCompData, 0, compNames.Length);
                }

                DateTime curtime = fileTimes == null ? fileFormat.fileInfo.createTime : fileTimes[i];   //文件创建时间
                byte[] fileData = CreateFileData(sampleNo[i], i, curtime, fileFormat.yDataList[i], curCompData);
                if (fileData == null)
                    return null;

                Array.Copy(fileData, 0, retDatas, copyOffset, fileData.Length);

                copyOffset += fileData.Length;
            }

            //添加文件索引
            var indexData = CreateIndexData(sampleNo);
            if (indexData == null)
                return null;
            Array.Copy(indexData, 0, retDatas, copyOffset, indexData.Length);

            return retDatas;
        }
    }
}
