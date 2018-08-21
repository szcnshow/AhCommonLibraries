using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 分光器稳定性测试(Photometric Accuracy Test)
    /// </summary>
    public class PhotometricTestInfo : BaseSelfTestInfo
    {
        /// <summary>
        /// 构造函数（主要用于反序列化）
        /// </summary>
        public PhotometricTestInfo()
        {
        }

        /// <summary>
        /// 带参数的构造函数 Thresold《0.8
        /// </summary>
        public PhotometricTestInfo(bool createNew)
            : base("光度精确度测试", "Photometric Accuracy Test", "%")
        {
        }

        /// <summary>
        /// 计算测试结果
        /// </summary>
        /// <param name="calcuParameter">参考干涉图</param>
        /// <returns></returns>
        public override bool CalculateResult(dynamic calcuParameter)
        {
            results = new List<double>();

            //加载参考透射光谱
            FileFormat.FileFormat referenceData = new FileFormat.FileFormat(calcuParameter as string);
            if (referenceData.xDatas == null)
            {
                ErrorString = "Cannot find reference file:" + ReferenceFile;
                return false;
            }
            var oldData = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { referenceData.xDatas, referenceData.yDatas }, firstX, lastX)[1];

            var newData = Ai.Hong.Algorithm.CommonMethod.GetRangeData(new List<double[]>() { SpectraDatas[0].xDatas, SpectraDatas[0].yDatas }, firstX, lastX)[1];

            double total = 0;
            for (int i = 0; i < oldData.Length; i++)
                total += newData[i] - oldData[i];
            FinalResult = Math.Abs(total / oldData.Length);

            return IsValidResult();
        }
    }

}
