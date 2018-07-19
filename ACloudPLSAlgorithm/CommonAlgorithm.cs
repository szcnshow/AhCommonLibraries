using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Algorithm
{
    /// <summary>
    /// 通用光谱算法
    /// </summary>
    public static class CommonAlgorithm
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

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
        /// <param name="beginvalue">起始数据</param>
        /// <param name="endvalue">结束数据</param>
        private static void SortInOrder(ref int beginvalue, ref int endvalue)
        {
            if (beginvalue > endvalue)
            {
                int temp = endvalue;
                endvalue = beginvalue;
                beginvalue = temp;
            }
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        private static double LinearInterpolation(double interpX, double x1, double y1, double x2, double y2)
        {
            double k = (y2 - y1) / (x2 - x1);
            double b = y2 - k * x2;

            return interpX * k + b;
        }

        /// <summary>
        /// 计算光谱积分，积分方式与OPUS B 类型相同
        /// </summary>
        /// <param name="xData">X轴数据</param>
        /// <param name="yData">Y轴数据</param>
        /// <param name="freqStart">积分起始X值</param>
        /// <param name="freqEnd">积分结束X值</param>
        /// <param name="IsUpPeak">True=向上峰位，False=向下峰位</param>
        /// <returns>积分结果，错误范围NAN</returns>
        public static double Integrate(double[] xData, double[] yData, double freqStart, double freqEnd, bool IsUpPeak=true)
        {
            int beginIndex = FindNearestPosition(xData, 0, xData.Length - 1, freqStart);
            int endIndex = FindNearestPosition(xData, 0, xData.Length - 1, freqEnd);

            //xData范围内没有找到freqStart或freqEnd
            if (beginIndex == -1 || endIndex == -1)
            {
                ErrorString = "Data out of range";
                return double.NaN;
            }

            SortInOrder(ref beginIndex, ref endIndex);

            double[] x = new double[endIndex - beginIndex + 1];
            double[] y = new double[endIndex - beginIndex + 1];

            //如果是向下的峰位，临时反转过来
            double sign = IsUpPeak ? 1 : -1;
            for (int i = beginIndex; i <= endIndex; i++)
            {
                x[i - beginIndex] = xData[i];
                y[i - beginIndex] = yData[i] * sign;
            }

            alglib.spline1dinterpolant c;
            alglib.spline1dbuildcubic(x, y, out c);

            double value = alglib.spline1dintegrate(c, x[x.Length - 1]);

            //计算基线下面的面积
            double beginyvalue = alglib.spline1dcalc(c, freqStart);
            double endyvalue = alglib.spline1dcalc(c, freqEnd);
            double basevalue = Math.Abs((freqEnd - freqStart) * (endyvalue + beginyvalue) / 2);

            return value - basevalue;
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
                double x2 = foundPos == 0 ? double.MaxValue :  Math.Abs(xData[foundPos - 1] - valueToFind);
                double x3 = foundPos >= xData.Length-1 ? double.MaxValue : Math.Abs(xData[foundPos + 1] - valueToFind);

                if (x1 < x2 && x1 < x3)
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
        /// 计算光谱的RMS
        /// </summary>
        /// <param name="xData">X轴数据</param>
        /// <param name="yData">Y轴数据</param>
        /// <param name="freqStart">RMS起始X值</param>
        /// <param name="freqEnd">RMS结束X值</param>
        /// <returns>RMS值</returns>
        public static double CalculateRMS(double[] xData, double[] yData, double freqStart, double freqEnd)
        {
            int beginIndex = FindNearestPosition(xData, 0, xData.Length - 1, freqStart);
            int endIndex = FindNearestPosition(xData, 0, xData.Length - 1, freqEnd);

            //xData范围内没有找到freqStart或freqEnd
            if (beginIndex == -1 || endIndex == -1)
            {
                ErrorString = "Data out of range";
                return double.NaN;
            }

            SortInOrder(ref beginIndex, ref endIndex);

            double[] x = new double[endIndex - beginIndex];
            double[] y = new double[endIndex - beginIndex];
            for (int i = beginIndex; i < endIndex; i++)
            {
                x[i - beginIndex] = xData[i];
                y[i - beginIndex] = yData[i];
            }

            int info;
            alglib.spline1dinterpolant s;
            alglib.spline1dfitreport rep;
            double rho = 5;

            alglib.spline1dfitpenalized(x, y, 50, rho, out info, out s, out rep);

            //计算平均值
            double argY = 0;
            for (int i = 0; i < y.Length; i++)
            {
                double newy = alglib.spline1dcalc(s, x[i]);
                y[i] -= newy;
                argY += Math.Abs(y[i]);
            }
            argY = argY / y.Length;

            //计算平方和
            double sumY = 0;
            for (int i = 0; i < y.Length; i++)
                sumY += (y[i] - argY) * (y[i] - argY);

            double rms = Math.Sqrt(sumY / y.Length);

            return rms;
        }

        /// <summary>
        /// 计算光谱文件的信噪比SNR
        /// </summary>
        /// <param name="xDatas">X轴数据</param>
        /// <param name="yDatas">Y轴数据</param>
        /// <param name="signalStart">信号区间起始波数</param>
        /// <param name="signalEnd">信号区间结束波数</param>
        /// <param name="noiseStart">噪声区间起始波数</param>
        /// <param name="noiseEnd">噪声区间结束波数</param>
        /// <param name="signalInteValue">信号积分值</param>
        /// <param name="noiseInteValue">噪声积分值</param>
        /// <returns>信噪比SNR值, 如果返回NAN，表示出错了</returns>
        public static double CalulateSNR(double[] xDatas, double[] yDatas, double signalStart, double signalEnd, double noiseStart, double noiseEnd, out double noiseInteValue, out double signalInteValue)
        {
            noiseInteValue = signalInteValue = 0;
            if (xDatas == null || yDatas == null || xDatas.Length != yDatas.Length || xDatas.Length < 1 ||
                signalStart < signalEnd || noiseStart < noiseEnd )
            {
                ErrorString = "Invalid parameters";
                return double.NaN;
            }

            signalInteValue = Integrate(xDatas, yDatas, signalStart, signalEnd);
            if (signalInteValue == 0)
                return double.NaN;

            noiseInteValue = CalculateRMS(xDatas, yDatas, noiseStart, noiseEnd);
            if (double.IsNaN( noiseInteValue))
                return double.NaN;

            return (float)(signalInteValue / noiseInteValue);
        }

        /// <summary>
        /// 标定峰位
        /// </summary>
        /// <param name="xData">X轴数据</param>
        /// <param name="yData">Y轴数据</param>
        /// <param name="peakValue">要标定的峰位</param>
        /// <param name="pointsToCal">计算的点</param>
        /// <param name="newyvalue">返回峰位的峰高</param>
        /// <param name="isUpPeak">是否向上的峰</param>
        /// <returns>找到的峰位</returns>
        public static double PickPeak(double[] xData, double[] yData, double peakValue, int pointsToCal, out double newyvalue,bool isUpPeak=true)
        {
            //找到要标记的峰在xData中的最近位置
            newyvalue = 0;
            int foundpos = FindNearestPosition(xData, 0, xData.Length - 1, peakValue);
            if (foundpos == -1)  //没找到
                return -1;

            if (isUpPeak)    //正峰（峰在上面)
            {
                //查找该位置最近的峰
                double maxValue = yData[foundpos];
                if (foundpos > 0)
                {
                    if (yData[foundpos - 1] >= maxValue)    //下降曲线，峰在左边
                    {
                        while (foundpos > 0 && yData[foundpos - 1] > yData[foundpos])
                            foundpos--;
                    }
                    else    //上升曲线，峰在右边
                    {
                        while (foundpos < yData.Length - 1 && yData[foundpos + 1] > yData[foundpos])
                            foundpos++;
                    }
                }

                //foundpos是当前的峰位
                //现在要找峰谷
                int leftpos = foundpos, rightpos = foundpos;
                for (int i = foundpos; i > 1; i--)
                {
                    if (yData[i] < yData[i - 1])
                        break;
                    leftpos--;
                }
                for (int i = foundpos; i < yData.Length - 1; i++)
                {
                    if (yData[i] < yData[i + 1])
                        break;
                    rightpos++;
                }

                //调整左右必须在光谱区间内
                if (foundpos - leftpos > pointsToCal)
                    leftpos = foundpos - pointsToCal;
                if (leftpos < 0)
                    leftpos = 0;

                if (rightpos - foundpos > pointsToCal)
                    rightpos = foundpos + pointsToCal;
                if (rightpos > yData.Length - 1)
                    rightpos = yData.Length - 1;

                //取左右各pointsToCal个点来做曲线拟合
                double[] x = new double[rightpos - leftpos + 1];
                double[] y = new double[rightpos - leftpos + 1];
                for (int i = leftpos; i <= rightpos; i++)
                {
                    x[i - leftpos] = xData[i];
                    y[i - leftpos] = yData[i];
                }

                alglib.spline1dinterpolant c;
                alglib.spline1dbuildcubic(x, y, out c);

                //计算拟合后的Y值
                for (int i = 0; i < x.Length; i++)
                {
                    y[i] = alglib.spline1dcalc(c, x[i]);
                }

                //查找Y的最大值
                foundpos = 0;
                double maxy = y[0];
                for (int i = 1; i < y.Length; i++)
                {
                    if (y[i] > maxy)
                    {
                        foundpos = i;
                        maxy = y[i];
                    }
                }

                double stepx = (x[1] - x[0]) / 100;   //按照X最小间隔的1/100来逐步逼近
                maxy = y[foundpos];
                double maxx = x[foundpos];

                //先查左边
                double cury = alglib.spline1dcalc(c, x[foundpos] - stepx);
                if (cury > maxy)   //左边的Y值大于最大Y值，因此最大Y值还在左边
                    stepx *= -1;     //实际上每次计算要减小stepx

                for (int i = 1; i < 100; i++)
                {
                    cury = alglib.spline1dcalc(c, x[foundpos] + i * stepx);
                    if (cury > maxy)   //找到更大的Y值了
                    {
                        maxx = x[foundpos] + i * stepx;
                        maxy = cury;
                    }
                    else
                        break;
                }
                newyvalue = maxy;
                return maxx;
            }
            else    //倒峰(峰在下面)
            {
                //查找该位置最近的峰
                double maxValue = yData[foundpos];
                if (foundpos > 0)
                {
                    if (yData[foundpos - 1] <= maxValue)    //上升曲线，峰在左边
                    {
                        while (foundpos > 0 && yData[foundpos - 1] < yData[foundpos])
                            foundpos--;
                    }
                    else    //下降曲线，峰在右边
                    {
                        while (foundpos < yData.Length - 1 && yData[foundpos + 1] < yData[foundpos])
                            foundpos++;
                    }
                }
                //foundpos是当前的峰位
                //现在要找两边的峰顶
                int leftpos = foundpos, rightpos = foundpos;
                for (int i = foundpos; i > 1; i--)   //找左边的峰顶
                {
                    if (yData[i - 1] < yData[i])
                        break;
                    leftpos--;
                }
                for (int i = foundpos; i < yData.Length - 1; i++)   //找右边的峰顶
                {
                    if (yData[i + 1] < yData[i])
                        break;
                    rightpos++;
                }

                //调整左右必须在光谱区间内
                if (foundpos - leftpos > pointsToCal)
                    leftpos = foundpos - pointsToCal;
                if (leftpos < 0)
                    leftpos = 0;

                if (rightpos - foundpos > pointsToCal)
                    rightpos = foundpos + pointsToCal;
                if (rightpos > yData.Length - 1)
                    rightpos = yData.Length - 1;

                //取左右各pointsToCal个点来做曲线拟合
                double[] x = new double[rightpos - leftpos + 1];
                double[] y = new double[rightpos - leftpos + 1];
                for (int i = leftpos; i <= rightpos; i++)
                {
                    x[i - leftpos] = xData[i];
                    y[i - leftpos] = yData[i];
                }

                alglib.spline1dinterpolant c;
                alglib.spline1dbuildcubic(x, y, out c);

                //计算拟合后的Y值
                for (int i = 0; i < x.Length; i++)
                {
                    y[i] = alglib.spline1dcalc(c, x[i]);
                }

                //查找Y的最小值
                foundpos = 0;
                double maxy = y[0];
                for (int i = 1; i < y.Length; i++)
                {
                    if (y[i] < maxy)
                    {
                        foundpos = i;
                        maxy = y[i];
                    }
                }

                double stepx = (x[1] - x[0]) / 100;   //按照X最小间隔的1/100来逐步逼近
                maxy = y[foundpos];
                double maxx = x[foundpos];

                //先查左边, 如果最小值在左边，stepx为负数
                double cury = alglib.spline1dcalc(c, x[foundpos] - stepx);
                if (cury < maxy)   //左边的Y值小于最小Y值，因此最小Y值还在左边
                    stepx *= -1;     //实际上每次计算要减小stepx

                for (int i = 1; i < 100; i++)
                {
                    cury = alglib.spline1dcalc(c, x[foundpos] + i * stepx);
                    if (cury < maxy)   //找到更小的Y值了
                    {
                        maxx = x[foundpos] + i * stepx;
                        maxy = cury;
                    }
                    else
                        break;
                }
                newyvalue = maxy;
                return maxx;
            }
        }

        /// <summary>
        /// 获取波数精确度
        /// </summary>
        /// <param name="pickedPeaks">测量得到的峰位</param>
        /// <returns>波数精确度</returns>
        public static double XAxisPrecision(List<double[]> pickedPeaks)
        {
            if (pickedPeaks == null || pickedPeaks.Count == 0)
            {
                ErrorString = "Invalid parameters";
                return double.NaN;
            }
            int peakCount = pickedPeaks[0].Length;
            foreach (var item in pickedPeaks)
            {
                if (item == null || item.Length != peakCount)
                {
                    ErrorString = "Invalid parameters";
                    return double.NaN;
                }
            }

            double[] preciDatas = new double[peakCount];
            double maxValue = double.MinValue;
            double minValue = double.MaxValue;

            for (int i = 0; i < peakCount; i++)    //按每个峰位处理
            {
                //每张光谱当前峰位的最大值和最小值
                maxValue = (from p in pickedPeaks select p[i]).Max();
                minValue = (from p in pickedPeaks select p[i]).Min();

                //波数精度 =MAX(B2:F2)-MIN(B2:F2)
                preciDatas[i] = maxValue - minValue;
            }

            //波数精度验证公式：MAX(H2:H12)
            return preciDatas.Max();
        }

        /// <summary>
        /// 波数准确度
        /// </summary>
        /// <param name="standardPeaks">标准峰位</param>
        /// <param name="pickedPeaks">测量得到的峰位</param>
        /// <returns>波数准确度</returns>
        public static double XAxisAccuracy(double[] standardPeaks, List<double[]> pickedPeaks)
        {
            if (standardPeaks == null || standardPeaks.Length == 0 || 
                pickedPeaks == null || pickedPeaks.Count == 0 || pickedPeaks[0].Length != standardPeaks.Length)
            {
                ErrorString = "Invalid parameters";
                return double.NaN;
            }
            int peakCount = pickedPeaks[0].Length;
            foreach (var item in pickedPeaks)
            {
                if (item == null || item.Length != peakCount)
                {
                    ErrorString = "Invalid parameters";
                    return double.NaN;
                }
            }

            //计算所有光谱中的相同峰位的值
            double[] accDatas = new double[peakCount];       //准确度
            double maxValue = double.MinValue;
            double minValue = double.MaxValue;

            for (int i = 0; i < peakCount; i++)    //处理每个峰位
            {
                //每张光谱当前峰位的最大值和最小值
                maxValue = (from p in pickedPeaks select p[i]).Max();
                minValue = (from p in pickedPeaks select p[i]).Min();

                //波数准确度 IF(AVERAGE(MAX(B2:F2),MIN(B2:F2))>=A2,MAX(B2:F2)-A2,MIN(B2:F2)-A2)
                //平均值大于标准值，取最大值，否则取最小值
                accDatas[i] = ((maxValue + minValue) / 2.0f) >= standardPeaks[i] ? maxValue : minValue;
                accDatas[i] -= standardPeaks[i];
            }

            //波数准确度验证结果的公式：MAX(ABS(G2),ABS(G3),ABS(G4),ABS(G5),ABS(G6),ABS(G7),ABS(G8),ABS(G9),ABS(G10),ABS(G11),ABS(G12)))
            double sureResultValue = (from p in accDatas select Math.Abs(p)).Max();
            return sureResultValue;
        }

        /// <summary>
        /// 计算相对强度(测量区间积分 / 基础区间积分)
        /// </summary>
        /// <param name="xDatas">X轴数据</param>
        /// <param name="yDatas">Y轴数据</param>
        /// <param name="baseRegion">基础区间</param>
        /// <param name="intensityRegion">测量区间</param>
        /// <returns>相对强度</returns>
        public static double[] GetRelativeIntensity(double[] xDatas, double[] yDatas, Tuple<double, double> baseRegion, List<Tuple<double, double>> intensityRegion)
        {
            if(baseRegion == null || intensityRegion == null || intensityRegion.Count == 0)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            //基础范围的积分
            double baseValue = Integrate(xDatas, yDatas, baseRegion.Item1, baseRegion.Item2);

            //测量区间积分 / 基础范围积分
            double[] retDatas = new double[intensityRegion.Count];
            for (int i = 0; i < intensityRegion.Count; i++)
            {
                retDatas[i] = Integrate(xDatas, yDatas, intensityRegion[i].Item1, intensityRegion[i].Item2);
                retDatas[i] = retDatas[i] / baseValue;
            }

            return retDatas;
        }

        /// <summary>
        /// 计算参考值和预测值之间的RMSECV
        /// </summary>
        /// <param name="predictValues">预测值</param>
        /// <param name="referenceValues">参考值</param>
        /// <returns></returns>
        public static double CalculateRMSECV(double[] predictValues, double[] referenceValues)
        {
            if (predictValues == null || predictValues.Length == 0 || referenceValues == null || referenceValues.Length != predictValues.Length)
                return double.NaN;

            double retdata = 0;
            for (int i = 0; i < predictValues.Length; i++)
                retdata += (predictValues[i] - referenceValues[i]) * (predictValues[i] - referenceValues[i]);

            return Math.Sqrt(retdata / predictValues.Length);
        }

        /// <summary>
        /// 获取平均光谱
        /// </summary>
        /// <param name="specDatas">光谱数据（Y轴）</param>
        /// <param name="specRows">光谱的行数</param>
        /// <param name="specCols">每行光谱的数据点数</param>
        /// <param name="withXDatas">是否包含X轴数据（如果包含，需要跳过）</param>
        /// <returns></returns>
        public static double[] GetAverageSpectrum(double[] specDatas, int specRows, int specCols, bool withXDatas=false)
        {
            if(withXDatas && specRows < 2)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            if (specDatas == null || specRows < 1 || specCols < 1 || specDatas.Length != specRows * specCols)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            double[] retdatas = new double[specCols];
            for (int col = 0; col < specCols; col++)
            {
                double sum = 0;
                for (int row = withXDatas ? 1 : 0; row < specRows; row++)   //如果有X轴，跳过
                {
                    sum += specDatas[row * specCols + col];
                }
                retdatas[col] = sum / (withXDatas ? specRows - 1 : specRows);
            }

            return retdatas;
        }

        /// <summary>
        /// 计算光谱的平均值
        /// </summary>
        /// <param name="datas">光谱数据</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">数据数量</param>
        /// <returns>光谱平均值</returns>
        public static double GetAverageValue(double[] datas, int start, int count)
        {
            double avg = 0;
            for (int i = 0; i < count; i++)
                avg += datas[i + start];

            return avg / count;
        }

        /// <summary>
        /// 计算标准方差
        /// </summary>
        /// <param name="datas">光谱数据</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">数据数量</param>
        /// <returns>标准方差</returns>
        public static double GetVariance(double[] datas, int start, int count)
        {
            double avg = GetAverageValue(datas, start, count);

            double total = 0;
            for (int i = 0; i < count; i++)
                total += (datas[i + start] - avg) * (datas[i + start] - avg);

            return Math.Sqrt(total / (count - 1));
        }

        /// <summary>
        /// 计算协方差
        /// </summary>
        /// <param name="data1">光谱数据1</param>
        /// <param name="start1">开始位置1</param>
        /// <param name="data2">光谱数据2</param>
        /// <param name="start2">开始位置2</param>
        /// <param name="count">数据数量</param>
        /// <returns>协方差</returns>
        public static double GetCovariance(double[] data1, int start1, double[] data2, int start2, int count)
        {
            double avg1 = GetAverageValue(data1, start1, count);
            double avg2 = GetAverageValue(data2, start2, count);
            double total = 0;

            for (int i = 0; i < count; i++)
                total += (data1[i + start1] - avg1) * (data2[i + start2] - avg2);

            return total / (count - 1);
        }

        /// <summary>
        /// 计算光谱间的相关系数
        /// </summary>
        /// <param name="data1">光谱数据1</param>
        /// <param name="start1">开始位置1</param>
        /// <param name="data2">光谱数据2</param>
        /// <param name="start2">开始位置2</param>
        /// <param name="count">数据数量</param>
        /// <returns>相关系数</returns>
        public static double CorrelationCoefficient(double[] data1, int start1, double[] data2, int start2, int count)
        {
            double variance1 = GetVariance(data1, start1, count);
            double variance2 = GetVariance(data2, start2, count);
            double covariance = GetCovariance(data1, start1, data2, start2, count);

            return covariance / (variance1 * variance2);
        }

        /// <summary>
        /// 计算光谱间的欧氏距离
        /// </summary>
        /// <param name="data1">光谱数据1</param>
        /// <param name="start1">开始位置1</param>
        /// <param name="data2">光谱数据2</param>
        /// <param name="start2">开始位置2</param>
        /// <param name="count">数据数量</param>
        /// <returns>欧氏距离</returns>
        public static double EuclideanDistance(double[] data1, int start1, double[] data2, int start2, int count)
        {
            if (count > data1.Length - start1 || count > data2.Length - start2)
                return -1;

            //公式 d=sqrt(sum(((y1(n)-y2(n))^2))
            double r = 0;
            for (int i = 0; i < count; i++)
                r += (data1[i + start1] - data2[i + start2]) * (data1[i + start1] - data2[i + start2]);

            return Math.Sqrt(r);
        }

        /// <summary>
        /// 计算RMSEP
        /// </summary>
        /// <param name="refDatas">参考值</param>
        /// <param name="predictDatas">预测值</param>
        /// <returns></returns>
        public static double CalculteRMSEP(double[] refDatas, double[] predictDatas)
        {
            if (refDatas == null || predictDatas == null || refDatas.Length == 0 || refDatas.Length != predictDatas.Length)
            {
                ErrorString = "Invalid parameters";
                return double.NaN;
            }

            double retdata = 0;
            for (int i = 0; i < refDatas.Length; i++)
            {
                retdata += (predictDatas[i] - refDatas[i]) * (predictDatas[i] - refDatas[i]);
            }

            return retdata;
        }

        /// <summary>
        /// 使用样品单通道谱和背景单通道谱计算样品吸收谱
        /// </summary>
        /// <param name="bkYDatas">背景单通道谱</param>
        /// <param name="sampleYDatas">样品单通道谱</param>
        /// <returns>吸收谱</returns>
        public static double[] CalculateAbsorb(double[] bkYDatas, double[] sampleYDatas)
        {
            System.Diagnostics.Trace.Assert(bkYDatas != null && bkYDatas.Length != 0 && sampleYDatas != null && sampleYDatas.Length == bkYDatas.Length, "Invalid parameters");

            double[] retDatas = new double[bkYDatas.Length];

            for (int index = 0; index < bkYDatas.Length; index++)
            {
                //比值 = 样品谱 / 背景谱
                retDatas[index] = bkYDatas[index] == 0 ? 1 : Math.Abs(sampleYDatas[index] / bkYDatas[index]);

                //计算Log10(1/比值)
                retDatas[index] = Math.Log10(1 / retDatas[index]);
            }

            return retDatas;
        }

        /// <summary>
        /// 创建透射光谱
        /// </summary>
        /// <param name="bkYDatas">背景单通道谱</param>
        /// <param name="sampleYDatas">样品单通道谱</param>
        /// <returns>透射光谱</returns>
        public static double[] CalculateTransimit(double[] bkYDatas, double[] sampleYDatas)
        {
            System.Diagnostics.Trace.Assert(bkYDatas != null && bkYDatas.Length != 0 && sampleYDatas != null && sampleYDatas.Length == bkYDatas.Length, "Invalid parameters");

            double[] retDatas = new double[bkYDatas.Length];

            for (int index = 0; index < bkYDatas.Length; index++)
            {
                //比值 = 样品谱 / 背景谱
                retDatas[index] = bkYDatas[index] == 0 ? 100 : Math.Abs(sampleYDatas[index] / bkYDatas[index])*100;
            }

            return retDatas;
        }

    }

}
