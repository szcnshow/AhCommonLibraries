using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Ai.Hong.Driver.IT;
using Ai.Hong.Driver;

namespace Ai.Hong.Driver.IT
{
    /// <summary>
    /// 激光波数校准
    /// </summary>
    public partial class DlgLaserCorrect : Window
    {
        /// <summary>
        /// 激光波数校准参数
        /// </summary>
        private LaserWavelengthTestInfo laserInfo = null;

        /// <summary>
        /// 当前设备
        /// </summary>
        private FTDriver scanner = null;

        /// <summary>
        /// 光谱保存路径
        /// </summary>
        private string savePath = null;

        /// <summary>
        /// 激光波数校准构造函数
        /// </summary>
        public DlgLaserCorrect(FTDriver scanner, LaserWavelengthTestInfo laserInfo, string savePath)
        {
            System.Diagnostics.Debug.Assert(scanner != null && laserInfo != null && string.IsNullOrWhiteSpace(savePath) == false);

            InitializeComponent();
            this.Closing += dlgLaserCorrect_Closing;
            this.scanner = scanner;
            this.laserInfo = laserInfo;
            this.savePath = savePath;
            scanProgress.Notify += scanProgress_Notify;
        }

        private void scanProgress_Notify(object sender, RoutedEventArgs e)
        {
            ScanNotifyArgs args = e as ScanNotifyArgs;
            if (args == null)
                return;

            //用户取消
            if(args.State == EnumScanNotifyState.UserAbort)
            {
                btnStartScan.Visibility = System.Windows.Visibility.Visible;
                scanProgress.Visibility = System.Windows.Visibility.Collapsed;
                args.AbortScan = true;
                return;
            }

            if (args.State == EnumScanNotifyState.RepeateFinished && laserInfo.CalculateResult() == true)
            {
                args.AbortScan = true;
                this.DialogResult = true;
                this.Close();
            }
            else if(args.State == EnumScanNotifyState.RepeateFinished)
            {
                args.AbortScan = true;
                this.DialogResult = false;
                this.Close();
            }
        }

        void dlgLaserCorrect_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (scanProgress.ScanningState == EnumScanNotifyState.Scanning)
                e.Cancel = true;
        }

        private void btnStartScan_Clicked(object sender, RoutedEventArgs e)
        {
            btnStartScan.Visibility = System.Windows.Visibility.Collapsed;
            scanProgress.Visibility = System.Windows.Visibility.Visible;
            scanProgress.StartScan(scanner, laserInfo.AcquireParameter);
        }

    }
}
