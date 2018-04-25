using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Ai.Hong.CommonResources.Styles
{
    /// <summary>
    /// Win32 class for Windows Style
    /// </summary>
    public class win32
    {
        /// <summary>
        /// Sent to a window in order to determine what part of the window corresponds to a particular screen coordinate  
        /// </summary>
        public const int WM_NCHITTEST = 0x0084;

        /// <summary>  
        /// Indicates the position of the cursor hot spot.  
        /// </summary>  
        public enum HitTest : int
        {
            /// <summary>  
            /// On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the DefWindowProc function produces a system beep to indicate an error).  
            /// </summary>  
            HTERROR = -2,

            /// <summary>  
            /// In a window currently covered by another window in the same thread (the message will be sent to underlying windows in the same thread until one of them returns a code that is not HTTRANSPARENT).  
            /// </summary>  
            HTTRANSPARENT = -1,

            /// <summary>  
            /// On the screen background or on a dividing line between windows.  
            /// </summary>  
            HTNOWHERE = 0,

            /// <summary>  
            /// In a client area.  
            /// </summary>  
            HTCLIENT = 1,

            /// <summary>  
            /// In a title bar.  
            /// </summary>  
            HTCAPTION = 2,

            /// <summary>  
            /// In a window menu or in a Close button in a child window.  
            /// </summary>  
            HTSYSMENU = 3,

            /// <summary>  
            /// In a size box (same as HTSIZE).  
            /// </summary>  
            HTGROWBOX = 4,

            /// <summary>  
            /// In a size box (same as HTGROWBOX).  
            /// </summary>  
            HTSIZE = 4,

            /// <summary>  
            /// In a menu.  
            /// </summary>  
            HTMENU = 5,

            /// <summary>  
            /// In a horizontal scroll bar.  
            /// </summary>  
            HTHSCROLL = 6,

            /// <summary>  
            /// In the vertical scroll bar.  
            /// </summary>  
            HTVSCROLL = 7,

            /// <summary>  
            /// In a Minimize button.  
            /// </summary>  
            HTMINBUTTON = 8,

            /// <summary>  
            /// In a Minimize button.  
            /// </summary>  
            HTREDUCE = 8,

            /// <summary>  
            /// In a Maximize button.  
            /// </summary>  
            HTMAXBUTTON = 9,

            /// <summary>  
            /// In a Maximize button.  
            /// </summary>  
            HTZOOM = 9,

            /// <summary>  
            /// In the left border of a resizable window (the user can click the mouse to resize the window horizontally).  
            /// </summary>  
            HTLEFT = 10,

            /// <summary>  
            /// In the right border of a resizable window (the user can click the mouse to resize the window horizontally).  
            /// </summary>  
            HTRIGHT = 11,

            /// <summary>  
            /// In the upper-horizontal border of a window.  
            /// </summary>  
            HTTOP = 12,

            /// <summary>  
            /// In the upper-left corner of a window border.  
            /// </summary>  
            HTTOPLEFT = 13,

            /// <summary>  
            /// In the upper-right corner of a window border.  
            /// </summary>  
            HTTOPRIGHT = 14,

            /// <summary>  
            /// In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window vertically).  
            /// </summary>  
            HTBOTTOM = 15,

            /// <summary>  
            /// In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).  
            /// </summary>  
            HTBOTTOMLEFT = 16,

            /// <summary>  
            /// In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).  
            /// </summary>  
            HTBOTTOMRIGHT = 17,

            /// <summary>  
            /// In the border of a window that does not have a sizing border.  
            /// </summary>  
            HTBORDER = 18,

            /// <summary>  
            /// In a Close button.  
            /// </summary>  
            HTCLOSE = 20,

            /// <summary>  
            /// In a Help button.  
            /// </summary>  
            HTHELP = 21,
        };  

        /// <summary>
        /// Sent to a window when the size or position of the window is about to change  
        /// </summary>
        public const int WM_GETMINMAXINFO = 0x0024;  
  
        /// <summary>
        ///  Retrieves a handle to the display monitor that is nearest to the window  
        /// </summary>
        public const int MONITOR_DEFAULTTONEAREST = 2;  
  
        /// <summary>
        /// Retrieves a handle to the display monitor  
        /// </summary>
        /// <param name="hwnd">Window's Handle</param>
        /// <param name="dwFlags">Drag flag</param>
        /// <returns></returns>
        [DllImport("user32.dll")]  
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);  
  
        /// <summary>
        ///  RECT structure, Rectangle used by MONITORINFOEX  
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]  
        public struct RECT  
        {  
            /// <summary>
            /// Left
            /// </summary>
            public int Left;  
            /// <summary>
            /// Top
            /// </summary>
            public int Top;  
            /// <summary>
            /// Right
            /// </summary>
            public int Right;  
            /// <summary>
            /// Bottom
            /// </summary>
            public int Bottom;  
        }  
  
        /// <summary>
        ///  MONITORINFOEX structure, Monitor information used by GetMonitorInfo function  
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]  
        public class MONITORINFOEX  
        {  
            /// <summary>
            /// Structure's size
            /// </summary>
            public int cbSize;  
            /// <summary>
            /// The display monitor rectangle  
            /// </summary>
            public RECT rcMonitor;
            /// <summary>
            ///  The working area rectangle  
            /// </summary>
            public RECT rcWork;
            /// <summary>
            /// flags
            /// </summary>
            public int dwFlags;  
            /// <summary>
            /// device
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]  
            public char[] szDevice;  
        }  
  
        /// <summary>
        /// Point structure, Point information used by MINMAXINFO structure
        /// </summary>
  
        [StructLayout(LayoutKind.Sequential)]  
        public struct POINT  
        {  
            /// <summary>
            /// x
            /// </summary>
            public int x;  
            /// <summary>
            /// y
            /// </summary>
            public int y;  
  
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public POINT(int x, int y)  
            {  
                this.x = x;  
                this.y = y;  
            }  
        }  
  
        /// <summary>
        ///  MINMAXINFO structure, Window's maximum size and position information 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]  
        public struct MINMAXINFO  
        {  
            /// <summary>
            /// 
            /// </summary>
            public POINT ptReserved;  
            /// <summary>
            /// The maximized size of the window  
            /// </summary>
            public POINT ptMaxSize;
            /// <summary>
            /// The position of the maximized window  
            /// </summary> 
            public POINT ptMaxPosition;
            /// <summary>
            /// 
            /// </summary>
            public POINT ptMinTrackSize;  
            /// <summary>
            /// 
            /// </summary>
            public POINT ptMaxTrackSize;  
        }  
  
        /// <summary>
        /// Get the working area of the specified monitor  
        /// </summary>
        /// <param name="hmonitor"></param>
        /// <param name="monitorInfo"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]  
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX monitorInfo);

        /// <summary>
        ///  Posted when the user presses the left mouse button while the cursor is within the nonclient area of a window  
        /// </summary>
        public const int WM_NCLBUTTONDOWN = 0x00A1;

        /// <summary>
        ///  Sends the specified message to a window or windows 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="wMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
 
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);  
    }
}
