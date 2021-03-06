﻿using System;
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

namespace Ai.Hong.Common
{
    /// <summary>
    /// Windows窗口样式
    /// </summary>
    public static class Win32WindowsStyle
    {
        private static readonly int cornerWidth = 8;
        private static readonly int customBorderThickness = 0;
        private static Point mousePoint = new Point();
        private static Window window = null;

        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="ws"></param>
        public static void InitWindow(System.Windows.Window ws)
        {
            window = ws;
            window.SourceInitialized += new System.EventHandler(win_SourceInitialized);
        }
        private static void win_SourceInitialized(object sender, EventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            if (source == null)
                // Should never be null  
                throw new Exception("Cannot get HwndSource instance.");

            source.AddHook(new HwndSourceHook(WndProc));
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32WindowsMsg.WM_GETMINMAXINFO: // WM_GETMINMAXINFO message  
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case Win32WindowsMsg.WM_NCHITTEST: // WM_NCHITTEST message  
                    return WmNCHitTest(lParam, ref handled);
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private static IntPtr WmNCHitTest(IntPtr lParam, ref bool handled)
        {
            // Update cursor point  
            // The low-order word specifies the x-coordinate of the cursor.  
            // #define GET_X_LPARAM(lp) ((int)(short)LOWORD(lp))  
            mousePoint.X = (int)(short)(lParam.ToInt32() & 0xFFFF);
            // The high-order word specifies the y-coordinate of the cursor.  
            // #define GET_Y_LPARAM(lp) ((int)(short)HIWORD(lp))  
            mousePoint.Y = (int)(short)(lParam.ToInt32() >> 16);

            // Do hit test  
            handled = true;
            if (Math.Abs(mousePoint.Y - window.Top) <= cornerWidth
                && Math.Abs(mousePoint.X - window.Left) <= cornerWidth)
            { // Top-Left  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTTOPLEFT);
            }
            else if (Math.Abs(window.ActualHeight + window.Top - mousePoint.Y) <= cornerWidth
                && Math.Abs(mousePoint.X - window.Left) <= cornerWidth)
            { // Bottom-Left  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTBOTTOMLEFT);
            }
            else if (Math.Abs(mousePoint.Y - window.Top) <= cornerWidth
                && Math.Abs(window.ActualWidth + window.Left - mousePoint.X) <= cornerWidth)
            { // Top-Right  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTTOPRIGHT);
            }
            else if (Math.Abs(window.ActualWidth + window.Left - mousePoint.X) <= cornerWidth
                && Math.Abs(window.ActualHeight + window.Top - mousePoint.Y) <= cornerWidth)
            { // Bottom-Right  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTBOTTOMRIGHT);
            }
            else if (Math.Abs(mousePoint.X - window.Left) <= customBorderThickness)
            { // Left  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTLEFT);
            }
            else if (Math.Abs(window.ActualWidth + window.Left - mousePoint.X) <= customBorderThickness)
            { // Right  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTRIGHT);
            }
            else if (Math.Abs(mousePoint.Y - window.Top) <= customBorderThickness)
            { // Top  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTTOP);
            }
            else if (Math.Abs(window.ActualHeight + window.Top - mousePoint.Y) <= customBorderThickness)
            { // Bottom  
                return new IntPtr((int)Win32WindowsMsg.HitTest.HTBOTTOM);
            }
            else
            {
                handled = false;
                return IntPtr.Zero;
            }
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            // MINMAXINFO structure  
            Win32WindowsMsg.MINMAXINFO mmi = (Win32WindowsMsg.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Win32WindowsMsg.MINMAXINFO));

            // Get handle for nearest monitor to window window  
            WindowInteropHelper wih = new WindowInteropHelper(window);
            IntPtr hMonitor = Win32WindowsMsg.MonitorFromWindow(wih.Handle, Win32WindowsMsg.MONITOR_DEFAULTTONEAREST);

            // Get monitor info  
            Win32WindowsMsg.MONITORINFOEX monitorInfo = new Win32WindowsMsg.MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            Win32WindowsMsg.GetMonitorInfo(new HandleRef(window, hMonitor), monitorInfo);

            // Get HwndSource  
            HwndSource source = HwndSource.FromHwnd(wih.Handle);
            if (source == null)
                // Should never be null  
                throw new Exception("Cannot get HwndSource instance.");
            if (source.CompositionTarget == null)
                // Should never be null  
                throw new Exception("Cannot get HwndTarget instance.");

            // Get transformation matrix  
            Matrix matrix = source.CompositionTarget.TransformFromDevice;

            // Convert working area  
            Win32WindowsMsg.RECT workingArea = monitorInfo.rcWork;
            Point dpiIndependentSize =
                matrix.Transform(new Point(
                        workingArea.Right - workingArea.Left,
                        workingArea.Bottom - workingArea.Top
                        ));

            // Convert minimum size  
            Point dpiIndenpendentTrackingSize = matrix.Transform(new Point(
                window.MinWidth,
                window.MinHeight
                ));

            // Set the maximized size of the window  
            //20151125
            //mmi.ptMaxSize.x = (int)dpiIndependentSize.X;
            //mmi.ptMaxSize.y = (int)dpiIndependentSize.Y;
            mmi.ptMaxSize.x = (int)workingArea.Right - workingArea.Left;
            mmi.ptMaxSize.y = (int)workingArea.Bottom - workingArea.Top;

            // Set the position of the maximized window  
            mmi.ptMaxPosition.x = 0;
            mmi.ptMaxPosition.y = 0;

            // Set the minimum tracking size  
            //20151125
            //mmi.ptMinTrackSize.x = (int)dpiIndenpendentTrackingSize.X;
            //mmi.ptMinTrackSize.y = (int)dpiIndenpendentTrackingSize.Y;
            mmi.ptMinTrackSize.x = (int)window.MinWidth;
            mmi.ptMinTrackSize.y = (int)window.MinHeight;

            Marshal.StructureToPtr(mmi, lParam, true);
        }
    }
}
