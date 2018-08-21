using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 100%线噪声测试(100% Line Noise Test)
    /// </summary>
    public class LineNoiseTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 单通道谱图
        /// </summary>
        [XmlIgnore]
        public List<Ai.Hong.FileFormat.FileFormat> singleBeams = new List<Ai.Hong.FileFormat.FileFormat>();

        /// <summary>
        /// 干涉图
        /// </summary>
        [XmlIgnore]
        public List<Ai.Hong.FileFormat.FileFormat> interfers = new List<Ai.Hong.FileFormat.FileFormat>();

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public LineNoiseTestInfo()
        {
        }

        /// <summary>
        /// 创建100%线噪声测试参数
        /// </summary>
        /// <param name="createNew"></param>
        public LineNoiseTestInfo(bool createNew):
            base("100%线噪声测试", "100 % Line Noise Test", "%")
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

            foreach (var data in SpectraDatas)
            {
                //pp峰值  取透射谱数据
                var rangeDatas = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { data.xDatas, data.yDatas }, firstX, lastX);

                //标注最大最小峰位并记录
                PickMaxMinPeak(rangeDatas[0], rangeDatas[1], out double minX, out double minY, out double maxX, out double maxY);

                results.Add(Math.Abs(maxY - minY));
            }

            //计算平均APP, 计算最大的 max(PP-APP)
            FinalResult = results.Average();

            return IsValidResult();
        }

    }

}
