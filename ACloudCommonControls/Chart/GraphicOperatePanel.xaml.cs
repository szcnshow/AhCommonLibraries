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

namespace Ai.Hong.Charts
{
    /// <summary>
    /// GraphicOperatePanel.xaml 的交互逻辑
    /// </summary>
    public partial class GraphicOperatePanel : UserControl
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
            DependencyProperty.Register("ButtonSize", typeof(double), typeof(GraphicOperatePanel), new PropertyMetadata(24.0));

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
            DependencyProperty.Register("ButtonColor", typeof(SolidColorBrush), typeof(GraphicOperatePanel), new PropertyMetadata(Brushes.Black));

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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GraphicOperatePanel), new UIPropertyMetadata(Orientation.Vertical));

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
            DependencyProperty.Register("MouseOnColor", typeof(SolidColorBrush), typeof(GraphicOperatePanel), new UIPropertyMetadata(Brushes.BlueViolet));

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
            DependencyProperty.Register("CheckedColor", typeof(SolidColorBrush), typeof(GraphicOperatePanel), new UIPropertyMetadata(Brushes.Red));

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
            DependencyProperty.Register("DisabledColor", typeof(SolidColorBrush), typeof(GraphicOperatePanel), new UIPropertyMetadata(Brushes.Gray));

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
            DependencyProperty.Register("ButtonMargin", typeof(Thickness), typeof(GraphicOperatePanel), new PropertyMetadata(new Thickness(3)));

        /// <summary>
        /// 按钮选中
        /// </summary>
        public static readonly RoutedEvent ButtonCheckedEvent = EventManager.RegisterRoutedEvent("ButtonChecked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GraphicOperatePanel));
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
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GraphicOperatePanel));
        /// <summary>
        /// 按钮点击
        /// </summary>
        public event RoutedEventHandler ButtonClicked
        {
            add { AddHandler(ButtonClickedEvent, value); }
            remove { RemoveHandler(ButtonClickedEvent, value); }
        }

        /// <summary>
        /// 颜色改变
        /// </summary>
        public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent("ColorChanged",
                RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<SolidColorBrush>), typeof(GraphicOperatePanel));
        /// <summary>
        /// 颜色改变
        /// </summary>
        public event RoutedPropertyChangedEventHandler<SolidColorBrush> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        /// <summary>
        /// 所有按钮
        /// </summary>
        List<ButtonBase> allButtons = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GraphicOperatePanel()
        {
            InitializeComponent();
            allButtons = new List<ButtonBase>(){
                btnSelect, btnMove, btnZoomIn, btnZoomOut, btnInformation, btnUpPeakPick, btnDownPeakPick,
                btnSizeAll, btnSizeYAxis, btnColor, btnHide, btnGridShow
            };

            //默认情况峰位标注按钮隐藏
            btnUpPeakPick.Visibility = System.Windows.Visibility.Collapsed;
            btnDownPeakPick.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Radio按钮消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Checked(object sender, RoutedEventArgs e)
        {
            if(sender is RadioButton )
            {
                RoutedEventArgs newarg = new RoutedEventArgs();
                newarg.RoutedEvent = ButtonCheckedEvent;
                newarg.Source = sender;
                RaiseEvent(newarg);
            }
        }

        /// <summary>
        /// 按钮点击消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender == btnColor)
            {
                Ai.Hong.Controls.ColorPicker colorPop = new Ai.Hong.Controls.ColorPicker();
                colorPop.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                colorPop.Closed += ColorPicker_Closed;
                colorPop.IsOpen = true;
            }
            else if (sender is Button)
            {
                RoutedEventArgs newarg = new RoutedEventArgs();
                newarg.RoutedEvent = ButtonClickedEvent;
                newarg.Source = sender;
                RaiseEvent(newarg);
            }
        }

        void ColorPicker_Closed(object sender, EventArgs e)
        {
            Ai.Hong.Controls.ColorPicker colorPop = sender as Ai.Hong.Controls.ColorPicker;
            if (colorPop == null || colorPop.selectedBursh == null)
                return;

            RoutedPropertyChangedEventArgs<SolidColorBrush> args = new RoutedPropertyChangedEventArgs<SolidColorBrush>(null, (SolidColorBrush)colorPop.selectedBursh);
            args.RoutedEvent = ColorChangedEvent;
            RaiseEvent(args);
        }

        /// <summary>
        /// 获取所有Radio按钮
        /// </summary>
        /// <returns></returns>
        public List<RadioButton> GetRadioButtons()
        {
            return new List<RadioButton>() { btnSelect, btnMove, btnZoomIn, btnInformation, btnUpPeakPick, btnDownPeakPick };
        }

        /// <summary>
        /// 获取所有按钮
        /// </summary>
        /// <returns></returns>
        public List<Button> GetClickButtons()
        {
            return new List<Button>() { btnZoomOut, btnSizeAll, btnSizeYAxis, btnColor, btnHide, btnGridShow };
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
        /// 选中按钮
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        public void CheckButton(string buttonName)
        {
            foreach (var item in allButtons)
            {
                if (item.Name == buttonName && item is RadioButton)
                    (item as RadioButton).IsChecked = true;
            }
        }

        /// <summary>
        /// 选中按钮
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        /// <param name="visible">是否可见</param>
        public void VisibleButton(string buttonName, bool visible)
        {
            foreach (var item in allButtons)
            {
                if (item.Name == buttonName && item is RadioButton)
                    (item as RadioButton).IsChecked = true;
            }
        }

        /// <summary>
        /// 获取所有按钮
        /// </summary>
        /// <returns></returns>
        public List<ButtonBase> GetAllButtons()
        {
            return allButtons;
        }
    }
}
