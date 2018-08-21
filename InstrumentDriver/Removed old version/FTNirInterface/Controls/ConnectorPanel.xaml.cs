using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ai.Hong.Driver.Controls
{
    /// <summary>
    /// Connector.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectorPanel : UserControl
    {
        /// <summary>
        /// Status changed notify event
        /// </summary>
        public static readonly RoutedEvent StatusChangedEvent = EventManager.RegisterRoutedEvent("StatusChanged",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ConnectorPanel));
        /// <summary>
        /// Status changed notify event
        /// </summary>
        public event RoutedEventHandler StatusChanged
        {
            add { AddHandler(StatusChangedEvent, value); }
            remove { RemoveHandler(StatusChangedEvent, value); }
        }

        private EnumHardwareStatus _status = EnumHardwareStatus.NotFound;
        /// <summary>
        /// Device status
        /// </summary>
        public EnumHardwareStatus Status { get { return _status; } set { if (value != _status) { _status = value; RaiseStatusEvent(); } } }

        /// <summary>
        /// 状态刷新频率(second)
        /// </summary>
        public int StatusRefreshRate = 5;

        /// <summary>
        /// 当前连接的设备
        /// </summary>
        public FTDriver ConnectedDevice;

        /// <summary>
        /// Last device info from config
        /// </summary>
        private DeviceInfo lastDevice;

        /// <summary>
        /// Refresh timer
        /// </summary>
        private DispatcherTimer refreshTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectorPanel()
        {
            InitializeComponent();
            Loaded += Connector_Loaded;
        }

        private void Connector_Loaded(object sender, RoutedEventArgs e)
        {
            deviceStatus.Width = this.ActualHeight;
            deviceStatus.Height = this.ActualHeight;

            refreshTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, StatusRefreshRate) };
            refreshTimer.Tick += RefreshDeviceStatus;
        }

        /// <summary>
        /// 开始连接设备
        /// </summary>
        /// <param name="device">当前设备类型</param>
        /// <param name="lastDevice">上次连接的设备</param>
        public void StartConenct(FTDriver device, DeviceInfo lastDevice)
        {
            refreshImage.Visibility = Visibility.Visible;
            deviceStatus.Visibility = Visibility.Collapsed;
            ConnectedDevice = device;
            this.lastDevice = lastDevice;

            System.Threading.Thread connThread = new System.Threading.Thread(ConnectThread);
            connThread.Start();
        }

        /// <summary>
        /// Check and display connect message
        /// </summary>
        /// <param name="ipaddress"></param>
        private void ConnectThread(object ipaddress)
        {
            try
            {
                //停止状态刷新Timer
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    UpdateStatus(true, EnumHardwareStatus.NotFound, null);
                    refreshTimer.Stop();
                });

                //连接上次的设备
                if (lastDevice != null)
                {
                    UpdateStatus(true, EnumHardwareStatus.NotFound, "尝试连接"+lastDevice.Name);
                    if (ConnectedDevice.Connect(lastDevice) == true)
                    {
                        UpdateStatus(false, EnumHardwareStatus.OK, lastDevice.Name);
                        Dispatcher.BeginInvoke((Action)delegate () { refreshTimer.Start(); });
                        return;
                    }
                }

                //枚举系统设备
                UpdateStatus(true, EnumHardwareStatus.NotFound, "查找系统设备");
                var devices = ConnectedDevice.EnumerateDevices(null);

                //尝试逐个连接
                foreach(var dev in devices)
                {
                    UpdateStatus(true, EnumHardwareStatus.NotFound, "尝试连接" + dev.Name);
                    if (ConnectedDevice.Connect(dev) == true)
                    {
                        lastDevice = dev;
                        UpdateStatus(false, EnumHardwareStatus.OK, lastDevice.Name);
                        Dispatcher.BeginInvoke((Action)delegate () { refreshTimer.Start(); });
                        return;
                    }
                }

                lastDevice = null;
                UpdateStatus(false, EnumHardwareStatus.NotFound, "连接设备失败");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="refreshVisible">是否显示刷新图标</param>
        /// <param name="status">状态</param>
        /// <param name="message">信息</param>
        private void UpdateStatus(bool refreshVisible, EnumHardwareStatus status, string message)
        {
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                refreshImage.Visibility = refreshVisible ? Visibility.Visible : Visibility.Collapsed;
                deviceStatus.Visibility = refreshVisible ? Visibility.Collapsed : Visibility.Visible;
                switch (status)
                {
                    case EnumHardwareStatus.OK:
                    case EnumHardwareStatus.Busy:
                        deviceStatus.Fill = System.Windows.Media.Brushes.LawnGreen;
                        break;
                    case EnumHardwareStatus.Warning:
                        deviceStatus.Fill = System.Windows.Media.Brushes.DarkOrange;
                        break;
                    case EnumHardwareStatus.Fault:
                    case EnumHardwareStatus.NotReady:
                        deviceStatus.Fill = System.Windows.Media.Brushes.Red;
                        break;
                    case EnumHardwareStatus.NotFound:
                    default:
                        deviceStatus.Fill = System.Windows.Media.Brushes.Gray;
                        break;
                }

                deviceMessage.Text = message;

                //引发状态变更通知
                if(status != Status)
                {
                    Status = status;
                    RaiseStatusEvent();
                }
                this.InvalidateVisual();
            });
        }

        /// <summary>
        /// Refresh device health status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDeviceStatus(object sender, EventArgs e)
        {
            var status = ConnectedDevice.GetDeviceStatus();
            UpdateStatus(false, status, deviceMessage.Text);
        }

        /// <summary>
        /// 引发事件
        /// </summary>
        private void RaiseStatusEvent()
        {
            Dispatcher.BeginInvoke((Action)delegate ()
           {
               RoutedEventArgs args = new RoutedEventArgs(StatusChangedEvent);
               args.Source = this;
               RaiseEvent(args);
           });
        }
    }
}
