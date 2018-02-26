using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.Algorithm
{
    /// <summary>
    /// 仪器标准化类
    /// </summary>
    unsafe public class SDStandardizing
    {
        const UInt32 DATAMARK = 0x53504453;

        /// <summary>
        /// 系数文件Filter
        /// </summary>
        public const string SDFileFilter = "Standard Coefficient|*.SDCoef";

        /// <summary>
        /// 仪器校准算法
        /// </summary>
        public enum enumAlgorithmType
        {
            /// <summary>
            /// PDS
            /// </summary>
            PDS=0,
            /// <summary>
            /// PLS
            /// </summary>
            PLS = 1,
            /// <summary>
            /// SST
            /// </summary>
            SST = 2
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString;

        /// <summary>
        /// 仪器标准化系数的参数
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, Pack=1)]
        public struct StandardizingCoefParameter
        {
            /// <summary>
            /// 数据标志
            /// </summary>
            public UInt32 datatype;
            /// <summary>
            /// 版本号
            /// </summary>
            public int version;
            /// <summary>
            /// 算法种类(PDS, PLS, SST)
            /// </summary>
            public int algorithm;
            /// <summary>
            /// 数据大小(BYTE,不包含CalCoefParameter)
            /// </summary>
            public int datasize;
            /// <summary>
            /// 起始波数
            /// </summary>
            public double firstX;
            /// <summary>
            /// 结束波数
            /// </summary>
            public double lastX;
            /// <summary>
            /// 光谱的数据点数量
            /// </summary>
            public int specCols;
            /// <summary>
            /// 算法参数大小（BYTE）
            /// </summary>
            public int parasize;
            /// <summary>
            /// 算法参数偏移量
            /// </summary>
            public int paraoffset;
            /// <summary>
            /// 标准化系数大小(BYTE)
            /// </summary>
            public int coefSize;
            /// <summary>
            /// 主系数的Offset
            /// </summary>
            public int coefOffset;
        };

        #region 32Bit DLL
        /// <summary>
        /// 是否有STANDARDIZING算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDHasAuthority")]
        private static extern bool SDHasAuthority32();

        /// <summary>
        /// 创建从机与主机的光谱校正系数
        /// </summary>
        /// <param name="masterData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="slaveData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="windowSize">校正窗口大小</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <param name="datasize">返回的数据大小(BYTE)</param>
        /// <returns>校正系数</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDPDSCreateCoefficient")]
        private static extern IntPtr PDSCoefficient32(double[] masterData, double[] slaveData,
            int specRows, int specCols, int windowSize, double firstX, double lastX, bool withXDatas, ref int datasize);

        /// <summary>
        /// 创建校正系数(PLS)
        /// </summary>        
        /// <param name="masterData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="slaveData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="windowSize">校正窗口大小</param>
        /// <param name="tolerance">偏差</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <param name="datasize">返回的数据大小(BYTE)</param>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDPLSCreateCoefficient")]
        private static extern IntPtr PLSCoefficient32(double[] masterData, double[] slaveData,
            int specRows, int specCols, int windowSize, double tolerance, double firstX, double lastX, bool withXDatas, ref int datasize);

        /// <summary>
        /// 创建校正系数(SST)
        /// </summary>        
        /// <param name="masterData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="slaveData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="rank">维数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <param name="datasize">返回的数据大小(BYTE)</param>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDSSTCreateCoefficient")]
        private static extern IntPtr SSTCoefficient32(double[] masterData, double[] slaveData,
            int specRows, int specCols, int rank, double firstX, double lastX, bool withXDatas, ref int datasize);

        /// <summary>
        /// 使用光谱校正系数，校正光谱
        /// </summary>
        /// <param name="coefData">光谱校正系数(BYTE)</param>
        /// <param name="spectrumData">光谱数据(double)，一行一条光谱,第一行是X轴数据，后面是Y轴数据</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDApplyCoefficient")]
        private static extern IntPtr SDApplyCoefficient32(byte[] coefData, double[] spectrumData, int specRows, int specCols, bool withXDatas);

        /// <summary>
        /// 从主机光谱中选择用于仪器校准的光谱
        /// </summary>
        /// <param name="masterData">主机光谱，一行一条光谱,第一行是X轴数据，后面是Y轴数据</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱的数据点数</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDSelectStandardSpectrum")]
        private static extern IntPtr SDSelectStandardSpectrum32(double[] masterData, int specRows, int specCols, int selectNumber);

        /// <summary>
        /// 通过欧式距离，使用KS算法选择有效光谱
        /// </summary>
        /// <param name="spectrumYDatas">主机光谱，一行一条Y轴数据</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱的数据点数</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSEuclideanSelectSample")]
        private static extern IntPtr KSEuclideanSelectSample32(double[] spectrumYDatas, int specRows, int specCols, int selectNumber);

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern string GetErrorMessage32();

        #endregion

        #region 64Bit DLL
        /// <summary>
        /// 是否有STANDARDIZING算法授权
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDHasAuthority")]
        private static extern bool SDHasAuthority64();

        /// <summary>
        /// 创建从机与主机的光谱校正系数
        /// </summary>
        /// <param name="masterData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="slaveData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="windowSize">校正窗口大小</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <param name="datasize">返回的数据大小(BYTE)</param>
        /// <returns>校正系数</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDPDSCreateCoefficient")]
        private static extern IntPtr PDSCoefficient64(double[] masterData, double[] slaveData,
            int specRows, int specCols, int windowSize, double firstX, double lastX, bool withXDatas, ref int datasize);

        /// <summary>
        /// 创建校正系数(PLS)
        /// </summary>        
        /// <param name="masterData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="slaveData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="windowSize">维数</param>
        /// <param name="tolerance">偏差</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <param name="datasize">返回的数据大小(BYTE)</param>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDPLSCreateCoefficient")]
        private static extern IntPtr PLSCoefficient64(double[] masterData, double[] slaveData,
            int specRows, int specCols, int windowSize, double tolerance, double firstX, double lastX, bool withXDatas, ref int datasize);

        /// <summary>
        /// 创建校正系数(SST)
        /// </summary>        
        /// <param name="masterData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="slaveData">光谱数据(double)（一条光谱一行,第一行是X轴数据，后面是Y轴数据）</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="rank">维数</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <param name="datasize">返回的数据大小(BYTE)</param>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDSSTCreateCoefficient")]
        private static extern IntPtr SSTCoefficient64(double[] masterData, double[] slaveData,
            int specRows, int specCols, int rank, double firstX, double lastX, bool withXDatas, ref int datasize);

        /// <summary>
        /// 使用光谱校正系数，校正光谱
        /// </summary>
        /// <param name="coefData">光谱校正系数(BYTE)</param>
        /// <param name="spectrumData">光谱数据(double)，一行一条光谱,第一行是X轴数据，后面是Y轴数据</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDApplyCoefficient")]
        private static extern IntPtr SDApplyCoefficient64(byte[] coefData, double[] spectrumData, int specRows, int specCols, bool withXDatas);

        /// <summary>
        /// 从主机光谱中选择用于仪器校准的光谱
        /// </summary>
        /// <param name="masterData">主机光谱，一行一条光谱,第一行是X轴数据，后面是Y轴数据</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱的数据点数</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDSelectStandardSpectrum")]
        private static extern IntPtr SDSelectStandardSpectrum64(double[] masterData, int specRows, int specCols, int selectNumber);

        /// <summary>
        /// 通过欧式距离，使用KS算法选择有效光谱
        /// </summary>
        /// <param name="spectrumYDatas">主机光谱，一行一条Y轴数据</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱的数据点数</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSEuclideanSelectSample")]
        private static extern IntPtr KSEuclideanSelectSample64(double[] spectrumYDatas, int specRows, int specCols, int selectNumber);

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        [DllImport("SpectrumAlgorithm_64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetErrorMessage")]
        private static extern string GetErrorMessage64();

        #endregion

        #region 外部调用接口，32bit和64bit自动适应
        /// <summary>
        /// 是否有STANDARDIZING算法授权
        /// </summary>
        public static bool HasAuthority()
        {
            try
            {
                if (CommonMethod.Is64BitVersion())
                    return SDHasAuthority64();
                else
                    return SDHasAuthority32();
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取C++ DLL返回的错误信息
        /// </summary>
        private static string GetErrorMessage()
        {
            if (CommonMethod.Is64BitVersion())
                return GetErrorMessage64();
            else
                return GetErrorMessage32();
        }

        /// <summary>
        /// 根据主机光谱创建辅机的校准系数(数据不包含X轴)
        /// </summary>
        /// <param name="algorithmType">校准算法</param>
        /// <param name="masterData">主机数据（一条光谱一行Y轴数据）</param>
        /// <param name="slaveData">辅机数据（一条光谱一行Y轴数据）</param>
        /// <param name="windowSize">校准窗口大小</param>
        /// <param name="tolerance">校准偏差（PLS）</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">True=masterData和slaveData第一行包含X轴数据</param>
        /// <returns>校准系数</returns>
        public static byte[] GetCoefficient(enumAlgorithmType algorithmType, IList<double[]> masterData, IList<double[]> slaveData, 
            int windowSize, double tolerance, double firstX, double lastX, bool withXDatas=false)
        {
            if (masterData == null || slaveData == null || masterData.Count < 2 ||  masterData.Count != slaveData.Count ||
                masterData[0].Length != slaveData[0].Length || windowSize < 1 || masterData[0].Length < windowSize + 1)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            double[] allmaster = CommonMethod.CombineSpectrumDatas(masterData);
            double[] allslave = CommonMethod.CombineSpectrumDatas(slaveData);
            if(allmaster == null || allslave == null)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            return GetCoefficient(algorithmType, allmaster, allslave, masterData.Count, masterData[0].Length, windowSize, tolerance, firstX, lastX, withXDatas);
        }

        /// <summary>
        /// 根据主机光谱创建辅机的校准系数
        /// </summary>
        /// <param name="algorithmType">校准算法</param>
        /// <param name="masterData">主机数据（一条光谱一行Y轴数据）</param>
        /// <param name="slaveData">辅机数据（一条光谱一行Y轴数据）</param>
        /// <param name="specRows">光谱的行数</param>
        /// <param name="specCols">每条光谱的数据点数量</param>
        /// <param name="windowSize">校准窗口大小(PDS), RANK(SST)</param>
        /// <param name="tolerance">校准偏差（PLS）</param>
        /// <param name="firstX">起始波数</param>
        /// <param name="lastX">结束波数</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <returns>校准系数</returns>
        public static byte[] GetCoefficient(enumAlgorithmType algorithmType, double[] masterData, double[] slaveData, int specRows, int specCols, 
            int windowSize, double tolerance,  double firstX, double lastX, bool withXDatas)
        {
            if (masterData == null || slaveData == null || specRows < 2 || specCols < windowSize + 1 || windowSize < 1 ||
                masterData.Length != specRows * specCols || masterData.Length != slaveData.Length)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            try
            {
                IntPtr retptr = IntPtr.Zero;
                int datasize = 0;
                switch (algorithmType)
                {
                    case enumAlgorithmType.PDS:
                        if (CommonMethod.Is64BitVersion())
                            retptr = PDSCoefficient64(masterData, slaveData, specRows, specCols, windowSize, firstX, lastX, withXDatas, ref datasize);
                        else
                            retptr = PDSCoefficient32(masterData, slaveData, specRows, specCols, windowSize, firstX, lastX, withXDatas, ref datasize);
                        break;
                    case enumAlgorithmType.PLS:
                        if (CommonMethod.Is64BitVersion())
                            retptr = PLSCoefficient64(masterData, slaveData, specRows, specCols, windowSize, tolerance, firstX, lastX, withXDatas, ref datasize);
                        else
                            retptr = PLSCoefficient32(masterData, slaveData, specRows, specCols, windowSize, tolerance, firstX, lastX, withXDatas, ref datasize);
                        break;
                    case enumAlgorithmType.SST:
                        if (CommonMethod.Is64BitVersion())
                            retptr = SSTCoefficient64(masterData, slaveData, specRows, specCols, windowSize, firstX, lastX, withXDatas, ref datasize);
                        else
                            retptr = SSTCoefficient32(masterData, slaveData, specRows, specCols, windowSize, firstX, lastX, withXDatas, ref datasize);
                        break;
                    default:
                        break;
                }

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = GetErrorMessage();
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
        /// 使用光谱校正系数，校正光谱（不包含X轴数据）
        /// </summary>
        /// <param name="coefData">光谱校正系数(BYTE)</param>
        /// <param name="slaveData">光谱数据</param>
        /// <returns>校正后的光谱Y轴数据</returns>
        public static List<double[]> ApplyCoefficient(byte[] coefData, IList<double[]> slaveData)
        {
            if (coefData == null || slaveData == null || slaveData.Count ==0)
            {
                ErrorString = "Invalid parameters";
                return null;
            }
            if (!IsValidCoef(coefData,slaveData[0].Length))
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            double[] allslave = CommonMethod.CombineSpectrumDatas(slaveData);
            if (allslave == null)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            double[] retdata = ApplyCoefficient(coefData, allslave, slaveData.Count, slaveData[0].Length, false);
            if (retdata == null || retdata.Length != allslave.Length)
                return null;

            //拷贝每条光谱
            //每条光谱的大小为slaveData[0].Length，总共slaveData.Count条
            List<double[]> retSpecs = new List<double[]>();
            for (int i = 0; i < slaveData.Count; i++)
            {
                double[] curdata = new double[slaveData[0].Length];
                Array.Copy(retdata, i * curdata.Length, curdata, 0, curdata.Length);
                retSpecs.Add(curdata);
            }

            return retSpecs;
        }

        /// <summary>
        /// 使用光谱校正系数，校正光谱，包含插值（包含X轴数据）
        /// </summary>
        /// <param name="coefData">光谱校正系数(BYTE)</param>
        /// <param name="slaveData">光谱数据</param>
        /// <returns>校正后的光谱Y轴数据</returns>
        public static List<double[]> ApplyCoefficientWithXData(byte[] coefData, IList<double[]> slaveData)
        {
            try
            {
                if (coefData == null || slaveData == null || slaveData.Count == 1)
                    throw new Exception("Invalid parameters");

                //判断是否需要插值
                var para = GetStandardizingCoefParameter(coefData);
                if (para.datasize == 0)
                    throw new Exception("Invalid parameters");

                //插值
                if (!CommonMethod.IsSameXDatas(slaveData[0], para.firstX, para.lastX, para.specCols))
                {
                    slaveData = PreProcessor.SplineCubicInterpolation(slaveData, para.firstX, para.lastX, para.specCols);
                    if (slaveData == null)
                        throw new Exception(PreProcessor.ErrorString);
                }

                //保留xData
                var xData = slaveData[0];

                //标准化时不需要xData
                slaveData.RemoveAt(0);
                var retSpecs = ApplyCoefficient(coefData, slaveData);
                if (retSpecs != null && retSpecs.Count != slaveData.Count)
                    throw new Exception(ErrorString);

                //恢复xData
                retSpecs.Insert(0, xData);

                return retSpecs;

            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 使用光谱校正系数，校正光谱
        /// </summary>
        /// <param name="coefData">光谱校正系数(BYTE)</param>
        /// <param name="slaveData">光谱数据</param>
        /// <param name="specRows">光谱的数量</param>
        /// <param name="specCols">每条光谱数据数量</param>
        /// <param name="withXDatas">是否包含X轴数据</param>
        /// <returns>校正后的光谱,与slaveData大小相同</returns>
        public static double[] ApplyCoefficient(byte[] coefData, double[] slaveData, int specRows, int specCols, bool withXDatas)
        {
            try
            {
                if (coefData == null || slaveData == null || specRows < 1)
                    throw new Exception("Invalid parameters");
                if (!IsValidCoef(coefData, specCols))
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;

                if (CommonMethod.Is64BitVersion())
                    retptr = SDApplyCoefficient64(coefData, slaveData, specRows, specCols, withXDatas);
                else
                    retptr = SDApplyCoefficient32(coefData, slaveData, specRows, specCols, withXDatas);

                if(retptr == IntPtr.Zero)
                {
                    ErrorString = GetErrorMessage();
                    return null;
                }

                return CommonMethod.CopyDataArrayFromIntptrAndFree<double>(ref retptr, specCols * specRows);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// 从主机光谱中选择用于仪器校准的光谱
        /// </summary>
        /// <param name="masterData">主机光谱，一行一条光谱,第一行是X轴数据，后面是Y轴数据</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        public static int[] SelectStandardSpectrum(IList<double[]> masterData, int selectNumber)
        {
            if (masterData == null || selectNumber < 1 || masterData.Count < selectNumber || masterData[0].Length < 1)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            double[] allmaster = CommonMethod.CombineSpectrumDatas(masterData);
            if (allmaster == null)
            {
                ErrorString = CommonMethod.ErrorString;
                return null;
            }

            return SelectStandardSpectrum(allmaster, masterData.Count, masterData[0].Length, selectNumber);
        }

        /// <summary>
        /// 从主机光谱中选择用于仪器校准的光谱
        /// </summary>
        /// <param name="masterData">主机光谱，一行一条光谱,第一行是X轴数据，后面是Y轴数据</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱的数据点数</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        public static int[] SelectStandardSpectrum(double[] masterData, int specRows, int specCols, int selectNumber)
        {
            try
            {
                if (masterData == null || selectNumber < 1 || specRows < selectNumber || masterData.Length != specRows * specCols)
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = SDSelectStandardSpectrum64(masterData, specRows, specCols, selectNumber);
                else
                    retptr = SDSelectStandardSpectrum32(masterData, specRows, specCols, selectNumber);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = GetErrorMessage();
                    return null;
                }

                return CommonMethod.CopyDataArrayFromIntptrAndFree<int>(ref retptr, selectNumber);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }
        }


        /// <summary>
        /// 通过欧式距离，使用KS算法选择有效光谱
        /// </summary>
        /// <param name="spectrumYDatas">主机光谱，一行一条Y轴数据</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        public static int[] KSEuclideanSelectSample(IList<double[]> spectrumYDatas, int selectNumber)
        {
            if (spectrumYDatas == null || selectNumber < 1 || spectrumYDatas.Count < selectNumber || spectrumYDatas[0].Length < 1)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            double[] allmaster = CommonMethod.CombineSpectrumDatas(spectrumYDatas);
            if (allmaster == null)
            {
                ErrorString = CommonMethod.ErrorString;
                return null;
            }

            return KSEuclideanSelectSample(allmaster, spectrumYDatas.Count, spectrumYDatas[0].Length, selectNumber);
        }

        /// <summary>
        /// 通过欧式距离，使用KS算法选择有效光谱
        /// </summary>
        /// <param name="spectrumYDatas">主机光谱，一行一条Y轴数据</param>
        /// <param name="specRows">光谱数量</param>
        /// <param name="specCols">每条光谱的数据点数</param>
        /// <param name="selectNumber">需要选择多少条光谱</param>
        /// <returns>选中光谱的序号(int)，大小=selectNumber</returns>
        public static int[] KSEuclideanSelectSample(double[] spectrumYDatas, int specRows, int specCols, int selectNumber)
        {
            try
            {
                if (spectrumYDatas == null || selectNumber < 1 || specRows < selectNumber || spectrumYDatas.Length != specRows * specCols)
                    throw new Exception("Invalid parameters");

                IntPtr retptr = IntPtr.Zero;
                if (CommonMethod.Is64BitVersion())
                    retptr = KSEuclideanSelectSample64(spectrumYDatas, specRows, specCols, selectNumber);
                else
                    retptr = KSEuclideanSelectSample32(spectrumYDatas, specRows, specCols, selectNumber);

                if (retptr == IntPtr.Zero)
                {
                    ErrorString = GetErrorMessage();
                    return null;
                }

                return CommonMethod.CopyDataArrayFromIntptrAndFree<int>(ref retptr, selectNumber);
            }
            catch (Exception ex)
            {
                ErrorString = ex.Message;
                return null;
            }

        }

        #endregion

        /// <summary>
        /// 获取校准系数的参数
        /// </summary>
        /// <param name="calCoefData">校准系数</param>
        /// <returns>校准系数的参数（dataSize=0表示错误）</returns>
        public static StandardizingCoefParameter GetStandardizingCoefParameter(byte[] calCoefData)
        {
            unsafe
            {
                if (calCoefData == null || calCoefData.Length < Marshal.SizeOf(typeof(StandardizingCoefParameter)))
                    return new StandardizingCoefParameter();

                //判断系数类型是否为标准化系数（前4位）
                UInt32 datatype = BitConverter.ToUInt32(calCoefData, 0);
                if(datatype != DATAMARK)
                    return new StandardizingCoefParameter();

                try
                {
                    //判断系数类型=0表示为标准化系数
                    fixed (byte* byteptr = calCoefData)
                    {
                        IntPtr ptr = new IntPtr(byteptr);
                        StandardizingCoefParameter para = (StandardizingCoefParameter)Marshal.PtrToStructure(ptr, typeof(StandardizingCoefParameter));

                        //逐个算法判断
                        bool found = false;
                        foreach (int item in Enum.GetValues(typeof(enumAlgorithmType)))
                        {
                            if (para.algorithm == (UInt32)item)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            return new StandardizingCoefParameter();
                        }

                        return para;
                    }
                }
                catch (Exception ex)
                {
                    ErrorString = ex.Message;
                    return new StandardizingCoefParameter();;
                }
            }
        }

        /// <summary>
        /// 判断是否为有效的校准参数
        /// </summary>
        /// <param name="calCoefData">校准参数</param>
        /// <param name="specCols">光谱数据点数</param>
        public static bool IsValidCoef(byte[] calCoefData, int specCols)
        {
            StandardizingCoefParameter para = GetStandardizingCoefParameter(calCoefData);
            return para.specCols == specCols && para.datatype == DATAMARK;
        }

        /// <summary>
        /// 判断是否为校准系数
        /// </summary>
        /// <param name="calCoefData">校准参数</param>
        public static bool IsStandardCoefData(byte[] calCoefData)
        {
            if (calCoefData == null || calCoefData.Length < 4)
                return false;

            //判断系数类型是否为标准化系数（前4位）
            UInt32 datatype = BitConverter.ToUInt32(calCoefData, 0);

            return datatype == DATAMARK;
        }
    }
}
