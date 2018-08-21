using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 能量分布测试(Energy Distribution Test)
    /// </summary>
    public class EnergyDistributeTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 计算比例的X位置
        /// </summary>
        [XmlElement]
        public double TargetX { get; set; }

        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public EnergyDistributeTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public EnergyDistributeTestInfo(bool createNew)
            : base("能量分布测试", "Energy Distribution Test", "%")
        {
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
            
            //找到最大Y值
            var maxY = yDatas.Max();

            //找到10000处的Y值
            int x = Ai.Hong.Algorithm.CommonMethod.FindNearestPosition(xDatas, 0, xDatas.Length - 1, TargetX);
            double curY = x == -1 ? yDatas[yDatas.Length - 1] : yDatas[x];

            FinalResult = (curY / maxY) * 100;

            return IsValidResult();
        }
    }

}
