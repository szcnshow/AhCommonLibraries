using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.Algorithm
{
    /// <summary>
    /// 相关系数定性分析（拉曼）
    /// </summary>
    public class AdvancedIdentify
    {
        /// <summary>
        /// 基线校正方法
        /// </summary>
        public enum BaseLineMethod
        {
            /// <summary>
            /// 不进行校正
            /// </summary>
            None=-1,
            /// <summary>
            /// 偏移校正
            /// </summary>
            Offset=0,
            /// <summary>
            /// 线性校正
            /// </summary>
            Linear=1,
            /// <summary>
            /// 曲线校正
            /// </summary>
            Curve=2
        }

        /// <summary>
        /// 导数方法
        /// </summary>
        public enum DerivativeMethod
        {
            /// <summary>
            /// 不处理
            /// </summary>
            None=-1,
            /// <summary>
            /// 平滑
            /// </summary>
            Smooth=0,
            /// <summary>
            /// 一阶导数
            /// </summary>
            First=1,
            /// <summary>
            /// 二阶导数
            /// </summary>
            Second=2,
            /// <summary>
            /// 三阶导数
            /// </summary>
            Third=3
        }

        /// <summary>
        /// 计算系数的方法
        /// </summary>
        public enum CoefficientMethod
        {
            //0=无权重, 1=强度, 2=强度平方, 3=|secondYDatas|/(specCols+1), 4=(first - second)的平方
            /// <summary>
            /// 无权重 ones()
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 强度 abs(dest)
            /// </summary>
            Weight=1,
            /// <summary>
            /// 强度平方square(dest)
            /// </summary>
            SquareWeight=2,
            /// <summary>
            /// 距离 (dest.t() * src) / (dest.t() * dest)
            /// </summary>
            Distance=3,
            /// <summary>
            /// 距离平方 square(src - dest)
            /// </summary>
            SquareDistance=4
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

        #region 32Bit DLL

        /// <summary>
        /// 是否有AdvancedIdentify算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADHasAuthority")]
        private static extern bool HasAuthority32();

        /// <summary>
        /// 光谱基线校正
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="polyDegree">基线校正多项式阶数, 0=offset, 1=linear, 2=curve...</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADBaselineCorrect")]
        private static extern bool ADBaselineCorrect32(double[] spectrumDatas, int specRows, int specCols, int polyDegree);

        /// <summary>
        /// 光谱相似系数
        /// </summary>
        /// <param name="srcYDatas">光谱的Y轴数据</param>
        /// <param name="destYDatas">光谱的Y轴数据</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="coefMethod">0=无权重, 1=强度, 2=强度平方, 3=|secondYDatas|/(specCols+1), 4=(first - second)的平方</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADCoefficient")]
        private static extern double ADCoefficient32(double[] srcYDatas, double[] destYDatas, int specCols, int coefMethod);

        /// <summary>
        /// AdvancedIdentify定性分析
        /// </summary>
        /// <param name="srcApiDatas">API原始光谱(未做基线或导数处理），一列X轴，一列Y轴</param>
        /// <param name="apiDatas">API光谱处理后，一列X轴，一列Y轴</param>
        /// <param name="inferDatas">干扰组分光谱处理后，一列X轴，多列Y轴</param>
        /// <param name="inferRows">干扰组分数据行数（包括X轴）</param>
        /// <param name="sampleDatas">待分析物光谱处理后，一列X轴，一列Y轴</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="coefMethod">相关方法，0=无权重, 1=强度, 2=强度平方, 3=|secondYDatas|/(specCols+1), 4=(first - second)的平方</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADIdendify")]
        private static extern double ADIdendify32(double[] srcApiDatas, double[] apiDatas, double[] inferDatas, int inferRows, double[] sampleDatas, int specCols, int coefMethod);
        
        #endregion

        #region 64Bit DLL

        /// <summary>
        /// 是否有AdvancedIdentify算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADHasAuthority")]
        private static extern bool HasAuthority64();

        /// <summary>
        /// 光谱基线校正
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="polyDegree">基线校正多项式阶数, 0=offset, 1=linear, 2=curve...</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADBaselineCorrect")]
        private static extern bool ADBaselineCorrect64(double[] spectrumDatas, int specRows, int specCols, int polyDegree);

        /// <summary>
        /// 光谱相似系数
        /// </summary>
        /// <param name="srcYDatas">光谱的Y轴数据</param>
        /// <param name="destYDatas">光谱的Y轴数据</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="coefMethod">0=无权重, 1=强度, 2=强度平方, 3=|secondYDatas|/(specCols+1), 4=(first - second)的平方</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADCoefficient")]
        private static extern double ADCoefficient64(double[] srcYDatas, double[] destYDatas, int specCols, int coefMethod);

        /// <summary>
        /// AdvancedIdentify定性分析
        /// </summary>
        /// <param name="srcApiDatas">API原始光谱(未做基线或导数处理），一列X轴，一列Y轴</param>
        /// <param name="apiDatas">API光谱处理后，一列X轴，一列Y轴</param>
        /// <param name="inferDatas">干扰组分光谱处理后，一列X轴，多列Y轴</param>
        /// <param name="inferRows">干扰组分数据行数（包括X轴）</param>
        /// <param name="sampleDatas">待分析物光谱处理后，一列X轴，一列Y轴</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="coefMethod">相关方法，0=无权重, 1=强度, 2=强度平方, 3=|secondYDatas|/(specCols+1), 4=(first - second)的平方</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ADIdendify")]
        private static extern double ADIdendify64(double[] srcApiDatas, double[] apiDatas, double[] inferDatas, int inferRows, double[] sampleDatas, int specCols, int coefMethod);
        #endregion


        #region 外部调用接口，32bit和64bit自动适应

        /// <summary>
        /// 是否有PLS算法的授权
        /// </summary>
        public static bool HasAuthority()
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return HasAuthority64();
                else
                    return HasAuthority32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 光谱基线校正
        /// </summary>
        /// <param name="spectrumDatas">第一行为X数据，后面几行为Y数据</param>
        /// <param name="specRows">数据的行数</param>
        /// <param name="specCols">数据的列数</param>
        /// <param name="method">基线校正方法</param>
        /// <returns></returns>
        public static bool BaselineCorrect(double[] spectrumDatas, int specRows, int specCols, BaseLineMethod method)
        {
            if(spectrumDatas == null || spectrumDatas.Length != specRows*specCols)
            {
                ErrorString = "Invalid parameters";
                return false;
            }

            if (method == BaseLineMethod.None)
                return true;

            try
            {
                if (CommonMethod.Is64BitVersion())
                    return ADBaselineCorrect64(spectrumDatas, specRows, specCols, (int)method);
                else
                    return ADBaselineCorrect32(spectrumDatas, specRows, specCols, (int)method);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 光谱相似系数
        /// </summary>
        /// <param name="srcYDatas">光谱的Y轴数据</param>
        /// <param name="destYDatas">光谱的Y轴数据</param>
        /// <param name="coefMethod">系数计算方法</param>
        public static double Coefficient(double[] srcYDatas, double[] destYDatas, CoefficientMethod coefMethod)
        {
            if (srcYDatas == null || destYDatas == null || srcYDatas.Length == 0 || destYDatas.Length != srcYDatas.Length)
            {
                ErrorString = "Invalid parameters";
                return 0;
            }

            try
            {
                if (CommonMethod.Is64BitVersion())
                    return ADCoefficient64(srcYDatas, destYDatas, srcYDatas.Length, (int)coefMethod);
                else
                    return ADCoefficient32(srcYDatas, destYDatas, srcYDatas.Length, (int)coefMethod);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// AdvancedIdentify定性分析(光谱的数据点数量必须相同, X轴也必须相同)
        /// </summary>
        /// <param name="srcApiDatas">API原始光谱(未做基线或导数处理），一列X轴，一列Y轴</param>
        /// <param name="apiDatas">API原始光谱插值后，一列X轴，一列Y轴</param>
        /// <param name="inferDatas">干扰组分原始光谱插值后，一列X轴，多列Y轴</param>
        /// <param name="inferRows">干扰组分数据行数（包括X轴）</param>
        /// <param name="sampleDatas">待分析物原始光谱插值后，一列X轴，一列Y轴</param>
        /// <param name="coefMethod">相关系数计算方法</param>
        /// <returns></returns>
        public static double Identify(double[] srcApiDatas, double[] apiDatas, double[] inferDatas, int inferRows, double[] sampleDatas, CoefficientMethod coefMethod)
        {
            if(apiDatas == null || apiDatas.Length == 0 || (apiDatas.Length % 2 != 0))
            {
                ErrorString = "Invalid parameters";
                return 0;
            }

            //apiDatas包括一行X和一行Y
            int specCols = apiDatas.Length / 2;

            if (sampleDatas==null || apiDatas.Length != sampleDatas.Length ||
                inferDatas == null || inferRows < 2 || inferDatas.Length != inferRows * specCols)
            {
                ErrorString = "Invalid parameters";
                return 0;
            }

            try
            {
                if (CommonMethod.Is64BitVersion())
                    return ADIdendify64(srcApiDatas, apiDatas, inferDatas, inferRows, sampleDatas, specCols, (int)coefMethod);
                else
                    return ADIdendify32(srcApiDatas, apiDatas, inferDatas, inferRows, sampleDatas, specCols, (int)coefMethod);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// 光谱基线校正
        /// </summary>
        /// <param name="xyDataList">第一项是X，后面是Y</param>
        /// <param name="method">基线校正方法.</param>
        /// <returns></returns>
        public static List<double[]> BaselineCorrect(List<double[]> xyDataList, BaseLineMethod method)
        {
            if(xyDataList.Count < 2)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            var xyDatas = CommonMethod.CombineSpectrumDatas(xyDataList);
            if(xyDatas == null)
            {
                ErrorString = CommonMethod.ErrorString;
                return null;
            }

            if (BaselineCorrect(xyDatas, xyDataList.Count, xyDataList[0].Length, method) == false)
                return null;

            return CommonMethod.SplitSpectrumDatas(xyDatas, xyDataList.Count);
        }

        /// <summary>
        /// AdvancedIdentify定性分析(光谱的数据点数量必须相同, X轴也必须相同)
        /// </summary>
        /// <param name="apiList">API原始光谱插值后，一列X轴，一列Y轴</param>
        /// <param name="inferList">干扰组分原光谱插值后，一列X轴，多列Y轴</param>
        /// <param name="sampleList">待分析物原始光谱插值后，一列X轴，一列Y轴</param>
        /// <param name="coefMethod">相关方法，0为标准，1为增强</param>
        /// <param name="baseline">基线校正方法</param>
        /// <param name="derivatives">导数平滑方法(有基线校正，忽略导数平滑)</param>
        /// <param name="smoothPoints">平滑窗口大小(5-99)</param>
        /// <param name="polyDegree">导数的阶数,2,3,4</param>
        /// <param name="firstX">起始计算X轴，-1表示使用API光谱的firstX</param>
        /// <param name="lastX">结束计算X轴，-1表示使用API光谱的lastX</param>
        /// <returns></returns>
        public static double Identify(List<double[]> apiList, List<double[]> inferList, List<double[]> sampleList, CoefficientMethod coefMethod,
            BaseLineMethod baseline = BaseLineMethod.None, DerivativeMethod derivatives = DerivativeMethod.None, 
            int smoothPoints = 7, int polyDegree = 2, double firstX = double.MinValue, double lastX = double.MaxValue)
        {
            if(apiList.Count != 2 || inferList.Count < 2 || sampleList.Count != 2)
            {
                ErrorString = "Invalid parameters";
                return 0;
            }

            if (smoothPoints>99 || smoothPoints<2 || polyDegree<2 || polyDegree>4)
            {
                ErrorString = "Invalid parameters";
                return 0;
            }

            //需要截取一部分光谱区间
            if(firstX != double.MinValue && lastX != double.MaxValue)
            {
                apiList = CommonMethod.GetRangeData(apiList, firstX, lastX);
                inferList = CommonMethod.GetRangeData(inferList, firstX, lastX);
                sampleList = CommonMethod.GetRangeData(sampleList, firstX, lastX);
            }

            firstX = apiList[0][0];
            lastX = apiList[0][apiList[0].Length - 1];
            int dataCount = apiList[0].Length;

            //格式不相同，全部兼容到api光谱
            if(!CommonMethod.IsSameXDatas(inferList[0], firstX, lastX, dataCount))
            {
                inferList = Ai.Hong.Algorithm.PreProcessor.SplineCubicInterpolation(inferList, firstX, lastX, dataCount);
            }
            if (!CommonMethod.IsSameXDatas(sampleList[0], firstX, lastX, dataCount))
            {
                sampleList = Ai.Hong.Algorithm.PreProcessor.SplineCubicInterpolation(sampleList, firstX, lastX, dataCount);
            }

            //限定2种计算系数的方法，1=无权重, 2=强度
            coefMethod = coefMethod != CoefficientMethod.Normal ? CoefficientMethod.Weight : coefMethod;

            //拷贝为数组，好在Ai.Hong.Algorithm中处理
            var srcApiDatas = Ai.Hong.Algorithm.CommonMethod.CombineSpectrumDatas(apiList);
            var apiDatas = Ai.Hong.Algorithm.CommonMethod.CombineSpectrumDatas(apiList);
            var inferDatas = Ai.Hong.Algorithm.CommonMethod.CombineSpectrumDatas(inferList);
            var sampleDatas = Ai.Hong.Algorithm.CommonMethod.CombineSpectrumDatas(sampleList);

            if(apiDatas == null || inferDatas == null || sampleDatas == null)
            {
                ErrorString = CommonMethod.ErrorString;
                return 0;
            }

            //需要基线校正
            if (baseline != BaseLineMethod.None)
            {
                BaselineCorrect(apiDatas, apiList.Count, dataCount, baseline);
                BaselineCorrect(inferDatas, inferList.Count, dataCount, baseline);
                BaselineCorrect(sampleDatas, sampleList.Count, dataCount, baseline);
            }
            else if(derivatives != DerivativeMethod.None)    //导数平滑
            {
                coefMethod = coefMethod == CoefficientMethod.Normal ? coefMethod : CoefficientMethod.SquareWeight;   //纪南的程序里面，导数后，增强方法计算系数的方式不一样（使用强度平方）

                PreProcessor.SGSmoothAndDerivative(apiDatas, apiList.Count, dataCount, smoothPoints, polyDegree, (int)derivatives);
                PreProcessor.SGSmoothAndDerivative(inferDatas, inferList.Count, dataCount, smoothPoints, polyDegree, (int)derivatives);
                PreProcessor.SGSmoothAndDerivative(sampleDatas, sampleList.Count, dataCount, smoothPoints, polyDegree, (int)derivatives);

                //纪南的导数没有除以X轴步长
                double stepx = apiDatas[1] - apiDatas[0];
                for (int i = dataCount; i < apiDatas.Length; i++)
                    apiDatas[i] *= stepx;

                stepx = inferDatas[1] - inferDatas[0];
                for (int i = dataCount; i < inferDatas.Length; i++)
                    inferDatas[i] *= stepx;

                stepx = sampleDatas[1] - sampleDatas[0];
                for (int i = dataCount; i < sampleDatas.Length; i++)
                    sampleDatas[i] *= stepx;
            }

            return Identify(srcApiDatas, apiDatas, inferDatas, inferList.Count, sampleDatas, coefMethod);
        }
    }
}
