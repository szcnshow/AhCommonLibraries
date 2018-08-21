using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 透射重复性测试(Transmittance Reproducibility)
    /// </summary>
    public class TransmitReproductTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public TransmitReproductTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public TransmitReproductTestInfo(bool createNew)
            : base("透射重复性测试", "Transmittance Reproducibility Test", "%")
        {
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();
            foreach (var spcdata in SpectraDatas)
            {
                var rangeDatas = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { spcdata.xDatas, spcdata.yDatas }, firstX, lastX);
                var yDatas = rangeDatas[1];

                //偏差计算（YDatas)
                double max = Math.Abs(yDatas.Max() - 100);
                double min = Math.Abs(yDatas.Min() - 100);

                results.Add(max > min ? max : min);
            }

            FinalResult = results.Max();

            return IsValidResult();
        }
    }

}
