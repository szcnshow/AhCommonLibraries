using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.FileFormat
{
    /// <summary>
    /// Matlab文件读写类
    /// </summary>
    public class MatlabFileFormat
    {
        private const int MAXDIMENSIONS = 10;			//最大支持10维数组

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

        /// <summary>
        /// 固定的光谱数据块名称
        /// </summary>
        public const string ACloudSpectrumDataName = "ACloudSpectrumXYDatas";

        /// <summary>
        /// 固定的浓度数据块名称
        /// </summary>
        public const string ACloudConcentrationDataName = "ACloudConcentrationDatas";

        #region MatlabHeader

        /// <summary>
        /// Matlab变量信息结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct MatVariableInfo
        {
            /// <summary>
            /// 变量数据类型
            /// </summary>
            public UInt16 entryType;			//变量数据类型
            /// <summary>
            /// 变量数据大小（字节）
            /// </summary>
            public UInt32 entrySize;		//变量数据大小（字节）,如果dataType=miMATRIX，dataSize需要包含64位对齐，否则不包含
            /// <summary>
            /// 数据在文件中的偏移量
            /// </summary>
            public UInt32 entryOffset;		//数据在文件中的偏移量
            /// <summary>
            /// 数据标志（complex, global, logical）
            /// </summary>
            public byte flag;				//数据标志（complex, global, logical）
            /// <summary>
            /// 数组数据类型
            /// </summary>
            public byte arrayType;			//数组数据类型
            /// <summary>
            /// 实际的数据类型
            /// </summary>
            public UInt16 realDataType;			//实际的数据类型
            /// <summary>
            /// 实际数据在Entry中的偏移
            /// </summary>
            public UInt32 realDataOffset;		//实际数据在Entry中的偏移
            /// <summary>
            /// 实际数据大小
            /// </summary>
            public UInt32 realDataSize;			//实际数据大小
            /// <summary>
            /// 虚数的数据类型
            /// </summary>
            public UInt16 imageDataType;			//虚数的数据类型
            /// <summary>
            /// 虚数在Entry中的偏移
            /// </summary>
            public UInt32 imageDataOffset;		//虚数在Entry中的偏移
            /// <summary>
            /// 虚数数据大小
            /// </summary>
            public UInt32 imageDataSize;		//虚数数据大小
            /// <summary>
            /// 维数
            /// </summary>
            public UInt16 dimensions;		//维数
            /// <summary>
            /// 各维的大小
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXDIMENSIONS)]
            public UInt32[] dimension;		//各维的大小
            /// <summary>
            /// 变量名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 127)]
            public string name;		//变量名称
        }

        /// <summary>
        /// 变量数据类型
        /// </summary>
        public enum enumMatDataType
        {
            /// <summary>
            /// 8 bit, signed
            /// </summary>
            miINT8 = 1,  //8 bit, signed
            /// <summary>
            /// 8 bit, unsigned
            /// </summary>
            miUINT8 = 2, //8 bit, unsigned
            /// <summary>
            /// 16 - bit, signed
            /// </summary>
            miINT16 = 3, //16 - bit, signed
            /// <summary>
            /// 16 - bit, unsigned
            /// </summary>
            miUINT16 = 4, //16 - bit, unsigned
            /// <summary>
            /// 32 - bit, signed
            /// </summary>
            miINT32 = 5,  //32 - bit, signed
            /// <summary>
            /// 32 - bit, unsigned
            /// </summary>
            miUINT32 = 6,  //32 - bit, unsigned
            /// <summary>
            /// IEEE® 754 single format
            /// </summary>
            miSINGLE = 7,  //IEEE® 754 single format
            /// <summary>
            /// IEEE 754 double format
            /// </summary>
            miDOUBLE = 9, //IEEE 754 double format
            /// <summary>
            /// 64 - bit, signed
            /// </summary>
            miINT64 = 12,  //64 - bit, signed
            /// <summary>
            /// 64 - bit, unsigned
            /// </summary>
            miUINT64 = 13,  //64 - bit, unsigned
            /// <summary>
            /// MATLAB array
            /// </summary>
            miMATRIX = 14,  //MATLAB array
            /// <summary>
            /// Compressed Data
            /// </summary>
            miCOMPRESSED = 15,  //Compressed Data
            /// <summary>
            /// Unicode UTF - 8 Encoded Character Data
            /// </summary>
            miUTF8 = 16,  //Unicode UTF - 8 Encoded Character Data
            /// <summary>
            /// Unicode UTF - 16 Encoded Character Data
            /// </summary>
            miUTF16 = 17, //Unicode UTF - 16 Encoded Character Data
            /// <summary>
            /// Unicode UTF - 32 Encoded Character Data
            /// </summary>
            miUTF32 = 18,   //Unicode UTF - 32 Encoded Character Data
        };

        /// <summary>
        /// 数组数据类型
        /// </summary>
        public enum enumMatArrayDataType
        {
            /// <summary>
            /// Cell array
            /// </summary>
            mxCELL_CLASS = 1,	//Cell array 1
            /// <summary>
            /// Structure
            /// </summary>
            mxSTRUCT_CLASS = 2,	//Structure 2
            /// <summary>
            /// Object
            /// </summary>
            mxOBJECT_CLASS = 3,	//Object 3
            /// <summary>
            /// Character array 4
            /// </summary>
            mxCHAR_CLASS = 4,	//Character array 4
            /// <summary>
            /// Sparse array
            /// </summary>
            mxSPARSE_CLASS = 5,	//Sparse array 5
            /// <summary>
            /// Double precision array
            /// </summary>
            mxDOUBLE_CLASS = 6,	//Double precision array 6
            /// <summary>
            /// Single precision array
            /// </summary>
            mxSINGLE_CLASS = 7,	//Single precision array 7
            /// <summary>
            /// 8 - bit, signed integer
            /// </summary>
            mxINT8_CLASS = 8,	//8 - bit, signed integer 8
            /// <summary>
            /// 8 - bit, unsigned integer
            /// </summary>
            mxUINT8_CLASS = 9,	//8 - bit, unsigned integer 9
            /// <summary>
            /// 16 - bit, signed integer
            /// </summary>
            mxINT16_CLASS = 10,	//16 - bit, signed integer 10
            /// <summary>
            /// 16 - bit, unsigned integer
            /// </summary>
            mxUINT16_CLASS = 11,	//16 - bit, unsigned integer 11
            /// <summary>
            /// 32 - bit, signed integer
            /// </summary>
            mxINT32_CLASS = 12, //32 - bit, signed integer 12
            /// <summary>
            /// 32 - bit, unsigned integer
            /// </summary>
            mxUINT32_CLASS = 13, //32 - bit, unsigned integer 13
            /// <summary>
            /// 64 - bit, signed integer
            /// </summary>
            mxINT64_CLASS = 14,		//64 - bit, signed integer 14
            /// <summary>
            /// 64 - bit, unsigned integer
            /// </summary>
            mxUINT64_CLASS = 15,	//64 - bit, unsigned integer 15
        };
        #endregion

        #region 32bitDLL
        /// <summary>
        /// 是否授权Matlab文件
        /// </summary>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATHasAuthority")]
        private static extern bool MATHasAuthority32();

        /// <summary>
        /// 是否为Matlab文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATIsMATFile")]
        private static extern bool MATIsMATFile32(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取Mat文件变量列表
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="entryCount">变量数量</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATGetVariableList")]
        private static extern IntPtr MATGetVariableList32(byte[] fileData, int fileSize, ref int entryCount);

        /// <summary>
        /// 获取Mat变量数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="variableInfo">变量信息</param>
        /// <param name="outDataSize">数据大小(BYTE)</param>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATGetVariableData")]
        private static extern IntPtr MATGetVariableData32(byte[] fileData, int fileSize, ref MatVariableInfo variableInfo, ref int outDataSize);

        /// <summary>
        /// 创建MATLAB文件头
        /// </summary>
        /// <param name="description">文件描述</param>
        /// <param name="retDataCount">返回数据大小(BYTE)</param>
        /// <returns>返回BYTE[]</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATCreateFileHeader")]
        private static extern IntPtr MATCreateFileHeader32(string description, ref int retDataCount);

        /// <summary>
        /// 创建double类型的数据块（不压缩格式）
        /// </summary>
        /// <param name="fileData">需要写入的内容(double)</param>
        /// <param name="dataSize">内容大小(double)</param>
        /// <param name="entryName">数据块名称</param>
        /// <param name="dataRows">数据行数</param>
        /// <param name="dataCols">每行数据点数</param>
        /// <param name="retDataCount">返回数据大小(BYTE)</param>
        /// <returns>Matlab格式数据块</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATCreateDoubleEntry")]
        private static extern IntPtr MATCreateDoubleEntry32(double[] fileData, int dataSize, string entryName, int dataRows, int dataCols, ref int retDataCount);

        /// <summary>
        /// 初始化MAT文件，创建matVariables信息，必须调用End
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>初始化结果</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATFileInitialize")]
        private static extern bool MATFileInitialize32(byte[] fileData, int fileSize);

        /// <summary>
        /// 清除MAT文件信息
        /// </summary>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATFileTerminate")]
        private static extern void MATFileTerminate32();

        /// <summary>
        /// 从MATLab中读取数据块(double)
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="entryName">数据块名称</param>
        /// <param name="dataRows">out:数据块行数 </param>
        /// <param name="dataCols">out:数据块列数</param>
        /// <returns>double数组,大小=dataRows*dataCols</returns>
        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATGetDoubleArrayData")]
        private static extern IntPtr MATGetDoubleArrayData32(byte[] fileData, int fileSize, string entryName, ref int dataRows, ref int dataCols);

        [DllImport("SpectrumFileFormat_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATCreateDoubleDataFile")]
        private static extern IntPtr MATCreateDoubleDataFile(double[] fileData, int dataSize, string entryName, int dataRows, int dataCols, ref int retDataCount);
        #endregion

        #region 64bitDLL
        /// <summary>
        /// 是否授权Matlab文件
        /// </summary>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATHasAuthority")]
        private static extern bool MATHasAuthority64();

        /// <summary>
        /// 是否为Matlab文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATIsMATFile")]
        private static extern bool MATIsMATFile64(byte[] fileData, int fileSize);

        /// <summary>
        /// 获取Mat文件变量列表
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="entryCount">变量数量</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATGetVariableList")]
        private static extern IntPtr MATGetVariableList64(byte[] fileData, int fileSize, ref int entryCount);

        /// <summary>
        /// 获取Mat变量数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="variableInfo">变量信息</param>
        /// <param name="outDataSize">数据大小(BYTE)</param>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATGetVariableData")]
        private static extern IntPtr MATGetVariableData64(byte[] fileData, int fileSize, ref MatVariableInfo variableInfo, ref int outDataSize);

        /// <summary>
        /// 创建MATLAB文件头
        /// </summary>
        /// <param name="description">文件描述</param>
        /// <param name="retDataCount">返回数据大小(BYTE)</param>
        /// <returns>返回BYTE[]</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATCreateFileHeader")]
        private static extern IntPtr MATCreateFileHeader64(string description, ref int retDataCount);

        /// <summary>
        /// 创建double类型的数据块（不压缩格式）
        /// </summary>
        /// <param name="fileData">需要写入的内容(double)</param>
        /// <param name="dataSize">内容大小(double)</param>
        /// <param name="entryName">数据块名称</param>
        /// <param name="dataRows">数据行数</param>
        /// <param name="dataCols">每行数据点数</param>
        /// <param name="retDataCount">返回数据大小(BYTE)</param>
        /// <returns>Matlab格式数据块</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATCreateDoubleEntry")]
        private static extern IntPtr MATCreateDoubleEntry64(double[] fileData, int dataSize, string entryName, int dataRows, int dataCols, ref int retDataCount);

        /// <summary>
        /// 初始化MAT文件，创建matVariables信息，必须调用MATFileTerminate
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns>初始化结果</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATFileInitialize")]
        private static extern bool MATFileInitialize64(byte[] fileData, int fileSize);

        /// <summary>
        /// 清除MAT文件信息
        /// </summary>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATFileTerminate")]
        private static extern void MATFileTerminate64();

        /// <summary>
        /// 从MATLab中读取数据块(double)
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="entryName">数据块名称</param>
        /// <param name="dataRows">out:数据块行数 </param>
        /// <param name="dataCols">out:数据块列数</param>
        /// <returns>double数组,大小=dataRows*dataCols</returns>
        [DllImport("SpectrumFileFormat_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MATGetDoubleArrayData")]
        private static extern IntPtr MATGetDoubleArrayData64(byte[] fileData, int fileSize, string entryName, ref int dataRows, ref int dataCols);


        #endregion

        /// <summary>
        /// 创建MATLAB文件头
        /// </summary>
        /// <param name="description">文件描述</param>
        /// <returns>返回BYTE[]</returns>
        public static byte[] CreateFileHeader(string description)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                description = "MATLAB 5.0 " + description;      //一定要加上MATLAB 5.0才能被再次读出来
                if (CommonMethod.Is64BitVersion())
                    retptr = MATCreateFileHeader64(description, ref datasize);
                else
                    retptr = MATCreateFileHeader32(description, ref datasize);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = FileFormat.GetDLLErrorMessage();
                    return null;
                }

                return CommonMethod.CopyByteFromIntptrAndFree(ref retptr, datasize);
            }
            catch(Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 创建double类型的数据块（不压缩格式）
        /// </summary>
        /// <param name="fileData">数据</param>
        /// <param name="dataRows">数据行数</param>
        /// <param name="entryName">数据块名称</param>
        /// <returns>Matlab数据块</returns>
        public static byte[] CreateDoubleEntry(double[] fileData, int dataRows, string entryName = ACloudSpectrumDataName)
        {
            if (fileData == null || fileData.Length < 0 || (fileData.Length % dataRows) != 0 || string.IsNullOrWhiteSpace(entryName))
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            try
            {
                int dataCols = fileData.Length / dataRows;

                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = MATCreateDoubleEntry64(fileData, fileData.Length, entryName, dataRows, dataCols, ref datasize);
                else
                    retptr = MATCreateDoubleEntry32(fileData, fileData.Length, entryName, dataRows, dataCols, ref datasize);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = FileFormat.GetDLLErrorMessage();
                    return null;
                }

                return CommonMethod.CopyByteFromIntptrAndFree(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
   }

        /// <summary>
        /// 创建double类型的数据块（不压缩格式）
        /// </summary>
        /// <param name="fileDatas">数据列表（列表中所有数据大小相同）</param>
        /// <param name="entryName">数据块名称</param>
        /// <returns>Matlab数据块</returns>
        public static byte[] CreateDoubleEntry(List<double[]> fileDatas, string entryName = ACloudSpectrumDataName)
        {
            if (fileDatas == null || fileDatas.Count == 0 || string.IsNullOrWhiteSpace(entryName))
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            try
            {
                double[] combinedDatas = CommonMethod.CombineSpectrumDatas(fileDatas);
                if (combinedDatas == null)
                {
                    ErrorString = CommonMethod.ErrorString;
                    return null;
                }

                return CreateDoubleEntry(combinedDatas, fileDatas.Count);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 打开Matlab文件(必须调用MatlabFileClose)
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <returns>是否成功</returns>
        public static bool MatlabFileOpen(byte[] fileData)
        {
            if (fileData == null)
            {
                ErrorString = "Invalid parameters";
                return false;
            }

            try
            {
                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = MATFileInitialize64(fileData, fileData.Length);
                else
                    retcode = MATFileInitialize32(fileData, fileData.Length);

                if (retcode == false)
                    ErrorString = FileFormat.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 关闭已经打开的Matlab文件
        /// </summary>
        /// <returns>是否成功</returns>
        public static void MatlabFileClose()
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    MATFileTerminate64();
                else
                    MATFileTerminate32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
            }
        }

        /// <summary>
        /// 加载double数组类型的数据块
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <param name="dataCols">数组的列数</param>
        /// <param name="entryName">数据块名称</param>
        /// <returns>是否成功</returns>
        public static double[] ReadDoubleArrayData(byte[] fileData, ref int dataCols, string entryName = ACloudSpectrumDataName)
        {
            int dataRows = 0;
            dataCols = 0;
            IntPtr retptr = IntPtr.Zero;

            try
            {
                if (CommonMethod.Is64BitVersion())
                    retptr = MATGetDoubleArrayData64(fileData, fileData.Length, entryName, ref dataRows, ref dataCols);
                else
                    retptr = MATGetDoubleArrayData32(fileData, fileData.Length, entryName, ref dataRows, ref dataCols);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = FileFormat.GetDLLErrorMessage();
                    return null;
                }

                return CommonMethod.CopyDoubleFromIntptrAndFree(ref retptr, dataRows * dataCols);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 加载double数组类型的数据块
        /// </summary>
        /// <param name="fileData">文件内容</param>
        /// <param name="entryName">数据块名称</param>
        /// <returns>按行分割的数据</returns>
        public static List<double[]> ReadDoubleArrayData(byte[] fileData, string entryName = ACloudSpectrumDataName)
        {
            int dataCols = 0;
            double[] aryDatas = ReadDoubleArrayData(fileData, ref dataCols, entryName);
            if (aryDatas == null)
                return null;

            try
            {
                List<double[]> retDatas = new List<double[]>();
                int dataRows = aryDatas.Length / dataCols;
                for (int i = 0; i < dataRows; i++)
                {
                    double[] tempdata = new double[dataCols];
                    Array.Copy(aryDatas, i * dataCols, tempdata, 0, dataCols);
                    retDatas.Add(tempdata);
                }

                return retDatas;
            }
            catch(Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        #region 32/64Bit自适应

        /// <summary>
        /// 是否授权为MAT文件
        /// </summary>
        public static bool HasAuthority()
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return MATHasAuthority64();
                else
                    return MATHasAuthority32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 是否为MAT文件
        /// </summary>
        /// <param name="fileData">文件数据</param>
        public static bool IsMATFile(byte[] fileData)
        {
            if (fileData == null || fileData.Length == 0)
                return false;

            bool retcode = false;

            try
            {
                if (CommonMethod.Is64BitVersion())
                    retcode = MATIsMATFile64(fileData, fileData.Length);
                else
                    retcode = MATIsMATFile32(fileData, fileData.Length);

                if (retcode == false)
                    ErrorString = FileFormat.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取MAT文件的变量信息列表
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <returns>MAT变量信息结构列表</returns>
        public static List<MatVariableInfo> GetVariableList(byte[] fileData)
        {
            if (fileData == null || fileData.Length == 0)
                return null;

            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = MATGetVariableList64(fileData, fileData.Length, ref datasize);
                else
                    retptr = MATGetVariableList32(fileData, fileData.Length, ref datasize);

                return CommonMethod.CopyStructureListFromIntptrAndFree<MatVariableInfo>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 读取变量的数据
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="variableInfo">需要读取的变量信息</param>
        /// <returns>变量的数据</returns>
        public static T[] GetVariableData<T>(byte[] fileData, MatVariableInfo variableInfo) where T : IComparable
        {
            if (fileData == null || fileData.Length == 0)
                return null;

            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;   //返回的是byte大小
                if (CommonMethod.Is64BitVersion())
                    retptr = MATGetVariableData64(fileData, fileData.Length, ref variableInfo, ref datasize);
                else
                    retptr = MATGetVariableData32(fileData, fileData.Length, ref variableInfo, ref datasize);

                if (retptr == IntPtr.Zero)
                    return null;

                //按照数据类型来拷贝返回数据
                int typesize = Marshal.SizeOf(typeof(T));
                if ((datasize % typesize) != 0)
                {
                    Marshal.FreeCoTaskMem(retptr);
                    return null;
                }

                datasize = datasize / typesize;

                return CommonMethod.CopyDataArrayFromIntptrAndFree<T>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        #endregion

        /*
        /// <summary>
        /// Matlab变量列表
        /// </summary>
        public List<MatVariableInfo> variableInfoList = null;
        private byte[] matFileData = null;

        private bool OpenFile(string filename)
        {
            //检查MAT是授权
            if (string.IsNullOrWhiteSpace(filename) || !System.IO.File.Exists(filename) ||
                !MatlabFileFormat.HasAuthority())
                return false;

            //检查是否为MAT文件
            matFileData = CommonMethod.ReadBinaryFile(filename);
            if(matFileData == null || matFileData.Length == 0 || !IsMATFile(matFileData))
            {
                matFileData = null;
                variableInfoList = null;
                return false;
            }

            //获取变量列表
            variableInfoList = MatlabFileFormat.GetVariableList(matFileData);
            if(variableInfoList == null)
            {
                matFileData = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取变量数据类型
        /// </summary>
        /// <param name="info">变量信息</param>
        /// <returns>数据类型</returns>
        public static Type GetVariableType(MatVariableInfo info)
        {
            switch ((enumMatDataType)info.realDataType)
            {
                case enumMatDataType.miINT8:
                case enumMatDataType.miUINT8:
                    return typeof(byte);
                case enumMatDataType.miINT16:
                    return typeof(Int16);
                case enumMatDataType.miUINT16:
                    return typeof(UInt16);
                case enumMatDataType.miINT32:
                    return typeof(Int32);
                case enumMatDataType.miUINT32:
                    return typeof(UInt32);
                case enumMatDataType.miSINGLE:
                    return typeof(float);
                case enumMatDataType.miDOUBLE:
                    return typeof(double);
                case enumMatDataType.miINT64:
                    return typeof(Int64);
                case enumMatDataType.miUINT64:
                    return typeof(UInt64);
                case enumMatDataType.miUTF8:
                case enumMatDataType.miUTF16:
                case enumMatDataType.miUTF32:
                    return typeof(string);
                case enumMatDataType.miMATRIX:
                case enumMatDataType.miCOMPRESSED:
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取数值型变量数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="info">变量信息</param>
        /// <returns></returns>
        public T[] GetNumericalArrayData<T>(MatVariableInfo info) where T:IComparable
        {
            return MatlabFileFormat.GetVariableData<T>(matFileData, info);
        }

        public List<string> GetStringArrayData(MatVariableInfo info)
        {
            enumMatDataType datatype = (enumMatDataType)info.realDataType;
            if (datatype != enumMatDataType.miUINT8 && datatype != enumMatDataType.miUINT16 && datatype != enumMatDataType.miUINT32)
                return null;

            byte[] datas = MatlabFileFormat.GetVariableData<byte>(matFileData, info);
            if (datas == null)
                return null;

            return null;
            //if (datatype == enumMatDataType.miUINT8)
            //    return Encoding.UTF8.GetString(datas);
        }

    }
         * */
    }
}
