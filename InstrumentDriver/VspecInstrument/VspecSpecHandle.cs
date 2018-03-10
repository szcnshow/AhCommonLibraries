using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ai.Hong.CommonLibrary;

namespace VspecInstrument
{
    public class VspecSpecHandle : FTNirInterface.SpectrumHandle
    {
        /// <summary>
        /// 光谱操作对象
        /// </summary>
        private SpecFileFormatDouble objHandle;

        /// <summary>
        /// 模型分析操作对象
        /// </summary>
        private  eftirPLSTypeLib.eftirPLSObject plsObj;

        private string ErrorString { get; set; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public override string GetError()
        {
            return ErrorString;
        }

        /// <summary>
        /// 初始化操作对象
        /// </summary>
        public VspecSpecHandle()
        {
            objHandle = new SpecFileFormatDouble();
            plsObj = new eftirPLSTypeLib.eftirPLSObject();// eftirPLSObject();
        }
        /// <summary>
        /// 读取光谱
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public override bool ReadFile(string Path)
        {
            if (objHandle.ReadFile(Path))
            {
                XDatas = objHandle.XDatas;
                YDatas = objHandle.YDatas;
                ErrorString = objHandle.ErrorString;
                return false;
            }
            return true;
        }

        public override double[] XDatas { get; set; }

        public override double[] YDatas { get; set; }

        /// <summary>
        /// 存储光谱到文件
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="FilePara">Ai.Hong.CommonLibrary.FileParameter</param>
        /// <returns></returns>
        public override bool SaveFile(string Path, double[] YData)
        {
            // FileParameter para = FilePara as FileParameter;
            //if(para==null)
            //{
            //    ErrorString = "Type of FilePara Error！\r\nPlease Check！";
            //    return false;
            //}
            float[] Data = new float[YDatas.Length];
            for (int i = 0; i < YDatas.Count(); i++)
            {
                Data[i] = (float)YDatas[i];
            }
            if (!SPCFile.SaveFile(Path, Data, objHandle.Parameter))
            {
                ErrorString = SPCFile.ErrorString;
                return false;
            }
            return true;
        }



        /// <summary>
        /// 加载模型 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override bool LoadMethod(string path)
        {
            if (plsObj == null)
            {
                ErrorString = "PLSObject Init Error!";
                return false;
            }
            int index = plsObj.LoadMethod(path);
            if (index != 0)
            {
                ErrorMessage(index);
            }
            return index == 0;
        }

        /// <summary>
        /// 分析光谱得到数据
        /// </summary>
        /// <param name="spcPath">光谱路径</param>
        /// <returns></returns>
        public override string Analysis(string spcPath)
        {
            if (plsObj == null)
            {
                ErrorString = "PLSObject Init Error!";
                return "";
            }
            string result = "";
            int index = plsObj.Predict(spcPath, ref result);
            if (index != 0)
            {
                ErrorMessage(index);
            }
            return result;
        }

        /// <summary>
        /// 调入模型分析并得到分析结果
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="spcPath"></param>
        /// <returns></returns>
        public  List<AnalysisResult> GetResultObj(string modelPath,string spcPath)
        {
            if (!LoadMethod(modelPath))
                return null;
            List<AnalysisResult> list = new List<AnalysisResult>();
            string result = Analysis(spcPath);
            if (string.IsNullOrEmpty(result))
                return null;
            foreach (var p in result.Split('\n'))
            {
                AnalysisResult res = new AnalysisResult();
                string[] tt=p.Split(';');
                res.name = tt[0];
                res.value = Convert.ToDouble(tt[1]);
                res.mah = Convert.ToDouble(tt[2]);
                res.spcResidual = Convert.ToDouble(tt[3]);
                res.fValue = Convert.ToDouble(tt[4]);
                res.fProbability = Convert.ToDouble(tt[5]);
                list.Add(res);
            }
            return list;
        }

        /// <summary>
        /// 由结果字符串获得结果对象
        /// </summary>
        /// <param name="spcPath"></param>
        /// <returns></returns>
        public List<AnalysisResult> GetResultObj(string result)
        {
            List<AnalysisResult> list = new List<AnalysisResult>();
            if (string.IsNullOrEmpty(result))
                return null;
            string[] analyte=result.Split('\n');
            if (analyte.Count() < 5)
            {
                ErrorString = "Analysis Result String Error!";
                return null;
            }
            foreach (var p in analyte)
            {
                AnalysisResult res = new AnalysisResult();
                string[] tt = p.Split(';');
                res.name = tt[0];
                res.value = Convert.ToDouble(tt[1]);
                res.mah = Convert.ToDouble(tt[2]);
                res.spcResidual = Convert.ToDouble(tt[3]);
                res.fValue = Convert.ToDouble(tt[4]);
                res.fProbability = Convert.ToDouble(tt[5]);
                list.Add(res);
            }
            return list;
        }


        /// <summary>
        /// 模型预测错误信息
        /// </summary>
        /// <param name="index"></param>
        private void ErrorMessage(int index)
        {
            switch (index)
            {
                case 2: ErrorString = "license_error"; break;
                case 3: ErrorString = "no_method_loaded"; break;
                case 4: ErrorString = "method_load_error"; break;
                case 5: ErrorString = "file_load_error"; break;
                case 6: ErrorString = "calibration_error"; break;
                case 7: ErrorString = "analysis_error"; break;
                default: return;
            }
        }


        public class AnalysisResult
        {
            /// <summary>
            /// 模型内部关联名称
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 组分预测值
            /// </summary>
            public double value { get; set; }

            /// <summary>
            /// 马氏距离
            /// </summary>
            public double mah { get; set; }

            /// <summary>
            /// 光谱残差
            /// </summary>
            public double spcResidual { get; set; }

            /// <summary>
            /// F值
            /// </summary>
            public double fValue { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public double fProbability { get; set; }

            /// <summary>
            /// 维数
            /// </summary>
            public int dimension { get; set; }

            /// <summary>
            /// 浓度单位
            /// </summary>
            public string unit { get; set; }
        }

    }

}
