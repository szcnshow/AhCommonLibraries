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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;

namespace Ai.Hong.CommonResources.Styles
{
    /// <summary>
    /// WindowStyle.xaml 的交互逻辑
    /// </summary>
    public partial class WindowStyle 
    {
        /// <summary>
        /// System buttons on the window's title
        /// </summary>
        public enum enumWindowButton
        {
            /// <summary>
            /// Close Button
            /// </summary>
            Close,
            /// <summary>
            /// Minimize Button
            /// </summary>
            Minimize,
            /// <summary>
            /// Maximize Button
            /// </summary>
            Maximize,
            /// <summary>
            /// Normal Button
            /// </summary>
            Normal,
            /// <summary>
            /// SystemSetup Button
            /// </summary>
            SystemSetup,
            /// <summary>
            /// SkinSetup Button
            /// </summary>
            SkinSetup
        }

        private static Dictionary<enumWindowButton, string> buttonNames = new Dictionary<enumWindowButton, string>() 
        { 
            { enumWindowButton.Close, "Close" }, 
            { enumWindowButton.Minimize, "Minimized" }, 
            { enumWindowButton.Maximize, "Maximized" }, 
            { enumWindowButton.Normal, "Normal" }, 
            { enumWindowButton.SystemSetup, "SystemSetup" }, 
            { enumWindowButton.SkinSetup, "SkinSetup" } 
        };

        /// <summary>
        /// Constructor
        /// </summary>
        public WindowStyle()
        {
            InitializeComponent();
        }

        
        //Point Win_Mouse = new Point();
        //Point Sys_Mouse = new Point();
       // Point Move_Mouse = new Point();
        double[] Win_Size = { 0, 0 };
        double[] Win_ActualSize = { 0, 0 };

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);

            Button maxBtn = WindowStyle.GetCommandButton(win, enumWindowButton.Maximize);
            Button normalBtn = WindowStyle.GetCommandButton(win, enumWindowButton.Normal);

            if (maxBtn == null || normalBtn == null)
                return;

