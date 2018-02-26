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
using System.Windows.Controls.Primitives;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// GraphicOperatePanel.xaml 的交互逻辑
    /// </summary>
    public partial class FileOperatePanel : UserControl
    {
        /// <summary>
        /// 按钮大小
        /// </summary>
        public double ButtonSize
        {
            get { return (double)GetValue(ButtonSizeProperty); }
            set { SetValue(ButtonSizeProperty, value); }
        }
        /// <summary>
        /// 按钮大小
        /// </summary>
        public static readonly DependencyProperty ButtonSizeProperty =
            DependencyProperty.Register("ButtonSize", typeof(double), typeof(FileOperatePanel), new PropertyMetadata(16.0));

        /// <summary>
        /// 按钮颜色
        /// </summary>
        public SolidColorBrush ButtonColor
        {
            get { return (SolidColorBrush)GetValue(ButtonColorProperty); }
            set { SetValue(ButtonColorProperty, value); }
        }
        /// <summary>
        /// 按钮颜色
        /// </summary>
        public static readonly DependencyProperty ButtonColorProperty =
            DependencyProperty.Register("ButtonColor", typeof(SolidColorBrush), typeof(FileOperatePanel), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// 排列方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        /// <summary>
        /// 排列方向
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FileOperatePanel), new UIPropertyMetadata(Orientation.Vertical));

        /// <summary>
        /// MouseOn颜色
        /// </summary>
        public SolidColorBrush MouseOnColor
        {
            get { return (SolidColorBrush)GetValue(MouseOnColorProperty); }
            set { SetValue(MouseOnColorProperty, value); }
        }
        /// <summary>
        /// MouseOn颜色
        /// </summary>
        public static readonly DependencyProperty MouseOnColorProperty =
            DependencyProperty.Register("MouseOnColor", typeof(SolidColorBrush), typeof(FileOperatePanel), new UIPropertyMetadata(Brushes.BlueViolet));

        /// <summary>
        /// 选中颜色
        /// </summary>
        public SolidColorBrush CheckedColor
        {
            get { return (SolidColorBrush)GetValue(CheckedColorProperty); }
            set { SetValue(CheckedColorProperty, value); }
        }
        /// <summary>
        /// 选中颜色
        /// </summary>
        public static readonly DependencyProperty CheckedColorProperty =
            DependencyProperty.Register("CheckedColor", typeof(SolidColorBrush), typeof(FileOperatePanel), new UIPropertyMetadata(Brushes.Red));

        /// <summary>
        /// Disabled颜色
        /// </summary>
        public SolidColorBrush DisabledColor
        {
            get { return (SolidColorBrush)GetValue(DisabledColorProperty); }
            set { SetValue(DisabledColorProperty, value); }
        }

        /// <summary>
        /// Disabled颜色
        /// </summary>
        public static readonly DependencyProperty DisabledColorProperty =
            DependencyProperty.Register("DisabledColor", typeof(SolidColorBrush), typeof(FileOperatePanel), new UIPropertyMetadata(Brushes.Gray));

        /// <summary>
        /// 按钮Margin
        /// </summary>
        public Thickness ButtonMargin
        {
            get { return (Thickness)GetValue(ButtonMarginProperty); }
            set { SetValue(ButtonMarginProperty, value); }
        }
        /// <summary>
        /// 按钮Margin
        /// </summary>
        public static readonly DependencyProperty ButtonMarginProperty =
            DependencyProperty.Register("ButtonMargin", typeof(Thickness), typeof(FileOperatePanel), new PropertyMetadata(new Thickness(3)));

        /// <summary>
        /// 按钮选中
        /// </summary>
        public static readonly RoutedEvent ButtonCheckedEvent = EventManager.RegisterRoutedEvent("ButtonChecked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileOperatePanel));
        /// <summary>
        /// 按钮选中
        /// </summary>
        public event RoutedEventHandler ButtonChecked
        {
            add { AddHandler(ButtonCheckedEvent, value); }
            remove { RemoveHandler(ButtonCheckedEvent, value); }
        }

        /// <summary>
        /// 按钮点击
        /// </summary>
        public static readonly RoutedEvent ButtonClickedEvent = EventManager.RegisterRoutedEvent("ButtonClicked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileOperatePanel));
        /// <summary>
        /// 按钮点击
        /// </summary>
        public event RoutedEventHandler ButtonClicked
        {
            add { AddHandler(ButtonClickedEvent, value); }
            remove { RemoveHandler(ButtonClickedEvent, value); }
        }

        /// <summary>
        /// 所有按钮
        /// </summary>
        List<Button> allButtons = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileOperatePanel()
        {
            InitializeComponent();
            allButtons = new List<Button>(){
                btnOpen, btnSave, btnSaveAs, btnSaveAll, btnClose, btnCloseAll, btnUndo, btnImport, btnExport
            };
        }

        /// <summary>
        /// 按钮点击消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newarg = new RoutedEventArgs();
            newarg.RoutedEvent = ButtonClickedEvent;
            newarg.Source = sender;
            RaiseEvent(newarg);
        }

        /// <summary>
        /// Enable/Disable按钮
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        /// <param name="enabled"></param>
        public void EnableButton(string buttonName, bool enabled)
        {
            foreach(var item in allButtons)
            {
                if (item.Name == buttonName)
                    item.IsEnabled = enabled;
            }
        }

        /// <summary>
        /// 设置按钮可见状态
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        /// <param name="visible">是否可见</param>
        public void VisibleButton(string buttonName, bool visible)
        {
            foreach (var item in allButtons)
            {
                if (item.Name == buttonName)
                    item.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 获取所有按钮
        /// </summary>
        /// <returns></returns>
        public List<Button> GetAllButtons()
        {
            return allButtons;
        }
    }
}
