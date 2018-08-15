using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 100%线偏离测试(100% Line Deviation Test)
    /// </summary>
    public class DeviationTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public DeviationTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public DeviationTestInfo(bool createNew)
            : base("100%线偏离测试", "100% Line Deviation Test", "%")
        {
        }

        /// <summary>
        /// 计算结果
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

            FinalResult = results.Average();

            return IsValidResult();
        }
    }

}
