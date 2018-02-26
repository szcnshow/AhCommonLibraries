using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// OkCancelPanel.xaml 的交互逻辑
    /// </summary>
    public partial class OkCancelPanel : UserControl
    {
        /// <summary>
        /// OK Clicked Event
        /// </summary>
        public static readonly RoutedEvent OKEvent = EventManager.RegisterRoutedEvent("OKClicked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OkCancelPanel));
        /// <summary>
        /// OK Clicked Event
        /// </summary>
        public event RoutedEventHandler OKClicked
        {
            add { AddHandler(OKEvent, value); }
            remove { RemoveHandler(OKEvent, value); }
        }

        /// <summary>
        /// Cancel Clicked Event
        /// </summary>
        public static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent("CancelClicked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OkCancelPanel));
        /// <summary>
        /// Cancel Clicked Event
        /// </summary>
        public event RoutedEventHandler CancelClicked
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public OkCancelPanel()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = OKEvent;
            args.Source = this;
            RaiseEvent(args);

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = CancelEvent;
            args.Source = this;
            RaiseEvent(args);
        }

        /// <summary>
        /// 设置OK按钮状态
        /// </summary>
        /// <param name="isEnabled"></param>
        public void SetOKButtonEnabled(bool isEnabled)
        {
            btnOk.IsEnabled = isEnabled;
        }

        /// <summary>
        /// 设置Cancel按钮状态
        /// </summary>
        /// <param name="isEnabled"></param>
        public void SetCancelButtonEnabled(bool isEnabled)
        {
            btnCancel.IsEnabled = isEnabled;
        }
    }
}
