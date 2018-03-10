using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VspecInstrument.Types
{
    public class IntegratingSphereTrans:InstrumentTypeBase
    {
        /// <summary>
        /// 波数精度-聚苯乙烯-温度校正
        /// </summary>
        /// <param name="targetPeak"></param>
        /// <returns></returns>
        public override double? TemperatureCalibrate(double targetPeak)
        {
            if (!isConnected)
            {
                errorCode = -11;
                return null;
            }
            string paraString = ReadSensors();
            if (paraString != null)
            {
                JsonString.GetSensors senser = JsonString.JsonToObj<JsonString.GetSensors>(paraString);
                //if (IsMoveFlagBack())
                //{
                //System.Windows.Forms.MessageBox.Show("当前是积分球仪器\r\n标定峰位： " + targetPeak.ToString() + "\r\n温度：" + senser.sensors[2].val.ToString() + "\r\n扣除温度偏差(4571 * targetPeak) / (4571.575 - 0.0205 * senser.sensors[1].val): " + ((4571 * targetPeak) / (4571.575 - 0.0205 * senser.sensors[1].val)).ToString());
                return (4571 * targetPeak) / (4571.575 - 0.0205 * senser.sensors[1].val);
                //}
                //else
                //{
                //    devTemperature = Convert.ToDouble(senser.sensors[1].val) * 0.0107 - 0.7;
                //    return targetPeak += devTemperature;
                //}
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 移动积分球位置 0 for sample.1 for background
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override bool? MoveFlag(int position)
        {

            if (!isConnected)
            {
                errorCode = -11;
                return false;
            }

            errorCode = instrumentObject.MoveFlag(position);
            return errorCode == 0;
        }

        /// <summary>
        /// 判断不控温样品池(透射) 是否放有样品
        /// </summary>
        /// <returns></returns>
        public override bool? IsTransmissionCellEmpty() 
        {
            string para = ReadSensors();
            if (para != null)
            {
                JsonString.GetSensors senser = JsonString.JsonToObj<JsonString.GetSensors>(para);
                if(senser==null)
                {
                    errorCode = -20;
                    return false;
                }
                double res = (from p in senser.sensors where p.id == 7 select p.val).FirstOrDefault();
                switch((int)res)
                {
                    case 0: return true;
                    case 1: return false;
                    default: return null;
                }
            }
            return null; 
        }

        /// <summary>
        /// 扫描背景
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">背景保存文件</param>
        public override string ScanBackground(string scanMethodFile, int scanCount, string backgroundFile, string addPara = null)
        {
            //移动积分球
            MoveFlag(1);

            //加载扫描配置
            errorCode = instrumentObject.LoadSettings(scanMethodFile);
            if (errorCode != 0)
            {
                errorCode = -3;
                return null;
            }
            int scans = scanCount;
            if (scans == 0)
                scans = Convert.ToInt32(Ai.Hong.CommonMethod.ReadIniFile(scanMethodFile, "Collection", "backgroundScans"));
            errorCode = instrumentObject.CollectBackground(scans, backgroundFile);
            if (errorCode != 0)
            {
                errorCode = -12;
                return null;
            }
            DeleteExtentedFile(backgroundFile, "_rif.spc");
            //Rename _rsb System.Windows.Forms.MessageBox.Show("Delete");.spc to .spc file
            string temp = backgroundFile.ToLower().Replace(".spc", "_rsb.spc");
            if (File.Exists(backgroundFile))
                File.Delete(backgroundFile);
            if (File.Exists(temp))
                File.Move(temp, backgroundFile);
            return backgroundFile;
        }

        /// <summary>
        /// 扫描样品
        /// </summary>
        /// <param name="scanMethodFile">扫描配置文件</param>
        /// <param name="scanCount">扫描次数</param>
        /// <param name="backgroundFile">样品保存文件</param>
        public override string ScanSample(string scanMethodFile, int scanCount, string sampleFile, string addPara = null)
        {
            //by 艾宏
            //转轮转动(转轮在外面设置)
            //SampleSpinner(1);

            if (errorCode != 0)
                return null;
            //加载扫描配置
            errorCode = instrumentObject.LoadSettings(scanMethodFile);
            if (errorCode != 0)
                return null;

            int scans = scanCount;
            if (scans == 0)
                scans = Convert.ToInt32(Ai.Hong.CommonMethod.ReadIniFile(scanMethodFile, "Collection", "sampleScans"));
            errorCode = instrumentObject.CollectSpectrum(scans, sampleFile);
            SampleSpinner(0);
            if (errorCode != 0)
                return null;

            DeleteExtentedFile(sampleFile, "_ifg.spc");
            DeleteExtentedFile(sampleFile, "_trn.spc");

            //Rename _rsb.spc to .spc file
            string tempstr = sampleFile.ToLower().Replace(".spc", "_sbm.spc");

            if (File.Exists(sampleFile))
                File.Delete(sampleFile);

            if (File.Exists(tempstr))
                File.Move(tempstr, sampleFile);

            return sampleFile;

        }
    }
}