            if (e.ClickCount == 2)
            {
                if (win.ResizeMode == ResizeMode.NoResize)
                {
                    if (maxBtn.Visibility == Visibility.Visible || normalBtn.Visibility == Visibility.Visible)
                    {
                        if (win.WindowState == WindowState.Maximized)
                            win.WindowState = WindowState.Normal;
                        else
                            win.WindowState = WindowState.Maximized;
                    }
                }
                else if (win.ResizeMode == ResizeMode.CanMinimize)
                {
                }
                else
                {
                    if (win.WindowState == WindowState.Maximized)
                        win.WindowState = WindowState.Normal;
                    else
                        win.WindowState = WindowState.Maximized;
                }
            }
            else if (win.WindowState != WindowState.Maximized)
            {
                try
                {
                    win.DragMove();
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            win.Close();
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimized_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            win.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 最大化窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximized_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            Win_Size[0] = win.Width;
            Win_Size[1] = win.Height;
            win.WindowState = WindowState.Maximized;
        }
        /// <summary>
        /// 还原窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Normal_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            win.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// 拖拽标题栏还原窗口大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                WindowInteropHelper wih = new WindowInteropHelper(Window.GetWindow((FrameworkElement)sender));
                win32.SendMessage(wih.Handle, win32.WM_NCLBUTTONDOWN, (int)win32.HitTest.HTCAPTION, 0);
            }
        }

        /// <summary>
        /// 左键点击窗口标题栏时，优先记录一下窗口当前状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);

            //Win_Mouse = Mouse.GetPosition(win);
            //Sys_Mouse.X = System.Windows.Forms.Control.MousePosition.X;
            //Sys_Mouse.Y = System.Windows.Forms.Control.MousePosition.Y;

            Win_Size[0] = win.Width;
            Win_Size[1] = win.Height;
            Win_ActualSize[0] = win.ActualWidth;
            Win_ActualSize[1] = win.ActualHeight;
        }

        /// <summary>
        /// Hide one system Button
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="windowButton"></param>
        public static void HideCommandButton(System.Windows.Window wnd, enumWindowButton windowButton)
        {
            Button btn = GetCommandButton(wnd, windowButton);
            if (btn != null)
                btn.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Enable or disable window's button
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="windowButton"></param>
        /// <param name="enabled"></param>
        public static void EnableCommandButton(System.Windows.Window wnd, enumWindowButton windowButton, bool enabled)
        {
            Button btn = GetCommandButton(wnd, windowButton);
            if (btn != null)
                btn.IsEnabled = enabled;
        }

        /// <summary>
        /// Get system button
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="windowButton"></param>
        /// <returns></returns>
        public static Button GetCommandButton(System.Windows.Window wnd, enumWindowButton windowButton)
        {
            if (wnd == null || wnd.Template == null)
                return null;

            return wnd.Template.FindName(buttonNames[windowButton], wnd) as Button;
        }

        /// <summary>
        /// 获取按钮的面板
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public static WrapPanel GetButtonWrapPanel(System.Windows.Window wnd)
        {
            if (wnd == null || wnd.Template == null)
                return null;

            return wnd.Template.FindName("titlebuttonWarpPanel", wnd) as WrapPanel;
        }

        #region Resize

        /// <summary>
        /// ThumbGrid is used for resize the window by mouse drag, hide it means this window cannot resize by customer
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="visible"></param>
        public static void VisibleResizeThumbGrid(System.Windows.Window wnd, bool visible)
        {
            if (wnd == null || wnd.Template == null)
                return;

            Grid thumbGrid = wnd.Template.FindName("PART_ResizerContainers", wnd) as Grid;
            if (thumbGrid != null)
                thumbGrid.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ResizeBottomLeft(object sender, DragDeltaEventArgs e)
        {
            ResizeLeft(sender, e);
            ResizeBottom(sender, e);
        }

        private void ResizeTopLeft(object sender, DragDeltaEventArgs e)
        {
            ResizeTop(sender, e);
            ResizeLeft(sender, e);
        }

        private void ResizeTopRight(object sender, DragDeltaEventArgs e)
        {
            ResizeRight(sender, e);
            ResizeTop(sender, e);
        }

        private void ResizeBottomRight(object sender, DragDeltaEventArgs e)
        {
            ResizeBottom(sender, e);
            ResizeRight(sender, e);
        }

        private void ResizeBottom(object sender, DragDeltaEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            if (win.WindowState != WindowState.Normal)
                return;

            if (win.ActualHeight <= win.MinHeight && e.VerticalChange < 0)
            {
                return;
            }

            if (double.IsNaN(win.Height))
            {
                win.Height = win.ActualHeight;
            }

            win.Height += e.VerticalChange;

            //win.originSize.Height = win.Height;
        }

        private void ResizeRight(object sender, DragDeltaEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            if (win.WindowState != WindowState.Normal)
                return;

            if (win.ActualWidth <= win.MinWidth && e.HorizontalChange < 0)
            {
                return;
            }

            if (double.IsNaN(win.Width))
            {
                win.Width = win.ActualWidth;
            }

            win.Width += e.HorizontalChange;
            //originSize.Width = Width;
        }

        private void ResizeLeft(object sender, DragDeltaEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            if (win.WindowState != WindowState.Normal)
                return;

            if (win.ActualWidth <= win.MinWidth && e.HorizontalChange > 0)
            {
                return;
            }

            if (double.IsNaN(win.Width))
            {
                win.Width = win.ActualWidth;
            }

            win.Width -= e.HorizontalChange;
            win.Left += e.HorizontalChange;
            //originSize.X = Left;
            //originSize.Width = Width;
        }

        private void ResizeTop(object sender, DragDeltaEventArgs e)
        {
            Window win = Window.GetWindow((FrameworkElement)sender);
            if (win.WindowState != WindowState.Normal)
                return;

            if (win.ActualHeight <= win.MinHeight && e.VerticalChange > 0)
            {
                return;
            }

            if (double.IsNaN(win.Height))
            {
                win.Height = win.ActualHeight;
            }

            win.Height -= e.VerticalChange;
            win.Top += e.VerticalChange;
            //originSize.Y = originSize.Top;
            //originSize.Height = Height;
        }

        #endregion
    }


    /// <summary>
    /// 颜色和画笔之间的转换
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class BrushToColorConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            if (value == null)
                return default(Color);
            else
                return brush.Color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is Color)
            {
                Color color = (Color)value;
                return new SolidColorBrush(color);
            }
            else
                return null;
        }
    }
}
