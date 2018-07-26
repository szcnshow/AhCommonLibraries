using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading;
using System.Windows.Threading;
using Ai.Hong.FileFormat;
using System.IO;

namespace Ai.Hong.Driver.Controls
{
    /// <summary>
    /// panelScanner.xaml 的交互逻辑
    /// </summary>
    public partial class ClockScannerPanel : UserControl
    {
        #region properties
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorString { get; set; }

        /// <summary>
        /// 扫描参数
        /// </summary>
        private ScanParameter parameter = null;

        /// <summary>
        /// 连接的设备
        /// </summary>
        private FTDriver scanner { get; set; }

        /// <summary>
        /// 用户取消扫描
        /// </summary>
        private bool userAbort = false;

        /// <summary>
        /// 当前扫描状态
        /// </summary>
        public EnumScanNotifyState ScanningState { get; set; }

        /// <summary>
        /// Status changed notify event
        /// </summary>
        public static readonly RoutedEvent NotifyEvent = EventManager.RegisterRoutedEvent("Notify",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ClockScannerPanel));
        /// <summary>
        /// Status changed notify event
        /// </summary>
        public event RoutedEventHandler Notify
        {
            add { AddHandler(NotifyEvent, value); }
            remove { RemoveHandler(NotifyEvent, value); }
        }

        /// <summary>
        /// 当前重复次数
        /// </summary>
        private int currentRepeat = 0;

        private List<FileFormat.FileFormat> ScannedDatas = null;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ClockScannerPanel()
        {
            InitializeComponent();
            scanProgress.Abort += scanProgress_Abort;
        }

        /// <summary>
        /// 取消扫描
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scanProgress_Abort(object sender, RoutedEventArgs e)
        {
            userAbort = true;
        }

        /// <summary>
        /// 模拟扫描
        /// </summary>
        private void SimulateScan()
        {
            ScanningState = EnumScanNotifyState.Scanning;
            int count = parameter.ScanCount;
            for(int i=0; i<count; i++)
            {
                Thread.Sleep(500);
                if(ScanCallback(EnumScanNotifyState.Scanning, count, i+1) == false)
                    break;
            }
            ScanningState = EnumScanNotifyState.OneFinished;
        }

        /// <summary>
        /// 启动扫描
        /// </summary>
        /// <param name="scanner">当前设备</param>
        /// <param name="parameter">扫描参数</param>
        public EnumScanNotifyState StartScan(FTDriver scanner, ScanParameter parameter)
        {
            if (scanner == null || parameter == null)
            {
                ScanningState = EnumScanNotifyState.ParameterError;
                return ScanningState;
            }

            this.parameter = parameter;
            this.scanner = scanner;

            //干涉图和单通道图不需要背景，否则检查没有背景光谱，或者背景光谱过期，提示错误
            if (parameter.ResultSpectrum != EnumResultSpectrum.Interfer && parameter.ResultSpectrum != EnumResultSpectrum.BackSingleBeam && parameter.ResultSpectrum != EnumResultSpectrum.SampleSingleBeam)
            {
                if (parameter.BackgroundSpectrum == null || (DateTime.Now - parameter.BackgroundTime).TotalMinutes > (int)parameter.BackgroundDuration)
                {
                    ScanningState = EnumScanNotifyState.Idel;
                    return EnumScanNotifyState.BackgroundError;
                }
            }
            currentRepeat = 0;

            //设置进度条
            scanProgress.Start(parameter.ScanCount);

            //设置扫描参数
            if (scanner.SetExperimentParemter(parameter) == false)
            {
                ErrorString = scanner.ErrorString;
                ScanningState = EnumScanNotifyState.ParameterError;
                return ScanningState;
            }

            //启动扫描,干涉图和背景单通道发送背景扫描命令，否则发送样品扫描命令
            var ret = scanner.SendAcquireCommand((parameter.ResultSpectrum==EnumResultSpectrum.Interfer || parameter.ResultSpectrum==EnumResultSpectrum.BackSingleBeam)  ? EnumAcquireCommand.BackStart: EnumAcquireCommand.SampleStart, ScanCallback);
            if (ret == false)
                ErrorString = scanner.ErrorString;

            ScannedDatas = new List<FileFormat.FileFormat>();

            ScanningState = ret ? EnumScanNotifyState.Scanning : EnumScanNotifyState.DeviceError;

            return ScanningState;
        }

