using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Algorithm
{
    /// <summary>
    /// 拉曼算法
    /// </summary>
    public class RamanAlgorithm
    {
        /// <summary>
        /// Pixel to raman convert parameter
        /// </summary>
        public class ConvertParameter
        {
            /// <summary>
            /// raman first x
            /// </summary>
            public double ramanFirstX { get; set; }
            /// <summary>
            /// raman last x
            /// </summary>
            public double ramanLastX { get; set; }
            /// <summary>
            /// raman data count
            /// </summary>
            public int ramanDataCount { get; set; }
            /// <summary>
            /// x step
            /// </summary>
            public double ramanXStep { get { return ramanDataCount == 0 ? double.NaN : (ramanLastX - ramanFirstX) / (ramanDataCount - 1); } }
            /// <summary>
            /// pixel 2 raman coefficients(length=4)
            /// </summary>
            public double[] convertCoefs { get; set; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = null;

        /// <summary>
        /// Convert pixel data to raman shift data use convert coefficient with interpolation
        /// </summary>
        /// <param name="para">Destination raman spectrum format</param>
        /// <param name="pixelFirstX">Pixel first X</param>
        /// <param name="pixelYDatas">Pixel y datas</param>
        /// <param name="outRamanXDatas">OUT: Raman x datas</param>
        /// <returns>Raman y datas</returns>
        public static double[] PiexToRaman(ConvertParameter para, int pixelFirstX, double[] pixelYDatas, out double[] outRamanXDatas)
        {
            outRamanXDatas = null;

            //X校正系数为3阶，共4个参数, Y轴校正为5阶，共6个参数
            if (para == null || para.convertCoefs == null || para.convertCoefs.Length != 4 ||
                pixelFirstX < 0 || pixelYDatas == null || pixelYDatas.Length < 5 ||
                para.ramanLastX <= para.ramanFirstX || para.ramanFirstX < 0 || para.ramanDataCount < 1)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            //**********  按照xRamanStep指定的步长，将pixelData中的像素光谱转换到拉曼光谱, 像素谱中的X值为0到length-1  ****/
            int xLen = pixelYDatas.Length;

            //先使用X校正后的系数将nirstRData中的X轴转换到波数
            double[] xcoef = para.convertCoefs;

            double[] tempRamanX = new double[xLen];
            for (int i = pixelFirstX; i < pixelFirstX + xLen; i++)  //像素谱的X轴数值从firstPixelX到pxielYDatas.Length-firstPixelX
            {
                double tempi = i;
                tempRamanX[i - pixelFirstX] = xcoef[0] + xcoef[1] * tempi + xcoef[2] * tempi * tempi + xcoef[3] * tempi * tempi * tempi;
            }

            //对Raman光谱重新插值
            List<double[]> datas = new List<double[]>(){tempRamanX, pixelYDatas};
            datas = Ai.Hong.Algorithm.PreProcessor.SplineCubicInterpolation(datas, para.ramanFirstX, para.ramanLastX, para.ramanDataCount);
            if(datas == null || datas.Count != 2)
            {
                ErrorString = Ai.Hong.Algorithm.PreProcessor.ErrorString;
                return null;
            }

            outRamanXDatas = datas[0];
            return datas[1];
        }

        /// <summary>
        /// Create intensity calibrate data(raman shift format)
        /// </summary>
        /// <param name="para">raman convert coefficient</param>
        /// <param name="standardCalCoefs">2241 standard coefs</param>
        /// <param name="pixelFirstX">First x of calibration spectrum</param>
        /// <param name="pixelYDatas">Calibratoin sepctrum y datas</param>
        /// <param name="userCalCoefs">User calibrate coefs</param>
        /// <returns>Intensity calibration datas</returns>
        public static double[] GetIntensityCalData(ConvertParameter para, double[] standardCalCoefs, int pixelFirstX, double[] pixelYDatas, double[] userCalCoefs = null)
        {
            //X校正系数为3阶，共4个参数, Y轴校正为5阶，共6个参数
            if (para == null || para.convertCoefs == null || para.convertCoefs.Length != 4 ||
                pixelFirstX < 0 || pixelYDatas == null || pixelYDatas.Length < 5 ||
                para.ramanLastX <= para.ramanFirstX || para.ramanFirstX < 0 || para.ramanDataCount < 1)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            if(standardCalCoefs == null || standardCalCoefs.Length < 2)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            if(userCalCoefs != null && userCalCoefs.Length < 2)
            {
                ErrorString = "Invalid Parameters";
                return null;
            }

            //为了防止覆盖，需要克隆一个数组
            double[] operateYDatas = new double[pixelYDatas.Length];
            Array.Copy(pixelYDatas, operateYDatas, operateYDatas.Length);

            //用户自定义校准需要另外处理
            if (userCalCoefs != null)
            {
                //按照像素光谱的格式来拟合用户自定义系数光谱
                double[] userCalDatas = PolynomialResult(userCalCoefs, pixelFirstX, 1.0, operateYDatas.Length);
                //标准物质像素光谱 = 用户系数拟合光谱 x 替代物光谱像素
                for (int i = 0; i < operateYDatas.Length; i++)
                    operateYDatas[i] *= userCalDatas[i];
            }

            //将标准物质像素光谱转换为拉曼谱
            double[] ramanXDatas;
            double[] ramanYDatas = PiexToRaman(para, pixelFirstX, operateYDatas, out ramanXDatas);
            if (ramanYDatas == null)
                return null;

            //标准系数拟合光谱
            double[] standardPolyDatas = PolynomialResult(standardCalCoefs, para.ramanFirstX, para.ramanXStep, para.ramanDataCount);

            //实际计算为 (Sample Measure / Sample Real ) = (2241 Measure / 2241 Real)
            //这里用2241 Real/ 2241 Measure, 出去之后直接乘上 Sample Measure，就得到Sample Real
            for (int i = 0; i < ramanYDatas.Length; i++)
                ramanYDatas[i] = standardPolyDatas[i] / (ramanYDatas[i] == 0 ? 1 : ramanYDatas[i]);
            double minY = ramanYDatas.Min();
            for (int i = 0; i < ramanYDatas.Length; i++)
                ramanYDatas[i] = ramanYDatas[i] / minY;

            return ramanYDatas;
        }

        /// <summary>
        /// Intensity and offset calibrate
        /// </summary>
        /// <param name="sourceYDatas">source raman y datas (same length to intensityCalDatas)</param>
        /// <param name="intensityCalDatas">intensity calibrate y datas(from CorrectIntensityCalData)</param>
        /// <param name="offsetCalData">offset calibrate coefficient</param>
        /// <returns>Calibrated Y datas </returns>
        public static double[] IntensityOffsetCalibrate(double[] sourceYDatas, double[] intensityCalDatas, double offsetCalData)
        {
            if (sourceYDatas == null || intensityCalDatas == null || sourceYDatas.Length != intensityCalDatas.Length || sourceYDatas.Length < 1)
                return null;

            //将校正光谱应用到当前扫描光谱上(ydata*ycal)，同时进行Offset校正
            double[] retDatas = new double[sourceYDatas.Length];
            for (int i = 0; i < retDatas.Length; i++)
                retDatas[i] = sourceYDatas[i]* intensityCalDatas[i] - offsetCalData;

            return retDatas;
        }

        /// <summary>
        /// 多项式计算(自变量为inData)
        /// </summary>
        /// <param name="inData">自变量输入值, 同时也是输出值</param>
        /// <param name="coefs">多项式系数</param>
        private static void PolynomialResult(ref double[] inData, double[] coefs)
        {
            if (inData == null || inData.Length == 0 || coefs == null || coefs.Length < 1)
                return;

            int i;
            int len = inData.Length;
            double[] c = coefs;
            double[] d = inData;
            switch (coefs.Length - 1)
            {
                case 0:     //0阶不用处理
                    break;
                case 1:     //1阶
                    for (i = 0; i < len; i++)
                        d[i] = c[0] + c[1] * d[i];
                    break;
                case 2:     //2阶
                    for (i = 0; i < len; i++)
                        d[i] = c[0] + c[1] * d[i] + c[2] * d[i] * d[i];
                    break;
                case 3:
                    for (i = 0; i < len; i++)
                        d[i] = c[0] + c[1] * d[i] + c[2] * d[i] * d[i] + c[3] * d[i] * d[i] * d[i];
                    break;
                case 4:
                    for (i = 0; i < len; i++)
                        d[i] = c[0] + c[1] * d[i] + c[2] * d[i] * d[i] + c[3] * d[i] * d[i] * d[i] + c[4] * d[i] * d[i] * d[i] * d[i];
                    break;
                case 5:
                    for (i = 0; i < len; i++)
                        d[i] = c[0] + c[1] * d[i] + c[2] * d[i] * d[i] + c[3] * d[i] * d[i] * d[i] + c[4] * d[i] * d[i] * d[i] * d[i] + c[5] * d[i] * d[i] * d[i] * d[i] * d[i];
                    break;
                default:    //超过5阶
                    int clen = c.Length;
                    int j;
                    for (i = 0; i < len; i++)
                    {
                        d[i] = c[0] + c[1] * d[i] + c[2] * d[i] * d[i] + c[3] * d[i] * d[i] * d[i] + c[4] * d[i] * d[i] * d[i] * d[i] + c[5] * d[i] * d[i] * d[i] * d[i] * d[i];
                        for (j = 6; j < c.Length; j++)
                            d[i] += c[j] * Math.Pow(d[i], j);
                    }
                    break;
            }
        }

        /// <summary>
        /// 多项式计算
        /// </summary>
        /// <param name="coefs">多项式系数</param>
        /// <param name="firstX">起始X值</param>
        /// <param name="stepx">X步长</param>
        /// <param name="xcount">X的数量</param>
        /// <returns>拟合后的Y值</returns>
        private static double[] PolynomialResult(double[] coefs, double firstX, double stepx, int xcount)
        {
            if (coefs == null || coefs.Length < 1 || xcount < 1)
                return null;

            double[] retData = new double[xcount];
            for (int i = 0; i < xcount; i++)
                retData[i] = firstX + i * stepx;

            PolynomialResult(ref retData, coefs);

            return retData;
        }

        /// <summary>
        /// 获取像素谱转拉曼谱的系数
        /// </summary>
        /// <param name="standardRamanPeaks">校准物质的拉曼峰位</param>
        /// <param name="pickedPixelPeaks">校准物质测量的像素峰位</param>
        /// <returns>转换系数</returns>
        public static double[] GetPixel2RamanConvertCoefs(double[] standardRamanPeaks, double[] pickedPixelPeaks)
        {
            if (standardRamanPeaks == null || standardRamanPeaks.Length < 3 || pickedPixelPeaks == null || pickedPixelPeaks.Length != standardRamanPeaks.Length)
            {
                ErrorString = "Invalid parameters";
                return null;
            }

            int info;
            alglib.barycentricinterpolant p;
            alglib.polynomialfitreport rep;

            //三阶多项式拟合,得到拟合系数
            alglib.polynomialfit(pickedPixelPeaks, standardRamanPeaks, 4, out info, out p, out rep);

            //计算多项式系数,多项式公式：y=c0+c1*x+c2*x^2+c3*x^3,分别带入0,1,2,3得到4个方程式
            double y0 = alglib.barycentriccalc(p, 0);   //y0=a+0b+0c+0d
            double y1 = alglib.barycentriccalc(p, 1);   //y1=a+1b+1c+1d
            double y2 = alglib.barycentriccalc(p, 2);   //y2=a+2b+4c+8d
            double y3 = alglib.barycentriccalc(p, 3);   //y3=a+3b+9c+27d

            //解四元一次方程
            //创建系数矩阵
            double[,] factor = new double[4, 4] { { 1, 0, 0, 0 }, { 1, 1, 1, 1 }, { 1, 2, 4, 8 }, { 1, 3, 9, 27 } };       //系数矩阵

            //求系数矩阵的逆矩阵(A-1)
            alglib.matinvreport factorRep;
            alglib.rmatrixinverse(ref factor, out info, out factorRep);

            //创建结果矩阵
            double[] result = new double[4] { y0, y1, y2, y3 };     //结果矩阵
            double[] multRet = new double[4];

            //系数矩阵的逆矩阵乘上结果矩阵，得到变量矩阵的值
            alglib.rmatrixmv(4, 4, factor, 0, 0, 0, result, 0, ref multRet, 0);

            return multRet;
        }

        /// <summary>
        /// 获取暗电流像素光谱的偏移（用于Offset校准）
        /// </summary>
        /// <param name="darkPixelYDatas">暗电流像素光谱</param>
        /// <returns>光谱偏移量</returns>
        public static double GetOffsetCalCoefs(double[] darkPixelYDatas)
        {
            if (darkPixelYDatas == null || darkPixelYDatas.Length == 0)
                return 0;

            return darkPixelYDatas.Average();
        }

        
    }
}
