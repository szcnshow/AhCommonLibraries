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
    /// ImageToolButton.xaml 的交互逻辑
    /// </summary>
    public partial class VectorImageButton : UserControl
    {
        /// <summary>
        /// 按钮的文字
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VectorImageButton), new UIPropertyMetadata(null));

        /// <summary>
        /// 按钮的图片
        /// </summary>
        public DrawingGroup VectorGeometry
        {
            get { return (DrawingGroup)GetValue(VectorGeometryProperty); }
            set { SetValue(VectorGeometryProperty, value); }
        }
        /// <summary>
        /// 按钮的图片
        /// </summary>
        public static readonly DependencyProperty VectorGeometryProperty =
            DependencyProperty.Register("VectorGeometry", typeof(DrawingGroup), typeof(VectorImageButton), new UIPropertyMetadata(null));

        /// <summary>
        /// 按钮的图片的宽度
        /// </summary>
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }
        /// <summary>
        /// 按钮的图片
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(VectorImageButton), new PropertyMetadata(16.0));

        /// <summary>
        /// 按钮的图片的高度
        /// </summary>
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
        /// <summary>
        /// 按钮的图片
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(VectorImageButton), new PropertyMetadata(16.0));

        /// <summary>
        /// 图片的空白
        /// </summary>
        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }
        /// <summary>
        /// 图片的空白
        /// </summary>
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(VectorImageButton), new UIPropertyMetadata(new Thickness(0)));

        /// <summary>
        /// 按钮的图片的宽度
        /// </summary>
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        /// <summary>
        /// 按钮的图片
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(VectorImageButton), new PropertyMetadata(0.0));

        /// <summary>
        /// 内容Padding
        /// </summary>
        public Thickness ContentPadding
        {
            get { return (Thickness)GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }
        /// <summary>
        /// 内容Padding
        /// </summary>
        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register("ContentPadding", typeof(Thickness), typeof(VectorImageButton), new UIPropertyMetadata(new Thickness(0)));

        /// <summary>
        /// 文字的空白
        /// </summary>
        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        /// <summary>
        /// 文字的空白
        /// </summary>
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(VectorImageButton), new UIPropertyMetadata(new Thickness(5, 0, 0, 0)));

        /// <summary>
        /// 鼠标上去的颜色
        /// </summary>
        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        /// <summary>
        /// 鼠标上去的颜色
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(VectorImageButton), new UIPropertyMetadata(Brushes.WhiteSmoke));

        /// <summary>
        /// 鼠标上去的边框颜色
        /// </summary>
        public Brush SelectedBorder
        {
            get { return (Brush)GetValue(SelectedBorderProperty); }
            set { SetValue(SelectedBorderProperty, value); }
        }
        /// <summary>
        /// 鼠标上去的边框颜色
        /// </summary>
        public static readonly DependencyProperty SelectedBorderProperty =
            DependencyProperty.Register("SelectedBorder", typeof(Brush), typeof(VectorImageButton), new UIPropertyMetadata(Brushes.White));

        /// <summary>
        /// 鼠标没上去的颜色
        /// </summary>
        public Brush UnselectedColor
        {
            get { return (Brush)GetValue(UnselectedColorProperty); }
            set { SetValue(UnselectedColorProperty, value); }
        }
        /// <summary>
        /// 鼠标没上去的颜色
        /// </summary>
        public static readonly DependencyProperty UnselectedColorProperty =
            DependencyProperty.Register("UnselectedColor", typeof(Brush), typeof(VectorImageButton),
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(05, 255, 255, 255))));

        /// <summary>
        /// 鼠标没上去的边框颜色
        /// </summary>
        public Brush UnselectedBorder
        {
            get { return (Brush)GetValue(UnselectedBorderProperty); }
            set { SetValue(UnselectedBorderProperty, value); }
        }
        /// <summary>
        /// 鼠标没上去的边框颜色
        /// </summary>
        public static readonly DependencyProperty UnselectedBorderProperty =
            DependencyProperty.Register("UnselectedBorder", typeof(Brush), typeof(VectorImageButton), new UIPropertyMetadata(null));

        /// <summary>
        /// 正常图形颜色
        /// </summary>
        public SolidColorBrush ImageColor
        {
            get { return (SolidColorBrush)GetValue(ImageColorProperty); }
            set { SetValue(ImageColorProperty, value); }
        }
        /// <summary>
        /// 正常图形颜色
        /// </summary>
        public static readonly DependencyProperty ImageColorProperty =
            DependencyProperty.Register("ImageColor", typeof(SolidColorBrush), typeof(VectorImageButton), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// 选中图形颜色
        /// </summary>
        public SolidColorBrush ImageSelectedColor
        {
            get { return (SolidColorBrush)GetValue(ImageSelectedColorProperty); }
            set { SetValue(ImageSelectedColorProperty, value); }
        }
        /// <summary>
        /// 选中图形颜色
        /// </summary>
        public static readonly DependencyProperty ImageSelectedColorProperty =
            DependencyProperty.Register("ImageSelectedColor", typeof(SolidColorBrush), typeof(VectorImageButton), new UIPropertyMetadata(Brushes.BlueViolet));

        /// <summary>
        /// 图形无效颜色
        /// </summary>
        public SolidColorBrush ImageDisabledColor
        {
            get { return (SolidColorBrush)GetValue(ImageDisabledColorProperty); }
            set { SetValue(ImageDisabledColorProperty, value); }
        }
        /// <summary>
        /// 图形无效颜色
        /// </summary>
        public static readonly DependencyProperty ImageDisabledColorProperty =
            DependencyProperty.Register("ImageDisabledColor", typeof(SolidColorBrush), typeof(VectorImageButton), new UIPropertyMetadata(Brushes.Gray));

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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VectorImageButton), new UIPropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// 是否保持选中状态
        /// </summary>
        public bool KeepSelected
        {
            get { return (bool)GetValue(KeepSelectedProperty); }
            set { SetValue(KeepSelectedProperty, value); }
        }
        /// <summary>
        /// 是否保持选中状态
        /// </summary>
        public static readonly DependencyProperty KeepSelectedProperty =
            DependencyProperty.Register("KeepSelected", typeof(bool), typeof(VectorImageButton), new UIPropertyMetadata(false));

        /// <summary>
        /// 按钮选择消息
        /// </summary>
        public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent("Clicked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VectorImageButton));
        /// <summary>
        /// 按钮选择消息
        /// </summary>
        public event RoutedEventHandler Clicked
        {
            add { AddHandler(ClickedEvent, value); }
            remove { RemoveHandler(ClickedEvent, value); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public VectorImageButton()
        {            
            InitializeComponent();
            IsEnabledChanged += VectorImageButton_IsEnabledChanged;
        }

        void VectorImageButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled == false)
            {
                txtBtn.Foreground = ImageDisabledColor;
                imgBtn.DrawColor = ImageDisabledColor;
            }
            else
            {
                //Foreground = ImageDisabledColor;
                txtBtn.Foreground = Foreground;
                imgBtn.DrawColor = ImageColor;
            }
        }
        
        private void btnControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (btnControl.Focus() == false)
            {
                btnControl.Focus();
            }
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ClickedEvent;
            args.Source = this;
            RaiseEvent(args); 
        }

        private void btnControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnControl.Focus();
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ClickedEvent;
            args.Source = this;
            RaiseEvent(args);
        }

        private void rootPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            imgBtn.DrawColor = ImageSelectedColor;
            txtBtn.Foreground = ImageSelectedColor;
            controlBorder.Background = SelectedColor;
            controlBorder.BorderBrush = SelectedBorder;
        }

        private void rootPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            imgBtn.DrawColor = ImageColor;
            txtBtn.Foreground = Foreground;
            controlBorder.Background = UnselectedColor;
            controlBorder.BorderBrush = UnselectedBorder;
        }
    }

    /// <summary>
    /// Orientation值与VerticalAlignment之间的转换
    /// </summary>
    [ValueConversion(typeof(VerticalAlignment), typeof(Orientation))]
    public class VerticalConvert : IValueConverter
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Horizontal :居中, Vertical:上对齐
            return (Orientation)value == Orientation.Horizontal ? VerticalAlignment.Center : VerticalAlignment.Top;
        }

        /// <summary>
        /// 回转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Orientation.Horizontal;
        }
    }


    /// <summary>
    /// Orientation值与VerticalAlignment之间的转换
    /// </summary>
    [ValueConversion(typeof(HorizontalAlignment), typeof(Orientation))]
    public class HorizontalConvert : IValueConverter
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Horizontal :左对齐, Vertical:居中
            return (Orientation)value == Orientation.Horizontal ? HorizontalAlignment.Left : HorizontalAlignment.Center;
        }

        /// <summary>
        /// 回转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Orientation.Horizontal;
        }
    }
}