        /// <summary>
        /// 重复扫描（第一次扫描需要调用StartScan）
        /// </summary>
        /// <returns></returns>
        private bool RepeatScan()
        {
            currentRepeat++;
            scanProgress.Start(parameter.ScanCount);

            //启动扫描
            var ret = scanner.SendAcquireCommand((parameter.ResultSpectrum == EnumResultSpectrum.Interfer || parameter.ResultSpectrum == EnumResultSpectrum.BackSingleBeam) ? EnumAcquireCommand.BackStart : EnumAcquireCommand.SampleStart, ScanCallback);
            if (ret == false)
                ErrorString = scanner.ErrorString;

            ScanningState = ret ? EnumScanNotifyState.Scanning : EnumScanNotifyState.DeviceError;

            return ret;
        }

        /// <summary>
        /// 扫描回调函数
        /// </summary>
        /// <param name="status">扫描错误代码</param>
        /// <param name="maxValue">总扫描次数</param>
        /// <param name="curValue">当前扫描次数</param>
        /// <returns>True=继续扫描, False=终止扫描</returns>
        bool ScanCallback(EnumScanNotifyState status, int maxValue, int curValue)
        {
            //每次扫描完成都通知驱动扫描线程退出, 判断完成一次扫描还是全部扫描
            if (userAbort)
                status = EnumScanNotifyState.UserAbort;
            else if(status == EnumScanNotifyState.OneFinished)  //一次扫描结束（扫描结束消息由驱动程序提供）
            {
                status = currentRepeat < parameter.RepeatCount ? EnumScanNotifyState.OneFinished : EnumScanNotifyState.RepeateFinished;
            }

            //另外启动一个线程来处理扫描通知
            Dispatcher.BeginInvoke((Action)delegate { DoScanProcessEvent(status, maxValue, curValue); });

            return userAbort == false && status == EnumScanNotifyState.Scanning;
        }

        private void DoScanProcessEvent(EnumScanNotifyState status, int maxValue, int curValue)
        {
            scanProgress.CurrentHours = curValue;
            ScanningState = status;
            switch (ScanningState)
            {
                case EnumScanNotifyState.OneFinished:   //本次扫描完成
                    ScannedDatas.AddRange(scanner.GetScanedDatas());
                    RaiseNofiyEvent(ScanningState);
                    
                    //有可能用户取消扫描，就不重复扫描了
                    if (userAbort == true)
                        ScanningState = EnumScanNotifyState.Idel;
                    else    //启动重复扫描
                    {
                        if (RepeatScan() == false)  //重复扫描出错了
                            RaiseNofiyEvent(ScanningState);
                    }
                    break;
                case EnumScanNotifyState.RepeateFinished:   //全部完成
                    ScannedDatas.AddRange(scanner.GetScanedDatas());
                    RaiseNofiyEvent(ScanningState);
                    ScanningState = EnumScanNotifyState.Idel;
                    break;
                default:
                    ErrorString = scanner.ErrorString;
                    RaiseNofiyEvent(ScanningState);
                    break;
            }

            if (ScanningState != EnumScanNotifyState.Scanning)
                scanProgress.Stop();
        }

        /// <summary>
        /// 引发事件
        /// </summary>
        private void RaiseNofiyEvent(EnumScanNotifyState state)
        {
            //发生消息后即刻返回，消息返回值处理由Dispatcher线程完成
            Dispatcher.BeginInvoke((Action)delegate()
            {
                ScanNotifyArgs args = new ScanNotifyArgs(NotifyEvent, state, ErrorString);
                RaiseEvent(args);
            });
        }

        /// <summary>
        /// 是否可以终止扫描（默认可以终止）
        /// </summary>
        public void CanAbort(bool can=true)
        {
            scanProgress.CanAbort = can;
        }

        private FileFormat.FileFormat LoadSpectrumFromResource(string resourceFile, string saveName, FileFormat.FileFormat.YAXISTYPE dataType)
        {
            System.Reflection.Assembly assemb = System.Reflection.Assembly.GetCallingAssembly();
            var bytes = Ai.Hong.Common.ResourceOperator.EmbededResourceBinary(assemb, resourceFile);
            FileFormat.FileFormat fmt = new FileFormat.FileFormat(bytes);
            fmt.dataInfo.dataType = dataType;
            fmt.fileInfo.filename = saveName;

            return fmt;
        }

        /// <summary>
        /// 获取扫描结果（包括结果谱图，单通道普通，干涉图）
        /// </summary>
        /// <returns>FileFormat列表</returns>
        public List<FileFormat.FileFormat> GetResult()
        {
            return ScannedDatas;
        }
    }
}
