using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.FileFormat
{
    unsafe class SPAFileFormat
    {
        public static string ErrorString = null;

        #region SPAHeaderDefine
        [StructLayout(LayoutKind.Explicit, Pack=1)]
        struct SPAHeader
        {
            [FieldOffset(0x00)]
            public Int32 specType;		//通常=03 
            [FieldOffset(0x04)]
            public Int32 dataCount;
            [FieldOffset(0x08)]
            public Int32 xType;			//01=cm-1
            [FieldOffset(0x0C)]
            public Int32 yType;			//0x0C=log(1/R), 0x0F=Single Beam
            [FieldOffset(0x10)]
            public float firstx;
            [FieldOffset(0x14)]
            public float lastx;
            [FieldOffset(0x18)]
            public Int32 unknown2;
            [FieldOffset(0x1C)]
            public Int32 scanPoints;		//扫描点数8480
            [FieldOffset(0x20)]
            public Int32 unknown3;
            [FieldOffset(0x24)]
            public Int32 sampleScans;		//样品扫描次数64
            [FieldOffset(0x28)]
            public Int32 sampleFTPeak;		//样品干涉图峰位4096
            [FieldOffset(0x2C)]
            public Int32 resolution;		//分辨率 = 32768 / resolution; 0x00表示分辨率为4cm-1
            [FieldOffset(0x30)]
            public Int32 bgFTPeak;			//背景干涉图峰位4096
            [FieldOffset(0x34)]
            public Int32 bgScans;			//背景扫描次数64
            [FieldOffset(0x38)]
            public float bgGrain;			//背景增益1.0
            [FieldOffset(0x3C)]
            public fixed byte unknown4[8];
            [FieldOffset(0x44)]
            public Int32 scanTime;			//采集时间(单位：0.01s)
            [FieldOffset(0x48)]
            public fixed byte unknown5[8];
            [FieldOffset(0x50)]
            public float lwn;
            [FieldOffset(0x54)]
            public float adaptRatio;		//采样间隔2.0
            [FieldOffset(0x58)]
            public Int32 unknown6;
            [FieldOffset(0x5C)]
            public float aperture;			//光阑100.0
            [FieldOffset(0x60)]
            public fixed byte unknown7[0x2C];
        }

        //仪器硬件信息
        [StructLayout(LayoutKind.Explicit,Pack=1)]
        struct InstrumentHeader
        {
            [FieldOffset(0x00)]
            public fixed byte unknown1[0x10];
            /// <summary>
            /// 数字化位数:  24
            /// </summary>
            [FieldOffset(0x10)]
            public UInt32 ADBits;		
            /// <summary>
            /// 高通滤波:  200.0000
            /// </summary>
            [FieldOffset(0x14)]
            public float highFilter;
            /// <summary>
            /// 低通滤波:  20000.0000
            /// </summary>
            [FieldOffset(0x18)]
            public float lowFilter;
            [FieldOffset(0x1C)]
            public fixed byte unknown2[0x10];
            /// <summary>
            /// 样品增益1.0
            /// </summary>
            [FieldOffset(0x2C)]
            public float sampleGrain;
            /// <summary>
            /// 动镜速度:  1.2659
            /// </summary>
            [FieldOffset(0x30)]
            public float speed;
            [FieldOffset(0x34)]
            public fixed byte unknown3[4];
        };

	    /// <summary>
	    /// 模型参数
	    /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct ModelParameter
        {
            /// <summary>
            /// 全部光谱数量
            /// </summary>
            public Int32 allFileCount;
            /// <summary>
            /// 建模光谱数量
            /// </summary>
            public Int32 calFileCount;
            /// <summary>
            /// 内部验证光谱数量
            /// </summary>
            public Int32 innerTestFileCount;
            /// <summary>
            /// 外部验证光谱数量
            /// </summary>
            public Int32 outerTestFileCount;
            /// <summary>
            /// 光谱文件X轴起始波数
            /// </summary>
            public double fileFirstX;		//原始光谱X轴起始波数
            /// <summary>
            /// 光谱文件X轴结束波数
            /// </summary>
            public double fileLastX;		//原始光谱X轴结束波数
            /// <summary>
            ///光谱数据点数量
            /// </summary>
            public Int32 fileSpecCols;
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
            public Int32 modelSpecCols;
            /// <summary>
            /// 模型X轴数据间隔
            /// </summary>
            public double modelStepX;		//X轴数据间隔
        };

	    /// <summary>
	    /// 组分信息
	    /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct ComponentInfo
        {
            /// <summary>
            /// 组分名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] name;
            /// <summary>
            /// 组分单位
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] unit;
            /// <summary>
            /// 平均马氏距离
            /// </summary>
            double mahalDistance;
            /// <summary>
            /// 最小浓度
            /// </summary>
            double lowestConcentration;
            /// <summary>
            /// 最大浓度
            /// </summary>
            double highestConcentration;
        };

        #endregion

        #region 32bitDLL
        /// <summary>
        /// 是否授权SPC文件
        /// </summary>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAHasAuthority")]
        private static extern bool SPAHasAuthority32();

        /// <summary>
        /// 是否为SPA文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAIsSPAFile")]
        private static extern bool SPAIsSPAFile32(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取SPA X轴信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>XHeader结构</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetXHeader")]
        private static extern IntPtr SPAGetXHeader32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取SPA文件Title
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>Title（字符串）</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetFileTitle")]
        private static extern IntPtr SPAGetFileTitle32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取Y轴数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(double)</param>
        /// <returns>Y轴数据（double）</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetYDatas")]
        private static extern IntPtr SPAGetYDatas32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取光谱创建仪器序列号
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>仪器序列号（string）</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetInstSN")]
        private static extern IntPtr SPAGetInstSN32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取仪器信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>仪器信息</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetInstrumentHeader")]
        private static extern IntPtr SPAGetInstrumentHeader32(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 创建SPA光谱文件
        /// </summary>
        /// <param name="xDataPara">X轴数据参数</param>
        /// <param name="instPara">仪器信息</param>
        /// <param name="yDatas">Y轴数据</param>
        /// <param name="fileTitle">文件描述</param>
        /// <param name="titleSize">文件描述大小(BYTE)</param>
        /// <param name="instSN">仪器序列号</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>返回SPA格式数据(BYTE)</returns>
	    [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPACreateFile")]
        private static extern IntPtr SPACreateFile32(ref SPAHeader xDataPara, ref InstrumentHeader instPara, double[] yDatas, byte[] fileTitle, int titleSize, string instSN, ref int outDataSize);

        /// <summary>
        /// 获取模型参数信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetModelParameter")]
        private static extern IntPtr SPAGetModelParameter32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取模型组分信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="compCount">组分数量</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetComponents")]
        private static extern IntPtr SPAGetComponents32(byte[] fileData, int fileSize, ref int compCount);

        #endregion

        #region 64bitDLL
        /// <summary>
        /// 是否授权SPC文件
        /// </summary>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAHasAuthority")]
        private static extern bool SPAHasAuthority64();

        /// <summary>
        /// 是否为SPA文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAIsSPAFile")]
        private static extern bool SPAIsSPAFile64(byte[] fileData, int fileSize);

        /// <summary>
        /// 读取SPA X轴信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>XHeader结构</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetXHeader")]
        private static extern IntPtr SPAGetXHeader64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取SPA文件Title
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>Title（字符串）</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetFileTitle")]
        private static extern IntPtr SPAGetFileTitle64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取Y轴数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(double)</param>
        /// <returns>Y轴数据（double）</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetYDatas")]
        private static extern IntPtr SPAGetYDatas64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取光谱创建仪器序列号
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>仪器序列号（string）</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetInstSN")]
        private static extern IntPtr SPAGetInstSN64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 读取仪器信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>仪器信息</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetInstrumentHeader")]
        private static extern IntPtr SPAGetInstrumentHeader64(byte[] fileData, int fileSize, ref int outDataSize);

        /// <summary>
        /// 创建SPA光谱文件
        /// </summary>
        /// <param name="xDataPara">X轴数据参数</param>
        /// <param name="instPara">仪器信息</param>
        /// <param name="yDatas">Y轴数据</param>
        /// <param name="fileTitle">文件描述</param>
        /// <param name="titleSize">文件描述大小(BYTE)</param>
        /// <param name="instSN">仪器序列号</param>
        /// <param name="outDataSize">返回数据大小(BYTE)</param>
        /// <returns>返回SPA格式数据(BYTE)</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPACreateFile")]
        private static extern IntPtr SPACreateFile64(ref SPAHeader xDataPara, ref InstrumentHeader instPara, double[] yDatas, byte[] fileTitle, int titleSize, string instSN, ref int outDataSize);

        /// <summary>
        /// 获取模型参数信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetModelParameter")]
        private static extern IntPtr SPAGetModelParameter64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取模型组分信息
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="compCount">组分数量</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SPAGetComponents")]
        private static extern IntPtr SPAGetComponents64(byte[] fileData, int fileSize, ref int compCount);

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
                    return SPAHasAuthority64();
                else
                    return SPAHasAuthority32();
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
        /// <param name="fileSize">文件大小</param>
        public static bool IsSPAFile(byte[] fileData, int fileSize)
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return SPAIsSPAFile64(fileData, fileSize);
                else
                    return SPAIsSPAFile32(fileData, fileSize);
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
        /// <param name="fileSize">文件大小</param>
        /// <returns>SPCHeader结构</returns>
        private static SPAHeader GetXHeader(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = SPAGetXHeader64(fileData, fileSize, ref datasize);
                else
                    retptr = SPAGetXHeader32(fileData, fileSize, ref datasize);

                bool retOK;
                SPAHeader retheader = CommonMethod.CopyStructureFromIntptrAndFree<SPAHeader>(ref retptr, out retOK);
                if (!retOK)
                    retheader.dataCount = int.MaxValue;     //表示错误数据

                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                SPAHeader retheader = new SPAHeader();
                retheader.dataCount = int.MaxValue;     //表示错误数据                
                return retheader;
            }
        }

        /// <summary>
        /// 获取文件的Title
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        private static string GetFileTitle(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = SPAGetFileTitle64(fileData, fileSize, ref datasize);
                else
                    retptr = SPAGetFileTitle32(fileData, fileSize, ref datasize);

                byte[] copybytes = CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
                if (copybytes == null)
                    return null;

                return CommonMethod.CovertByteArrayToString(copybytes, Encoding.Default);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
 }

        /// <summary>
        /// 获取文件的仪器序列号
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        private static string GetInstSN(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = SPAGetInstSN64(fileData, fileSize, ref datasize);
                else
                    retptr = SPAGetInstSN32(fileData, fileSize, ref datasize);

                byte[] copybytes = CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
                if (copybytes == null)
                    return null;

                return Encoding.UTF8.GetString(copybytes);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 读取Y轴数据
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        private static double[] GetYData(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = SPAGetYDatas64(fileData, fileSize, ref datasize);
                else
                    retptr = SPAGetYDatas32(fileData, fileSize, ref datasize);

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 读取仪器硬件参数
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>SPCHeader结构</returns>
        private static InstrumentHeader GetInstrumentHeader(byte[] fileData, int fileSize)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = SPAGetInstrumentHeader64(fileData, fileSize, ref datasize);
                else
                    retptr = SPAGetInstrumentHeader32(fileData, fileSize, ref datasize);

                bool retOK;
                InstrumentHeader retheader = CommonMethod.CopyStructureFromIntptrAndFree<InstrumentHeader>(ref retptr, out retOK);
                if (!retOK)
                    retheader.ADBits = int.MaxValue;     //表示错误数据

                return retheader;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                InstrumentHeader retheader = new InstrumentHeader();
                retheader.ADBits = int.MaxValue;
                return retheader;
            }
        }

        private static byte[] CreateFile(SPAHeader xPara, InstrumentHeader instPara, double[] yDatas, string fileTitle, string instSN)
        {
            IntPtr retptr = IntPtr.Zero;
            int datasize = 0;

            try
            {
                byte[] titleBytes = null;
                if (!string.IsNullOrWhiteSpace(fileTitle))
                    titleBytes = Encoding.Default.GetBytes(fileTitle);

                if (CommonMethod.Is64BitVersion())
                    retptr = SPACreateFile64(ref xPara, ref instPara, yDatas, titleBytes, titleBytes == null ? 0 : titleBytes.Length, instSN, ref datasize);
                else
                    retptr = SPACreateFile32(ref xPara, ref instPara, yDatas, titleBytes, titleBytes == null ? 0 : titleBytes.Length, instSN, ref datasize);

                return CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取模型参数信息
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        private static ModelParameter QuantGetModelParameter(byte[] fileData)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;

                if (fileData == null || fileData.Length == 0)
                    throw new Exception("Invalid Parameter");

                if (CommonMethod.Is64BitVersion())
                    retptr = SPAGetModelParameter64(fileData, fileData.Length);
                else
                    retptr = SPAGetModelParameter32(fileData, fileData.Length);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                bool retOK = true;
                var retData =  CommonMethod.CopyStructureFromIntptrAndFree <ModelParameter>(ref retptr, out retOK);
                if (!retOK)
                {
                    ErrorString = "Invalid data reading";
                    retData.allFileCount = int.MaxValue;    //错误
                }

                return retData;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                var retData = new ModelParameter();
                retData.allFileCount = int.MaxValue;
                return retData;
            }

        }

        /// <summary>
        /// 获取组分信息
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        private static List<ComponentInfo> QuantGetComponentInfo(byte[] fileData)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int compCount = 0;
                if (fileData == null || fileData.Length == 0)
                    throw new Exception("Invalid Parameter");

                if (CommonMethod.Is64BitVersion())
                    retptr = SPAGetComponents64(fileData, fileData.Length, ref compCount);
                else
                    retptr = SPAGetComponents32(fileData, fileData.Length, ref compCount);

                if (retptr == IntPtr.Zero)
                    throw new Exception(FileFormat.GetDLLErrorMessage());

                return CommonMethod.CopyStructureListFromIntptrAndFree<ComponentInfo>(ref retptr, compCount);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 读取光谱文件
        /// </summary>
        public static bool ReadFile(byte[] fileData, int fileSize, FileFormat fileFormat)
        {
            fileFormat.dataInfoList = null;
            fileFormat.acquisitionInfo = null;
            fileFormat.xDataList = null;
            fileFormat.fileInfo = null;
            fileFormat.yDataList = null;

            SPAHeader mainheader = GetXHeader(fileData, fileSize);
            if (mainheader.dataCount == int.MaxValue)  //读取文件头错误
                return false;

            //填充光谱文件信息
            fileFormat.fileInfo = new FileFormat.FileInfo();
            //fileFormat.fileInfo.createTime = CommonMethod.DWordToDateTime(0xD9A36D78);
            fileFormat.fileInfo.dataCount = (int)mainheader.dataCount;
            fileFormat.fileInfo.fileMemo = GetFileTitle(fileData, fileSize);
            fileFormat.fileInfo.instDescription = GetInstSN(fileData, fileSize);
            fileFormat.fileInfo.modifyFlag = 0;
            fileFormat.fileInfo.resolution = mainheader.resolution == 0 ? 4.0 : 32768 / mainheader.resolution;

            fileFormat.fileInfo.specType = FileFormat.SPECTYPE.SPCNIR;      //固定值
            fileFormat.fileInfo.xType = FileFormat.XAXISTYPE.XWAVEN;        //mainheader.xType = 1
            fileFormat.fileInfo.zType = FileFormat.ZAXISTYPE.XMSEC;         //固定值
            fileFormat.fileInfo.fzinc = 0.5f;           //固定值
            fileFormat.fileInfo.fspare = new float[8];  //固定值
            
            //读取X轴数据（肯定是均匀的X轴）
            fileFormat.xDataList = new List<double[]>();
            double[] tempx = new double[mainheader.dataCount];
            double stepx = (mainheader.lastx - mainheader.firstx) /(tempx.Length -1);
            for(int i=0; i<tempx.Length; i++)
                tempx[i] = mainheader.firstx + i*stepx;
            fileFormat.xDataList.Add(tempx);

            //获取Y数据以及格式信息
            fileFormat.dataInfoList = new List<FileFormat.DataInfo>();
            fileFormat.yDataList = new List<double[]>();

            //读取Y轴数据
            double[] ydata = GetYData(fileData, fileSize);
            if (ydata == null || ydata.Length == 0)
                return false;

            //Y轴数据格式
            FileFormat.DataInfo info = new FileFormat.DataInfo();
            info.dataType = mainheader.yType == 0x0C ? FileFormat.YAXISTYPE.YABSRB : (mainheader.yType == 0x0F ? FileFormat.YAXISTYPE.YSCRF : FileFormat.YAXISTYPE.YSCSM);
            info.firstX = mainheader.firstx;
            info.lastX = mainheader.lastx;
            info.maxYValue = ydata.Max();
            info.minYValue = ydata.Min();

            fileFormat.dataInfoList.Add(info);
            fileFormat.yDataList.Add(ydata);
            
            InstrumentHeader instHeader = GetInstrumentHeader(fileData, fileSize);

            //读取光谱参数
            fileFormat.acquisitionInfo = new FileFormat.AcquisitionInfo();
            fileFormat.acquisitionInfo.GAIN = mainheader.bgGrain;
            fileFormat.acquisitionInfo.HIGHPASS = instHeader.highFilter;
            fileFormat.acquisitionInfo.LOWPASS = instHeader.lowFilter;
            fileFormat.acquisitionInfo.LWN = mainheader.lwn;
            fileFormat.acquisitionInfo.SPEED = instHeader.speed.ToString();
            fileFormat.acquisitionInfo.SCANS = mainheader.sampleScans;
            fileFormat.acquisitionInfo.SCANSBG = mainheader.bgScans;
            fileFormat.acquisitionInfo.IRMODE = FileFormat.enumIRMODE.NearIR;
            
            return true;
        }

        public static byte[] SaveFile(FileFormat fileFormat)
        {
            SPAHeader mainheader = new SPAHeader();

            //填充光谱文件信息
            mainheader.dataCount = fileFormat.fileInfo.dataCount;
            mainheader.resolution = (int)(32768 / fileFormat.fileInfo.resolution);
            mainheader.firstx = (float)fileFormat.dataInfo.firstX;
            mainheader.lastx = (float)fileFormat.dataInfo.lastX;
            if(fileFormat.dataInfo.dataType == FileFormat.YAXISTYPE.YABSRB)
                mainheader.yType = 0x0C;
            else if(fileFormat.dataInfo.dataType == FileFormat.YAXISTYPE.YSCRF)
                mainheader.yType = 0x0F;
            mainheader.bgGrain = (float)fileFormat.acquisitionInfo.GAIN;
            mainheader.lwn = (float)fileFormat.acquisitionInfo.LWN;
            mainheader.sampleScans = fileFormat.acquisitionInfo.SCANS ;
            mainheader.bgScans = fileFormat.acquisitionInfo.SCANSBG ;

            //读取光谱参数
            InstrumentHeader instPara = new InstrumentHeader();
            instPara.highFilter = (float)fileFormat.acquisitionInfo.HIGHPASS;
            instPara.lowFilter = (float)fileFormat.acquisitionInfo.LOWPASS;


            return CreateFile(mainheader, instPara, fileFormat.yDatas, fileFormat.fileInfo.fileMemo, fileFormat.fileInfo.instDescription);
        }

        /// <summary>
        /// 获取模型的参数信息
        /// </summary>
        /// <param name="fileData">模型文件数据</param>
        /// <returns>模型参数</returns>
        public static Ai.Hong.FileFormat.QuantModelFormat.QuantModelParameter GetModelParameter(byte[] fileData)
        {
            ModelParameter modelInfo = QuantGetModelParameter(fileData);
            if (modelInfo.allFileCount == int.MaxValue)
                return null;

            Ai.Hong.FileFormat.QuantModelFormat.QuantModelParameter retModelInfo = new QuantModelFormat.QuantModelParameter();
            retModelInfo.allFileCount = modelInfo.allFileCount;
            retModelInfo.calFileCount = modelInfo.calFileCount;
            retModelInfo.innerTestFileCount = modelInfo.innerTestFileCount;
            retModelInfo.outerTestFileCount = modelInfo.outerTestFileCount;

            retModelInfo.fileFirstX = modelInfo.fileFirstX;
            retModelInfo.fileLastX = modelInfo.fileLastX;
            retModelInfo.fileXStep = modelInfo.fileXStep;
            retModelInfo.fileSpecCols = modelInfo.fileSpecCols;

            retModelInfo.modelFirstX = modelInfo.modelFirstX;
            retModelInfo.modelLastX = modelInfo.modelLastX;
            retModelInfo.modelSpecCols = modelInfo.modelSpecCols;
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
            var comps = QuantGetComponentInfo(fileData);
            if (comps == null)
                return null;

            var retDatas = new List<Ai.Hong.FileFormat.QuantModelFormat.QuantComponentInfo>();
            foreach (var item in comps)
            {
                var curInfo = new Ai.Hong.FileFormat.QuantModelFormat.QuantComponentInfo();
                curInfo.name = CommonMethod.CovertByteArrayToString(item.name, Encoding.Default);
                curInfo.unit = CommonMethod.CovertByteArrayToString(item.unit, Encoding.Default);
                retDatas.Add(curInfo);
            }

            return retDatas;
        }
    }
}
