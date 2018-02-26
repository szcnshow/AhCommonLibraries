using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.Algorithm
{
    /// <summary>
    /// PLS分析结果
    /// </summary>
    public class PLSResult
    {
        /// <summary>
        /// 预测浓度
        /// </summary>
        public double concentration { get; set; }
        /// <summary>
        /// 马氏距离
        /// </summary>
        public double mahDistance { get; set; }
        /// <summary>
        /// 光谱残差
        /// </summary>
        public double residual { get; set; }
        /// <summary>
        /// F Value
        /// </summary>
        public double fValue { get; set; }
        /// <summary>
        /// F Probability
        /// </summary>
        public double fProb { get; set; }
        /// <summary>
        /// 当前组分的维数
        /// </summary>
        public int rank { get; set; }
        /// <summary>
        /// 当前组分建模光谱数量
        /// </summary>
        public int spectrumCount { get; set; }
    }

    /// <summary>
    /// PLS算法类
    /// </summary>
    unsafe public class PLS
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

        private static UInt32 PLSCOEFMARK = 0x43534C50;	//PLS系数标志

        /// <summary>
        /// PLS模型系数的参数
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
        public struct PLSCoefParameter
        {
            /// <summary>
            /// 文件标志
            /// </summary>
            public UInt32 filemark;			//0:文件标志
            /// <summary>
            /// 数据类型（预测模型，建模模型）
            /// </summary>
            public int datatype;		//0.数据类型
            /// <summary>
            /// 数据大小
            /// </summary>
            public int datasize;				//1.数据大小
            /// <summary>
            /// rank维数(int)
            /// </summary>
            public int maxRank;				//2.rank维数(int)
            /// <summary>
            /// 行数(建模光谱数量)
            /// </summary>
            public int specRows;				//3.行数(rows)
            /// <summary>
            /// 列数(建模光谱数据点数)
            /// </summary>
            public int specCols;				//4.列数(cols)
            /// <summary>
            /// 模型的最大马氏距离
            /// </summary>
            public double mahaDistance;		//模型的最大马氏距离
            /// <summary>
            /// 预测系数的Offset(predictCoef)
            /// </summary>
            public int predictCoefOffset;		//5.预测系数的Offset和大小(predictCoef)
            /// <summary>
            /// 预测系数的大小(predictCoef)
            /// </summary>
            public int predictCoefSize;
            /// <summary>
            /// x权重的Offset(xweight)
            /// </summary>
            public int xweightOffset;			//6.x权重的Offset大小(xweight)
            /// <summary>
            /// x权重的大小(xweight)
            /// </summary>
            public int xweightSize;
            /// <summary>
            /// xdefloading的Offset
            /// </summary>
            public int xdefloadingOffset;		//7.xdefloading的Offset和大小
            /// <summary>
            /// xdefloading的大小
            /// </summary>
            public int xdefloadingSize;
            /// <summary>
            /// xscore的Offset
            /// </summary>
            public int xscoreOffset;			//8.xscore的Offset和大小
            /// <summary>
            /// xscore的大小
            /// </summary>
            public int xscoreSize;
            /// <summary>
            /// yloading的Offset
            /// </summary>
            public int yloadingOffset;			//9.yloading的Offset和大小
            /// <summary>
            /// yloading的大小
            /// </summary>
            public int yloadingSize;
            /// <summary>
            /// xdirloading的Offset
            /// </summary>
            public int xdirloadingOffset;		//10.xdirloading的Offset和大小
            /// <summary>
            /// xdirloading的大小
            /// </summary>
            public int xdirloadingSize;
            /// <summary>
            /// 平均光谱Offset
            /// </summary>
            public int avgSpectrumOffset;		//11.平均光谱的Offset和大小
            /// <summary>
            /// 平均光谱大小
            /// </summary>
            public int avgSpectrumSize;
            /// <summary>
            /// 数据版本号
            /// </summary>
            public int version;				    //数据版本号
            /// <summary>
            /// 起始波数
            /// </summary>
            public double firstX;				//起始波数
            /// <summary>
            /// 结束波数
            /// </summary>
            public double lastX;				//结束波数
            /// <summary>
            /// 分辨率
            /// </summary>
            public double resolution;			//分辨率
            /// <summary>
            /// 原始平均光谱的Offse
            /// </summary>
            public int mscAvgSpecOffset;		//16. 原始平均光谱的Offset和大小
            /// <summary>
            /// 原始平均光谱的大小
            /// </summary>
            public int mscAvgSpecSize;
            /// <summary>
            /// 备用字段
            /// </summary>
            public fixed byte undefined[40];
        };

        #region 32Bit DLL

        /// <summary>
        /// 是否有PLS算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSHasAuthority")]
        private static extern bool PLSHasAuthority32();

        /// <summary>
        /// 创建单组分的PLS系数
        /// </summary>
        /// <param name="specDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每个光谱的数据点数量</param>
        /// <param name="conDatas">浓度数据，数量必须与cols相等</param>
        /// <param name="conSize">浓度数据数量</param>
        /// <param name="maxrank">计算的维数</param>
        /// <param name="outDataSize">返回的数据大小(BYTE)</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="resolution">分辨率</param>
        /// <param name="fullRangeAvgSpecDatas">全谱区建模光谱平均光谱，可以为NULL</param>
        /// <param name="fullRangeDataCols">全谱区建模光谱平均光谱数据点数</param>
        /// <returns>组合后的PLS系数</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSCreateCoef")]
        private static extern IntPtr PLSCreateCoef32(double[] specDatas, int specRows, int specCols, double[] conDatas, int conSize, 
            int maxrank, ref int outDataSize, double firstX, double lastX, double resolution,
            double[] fullRangeAvgSpecDatas, int fullRangeDataCols);

        /// <summary>
        /// 保存预测模型
        /// </summary>
        /// <param name="oneCompAllCoef">一个组分的全部维PLS系数</param>
        /// <param name="useRank">使用维数</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSSavePredictCoef")]
        private static extern IntPtr PLSSavePredictCoef32(byte[] oneCompAllCoef, int useRank, ref int outDataSize);

        /// <summary>
        /// 用单组分的计算维系数预测光谱
        /// </summary>
        /// <param name="oneCompAllCoef">单组分多维系数</param>
        /// <param name="specDatas">光谱数据（一条光谱一行）</param>
        /// <param name="specRows">光谱的数量（行）</param>
        /// <param name="specCols">每条光谱的数据量（列）</param>
        /// <param name="rank">使用的维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>返回预测结果(double)，数量等于specRows</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSPredict")]
        private static extern IntPtr PLSPredict32(byte[] oneCompAllCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 获取建模光谱的马氏距离
        /// </summary>
        /// <param name="oneCompAllCoef">建模系数</param>
        /// <param name="rank">计算维数</param>
        /// <param name="outDataSize">返回数据数量(double)，等于建模光谱的数量</param>
        /// <returns>马氏距离列表</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetCoefMahaDistance")]
        private static extern IntPtr PLSGetCoefMahaDistance32(byte[] oneCompAllCoef, int rank, ref int outDataSize);

        /// <summary>
        /// 获取一条样品光谱到建模光谱间的马氏距离
        /// </summary>
        /// <param name="oneCompAllCoef">建模系数</param>
        /// <param name="sampleDatas">样品光谱，一行一条光谱</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">光谱数据点数</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>马氏距离，数量等于specRows或者specRows-1</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetSpectrumMahaDistance")]
        private static extern IntPtr PLSGetSpectrumMahaDistance32(byte[] oneCompAllCoef, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 获取光谱的残差
        /// </summary>
        /// <param name="oneCompCoefDatas">建模系数</param>
        /// <param name="sampleDatas">光谱数据（一条光谱一行）</param>
        /// <param name="specRows">光谱的数量（行）</param>
        /// <param name="specCols">每条光谱的数据量（列）</param>
        /// <param name="rank">使用的维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>返回光谱残差(double)，数量等于specRows或者specRows-1</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetSpectrumResidual")]
        private static extern IntPtr PLSGetSpectrumResidual32(byte[] oneCompCoefDatas, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 通过残差计算FValue
        /// </summary>
        /// <param name="residuals">残差</param>
        /// <param name="datasize">残差的数量</param>
        /// <returns>FValue(double)，数量=datasize</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetFValue")]
        private static extern IntPtr PLSGetFValue32(double[] residuals, int datasize);

        /// <summary>
        /// 通过光谱数据计算的F Value
        /// </summary>
        /// <param name="oneCompCoefDatas">建模系数</param>
        /// <param name="sampleDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>数量等于specRows或者specRows-1</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetSpectrumFValue")]
        private static extern IntPtr PLSGetSpectrumFValue32(byte[] oneCompCoefDatas, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 计算一个fcdf，与matlab相同
        /// </summary>
        /// <param name="x">计算对象</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetOneFcdf")]
        private static extern double PLSGetOneFcdf32(double x, double a, double b);

        /// <summary>
        /// 计算一组fcdf，与matlab相同
        /// </summary>
        /// <param name="x">计算对象</param>
        /// <param name="datasize">数据数量</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>fcdf，数量与datasize相同(double)</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetMultiFcdf")]
        private static extern IntPtr PLSGetMultiFcdf32(double[] x, int datasize, double a, double b);

        /// <summary>
        /// 一次性获取光谱残差, F值和F分布
        /// </summary>
        /// <param name="oneCompCoefDatas">建模系数</param>
        /// <param name="sampleDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>第一行：光谱残差, 第二行:FValue，第三行：FProbability，数量等于specRows*3或者(specRows-1)*3</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetResidualFValueAndFProb")]
        private static extern IntPtr PLSGetResidualFValueAndFProb32(byte[] oneCompCoefDatas, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// PCA分析
        /// </summary>
        /// <param name="sampleDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>前面为xloading,大小=rank*specCols, 后面为xscores，，数量等于rank*specRows或者rank*(specRows-1)</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSPCA")]
        private static extern IntPtr PLSPCA32(double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 获取建模光谱的平均光谱（double）
        /// </summary>
        /// <param name="oneCompCoefDatas">模型系数</param>
        /// <param name="outDataSize">返回数据数量（double）</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetAverageSpectrumData")]
        private static extern IntPtr PLSGetAverageSpectrumData32(byte[] oneCompCoefDatas, ref int outDataSize);

        /// <summary>
        /// 获取原始全区间光谱的平均光谱（double）
        /// </summary>
        /// <param name="oneCompCoefDatas">模型系数</param>
        /// <param name="outDataSize">返回数据数量（double）</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetMSCAverageSpectrumData")]
        private static extern IntPtr PLSGetMSCAverageSpectrumData32(byte[] oneCompCoefDatas, ref int outDataSize);

        #endregion

        #region 64Bit DLL
        /// <summary>
        /// 是否有PLS算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSHasAuthority")]
        private static extern bool PLSHasAuthority64();

        /// <summary>
        /// 创建单组分的PLS系数
        /// </summary>
        /// <param name="specDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每个光谱的数据点数量</param>
        /// <param name="conDatas">浓度数据，数量必须与cols相等</param>
        /// <param name="conSize">浓度数据数量</param>
        /// <param name="maxrank">计算的维数</param>
        /// <param name="outDataSize">返回的数据大小(BYTE)</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="resolution">分辨率</param>
        /// <param name="fullRangeAvgSpecDatas">全谱区建模光谱平均光谱，可以为NULL</param>
        /// <param name="fullRangeDataCols">全谱区建模光谱平均光谱数据点数</param>
        /// <returns>组合后的PLS系数</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSCreateCoef")]
        private static extern IntPtr PLSCreateCoef64(double[] specDatas, int specRows, int specCols, double[] conDatas, int conSize,
            int maxrank, ref int outDataSize, double firstX, double lastX, double resolution,
            double[] fullRangeAvgSpecDatas, int fullRangeDataCols);

        /// <summary>
        /// 保存预测模型
        /// </summary>
        /// <param name="oneCompAllCoef">一个组分的全部维PLS系数</param>
        /// <param name="useRank">使用维数</param>
        /// <param name="outDataSize">返回数据大小</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSSavePredictCoef")]
        private static extern IntPtr PLSSavePredictCoef64(byte[] oneCompAllCoef, int useRank, ref int outDataSize);

        /// <summary>
        /// 用单组分的计算维系数预测光谱
        /// </summary>
        /// <param name="oneCompAllCoef">单组分多维系数</param>
        /// <param name="specDatas">光谱数据（一条光谱一行）</param>
        /// <param name="specRows">光谱的数量（行）</param>
        /// <param name="specCols">每条光谱的数据量（列）</param>
        /// <param name="rank">使用的维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>返回预测结果(double)，数量等于specRows或者specRows-1</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSPredict")]
        private static extern IntPtr PLSPredict64(byte[] oneCompAllCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 获取建模光谱的马氏距离
        /// </summary>
        /// <param name="oneCompAllCoef">建模系数</param>
        /// <param name="rank">计算维数</param>
        /// <param name="outDataSize">返回数据数量(double)，等于建模光谱的数量</param>
        /// <returns>马氏距离列表</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetCoefMahaDistance")]
        private static extern IntPtr PLSGetCoefMahaDistance64(byte[] oneCompAllCoef, int rank, ref int outDataSize);

        /// <summary>
        /// 获取一条样品光谱到建模光谱间的马氏距离
        /// </summary>
        /// <param name="oneCompAllCoef">建模系数</param>
        /// <param name="sampleDatas">样品光谱，一行一条光谱</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">光谱数据点数</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>马氏距离，数量等于specRows或者specRows-1</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetSpectrumMahaDistance")]
        private static extern IntPtr PLSGetSpectrumMahaDistance64(byte[] oneCompAllCoef, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 获取光谱的残差
        /// </summary>
        /// <param name="oneCompCoefDatas">建模系数</param>
        /// <param name="sampleDatas">光谱数据（一条光谱一行）</param>
        /// <param name="specRows">光谱的数量（行）</param>
        /// <param name="specCols">每条光谱的数据量（列）</param>
        /// <param name="rank">使用的维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>返回光谱残差(double),数量等于specRows或者specRows-1</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetSpectrumResidual")]
        private static extern IntPtr PLSGetSpectrumResidual64(byte[] oneCompCoefDatas, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 通过残差计算FValue
        /// </summary>
        /// <param name="residuals">残差</param>
        /// <param name="datasize">残差的数量</param>
        /// <returns>FValue(double)，数量=datasize</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetFValue")]
        private static extern IntPtr PLSGetFValue64(double[] residuals, int datasize);

        /// <summary>
        /// 通过光谱数据计算的F Value
        /// </summary>
        /// <param name="oneCompCoefDatas">建模系数</param>
        /// <param name="sampleDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>FValue，数量等于specRows或者specRows-1</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetSpectrumFValue")]
        private static extern IntPtr PLSGetSpectrumFValue64(byte[] oneCompCoefDatas, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 计算一个fcdf，与matlab相同
        /// </summary>
        /// <param name="x">计算对象</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetOneFcdf")]
        private static extern double PLSGetOneFcdf64(double x, double a, double b);

        /// <summary>
        /// 计算一组fcdf，与matlab相同
        /// </summary>
        /// <param name="x">计算对象</param>
        /// <param name="datasize">数据数量</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>fcdf，数量与datasize相同(double)</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetMultiFcdf")]
        private static extern IntPtr PLSGetMultiFcdf64(double[] x, int datasize, double a, double b);

        /// <summary>
        /// 一次性获取光谱残差, F值和F分布
        /// </summary>
        /// <param name="oneCompCoefDatas">建模系数</param>
        /// <param name="sampleDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>第一行：光谱残差, 第二行:FValue，第三行：FProbability，返回数据数量 = specRows * 3或者3*(specRows-1)</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetResidualFValueAndFProb")]
        private static extern IntPtr PLSGetResidualFValueAndFProb64(byte[] oneCompCoefDatas, double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// PCA分析
        /// </summary>
        /// <param name="sampleDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas第一行是否为X轴数据</param>
        /// <returns>前面为xloading,大小=rank*specCols, 后面为xscores，大小=rank*specRows或者rank*(specRows-1)</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSPCA")]
        private static extern IntPtr PLSPCA64(double[] sampleDatas, int specRows, int specCols, int rank, bool withXDatas);

        /// <summary>
        /// 获取建模光谱的平均光谱（double）
        /// </summary>
        /// <param name="oneCompCoefDatas">模型系数</param>
        /// <param name="outDataSize">返回数据数量（double）</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetAverageSpectrumData")]
        private static extern IntPtr PLSGetAverageSpectrumData64(byte[] oneCompCoefDatas, ref int outDataSize);

        /// <summary>
        /// 获取原始全区间光谱的平均光谱（double）
        /// </summary>
        /// <param name="oneCompCoefDatas">模型系数</param>
        /// <param name="outDataSize">返回数据数量（double）</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "PLSGetMSCAverageSpectrumData")]
        private static extern IntPtr PLSGetMSCAverageSpectrumData64(byte[] oneCompCoefDatas, ref int outDataSize);

        #endregion

        /// <summary>
        /// 判断PLS系数和光谱是否匹配
        /// </summary>
        /// <param name="plsCoef">PLS系数</param>
        /// <param name="specDatas">光谱数据（一条光谱一行）</param>
        /// <param name="specRows">数据行数</param>
        /// <param name="specCols">数据列数</param>
        /// <param name="rank">使用的维数</param>
        /// <returns></returns>
        private static bool IsValidCoefAndSpecData(byte[] plsCoef, double[] specDatas, int specRows, int specCols, int rank)
        {
            if (plsCoef == null || specDatas == null || specDatas.Length != specRows * specCols || !IsValidPLSCoef(plsCoef, specCols, rank))
            {
                ErrorString = "Invalid parameters";
                return false;
            }
            return true;
        }

        #region 外部调用接口，32bit和64bit自动适应

        /// <summary>
        /// 是否有PLS算法的授权
        /// </summary>
        public static bool HasAuthority()
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return PLSHasAuthority64();
                else
                    return PLSHasAuthority32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            } 
        }

        /// <summary>
        /// 创建单组分的PLS系数
        /// </summary>
        /// <param name="specYDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="conDatas">浓度数据，数量必须与cols相等</param>
        /// <param name="maxrank">计算的维数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="resolution">光谱分辨率</param>
        /// <param name="fullRangeAvgDatas">全部区间的平均光谱</param>
        /// <returns>组合后的PLS系数</returns>
        public static byte[] CreatePLSCoef(IList<double[]> specYDatas, double[] conDatas, int maxrank, double firstX = double.NaN, double lastX = double.NaN, double resolution = double.NaN,
            double[] fullRangeAvgDatas=null)
        {
            try
            {
                if (specYDatas == null || specYDatas.Count == 0 || specYDatas.Count != conDatas.Length || maxrank < 1)
                    throw new Exception("Invalid parameters");

                double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
                if (alldatas == null)
                    throw new Exception(CommonMethod.ErrorString);

                return CreatePLSCoef(alldatas, conDatas, specYDatas.Count, specYDatas[0].Length, maxrank,
                    firstX, lastX, resolution, fullRangeAvgDatas);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 创建单组分的PLS系数
        /// </summary>
        /// <param name="specYDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="conDatas">浓度数据，数量必须与cols相等</param>
        /// <param name="specRows">光谱的行数</param>
        /// <param name="specCols">每条光谱的数据点数</param>
        /// <param name="maxrank">计算的维数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="resolution">光谱分辨率</param>
        /// <param name="fullRangeAvgDatas">全部区间的平均光谱</param>
        /// <returns>组合后的PLS系数</returns>
        public static byte[] CreatePLSCoef(double[] specYDatas, double[] conDatas, int specRows, int specCols, int maxrank, 
            double firstX = double.NaN, double lastX = double.NaN, double resolution = double.NaN,
            double[] fullRangeAvgDatas = null)
        {
            try
            {
                if (specYDatas == null || specYDatas.Length != specRows*specCols || conDatas.Length != specRows || maxrank < 1
                    || specRows < 1 || specCols < 1)
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSCreateCoef64(specYDatas, specRows, specCols, conDatas, conDatas.Length, maxrank,
                        ref datasize, firstX, lastX, resolution, fullRangeAvgDatas, fullRangeAvgDatas == null ? 0 : fullRangeAvgDatas.Length);
                else
                    retptr = PLSCreateCoef32(specYDatas, specRows, specCols, conDatas, conDatas.Length, maxrank,
                        ref datasize, firstX, lastX, resolution, fullRangeAvgDatas, fullRangeAvgDatas == null ? 0 : fullRangeAvgDatas.Length);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call SplineCubicInterpolation Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 保存PLS预测模型
        /// </summary>
        /// <param name="plsCoef">PLS系数</param>
        /// <param name="useRank">使用的维数</param>
        /// <returns>返回预测结果(double)，数量等于specRows或者specRows-1</returns>
        public static byte[] CreatePredictOnlyCoef(byte[] plsCoef, int useRank)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSSavePredictCoef64(plsCoef, useRank, ref datasize);
                else
                    retptr = PLSSavePredictCoef32(plsCoef, useRank, ref datasize);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = CommonMethod.GetDLLErrorMessage();
                    return null;
                }
                return CommonMethod.CopyDataArrayFromIntptrAndFree<byte>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 用PLS系数同时预测多条光谱的一个组分
        /// </summary>
        /// <param name="plsCoef">PLS系数</param>
        /// <param name="specYDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="rank">使用的维数</param>
        /// <returns>返回预测结果(double)，数量等于specRows</returns>
        public static double[] Predict(byte[] plsCoef, IList<double[]> specYDatas, int rank)
        {
            try
            {
                //将所有Y轴拷贝到一起
                double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
                if (alldatas == null)
                    throw new Exception(CommonMethod.ErrorString);

                return Predict(plsCoef, alldatas, specYDatas.Count, specYDatas[0].Length, rank);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 用PLS系数同时预测多条光谱的一个组分
        /// </summary>
        /// <param name="plsCoef">PLS系数</param>
        /// <param name="specDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="specRows">数据的行</param>
        /// <param name="specCols">每条光谱的数据点数量</param>
        /// <param name="rank">使用的维数</param>
        /// <param name="withXDatas">specDatas是否包含X轴数据</param>
        /// <returns>返回预测结果(double)，数量等于specRows或者specRows-1</returns>
        public static double[] Predict(byte[] plsCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas = false)
        {
            try
            {
                if (!IsValidCoefAndSpecData(plsCoef, specDatas, specRows, specCols, rank))
                    return null;

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSPredict64(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                else
                    retptr = PLSPredict32(plsCoef, specDatas, specRows, specCols, rank, withXDatas);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call SplineCubicInterpolation Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, withXDatas ? specRows - 1 : specRows);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 用PLS系数预测单条光谱的一个组分
        /// </summary>
        /// <param name="plsCoef">PLS系数</param>
        /// <param name="specYDatas">光谱Y轴数据</param>
        /// <param name="rank">使用的维数</param>
        /// <returns>返回预测结果</returns>
        public static double Predict(byte[] plsCoef, double[] specYDatas, int rank)
        {
            try
            {
                if (plsCoef == null || specYDatas == null || IsValidPLSCoef(plsCoef, specYDatas.Length, rank) == false)
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSPredict64(plsCoef, specYDatas, 1, specYDatas.Length, rank, false);
                else
                    retptr = PLSPredict32(plsCoef, specYDatas, 1, specYDatas.Length, rank, false);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call SplineCubicInterpolation Error");

                double[] retdata = CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, 1);
                if (retdata == null || retdata.Length < 1)
                    throw new Exception("Call SplineCubicInterpolation Error");

                return retdata[0];
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return double.NaN;
            }
        }

        /// <summary>
        /// 获取建模光谱的马氏距离
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="rank">计算维数</param>
        /// <returns>马氏距离列表，数量=建模光谱数量，顺序与建模光谱顺序相同</returns>
        public static double[] GetCoefMahaDistance(byte[] plsCoef, int rank)
        {
            try
            {
                //检查是否为匹配的PLS系数
                PLSCoefParameter para = PLSParameter(plsCoef);
                if (para.datasize == 0 || rank > para.maxRank)
                    throw new Exception("Invalid parameters");

                int datasize = 0;
                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetCoefMahaDistance64(plsCoef, rank, ref datasize);
                else
                    retptr = PLSGetCoefMahaDistance32(plsCoef, rank, ref datasize);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call GetCoefMahaDistance Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取多条样品光谱到建模光谱间的马氏距离
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specDatas">样品光谱，一行一条光谱</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱数据点数</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas是否包含X轴数据</param>
        /// <returns>马氏距离(double)，数量等于specRows或者specRows-1</returns>
        public static double[] GetSpectrumMahaDistance(byte[] plsCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas = false)
        {
            try
            {
                if (!IsValidCoefAndSpecData(plsCoef, specDatas, specRows, specCols, rank))
                    return null;

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetSpectrumMahaDistance64(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                else
                    retptr = PLSGetSpectrumMahaDistance32(plsCoef, specDatas, specRows, specCols, rank, withXDatas);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call GetSpectrumMahaDistance Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, withXDatas ? specRows - 1 : specRows);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取多条样品光谱到建模光谱间的马氏距离
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specYDatas">光谱数据（一条光谱一行）</param>
        /// <param name="rank">使用的维数</param>
        /// <returns>马氏距离(double)，数量=specYdatas的大小</returns>
        public static double[] GetSpectrumMahaDistance(byte[] plsCoef, IList<double[]> specYDatas, int rank)
        {
            try
            {
                double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
                if (alldatas == null)
                    throw new Exception(CommonMethod.ErrorString);

                return GetSpectrumMahaDistance(plsCoef, alldatas, specYDatas.Count, specYDatas[0].Length, rank);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 一次获取多条光谱的残差
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specYDatas">光谱数据（一条光谱一行）</param>
        /// <param name="rank">使用的维数</param>
        /// <returns>返回光谱残差，数量=specYdatas的大小</returns>
        public static double[] GetSpectrumResidual(byte[] plsCoef, IList<double[]> specYDatas, int rank)
        {
            try
            {
                double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
                if (alldatas == null)
                    throw new Exception(CommonMethod.ErrorString);

                return GetSpectrumResidual(plsCoef, alldatas, specYDatas.Count, specYDatas[0].Length, rank);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 一次获取多条光谱的残差
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specDatas">光谱数据（一条光谱一行）</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱数据点数</param>
        /// <param name="withXDatas">specDatas是否包含X轴数据</param>
        /// <param name="rank">使用的维数</param>
        /// <returns>返回光谱残差,数量等于specRows或者specRows-1</returns>
        public static double[] GetSpectrumResidual(byte[] plsCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas = false)
        {
            try
            {
                if (!IsValidCoefAndSpecData(plsCoef, specDatas, specRows, specCols, rank))
                    return null;

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetSpectrumResidual64(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                else
                    retptr = PLSGetSpectrumResidual32(plsCoef, specDatas, specRows, specCols, rank, withXDatas);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call GetSpectrumResidual Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, withXDatas ? specRows - 1 : specRows);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 通过残差计算FValue
        /// </summary>
        /// <param name="residuals">残差</param>
        /// <returns>FValue(double)，数量=residuals的大小</returns>
        public static double[] GetFValue(double[] residuals)
        {
            try
            {
                if (residuals == null || residuals.Length < 1)
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetFValue64(residuals, residuals.Length);
                else
                    retptr = PLSGetFValue32(residuals, residuals.Length);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call GetFValue Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, residuals.Length);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 计算光谱的F Value
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specYDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="rank">计算维数</param>
        /// <returns>FValue</returns>
        public static double[] GetSpectrumFValue(byte[] plsCoef, IList<double[]> specYDatas, int rank)
        {
            try
            {
                double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
                if (alldatas == null)
                    throw new Exception(CommonMethod.ErrorString);

                return GetSpectrumFValue(plsCoef, alldatas, specYDatas.Count, specYDatas[0].Length, rank);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 计算光谱的F Value
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱数据点数</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas是否包含X轴数据</param>
        /// <returns>FValue</returns>
        public static double[] GetSpectrumFValue(byte[] plsCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas = false)
        {
            try
            {
                if (!IsValidCoefAndSpecData(plsCoef, specDatas, specRows, specCols, rank))
                    return null;

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetSpectrumFValue64(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                else
                    retptr = PLSGetSpectrumFValue32(plsCoef, specDatas, specRows, specCols, rank, withXDatas);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call GetFValue Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, withXDatas ? specRows - 1 : specRows);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 计算一个fcdf，与matlab相同
        /// </summary>
        /// <param name="x">计算对象</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetOneFcdf(double x, double a, double b)
        {
            if (CommonMethod.Is64BitVersion())
                return PLSGetOneFcdf64(x, a, b);
            else
                return PLSGetOneFcdf32(x, a, b);
        }

        /// <summary>
        /// 计算一组fcdf，与matlab相同
        /// </summary>
        /// <param name="x">计算对象</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>fcdf，数量与x大小相同(double)</returns>
        public static double[] GetMultiFcdf(double[] x, double a, double b)
        {
            try
            {
                if (x == null || x.Length == 0)
                    throw new Exception("Invalid parameter");

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetMultiFcdf64(x, x.Length, a, b);
                else
                    retptr = PLSGetMultiFcdf32(x, x.Length, a, b);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call GetMultiFcdf Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, x.Length);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 一次性获取光谱残差, F值和F分布
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specYDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="rank">计算维数</param>
        /// <returns>第一行：光谱残差, 第二行:FValue，第三行：FProbability，返回数据数量 = specRows * 3</returns>
        public static double[] GetResidualFValueAndFProb(byte[] plsCoef, IList<double[]> specYDatas, int rank)
        {
            double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
            if (alldatas == null)
            {
                ErrorString = CommonMethod.ErrorString;
                return null;
            }
            return GetResidualFValueAndFProb(plsCoef, alldatas, specYDatas.Count, specYDatas[0].Length, rank);
        }

        /// <summary>
        /// 一次性获取光谱残差, F值和F分布
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas是否包含X轴数据</param>
        /// <returns>第一行：光谱残差, 第二行:FValue，第三行：FProbability，返回数据数量 = specRows * 3或者(specRows-1)*3</returns>
        public static double[] GetResidualFValueAndFProb(byte[] plsCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas = false)
        {
            try
            {
                if (!IsValidCoefAndSpecData(plsCoef, specDatas, specRows, specCols, rank))
                    return null;

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetResidualFValueAndFProb64(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                else
                    retptr = PLSGetResidualFValueAndFProb32(plsCoef, specDatas, specRows, specCols, rank, withXDatas);

                if (retptr == IntPtr.Zero)
                    throw new Exception("Call GetFValue Error");

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, withXDatas ? (specRows - 1)*3 : specRows*3);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// PCA分析
        /// </summary>
        /// <param name="specYDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="rank">使用的维数</param>
        /// <param name="xScores">返回xscores，大小=rank*specRows</param>
        /// <returns>xloading,大小=rank*specCols</returns>
        public static List<double[]> PCA(IList<double[]> specYDatas, int rank, out List<double[]> xScores)
        {
            xScores = null;
            try
            {
                //将所有Y轴拷贝到一起
                double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
                if (alldatas == null)
                    throw new Exception(CommonMethod.ErrorString);

                return PCA(alldatas, specYDatas.Count, specYDatas[0].Length, rank, out xScores);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// PCA分析
        /// </summary>
        /// <param name="specDatas">光谱Y轴数据（一条光谱一行）</param>
        /// <param name="specRows">光谱数据的数量</param>
        /// <param name="specCols">每条光谱的数据点数量</param>
        /// <param name="rank">使用的维数</param>
        /// <param name="xScores">返回xscores，大小=rank*specRows</param>
        /// <param name="withXDatas">specDatas是否包含X轴数据</param>
        /// <returns>xloading,大小=rank*specCols</returns>
        public static List<double[]> PCA(double[] specDatas, int specRows, int specCols, int rank, out List<double[]> xScores, bool withXDatas = false)
        {
            xScores = null;
            try
            {
                if (specDatas == null || specDatas.Length != specRows * specCols)
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSPCA64(specDatas, specRows, specCols, rank, withXDatas);
                else
                    retptr = PLSPCA32(specDatas, specRows, specCols, rank, withXDatas);

                if (retptr == IntPtr.Zero)
                    throw new Exception(CommonMethod.GetDLLErrorMessage());

                if (withXDatas) specRows--;     //结果少一行

                List<double[]> xloading = new List<double[]>();
                //拷贝xloading
                //每条xloading的大小为specCols，总共rank条
                for (int i = 0; i < rank; i++)
                {
                    double[] curdata = new double[specCols];
                    IntPtr curptr = IntPtr.Add(retptr, i * specCols);
                    Marshal.Copy(curptr, curdata, 0, specCols);
                    xloading.Add(curdata);
                }

                //拷贝xScores
                //每条xScores的大小为specRow，总共rank条
                xScores = new List<double[]>();
                for (int i = 0; i < rank; i++)
                {
                    double[] curdata = new double[specRows];
                    IntPtr curptr = IntPtr.Add(retptr, i * specRows + rank * specCols); //跳过前面的xloading
                    Marshal.Copy(curptr, curdata, 0, specRows);
                    xScores.Add(curdata);
                }

                //释放内存
                Marshal.FreeCoTaskMem(retptr);

                return xloading;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 判断PLS系数是否与当前光谱和维数相匹配
        /// </summary>
        /// <param name="plsCoefs">PLS系数</param>
        /// <param name="specCols">一条光谱的数据点数</param>
        /// <param name="rank">计算维数</param>
        public static bool IsValidPLSCoef(byte[] plsCoefs, int specCols, int rank)
        {
            PLSCoefParameter para = PLSParameter(plsCoefs);
            if (para.datasize == 0 || specCols != para.specCols || rank > para.maxRank)
            {
                ErrorString = "Invalid Calling Parameter";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取PLS模型系数的参数
        /// </summary>
        /// <param name="plsCoefs"></param>
        /// <returns></returns>
        public static PLSCoefParameter PLSParameter(byte[] plsCoefs)
        {
            unsafe
            {
                if (plsCoefs == null || plsCoefs.Length < 4)   //,表示错误不是正确系数
                    return new PLSCoefParameter();

                //判断是否为PLS系数
                UInt32 datatype = BitConverter.ToUInt32(plsCoefs, 0);
                if (datatype != PLSCOEFMARK)   //表示错误不是正确系数
                    return new PLSCoefParameter();

                fixed (byte* byteptr = plsCoefs)
                {
                    IntPtr ptr = new IntPtr(byteptr);
                    PLSCoefParameter para = (PLSCoefParameter)Marshal.PtrToStructure(ptr, typeof(PLSCoefParameter));

                    return para;
                }
            }
        }

        /// <summary>
        /// 分析多条光谱，获取浓度，马氏距离，残差，FValue，FProb
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="specRows">光谱的行数（光谱数量）</param>
        /// <param name="specCols">光谱的数据点数量</param>
        /// <param name="rank">计算维数</param>
        /// <param name="withXDatas">specDatas是否包含X轴数据</param>
        /// <returns>返回浓度，马氏距离，残差，FValue，FProb</returns>
        public static PLSResult[] PredictWithAllResult(byte[] plsCoef, double[] specDatas, int specRows, int specCols, int rank, bool withXDatas = false)
        {
            try
            {
                if (!IsValidCoefAndSpecData(plsCoef, specDatas, specRows, specCols, rank))
                    return null;

                var plsPara = PLSParameter(plsCoef);

                double[] cons = Predict(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                if (cons == null)
                    throw new Exception(ErrorString);

                ////建模光谱的平均马氏距离
                //double[] modelMahas = GetCoefMahaDistance(plsCoef, rank);
                //if (modelMahas == null)
                //    throw new Exception(ErrorString);
                //double avgMaha = modelMahas.Average();

                double[] mahas = GetSpectrumMahaDistance(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                if (mahas == null)
                    throw new Exception(ErrorString);

                double[] resandF = GetResidualFValueAndFProb(plsCoef, specDatas, specRows, specCols, rank, withXDatas);
                if (resandF == null)
                    throw new Exception(ErrorString);

                if (withXDatas)     //如果包含X轴，返回数据将少一行
                    specRows--;

                if (cons.Length != specRows || mahas.Length != specRows || resandF.Length != specRows * 3)
                    throw new Exception("Invalid result value count");

                PLSResult[] retdata = new PLSResult[specRows];
                for (int i = 0; i < specRows; i++)
                {
                    retdata[i] = new PLSResult();
                    retdata[i].concentration = cons[i];
                    retdata[i].mahDistance = mahas[i];
                    retdata[i].residual = resandF[i];
                    retdata[i].fValue = resandF[specRows + i];
                    retdata[i].fProb = resandF[specRows * 2 + i];
                    retdata[i].rank = rank;
                    retdata[i].spectrumCount = plsPara.specRows;
                }
                return retdata;
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// 一次性获取光谱残差, F值和F分布
        /// </summary>
        /// <param name="plsCoef">建模系数</param>
        /// <param name="specYDatas">光谱Y值列表，行=光谱数量, 列=光谱数据点数</param>
        /// <param name="rank">计算维数</param>
        /// <returns>第一行：光谱残差, 第二行:FValue，第三行：FProbability，返回数据数量 = specRows * 3</returns>
        public static PLSResult[] PredictWithAllResult(byte[] plsCoef, IList<double[]> specYDatas, int rank)
        {
            double[] alldatas = CommonMethod.CombineSpectrumDatas(specYDatas);
            if (alldatas == null)
            {
                ErrorString = CommonMethod.ErrorString;
                return null;
            }
            return PredictWithAllResult(plsCoef, alldatas, specYDatas.Count, specYDatas[0].Length, rank);
        }

        /// <summary>
        /// 获取建模光谱的平均光谱（double）
        /// </summary>
        /// <param name="plsCoef">模型系数</param>
        /// <returns>平均光谱</returns>
        public static double[] GetAverageSpectrum(byte[] plsCoef)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetAverageSpectrumData64(plsCoef, ref datasize);
                else
                    retptr = PLSGetAverageSpectrumData32(plsCoef, ref datasize);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = CommonMethod.GetDLLErrorMessage();
                    return null;
                }
                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取全谱区光谱的平均光谱，主要用于MSC预处理（double）
        /// </summary>
        /// <param name="plsCoef">模型系数</param>
        /// <returns>平均光谱</returns>
        public static double[] GetMSCAverageSpectrum(byte[] plsCoef)
        {
            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                if (CommonMethod.Is64BitVersion())
                    retptr = PLSGetMSCAverageSpectrumData64(plsCoef, ref datasize);
                else
                    retptr = PLSGetMSCAverageSpectrumData32(plsCoef, ref datasize);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = CommonMethod.GetDLLErrorMessage();
                    return null;
                }
                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, datasize);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }
    }
}
