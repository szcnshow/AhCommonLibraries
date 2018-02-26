using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Ai.Hong.FileFormat
{
    /// <summary>
    /// 通用操作函数
    /// </summary>
    public class CommonMethod
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

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

        /// <summary>
        /// 合并光谱列表中的Y轴数据
        /// </summary>
        /// <param name="fileDatas">Y轴数据列表</param>
        /// <returns>合并Y轴数据</returns>
        public static double[] CombineSpectrumDatas(IList<double[]> fileDatas)
        {
            if (fileDatas == null || fileDatas.Count == 0)
                return null;

            //检测是不是所有光谱的Y轴数据量都相同
            foreach (var item in fileDatas)
            {
                if (item == null || item.Length == 0 || item.Length != fileDatas[0].Length)
                {
                    ErrorString = "Invalid file format";
                    return null;
                }
            }

            double[] retdata = new double[fileDatas[0].Length * fileDatas.Count];
            int index = 0;
            foreach (var data in fileDatas)
            {
                Array.Copy(data, 0, retdata, index, data.Length);
                index += data.Length;
            }
            return retdata;
        }

        /// <summary>
        /// 从内存中拷贝float数据，并且释放内存
        /// </summary>
        /// <param name="ptrData">内存数据</param>
        /// <param name="dataSize">数据大小(float)</param>
        /// <returns></returns>
        public static T[] CopyDataArrayFromIntptrAndFree<T>(ref IntPtr ptrData, int dataSize)
        {
            //拷贝校正系数出来
            if (ptrData == IntPtr.Zero)
            {
                ErrorString = GetDLLErrorMessage();
                return null;
            }

            T[] retdata = new T[dataSize];
            try
            {
                if(typeof(T) == typeof(byte))
                {
                    byte[] temp = (byte[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(int))
                {
                    int[] temp = (int[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(char))
                {
                    char[] temp = (char[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(float))
                {
                    float[] temp = (float[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                else if (typeof(T) == typeof(double))
                {
                    double[] temp = (double[])((object)retdata);
                    Marshal.Copy(ptrData, temp, 0, dataSize);
                }
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;

                return retdata;

            }
            catch (Exception ex)
            {
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 从内存中拷贝float数据，并且释放内存
        /// </summary>
        /// <param name="ptrData">内存数据</param>
        /// <param name="dataSize">数据大小(float)</param>
        /// <returns></returns>
        public static float[] CopyFloatFromIntptrAndFree(ref IntPtr ptrData, int dataSize)
        {
            //拷贝校正系数出来
            if (ptrData == IntPtr.Zero)
                return null;

            try
            {
                float[] retdata = new float[dataSize];
                Marshal.Copy(ptrData, retdata, 0, dataSize);
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;

                return retdata;

            }
            catch (Exception ex)
            {
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 从内存中拷贝double数据，并且释放内存
        /// </summary>
        /// <param name="ptrData">内存数据</param>
        /// <param name="dataSize">数据大小(double)</param>
        /// <returns></returns>
        public static double[] CopyDoubleFromIntptrAndFree(ref IntPtr ptrData, int dataSize)
        {
            //拷贝校正系数出来
            if (ptrData == IntPtr.Zero)
                return null;

            try
            {
                double[] retdata = new double[dataSize];
                Marshal.Copy(ptrData, retdata, 0, dataSize);
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;

                return retdata;

            }
            catch (Exception ex)
            {
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 从内存中拷贝Byte数据，并且释放内存
        /// </summary>
        /// <param name="ptrData">内存数据</param>
        /// <param name="dataSize">数据大小(byte)</param>
        /// <returns></returns>
        public static byte[] CopyByteFromIntptrAndFree(ref IntPtr ptrData, int dataSize)
        {
            //拷贝校正系数出来
            if (ptrData == IntPtr.Zero)
                return null;

            try
            {
                byte[] retdata = new byte[dataSize];
                Marshal.Copy(ptrData, retdata, 0, dataSize);
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;

                return retdata;

            }
            catch (Exception ex)
            {
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 从内存中拷贝Int数据，并且释放内存
        /// </summary>
        /// <param name="ptrData">内存数据</param>
        /// <param name="dataSize">数据大小(byte)</param>
        /// <returns></returns>
        public static int[] CopyIntFromIntptrAndFree(ref IntPtr ptrData, int dataSize)
        {
            //拷贝校正系数出来
            if (ptrData == IntPtr.Zero)
                return null;

            try
            {
                int[] retdata = new int[dataSize];
                Marshal.Copy(ptrData, retdata, 0, dataSize);
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;

                return retdata;

            }
            catch (Exception ex)
            {
                Marshal.FreeCoTaskMem(ptrData);
                ptrData = IntPtr.Zero;
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 判断是否为64为版本调用
        /// </summary>
        public static bool Is64BitVersion()
        {
            return IntPtr.Size == 8;
        }

        /// <summary>
        /// 把byte*转换为string
        /// </summary>
        /// <param name="data">byte*数据</param>
        /// <param name="size">数据大小</param>
        public unsafe static string ConvertFixedByteToString(byte* data, int size)
        {
            //byte[] dataary = ConvertFixedByteToByteArray(data, size);
            byte[] dataary = CopyDataArrayFromFixedPtr<byte>(data, size);
            if (dataary == null)
                return null;

            return Encoding.UTF8.GetString(dataary);
        }

        /// <summary>
        /// 把string转换为byte*
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="data">byte*数据</param>
        /// <param name="size">数据大小</param>
        public unsafe static void ConvertStringToFixedByte(string str, byte* data, int size)
        {
            if (string.IsNullOrWhiteSpace(str) || data == null || size < 1)
                return;

            byte[] byteary = Encoding.UTF8.GetBytes(str);
            if (byteary == null)
                return;
            int copysize = byteary.Length < size ? byteary.Length : size;

            //ConvertByteArrayToFixedByte(byteary, data, copysize);
            CopyDataArrayToFixedPtr<byte>(data, copysize, byteary);
        }

        /// <summary>
        /// 把string转换到byte[]
        /// </summary>
        /// <param name="str">要转换的String</param>
        /// <param name="byteBuffer">byte数组</param>
        /// <param name="size">byte Buffer的大小</param>
        /// <param name="encoding">编码</param>
        public unsafe static void ConvertStringToByteArray(string str, byte[] byteBuffer, int size, Encoding encoding)        
        {
            if (string.IsNullOrWhiteSpace(str) || byteBuffer == null || size < 1)
                return;
            byte[] byteary = encoding.GetBytes(str);
            if (byteary == null)
                return;
            int copysize = byteary.Length < size ? byteary.Length : size;

            Array.Copy(byteary, byteBuffer, copysize);
        }

        /// <summary>
        /// 将字节数组转换为String
        /// </summary>
        /// <param name="byteBuffer">字节数组</param>
        /// <param name="encoding">编码方式</param>
        public static string CovertByteArrayToString(byte[] byteBuffer, Encoding encoding )
        {
            if (byteBuffer == null || byteBuffer.Length == 0)
                return null;

            int copysize = 0;
            while (byteBuffer[copysize] != 0 && copysize <byteBuffer.Length)
                copysize++;

            return encoding.GetString(byteBuffer, 0, copysize);
        }

        /// <summary>
        /// 把byte*转换为byte[]
        /// </summary>
        /// <param name="fixeddata">byte*数据</param>
        /// <param name="size">数据大小</param>
        public unsafe static byte[] ConvertFixedByteToByteArrayaaa(byte* fixeddata, int size)
        {
            if (fixeddata == null || size < 1)
                return null;

            byte[] retdata = new byte[size];
            IntPtr ptr = new IntPtr(fixeddata);
            Marshal.Copy(ptr, retdata, 0, size);
            ptr = IntPtr.Zero;
            return retdata;
        }

        /// <summary>
        /// 把byte[]转换为byte*
        /// </summary>
        /// <param name="data">需要转换的数组</param>
        /// <param name="fixeddata">byte[]数据</param>
        /// <param name="size">数据大小</param>
        public unsafe static void ConvertByteArrayToFixedByte(byte[] data, byte* fixeddata, int size)
        {
            if (data == null || fixeddata == null || data.Length == 0 || data.Length > size)
                return;

            IntPtr ptr = new IntPtr(fixeddata);
            int copysize = data.Length < size ? data.Length : size;
            Marshal.Copy(data, 0, ptr, copysize);
            ptr = IntPtr.Zero;
        }

        /// <summary>
        /// 读取全部文件内容
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件内容</returns>
        public static byte[] ReadBinaryFile(string filename)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    byte[] retdata = new byte[stream.Length];
                    if (stream.Read(retdata, 0, retdata.Length) != retdata.Length)
                    {
                        ErrorString = "File read error";
                        retdata = null;
                    }
                    stream.Close();

                    return retdata;
                }
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 将数据写入文件
        /// </summary>
        /// <param name="filename">写入文件名</param>
        /// <param name="fileData">文件内容</param>
        public static bool SaveBinaryFile(string filename, byte[] fileData)
        {
            if (fileData == null || filename == null)
                return false;

            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(fileData, 0, fileData.Length);
                    stream.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 将内存复制到struct中,并释放
        /// </summary>
        /// <typeparam name="T">Struct类型</typeparam>
        /// <param name="dataPtr">数据的PTR</param>
        /// <param name="retOK">是否正确转换</param>
        /// <returns></returns>
        public static T CopyStructureFromIntptrAndFree<T>(ref IntPtr dataPtr, out bool retOK)
        {
            retOK = false;
            try
            {
                if (dataPtr == IntPtr.Zero)
                {
                    ErrorString = GetDLLErrorMessage();
                    return default(T);
                }

                T retdata = (T)Marshal.PtrToStructure(dataPtr, typeof(T));
                Marshal.FreeCoTaskMem(dataPtr);
                dataPtr = IntPtr.Zero;

                retOK = true;
                return retdata;
            }
            catch (System.Exception)
            {
                Marshal.FreeCoTaskMem(dataPtr);
                dataPtr = IntPtr.Zero;
                return default(T);
            }

        }

        /// <summary>
        /// 将内存复制到struct中,并释放
        /// </summary>
        /// <typeparam name="T">Struct类型</typeparam>
        /// <param name="dataPtr">数据的PTR</param>
        /// <param name="count">数据数量</param>
        /// <returns></returns>
        public static List<T> CopyStructureListFromIntptrAndFree<T>(ref IntPtr dataPtr, int count) where T:struct
        {
            List<T> retlist = new List<T>();
            try
            {
                if (dataPtr == IntPtr.Zero)
                    return null;

                for(int i=0; i<count; i++)
                {
                    IntPtr curptr = IntPtr.Add(dataPtr, i * Marshal.SizeOf(typeof(T)));
                    T curdata = (T)Marshal.PtrToStructure(curptr, typeof(T));
                    retlist.Add(curdata);
                }
                Marshal.FreeCoTaskMem(dataPtr);
                dataPtr = IntPtr.Zero;

                return retlist;
            }
            catch (System.Exception)
            {
                Marshal.FreeCoTaskMem(dataPtr);
                dataPtr = IntPtr.Zero;
                return null;
            }

        }

        /* Date/Time LSB: min=6b,hour=5b,day=5b,month=4b,year=12b */
        /// <summary>
        /// 把DWORD转换到DateTime
        /// </summary>
        /// <param name="dtime">DWORD存储的日期时间</param>
        /// <returns></returns>
        public static DateTime DWordToDateTime(uint dtime)
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

        /* Date/Time LSB: min=6b,hour=5b,day=5b,month=4b,year=12b */
        /// <summary>
        /// 日期时间转换到DWORD
        /// </summary>
        /// <param name="time">日期时间</param>
        /// <returns></returns>
        public static UInt32 DataTimeToDWord(DateTime time)
        {
            if (time == null)
                return (uint)((2000 << 20) | (1 << 16) | (1 << 11));    //返回2000-1-1 00:00:00
            else
                return (uint)((time.Year << 20) | (time.Month << 16) | (time.Day << 11) | (time.Hour << 6) | time.Minute);
        }

        /// <summary>
        /// 将固定的指针的内容拷贝到数组中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fixedPtr">固定指针</param>
        /// <param name="ptrSize">数据数量</param>
        /// <returns></returns>
        public unsafe static T[] CopyDataArrayFromFixedPtr<T>(void* fixedPtr, int ptrSize)
        {
            if (fixedPtr == null || ptrSize < 1)
                return null;

            T[] retdata = new T[ptrSize];
            IntPtr ptr = new IntPtr(fixedPtr);
            if (typeof(T) == typeof(byte))
            {
                byte[] temp = (byte[])((object)retdata);
                Marshal.Copy(ptr, temp, 0, ptrSize);
            }
            else if (typeof(T) == typeof(int))
            {
                int[] temp = (int[])((object)retdata);
                Marshal.Copy(ptr, temp, 0, ptrSize);
            }
            else if (typeof(T) == typeof(char))
            {
                char[] temp = (char[])((object)retdata);
                Marshal.Copy(ptr, temp, 0, ptrSize);
            }
            else if (typeof(T) == typeof(float))
            {
                float[] temp = (float[])((object)retdata);
                Marshal.Copy(ptr, temp, 0, ptrSize);
            }
            else if(typeof(T) == typeof(double))
            {
                double[] temp = (double[])((object)retdata);
                Marshal.Copy(ptr, temp, 0, ptrSize);
            }
            return retdata;
        }

        /// <summary>
        /// 将数组拷贝到固定指针中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fixedPtr">固定指针</param>
        /// <param name="ptrSize">数据大小</param>
        /// <param name="datas">要拷贝的数组</param>
        /// <returns></returns>
        public unsafe static bool CopyDataArrayToFixedPtr<T>(void* fixedPtr, int ptrSize, T[] datas)
        {
            if (fixedPtr == null || ptrSize < 1 || datas == null || datas.Length > ptrSize)
                return false;

            IntPtr ptr = new IntPtr(fixedPtr);
            if (typeof(T) == typeof(byte))
            {
                byte[] temp = (byte[])((object)datas);
                Marshal.Copy(temp, 0, ptr, ptrSize);
            }
            else if (typeof(T) == typeof(int))
            {
                int[] temp = (int[])((object)datas);
                Marshal.Copy(temp, 0, ptr, ptrSize);
            }
            else if (typeof(T) == typeof(char))
            {
                char[] temp = (char[])((object)datas);
                Marshal.Copy(temp, 0, ptr, ptrSize);
            }
            else if (typeof(T) == typeof(float))
            {
                float[] temp = (float[])((object)datas);
                Marshal.Copy(temp, 0, ptr, ptrSize);
            }
            else if (typeof(T) == typeof(double))
            {
                double[] temp = (double[])((object)datas);
                Marshal.Copy(temp, 0, ptr, ptrSize);
            }
            return true;
        }

        /// <summary>
        /// 从内存中拷贝字符串，并释放内存
        /// </summary>
        /// <param name="ptrData">内存指针</param>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        public static string CopyStringFromIntptrAndFree(ref IntPtr ptrData, Encoding encode)
        {
            if (ptrData == IntPtr.Zero)
                return null;

            string retstr = null;

            if (encode == Encoding.ASCII)
                retstr = Marshal.PtrToStringAnsi(ptrData);
            else
                retstr = Marshal.PtrToStringAuto(ptrData);

            Marshal.FreeCoTaskMem(ptrData);
            ptrData = IntPtr.Zero;

            return retstr;
        }

        /// <summary>
        /// 字符串转数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        public static byte[] StringToByte(string str, Encoding encode)
        {
            byte[] tempbytes = encode.GetBytes(str);
            if (tempbytes == null)
                return null;

            int addlen = 0;
            if (encode == Encoding.ASCII)
                addlen = 1;
            else
                addlen = 2;

            byte[] retbytes = new byte[tempbytes.Length + addlen];
            Array.Copy(tempbytes, retbytes, tempbytes.Length);

            return retbytes;
        }

        /// <summary>
        /// 将Float数组转换为Double数组
        /// </summary>
        /// <param name="datas">Float数组</param>
        /// <returns></returns>
        public static double[] FloatArrayToDouble(float[] datas)
        {
            if (datas == null || datas.Length == 0)
                return null;

            double[] retdatas = new double[datas.Length];
            for(int i=0; i<datas.Length; i++)
            {
                retdatas[i] = datas[i];
            }

            return retdatas;
        }

        /// <summary>
        /// 将Double数组转换为Float数组
        /// </summary>
        /// <param name="datas">Double数组</param>
        /// <returns></returns>
        public static float[] DoubleArrayToFloat(double[] datas)
        {
            if (datas == null || datas.Length == 0)
                return null;

            float[] retdatas = new float[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                retdatas[i] = (float)datas[i];
            }

            return retdatas;
        }


        /// <summary>
        /// 比较两个double是否相同
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="maximum">最大偏差</param>
        /// <returns></returns>
        public static bool IsSameDouble(double value1, double value2, double maximum)
        {
            return Math.Abs(value1 - value2) <= Math.Abs(maximum);
        }

        /// <summary>
        /// 判断两个X轴数据格式是否相同
        /// </summary>
        /// <param name="firstData">X值1</param>
        /// <param name="lastData">X值2</param>
        /// <returns></returns>
        public static bool IsSameXDatas(double[] firstData, double[] lastData)
        {
            if (firstData == null || lastData == null || firstData.Length<1 || firstData.Length != lastData.Length)
                return false;

            int cols = firstData.Length;
            double maximum = 0;

            //按照步长的1/100来比较
            if (cols > 1)
            {
                maximum = (firstData[cols - 1] - firstData[0]) / (cols - 1);
                maximum = maximum / 100;
            }

            return IsSameDouble(firstData[0],lastData[0], maximum) && IsSameDouble(firstData[cols-1], lastData[cols-1], maximum);
        }

        /// <summary>
        /// 判断X轴数据格式是否与标准参数相同
        /// </summary>
        /// <param name="specData">X数据</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="dataCount">数据点数</param>
        /// <returns></returns>
        public static bool IsSameXDatas(double[] specData, double firstX, double lastX, int dataCount)
        {
            if (specData == null || specData.Length != dataCount)
                return false;

            //步长
            double maximum = (lastX - firstX) / (dataCount - 1);
            maximum = maximum / 100;    //步长的1/100

            return IsSameDouble(specData[0], firstX, maximum) && IsSameDouble(specData[specData.Length - 1], lastX, maximum);
        }
    }

    
}
