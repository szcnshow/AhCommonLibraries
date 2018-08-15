using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 激光峰位校准
    /// </summary>
    public class LaserWavelengthTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 验证峰位
        /// </summary>
        [XmlElement]
        public List<double> VerifyPeaks { get; set; }

        /// <summary>
        /// 验证峰位阈值
        /// </summary>
        [XmlAttribute]
        public double VerifyPeakThreshold { get; set; }

        /// <summary>
        /// 设备激光波数
        /// </summary>
        [XmlAttribute]
        public double LaserLength { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public LaserWavelengthTestInfo()
        {
        }

        /// <summary>
        /// 创建新的测量参数
        /// </summary>
        /// <param name="createNew">新建参数</param>
        public LaserWavelengthTestInfo(bool createNew)
            :base("激光波数校准", "Laser Wavelength Correct", "cm-1")
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

            try
            {
                Trace.Assert(SpectraDatas != null && SpectraDatas.Count != 0 && VerifyPeaks != null && VerifyPeaks.Count == 2, "Invalid SpectraDatas or verifyPeaks");

                //判断目标峰位7181.68和验证峰位7232.29, 7242.77是否在阈值0.1内
                FinalResult = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, TargetResult, 4, out double picked, false);
                var verResult0 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, VerifyPeaks[0], 4, out picked, false);
                var verResult1 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, VerifyPeaks[1], 4, out picked, false);
                results.Add(FinalResult);
                results.Add(verResult0);
                results.Add(verResult1);

                if (!IsValidResult() ||
                    !IsValidResult(verResult0, VerifyPeaks[0], VerifyPeakThreshold, VerifyPeakThreshold) ||     //验证峰位阈值为1.0cm-1
                    !IsValidResult(verResult1, VerifyPeaks[1], VerifyPeakThreshold, VerifyPeakThreshold))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 调整激光频率
        /// </summary>
        public void AdjustLaserWavelength()
        {
            //FinalResult = laserLength * TargetResult / results[0];

            //Modify laser wavelength
        }
    }
}
