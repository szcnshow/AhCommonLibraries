using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 波数重现性测试（Wavenumber Reproducibility Test）
    /// </summary>
    public class WavenumberReproductTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 验证峰位
        /// </summary>
        [XmlElement]
        public double VerifyPeak { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public WavenumberReproductTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数Thresold ±0.1
        /// </summary>
        /// <param name="createNew"></param>
        public WavenumberReproductTestInfo(bool createNew) :
            base("波数重现性测试", "Wavenumber Reproducibility Test", "cm-1")
        {
            VerifyPeak = 7181.68;
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter"></param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter = null)
        {
            results = new List<double>();

            foreach (var data in SpectraDatas)
            {
                //判断目标峰位7181.68
                var curpeak = Algorithm.CommonAlgorithm.PickPeak(data.xDatas, data.yDatas, VerifyPeak, 4, out double picked, false);
                results.Add(curpeak);
            }

            //获取最大偏离值
            FinalResult = results.Max() - results.Min();

            return IsValidResult();
        }
    }
}
