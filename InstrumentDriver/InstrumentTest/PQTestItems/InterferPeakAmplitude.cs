using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 干涉图峰位振幅测试（Long Term Stability Test, Interferogram Peak Amplitude Test）
    /// </summary>
    public class InterferPeakTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public InterferPeakTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数(TargetResult = 参考光谱=100% Thresold>70%
        /// </summary>
        public InterferPeakTestInfo(bool createNew)
            : base("干涉图振幅测试", "Interferogram Peak Amplitude Test", "%")
        {
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="calcuParameter">参考干涉图</param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter)
        {
            results = new List<double>();

            //参考干涉图的振幅
            FileFormat.FileFormat referenceData = new FileFormat.FileFormat(calcuParameter as string);
            if (referenceData.xDatas == null)
            {
                ErrorString = "Cannot find reference file:" + ReferenceFile;
                return false;
            }
            PickMaxMinPeak(referenceData.xDatas, referenceData.yDatas, out double minX, out double minY, out double maxX, out double maxY);
            var refValue = Math.Abs(maxY - minY);

            foreach (var data in SpectraDatas)
            {
                //标注最大最小峰位并记录
                PickMaxMinPeak(data.xDatas, data.yDatas, out minX, out minY, out maxX, out maxY);

                results.Add(Math.Abs(maxY - minY) * 100 / refValue);    //变成%
            }
            FinalResult = results.Average();

            return IsValidResult();
        }
    }

}
