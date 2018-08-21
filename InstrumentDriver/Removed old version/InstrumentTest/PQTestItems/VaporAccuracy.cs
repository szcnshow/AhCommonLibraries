using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 水蒸气峰位精度（Wavenumber Accuracy Test）
    /// </summary>
    public class VaporAccuracyTestInfo : BaseSelfTestInfo
    {
        private List<double> _verifyPeaks = null;
        /// <summary>
        /// 验证峰位
        /// </summary>
        [XmlElement]
        public List<double> VerifyPeaks { get { return _verifyPeaks; } set { _verifyPeaks = value; DoPropertyChanged("verifyPeaks"); } }

        private double _laserLength { get; set; }
        /// <summary>
        /// 设备激光波数
        /// </summary>
        [XmlAttribute]
        public double LaserLength { get { return _laserLength; } set { _laserLength = value; DoPropertyChanged("layerLength"); } }


        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public VaporAccuracyTestInfo()
        {
        }

        /// <summary>
        /// 创建新的测量参数
        /// 不同类型的仪器分别设置
        /// </summary>
        /// <param name="createNew">仪器品牌</param>
        public VaporAccuracyTestInfo(bool createNew) :
            base("水蒸气峰位测试", "Vapor peak accuracy test", "cm-1")
        {
            this._laserLength = 15798.620000;
            this._verifyPeaks = new List<double>() { 7232.29, 7242.77 };
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
                System.Diagnostics.Trace.Assert(SpectraDatas != null && SpectraDatas.Count != 0 && VerifyPeaks != null && VerifyPeaks.Count == 2, "Invalid SpectraDatas or verifyPeaks");

                double verifyThresold = 0.5;

                //判断目标峰位7181.68和验证峰位7232.29, 7242.77是否在阈值0.1内
                FinalResult = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, TargetResult, 4, out double picked, false);
                var verResult0 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, VerifyPeaks[0], 4, out picked, false);
                var verResult1 = Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, VerifyPeaks[1], 4, out picked, false);
                if (!IsValidResult() ||
                    !IsValidResult(verResult0, VerifyPeaks[0], verifyThresold, verifyThresold) ||
                    !IsValidResult(verResult1, VerifyPeaks[1], verifyThresold, verifyThresold))
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
            double newvalue = LaserLength * results[0] / TargetResult;

            //Modify laser wavelength
        }
    }
}
