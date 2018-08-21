using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 100%线斜率测试(100% Line Slope Test)
    /// </summary>
    public class LineSlopeTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 计算的X位置
        /// </summary>
        [XmlElement]
        public List<System.Windows.Point> slopeX { get; set; }

        /// <summary>
        /// 计算的阈值范围
        /// </summary>
        [XmlElement]
        public List<System.Windows.Point> slopeThresold { get; set; }

        /// <summary>
        /// 检测到的结果
        /// </summary>
        [XmlIgnore]
        public List<System.Windows.Point> slopeResult { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public LineSlopeTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public LineSlopeTestInfo(bool createNew)
            : base("100%线斜率测试", "100% Line Slope Test", "%")
        {
            slopeX = new List<System.Windows.Point>()
            {
                new System.Windows.Point(4500, 4900),
                new System.Windows.Point(6000, 6400),
                new System.Windows.Point(7500, 7900),
                new System.Windows.Point(9000, 9400),
            };
            slopeThresold = new List<System.Windows.Point>()
            {
                new System.Windows.Point(98.0, 102.0),
                new System.Windows.Point(99.0, 101.0),
                new System.Windows.Point(99.0, 101.0),
                new System.Windows.Point(98.0, 102.0),
            };
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            var xDatas = SpectraDatas[0].xDatas;
            var yDatas = SpectraDatas[0].yDatas;

            slopeResult = new List<System.Windows.Point>();
            results = new List<double>();

            for (int i = 0; i < slopeX.Count; i++)
            {
                var rangeDatas = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { xDatas, yDatas }, slopeX[i].X, slopeX[i].Y);
                double max = rangeDatas[1].Max();
                double min = rangeDatas[1].Min();

                slopeResult.Add(new System.Windows.Point(min, max));
            }

            return IsValidResult();
        }

        /// <summary>
        /// 特别的结果判断
        /// </summary>
        /// <returns></returns>
        public new bool IsValidResult()
        {
            if (slopeResult == null || slopeResult.Count == 0 || slopeThresold == null || slopeThresold.Count != slopeResult.Count)
                return false;

            //计算各区间的最小最大Y值
            bool result = true;

            for (int i = 0; i < slopeX.Count; i++)
            {
                if (slopeResult[i].X < slopeThresold[i].X || slopeResult[i].Y > slopeThresold[i].Y)
                    result = false;
            }

            return result;
        }
    }

}
