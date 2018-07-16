using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// panelScanner.xaml 的交互逻辑
    /// </summary>
    public partial class ClockWaiting : UserControl
    {
        private const bool isSimulate = true;

        /// <summary>
        /// 分针的角度，内部Binding使用
        /// </summary>
        private double MinuteAngle
        {
            get { return (double)GetValue(MinuteAngleProperty); }
            set { SetValue(MinuteAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinuteAngle.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty MinuteAngleProperty =
            DependencyProperty.Register("MinuteAngle", typeof(double), typeof(ClockWaiting), new PropertyMetadata(-90.0));

        /// <summary>
        /// 时针的角度，内部Binding使用
        /// </summary>
        private double HourAngle
        {
            get { return (double)GetValue(HourAngleProperty); }
            set { SetValue(HourAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourAngle.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty HourAngleProperty =
            DependencyProperty.Register("HourAngle", typeof(double), typeof(ClockWaiting), new PropertyMetadata(0.0));

        /// <summary>
        /// 取消按钮的不透明度（用于隐藏取消按钮）
        /// </summary>
        private double CancelOptical
        {
            get { return (double)GetValue(CancelOpticalProperty); }
            set { SetValue(CancelOpticalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelOptical.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty CancelOpticalProperty =
            DependencyProperty.Register("CancelOptical", typeof(double), typeof(ClockWaiting), new PropertyMetadata(1.0));
        
        /// <summary>
        /// 一圈总共多少小时  
        /// </summary>
        public double TotalHours
        {
            get { return (double)GetValue(TotalHoursProperty); }
            set { SetValue(TotalHoursProperty, value); }
        }

        /// <summary>
        /// 一圈总共多少小时  
        /// </summary>
        public static readonly DependencyProperty TotalHoursProperty =
            DependencyProperty.Register("TotalHours", typeof(double), typeof(ClockWaiting), new PropertyMetadata(24.0));

        /// <summary>
        /// 当前小时数
        /// </summary>
        public double CurrentHours
        {
            get { return (double)GetValue(CurrentHoursProperty); }
            set 
            {
                //if (timer == null)
                //    return;

                SetValue(CurrentHoursProperty, value);
                var angle = value * 360.0 / TotalHours;
                while (angle > 360)
                    angle -= 360;
                HourAngle = angle;

                string pathstr = "M15.0,15.0 L15.0,0";

                double r = 15;
                double begin = 0;
                double x = begin+Math.Sin(Math.PI * 2 * angle / 360.0) * r + r;
                double y = begin-Math.Cos(Math.PI * 2 * angle / 360.0) * r + r;

                string isbig = angle <= 180 ? "0" : "1";
                pathstr += "A15,15,0," + isbig + ",1," + x.ToString() + "," + y.ToString();

                pathstr += "Z";
                var ge = PathGeometry.Parse(pathstr);
                ProgressGeometry = ge;
            }
        }

        /// <summary>
        /// 当前小时数
        /// </summary>
        public static readonly DependencyProperty CurrentHoursProperty =
            DependencyProperty.Register("CurrentHours", typeof(double), typeof(ClockWaiting), new PropertyMetadata(0.0));

        /// <summary>
        /// 分针的速度(多少秒钟一圈)
        /// </summary>
        public double MinuteSpeed
        {
            get { return (double)GetValue(MinuteSpeedProperty); }
            set { SetValue(MinuteSpeedProperty, value); }
        }

        /// <summary>
        /// 分针的速度(多少秒钟一圈)
        /// </summary>
        public static readonly DependencyProperty MinuteSpeedProperty =
            DependencyProperty.Register("MinuteSpeed", typeof(double), typeof(ClockWaiting), new PropertyMetadata(1.0));

        /// <summary>
        /// 弧形几何形状，内部Binding使用
        /// </summary>
        private Geometry ProgressGeometry
        {
            get { return (Geometry)GetValue(ProgressGeometryProperty); }
            set { SetValue(ProgressGeometryProperty, value); }
        }

        /// <summary>
        /// 弧形几何形状，内部Binding使用
        /// </summary>
        private static readonly DependencyProperty ProgressGeometryProperty =
            DependencyProperty.Register("ProgressGeometry", typeof(Geometry), typeof(ClockWaiting), new PropertyMetadata(null));

        /// <summary>
        /// 是否能够终止扫描
        /// </summary>
        public bool CanAbort
        {
            get { return (bool)GetValue(CanAbortProperty); }
            set
            {
                CancelOptical = value ? 1.0 : 0;
                SetValue(CanAbortProperty, value);
            }
        }

        /// <summary>
        /// 是否能够终止扫描
        /// </summary>
        public static readonly DependencyProperty CanAbortProperty =
            DependencyProperty.Register("CanAbort", typeof(bool), typeof(ClockWaiting), new PropertyMetadata(true));
        
        /// <summary>
        /// 状态变更通知消息
        /// </summary>
        public static readonly RoutedEvent AbortEvent = EventManager.RegisterRoutedEvent("Abort",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ClockWaiting));
        /// <summary>
        /// 状态变更通知消息
        /// </summary>
        public event RoutedEventHandler Abort
        {
            add { AddHandler(AbortEvent, value); }
            remove { RemoveHandler(AbortEvent, value); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ClockWaiting()
        {
            InitializeComponent();
            scanProcess.DataContext = this;
        }

        /// <summary>
        /// 分针转动的控制Timer
        /// </summary>
        System.Windows.Threading.DispatcherTimer timer = null;

        /// <summary>
        /// 启动时间
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="totalHours">总共多少小时</param>
        /// <param name="minuteSpeed">分针多少秒钟一圈</param>
        public void Start(double totalHours, double minuteSpeed=1.0)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                this.TotalHours = totalHours;
                this.MinuteSpeed = minuteSpeed;
                this.MinuteAngle = -90;
            });

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;

            //获得分针运动的速度：秒数和毫秒数
            double minute = minuteSpeed / 8.0;
            timer.Interval = new TimeSpan(0, 0, 0, (int)Math.Truncate(minute) , ((int)Math.Truncate(minute*1000) % 1000));
            startTime = DateTime.Now;
            timer.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if(timer != null)
                timer.Stop();
            timer = null;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            var diff = DateTime.Now - startTime;
            Dispatcher.BeginInvoke((Action)delegate()
            {
                MinuteAngle = diff.Milliseconds * 360 / (MinuteSpeed * 1000) - 90;
            });
        }

        /// <summary>
        /// 引发事件
        /// </summary>
        private void RaiseAbortEvent()
        {
            Dispatcher.BeginInvoke((Action)delegate()
            {
                RoutedEventArgs args = new RoutedEventArgs(AbortEvent);
                args.Source = this;
                RaiseEvent(args);
                if (timer == null)
                    imgDelete.Visibility = Visibility.Collapsed;
            });
        }

        private void imgDelete_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(CanAbort)
                RaiseAbortEvent();
        }

        private void scanProcess_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //还没有开始
            if (timer == null || !CanAbort)
                return;

            if (imgDelete.Visibility == System.Windows.Visibility.Visible)
                imgDelete.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                double x = scanProcess.ActualWidth * 0.4;
                double w = scanProcess.ActualWidth * 0.2;
                Point pt = e.GetPosition(scanProcess);

                if (pt.X > x && pt.X < x + w && pt.Y > x && pt.Y < x + w)
                    imgDelete.Visibility = Visibility.Visible;
            }
        }
    }
}
