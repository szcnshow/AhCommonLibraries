using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTNirInterface
{
    /// <summary>
    /// 光谱处理类
    /// </summary>
    public class SpectrumHandle
    {
        /// <summary>
        /// 读取光谱
        /// </summary>
        /// <param name="Path">光谱文件名</param>
        /// <returns></returns>
        public virtual bool ReadFile(string Path) { return false; }

        /// <summary>
        /// X轴数据
        /// </summary>
        public virtual double[] XDatas { get; set; }

        /// <summary>
        /// Y轴数据
        /// </summary>
        public virtual double[] YDatas { get; set; }

        /// <summary>
        /// 存储光谱到文件
        /// </summary>
        /// <param name="Path">保存文件名</param>
        /// <param name="YData">Y轴数据</param>
        /// <returns></returns>
        public virtual bool SaveFile(string Path, double[] YData) { return false; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public virtual string GetError() { return int.MaxValue.ToString(); }

   
        #region  使用EFTIR分析

        /// <summary>
        /// Load模型
        /// </summary>
        /// <param name="path">模型路径</param>
        /// <returns></returns>
        public virtual bool LoadMethod(string path) { return false; }

        /// <summary>
        /// 分析光谱
        /// </summary>
        /// <param name="spcPath">光谱文件名</param>
        /// <returns></returns>
        public virtual string Analysis(string spcPath) { return null; }

      //  public virtual T GetResultObj<T>(T t,string modelPath) where T : class { return null; }

        #endregion
    }

    /// <summary>
    /// 光谱格式
    /// </summary>
    public class SpectrumFormat
    {
        /// <summary>
        /// 将光谱文件转换为byte[]数组
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual byte[] FileToByte(string path) { return null; }

        /// <summary>
        /// 将byte[]数组转换为文件并保存
        /// </summary>
        /// <param name="bytes">需要保存的数据</param>
        /// <param name="path">保存的文件名</param>
        /// <returns></returns>
        public virtual bool SaveToFile(byte[] bytes,string path) { return false; }


        /// <summary>
        /// 将byte[]数组转换为对象
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        public virtual object SaveToFile(byte[] bytes) { return null; }
    }
}
