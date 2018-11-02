using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
//using Ai.Hong.CommonLibrary;

namespace Ai.Hong.Algorithm
{
    /// <summary>
    /// 通用函数
    /// </summary>
    public class CommonMethod
    {
        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern IntPtr GetErrorMessage32();

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern IntPtr GetErrorMessage64();

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

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
                    ErrorString = "Data count is different between spectral";
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
        /// 从组合的数据中分离为单条数据的列表
        /// </summary>
        /// <param name="combinedDatas">组合数据</param>
        /// <param name="specRows">数据行数</param>
        /// <returns>分离后的列表数据</returns>
        public static List<T[]> SplitSpectrumDatas<T>(T[] combinedDatas, int specRows)
        {
            if (combinedDatas == null || specRows < 1 || (combinedDatas.Length % specRows) != 0)
                return null;

            int specCols = combinedDatas.Length / specRows;

            //拷贝每条光谱
            List<T[]> retDatas = new List<T[]>();
            for (int i = 0; i < specRows; i++)
            {
                T[] curdata = new T[specCols];
                Array.Copy(combinedDatas, i * curdata.Length, curdata, 0, curdata.Length);
                retDatas.Add(curdata);
            }

            return retDatas;
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
                return null;

            T[] retdata = new T[dataSize];
            try
            {
                if (typeof(T) == typeof(byte))
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
        /// 判断是否为64为版本调用
        /// </summary>
        public static bool Is64BitVersion()
        {
            return IntPtr.Size == 8;
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
            if (firstData == null || lastData == null || firstData.Length < 1 || firstData.Length != lastData.Length)
                return false;

            int cols = firstData.Length;
            double maximum = 0;

            //按照步长的1/100来比较
            if (cols > 1)
            {
                maximum = (firstData[cols - 1] - firstData[0]) / (cols - 1);
                maximum = maximum / 100;
            }

            return IsSameDouble(firstData[0], lastData[0], maximum) && IsSameDouble(firstData[cols - 1], lastData[cols - 1], maximum);
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

        /// <summary>
        /// 判断X轴数据格式是否与标准参数相同
        /// </summary>
        /// <param name="specFirstX">光谱X轴起始</param>
        /// <param name="specLastX">光谱X轴结束</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="dataCount">数据点数</param>
        /// <returns></returns>
        public static bool IsSameXDatas(double specFirstX, double specLastX, double firstX, double lastX, int dataCount)
        {
            //步长
            double maximum = (lastX - firstX) / (dataCount - 1);
            maximum = maximum / 100;    //步长的1/100

            return IsSameDouble(specFirstX, firstX, maximum) && IsSameDouble(specLastX, lastX, maximum);
        }

        /// <summary>
        /// 是否在区间内
        /// </summary>
        /// <param name="value">要查找的值</param>
        /// <param name="startValue">区间起始值</param>
        /// <param name="endValue">区间结束值</param>
        /// <returns>是否在区间内</returns>
        private static bool ValueInside(double value, double startValue, double endValue)
        {
            if (startValue < endValue)
            {
                if (value >= startValue && value <= endValue)
                    return true;
                else
                    return false;
            }
            else
            {
                if (value >= endValue && value <= startValue)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 比较大小，如果begin>end，交换
        /// </summary>
        /// <param name="beginvalue">小的索引</param>
        /// <param name="endvalue">大的索引</param>
        public static void SortInOrder(ref int beginvalue, ref int endvalue)
        {
            if (beginvalue > endvalue)
            {
                int temp = endvalue;
                endvalue = beginvalue;
                beginvalue = temp;
            }
        }

        /// <summary>
        /// 查找valueToFind在X轴数据中的位置
        /// </summary>
        /// <param name="xData">X轴数据</param>
        /// <param name="beginx">查找起始点</param>
        /// <param name="endx">查找结束点</param>
        /// <param name="valueToFind">要查找的值</param>
        /// <returns>返回valueToFind的位置</returns>
        public static int FindNearestPosition(double[] xData, int beginx, int endx, double valueToFind)
        {
            if (xData == null || xData.Length == 0 || !ValueInside(valueToFind, xData[beginx], xData[endx]))
                return -1;

            int foundPos = -1;
            if (xData[beginx] == valueToFind)
                foundPos = beginx;
            else if (xData[endx] == valueToFind)
                foundPos = endx;
            else if (beginx == endx || beginx == endx - 1)      //只相差1个，不用再分了
            {
                if (Math.Abs(xData[endx] - valueToFind) > Math.Abs(xData[beginx] - valueToFind))
                    foundPos = beginx;
                else
                    foundPos = endx;
            }

            if (foundPos != -1)  // 找到了,还要比较左右看看哪个更接近
            {
                double x1 = Math.Abs(xData[foundPos] - valueToFind);
                double x2 = (foundPos - 1 > 0) ? Math.Abs(xData[foundPos - 1] - valueToFind) : x1;
                double x3 = (foundPos + 1) < xData.Length ? Math.Abs(xData[foundPos + 1] - valueToFind) : x1;

                if (x1 <= x2 && x1 <= x3)
                    return foundPos;
                else if (x2 < x1 && x2 < x3)
                    return foundPos - 1;
                else
                    return foundPos + 1;
            }

            int midPos = beginx + (endx - beginx) / 2;      //中间点
            if (xData[0] < xData[xData.Length - 1])      //递增序列
            {
                if (xData[midPos] < valueToFind)      //中间点小于valutToFind，在后面查找
                {
                    return FindNearestPosition(xData, midPos, endx, valueToFind);
                }
                else //中间点大于valutToFind，在前面查找
                {
                    return FindNearestPosition(xData, beginx, midPos, valueToFind);
                }
            }
            else        //递减序列
            {
                if (xData[midPos] < valueToFind)      //中间点小于valutToFind，在前面查找
                {
                    return FindNearestPosition(xData, beginx, midPos, valueToFind);
                }
                else //中间点大于valutToFind，在后面查找
                {
                    return FindNearestPosition(xData, midPos, endx, valueToFind);
                }
            }
        }

        /// <summary>
        /// 确保开始和结束数值在数组取值范围内
        /// </summary>
        /// <param name="datas">数组取值</param>
        /// <param name="firstX">开始数据</param>
        /// <param name="lastX">结束数据</param>
        public static void FillIntoDataRange(double[] datas, ref double firstX, ref double lastX)
        {
            if (firstX < datas[0])
                firstX = datas[0];
            if (lastX > datas[datas.Length - 1])
                lastX = datas[datas.Length - 1];
        }

        /// <summary>
        /// 获取指定区间内的数据
        /// </summary>
        /// <param name="datas">相同长度的数据列表</param>
        /// <param name="beginIndex">起始区间</param>
        /// <param name="count">区间长度</param>
        /// <returns>数据列表</returns>
        public static List<double[]> GetRangeData(List<double[]> datas, int beginIndex, int count)
        {
            int minlen = datas.Select(p => p.Length).Min();     //必须要保证数据在区间范围内
            if(datas == null || beginIndex < 0 || beginIndex +  count > minlen)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            //检测是否所有数据长度相同
            foreach(var data in datas)
            {
                if (data.Length != datas[0].Length)
                {
                    ErrorString = "Invalid parameters";
                    return null;
                }
            }

            List<double[]> retDatas = new List<double[]>();
            foreach(var data in datas)
            {
                if (beginIndex == 0 && count == data.Length)    //与原有数据区间完全相同，直接引用
                    retDatas.Add(data);
                else    //是原有数据的一部分，拷贝区间数据
                {
                    double[] newdata = new double[count];
                    Array.Copy(data, beginIndex, newdata, 0, count);
                    retDatas.Add(newdata);
                }
            }

            return retDatas;
        }

        /// <summary>
        /// 获取指定区间内的数据
        /// </summary>
        /// <param name="xyDatas">相同长度的数据列表，第一行是X轴，后面是Y轴(可以多个)</param>
        /// <param name="firstX">起始区间</param>
        /// <param name="lastX">结束区间</param>
        /// <returns>数据列表</returns>
        public static List<double[]> GetRangeData(List<double[]> xyDatas, double firstX, double lastX)
        {
            if (xyDatas == null || xyDatas.Count < 2 )
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            //检测是否所有数据长度相同
            foreach (var data in xyDatas)
            {
                if (data.Length != xyDatas[0].Length)
                {
                    ErrorString = "Invalid parameters";
                    return null;
                }
            }

            List<double[]> retDatas = new List<double[]>();

            //数据格式相同,直接返回
            if(IsSameXDatas(xyDatas[0], firstX, lastX, xyDatas[0].Length))
            {
                retDatas.AddRange(xyDatas);
                return retDatas;
            }

            //查找区间对应的索引
            int beginIndex = FindNearestPosition(xyDatas[0], 0, xyDatas[0].Length-1, firstX);
            int endIndex = FindNearestPosition(xyDatas[0], 0, xyDatas[0].Length-1, lastX);
            if(beginIndex <0 || endIndex <0 || beginIndex>endIndex || endIndex >=xyDatas[0].Length)
            {
                ErrorString = "Invalid range parameter";
                return null;
            }

            return GetRangeData(xyDatas, beginIndex, endIndex - beginIndex + 1);
        }

        /// <summary>
        /// 将原始数组拷贝到目标数组的后面
        /// </summary>
        /// <typeparam name="T">数据格式</typeparam>
        /// <param name="SrcArray">原始数组</param>
        /// <param name="SrcOffset">原始数组起始位置</param>
        /// <param name="SrcLength">原始数组长度</param>
        /// <param name="DstArray">目标数组</param>
        /// <param name="DstOffset">目标数组起始位置</param>
        /// <returns>-1：目标数组的空间不够，else：拷贝后目标数组有效数据长度</returns>
        public static int CombineArrayAtTail<T>(T[] SrcArray, int SrcOffset, int SrcLength, T[] DstArray, int DstOffset)
        {
            //目标Array空间不够
            if (SrcLength > DstArray.Length)
                return -1;

            int copyOffset = DstOffset;

            //空间不够，把DstArray的数据向前移，在最后留出SrcLength空间
            if(DstOffset+SrcLength > DstArray.Length)
            {
                int moveout = DstOffset + SrcLength - DstArray.Length;
                Array.Copy(DstArray, moveout, DstArray, 0, DstOffset - moveout);
                copyOffset = DstArray.Length - SrcLength;
            }
            Array.Copy(SrcArray, SrcOffset, DstArray, copyOffset, SrcLength);

            return copyOffset + SrcLength;
        }
    }
    
}
