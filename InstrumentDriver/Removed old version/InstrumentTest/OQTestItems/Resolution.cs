using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Windows;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 分辨率测试
    /// </summary>
    public class ResolutionTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 计算分辨率的峰位
        /// </summary>
        [XmlElement]
        public double ResolutionPeak { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public ResolutionTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="createNew"></param>
        public ResolutionTestInfo(bool createNew) :
            base("分辨率测试", "Resolution Test", "cm-1")
        {
        }

        private bool InRange(double range1, double range2, double value)
        {
            if (range1 < range2)
            {
                return value >= range1 && value <= range2;
            }
            else
            {
                return value <= range1 && value >= range2;
            }
        }

        /// <summary>
        /// 计算两点直线的斜率和偏移
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns>X=k, Y=b</returns>
        private Point GetLineSlopAndBias(Point pt1, Point pt2)
        {
            //计算直线的k和b
            double k1 = (pt1.Y - pt2.Y) / (pt1.X - pt2.X);
            double b1 = pt1.Y - k1 * pt1.X;

            return new Point(k1, b1);
        }

        /// <summary>
        /// 判断两条直线是否相交，返回交点
        /// </summary>
        /// <param name="line1Pt1">直线1端点1</param>
        /// <param name="line1Pt2">直线1端点2</param>
        /// <param name="line2Pt1">直线2端点1</param>
        /// <param name="line2Pt2">直线2端点2</param>
        /// <param name="crossPt">Out 交点</param>
        /// <returns></returns>
        private bool GetLineCrossPoint(Point line1Pt1, Point line1Pt2, Point line2Pt1, Point line2Pt2, out Point crossPt)
        {
            //计算直线的k和b
            var temp = GetLineSlopAndBias(line1Pt1, line1Pt2);
            double k1 = temp.X;
            double b1 = temp.Y;

            temp = GetLineSlopAndBias(line2Pt1, line2Pt2);
            double k2 = temp.X;
            double b2 = temp.Y;

            double crossX = (b2 - b1) / (k1 - k2);
            double crossY = k1 * crossX + b1;

            crossPt = new Point(crossX, crossY);

            return InRange(line1Pt1.X, line1Pt2.X, crossX) && InRange(line1Pt1.Y, line1Pt2.Y, crossY) &&
                InRange(line2Pt1.X, line2Pt2.X, crossX) && InRange(line2Pt1.Y, line2Pt2.Y, crossY);
        }

        /// <summary>
        /// 计算半高宽线
        /// </summary>
        /// <param name="basePt1">基线端点1</param>
        /// <param name="basePt2">基线端点2</param>
        /// <param name="peakPt">峰位点</param>
        /// <returns></returns>
        private List<Point> GetHalfPeakLine(Point basePt1, Point basePt2, Point peakPt)
        {
            //计算基线斜率和偏移
            var temp = GetLineSlopAndBias(basePt1, basePt2);
            double k1 = temp.X;
            double b1 = temp.Y;

            //求峰位垂线与基线的交点 y = kx+b
            double centerY = peakPt.X * k1 + b1;

            //峰位垂线的Y轴中点
            centerY = centerY + (peakPt.Y - centerY) / 2;

            //半高宽线与基线平行，k不变, 计算半高宽的偏移 b=y-kx
            b1 = centerY - k1 * peakPt.X;

            //假设半高宽线是很长的一条线(x = (-10000000 = 10000000)
            double x1 = 100000000.0;
            return new List<Point>()
            {
                new Point(-x1, -x1 * k1+b1),
                new Point(x1, x1 * k1+b1),
            };
        }

        /// <summary>
        /// 计算中点
        /// </summary>
        /// <param name="firstX"></param>
        /// <param name="firstY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <returns></returns>
        private string CalMidPoint(double firstX, double firstY, double endX, double endY, double targetX, double targetY)
        {

            if (firstY == endY)
            {
                return ((firstX + endX) / 2).ToString() + "+" + ((targetY + firstY) / 2).ToString();
            }
            else
            {
                double temp = targetX * (firstX - endX) * (firstX - endX) - (endY * targetY - firstY * targetY) * (firstX - endX) + (firstX * endY - firstY * endX) * (endY - firstY);
                double x = temp / ((endY - firstY) * (endY - firstY) + (endX - firstX) * (endX - firstX));
                double y = (firstX * x - endX * x + endX * targetX - firstX * targetX - firstY * targetY + endY * targetY) / (endY - firstY);
                return ((x + targetX) / 2).ToString() + "+" + ((y + targetY) / 2).ToString();
            }
        }

        //private double CalMidPoint(double x1, double y1, double x2, double y2, double peakX, double peakY)
        //{
        //    double midx, midy;

        //    if (y1 == y2)
        //    {
        //        midx = x1 + (x2 - x1) / 2;
        //        midy = y1 + (peakY - y1) / 2;
        //    }
        //    else
        //    {
        //        double temp = targetX * (firstX - endX) * (firstX - endX) - (endY * targetY - firstY * targetY) * (firstX - endX) + (firstX * endY - firstY * endX) * (endY - firstY);
        //        double x = temp / ((endY - firstY) * (endY - firstY) + (endX - firstX) * (endX - firstX));
        //        double y = (firstX * x - endX * x + endX * targetX - firstX * targetX - firstY * targetY + endY * targetY) / (endY - firstY);
        //        return ((x + targetX) / 2).ToString() + "+" + ((y + targetY) / 2).ToString();
        //    }
        //}

        ///// <summary>
        ///// 分辨率计算
        ///// </summary>
        ///// <param name="firstX"></param>
        ///// <param name="firstY"></param>
        ///// <param name="endX"></param>
        ///// <param name="endY"></param>
        ///// <param name="midPointX"></param>
        ///// <param name="midPointY"></param>
        ///// <param name="DataX"></param>
        ///// <param name="DataY"></param>
        ///// <param name="IsUpPeak">True=向上的峰，False=向下的峰</param>
        ///// <returns></returns>
        //private double CalResolution(double firstX, double firstY, double endX, double endY, double midPointX, double midPointY, double[] DataX, double[] DataY, bool IsUpPeak)
        //{
        //    double Resolution = Ai.Hong.Algorithm.CommonAlgorithm.Integrate(DataX, DataY, firstX, endX, IsUpPeak); 
        //    int tar = Ai.Hong.Algorithm.CommonMethod.FindNearestPosition(DataX, 0, DataX.Length - 1, resolutionPeak);
        //    double tary;
        //    double tarx = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(DataX, DataY, resolutionPeak, 4, out tary, true);
        //    string hi = CalMidPoint(firstX, firstY, endX, endY, tarx, tary);
        //    string[] tt = hi.Split('+');
        //    double xMid = Convert.ToDouble(tt[0]);
        //    double yMid = Convert.ToDouble(tt[1]);
        //    double tem = (xMid - tarx) * (xMid - tarx) + (yMid - tary) * (yMid - tary);
        //    //峰高
        //    double PickHeight = 2 * Math.Sqrt(tem);
        //    Resolution = Resolution / PickHeight;
        //    return Resolution;
        //}

        ///// <summary>
        ///// 计算测试结果
        ///// </summary>
        ///// <param name="calcuParameter">bool, True=向上的峰，False=向下的峰</param>
        ///// <returns></returns>
        //public override bool CalculateResult(dynamic calcuParameter)
        //{
        //    var xDatas = SpectraDatas[0].xDatas;
        //    var yDatas = SpectraDatas[0].yDatas;

        //    //计算 - log
        //    //double[] yDatas = new double[SpectraDatas[0].yDatas.Length];
        //    //for (int i = 0; i < yDatas.Length; i++)
        //    //    yDatas[i] = -Math.Log10(SpectraDatas[0].yDatas[i]);

        //    //标定峰位（向上的峰位）
        //    bool IsUpPeak = (bool)calcuParameter;
        //    double newYValue;
        //    double targetPeak = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, resolutionPeak, 4, out newYValue, IsUpPeak);
        //    double newStartY;
        //    double newStartX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, resolutionPeak - 2, 4, out newStartY, !IsUpPeak);
        //    double newEndY;
        //    double newEndX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, resolutionPeak + 2, 4, out newEndY, !IsUpPeak);

        //    string midPoint = CalMidPoint(newStartX, newStartY, newEndX, newEndY, targetPeak, newYValue);
        //    string[] mid = midPoint.Split('+');
        //    double midPointX = Convert.ToDouble(mid[0]);
        //    double midPointY = Convert.ToDouble(mid[1]);

        //    int midX = Ai.Hong.Algorithm.CommonMethod.FindNearestPosition(xDatas, 0, xDatas.Length - 1, midPointX);
        //    FinalResult = CalResolution(newStartX, newStartY, newEndX, newEndY, midPointX, midPointY, xDatas, yDatas, IsUpPeak);

        //    return IsValidResult();
        //}


        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter">bool, True=向上的峰，False=向下的峰</param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter)
        {
            var xDatas = SpectraDatas[0].xDatas;
            var yDatas = SpectraDatas[0].yDatas;

            //标定峰位
            bool IsUpPeak = (bool)calcuParameter;
            double peakX = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, ResolutionPeak, 4, out double peakY, IsUpPeak);
            double basePt1X = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, ResolutionPeak - 2, 4, out double basePt1Y, !IsUpPeak);
            double basePt2X = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(xDatas, yDatas, ResolutionPeak + 2, 4, out double basePt2Y, !IsUpPeak);

            var midLine = GetHalfPeakLine(new Point(basePt1X, basePt1Y), new Point(basePt2X, basePt2Y), new Point(peakX, peakY));

            //找到峰位的数据区间
            int baseIndex1 = Algorithm.CommonAlgorithm.FindNearestPosition(xDatas, 0, xDatas.Length - 1, basePt1X);
            int baseIndex2 = Algorithm.CommonAlgorithm.FindNearestPosition(xDatas, 0, xDatas.Length - 1, basePt2X);
            int peakIndex = Algorithm.CommonAlgorithm.FindNearestPosition(xDatas, 0, xDatas.Length - 1, peakX);

            int startx, endx;

            //一边的峰位
            startx = baseIndex1 < peakIndex ? baseIndex1 : peakIndex;
            endx = baseIndex1 < peakIndex ? peakIndex : baseIndex1;

            Point crossPt1 = new Point(0, 0);

            for (int i = startx; i < endx; i++)
            {
                if (GetLineCrossPoint(new Point(xDatas[i], yDatas[i]), new Point(xDatas[i + 1], yDatas[i + 1]), midLine[0], midLine[1], out crossPt1) == true)
                {
                    break;
                }
            }

            //另一边的峰位
            startx = baseIndex2 < peakIndex ? baseIndex2 : peakIndex;
            endx = baseIndex2 < peakIndex ? peakIndex : baseIndex2;

            Point crossPt2 = new Point(0, 0);

            for (int i = startx; i < endx; i++)
            {
                if (GetLineCrossPoint(new Point(xDatas[i], yDatas[i]), new Point(xDatas[i + 1], yDatas[i + 1]), midLine[0], midLine[1], out crossPt2) == true)
                {
                    break;
                }
            }

            //计算半高线两边交点的间的长度
            FinalResult = Math.Sqrt((crossPt1.X - crossPt2.X) * (crossPt1.X - crossPt2.X) + (crossPt1.Y - crossPt2.Y) * (crossPt1.Y - crossPt2.Y));

            results = new List<double>() { FinalResult, peakX, peakY, basePt1X, basePt1Y, basePt2X, basePt2Y, midLine[0].X, midLine[0].Y, midLine[1].X, midLine[1].Y, crossPt1.X, crossPt1.Y, crossPt2.X, crossPt2.Y };
            return IsValidResult();
        }
    }

}
