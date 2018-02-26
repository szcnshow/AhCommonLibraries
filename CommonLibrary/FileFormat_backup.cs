using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Ai.Hong.CommonLibrary
{
    class MappedFile
    {
        #region CreateFile DLLs and Const
        [DllImport("kernel32.dll")]
        private static extern int CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttributes, uint flProtect, uint dwMaxSizeHi, uint dwMaxSizeLow, string lpName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool WriteFile(int hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, uint lpOverlapped);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMapping, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnmapViewOfFile(IntPtr pvBaseAddress);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", EntryPoint = "GetLastError")]
        public static extern int GetLastError();

        [DllImport("kernel32", EntryPoint = "GetFileSize")]
        public static extern UInt32 GetFileSize(int hFile, ref UInt32 FileSizeHigh);

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint OPEN_EXISTING = 3;
        const uint CREATE_NEW = 4;
        const uint CREATE_ALWAYS = 5;

        const uint ERROR_ALREADY_EXISTS = 183;

        const uint FILE_MAP_COPY = 0x0001;
        const uint FILE_MAP_WRITE = 0x0002;
        const uint FILE_MAP_READ = 0x0004;
        const uint FILE_MAP_ALL_ACCESS = 0x0002 | 0x0004;

        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;

        const uint PAGE_READONLY = 0x02;
        const uint PAGE_READWRITE = 0x04;
        const uint PAGE_WRITECOPY = 0x08;
        const uint PAGE_EXECUTE = 0x10;
        const uint PAGE_EXECUTE_READ = 0x20;
        const uint PAGE_EXECUTE_READWRITE = 0x40;

        const uint SEC_COMMIT = 0x8000000;
        const uint SEC_IMAGE = 0x1000000;
        const uint SEC_NOCACHE = 0x10000000;
        const uint SEC_RESERVE = 0x4000000;

        const int INVALID_HANDLE_VALUE = -1;
        #endregion

        private string _filename;
        int hFile = INVALID_HANDLE_VALUE;
        IntPtr hMap = IntPtr.Zero;
        IntPtr lpMap = IntPtr.Zero;

        public IntPtr FilePointer { get { return lpMap; } }
        public string FileName
        {
            get { return _filename; }
            set
            {
                _filename = value;
                OpenAndMapFile();
            }
        }

        public MappedFile(string filename)
        {
            _filename = filename;
            OpenAndMapFile();
        }

        ~MappedFile()
        {
            CloseOpenedFile();
        }

        public void UnloadFile()
        {
            CloseOpenedFile();
        }
        public bool FileOpened { get { return lpMap != IntPtr.Zero; } }     //文件是否打开
        //关闭文件及其Mapping
        void CloseOpenedFile()
        {
            if (lpMap != IntPtr.Zero)
                UnmapViewOfFile(lpMap);

            lpMap = IntPtr.Zero;
            if (hMap != IntPtr.Zero)
                CloseHandle(hMap);

            hMap = IntPtr.Zero;
            if (hFile != INVALID_HANDLE_VALUE)
                CloseHandle((IntPtr)hFile);

            hFile = INVALID_HANDLE_VALUE;
        }

        //打开文件及其Mapping
        bool OpenAndMapFile()
        {
            CloseOpenedFile();

            if (_filename == null)
                return false;

            try
            {
                hFile = CreateFile(_filename, GENERIC_READ, FILE_SHARE_READ, 0, OPEN_EXISTING, 0, 0);
                if (hFile == INVALID_HANDLE_VALUE)
                    throw new Exception("Can't Open File");

                hMap = CreateFileMapping(hFile, IntPtr.Zero, PAGE_READONLY, 0, 0, string.Empty);
                if (hMap == IntPtr.Zero)
                    throw new Exception("Can't Create File Mapping");

                lpMap = (IntPtr)MapViewOfFile(hMap, FILE_MAP_READ, 0, 0, 0);
                if (lpMap == IntPtr.Zero)
                    throw new Exception("Can't Create View of Mapping");

                uint sizeHight = 0;
                FileSize = GetFileSize(hFile, ref sizeHight);
            }
            catch (Exception)
            {
                CloseOpenedFile();
                return false;
            }

            return true;
        }

        //将偏移offset的内存复制到struct中
        public T MapStruct<T>(uint offset, Type abc)
        {
            try
            {
                IntPtr newptr = new IntPtr(lpMap.ToInt32() + (int)offset);
                if (newptr == IntPtr.Zero)
                    return default(T);

                return (T)Marshal.PtrToStructure(newptr, abc);
            }
            catch (System.Exception)
            {
                return default(T);
            }

        }

        //读取文件大小, 只取低32位
        public uint FileSize
        {
            get;
            private set;
        }

        //将偏移offset的内存中读取dataSize个Float数据
        public float[] ReadFloatData(uint offset, uint dataSize)
        {
            try
            {
                IntPtr newptr = new IntPtr(lpMap.ToInt32() + (int)offset); 
                if (newptr == IntPtr.Zero)
                    return null;

                float[] retdata = new float[dataSize];
                if (retdata == null)
                    return null;

                Marshal.Copy(newptr, retdata, 0, (int)dataSize);
                return retdata;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        //将偏移offset的内存中读取byte个数据
        public byte[] ReadByteData(uint offset, uint dataSize)
        {
            try
            {
                IntPtr newptr = new IntPtr(lpMap.ToInt32() + (int)offset);
                if (newptr == IntPtr.Zero)
                    return null;

                byte[] retdata = new byte[dataSize];
                if (retdata == null)
                    return null;

                Marshal.Copy(newptr, retdata, 0, (int)dataSize);
                return retdata;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public bool ReadByteData(byte[] buffer, uint bufferOffset, uint fileOffset, uint readSize)
        {
            if (fileOffset + readSize > FileSize)
                return false;
            if (bufferOffset + readSize > buffer.Length)
                return false;

            IntPtr newptr = new IntPtr(lpMap.ToInt32() + (int)fileOffset);
            if (newptr == IntPtr.Zero)
                return false;

            Marshal.Copy(newptr, buffer, (int)bufferOffset, (int)readSize);

            return true;
        }

 
        //将偏移offset的内存中读取DWORD个数据
        public int[] ReadIntData(uint offset, uint dataSize)
        {
            try
            {
                IntPtr newptr = new IntPtr(lpMap.ToInt32() + (int)offset); 
                if (newptr == IntPtr.Zero)
                    return null;

                int[] retdata = new int[dataSize];
                if (retdata == null)
                    return null;

                Marshal.Copy(newptr, retdata, 0, (int)dataSize);
                return retdata;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public bool CreateAndWriteFile(string filename, byte[] buffer, uint writeSize=0)
        {
            int hFile = INVALID_HANDLE_VALUE;

            //创建新文件并写入
            if ((hFile = CreateFile(filename, GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0)) == INVALID_HANDLE_VALUE)
                return false;

            if (writeSize == 0)
                writeSize = (uint)buffer.Length;

            uint retsize = 0;
            WriteFile(hFile, buffer, writeSize, ref retsize, 0);
            CloseHandle((IntPtr)hFile);

            if (writeSize != retsize)
                return false;

            return true;
        }
    }

    class MemoryMappedFile
    {
        private byte[] fileData = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileData"></param>
        public MemoryMappedFile(byte[] fileData)
        {
            this.fileData = fileData;
        }

        /// <summary>
        /// 将内存映射到结构
        /// </summary>
        /// <typeparam name="T">结构类型</typeparam>
        /// <param name="offset">内存数据</param>
        /// <param name="T">结构类型</param>
        /// <returns>结构</returns>
        public T MapStruct<T>(uint offset, Type abc)
        {
            try
            {
                byte[] bytes = new byte[Marshal.SizeOf(abc)];
                Array.Copy(fileData, (int)offset, bytes, 0, bytes.Length);
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();

                return theStructure;
            }
            catch (System.Exception)
            {
                return default(T);
            }

        }

        /// <summary>
        /// 读取文件大小
        /// </summary>
        public uint FileSize
        {
            get { return (uint)fileData.Length; }
        }

        /// <summary>
        /// 将偏移offset的内存中读取dataSize个Float数据
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="dataSize">数据量</param>
        /// <returns></returns>
        public float[] ReadFloatData(uint offset, uint dataSize)
        {
            try
            {
                float[] retdata = new float[dataSize];

                for (int i = 0; i < dataSize; i ++)
                {
                    retdata[i] = BitConverter.ToSingle(fileData, (int)offset + i * sizeof(float));
                }

                return retdata;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 将偏移offset的内存中读取dataSize个byte数据
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="dataSize">数据量</param>
        /// <returns></returns>
        public byte[] ReadByteData(uint offset, uint dataSize)
        {
            try
            {
                byte[] retdata = new byte[dataSize];
                Array.Copy(fileData, (int)offset, retdata, 0, dataSize);

                return retdata;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 将偏移offset的内存中读取readSize个byte数据到buffer中
        /// </summary>
        /// <param name="buffer">写入的内存byte[]</param>
        /// <param name="bufferOffset">byte[]的偏移量</param>
        /// <param name="fileOffset">文件中读取的偏移量</param>
        /// <param name="readSize">读取的数量</param>
        /// <returns></returns>
        public bool ReadByteData(byte[] buffer, uint bufferOffset, uint fileOffset, uint readSize)
        {
            if (fileOffset + readSize > FileSize)
                return false;
            if (bufferOffset + readSize > buffer.Length)
                return false;

            Array.Copy(fileData, (int)fileOffset, buffer, bufferOffset, readSize);

            return true;
        }


        /// <summary>
        /// 将偏移offset的内存中读取dataSize个int数据
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="dataSize">数据量</param>
        /// <returns></returns>
        public int[] ReadIntData(uint offset, uint dataSize)
        {
            try
            {
                int[] retdata = new int[dataSize];

                for (int i = 0; i < dataSize; i++)
                {
                    retdata[i] = BitConverter.ToInt32(fileData, (int)offset + i * sizeof(int));
                }

                return retdata;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }

    //光谱文件参数表
    public class FileParameter
    {
        /// <summary>
        /// 光谱点数量
        /// </summary>
        public uint dataCount;

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
        /// 放大系数
        /// </summary>
        public double scaleYValue;

        /// <summary>
        /// 扫描时间
        /// </summary>
        public DateTime time;

        /// <summary>
        /// 分辨率
        /// </summary>
        public string resolution;

        /// <summary>
        /// 光谱类型(近红外，拉曼...)
        /// </summary>
        public string specType;

        /// <summary>
        /// X轴坐标类型(1/CM...)
        /// </summary>
        public string xType;

        /// <summary>
        /// 数据类型(吸收谱, 干涉图...)
        /// </summary>
        public string dataType;

        public static FileParameter CreateFromData(double[] xDatas, double[] yDatas, int resolution = 4, string dataType = null, string specType = null, string xType = null)
        {
            if (xDatas == null || yDatas == null || xDatas.Length != yDatas.Length || xDatas.Length == 0)
                return null;

            FileParameter retParameter = new FileParameter();
            retParameter.dataCount = (uint)xDatas.Length;
            retParameter.firstX = xDatas[0];
            retParameter.lastX = xDatas[xDatas.Length - 1];
            retParameter.maxYValue = yDatas.Max();
            retParameter.minYValue = yDatas.Min();
            retParameter.scaleYValue = 1;
            retParameter.time = DateTime.Now;
            retParameter.dataType = dataType == null ? "ABSORBANCE" : dataType;
            retParameter.xType = xType == null ? "1/CM" : xType;
            retParameter.specType = specType == null ? "INFRARED SPECTRUM" : "RAMAN SHIFT";
            retParameter.resolution = resolution.ToString(); ;

            return retParameter;
        }

        public static FileParameter CreateFromData(float[] xDatas, float[] yDatas, int resolution = 4, string dataType = null, string specType = null, string xType = null)
        {
            if (xDatas == null || yDatas == null || xDatas.Length != yDatas.Length || xDatas.Length == 0)
                return null;

            FileParameter retParameter = new FileParameter();
            retParameter.dataCount = (uint)xDatas.Length;
            retParameter.firstX = xDatas[0];
            retParameter.lastX = xDatas[xDatas.Length - 1];
            retParameter.maxYValue = yDatas.Max();
            retParameter.minYValue = yDatas.Min();
            retParameter.scaleYValue = 1;
            retParameter.time = DateTime.Now;
            retParameter.dataType = dataType == null ? "ABSORBANCE" : dataType;
            retParameter.xType = xType == null ? "1/CM" : xType;
            retParameter.specType = specType == null ? "INFRARED SPECTRUM" : "RAMAN SHIFT";
            retParameter.resolution = resolution.ToString(); ;

            return retParameter;
        }
    };

    /// <summary>
    /// OPUS:Bruker格式，SPC:EssentialIR格式，FDI是带结果的OPUS格式, RDI是带结果的SPC格式, JCAMP:JCAMP-DX格式, DPT:Data Table格式
    /// </summary>
    public enum EnumFileType {FDI, OPUS, SPC, JCAMP, DPT, UNKNOWN, RDI }       
    public class SpecFileFormat
    {
        public const uint RFDAMark = 0x52464441;    //RFDI的二进制码,SampleResult标志

        public string ErrorString { get; set; }
        public FileParameter Parameter{get;set;}
        public float[] XDatas;
        public float[] YDatas;
        public EnumFileType OpenedFileType = EnumFileType.UNKNOWN;
        public string OpenedFile = null;

        public bool ReadFile(string filename)
        {
            OpenedFileType = RealFileType(filename);
            OpenedFile = filename;

            switch (OpenedFileType)
            {
                case EnumFileType.OPUS:
                case EnumFileType.FDI:
                    Parameter = OPUSFileFormat.ReadOpusFileParameter(filename, OPUSFileFormat.DBTAB);
                    ErrorString = OPUSFileFormat.ErrorString;

                    if(Parameter != null)
                    {
                        YDatas = OPUSFileFormat.ReadOpusFileData(filename, OPUSFileFormat.DBTAB);
                        ErrorString = OPUSFileFormat.ErrorString;

                        if(YDatas != null)
                        {
                            //需要处理XDatas
                            XDatas = new float[YDatas.Length];
                            float xstep = (float)((Parameter.lastX - Parameter.firstX) / (Parameter.dataCount - 1));
                            for (int i = 0; i < YDatas.Length; i++)
                                XDatas[i] = (float)Parameter.firstX + i * xstep;
                        }
                    }
                    break;
                case EnumFileType.SPC:
                case EnumFileType.RDI:
                    if (SPCFile.ReadFile(filename) == true)
                    {
                        Parameter = SPCFile.spcParameter;
                        XDatas = SPCFile.XDatas;
                        YDatas = SPCFile.YDatas;
                    }
                    else
                        ErrorString = SPCFile.ErrorString;
                    break;
                case EnumFileType.JCAMP:
                    Parameter = JCAMP_DXFile.ReadFile(filename, out XDatas, out YDatas);
                    ErrorString = JCAMP_DXFile.ErrorString;
                    break;
                default:
                    ErrorString = "文件类型参数错误";
                    break;
            }
            return Parameter != null;
        }

        public bool ReadFile(byte[] fileData)
        {
            switch (OpenedFileType)
            {
                case EnumFileType.OPUS:
                case EnumFileType.FDI:
                    Parameter = OPUSFileFormat.ReadOpusFileParameter(fileData, OPUSFileFormat.DBTAB);
                    ErrorString = OPUSFileFormat.ErrorString;

                    if (Parameter != null)
                    {
                        YDatas = OPUSFileFormat.ReadOpusFileData(fileData, OPUSFileFormat.DBTAB);
                        ErrorString = OPUSFileFormat.ErrorString;

                        if (YDatas != null)
                        {
                            //需要处理XDatas
                            XDatas = new float[YDatas.Length];
                            float xstep = (float)((Parameter.lastX - Parameter.firstX) / (Parameter.dataCount - 1));
                            for (int i = 0; i < YDatas.Length; i++)
                                XDatas[i] = (float)Parameter.firstX + i * xstep;
                        }
                    }
                    break;
                case EnumFileType.SPC:
                case EnumFileType.RDI:
                    if (SPCFile.ReadFile(fileData) == true)
                    {
                        Parameter = SPCFile.spcParameter;
                        XDatas = SPCFile.XDatas;
                        YDatas = SPCFile.YDatas;
                    }
                    else
                        ErrorString = SPCFile.ErrorString;
                    break;
                case EnumFileType.JCAMP:
                default:
                    ErrorString = "文件类型参数错误";
                    break;
            }
            return Parameter != null;
        }

        public bool SaveFile(string filename, EnumFileType fileType)
        {
            try
            {
                if (OpenedFile == null || YDatas == null || Parameter==null)
                    throw new Exception("没有打开源文件");

                switch (fileType)
                {
                    case EnumFileType.FDI:
                        if (OpenedFileType != EnumFileType.FDI)
                            throw new Exception("源文件没有包含检测信息，不能保存为FDI格式");

                        if (OpenedFile != filename)      //如果两个文件不同名，需要拷贝
                            File.Copy(OpenedFile, filename, true);
                        break;

                    case EnumFileType.OPUS:
                        if(OpenedFileType == EnumFileType.OPUS || OpenedFileType==EnumFileType.FDI) //本身是OPUS文件
                        {
                            if(OpenedFile != filename)      //如果两个文件不同名，需要拷贝
                                File.Copy(OpenedFile, filename, true);
                            OPUSFileFormat.DeleteIdentBlock(filename);      //清除文件中的检测信息
                        }
                        else    //不是OPUS文件
                        {
                            if (OPUSFileFormat.SaveFile(filename, YDatas, Parameter) == false)
                                throw new Exception(OPUSFileFormat.ErrorString);
                        }
                        break;
                    case EnumFileType.RDI:
                        if (OpenedFileType != EnumFileType.RDI)
                            throw new Exception("源文件没有包含检测信息，不能保存为RDI格式");
                        if (OpenedFile != filename)      //如果两个文件不同名，需要拷贝
                            File.Copy(OpenedFile, filename, true);
                        break;
                    case EnumFileType.SPC:
                        if(SPCFile.SaveFile(filename, YDatas, Parameter) == false)
                            throw new Exception(SPCFile.ErrorString);
                        break;
                    case EnumFileType.JCAMP:
                        if(JCAMP_DXFile.SaveFile(filename, Parameter, XDatas, YDatas) == false)
                            throw new Exception(JCAMP_DXFile.ErrorString);
                        break;
                    case EnumFileType.DPT:
                        if (DPTFormat.SaveFile(filename, YDatas, Parameter) == false)
                            throw new Exception(JCAMP_DXFile.ErrorString);
                        break;
                    default:
                        throw new Exception("文件类型参数错误");
                }            
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }

            return true;
        }

        public IntPtr ReadIdentBlock(string filename, uint blockID = RFDAMark)
        {
            IntPtr retPtr = IntPtr.Zero;
            EnumFileType type = RealFileType(filename);
            switch (type)
            {
                case EnumFileType.OPUS:
                case EnumFileType.FDI:
                    retPtr = OPUSFileFormat.ReadIdentBlock(filename);
                    ErrorString = OPUSFileFormat.ErrorString;
                    break;
                case EnumFileType.RDI:
                case EnumFileType.SPC:
                    retPtr = SPCFile.ReadIdentifyBlock(filename, blockID);
                    ErrorString = SPCFile.ErrorString;
                    break;
                default:
                    ErrorString = "文件类型参数错误";
                    break;
            }
            return retPtr;
        }

        public bool WriteIdentBlock(string filename, IntPtr writeData, uint dataSize)
        {
            bool retCode = false;
            EnumFileType type = RealFileType(filename);
            switch (type)
            {
                case EnumFileType.OPUS:
                case EnumFileType.FDI:
                    retCode = OPUSFileFormat.WriteIdentBlock(filename, writeData, dataSize);
                    ErrorString = OPUSFileFormat.ErrorString;
                    break;
                case EnumFileType.RDI:
                case EnumFileType.SPC:
                    retCode = SPCFile.WriteIdentifyBlock(filename, writeData, dataSize);
                    ErrorString = SPCFile.ErrorString;
                    break;
                case EnumFileType.JCAMP:
                default:
                    ErrorString = "文件类型参数错误";
                    break;
            }

            return retCode;
        }

        //获取真正的文件格式，目前支持OPUS, SPC, DX
        public EnumFileType RealFileType(string filename)
        {
            if (OPUSFileFormat.IsOpusFile(filename))
            {
                if (OPUSFileFormat.HasIdentBlock(filename))
                    return EnumFileType.FDI;
                else
                    return EnumFileType.OPUS;
            }
            if (SPCFile.IsSPCFile(filename))
            {
                if (SPCFile.HasIdentifyBlock(filename))
                    return EnumFileType.RDI;
                else
                    return EnumFileType.SPC;
            }
            else if(JCAMP_DXFile.IsDXFile(filename))
                return EnumFileType.JCAMP;
            else
                return EnumFileType.UNKNOWN;
        }

        /// <summary>
        /// 创建SpecFileFormat
        /// </summary>
        /// <param name="xDatas"></param>
        /// <param name="yDatas"></param>
        /// <param name="resolution">分辨率</param>
        /// <param name="dataType">数据类型(ABSORB, TRANS...)</param>
        /// <param name="specType">光谱类型(NIR, IR, RAMAN...)</param>
        /// <param name="xType">X轴类型(1/CM, RAMAN SHIFT...)</param>
        public static SpecFileFormat CreateFromData(float[] xDatas, float[] yDatas, int resolution = 4, string dataType = null, string specType = null, string xType = null)
        {
            if (xDatas == null || yDatas == null || xDatas.Length != yDatas.Length || xDatas.Length == 0)
                return null;

            SpecFileFormat retData = new SpecFileFormat();
            retData.XDatas = (float[])xDatas.Clone();
            retData.YDatas = (float[])yDatas.Clone();

            retData.Parameter = FileParameter.CreateFromData(xDatas, yDatas, resolution, dataType, specType, xType);

            return retData;
        }
    }

    public static class OPUSFileFormat
    {
        #region OPUSHeaderDefine
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct OpusHeader
        {
            [FieldOffset(0x00)]
            public uint magicNumber;		    //OPUS文件标志 # OAOAFEFE    INT32
            [FieldOffset(0x04)]
            public double programNumber;		//文件版本号     REAL64		e.g.: 901225.00
            [FieldOffset(0x0C)]
            public uint pFirstDirectory;	    //第一个目录位置	INT32
            [FieldOffset(0x10)]
            public uint maxDirBlkCount;		//块中最大的目录数		INT32
            [FieldOffset(0x14)]
            public uint curDirBlkCount;		//当前块的目录数	INT32
        };

        //directory Block 不止一个，通常是3个，根据block数量变化，最后一个block的pointer为NULL
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct DirectoryBlock
        {
            [FieldOffset(0)]
            public uint type;			    //Block type                 INT32
            [FieldOffset(4)]
            public uint length;			    //Length in 32BitWrds        INT32    block1
            [FieldOffset(8)]
            public uint pointer;			//Pointer in bytes           INT32
        };

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct ParameterBlock
        {
            [FieldOffset(0)]
            public UInt32 name;
            [FieldOffset(4)]
            public Int16 type;
            [FieldOffset(6)]
            public Int16 space;
            [FieldOffset(8)]
            public uniondata value;
        };

        [StructLayout(LayoutKind.Explicit, Size = 1)]
        public struct uniondata
        {
            [FieldOffset(0)]
            public Int32 intdata;
            [FieldOffset(0)]
            public double doubledata;
        };

        #region region OPusConst
        const uint OPUSFILEMARK = 0xFEFE0A0A;

        //0-1 bit
        const int SHIFT_DBSCPLX = 0;
        const uint MASK_DBMCPLX = 0x03;
        const uint DBTREAL = 1;		//real part of cplx data
        const uint DBTIMAG = 2;		//imag part of cplx data
        const uint DBTAMPL = 3;		//amplitude data

        //2-3 bit
        const int SHIFT_DBSSTYP = 2;
        const uint MASK_DBMSTYP = 0x03;
        const uint DBTSAMP = 1;		//sample data
        const uint DBTREF = 2;		//reference data
        const uint DBTRATIO = 3;		//ratioed data

        //4-9 bit
        const int SHIFT_DBSPARM = 4;
        const uint MASK_DBMPARM = 0x3F;
        const uint DBTDSTAT = 1;		//data status Parameter
        const uint DBTINSTR = 2;		//Instrument status parameters
        const uint DBTAQPAR = 3;		//standard acquisition parameters
        const uint DBTFTPAR = 4;		//FT-Parameters
        const uint DBTPLTPAR = 5;		//Plot- and display parameters
        const uint DBTPRCPAR = 6;		//Processing parameters
        const uint DBTGCPAR = 7;		//GC-parameters
        const uint DBTLIBPAR = 8;		//Library search parameters
        const uint DBTCOMPAR = 9;		//Communication parameters
        const uint DBTORGPAR = 10;		//Sample origin parameters

        //10-16 bit   up:向上的峰	down:向下的峰
        const int SHIFT_DBSDATA = 10;
        const uint MASK_DBMDATA = 0x7F;
        public const uint DBTSPEC = 1;		//spectrum, undefined Y-units 单通道谱
        const uint DBTIGRM = 2;		//interferogram            
        const uint DBTPHAS = 3;		//phase spectrum
        public const uint DBTAB = 4;		//absorbance spectrum				up  吸收谱
        const uint DBTTR = 5;		//transmittance spectrum			down
        const uint DBTKM = 6;		//kubelka-munck spectrum			up
        const uint DBTTRACE = 7;		//trace (intensity over time)		down
        const uint DBTGCIG = 8;		//gc file, series of interferograms
        const uint DBTGCSP = 9;		//gc file, series of spectra
        const uint DBTRAMAN = 10;		//raman spectrum					up
        const uint DBTEMIS = 11;		//emission spectrum					up
        const uint DBTREFL = 12;		//reflectance spectrum				down
        const uint DBTDIR = 13;		//directory block
        const uint DBTPOWER = 14;		//power spectrum (from phase calculation)
        const uint DBTLOGREFL = 15;		//- log reflectance (like absorbance)	up
        const uint DBTATR = 16;		//ATR-spectrum							down
        const uint DBTPAS = 17;		//photoacoustic spectrum				up
        const uint DBTARITR = 18;		//result of arithmetics, looks like TR  down
        const uint DBTARIAB = 19;		//result of arithmetics, looks like AB  up

        //17-18
        const int SHIFT_DBSDERIV = 17;
        const uint MASK_DBMDERIV = 0x03;
        const uint DBT1DERIV = 1;		//first derivative
        const uint DBT2DERIV = 2;		//second derivative
        const uint DBTNDERIV = 3;		//n-th derivative

        //19-25
        const int SHIFT_DBSEXTND = 19;
        const uint MASK_DBTMXTND = 0x7F;
        const uint DBTINFO = 1;		//compound Information
        const uint DBTPEAK = 2;		//peak table
        const uint DBTSTRC = 3;		//molecular structure
        const uint DBTMACRO = 4;		//macro
        const uint DBTLOG = 5;		//log of all actions which change data
        const uint DBBSTAT = (DBTDSTAT << SHIFT_DBSPARM);
        const uint DBBINSTR = (DBTINSTR << SHIFT_DBSPARM);
        const uint DBBAQPAR = (DBTAQPAR << SHIFT_DBSPARM);
        const uint DBBFTPAR = (DBTFTPAR << SHIFT_DBSPARM);
        const uint DBBPRCPAR = (DBTPRCPAR << SHIFT_DBSPARM);
        const uint DBBRATIO = (DBTRATIO << SHIFT_DBSSTYP);
        const uint DBBSAMP = (DBTSAMP << SHIFT_DBSSTYP);
        const uint DBBREF = (DBTREF << SHIFT_DBSSTYP);
        const uint DBBAB = (DBTAB << SHIFT_DBSDATA);
        const uint DBBIGRM = (DBTIGRM << SHIFT_DBSDATA);
        const uint DBBPHAS = (DBTPHAS << SHIFT_DBSDATA);
        const uint DBBAMPL = (DBTAMPL << SHIFT_DBSCPLX);

        //const uint AcquiParaEntry = DBBAQPAR;      //acquisition parameter, reference
        //const uint ABParaEntry = DBBSTAT | DBBRATIO | DBBAB | DBBAMPL; //data status parameter
        //const uint ABDataEntry = DBBRATIO | DBBAB | DBBAMPL;  //amplitude data
        const uint AcquiParaEntry1 = 0x40000068;
        const uint AcquiParaEntry2 = 0x00000068;
        const uint ABParaEntry1 = 0x4000101F;
        const uint ABParaEntry2 = 0x0000101F;
        const uint ABDataEntry1 = 0x4000100F;
        const uint ABDataEntry2 = 0x0000100F;

        //将鉴别结果Blcok.Type定义为：
        //也就是：type = 101010    0000101     00      0000000  111110    00        00
        //               MYMARK   DBSEXTND  DBSDERIV   DBSDATA  DBSPARM  DBSSTYP  DBSCPLX
        //1010 1000 0010 1000 0000 0011 1110 0000
        // A     8    2   8     0    3   D    0
        public const uint IDENTFYYTYPE = 0xA82803D0;
        #endregion OpusConst
        #endregion

        public static string ErrorString { get; private set; } //返回错误信息
        static OPUSFileFormat()
        {
        }

        static bool IsDataParameterBlock(string blockName)
        {
            if (blockName == "DPF" || blockName == "NPT" || blockName == "FXV" ||
                blockName == "LXV" || blockName == "CSF" || blockName == "MXY" ||
                blockName == "MNY" || blockName == "DXU" || blockName == "DYU" ||
                blockName == "DER" || blockName == "DAT" || blockName == "TIM" ||
                blockName == "XTX" || blockName == "YTX")
                return true;

            return false;
        }

        static string DWORDToString(uint name)
        {
            string retstr = "";

            for (int i = 0; i < 4; i++)
            {
                if (name >> (i * 8) != 0)
                    retstr += Convert.ToChar((name >> (i * 8)) & 0xFF);
            }
            return retstr;
        }

        //判断是否是OPUS格式文件，如果是，返回文件头
        static OpusHeader GetOpusHeader(MappedFile mapFile)
        {
            if (mapFile != null && mapFile.FileOpened && mapFile.FileSize > Marshal.SizeOf(typeof(OpusHeader)))
            {
                OpusHeader header = mapFile.MapStruct<OpusHeader>(0, typeof(OpusHeader));
                if (header.magicNumber == OPUSFILEMARK)	//不是OPUS光谱文件
                    return header;
            }

            return default(OpusHeader);
        }

        public static bool IsOpusFile(string filename)
        {
            MappedFile mapFile = new MappedFile(filename);

            if (mapFile == null || !mapFile.FileOpened)
                return false;

            OpusHeader header = GetOpusHeader(mapFile);

            return header.curDirBlkCount > 0;
        }

        //获取光谱的分辨率
        static float GetResolution(MappedFile mapFile)
        {
            OpusHeader header = mapFile.MapStruct<OpusHeader>(0, typeof(OpusHeader));
            if (header.magicNumber != OPUSFILEMARK)	//不是OPUS光谱文件
                return 0f;

            //查找AB光谱参数Block,   跳过Header, 0x18为Header的大小
            uint i;
            //2012-11-25: 改为在循环里面MapStruct
            DirectoryBlock blockEntry = new DirectoryBlock();   //mapFile.MapStruct<DirectoryBlock>(header.pFirstDirectory, typeof(DirectoryBlock));
            for (i = 0; i < header.curDirBlkCount; i++)	//在block Entries中查找与dataCount相同block
            {
                //下一块
                blockEntry = mapFile.MapStruct<DirectoryBlock>(
                    header.pFirstDirectory + i * (uint)Marshal.SizeOf(typeof(DirectoryBlock)), typeof(DirectoryBlock));

                if (blockEntry.type == AcquiParaEntry1 || blockEntry.type == AcquiParaEntry2)
                    break;
            }
            if (i >= header.curDirBlkCount)  //没找到数据参数
                return 0f;

            uint curpos = blockEntry.pointer;
            uint datalen = blockEntry.length * 4;	//参数数据块的大小

            //2012-11-25: 改为在循环里面MapStruct
            ParameterBlock blockPara = new ParameterBlock();  //mapFile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));
            while (true)
            {
                blockPara = mapFile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));

                string paraname = DWORDToString(blockPara.name);
                if (paraname == "RES")
                    return (float)blockPara.value.doubledata;

                if (paraname == "END")   //到结束块了
                    break;
                //8 = name+word+word: sizeof(DWORD) + sizeof(WORD) + sizeof(DWORD) 
                curpos += 8 + (uint)blockPara.space * 2;
                if (curpos - blockEntry.pointer >= datalen)	//超过Block大小了
                    break;
            }

            return 0f;
        }

        //blockType:数据块类型, DBTAB: AB数据 DBTSPEC:RSC数据
        public static FileParameter ReadOpusFileParameter(string filename, uint blockType)
        {
            MappedFile mapfile = null;
            try
            {
                mapfile = new MappedFile(filename);
                if (mapfile == null || !mapfile.FileOpened)
                    throw new Exception("打开文件错误");

                uint curpos = 0;
                uint i;

                OpusHeader header = mapfile.MapStruct<OpusHeader>(0, typeof(OpusHeader));
                if (header.magicNumber != OPUSFILEMARK)	//不是OPUS光谱文件
                    throw new Exception("不是OPUS光谱");

                //修改：2013-11-25, 改为在循环里面MapStruct
                //查找AB光谱参数Block,   跳过Header, 0x18为Header的大小
                //DirectoryBlock blockEntry = mapfile.MapStruct<DirectoryBlock>(
                //    header.pFirstDirectory, typeof(DirectoryBlock));
                DirectoryBlock blockEntry = new DirectoryBlock();
                for (i = 0; i < header.curDirBlkCount; i++)	//在block Entries中查找与dataCount相同block
                {
                    //下一块
                    blockEntry = mapfile.MapStruct<DirectoryBlock>(
                        header.pFirstDirectory + i * (uint)Marshal.SizeOf(typeof(DirectoryBlock)), typeof(DirectoryBlock));

                    if (blockEntry.type == ABParaEntry1 || blockEntry.type == ABParaEntry2)
                        break;
                }
                if (i >= header.curDirBlkCount)  //没找到数据参数
                    throw new Exception("没有找到数据块");

                //获取光谱参数值, 只取：NPT数据数量，FXV最大X值，LXV最小X值，CSF Y值放大系数，MXY最大Y值，MNY最小Y值
                uint datalen = blockEntry.length * 4;	//参数数据块的大小
                curpos = blockEntry.pointer;

                //判断是数据参数Block
                ParameterBlock blockPara = mapfile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));
                if (!IsDataParameterBlock(DWORDToString(blockPara.name)))
                    throw new Exception("没有找到数据块");

                FileParameter dataParameter = new FileParameter();
                while (true)
                {
                    blockPara = mapfile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));
                    string paraname = DWORDToString(blockPara.name);
                    switch (paraname)
                    {
                        case "NPT":
                            dataParameter.dataCount = (uint)blockPara.value.intdata;
                            break;
                        case "FXV":
                            dataParameter.firstX = blockPara.value.doubledata;
                            break;
                        case "LXV":
                            dataParameter.lastX = blockPara.value.doubledata;
                            break;
                        case "CSF":
                            dataParameter.scaleYValue = blockPara.value.doubledata;
                            break;
                        case "MXY":
                            dataParameter.maxYValue = blockPara.value.doubledata;
                            break;
                        case "MNY":
                            dataParameter.minYValue = blockPara.value.doubledata;
                            break;
                    }
                    if (paraname == "END")   //到结束块了
                        break;

                    //8 = name+word+word: sizeof(DWORD) + sizeof(WORD) + sizeof(DWORD) 
                    curpos += 8 + (uint)blockPara.space * 2;
                    if (curpos - blockEntry.pointer >= datalen)	//超过Block大小了
                        break;
                }

                dataParameter.resolution = "4.0";
                dataParameter.specType = "INFRARED SPECTRUM";           //设置为傅立叶红外光谱
                dataParameter.xType = "1/CM";                           //设置为Wavenumber (cm-1)
                if (blockType == DBTAB)
                    dataParameter.dataType = "ABSORBANCE";              //设置为吸收谱
                else if (blockType == OPUSFileFormat.DBTSPEC)
                    dataParameter.dataType = "TRANSMITTANCE";           //设置为透过谱(单通道)
                else
                    dataParameter.dataType = "UNKNOWN";

                dataParameter.resolution = GetResolution(mapfile).ToString("F2");

                if (dataParameter.dataCount == 0 || (dataParameter.firstX < 0.0001) ||
                    (dataParameter.lastX < 0.0001) || (dataParameter.scaleYValue < 0.00001))	//没有找到关键参数
                    throw new Exception("数据错误");

                //放大系数也影响了maxYValue和minYValue
                dataParameter.maxYValue = dataParameter.maxYValue / dataParameter.scaleYValue;
                dataParameter.minYValue = dataParameter.minYValue / dataParameter.scaleYValue;

                return dataParameter;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
            finally
            {
                if (mapfile != null)
                    mapfile.UnloadFile();
            }
        }

        //ABData:返回数据, dataSize:数据数量
        public static float[] ReadOpusFileData(string filename, uint blockType)
        {
            MappedFile mapfile = null;
            try
            {
                mapfile = new MappedFile(filename);
                if (mapfile == null || !mapfile.FileOpened)
                    throw new Exception("打开文件错误");

                uint i;

                OpusHeader header = mapfile.MapStruct<OpusHeader>(0, typeof(OpusHeader));
                if (header.magicNumber != OPUSFILEMARK)	//不是OPUS光谱文件
                    throw new Exception("不是OPUS光谱");

                //修改：2013-11-25, 改为在循环里面MapStruct
                //查找AB数据块位置 跳过Header, 0x18为Header的大小
                //DirectoryBlock blockEntry = mapfile.MapStruct<DirectoryBlock>(
                //    header.pFirstDirectory, typeof(DirectoryBlock));
                DirectoryBlock blockEntry = new DirectoryBlock();
                for (i = 0; i < header.curDirBlkCount; i++)	//在block Entries中查找与dataCount相同block
                {
                    //下一块
                    blockEntry = mapfile.MapStruct<DirectoryBlock>(
                        header.pFirstDirectory + i * (uint)Marshal.SizeOf(typeof(DirectoryBlock)), typeof(DirectoryBlock));
                    if (blockEntry.type == ABDataEntry1 || blockEntry.type == ABDataEntry2)
                        break;

                }
                if (i >= header.curDirBlkCount)
                    throw new Exception("没有找到数据块");

                float[] retData = new float[blockEntry.length];
                if (retData == null)
                    throw new Exception("内存不足");

                return mapfile.ReadFloatData(blockEntry.pointer, blockEntry.length);
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
            finally
            {
                if (mapfile != null)
                    mapfile.UnloadFile();
            }

        }

        //获取光谱的分辨率
        static float GetResolution(MemoryMappedFile mapFile)
        {
            OpusHeader header = mapFile.MapStruct<OpusHeader>(0, typeof(OpusHeader));
            if (header.magicNumber != OPUSFILEMARK)	//不是OPUS光谱文件
                return 0f;

            //查找AB光谱参数Block,   跳过Header, 0x18为Header的大小
            uint i;
            //2012-11-25: 改为在循环里面MapStruct
            DirectoryBlock blockEntry = new DirectoryBlock();   //mapFile.MapStruct<DirectoryBlock>(header.pFirstDirectory, typeof(DirectoryBlock));
            for (i = 0; i < header.curDirBlkCount; i++)	//在block Entries中查找与dataCount相同block
            {
                //下一块
                blockEntry = mapFile.MapStruct<DirectoryBlock>(
                    header.pFirstDirectory + i * (uint)Marshal.SizeOf(typeof(DirectoryBlock)), typeof(DirectoryBlock));

                if (blockEntry.type == AcquiParaEntry1 || blockEntry.type == AcquiParaEntry2)
                    break;
            }
            if (i >= header.curDirBlkCount)  //没找到数据参数
                return 0f;

            uint curpos = blockEntry.pointer;
            uint datalen = blockEntry.length * 4;	//参数数据块的大小

            //2012-11-25: 改为在循环里面MapStruct
            ParameterBlock blockPara = new ParameterBlock();  //mapFile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));
            while (true)
            {
                blockPara = mapFile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));

                string paraname = DWORDToString(blockPara.name);
                if (paraname == "RES")
                    return (float)blockPara.value.doubledata;

                if (paraname == "END")   //到结束块了
                    break;
                //8 = name+word+word: sizeof(DWORD) + sizeof(WORD) + sizeof(DWORD) 
                curpos += 8 + (uint)blockPara.space * 2;
                if (curpos - blockEntry.pointer >= datalen)	//超过Block大小了
                    break;
            }

            return 0f;
        }

        //blockType:数据块类型, DBTAB: AB数据 DBTSPEC:RSC数据
        public static FileParameter ReadOpusFileParameter(byte[] fileData, uint blockType)
        {
            if (fileData == null)
                return null;

            MemoryMappedFile mapfile = null;
            try
            {
                mapfile = new MemoryMappedFile(fileData);
                if (mapfile == null)
                    throw new Exception("打开文件错误");

                uint curpos = 0;
                uint i;

                OpusHeader header = mapfile.MapStruct<OpusHeader>(0, typeof(OpusHeader));
                if (header.magicNumber != OPUSFILEMARK)	//不是OPUS光谱文件
                    throw new Exception("不是OPUS光谱");

                //修改：2013-11-25, 改为在循环里面MapStruct
                //查找AB光谱参数Block,   跳过Header, 0x18为Header的大小
                //DirectoryBlock blockEntry = mapfile.MapStruct<DirectoryBlock>(
                //    header.pFirstDirectory, typeof(DirectoryBlock));
                DirectoryBlock blockEntry = new DirectoryBlock();
                for (i = 0; i < header.curDirBlkCount; i++)	//在block Entries中查找与dataCount相同block
                {
                    //下一块
                    blockEntry = mapfile.MapStruct<DirectoryBlock>(
                        header.pFirstDirectory + i * (uint)Marshal.SizeOf(typeof(DirectoryBlock)), typeof(DirectoryBlock));

                    if (blockEntry.type == ABParaEntry1 || blockEntry.type == ABParaEntry2)
                        break;
                }
                if (i >= header.curDirBlkCount)  //没找到数据参数
                    throw new Exception("没有找到数据块");

                //获取光谱参数值, 只取：NPT数据数量，FXV最大X值，LXV最小X值，CSF Y值放大系数，MXY最大Y值，MNY最小Y值
                uint datalen = blockEntry.length * 4;	//参数数据块的大小
                curpos = blockEntry.pointer;

                //判断是数据参数Block
                ParameterBlock blockPara = mapfile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));
                if (!IsDataParameterBlock(DWORDToString(blockPara.name)))
                    throw new Exception("没有找到数据块");

                FileParameter dataParameter = new FileParameter();
                while (true)
                {
                    blockPara = mapfile.MapStruct<ParameterBlock>(curpos, typeof(ParameterBlock));
                    string paraname = DWORDToString(blockPara.name);
                    switch (paraname)
                    {
                        case "NPT":
                            dataParameter.dataCount = (uint)blockPara.value.intdata;
                            break;
                        case "FXV":
                            dataParameter.firstX = blockPara.value.doubledata;
                            break;
                        case "LXV":
                            dataParameter.lastX = blockPara.value.doubledata;
                            break;
                        case "CSF":
                            dataParameter.scaleYValue = blockPara.value.doubledata;
                            break;
                        case "MXY":
                            dataParameter.maxYValue = blockPara.value.doubledata;
                            break;
                        case "MNY":
                            dataParameter.minYValue = blockPara.value.doubledata;
                            break;
                    }
                    if (paraname == "END")   //到结束块了
                        break;

                    //8 = name+word+word: sizeof(DWORD) + sizeof(WORD) + sizeof(DWORD) 
                    curpos += 8 + (uint)blockPara.space * 2;
                    if (curpos - blockEntry.pointer >= datalen)	//超过Block大小了
                        break;
                }

                dataParameter.resolution = "4.0";
                dataParameter.specType = "INFRARED SPECTRUM";           //设置为傅立叶红外光谱
                dataParameter.xType = "1/CM";                           //设置为Wavenumber (cm-1)
                if (blockType == DBTAB)
                    dataParameter.dataType = "ABSORBANCE";              //设置为吸收谱
                else if (blockType == OPUSFileFormat.DBTSPEC)
                    dataParameter.dataType = "TRANSMITTANCE";           //设置为透过谱(单通道)
                else
                    dataParameter.dataType = "UNKNOWN";

                dataParameter.resolution = GetResolution(mapfile).ToString("F2");

                if (dataParameter.dataCount == 0 || (dataParameter.firstX < 0.0001) ||
                    (dataParameter.lastX < 0.0001) || (dataParameter.scaleYValue < 0.00001))	//没有找到关键参数
                    throw new Exception("数据错误");

                //放大系数也影响了maxYValue和minYValue
                dataParameter.maxYValue = dataParameter.maxYValue / dataParameter.scaleYValue;
                dataParameter.minYValue = dataParameter.minYValue / dataParameter.scaleYValue;

                return dataParameter;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        //ABData:返回数据, dataSize:数据数量
        public static float[] ReadOpusFileData(byte[] fileData, uint blockType)
        {
            if (fileData == null)
                return null;

            MemoryMappedFile mapfile = null;
            try
            {
                mapfile = new MemoryMappedFile(fileData);
                if (mapfile == null)
                    throw new Exception("打开文件错误");

                uint i;

                OpusHeader header = mapfile.MapStruct<OpusHeader>(0, typeof(OpusHeader));
                if (header.magicNumber != OPUSFILEMARK)	//不是OPUS光谱文件
                    throw new Exception("不是OPUS光谱");

                //修改：2013-11-25, 改为在循环里面MapStruct
                //查找AB数据块位置 跳过Header, 0x18为Header的大小
                //DirectoryBlock blockEntry = mapfile.MapStruct<DirectoryBlock>(
                //    header.pFirstDirectory, typeof(DirectoryBlock));
                DirectoryBlock blockEntry = new DirectoryBlock();
                for (i = 0; i < header.curDirBlkCount; i++)	//在block Entries中查找与dataCount相同block
                {
                    //下一块
                    blockEntry = mapfile.MapStruct<DirectoryBlock>(
                        header.pFirstDirectory + i * (uint)Marshal.SizeOf(typeof(DirectoryBlock)), typeof(DirectoryBlock));
                    if (blockEntry.type == ABDataEntry1 || blockEntry.type == ABDataEntry2)
                        break;

                }
                if (i >= header.curDirBlkCount)
                    throw new Exception("没有找到数据块");

                float[] retData = new float[blockEntry.length];
                if (retData == null)
                    throw new Exception("内存不足");

                return mapfile.ReadFloatData(blockEntry.pointer, blockEntry.length);
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        //在Entry中查找是否包含Identify块, entryOffset:数据块位置
        //return: 代表Identify块的DirectoryBlock
        static DirectoryBlock FindIdentBlock(MappedFile mapFile, out uint entryOffset)
        {
            entryOffset = 0;

            OpusHeader header = GetOpusHeader(mapFile);
            if (header.curDirBlkCount == 0)     //不是OPUS格式
                return default(DirectoryBlock);

            //查找AB数据块位置 跳过Header, 0x18为Header的大小
            uint blkIndex = 0;
            for (blkIndex = 0; blkIndex < header.curDirBlkCount; blkIndex++)	//在block Entries中查找与dataCount相同block
            {
                DirectoryBlock blockEntry = mapFile.MapStruct<DirectoryBlock>(
                    header.pFirstDirectory + blkIndex * (uint)Marshal.SizeOf(typeof(DirectoryBlock)), typeof(DirectoryBlock));
                if (blockEntry.type == IDENTFYYTYPE)	//查找是否有IDENTFYYTYPE
                {
                    entryOffset = header.pFirstDirectory + blkIndex * (uint)Marshal.SizeOf(typeof(DirectoryBlock));
                    return blockEntry;
                }
            }

            return default(DirectoryBlock); ;
        }

        //将结构拷贝到byte[]中, structData:要拷贝的结构, destBuffer:目标数组, bufferOffset:目标数组的其实位置
        //返回：拷贝的结构大小
        static uint CopyStructToByteArray<T>(T structData, byte[] destBuffer, uint bufferOffset)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                //将新增的DirectoryBlock拷贝到写入缓冲区
                int structSize = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(structSize);
                if (ptr == IntPtr.Zero)
                    throw new Exception("内存不足");

                if (bufferOffset + structSize > destBuffer.Length)
                    throw new Exception("内存不足");

                Marshal.StructureToPtr(structData, ptr, true);
                Marshal.Copy(ptr, destBuffer, (int)bufferOffset, structSize);

                return (uint)structSize;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return 0;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);   //释放内存
            }

        }
        //写入分析结果, filename:要写入的文件，writeData：样品检测数据，dataSize：数据大小
        public static bool WriteIdentBlock(string filename, IntPtr writeData, uint dataSize)
        {
            MappedFile fileMap = null;
            try
            {
                fileMap = new MappedFile(filename);
                if (fileMap == null || !fileMap.FileOpened)
                    throw new Exception("打开文件错误");

                //是否是OPUS文件
                OpusHeader header = GetOpusHeader(fileMap);
                if (header.curDirBlkCount == 0)     //不是OPUS格式
                    throw new Exception("不是OPUS光谱");

                //查找是否有IdentifyBlock数据块
                uint identEntryOffset;   //记录Identify DirectoryBlock 的位置
                DirectoryBlock identifyEntry = FindIdentBlock(fileMap, out identEntryOffset);

                byte[] writeBuffer = null;
                uint entrySize = (uint)Marshal.SizeOf(typeof(DirectoryBlock));  //一个IdentifyBlock数据块的大小
                if (identEntryOffset == 0)  //没有找到IdentfiyBlock, 需要新建一个DirectoryBlock，插入到所有DirectoryBlock后面
                {
                    uint writeOffset = 0;   //当前文件写入位置
                    uint readOffset = 0;    //当前文件读取位置

                    //增加一个DirectoryBlock，除DataSize外，多200空余空间
                    writeBuffer = new byte[fileMap.FileSize + entrySize + dataSize + 200];
                    if (writeBuffer == null)
                        throw new Exception("内存不足");

                    //在Header中要增加block的数量
                    header.curDirBlkCount++;
                    CopyStructToByteArray<OpusHeader>(header, writeBuffer, writeOffset);

                    writeOffset = header.pFirstDirectory;
                    readOffset = header.pFirstDirectory;

                    //逐个修改DirectoryBlock所值数据块的位置(都增加entrySize)
                    for (uint i = 0; i < header.curDirBlkCount - 1; i++)    //header.curDirBlkCount已经增加了,这里要-1
                    {
                        DirectoryBlock blockEntry = fileMap.MapStruct<DirectoryBlock>(
                            readOffset, typeof(DirectoryBlock));

                        blockEntry.pointer += entrySize;
                        CopyStructToByteArray<DirectoryBlock>(blockEntry, writeBuffer, writeOffset);

                        readOffset += entrySize;
                        writeOffset += entrySize;
                    }

                    //新建Identify BlockEntry
                    DirectoryBlock newEntry;
                    newEntry.type = IDENTFYYTYPE;         //这是IdentifyBlock数据块
                    newEntry.length = dataSize + 200;     //多200空余空间

                    //实际数据存放在文件最后面, 文件中多增加了一个DirectoryBlock
                    newEntry.pointer = fileMap.FileSize + entrySize;

                    //将新增的DirectoryBlock拷贝到写入缓冲区
                    CopyStructToByteArray<DirectoryBlock>(newEntry, writeBuffer, writeOffset);
                    writeOffset += entrySize;

                    //将文件剩余内容读入缓冲区
                    fileMap.ReadByteData(writeBuffer, writeOffset, readOffset, fileMap.FileSize - readOffset);
                    writeOffset += fileMap.FileSize - readOffset;

                    //将实际数据写入缓冲区
                    Marshal.Copy(writeData, writeBuffer, (int)writeOffset, (int)dataSize);
                }
                else if (identifyEntry.length < dataSize)   //有IdentfiyBlock，但空间不够，修改IdentfiyBlock，并把数据写到文件最后
                {
                    writeBuffer = new byte[fileMap.FileSize + dataSize + 200];    //除DataSize外，多200空余空间
                    if (writeBuffer == null)
                        throw new Exception("内存不足");

                    //将全部OPUS文件读入
                    fileMap.ReadByteData(writeBuffer, 0, 0, fileMap.FileSize);

                    //创建Identify BlockEntry
                    DirectoryBlock newEntry;
                    newEntry.type = IDENTFYYTYPE;         //这是IdentifyBlock数据块
                    newEntry.length = dataSize + 200;     //多200空余空间

                    //实际数据存放在文件最后面
                    newEntry.pointer = fileMap.FileSize;

                    //将新的DirectoryBlock覆盖原来的DirectoryBlock,
                    CopyStructToByteArray<DirectoryBlock>(newEntry, writeBuffer, identEntryOffset);

                    //将实际数据写入缓冲区,fileMap.FileSize:原来文件的最后
                    Marshal.Copy(writeData, writeBuffer, (int)fileMap.FileSize, (int)dataSize);
                }
                else        //空间够了，直接写入, 不需要改变identifyEntry的设置
                {
                    writeBuffer = new byte[fileMap.FileSize];
                    if (writeBuffer == null)
                        throw new Exception("内存不足");

                    //将全部OPUS文件读入
                    fileMap.ReadByteData(writeBuffer, 0, 0, fileMap.FileSize);

                    //将实际数据写入缓冲区,fileMap.FileSize:原来文件的最后
                    Marshal.Copy(writeData, writeBuffer, (int)identifyEntry.pointer, (int)dataSize);
                }

                //强制关闭当前文件
                fileMap.UnloadFile();

                fileMap.CreateAndWriteFile(filename, writeBuffer);

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (fileMap != null)
                    fileMap.UnloadFile();
            }

        }

        //读取分析结果, 如果data == null, 返回所需要的空间
        public static IntPtr ReadIdentBlock(string filename)
        {
            MappedFile fileMap = null;
            IntPtr retPtr = IntPtr.Zero;
            try
            {
                fileMap = new MappedFile(filename);
                if (fileMap == null || !fileMap.FileOpened)
                    throw new Exception("打开文件错误");

                //是否是OPUS文件
                OpusHeader header = GetOpusHeader(fileMap);
                if (header.curDirBlkCount == 0)     //不是OPUS格式
                    throw new Exception("不是OPUS光谱");

                //查找是否有IdentifyBlock数据块
                uint entryOffset;
                DirectoryBlock identifyEntry = FindIdentBlock(fileMap, out entryOffset);
                if (entryOffset == 0)    //没找到数据块
                    throw new Exception("没有找到检测结果");

                //读出identifyEntry所指向的数据
                byte[] readBuffer = fileMap.ReadByteData(identifyEntry.pointer, identifyEntry.length);
                if (readBuffer == null || readBuffer.Length == 0)
                    throw new Exception("没有找到检测结果");

                //拷贝到内存中去
                retPtr = Marshal.AllocHGlobal(readBuffer.Length);
                if (retPtr == IntPtr.Zero)
                    throw new Exception("内存不足");

                Marshal.Copy(readBuffer, 0, retPtr, readBuffer.Length);

                fileMap.UnloadFile();

                return retPtr;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                if (retPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(retPtr);
                    retPtr = IntPtr.Zero;
                }
                return IntPtr.Zero;
            }
            finally
            {
                if (fileMap != null)
                    fileMap.UnloadFile();
            }
        }

        //有IdentifyBlock
        public static bool HasIdentBlock(string filename)
        {
            MappedFile mapFile = null;
            try
            {
                mapFile = new MappedFile(filename);
                if (mapFile.FileOpened == false)
                    throw new Exception("不能打开文件：" + filename);

                //查找是否有IdentifyBlock数据块
                uint entryOffset;
                DirectoryBlock identifyEntry = FindIdentBlock(mapFile, out entryOffset);
                if (entryOffset == 0)    //没找到数据块
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (mapFile != null)
                    mapFile.UnloadFile();
            }

        }

        //删除OPUS文件中的IdentifyBlock
        public static bool DeleteIdentBlock(string filename)
        {
            FileStream fileStream = null;
            BinaryWriter writeStream = null;
            BinaryReader readStream = null;
            int blockSize = Marshal.SizeOf(typeof(DirectoryBlock));

            try
            {
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
                readStream = new BinaryReader(fileStream);
                if (readStream.ReadUInt32() != OPUSFILEMARK)    //magicNumber
                    throw new Exception("Not OPUS file");

                readStream.ReadDouble();    //programNumber
                int offset = readStream.ReadInt32();    //pFirstDirectory
                readStream.ReadInt32();    //maxDirBlkCount
                int blockCount = readStream.ReadInt32();    //curDirBlkCount

                //查找IdentifyBlock的位置
                readStream.BaseStream.Seek(offset, SeekOrigin.Begin);
                int identifyOffset = 0;
                int index;
                for (index = 0; index < blockCount; index++)
                {
                    if (readStream.ReadUInt32() == IDENTFYYTYPE)
                        break;
                    identifyOffset = (int)readStream.BaseStream.Seek(blockSize, SeekOrigin.Current);
                }

                if (index >= blockCount)     //没找到IdentifyBlock
                    throw new Exception("Can't find IdentifyBlock");

                readStream.Close();
                readStream = null;
                fileStream.Close();
                fileStream = null;

                fileStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
                writeStream = new BinaryWriter(fileStream);
                writeStream.Seek(identifyOffset, SeekOrigin.Begin);
                writeStream.Write((int)0);   //改变类型为未定义，相当于删除IdentifyBlock

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (readStream != null)
                    readStream.Close();
                if (writeStream != null)
                    writeStream.Close();
                if (fileStream != null)
                    fileStream.Close();
            }

        }
        //保存为OPUS文件 

        //将字符名字转换为DWORD名字，比如：BMS-> 0x534D4200，高位为0
        static UInt32 StringToDWORD(string name)
        {
            if (name.Length != 3)    //必须是3个字符
                return 0;

            UInt32 retdata = 0;
            retdata = (UInt32)(Convert.ToByte(name[0]));
            retdata = retdata | (UInt32)((Convert.ToByte(name[1]) << 8));
            retdata = retdata | (UInt32)((Convert.ToByte(name[2]) << 16));

            return retdata;
        }

        //填写一个数值参数块
        private static uint WriteNumberParameter(BinaryWriter stream, string name, double doubleValue, uint intValue = 0xFFFFFFFF)
        {
            try
            {
                UInt32 dwordName = StringToDWORD(name);
                stream.Write(dwordName);
                if (name == "END")
                {
                    stream.Write((UInt16)0);
                    stream.Write((UInt16)0);
                    return 8;
                }
                else if (intValue == 0xFFFFFFFF)      //是double值
                {
                    stream.Write((UInt16)1);
                    stream.Write((UInt16)4);
                    stream.Write(doubleValue);
                    return 16;
                }
                else        //int值
                {
                    stream.Write((UInt16)0);
                    stream.Write((UInt16)2);
                    stream.Write(intValue);
                    return 12;
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        //填写一个字符串/枚举参数块
        private static uint WriteStringParameter(BinaryWriter stream, string name, string strValue, string enumValue = null)
        {
            try
            {
                UInt32 dwordName = StringToDWORD(name);
                stream.Write(dwordName);
                if (name == "END")
                {
                    stream.Write((UInt16)0);
                    stream.Write((UInt16)0);
                    return 8;
                }
                else if (enumValue == null)      //是字符串值
                {
                    byte[] buf = Encoding.ASCII.GetBytes(strValue);
                    int writesize = ((buf.Length - 1) / 4 + 1) * 4;     //DWORD对齐
                    stream.Write((UInt16)2);    //类型
                    stream.Write((UInt16)(writesize / 2));
                    stream.Write(buf);
                    for (int i = buf.Length; i < writesize; i++)   //填充剩下的空白
                        stream.Write((byte)0);

                    return 8 + (uint)writesize;
                }
                else        //枚举值4位
                {
                    byte[] buf = Encoding.ASCII.GetBytes(enumValue);
                    int writesize = ((buf.Length - 1) / 4 + 1) * 4;     //DWORD对齐
                    stream.Write((UInt16)3);
                    stream.Write((UInt16)(writesize / 2)); ;            //枚举的长度是2
                    stream.Write(buf);
                    for (int i = buf.Length; i < writesize; i++)        //填充剩下的空白
                        stream.Write((byte)0);

                    return 8 + (uint)writesize;
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        //文件包括:Header, Directory, Parameter, data
        public static bool SaveFile(string filename, float[] writeDatas, FileParameter filePara)
        {
            FileStream filestream = null;
            BinaryWriter savestream = null;
            try
            {
                int blockSize = 12;        //目录大小

                byte[] writeBuffer = new byte[1000];
                filestream = new FileStream(filename, FileMode.Create);
                savestream = new BinaryWriter(filestream);
                int blockOffset = 0x18;

                //先写入文件头
                savestream.Write(OPUSFILEMARK);             //OPUS文件标志
                savestream.Write((double)(9.206220e+005));        //版本号
                savestream.Write((uint)0x18);                 //第一个目录位置
                savestream.Write((uint)0x28);                 //最大目录数量
                savestream.Write((uint)3);                    //当前目录数量(只有3个)

                //写入目录，总共3个目录(DirectoryBlock)

                //AB光谱数据的参数目录
                uint type = DBTAMPL | (DBTDSTAT << SHIFT_DBSPARM) | (DBTAB << SHIFT_DBSDATA);
                uint pointer = 0;         //跳过Header和所有3个目录
                uint length = 0;        //以DWORD表示的大小, 2个int, 5个double, 1个END
                type = 0x101F;
                savestream.Write(type);
                savestream.Write(length);
                savestream.Write(pointer);

                //AB光谱的分辨率目录
                //type = DBTAMPL | (DBTAQPAR << SHIFT_DBSPARM) | (DBTAB << SHIFT_DBSDATA);
                type = 0x30;
                pointer = 0;          //紧接到上一个数据写入
                length = 0;   //以DWORD表示的大小, 1个double, 1个END
                savestream.Write(type);
                savestream.Write(length);
                savestream.Write(pointer);

                //光谱数据目录
                //type = (DBTAMPL<<SHIFT_DBSCPLX) | (DBTAB << SHIFT_DBSDATA);
                type = 0x100F;
                pointer = 0;                    //紧接到上一个数据写入
                length = filePara.dataCount;    //这里写入的是float, 本身就是4byte，所以直接等于dataCount
                savestream.Write(type);
                savestream.Write(length);
                savestream.Write(pointer);

                //写入光谱参数
                int paraPointer = (int)savestream.Seek(0, SeekOrigin.Current);  //AB参数当前位置
                uint paraLen = 0;   //AB参数的长度
                paraLen += WriteNumberParameter(savestream, "DPF", 0, 1);     //这是intPara, 表示数据类型为float
                paraLen += WriteNumberParameter(savestream, "NPT", 0, filePara.dataCount);     //double
                paraLen += WriteNumberParameter(savestream, "FXV", filePara.firstX);           //double
                paraLen += WriteNumberParameter(savestream, "LXV", filePara.lastX);            //double
                //paraLen += WriteNumberParameter(savestream, "CSF", filePara.scaleYValue);      //double
                paraLen += WriteNumberParameter(savestream, "CSF", 1.0);                        //这里放大系数始终设置为1.0
                paraLen += WriteNumberParameter(savestream, "MXY", filePara.maxYValue);        //double
                paraLen += WriteNumberParameter(savestream, "MNY", filePara.minYValue);        //double
                paraLen += WriteStringParameter(savestream, "DXU", null, "WN");    //ENUM      data x-units    WN         wavenumber cm-1
                paraLen += WriteStringParameter(savestream, "DYU", null, "AB");    //ENUM      data y-units    AB        absorbance
                paraLen += WriteStringParameter(savestream, "DAT", DateTime.Now.ToString("yyyy-MM-dd"), null);    //STRING     Date of measurement
                paraLen += WriteStringParameter(savestream, "TIM", DateTime.Now.ToString("hh-mm-ss"), null);    //STRING     Time of measurement
                paraLen += WriteNumberParameter(savestream, "END", 0, 0);

                //写入分辨率参数
                double resolution;
                if (double.TryParse(filePara.resolution, out resolution) == false)  //缺省是4.0
                    resolution = 4.0;
                int resPointer = (int)savestream.Seek(0, SeekOrigin.Current);       //Res参数当前位置
                uint resLen = 0;    //分辨率参数的长度
                resLen += WriteNumberParameter(savestream, "RES", resolution);
                resLen += WriteNumberParameter(savestream, "END", 0, 0);

                //光谱数据
                int dataPointer = (int)savestream.Seek(0, SeekOrigin.Current);       //数据当前位置
                for (int i = 0; i < writeDatas.Length; i++)
                {
                    savestream.Write(writeDatas[i]);
                }

                //修改目录中的Length和Pointer
                //AB光谱的分辨率目录
                savestream.Seek(blockOffset + 4, SeekOrigin.Begin);   //4:跳过类型(4byte)
                savestream.Write(paraLen / 4);                        //length
                savestream.Write(paraPointer);      //pointer

                //分辨率参数
                savestream.Seek(blockOffset + blockSize + 4, SeekOrigin.Begin);
                savestream.Write(resLen / 4);                               //length
                savestream.Write(resPointer);      //pointer

                //数据参数
                savestream.Seek(blockOffset + blockSize * 2 + 4, SeekOrigin.Begin);
                savestream.Write(writeDatas.Length);                               //length
                savestream.Write(dataPointer);      //pointer

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (savestream != null)
                    savestream.Close();
                if (filestream != null)
                    filestream.Close();
            }
        }
    }

    public unsafe static class SPCFile
    {
        #region SPCHeaderDefine
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        struct SPCHeader    //SPC文件头
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

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        struct SPCSubHeader //SPC内容标记
        {
            [FieldOffset(0x00)]
            public byte subflgs;	/* Flags as defined above */
            [FieldOffset(0x01)]
            public byte subexp;	    /* Exponent for sub-file's Y values (80h=>float) Y数值放大系数 2^subexp, 80H表示直接float*/
            [FieldOffset(0x02)]
            public UInt16 subindx;	/* Integer index number of trace subfile (0=first) */
            [FieldOffset(0x04)]
            public float subtime;	/* Floating time for trace (Z axis corrdinate) Z轴(时间轴)的时间值*/
            [FieldOffset(0x08)]
            public float subnext;	/* Floating time for next trace (May be same as beg) 本subFile所代表的采集时间*/
            [FieldOffset(0x0C)]
            public float subnois;	/* Floating peak pick noise level if high byte nonzero 噪声值，Galactic软件用*/
            [FieldOffset(0x10)]
            public UInt32 subnpts;	/* Integer number of subfile points for TXYXYS type 本文件的数据点数量*/
            [FieldOffset(0x14)]
            public UInt32 subscan;	/* Integer number of co-added scans or 0 (for collect) 产生subFile数据所扫描的次数*/
            [FieldOffset(0x18)]
            public float subwlevel;	/* Floating W axis value (if fwplanes non-zero) 4D文件时用*/
            [FieldOffset(0x1C)]
            public fixed byte subresv[4];	/* Reserved area (must be set to zero) */

            //sub Header Size: 0x20
        };


        [StructLayout(LayoutKind.Explicit, Size = 8)]
        struct SPCLogHeader		/* SPCHeader.Flogoff表示本Header的位置 log block header format */
        {
            [FieldOffset(0x00)]
            public UInt32 logsizd;	    /* byte size of disk block. SPCLogHeader+LogData的大小*/
            [FieldOffset(0x04)]
            public UInt32 logsizm; 	/* byte size of memory block. logsizd对齐4096后的数据*/
            [FieldOffset(0x08)]
            public UInt32 logtxto;	/* byte offset to text. Log Data的偏移(从本Header开始)*/
            [FieldOffset(0x0C)]
            public UInt32 logbins;	/* byte size of binary area (immediately after logstc). 紧接本Head后Binary数据的大小（这里保存我们的IdentifyBlock）*/
            [FieldOffset(0x10)]
            public UInt32 logdsks;	    /* byte size of disk area (immediately after logbins) logbins数据之后，logData数据之前的私有数据大小。*/
            [FieldOffset(0x14)]
            public fixed byte logspar[44];	/* reserved (must be zero) */
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

        public enum SPECTYPE //fexper 光谱类型(红外，近红外，拉曼...)
        {
            SPCGEN = 0,	    /* General SPC (could be anything) */
            SPCGC = 1,	    /* 气相色谱图 Gas Chromatogram */
            SPCCGM = 2,	    /* 通用色谱图 General Chromatogram (same as SPCGEN with TCGRAM) */
            SPCHPLC = 3,	/* HPLC色谱图 HPLCHPLC Chromatogram */
            SPCFTIR = 4,	    /* 傅立叶变换中红外，近红外，拉曼FT-IR, FT-NIR, FT-Raman Spectrum or Igram (Can also be used for scanning IR.) */
            SPCNIR = 5,	    /* 近红外 NIR Spectrum (Usually multi-spectral data sets for calibration.) */
            SPCUV = 7,	    /* UV-VIS Spectrum (Can be used for single scanning UV-VIS-NIR.) */
            SPCXRY = 8,	    /* X-射线 X-ray Diffraction Spectrum */
            SPCMS = 9,	    /* 多种光谱 Mass Spectrum  (Can be single, GC-MS, Continuum, Centroid or TOF.) */
            SPCNMR = 10,	/* 核磁 NMR Spectrum or FID */
            SPCRMN = 11,	/* 拉曼谱 Raman Spectrum (Usually Diode Array, CCD, etc. use SPCFTIR for FT-Raman.) */
            SPCFLR = 12,	/* 荧光 Fluorescence Spectrum */
            SPCATM = 13,	/* 原子 Atomic Spectrum */
            SPCDAD = 14	    /* 色谱 Chromatography Diode Array Spectra */
        }

        public enum XAXISTYPE  //fxtype X坐标类型
        {
            XARB = 0,	/* Arbitrary */
            XWAVEN = 1,	/* Wavenumber (cm-1) */
            XUMETR = 2,	/* Micrometers (um) */
            XNMETR = 3,	/* Nanometers (nm) */
            XSECS = 4,	/* Seconds */
            XMINUTS = 5,	/* Minutes */
            XHERTZ = 6,	/* Hertz (Hz) */
            XKHERTZ = 7,	/* Kilohertz (KHz) */
            XMHERTZ = 8,	/* Megahertz (MHz) */
            XMUNITS = 9,	/* Mass (M/z) */
            XPPM = 10,	/* Parts per million (PPM) */
            XDAYS = 11,	/* Days */
            XYEARS = 12,	/* Years */
            XRAMANS = 13	    /* Raman Shift (cm-1) */
        }

        public enum YAXISTYPE  //fytype H坐标类型
        {
            YARB = 0,	/* Arbitrary Intensity */
            YIGRAM = 1,	/* Interferogram */
            YABSRB = 2,	/* Absorbance */
            YKMONK = 3,	/* Kubelka-Monk */
            YCOUNT = 4,	/* Counts */
            YVOLTS = 5,	/* Volts */
            YDEGRS = 6,	/* Degrees */
            YAMPS = 7,	/* Milliamps */
            YMETERS = 8,	/* Millimeters */
            YMVOLTS = 9,	/* Millivolts */
            YLOGDR = 10,	/* Log(1/R) */
            YPERCNT = 11,	/* Percent */
            YINTENS = 12,	/* Intensity */
            YRELINT = 13,	/* Relative Intensity */
            YENERGY = 14,	/* Energy */
            YDECBL = 16,	/* Decibel */
            YDEGRF = 19,	/* Temperature (F) */
            YDEGRC = 20,	/* Temperature (C) */
            YDEGRK = 21,	/* Temperature (K) */
            YINDRF = 22,	/* Index of Refraction [N] */
            YEXTCF = 23,	/* Extinction Coeff. [K] */
            YREAL = 24,	/* Real */
            YIMAG = 25,	/* Imaginary */
            YCMPLX = 26,	/* Complex */
            YTRANS = 128,	/* Transmission (ALL HIGHER MUST HAVE VALLEYS!) */
            YREFLEC = 129,	/* Reflectance */
            YVALLEY = 130,	/* Arbitrary or Single Beam with Valley Peaks */
            YEMISN = 131	/* Emission */
        }

        public enum ZAXISTYPE  //Z坐标类型
        {
            XARB = 0,	/* Arbitrary */
            XWAVEN = 1,	/* Wavenumber (cm-1) */
            XUMETR = 2,	/* Micrometers (um) */
            XNMETR = 3,	/* Nanometers (nm) */
            XSECS = 4,	/* Seconds */
            XMINUTS = 5,	/* Minutes */
            XHERTZ = 6,	/* Hertz (Hz) */
            XKHERTZ = 7,	/* Kilohertz (KHz) */
            XMHERTZ = 8,	/* Megahertz (MHz) */
            XMUNITS = 9,	/* Mass (M/z) */
            XPPM = 10,	/* Parts per million (PPM) */
            XDAYS = 11,	/* Days */
            XYEARS = 12,	/* Years */
            XRAMANS = 13,	/* Raman Shift (cm-1) */
            XEV = 14,	/* eV */
            ZTEXTL = 15,	/* XYZ text labels in fcatxt (old 0x4D version only) */
            XDIODE = 16,	/* Diode Number */
            XCHANL = 17,	/* Channel */
            XDEGRS = 18,	/* Degrees */
            XDEGRF = 19,	/* Temperature (F) */
            XDEGRC = 20,	/* Temperature (C) */
            XDEGRK = 21,	/* Temperature (K) */
            XPOINT = 22,	/* Data Points */
            XMSEC = 23,	/* Milliseconds (mSec) */
            XUSEC = 24,	/* Microseconds (uSec) */
            XNSEC = 25,	/* Nanoseconds (nSec) */
            XGHERTZ = 26,	/* Gigahertz (GHz) */
            XCM = 27,	/* Centimeters (cm) */
            XMETERS = 28,	/* Meters (m) */
            XMMETR = 29,	/* Millimeters (mm) */
            XHOURS = 30,	/* Hours */
            XDBLIGM = 255	/* Double interferogram (no display labels) */
        }

        enum SubFlgs
        {
            SUBCHGD = 1,	    /* Subflgs bit if subfile changed 文件有修改*/
            SUBNOPT = 8,	    /* Subflgs bit if peak table file should not be used 没有峰位表*/
            SUBMODF = 128	/* Subflgs bit if subfile modified by arithmetic 由算法修改的文件*/
        }
        #endregion

        const uint ExtraDataSize = 200;  //新建IdentifyBlock时，另外增加200字节的空间
        public static string ErrorString { get; private set; }
        static SPCFile()
        {

        }

        public static FileParameter spcParameter = null;           //光谱参数
        public static float[] XDatas = null;        //X轴数据(如果oftflgs == TXVALS)
        public static float[] YDatas = null;        //光谱数据
        static string curFileName = null;              //当前打开文件名

        //判断是否是spc文件
        public static bool IsSPCFile(string filename)
        {
            MappedFile mapFile = new MappedFile(filename);

            bool retvalue = IsSPCFile(mapFile);
            mapFile.UnloadFile();

            return retvalue;
        }

        //判断是否是spc文件
        static bool IsSPCFile(MappedFile mapFile)
        {
            if (mapFile == null || !mapFile.FileOpened)
                return false;

            SPCHeader header = mapFile.MapStruct<SPCHeader>(0, typeof(SPCHeader));
            if (header.fversn != 0x4B)  //只识别新格式的SPC文件
                return false;

            //不管TCGRAM, TRANDM, TORDRD这几个参数
            byte temp = (byte)Ftflgs.TCGRAM | (byte)Ftflgs.TRANDM | (byte)Ftflgs.TORDRD;
            header.ftflgs &= (byte)~temp;

            //只识别单一subFile, 4Byte Data的数据
            if ((byte)(header.ftflgs & (byte)Ftflgs.TSPREC) > 0 || (byte)(header.ftflgs & (byte)Ftflgs.TMULTI) > 0)
                return false;

            return true;
        }

        /* Date/Time LSB: min=6b,hour=5b,day=5b,month=4b,year=12b */
        static DateTime DWordToDateTime(uint dtime)
        {
            int min = (int)(dtime & 0x3F);
            int hour = (int)((dtime >> 6) & 0x1F);
            int day = (int)((dtime >> 11) & 0x1F);
            int month = (int)((dtime >> 16) & 0xF);
            int year = (int)((dtime >> 20) & 0xFFF);

            try
            {
                DateTime time = new DateTime(year, month, day, hour, min, 0);
                return time;
            }
            catch (System.Exception)
            {
                return new DateTime(2000, 1, 1, 0, 0, 0);   //返回2000-1-1 00:00:00
            }
        }


        static UInt32 DataTimeToDWord(DateTime time)
        {
            if (time == null)
                return (uint)((2000 << 20) | (1 << 16) | (1 << 11));    //返回2000-1-1 00:00:00
            else
                return (uint)((time.Year << 20) | (time.Month << 16) | (time.Day << 11) | (time.Hour << 6) | time.Minute);
        }

        /// <summary>
        /// 从文件中读取各种格式的数据，并转换到float数组, 只能是4Byte数据
        /// </summary>
        /// <param name="mapFile">打开的文件</param>
        /// <param name="fileOffset">要读取位置</param>
        /// <param name="dataSize">数据数量</param>
        /// <param name="scaleUnit">放大系数</param>
        /// <returns></returns>
        static float[] ReadData(MappedFile mapFile, uint fileOffset, uint dataSize, byte scaleUnit)
        {
            if (scaleUnit == 0x80)   //已经是float, 直接读取即可
                return mapFile.ReadFloatData(fileOffset, dataSize);

            //是DWORD, 需要转换
            int[] intData = mapFile.ReadIntData(fileOffset, dataSize);
            if (intData == null)
                return null;

            //转换公式: FloatY = (2^Exponent)*IntegerY/(2^32)
            float[] floatData = new float[intData.Length];
            float scales = (float)Math.Pow(2, (int)(scaleUnit - 32));
            for (int i = 0; i < intData.Length; i++)
                floatData[i] = intData[i] * scales;

            return floatData;
        }


        /// <summary>
        /// 从内存中读取各种格式的数据，并转换到float数组, 只能是4Byte数据
        /// </summary>
        /// <param name="mapFile">打开的文件</param>
        /// <param name="fileOffset">要读取位置</param>
        /// <param name="dataSize">数据数量</param>
        /// <param name="scaleUnit">放大系数</param>
        /// <returns></returns>
        static float[] ReadData(MemoryMappedFile mapFile, uint fileOffset, uint dataSize, byte scaleUnit)
        {
            if (scaleUnit == 0x80)   //已经是float, 直接读取即可
                return mapFile.ReadFloatData(fileOffset, dataSize);

            //是DWORD, 需要转换
            int[] intData = mapFile.ReadIntData(fileOffset, dataSize);
            if (intData == null)
                return null;

            //转换公式: FloatY = (2^Exponent)*IntegerY/(2^32)
            float[] floatData = new float[intData.Length];
            float scales = (float)Math.Pow(2, (int)(scaleUnit - 32));
            for (int i = 0; i < intData.Length; i++)
                floatData[i] = intData[i] * scales;

            return floatData;
        }

        static string ConvertFixedByteToString(byte* data, int size)
        {
            string retstr = "";
            for (int i = 0; i < size; i++)
            {
                if (data[i] != 0)
                    retstr += Convert.ToChar(data[i]);
            }

            return retstr;
        }

        public static string GetXTypeName(XAXISTYPE type)
        {
            switch (type)
            {
                case XAXISTYPE.XARB:    /* Arbitrary */
                    return "Arbitrary";
                case XAXISTYPE.XWAVEN:  	/* Wavenumber (cm-1) */
                    return "Wavenumber (cm-1)";
                case XAXISTYPE.XUMETR:
                    return "Micrometers (um)";
                case XAXISTYPE.XNMETR:
                    return "Nanometers (nm)";
                case XAXISTYPE.XSECS:
                    return "Seconds";
                case XAXISTYPE.XMINUTS:
                    return "Minutes";
                case XAXISTYPE.XHERTZ:
                    return "Hertz (Hz)";
                case XAXISTYPE.XKHERTZ:
                    return "Kilohertz (KHz)";
                case XAXISTYPE.XMHERTZ:
                    return "Megahertz (MHz)";
                case XAXISTYPE.XMUNITS:
                    return "Mass (M/z)";
                case XAXISTYPE.XPPM:
                    return "Parts per million (PPM)";
                case XAXISTYPE.XDAYS:
                    return "Days";
                case XAXISTYPE.XYEARS:
                    return "Years";
                case XAXISTYPE.XRAMANS:
                    return "Raman Shift (cm-1)";
                default:
                    return "Other Foramt";
            }
        }

        //返回参数
        public static FileParameter ReadParameter(string filename)
        {
            //如果是当前文件并且已经打开了，直接返回
            if (curFileName == filename && spcParameter != null)
                return spcParameter;

            //读入文件
            if (ReadFile(filename) == false)
                return null;

            return spcParameter;
        }

        //读取SPC文件，结果存放在XDatas和YDatas中
        public static bool ReadFile(string filename)
        {
            //如果是当前文件并且已经打开了，直接返回
            if (curFileName == filename && YDatas != null)
                return true;

            MappedFile mapFile = null;
            uint readOffset = 0;    //文件读取位置

            try
            {
                mapFile = new MappedFile(filename);
                if (mapFile == null || !mapFile.FileOpened)
                    throw new Exception("打开文件错误");

                SPCHeader header = mapFile.MapStruct<SPCHeader>(0, typeof(SPCHeader));
                if (header.fversn != 0x4B)  //只识别新格式的SPC文件
                    throw new Exception("不能识别的文件格式");

                //不管TCGRAM, TRANDM, TORDRD这几个参数
                byte temp = (byte)Ftflgs.TCGRAM | (byte)Ftflgs.TRANDM | (byte)Ftflgs.TORDRD;
                header.ftflgs &= (byte)~temp;

                //只识别单一subFile, 4Byte Data的数据
                if ((byte)(header.ftflgs & (byte)Ftflgs.TSPREC) > 0 || (byte)(header.ftflgs & (byte)Ftflgs.TMULTI) > 0)
                    throw new Exception("不能识别的文件格式");

                uint headerSize = (uint)Marshal.SizeOf(header);
                uint subEntrySize = (uint)Marshal.SizeOf(typeof(SPCSubHeader));

                bool needTalabs = (header.ftflgs & (byte)Ftflgs.TALABS) > 0;
                if (needTalabs) //去除TALABS标志，只剩下TXYXYS和TXVALS标记
                    header.ftflgs -= (byte)Ftflgs.TALABS;

                if ((header.ftflgs & (byte)Ftflgs.TXVALS) == 0)  //统一的X轴文件, 这里不会有TXYXYS标记
                {
                    //单文件，平均X值 values [SPCHDR SUBHDR YData LOGSTC (optional) Log Data (optional)]
                    readOffset = headerSize;

                    //创建X数组
                    XDatas = new float[header.fnpts];
                    double xstep = (float)((header.flast - header.ffirst) / (header.fnpts - 1));
                    XDatas[0] = (float)header.ffirst;
                    for (int i = 1; i < header.fnpts; i++)
                        XDatas[i] = XDatas[0] + (float)(i * xstep);

                    SPCSubHeader sub = mapFile.MapStruct<SPCSubHeader>(readOffset, typeof(SPCSubHeader));
                    readOffset += subEntrySize;

                    //读取Y数据
                    YDatas = ReadData(mapFile, readOffset, header.fnpts, header.fexp);
                }
                else if ((header.ftflgs & (byte)Ftflgs.TXYXYS) == 0)  //X轴文件在SPCHeader和SPCSubHeader之间
                {
                    //先读取X数据
                    readOffset = headerSize;
                    XDatas = mapFile.ReadFloatData(readOffset, header.fnpts);
                    readOffset += header.fnpts * 4;     //跳过xData

                    //读SPCSubHeader
                    SPCSubHeader sub = mapFile.MapStruct<SPCSubHeader>(readOffset, typeof(SPCSubHeader));
                    readOffset += subEntrySize;

                    //读取Y数据
                    YDatas = ReadData(mapFile, readOffset, header.fnpts, header.fexp);
                }
                else    //X轴在紧跟SPCSubHeader后面
                {
                    //先读SPCSubHeader
                    SPCSubHeader sub = mapFile.MapStruct<SPCSubHeader>(readOffset, typeof(SPCSubHeader));
                    readOffset += subEntrySize;

                    //再读取X数据
                    readOffset = headerSize;
                    XDatas = mapFile.ReadFloatData(readOffset, header.fnpts);

                    //读取Y数据
                    YDatas = ReadData(mapFile, readOffset, header.fnpts, header.fexp);
                }

                //光谱文件参数
                spcParameter = new FileParameter();

                spcParameter.dataCount = header.fnpts;  //数据点数量
                spcParameter.firstX = XDatas[0];
                spcParameter.lastX = XDatas[XDatas.Length - 1];
                spcParameter.time = DWordToDateTime(header.fdate);
                spcParameter.scaleYValue = 1;

                spcParameter.minYValue = float.MaxValue;
                spcParameter.maxYValue = float.MinValue;
                for (int i = 0; i < YDatas.Length; i++)
                {
                    if (spcParameter.minYValue > YDatas[i]) spcParameter.minYValue = YDatas[i];
                    if (spcParameter.maxYValue < YDatas[i]) spcParameter.maxYValue = YDatas[i];
                }

                spcParameter.xType = "1/CM";           //(XAXISTYPE)header.fxtype;      //X轴的单位
                spcParameter.resolution = ConvertFixedByteToString(header.fcatxt, 30);  //分辨率描述
                if ((SPECTYPE)header.fexper == SPECTYPE.SPCFTIR || (SPECTYPE)header.fexper == SPECTYPE.SPCNIR)
                    spcParameter.specType = "INFRARED SPECTRUM";    //红外光谱
                else if ((SPECTYPE)header.fexper == SPECTYPE.SPCRMN)
                    spcParameter.specType = "RAMAN SPECTRUM";       //拉曼光谱
                else
                    spcParameter.specType = "KNOWN SPECTRUM";       //未知光谱

                if ((YAXISTYPE)header.fytype == YAXISTYPE.YABSRB)
                    spcParameter.dataType = "ABSORBANCE";              //设置为吸收谱
                else if ((YAXISTYPE)header.fytype == YAXISTYPE.YTRANS)
                    spcParameter.dataType = "TRANSMITTANCE";           //设置为透过谱(单通道)
                else
                    spcParameter.dataType = "UNKNOWN";

                curFileName = filename;

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (mapFile != null)
                    mapFile.UnloadFile();
            }
        }

        /// <summary>
        /// 从内存数据中读取SPC光谱，结果存放在XDatas和YDatas中
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public static bool ReadFile(byte[] fileData)
        {
            if (fileData == null)
                return false;

            MemoryMappedFile mapFile = null;
            uint readOffset = 0;    //文件读取位置
            curFileName = null;

            try
            {
                mapFile = new MemoryMappedFile(fileData);
                if (mapFile == null)
                    throw new Exception("打开文件错误");

                SPCHeader header = mapFile.MapStruct<SPCHeader>(0, typeof(SPCHeader));
                if (header.fversn != 0x4B)  //只识别新格式的SPC文件
                    throw new Exception("不能识别的文件格式");

                //不管TCGRAM, TRANDM, TORDRD这几个参数
                byte temp = (byte)Ftflgs.TCGRAM | (byte)Ftflgs.TRANDM | (byte)Ftflgs.TORDRD;
                header.ftflgs &= (byte)~temp;

                //只识别单一subFile, 4Byte Data的数据
                if ((byte)(header.ftflgs & (byte)Ftflgs.TSPREC) > 0 || (byte)(header.ftflgs & (byte)Ftflgs.TMULTI) > 0)
                    throw new Exception("不能识别的文件格式");

                uint headerSize = (uint)Marshal.SizeOf(header);
                uint subEntrySize = (uint)Marshal.SizeOf(typeof(SPCSubHeader));

                bool needTalabs = (header.ftflgs & (byte)Ftflgs.TALABS) > 0;
                if (needTalabs) //去除TALABS标志，只剩下TXYXYS和TXVALS标记
                    header.ftflgs -= (byte)Ftflgs.TALABS;

                if ((header.ftflgs & (byte)Ftflgs.TXVALS) == 0)  //统一的X轴文件, 这里不会有TXYXYS标记
                {
                    //单文件，平均X值 values [SPCHDR SUBHDR YData LOGSTC (optional) Log Data (optional)]
                    readOffset = headerSize;

                    //创建X数组
                    XDatas = new float[header.fnpts];
                    double xstep = (float)((header.flast - header.ffirst) / (header.fnpts - 1));
                    XDatas[0] = (float)header.ffirst;
                    for (int i = 1; i < header.fnpts; i++)
                        XDatas[i] = XDatas[0] + (float)(i * xstep);

                    SPCSubHeader sub = mapFile.MapStruct<SPCSubHeader>(readOffset, typeof(SPCSubHeader));
                    readOffset += subEntrySize;

                    //读取Y数据
                    YDatas = ReadData(mapFile, readOffset, header.fnpts, header.fexp);
                }
                else if ((header.ftflgs & (byte)Ftflgs.TXYXYS) == 0)  //X轴文件在SPCHeader和SPCSubHeader之间
                {
                    //先读取X数据
                    readOffset = headerSize;
                    XDatas = mapFile.ReadFloatData(readOffset, header.fnpts);
                    readOffset += header.fnpts * 4;     //跳过xData

                    //读SPCSubHeader
                    SPCSubHeader sub = mapFile.MapStruct<SPCSubHeader>(readOffset, typeof(SPCSubHeader));
                    readOffset += subEntrySize;

                    //读取Y数据
                    YDatas = ReadData(mapFile, readOffset, header.fnpts, header.fexp);
                }
                else    //X轴在紧跟SPCSubHeader后面
                {
                    //先读SPCSubHeader
                    SPCSubHeader sub = mapFile.MapStruct<SPCSubHeader>(readOffset, typeof(SPCSubHeader));
                    readOffset += subEntrySize;

                    //再读取X数据
                    readOffset = headerSize;
                    XDatas = mapFile.ReadFloatData(readOffset, header.fnpts);

                    //读取Y数据
                    YDatas = ReadData(mapFile, readOffset, header.fnpts, header.fexp);
                }

                //光谱文件参数
                spcParameter = new FileParameter();

                spcParameter.dataCount = header.fnpts;  //数据点数量
                spcParameter.firstX = XDatas[0];
                spcParameter.lastX = XDatas[XDatas.Length - 1];
                spcParameter.time = DWordToDateTime(header.fdate);
                spcParameter.scaleYValue = 1;

                spcParameter.minYValue = YDatas.Min();
                spcParameter.maxYValue = YDatas.Max();

                spcParameter.xType = "1/CM";           //(XAXISTYPE)header.fxtype;      //X轴的单位
                spcParameter.resolution = ConvertFixedByteToString(header.fcatxt, 30);  //分辨率描述
                if ((SPECTYPE)header.fexper == SPECTYPE.SPCFTIR || (SPECTYPE)header.fexper == SPECTYPE.SPCNIR)
                    spcParameter.specType = "INFRARED SPECTRUM";    //红外光谱
                else if ((SPECTYPE)header.fexper == SPECTYPE.SPCRMN)
                    spcParameter.specType = "RAMAN SPECTRUM";       //拉曼光谱
                else
                    spcParameter.specType = "KNOWN SPECTRUM";       //未知光谱

                if ((YAXISTYPE)header.fytype == YAXISTYPE.YABSRB)
                    spcParameter.dataType = "ABSORBANCE";              //设置为吸收谱
                else if ((YAXISTYPE)header.fytype == YAXISTYPE.YTRANS)
                    spcParameter.dataType = "TRANSMITTANCE";           //设置为透过谱(单通道)
                else
                    spcParameter.dataType = "UNKNOWN";

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        //将string编码到以0结尾的byte数组中
        static byte[] StringToByte(string str, int bufferSize)
        {
            byte[] retBuffer = new byte[bufferSize];

            int i = str.Length > bufferSize - 1 ? bufferSize - 1 : str.Length;  //最后1位不填
            Encoding.ASCII.GetBytes(str, 0, i, retBuffer, 0);

            return retBuffer;
        }

        /// <summary>
        /// 将数据保存为SPC格式光谱
        /// </summary>
        /// <param name="filename">保存文件名</param>
        /// <param name="writeDatas">Y轴数据</param>
        /// <param name="specPara">光谱参数</param>
        public static bool SaveFile(string filename, float[] writeDatas, FileParameter specPara)
        {
            FileStream filestream = null;
            BinaryWriter stream = null;
            try
            {
                if (writeDatas == null || filename == null)
                    throw new Exception("传入参数错误");

                byte[] buffer;

                filestream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                stream = new BinaryWriter(filestream);
                //先写入SPCHDR结构
                stream.Write((byte)0);          //ftflgs 标志：0
                stream.Write((byte)0x4b);       //fversn 0x4B=> new LSB

                //fexper 光谱类型
                if (specPara.specType == "INFRARED SPECTRUM")       //红外光谱
                    stream.Write((byte)SPECTYPE.SPCFTIR);
                else if (specPara.specType == "RAMAN SPECTRUM")
                    stream.Write((byte)SPECTYPE.SPCRMN);       //拉曼光谱
                else
                    stream.Write((byte)0);       //未知光谱

                stream.Write((byte)0x80);       //fexp;  80H float
                stream.Write((UInt32)writeDatas.Length);      //fnpts; 数据点数量
                stream.Write((double)specPara.firstX);         //ffirst; 开始X
                stream.Write((double)specPara.lastX);          //flast; 结束X
                stream.Write((UInt32)1);        //fnsub; SubFile: 固定1

                //fxtype X坐标类型
                if (specPara.xType == "1/CM")
                    stream.Write((byte)XAXISTYPE.XWAVEN);   //1: Wavenumber (cm-1)
                else
                    stream.Write((byte)XAXISTYPE.XARB);     //0: 任意值

                //fytype Y值类型
                if (specPara.dataType == "ABSORBANCE")
                    stream.Write((byte)YAXISTYPE.YABSRB);               //设置为吸收谱
                else if (specPara.dataType == "TRANSMITTANCE")
                    stream.Write((byte)YAXISTYPE.YTRANS);               //设置为透过谱(单通道)
                else
                    stream.Write((byte)YAXISTYPE.YARB);                 //未知光谱

                stream.Write((byte)0);          //fztype; Z值类型
                stream.Write((byte)0);          //fpost;  ? 暂时为0
                stream.Write((UInt32)DataTimeToDWord(specPara.time));        //fdate;  时间日期 min=6b,hour=5b,day=5b,month=4b,year=12b，不设置

                //分辨率
                buffer = StringToByte(specPara.resolution, 9);  //fres:分辨率描述char[9]
                stream.Write(buffer);

                buffer = StringToByte("FASTIDNT", 9);           //fsource: 测量仪器描述 char[9]
                stream.Write(buffer);

                stream.Write((UInt16)0);        //fpeakpt;  干涉图峰位

                for (int i = 0; i < 8; i++)     //fspare; ? float[8]
                    stream.Write((float)0.0f);

                buffer = new byte[130];           //fcmnt: char[130]
                stream.Write(buffer);

                buffer = new byte[30];           //fcatxt: char[30] X,y,z坐标轴文字(TABLABS)
                stream.Write(buffer);

                stream.Write((UInt32)0);          //flogoff;; log block的文件指针
                stream.Write((UInt32)0);          //fmods; 光谱所做的处理 
                stream.Write((byte)0);          //fprocs; ?
                stream.Write((byte)0);          //flevel; ?
                stream.Write((UInt16)0);        //fsampin;Z值类型
                stream.Write((float)1);         //ffactor; 放大系数, 固定1

                buffer = new byte[48];          //fmethod; ? char[48]
                stream.Write(buffer);

                stream.Write((float)0);         //fzinc; ?
                stream.Write((UInt32)0);        //fwplanes ?
                stream.Write((float)0);          //fwinc; ?
                stream.Write((byte)0);          //fwtype;; ?

                buffer = new byte[187];         //freserv; 保留 char[187]
                stream.Write(buffer);

                //写入SUBHDR;
                stream.Write((byte)0);          //subflgs; ?
                stream.Write((byte)0x80);       //subexp; 80h=>float
                stream.Write((UInt16)0);        //subindx; Integer index number of trace subfile (0=first)
                stream.Write((float)0);         //subtime;
                stream.Write((float)0);         //subnext;
                stream.Write((float)0);         //subnois;
                stream.Write((UInt32)writeDatas.Length);        //subnpts;  //数据点数量
                stream.Write((UInt32)0);        //subscan;
                stream.Write((float)0);         //subwlevel;;

                buffer = new byte[4];           //subresv; 保留 char[4]
                stream.Write(buffer);

                //写入数据
                for (int i = 0; i < writeDatas.Length; i++)
                    stream.Write((float)writeDatas[i]);

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (filestream != null)
                    filestream.Close();
            }
        }

        /// <summary>
        /// 将float数据保存为SPC格式光谱
        /// </summary>
        /// <param name="filename">保存文件名</param>
        /// <param name="xDatas">X轴数据</param>
        /// <param name="yDatas">Y轴数据</param>
        /// <returns></returns>
        public static bool SaveFile(string filename, float[] xDatas, float[] yDatas, int resolution=4, string dataType=null, string specType=null, string xType=null)
        {
            if (xDatas == null || yDatas == null || xDatas.Length != yDatas.Length || xDatas.Length == 0)
            {
                ErrorString = "传入参数错误";
                return false;
            }

            FileParameter para = new FileParameter();
            para.firstX = xDatas[0];
            para.lastX = xDatas[xDatas.Length - 1];
            para.maxYValue = yDatas.Max();
            para.minYValue = yDatas.Min();
            para.dataCount = (uint)xDatas.Length;
            para.time = DateTime.Now;
            para.scaleYValue = 1;
            para.dataType = dataType == null ? "ABSORBANCE" : dataType;
            para.xType = xType == null ? "1/CM" : xType;
            para.specType = specType == null ? "INFRARED SPECTRUM" : "RAMAN SHIFT";
            para.resolution = resolution.ToString(); ;

            return SaveFile(filename, yDatas, para);
        }

        /// <summary>
        /// 将double数据保存为SPC格式光谱
        /// </summary>
        /// <param name="filename">保存文件名</param>
        /// <param name="xDatas">X轴数据</param>
        /// <param name="yDatas">Y轴数据</param>
        /// <returns></returns>
        public static bool SaveFile(string filename, double[] xDatas, double[] yDatas, int resolution = 4, string dataType = null, string specType = null, string xType = null)
        {
            if (xDatas == null || yDatas == null || xDatas.Length != yDatas.Length || xDatas.Length == 0)
            {
                ErrorString = "传入参数错误";
                return false;
            }

            FileParameter para = FileParameter.CreateFromData(xDatas, yDatas, resolution, dataType, specType, xType);
            
            float[] tempYDatas = new float[yDatas.Length];
            for (int i = 0; i < yDatas.Length; i++)
                tempYDatas[i] = (float)yDatas[i];

            return SaveFile(filename, tempYDatas, para);
        }

        //将结构拷贝到byte[]中, structData:要拷贝的结构, destBuffer:目标数组, bufferOffset:目标数组的其实位置
        //返回：拷贝的结构大小
        static uint CopyStructToByteArray<T>(T structData, byte[] destBuffer, uint bufferOffset)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                //将新增的DirectoryBlock拷贝到写入缓冲区
                int structSize = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(structSize);
                if (ptr == IntPtr.Zero)
                    throw new Exception("内存不足");

                if (bufferOffset + structSize > destBuffer.Length)
                    throw new Exception("内存不足");

                Marshal.StructureToPtr(structData, ptr, true);
                Marshal.Copy(ptr, destBuffer, (int)bufferOffset, structSize);

                return (uint)structSize;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return 0;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);   //释放内存
            }

        }

        /// <summary>
        /// 是否包含分析结果
        /// </summary>
        /// <param name="filename">光谱文件名</param>
        /// <param name="blockID">结果类型标志</param>
        /// <returns>包含结果的指针</returns>
        public static bool HasIdentifyBlock(string filename, uint blockID = SpecFileFormat.RFDAMark)
        {
            MappedFile mapFile = null;

            try
            {
                mapFile = new MappedFile(filename);
                if (mapFile == null || !mapFile.FileOpened)
                    throw new Exception("打开文件错误");

                SPCHeader header = mapFile.MapStruct<SPCHeader>(0, typeof(SPCHeader));
                if (header.flogoff == 0)  //只识别新格式的SPC文件
                    throw new Exception("没有找到检测结果");

                //读取SPCLogHeader
                SPCLogHeader logHeader = mapFile.MapStruct<SPCLogHeader>(header.flogoff, typeof(SPCLogHeader));
                if (logHeader.logbins < ExtraDataSize)   //IdentifyBlock数据肯定要大于ExtraDataSize
                    throw new Exception("没有找到检测结果");

                uint logHeaderSize = (uint)Marshal.SizeOf(typeof(SPCLogHeader));    //SPCLogHeader大小

                //检查是不是SampleResult数据
                //紧接SPCLogHeader后读取标志, 看看是不是 blockID(二进制码)
                int[] tempint = mapFile.ReadIntData(header.flogoff + logHeaderSize, 1);
                if (tempint.Length != 1 || (uint)tempint[0] != blockID)
                    throw new Exception("没有找到检测结果");

                return true;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (mapFile != null)
                    mapFile.UnloadFile();
            }
        }
        /// <summary>
        /// 将检测结果写入到SPCLogHeader与LogData之间
        /// </summary>
        /// <param name="filename">要写入的文件名</param>
        /// <param name="writeBlockData">要写入的数据</param>
        /// <param name="writeBlockSize">写入数据的大小</param>
        /// <returns>True:正常写入，False:写入错误</returns>
        public static bool WriteIdentifyBlock(string filename, IntPtr writeBlockData, uint writeBlockSize)
        {
            MappedFile mapFile = null;
            try
            {
                mapFile = new MappedFile(filename);
                if (mapFile == null || !mapFile.FileOpened)
                    throw new Exception("打开文件错误");

                SPCHeader header = mapFile.MapStruct<SPCHeader>(0, typeof(SPCHeader));
                if (header.fversn != 0x4B)  //只识别新格式的SPC文件
                    throw new Exception("不能识别的文件格式");

                uint headerSize = (uint)Marshal.SizeOf(typeof(SPCHeader));          //SPCHeader大小
                uint logHeaderSize = (uint)Marshal.SizeOf(typeof(SPCLogHeader));    //SPCLogHeader大小

                uint writeOffset = 0;

                //增加一个logData,在logData和logHeader中间保存Result数据
                byte[] logbuffer = Encoding.ASCII.GetBytes("CREATE=MTG-TECH(HAI)\0");

                //先按照新建IdentifyBlock来分配内存, 增加一个SPCLogHeader和ExtraDataSize
                uint bufSize = mapFile.FileSize;
                bufSize += logHeaderSize;               //SPCLogHeader大小
                bufSize += writeBlockSize;                   //待写入的ResultData大小
                bufSize += ExtraDataSize;               //富余空间
                bufSize += (uint)logbuffer.Length + 1;      //如果没有logFile，创建一个LogFile的大小, 最后要保留一位为NULL
                byte[] writeBuffer = new byte[bufSize]; //所有内容写入的Buffer
                if (writeBuffer == null)
                    throw new Exception("内存不足");

                if (header.flogoff == 0)     //没有logFile块，也就没有IdentifyBlock
                {
                    //增加一个logFile块，放置到文件最后, 修改并将Offset写入文件头
                    header.flogoff = mapFile.FileSize;  //设置IdentifyBlock的offset
                    CopyStructToByteArray<SPCHeader>(header, writeBuffer, 0);

                    //读取剩余文件
                    mapFile.ReadByteData(writeBuffer, headerSize, headerSize, mapFile.FileSize - headerSize);
                    writeOffset = mapFile.FileSize;

                    //创建并写入SPCLogHeader
                    SPCLogHeader logHeader;
                    logHeader.logsizd = logHeaderSize + (uint)logbuffer.Length;             //SPCLogHeader+LogData的大小
                    logHeader.logsizm = (((logHeader.logsizd - 1) / 4096) + 1) * 4096;      //logsizd对齐4096
                    logHeader.logtxto = logHeaderSize + writeBlockSize + ExtraDataSize;     //LogData的偏移(从SPCLogHeader开始,包括writeData和富余空间)
                    logHeader.logbins = writeBlockSize + ExtraDataSize;                     //IdentifyBlock的大小
                    logHeader.logdsks = 0;                                                  //其它数据大小，设置为0

                    //先写入logFile的Header
                    CopyStructToByteArray<SPCLogHeader>(logHeader, writeBuffer, writeOffset);
                    writeOffset += logHeaderSize;

                    //再写入IdentifyBlock
                    Marshal.Copy(writeBlockData, writeBuffer, (int)writeOffset, (int)writeBlockSize);
                    writeOffset += writeBlockSize + ExtraDataSize;

                    //最后写入LogText
                    Array.Copy(logbuffer, 0, writeBuffer, writeOffset, logbuffer.Length);

                    writeOffset += (uint)logbuffer.Length + 1;    //+1：留一个结尾的NULL
                }
                else
                {
                    //读取SPCLogHeader
                    SPCLogHeader logHeader = mapFile.MapStruct<SPCLogHeader>(header.flogoff, typeof(SPCLogHeader));
                    if (logHeader.logbins >= writeBlockSize)    //现有空间能够写得下
                    {
                        //读入所有文件,不需要修改SPCHeader和SPCLogHeader(BlockDataSize没有在
                        mapFile.ReadByteData(writeBuffer, 0, 0, mapFile.FileSize);

                        //紧接SPCLogHeader后面写入IdentifyBlock
                        Marshal.Copy(writeBlockData, writeBuffer, (int)(header.flogoff + logHeaderSize), (int)writeBlockSize);

                        writeOffset += mapFile.FileSize;    //文件的大小没有变化
                    }
                    else    //现有空间不够
                    {
                        //读入SPCLogHeader前的所有内容
                        mapFile.ReadByteData(writeBuffer, 0, 0, header.flogoff);
                        writeOffset = header.flogoff;
                        uint logDataOffset = header.flogoff + logHeader.logtxto;  //LogData的位置

                        //修改并写入SPCLogHeader
                        logHeader.logbins = writeBlockSize + ExtraDataSize;   //设置IdentifyBlock的大小
                        logHeader.logdsks = 0;  //删除另一个私有数据
                        logHeader.logtxto = logHeaderSize + writeBlockSize + ExtraDataSize;  //修改logData的偏移位置

                        CopyStructToByteArray<SPCLogHeader>(logHeader, writeBuffer, writeOffset);
                        writeOffset += logHeaderSize;

                        //写入IdentifyBlock
                        Marshal.Copy(writeBlockData, writeBuffer, (int)writeOffset, (int)writeBlockSize);
                        writeOffset += writeBlockSize + ExtraDataSize;

                        //读入所有的LogData
                        mapFile.ReadByteData(writeBuffer, writeOffset, logDataOffset, logHeader.logsizd - logHeaderSize);
                        writeOffset += logHeader.logsizd - logHeaderSize;
                    }
                }

                //强制关闭当前文件
                mapFile.UnloadFile();

                return mapFile.CreateAndWriteFile(filename, writeBuffer, writeOffset);
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
            finally
            {
                if (mapFile != null)
                    mapFile.UnloadFile();
            }
        }

        /// <summary>
        /// 读取检测结果
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="blockID">结果类型标志</param>
        /// <returns>包含结果的指针</returns>
        public static IntPtr ReadIdentifyBlock(string filename, uint blockID = SpecFileFormat.RFDAMark)
        {
            MappedFile mapFile = null;

            try
            {
                mapFile = new MappedFile(filename);
                if (mapFile == null || !mapFile.FileOpened)
                    throw new Exception("打开文件错误");

                SPCHeader header = mapFile.MapStruct<SPCHeader>(0, typeof(SPCHeader));
                if (header.flogoff == 0)  //只识别新格式的SPC文件
                    throw new Exception("没有找到检测结果");

                //读取SPCLogHeader
                SPCLogHeader logHeader = mapFile.MapStruct<SPCLogHeader>(header.flogoff, typeof(SPCLogHeader));
                if (logHeader.logbins < ExtraDataSize)   //IdentifyBlock数据肯定要大于ExtraDataSize
                    throw new Exception("没有找到检测结果");

                uint logHeaderSize = (uint)Marshal.SizeOf(typeof(SPCLogHeader));    //SPCLogHeader大小

                //检查是不是SampleResult数据
                //紧接SPCLogHeader后读取标志, 看看是不是 blockID(二进制码)
                int[] tempint = mapFile.ReadIntData(header.flogoff + logHeaderSize, 1);
                if (tempint.Length != 1 || (uint)tempint[0] != blockID)
                    throw new Exception("没有找到检测结果");

                //读出identifyEntry所指向的数据, 偏移量为：header.flogoff + logHeaderSize，大小为：logHeader.logbins
                byte[] readBuffer = mapFile.ReadByteData(header.flogoff + logHeaderSize, logHeader.logbins);
                if (readBuffer == null || readBuffer.Length == 0)
                    throw new Exception("没有找到检测结果");

                //拷贝到内存中去
                IntPtr retPtr = Marshal.AllocHGlobal(readBuffer.Length);
                if (retPtr == IntPtr.Zero)
                    throw new Exception("内存不足");

                Marshal.Copy(readBuffer, 0, retPtr, readBuffer.Length);

                return retPtr;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                return IntPtr.Zero;
            }
            finally
            {
                if (mapFile != null)
                    mapFile.UnloadFile();
            }
        }
    }

    public static class JCAMP_DXFile
    {
        public static string ErrorString { get; set; }
        //格式：##RESOLUTION=16
        static void FillParameter(FileParameter Para, string curLine)
        {
            string[] property = curLine.Split('=');
            if (property.Length < 2 || property[1] == "")
                return;

            property[0].ToUpper().Trim();
            property[1].Trim();
            switch (property[0])
            {
                case "##FIRSTX":
                    double.TryParse(property[1], out Para.firstX);
                    break;
                case "##LASTX":
                    double.TryParse(property[1], out Para.lastX);
                    break;
                case "##NPOINTS":
                    uint.TryParse(property[1], out Para.dataCount);
                    break;
                case "##MAXY":
                    double.TryParse(property[1], out Para.maxYValue);
                    break;
                case "##MINY":
                    double.TryParse(property[1], out Para.minYValue);
                    break;
                case "##YFACTOR":   //数据放大系数
                    double.TryParse(property[1], out Para.scaleYValue);
                    break;
                case "##RESOLUTION":
                    Para.resolution = property[1];
                    break;
                case "##DATE":  //标准格式是YY/MM/DD, 但OPUS存的是DD/MM/YYYY
                    int year, month, day;
                    string[] date = property[1].Split('/');
                    if (date.Length != 3)    //格式有错误
                        return;

                    if (date[2].Length > 2)    //DD/MM/YYYY格式
                    {
                        int.TryParse(date[0], out day);
                        int.TryParse(date[1], out month);
                        int.TryParse(date[2], out year);
                    }
                    else    //标准格式是YY/MM/DD
                    {
                        int.TryParse(date[0], out year);
                        int.TryParse(date[1], out month);
                        int.TryParse(date[2], out day);
                    }
                    year = year < 50 ? year + 2000 : year + 1900;   //变成20??年或者19??年
                    DateTime.TryParse(property[1], out Para.time);
                    try
                    {
                        Para.time = new DateTime(year, month, day);
                    }
                    catch (System.Exception)
                    {
                        return;
                    }
                    break;
                case "##TIME":
                    if (Para.time.Year < 1900)  //没设置有效的日期
                        return;

                    int hour, minute, second;
                    string[] time = property[1].Split('/');
                    if (time.Length != 3)    //格式有错误
                        return;

                    int.TryParse(time[0], out hour);
                    int.TryParse(time[1], out minute);
                    int.TryParse(time[2], out second);

                    try
                    {
                        Para.time = new DateTime(Para.time.Year, Para.time.Month, Para.time.Day, hour, minute, second);
                    }
                    catch (System.Exception)
                    {
                        return;
                    }
                    break;
                case "##DATA TYPE":     //光谱的类型(红外，拉曼...)
                    Para.specType = property[1];
                    break;
                case "##XUNITS":        //X轴坐标类型(1/cm...)
                    Para.xType = property[1];
                    break;
                case "##YUNITS":        //谱图类型(吸收谱，干涉谱...)
                    Para.dataType = property[1];
                    break;
            }
        }

        public static FileParameter ReadFile(string filename, out float[] XDatas, out float[] YDatas)
        {
            StreamReader dxFile = null;
            try
            {
                dxFile = new StreamReader(filename, Encoding.GetEncoding("GB2312"));
                if (dxFile == null)
                    throw new Exception("打开文件错误");

                FileParameter _parameter = new FileParameter();

                //查找数据位置
                string curLine = dxFile.ReadLine();
                while (curLine != null)
                {
                    if (curLine.IndexOf("##XYDATA") == 0)   //下面是数据了
                    {
                        if (_parameter.dataCount == 0 || _parameter.scaleYValue == 0 ||
                            (_parameter.firstX == 0 && _parameter.lastX == 0))
                            throw new Exception("文件格式错误");
                        else
                            break;
                    }

                    if (curLine.Length > 2 && curLine[0] == '#' && curLine[1] == '#')
                        FillParameter(_parameter, curLine);

                    curLine = dxFile.ReadLine();
                }
                if (curLine == null)
                    throw new Exception("文件格式错误");

                int datatype = -1;  //数据的格式
                curLine = curLine.Substring(curLine.IndexOf('=') + 1);
                curLine.Trim().ToUpper();
                switch (curLine)
                {
                    case "(X++(Y..Y))":
                        datatype = 0;
                        break;
                    case "(XY..XY)":
                        datatype = 1;
                        break;
                    default:
                        throw new Exception("文件格式错误");
                }

                curLine = dxFile.ReadLine();
                char splitChar;
                if (curLine.IndexOf('+') > 0)
                    splitChar = '+';
                else if (curLine.IndexOf(' ') > 0)
                    splitChar = ' ';
                else
                    throw new Exception("文件格式错误");

                XDatas = new float[_parameter.dataCount];
                YDatas = new float[_parameter.dataCount];
                float stepXValue = (float)((_parameter.lastX - _parameter.firstX) / (_parameter.dataCount - 1));
                int dataIndex = 0;
                while (curLine != null)
                {
                    if (curLine.Length < 1 || curLine[0] == '#')
                        break;

                    string[] datas = curLine.Split(splitChar);
                    switch (datatype)
                    {
                        case 0:     //(X++(Y..Y)) 一个X后面跟多个Y
                            if (float.TryParse(datas[0], out XDatas[dataIndex]) == false)
                                throw new Exception("文件格式错误");
                            for (int i = 1; i < datas.Length; i++)
                            {
                                if (dataIndex >= YDatas.Length)
                                    break;

                                if (float.TryParse(datas[i], out YDatas[dataIndex]) == false)
                                    throw new Exception("文件格式错误");

                                dataIndex++;
                            }
                            break;
                        case 1:     //(XY..XY) 一个X后面跟一个Y,数据肯定是双数
                            if ((datas.Length % 2) == 1)
                                throw new Exception("文件格式错误");

                            for (int i = 1; i < datas.Length / 2; i++)
                            {
                                if (dataIndex >= YDatas.Length)
                                    break;

                                if (float.TryParse(datas[i * 2], out XDatas[dataIndex]) == false)
                                    throw new Exception("文件格式错误");
                                if (float.TryParse(datas[i * 2 + 1], out YDatas[dataIndex]) == false)
                                    throw new Exception("文件格式错误");

                                dataIndex++;
                            }
                            break;
                    }
                    curLine = dxFile.ReadLine();    //读下一行
                }

                //转换YData和XData     这里怎么处理每行的X值???
                for (int i = 0; i < YDatas.Length; i++)
                {
                    YDatas[i] *= (float)_parameter.scaleYValue;
                    XDatas[i] = (float)_parameter.firstX + i * stepXValue;
                }

                return _parameter;
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                XDatas = null;
                YDatas = null;

                return null;
            }
            finally
            {
                if (dxFile != null)
                    dxFile.Close();
            }
        }

        //将文件保存为JCAMP_DX格式
        public static bool SaveFile(string jcampFile, FileParameter _parameter, float[] XDatas, float[] YDatas)
        {
            StreamWriter stream = null;
            try
            {
                if (_parameter == null || YDatas == null)
                    return false;

                stream = new StreamWriter(jcampFile, false, Encoding.GetEncoding("gb2312"));

                //文件头
                DateTime filetime = File.GetCreationTime(jcampFile);
                stream.WriteLine("##TITLE=" + Path.GetFileName(jcampFile));
                stream.WriteLine("##JCAMP-DX=4.24");
                stream.WriteLine("##DATA TYPE=" + _parameter.specType);
                stream.WriteLine("##DATE=" + filetime.ToString("yy") + "/" + filetime.ToString("MM") + "/" + filetime.ToString("dd"));
                stream.WriteLine("##TIME=" + filetime.ToString("HH") + "/" + filetime.ToString("mm") + "/" + filetime.ToString("ss"));
                stream.WriteLine("##SAMPLING PROCEDURE=Instrument type and / or accessory");
                stream.WriteLine("##ORIGIN=Administrator");
                stream.WriteLine("##XUNITS=" + _parameter.xType);
                stream.WriteLine("##YUNITS=" + _parameter.dataType);
                stream.WriteLine("##RESOLUTION=" + _parameter.resolution);
                stream.WriteLine("##FIRSTX=" + _parameter.firstX);
                stream.WriteLine("##LASTX=" + _parameter.lastX);
                stream.WriteLine("##DELTAX=" + (_parameter.lastX - _parameter.firstX) / (_parameter.dataCount - 1));
                stream.WriteLine("##MAXY=" + _parameter.maxYValue);
                stream.WriteLine("##MINY=" + _parameter.minYValue);
                stream.WriteLine("##XFACTOR=1");
                stream.WriteLine("##YFACTOR=1e-6");   //固定10的-6次方, 例子中是：1.7720042e-011
                stream.WriteLine("##NPOINTS=" + _parameter.dataCount);
                stream.WriteLine("##FIRSTY=" + YDatas[0]);
                stream.WriteLine("##XYDATA=(X++(Y..Y))");

                //实际内容, 
                int count = 10;
                double yfactor = 1e6;  //10的6次方
                double stepx = (_parameter.lastX - _parameter.firstX) / (_parameter.dataCount - 1);
                for (int line = 0; line < ((YDatas.Length - 1) / count) + 1; line++)  //10个一行
                {
                    //xValue
                    string writestr = ((int)(_parameter.firstX + line * count * stepx)).ToString();
                    for (int col = 0; col < count; col++)
                    {
                        if (line * count + col >= YDatas.Length)
                            break;

                        writestr += "+" + ((int)(YDatas[line * count + col] * yfactor)).ToString();
                    }
                    stream.WriteLine(writestr);
                }
                stream.WriteLine("##END=");
                stream.Close();
            }
            catch (System.Exception ex)
            {
                if (stream != null)
                    stream.Close();

                ErrorString = ex.Message;
                return false;
            }

            return true;
        }

        public static bool IsDXFile(string filename)
        {
            try
            {
                StreamReader reader = new StreamReader(filename, Encoding.GetEncoding("GB2312"));
                string title = reader.ReadLine();
                if (title.IndexOf("##TITLE=") == 0)
                    return true;

                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }

    public static class DPTFormat
    {
        public static string ErrorString { get; set; }

        public static bool ReadFile(string filename)
        {
            return false;
        }

        public static bool SaveFile(string filename, float[] writeDatas, FileParameter filePara)
        {
            StreamWriter stream = null;
            try
            {
                stream = new StreamWriter(filename, false, Encoding.GetEncoding("gb2312"));
                if (writeDatas == null)
                {
                    throw new Exception("原始光谱文件打开错误。");
                }
                double stepx = (filePara.lastX - filePara.firstX) / (filePara.dataCount - 1);
                for (int i = 0; i < writeDatas.Length; i++)
                {
                    stream.WriteLine((filePara.firstX + stepx * i).ToString("F2") + "," + writeDatas[i].ToString("F7"));
                }
                stream.Close();
            }
            catch (System.Exception ex)
            {
                ErrorString = ex.Message;
                if (stream != null)
                    stream.Close();

                return false;
            } 
            
            return true;
        }

        public static IntPtr ReadIdentBlock(string filename)
        {
            return IntPtr.Zero;
        }
        public static bool WriteIdentBlock(string filename, IntPtr writeData, uint dataSize)
        {
            return false;
        }
    }

    public class SpecFileFormatDouble:SpecFileFormat
    {
        public new double[] XDatas;
        public new double[] YDatas;

        public new bool ReadFile(string filename)
        {
            if (base.ReadFile(filename) == false || base.XDatas==null || base.YDatas==null)
                return false;

            XDatas = new double[base.XDatas.Length];
            for (int i = 0; i < base.XDatas.Length; i++)
                XDatas[i] = base.XDatas[i];

            YDatas = new double[base.YDatas.Length];
            for (int i = 0; i < base.YDatas.Length; i++)
                YDatas[i] = base.YDatas[i];

            return true;
        }

        public new bool ReadFile(byte[] fileData)
        {
            if (base.ReadFile(fileData) == false || base.XDatas == null || base.YDatas == null)
                return false;

            XDatas = new double[base.XDatas.Length];
            for (int i = 0; i < base.XDatas.Length; i++)
                XDatas[i] = base.XDatas[i];

            YDatas = new double[base.YDatas.Length];
            for (int i = 0; i < base.YDatas.Length; i++)
                YDatas[i] = base.YDatas[i];

            return true;
        }

        public new bool SaveFile(string filename, EnumFileType fileType)
        {
            if (XDatas == null || YDatas == null)
                return false;

            base.XDatas = new float[XDatas.Length];
            for (int i = 0; i < XDatas.Length; i++)
                base.XDatas[i] = (float)XDatas[i];

            base.YDatas = new float[YDatas.Length];
            for (int i = 0; i < YDatas.Length; i++)
                base.YDatas[i] = (float)YDatas[i];

            return base.SaveFile(filename, fileType);
        }

        /// <summary>
        /// 创建SpecFileFormat
        /// </summary>
        /// <param name="xDatas"></param>
        /// <param name="yDatas"></param>
        /// <param name="resolution">分辨率</param>
        /// <param name="dataType">数据类型(ABSORB, TRANS...)</param>
        /// <param name="specType">光谱类型(NIR, IR, RAMAN...)</param>
        /// <param name="xType">X轴类型(1/CM, RAMAN SHIFT...)</param>
        public static SpecFileFormatDouble CreateFromData(double[] xDatas, double[] yDatas, int resolution = 4, string dataType = null, string specType = null, string xType = null)
        {
            if (xDatas == null || yDatas == null || xDatas.Length != yDatas.Length || xDatas.Length == 0)
                return null;

            SpecFileFormatDouble retData = new SpecFileFormatDouble();
            retData.XDatas = (double[])xDatas.Clone();
            retData.YDatas = (double[])yDatas.Clone();

            retData.Parameter = FileParameter.CreateFromData(xDatas, yDatas, resolution, dataType, specType, xType);

            return retData;
        }
    }

}
