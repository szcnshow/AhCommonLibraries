using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// TextBoxAndButton.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxAndButton : UserControl
    {
        /// <summary>
        /// 输入内容
        /// </summary>
        public string TextProperty
        {
            get { return (string)GetValue(TextPropertyProperty); }
            set { SetValue(TextPropertyProperty, value); }
        }

        /// <summary>
        /// 输入内容
        /// </summary>
        public static readonly DependencyProperty TextPropertyProperty =
            DependencyProperty.Register("TextProperty", typeof(string), typeof(TextBoxAndButton), new UIPropertyMetadata(null));

        /// <summary>
        /// 按钮消息
        /// </summary>
        public static readonly RoutedEvent BrowseClickedEvent = EventManager.RegisterRoutedEvent("BrowseClicked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBoxAndButton));
        /// <summary>
        /// 按钮消息
        /// </summary>
        public event RoutedEventHandler BrowseClicked
        {
            add { AddHandler(BrowseClickedEvent, value); }
            remove { RemoveHandler(BrowseClickedEvent, value); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextBoxAndButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Clicked(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(BrowseClickedEvent, this);
            RaiseEvent(args);
        }

        
    }
}
