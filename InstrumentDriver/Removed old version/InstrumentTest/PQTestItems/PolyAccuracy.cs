using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 聚苯乙烯峰位测试，先扫描背景光谱，然后将IVU转换到聚苯乙烯，扫描带有聚苯乙烯的背景光谱，计算聚苯乙烯的吸收谱
    /// </summary>
    public class PolyAccuracyTestInfo : BaseSelfTestInfo
    {
        private Driver.EnumDeviceIVU _IVUFilter = Driver.EnumDeviceIVU.Polystyrene;
        /// <summary>
        /// 验证轮位置
        /// </summary>
        [XmlAttribute]
        public Driver.EnumDeviceIVU IVUFilter { get { return _IVUFilter; } set { _IVUFilter = value; } }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public PolyAccuracyTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数Thresold±0.5
        /// </summary>
        public PolyAccuracyTestInfo(bool createNew)
            : base("聚苯乙烯峰位测试", "Polystyrene Wavelength Test", "cm-1")
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

            //获得仪器温度
            //var tempstr = curInstrument.HardwareGetProperty(Driver.EnumHardware.Device, Driver.EnumHardwareProperties.Temperature);
            //float temperature = float.Parse(tempstr);
            float temperature = 35.0f;

            FinalResult = Ai.Hong.Algorithm.CommonAlgorithm.PickPeak(SpectraDatas[0].xDatas, SpectraDatas[0].yDatas, TargetResult, 4, out double yvalue, false);

            //T: instrument internal temperature in degree C
            //a. Fiber system, Report Peak Position = Measured Peak Position + 0.0107*T - 0.7
            //b. Integrating sphere system, Report Peak Position = (4571.0 * Measured Peak Position) / (-0.0205 * T + 4571.575)

            FinalResult = (4571 * FinalResult) / (4571.575 - 0.0205 * temperature);

            //临时屏蔽
            //if (curInstrument.deviceModel == Driver.EnumDeviceModel.SphereIntegrate)
            //{
            //    FinalResult = (4571 * FinalResult) / (4571.575 - 0.0205 * temperature);
            //}
            //else if (curInstrument.deviceModel == Driver.EnumDeviceModel.Fiber)
            //{
            //    FinalResult = FinalResult + temperature * 0.0107 - 0.7;
            //}

            return IsValidResult();
        }
    }

}
