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

namespace Ai.Hong.Driver
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
        /// 是否为背景光谱
        /// </summary>
        public bool IsBackground = true;

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
            int count = parameter.Count;
            for(int i=0; i<count; i++)
            {
                Thread.Sleep(500);
                if(ScanCallback(Driver.EnumHardwareError.OK, count, i+1) == false)
                    break;
            }
            ScanningState = EnumScanNotifyState.oneFinished;
        }

        /// <summary>
        /// 启动扫描
        /// </summary>
        /// <param name="scanner">当前设备</param>
        /// <param name="parameter">扫描参数</param>
        /// <param name="isBackground">True=扫描背景, False=扫描样品</param>
        public EnumScanNotifyState StartScan(FTDriver scanner, ScanParameter parameter, bool isBackground)
        {
            this.parameter = parameter;
            this.scanner = scanner;
            this.IsBackground = isBackground;

            //扫描样品时，没有背景光谱，或者背景光谱过期，提示错误
            if (isBackground == false && (parameter.BackgroundSpectrum == null || 
                (DateTime.Now - parameter.BackgroundTime).TotalMinutes > (int)parameter.BackgroundDuration))
            {
                ScanningState = EnumScanNotifyState.Idel;
                return EnumScanNotifyState.backgroundError;
            }

            currentRepeat = 0;
            scanProgress.Start(parameter.Count);

            if(scanner == null || parameter == null)
            {
                ScanningState = EnumScanNotifyState.parameterError;
                return ScanningState;
            }

            //设置扫描参数
            if (scanner.SetExperimentParemter(parameter) == false)
            {
                ErrorString = scanner.ErrorString;
                ScanningState = EnumScanNotifyState.parameterError;
                return ScanningState;
            }

            //启动扫描
            scanner.ProcessCallback = ScanCallback;
            var ret = scanner.SendAcquireCommand( EnumAcquireCommand.Start);
            if (ret == false)
                ErrorString = scanner.ErrorString;

            ScanningState = ret ? EnumScanNotifyState.Scanning : EnumScanNotifyState.deviceError;

            return ScanningState;
        }

        /// <summary>
        /// 重复扫描（第一次扫描需要调用StartScan）
        /// </summary>
        /// <returns></returns>
        private bool RepeatScan()
        {
            currentRepeat++;
            scanProgress.Start(parameter.Count);

            //启动扫描
            scanner.ProcessCallback = ScanCallback;
            var ret = scanner.SendAcquireCommand(EnumAcquireCommand.Start);
            if (ret == false)
                ErrorString = scanner.ErrorString;

            ScanningState = ret ? EnumScanNotifyState.Scanning : EnumScanNotifyState.deviceError;

            return ret;
        }

        /// <summary>
        /// 扫描回调函数
        /// </summary>
        /// <param name="errorCode">扫描错误代码</param>
        /// <param name="maxValue">总扫描次数</param>
        /// <param name="curValue">当前扫描次数</param>
        /// <returns>True=继续扫描, False=终止扫描</returns>
        bool ScanCallback(Driver.EnumHardwareError errorCode, int maxValue, int curValue)
        {
            Dispatcher.BeginInvoke((Action)delegate()
            {
                scanProgress.CurrentHours = curValue;
            });

            //扫描出现错误，或者扫描结束
            ErrorString = null;
            if (errorCode != Driver.EnumHardwareError.OK) //扫描出现错误
            {
                scanProgress.Stop();
                ErrorString = scanner.ErrorString;
                ScanningState = EnumScanNotifyState.deviceError;
                RaiseNofiyEvent(ScanningState);
            }
            else if (userAbort)  //用户取消
            {
                scanProgress.Stop();
                ErrorString = "User Abort Scanning";
                ScanningState = EnumScanNotifyState.userAbort;
                RaiseNofiyEvent(ScanningState);
            }
            else if (maxValue == curValue)   //完成一次扫描
            {
                scanProgress.Stop();
                ScanningState = currentRepeat < parameter.Repeat ? EnumScanNotifyState.oneFinished : EnumScanNotifyState.repeateFinished;

                //不管重复扫描是否完成，都要结束本次扫描，由RaiseNofiyEvent开启重复扫描
                RaiseNofiyEvent(ScanningState);
            }

            return ScanningState == EnumScanNotifyState.Scanning;
        }

        /// <summary>
        /// 引发事件
        /// </summary>
        private void RaiseNofiyEvent(EnumScanNotifyState state)
        {
            scanProgress.Stop();

            //发生消息后即刻返回，消息返回值处理由Dispatcher线程完成
            Dispatcher.BeginInvoke((Action)delegate()
            {
                ScanNotifyArgs args = new ScanNotifyArgs(NotifyEvent, state, ErrorString);
                RaiseEvent(args);

                //如果是重复扫描，还没有到达重复扫描的次数
                if (args.abortScan == false && args.state == EnumScanNotifyState.oneFinished)
                {
                    Thread.Sleep(10);   //暂停10ms等待当前扫描线程结束
                    RepeatScan();   //启动新的扫描
                }
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
            return scanner.ScannedDatas;
        }
    }
}
