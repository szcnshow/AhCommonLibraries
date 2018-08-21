using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 能量测试（Energy Test）
    /// </summary>
    public class EnergyTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public EnergyTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold 《 30%
        /// </summary>
        public EnergyTestInfo(bool createNew)
            : base("光源能量衰减测试", "Source Energy Attenuation Test","%")
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

            //参考单通道背景图的积分
            FileFormat.FileFormat referenceData = new FileFormat.FileFormat(calcuParameter as string);
            if (referenceData.xDatas == null)
            {
                ErrorString = "Cannot find reference file:" + ReferenceFile;
                return false;
            }
            var refValue = Ai.Hong.Algorithm.CommonAlgorithm.Integrate(referenceData.xDatas, referenceData.yDatas, firstX, lastX);

            //当前测量得到的单通道背景图的积分，计算与原有参考光谱积分的差
            foreach (var data in SpectraDatas)
            {
                var scanvalue = Ai.Hong.Algorithm.CommonAlgorithm.Integrate(data.xDatas, data.yDatas, firstX, lastX);
                results.Add(Math.Abs(refValue - scanvalue) / refValue);
            }

            FinalResult = results.Average() * 100;

            //平均差小于30%为合格
            return IsValidResult();
        }
    }
}
