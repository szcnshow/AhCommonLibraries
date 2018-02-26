using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.Algorithm
{
    /// <summary>
    /// 预处理方法类
    /// </summary>
    public class PreProcessor
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString;

        #region 32bitDLL
        /// <summary>
        /// 是否有预处理算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPHasAuthority")]
        private static extern bool PPHasAuthority32();

        /// <summary>
        /// 三次样条曲线插值(使用步长)
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="stepX">波数间隔</param>
        /// <param name="outColsSize">返回的光谱数据点数</param>
        /// <returns>返回处理后的数据(需要自己释放), 第一行是X轴，其余行为Y轴, 大小= specRows * outColsSize;</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSplineCubicInterpolation")]
        private static extern IntPtr PPSplineCubicInterpolation32(double[] specDatas, int specRows, int specCols, double firstX, double lastX, double stepX, ref int outColsSize);

        /// <summary>
        /// 三次样条曲线插值(使用数据点数量)
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="dataCount">数据点数量</param>
        /// <returns>返回处理后的数据(需要自己释放), 第一行是X轴，其余行为Y轴, 大小= specRows * outColsSize;</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPInterpolationUseDataCount")]
        private static extern IntPtr PPInterpolationUseDataCount32(double[] specDatas, int specRows, int specCols, double firstX, double lastX, int dataCount);

        /// <summary>
        /// 矢量归一化（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPVectorNormalize")]
        private static extern bool PPVectorNormalize32(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// 最大最小归一化（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPMaxminNormalize")]
        private static extern bool PPMaxminNormalize32(double[] specDatas, int specRows, int specCols);

        /// <summary>
        /// 减去一条直线(自动基线校正)（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSubtractStraightLine")]
        private static extern bool PPSubtractStraightLine32(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// 消除常量偏移（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSubtractConstantOffset")]
        private static extern bool PPSubtractConstantOffset32(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// SNV标准正态变换（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSNVNormalDistribution")]
        private static extern bool PPSNVNormalDistribution32(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// MSC多元散射校正（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPMutiScatterCorrection")]
        private static extern bool PPMutiScatterCorrection32(double[] specDatas, int specRows, int specCols);

        /// <summary>
        /// 预测时的MSC多元散射校正（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="averageYDatas">建模平均光谱</param>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPMutiScatterCorrectionOnPredict")]
        private static extern bool PPMutiScatterCorrectionOnPredict32(double[] averageYDatas, double[] specDatas, int specRows, int specCols);

        /// <summary>
        /// Savitzky–Golay平滑和求导数（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="windowSize">平滑点数</param>
        /// <param name="polyDegree">多项式阶数</param>
        /// <param name="derDegree">导数阶数, 0=平滑</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSGSmoothAndDerivative")]
        private static extern bool PPSGSmoothAndDerivative32(double[] spectrumDatas, int specRows, int specCols, int windowSize, int polyDegree, int derDegree);

        #endregion

        #region 64bitDLL
        /// <summary>
        /// 是否有预处理算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPHasAuthority")]
        private static extern bool PPHasAuthority64();

        /// <summary>
        /// 三次样条曲线插值
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="stepX">波数间隔</param>
        /// <param name="outColsSize">返回的光谱数据点数</param>
        /// <returns>返回处理后的数据(需要自己释放), 大小= specRows * outColsSize;</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSplineCubicInterpolation")]
        private static extern IntPtr PPSplineCubicInterpolation64(double[] specDatas, int specRows, int specCols, double firstX, double lastX, double stepX, ref int outColsSize);

        /// <summary>
        /// 三次样条曲线插值(使用数据点数量)
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="dataCount">光谱数据点数</param>
        /// <returns>返回处理后的数据(需要自己释放), 第一行是X轴，其余行为Y轴, 大小= specRows * outColsSize;</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPInterpolationUseDataCount")]
        private static extern IntPtr PPInterpolationUseDataCount64(double[] specDatas, int specRows, int specCols, double firstX, double lastX, int dataCount);

        /// <summary>
        /// 矢量归一化（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPVectorNormalize")]
        private static extern bool PPVectorNormalize64(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// 最大最小归一化（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPMaxminNormalize")]
        private static extern bool PPMaxminNormalize64(double[] specDatas, int specRows, int specCols);

        /// <summary>
        /// 减去一条直线(自动基线校正)（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSubtractStraightLine")]
        private static extern bool PPSubtractStraightLine64(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// 消除常量偏移（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSubtractConstantOffset")]
        private static extern bool PPSubtractConstantOffset64(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// SNV标准正态变换（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSNVNormalDistribution")]
        private static extern bool PPSNVNormalDistribution64(double[] spectrumDatas, int specRows, int specCols);

        /// <summary>
        /// MSC多元散射校正（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPMutiScatterCorrection")]
        private static extern bool PPMutiScatterCorrection64(double[] specDatas, int specRows, int specCols);

        /// <summary>
        /// 预测时的MSC多元散射校正（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="averageYDatas">建模平均光谱</param>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPMutiScatterCorrectionOnPredict")]
        private static extern bool PPMutiScatterCorrectionOnPredict64(double[] averageYDatas, double[] specDatas, int specRows, int specCols);

        /// <summary>
        /// Savitzky–Golay平滑和求导数（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="windowSize">平滑点数</param>
        /// <param name="polyDegree">多项式阶数</param>
        /// <param name="derDegree">导数阶数, 0=平滑</param>
        /// <returns>正确或错误</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PPSGSmoothAndDerivative")]
        private static extern bool PPSGSmoothAndDerivative64(double[] spectrumDatas, int specRows, int specCols, int windowSize, int polyDegree, int derDegree);

        #endregion

        #region 外部调用接口，32bit和64bit自动适应
        /// <summary>
        /// 是否有预处理算法授权
        /// </summary>
        public static bool HasAuthority()
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return PPHasAuthority64();
                else
                    return PPHasAuthority32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 三次样条曲线插值（使用X轴步长）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="stepX">波数间隔</param>
        /// <returns>返回处理后的数据,第一行是X轴，其余行为Y轴;</returns>
        public static List<double[]> SplineCubicInterpolation(IList<double[]> specDatas, double firstX, double lastX, double stepX)
        {
            if (specDatas == null || (firstX < lastX && stepX < 0) || (firstX > lastX && stepX > 0) || stepX == 0 || specDatas.Count < 2)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            double[] alldatas = CommonMethod.CombineSpectrumDatas(specDatas);
            if (alldatas == null)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            return SplineCubicInterpolation(alldatas, specDatas.Count, specDatas[0].Length, firstX, lastX, stepX);
        }

        /// <summary>
        /// 三次样条曲线插值（使用X轴步长）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="stepX">波数间隔</param>
        /// <returns>返回处理后的数据, 第一行是X轴，其余行为Y轴</returns>
        public static List<double[]> SplineCubicInterpolation(double[] specDatas, int specRows, int specCols, double firstX, double lastX, double stepX)
        {
            try
            {
                if (specDatas == null || specRows < 2 || specCols < 2 || stepX == 0 ||
                    (firstX < lastX && stepX < 0) || (firstX > lastX && stepX > 0))
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;
                int outColsSize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = PPSplineCubicInterpolation64(specDatas, specRows, specCols, firstX, lastX, stepX, ref outColsSize);
                else
                    retptr = PPSplineCubicInterpolation32(specDatas, specRows, specCols, firstX, lastX, stepX, ref outColsSize);
                
                if (retptr == IntPtr.Zero)
                    throw new Exception(CommonMethod.GetDLLErrorMessage());

                List<double[]> retdatas = new List<double[]>();

                //逐行拷贝返回数据，第一行是X轴，其余行为Y轴
                for (int i = 0; i < specRows; i++)
                {
                    double[] rowdata = new double[outColsSize];
                    Marshal.Copy(IntPtr.Add(retptr, i*outColsSize*sizeof(double)), rowdata, 0, outColsSize);
                    retdatas.Add(rowdata);
                }
                Marshal.FreeCoTaskMem(retptr);

                return retdatas;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 三次样条曲线插值（使用数据点数量）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="dataCout">光谱数据点数</param>
        /// <returns>返回处理后的数据,第一行是X轴，其余行为Y轴;</returns>
        public static List<double[]> SplineCubicInterpolation(IList<double[]> specDatas, double firstX, double lastX, int dataCout)
        {
            double[] alldatas = null;
            if (specDatas == null || dataCout < 1  || (alldatas = CommonMethod.CombineSpectrumDatas(specDatas)) == null)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            return SplineCubicInterpolation(alldatas, specDatas.Count, specDatas[0].Length, firstX, lastX, dataCout);
        }

        /// <summary>
        /// 三次样条曲线插值（使用数据点数量）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="dataCount">数据点数量</param>
        /// <returns>返回处理后的数据, 第一行是X轴，其余行为Y轴</returns>
        public static List<double[]> SplineCubicInterpolation(double[] specDatas, int specRows, int specCols, double firstX, double lastX, int dataCount)
        {
            try
            {
                double[] newDatas = SplineCubicInterpolation(specDatas, specRows, specCols, firstX, lastX, dataCount, true);
                if (newDatas == null)
                    return null;

                return CommonMethod.SplitSpectrumDatas<double>(newDatas, specRows);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 三次样条曲线插值（使用数据点数量,直接返回数组）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="dataCount">数据点数量</param>
        /// <param name="returnArray">直接返回数组</param>
        /// <returns>返回处理后的数据, 第一行是X轴，其余行为Y轴</returns>
        public static double[] SplineCubicInterpolation(double[] specDatas, int specRows, int specCols, double firstX, double lastX, int dataCount, bool returnArray)
        {
            try
            {
                if (specDatas == null || specRows < 2 || specCols < 2 || dataCount < 1)
                    throw new Exception("Invalid parameters");

                //如果数据格式相同，不需要处理
                if (CommonMethod.IsSameXDatas(specDatas[0], specDatas[specCols-1], firstX, lastX, dataCount) && dataCount==specCols)
                    return specDatas;

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PPInterpolationUseDataCount64(specDatas, specRows, specCols, firstX, lastX, dataCount);
                else
                    retptr = PPInterpolationUseDataCount32(specDatas, specRows, specCols, firstX, lastX, dataCount);

                if (retptr == IntPtr.Zero)
                    throw new Exception(CommonMethod.GetDLLErrorMessage());

                double[] retdatas = new double[dataCount * specRows];
                Marshal.Copy(retptr, retdatas, 0, retdatas.Length);

                Marshal.FreeCoTaskMem(retptr);

                return retdatas;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 矢量归一化（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns></returns>
        public static bool VectorNormalize(double[] spectrumDatas, int specRows, int specCols)
        {
            try
            {
                if (spectrumDatas == null || spectrumDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPVectorNormalize64(spectrumDatas, specRows, specCols);
                else
                    retcode = PPVectorNormalize32(spectrumDatas, specRows, specCols);

                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }        
        }

        /// <summary>
        /// 最大最小归一化（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        public static bool MaxminNormalize(double[] specDatas, int specRows, int specCols)
        {
            try
            {
                if (specDatas == null || specDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPMaxminNormalize64(specDatas, specRows, specCols);
                else
                    retcode = PPMaxminNormalize32(specDatas, specRows, specCols);

                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }        
        }

        /// <summary>
        /// 减去一条直线(自动基线校正)（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        public static bool SubtractStraightLine(double[] spectrumDatas, int specRows, int specCols)
        {
            try
            {
                if (spectrumDatas == null || spectrumDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPSubtractStraightLine64(spectrumDatas, specRows, specCols);
                else
                    retcode = PPSubtractStraightLine32(spectrumDatas, specRows, specCols);

                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 消除常量偏移（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        public static bool SubtractConstantOffset(double[] spectrumDatas, int specRows, int specCols)
        {
            try
            {
                if (spectrumDatas == null || spectrumDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPSubtractConstantOffset64(spectrumDatas, specRows, specCols);
                else
                    retcode = PPSubtractConstantOffset32(spectrumDatas, specRows, specCols);

                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// SNV标准正态变换（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        public static bool SNVNormalDistribution(double[] spectrumDatas, int specRows, int specCols)
        {
            try
            {
                if (spectrumDatas == null || spectrumDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPSNVNormalDistribution64(spectrumDatas, specRows, specCols);
                else
                    retcode = PPSNVNormalDistribution32(spectrumDatas, specRows, specCols);

                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// MSC多元散射校正（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        public static bool MutiScatterCorrection(double[] specDatas, int specRows, int specCols)
        {
            try
            {
                if (specDatas == null || specDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPMutiScatterCorrection64(specDatas, specRows, specCols);
                else
                    retcode = PPMutiScatterCorrection32(specDatas, specRows, specCols);
                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 预测时的MSC多元散射校正（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="averageData">建模平均光谱（从PLS Coefficient获取）</param>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <returns>正确或错误</returns>
        public static bool MutiScatterCorrectionOnPredict(double[] averageData, double[] specDatas, int specRows, int specCols)
        {
            try
            {
                if (specDatas == null || averageData==null || specDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPMutiScatterCorrectionOnPredict64(averageData, specDatas, specRows, specCols);
                else
                    retcode = PPMutiScatterCorrectionOnPredict32(averageData, specDatas, specRows, specCols);
                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Savitzky–Golay平滑和求导数（处理后的数据在spectrumDatas返回）
        /// </summary>
        /// <param name="specDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="windowSize">平滑点数</param>
        /// <param name="polyDegree">多项式阶数</param>
        /// <param name="derDegree">导数阶数, 0=平滑</param>
        /// <returns>正确或错误</returns>
        public static bool SGSmoothAndDerivative(double[] specDatas, int specRows, int specCols, int windowSize, int polyDegree, int derDegree)
        {
            try
            {
                if (specDatas == null || specDatas.Length != specRows * specCols)
                {
                    ErrorString = "Invalid parameters";
                    return false;
                }

                bool retcode = false;
                if (CommonMethod.Is64BitVersion())
                    retcode = PPSGSmoothAndDerivative64(specDatas, specRows, specCols, windowSize, polyDegree, derDegree);
                else
                    retcode = PPSGSmoothAndDerivative32(specDatas, specRows, specCols, windowSize, polyDegree, derDegree);
                if (!retcode)
                    ErrorString = CommonMethod.GetDLLErrorMessage();

                return retcode;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        #endregion
    }
}
