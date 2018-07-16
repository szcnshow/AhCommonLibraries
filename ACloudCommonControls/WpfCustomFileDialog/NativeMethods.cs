// Copyright © Decebal Mihailescu 2010
// Some code was obtained by reverse engineering the PresentationFramework.dll using Reflector

// All rights reserved.
// This code is released under The Code Project Open License (CPOL) 1.02
// The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
// REMAINS UNCHANGED.
using MS.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Reflection;

namespace Ai.Hong.Controls
{


    #region POINT
    /// <summary>
    /// POINT
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

        #region Constructors
        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="point"></param>
        public POINT(Point point)
        {
            x = (int)point.X;
            y = (int)point.Y;
        }
        #endregion
    }
    #endregion

    #region MinMax
    /// <summary>
    /// Windows min and max information
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        public POINT ptReserved;
        /// <summary>
        /// 最大尺寸
        /// </summary>
        public POINT ptMaxSize;
        /// <summary>
        /// 最大位置
        /// </summary>
        public POINT ptMaxPosition;
        /// <summary>
        /// 最小跟踪尺寸
        /// </summary>
        public POINT ptMinTrackSize;
        /// <summary>
        /// 最大跟踪尺寸
        /// </summary>
        public POINT ptMaxTrackSize;
    };
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    internal class OFNOTIFY
    {
        internal IntPtr hdr_hwndFrom;
        internal IntPtr hdr_idFrom;
        internal int hdr_code;
        internal IntPtr lpOFN;
        internal IntPtr pszFile;
    }
    #region DialogChangeProperties
    internal enum DialogChangeProperties
    {
        CDM_FIRST = (NativeMethods.Msg.WM_USER + 100),
        CDM_GETSPEC = (CDM_FIRST + 0x0000),
        CDM_GETFILEPATH = (CDM_FIRST + 0x0001),
        CDM_GETFOLDERPATH = (CDM_FIRST + 0x0002),
        CDM_GETFOLDERIDLIST = (CDM_FIRST + 0x0003),
        CDM_SETCONTROLTEXT = (CDM_FIRST + 0x0004),
        CDM_HIDECONTROL = (CDM_FIRST + 0x0005),
        CDM_SETDEFEXT = (CDM_FIRST + 0x0006)
    }
    #endregion
    #region ImeNotify

    internal enum ImeNotify
    {
        IMN_CLOSESTATUSWINDOW = 0x0001,
        IMN_OPENSTATUSWINDOW = 0x0002,
        IMN_CHANGECANDIDATE = 0x0003,
        IMN_CLOSECANDIDATE = 0x0004,
        IMN_OPENCANDIDATE = 0x0005,
        IMN_SETCONVERSIONMODE = 0x0006,
        IMN_SETSENTENCEMODE = 0x0007,
        IMN_SETOPENSTATUS = 0x0008,
        IMN_SETCANDIDATEPOS = 0x0009,
        IMN_SETCOMPOSITIONFONT = 0x000A,
        IMN_SETCOMPOSITIONWINDOW = 0x000B,
        IMN_SETSTATUSWINDOWPOS = 0x000C,
        IMN_GUIDELINE = 0x000D,
        IMN_PRIVATE = 0x000E
    }
    #endregion
    
    #region 
    internal enum GWL
    {
        GWL_WNDPROC = (-4),
        GWL_HINSTANCE = (-6),
        GWL_HWNDPARENT = (-8),
        GWL_STYLE = (-16),
        GWL_EXSTYLE = (-20),
        GWL_USERDATA = (-21),
        GWL_ID = (-12)
    }
    #endregion
    #region FolderViewMode
    /// <summary>
    /// 
    /// </summary>
    public enum FolderViewMode
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0x7028,
        /// <summary>
        /// 
        /// </summary>
        Icon = Default + 1,
        /// <summary>
        /// 
        /// </summary>
        SmallIcon = Default + 2,
        /// <summary>
        /// 
        /// </summary>
        List = Default + 3,
        /// <summary>
        /// 
        /// </summary>
        Details = Default + 4,
        /// <summary>
        /// 
        /// </summary>
        Thumbnails = Default + 5,
        /// <summary>
        /// 
        /// </summary>
        Title = Default + 6,
        /// <summary>
        /// 
        /// </summary>
        Thumbstrip = Default + 7,
    }
    #endregion
    #region RECT

    /// <summary>
    /// RECT
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// Left
        /// </summary>
        public int left;
        /// <summary>
        /// Top
        /// </summary>
        public int top;
        /// <summary>
        /// Right
        /// </summary>
        public int right;
        /// <summary>
        /// Bottom
        /// </summary>
        public int bottom;

        #region Properties
        /// <summary>
        /// Location
        /// </summary>
        public POINT Location
        {
            get { return new POINT((int)left, (int)top); }
            set
            {
                right -= (left - value.x);
                bottom -= (bottom - value.y);
                left = value.x;
                top = value.y;
            }
        }

        internal uint Width
        {
            get { return (uint)Math.Abs(right - left); }
            set { right = left + (int)value; }
        }

        internal uint Height
        {
            get { return (uint)Math.Abs(bottom - top); }
            set { bottom = top + (int)value; }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return left + ":" + top + ":" + right + ":" + bottom;
        }
        #endregion
        /// <summary>
        /// Is Empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (this.left < this.right)
                {
                    return (this.top >= this.bottom);
                }
                return true;
            }
        }
    }
    #endregion
    #region SWP_Flags
    /// <summary>
    /// SWP Flags
    /// </summary>
    [Flags]
    public enum SWP_Flags
    {
        /// <summary>
        /// 
        /// </summary>
        SWP_NOSIZE = 0x0001,
        /// <summary>
        /// 
        /// </summary>
        SWP_NOMOVE = 0x0002,
        /// <summary>
        /// 
        /// </summary>
        SWP_NOZORDER = 0x0004,
        /// <summary>
        /// 
        /// </summary>
        SWP_NOACTIVATE = 0x0010,
        /// <summary>
        /// 
        /// </summary>
        SWP_FRAMECHANGED = 0x0020, /* The frame changed: send WM_NCCALCSIZE */
        /// <summary>
        /// 
        /// </summary>
        SWP_SHOWWINDOW = 0x0040,
        /// <summary>
        /// 
        /// </summary>
        SWP_HIDEWINDOW = 0x0080,
        /// <summary>
        /// 
        /// </summary>
        SWP_NOOWNERZORDER = 0x0200, /* Don't do owner Z ordering */
        /// <summary>
        /// 
        /// </summary>
        SWP_DRAWFRAME = SWP_FRAMECHANGED,
        /// <summary>
        /// 
        /// </summary>
        SWP_NOREPOSITION = SWP_NOOWNERZORDER
    }
    #endregion

    /// <summary>
    /// Open filename I
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class OPENFILENAME_I
    {
        /// <summary>
        /// WinProc
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// lStructSize
        /// </summary>
        public int lStructSize = Marshal.SizeOf(typeof(OPENFILENAME_I));
        /// <summary>
        /// wndOwner
        /// </summary>
        public IntPtr hwndOwner;
        /// <summary>
        /// hInstance
        /// </summary>
        public IntPtr hInstance;
        /// <summary>
        /// lpstrFilter
        /// </summary>
        public string lpstrFilter;
        /// <summary>
        /// lpstrCustomFilter
        /// </summary>
        public IntPtr lpstrCustomFilter;
        /// <summary>
        /// nMaxCustFilter
        /// </summary>
        public int nMaxCustFilter;
        /// <summary>
        /// nFilterIndex
        /// </summary>
        public int nFilterIndex;
        /// <summary>
        /// lpstrFile
        /// </summary>
        public IntPtr lpstrFile;
        /// <summary>
        /// nMaxFile
        /// </summary>
        public int nMaxFile = 260;
        /// <summary>
        /// lpstrFileTitle
        /// </summary>
        public IntPtr lpstrFileTitle;
        /// <summary>
        /// nMaxFileTitle
        /// </summary>
        public int nMaxFileTitle = 260;
        /// <summary>
        /// lpstrInitialDir
        /// </summary>
        public string lpstrInitialDir;
        /// <summary>
        /// lpstrTitle
        /// </summary>
        public string lpstrTitle;
        /// <summary>
        /// Flags
        /// </summary>
        public int Flags;
        /// <summary>
        /// nFileOffset
        /// </summary>
        public short nFileOffset;
        /// <summary>
        /// nFileExtension
        /// </summary>
        public short nFileExtension;
        /// <summary>
        /// lpstrDefExt
        /// </summary>
        public string lpstrDefExt;
        /// <summary>
        /// lCustData
        /// </summary>
        public IntPtr lCustData;
        /// <summary>
        /// lpfnHook
        /// </summary>
        public WndProc lpfnHook;
        /// <summary>
        /// lpTemplateName
        /// </summary>
        public string lpTemplateName;
        /// <summary>
        /// pvReserved
        /// </summary>
        public IntPtr pvReserved;
        /// <summary>
        /// dwReserved
        /// </summary>
        public int dwReserved;
        /// <summary>
        /// FlagsEx
        /// </summary>
        public int FlagsEx;
    }
    #region WINDOWPOS

    /// <summary>
    /// Window's Position
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        /// <summary>
        /// Window handle
        /// </summary>
        public IntPtr hwnd;
        /// <summary>
        /// Next windows handle
        /// </summary>
        public IntPtr hwndAfter;
        /// <summary>
        /// position x
        /// </summary>
        public int x;
        /// <summary>
        /// position y
        /// </summary>
        public int y;
        /// <summary>
        /// windows size
        /// </summary>
        public int cx;
        /// <summary>
        /// window size
        /// </summary>
        public int cy;
        /// <summary>
        /// window flag
        /// </summary>
        public uint flags;

        #region Overrides
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return x + ":" + y + ":" + cx + ":" + cy + ":" + ((SWP_Flags)flags).ToString();
        }
        #endregion
    }
    #endregion

    #region WINDOWPLACEMENT
    /// <summary>
    /// Windows placement
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        /// <summary>
        /// length
        /// </summary>
        public int length;
        /// <summary>
        /// flags
        /// </summary>
        public int flags;
        /// <summary>
        /// show command
        /// </summary>
        public int showCmd;
        /// <summary>
        /// min position x
        /// </summary>
        public int ptMinPosition_x;
        /// <summary>
        /// min position y
        /// </summary>
        public int ptMinPosition_y;
        /// <summary>
        /// max position x 
        /// </summary>
        public int ptMaxPosition_x;
        /// <summary>
        /// max position y
        /// </summary>
        public int ptMaxPosition_y;
        /// <summary>
        /// normal rect left
        /// </summary>
        public int rcNormalPosition_left;
        /// <summary>
        /// normal position top
        /// </summary>
        public int rcNormalPosition_top;
        /// <summary>
        /// normal position right
        /// </summary>
        public int rcNormalPosition_right;
        /// <summary>
        /// normal position bottom
        /// </summary>
        public int rcNormalPosition_bottom;
    }

    #endregion
    #region ZOrderPos
    internal enum ZOrderPos
    {
        HWND_TOP = 0,
        HWND_BOTTOM = 1,
        HWND_TOPMOST = -1,
        HWND_NOTOPMOST = -2
    }
    #endregion
	#region SetWindowPosFlags
    
    [Flags]
    internal enum SetWindowPosFlags
	{
		SWP_NOSIZE          = 0x0001,
		SWP_NOMOVE          = 0x0002,
		SWP_NOZORDER        = 0x0004,
		SWP_NOREDRAW        = 0x0008,
		SWP_NOACTIVATE      = 0x0010,
		SWP_FRAMECHANGED    = 0x0020,
		SWP_SHOWWINDOW      = 0x0040,
		SWP_HIDEWINDOW      = 0x0080,
		SWP_NOCOPYBITS      = 0x0100,
		SWP_NOOWNERZORDER   = 0x0200, 
		SWP_NOSENDCHANGING  = 0x0400,
		SWP_DRAWFRAME       = 0x0020,
		SWP_NOREPOSITION    = 0x0200,
		SWP_DEFERERASE      = 0x2000,
		SWP_ASYNCWINDOWPOS  = 0x4000
	}
	#endregion

    #region WINDOWINFO

    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWINFO
    {
        public UInt32 cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public UInt32 dwStyle;
        public UInt32 dwExStyle;
        public UInt32 dwWindowStatus;
        public UInt32 cxWindowBorders;
        public UInt32 cyWindowBorders;
        public UInt16 atomWindowType;
        public UInt16 wCreatorVersion;
    }
    #endregion

    /// <summary>
    /// Native method
    /// </summary>
    public static class NativeMethods
    {
        #region Delegates
        /// <summary>
        /// Window enumerator callback
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumWindowsCallBack(IntPtr hWnd, int lParam);
        #endregion
        /// <summary>
        /// Ajust window display rectangle
        /// </summary>
        /// <param name="lpRect"></param>
        /// <param name="dwStyle"></param>
        /// <param name="bMenu"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRect(
            ref RECT lpRect,
            Int32 dwStyle,
            bool bMenu
        );
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetOpenFileName([In, Out] OPENFILENAME_I ofn);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetSaveFileName([In, Out] OPENFILENAME_I ofn);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool MoveWindow(HandleRef hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// Get window's class name
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lpClassName"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(HandleRef hwnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowInfo(IntPtr hwnd, out WINDOWINFO pwi);
        /// <summary>
        /// Enumerate child window
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, NativeMethods.EnumWindowsCallBack lpEnumFunc, int lParam);
        /// <summary>
        /// Get dialog control's ID
        /// </summary>
        /// <param name="hWndCtl"></param>
        /// <returns></returns>
        [DllImport("User32.Dll")]
        public static extern int GetDlgCtrlID(IntPtr hWndCtl);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
        /// <summary>
        /// Set window's parent
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndParent"></param>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IntSetWindowText(HandleRef hWnd, string text);
        [SecurityCritical, SecuritySafeCritical]
        internal static void SetWindowText(HandleRef hWnd, string text)
        {
            if (!IntSetWindowText(hWnd, text))
            {
                throw new Win32Exception();
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint RegisterWindowMessage(string lpString);
        /// <summary>
        /// Get foucs control
        /// </summary>
        /// <returns></returns>
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetFocus();
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetParent", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr IntGetParent(HandleRef hWnd);
        [SecurityCritical]
        internal static IntPtr GetParent(HandleRef hWnd)
        {
            SetLastError(0);
            IntPtr ptr = IntGetParent(hWnd);
            int error = Marshal.GetLastWin32Error();
            if ((ptr == IntPtr.Zero) && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return ptr;
        }
        [SecurityCritical]
        internal static int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount)
        {
            SetLastError(0);
            int num = IntGetWindowText(hWnd, lpString, nMaxCount);
            if (num == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return num;
        }

        [SecurityCritical]
        internal static IntPtr CriticalSetWindowLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(IntCriticalSetWindowLong(hWnd, nIndex, (int)dwNewLong.ToInt64()));
            }
            return IntCriticalSetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        private static extern int IntCriticalSetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntCriticalSetWindowLongPtr(HandleRef hWnd, int nIndex, OPENFILENAME_I.WndProc dwNewLong);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr IntCriticalSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        internal static extern IntPtr UnsafeSendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// Is a window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool IsWindow(HandleRef hWnd);
        [DllImport("user32.dll", EntryPoint = "GetWindowPlacement", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool IntGetWindowPlacement(HandleRef hWnd, ref WINDOWPLACEMENT placement);

        [SecurityCritical, SecuritySafeCritical]
        internal static void GetWindowPlacement(HandleRef hWnd, ref WINDOWPLACEMENT placement)
        {
            if (!IntGetWindowPlacement(hWnd, ref placement))
            {
                throw new Win32Exception();
            }
        }
        [SecuritySafeCritical]
        internal static void SetWindowPlacement(HandleRef hWnd, [In] ref WINDOWPLACEMENT placement)
        {
            if (!IntSetWindowPlacement(hWnd, ref placement))
            {
                throw new Win32Exception();
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowPlacement", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool IntSetWindowPlacement(HandleRef hWnd, [In] ref WINDOWPLACEMENT placement);

        /// <summary>
        /// Enable window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        [SecurityCritical]
        public static bool EnableWindow(HandleRef hWnd, bool enable)
        {
            SetLastError(0);
            bool flag = IntEnableWindow(hWnd, enable);
            if (!flag)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return flag;
        }

        /// <summary>
        /// Int enable window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "EnableWindow", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool IntEnableWindow(HandleRef hWnd, bool enable);

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntGetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);
        [SecurityCritical]
        internal static int GetWindowTextLength(HandleRef hWnd)
        {
            SetLastError(0);
            int num = IntGetWindowTextLength(hWnd);
            if (num == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return num;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "GetWindowTextLength", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntGetWindowTextLength(HandleRef hWnd);

        [DllImport("user32.dll")]
        internal static extern bool InvalidateRect(HandleRef hWnd, IntPtr lpRect, bool bErase);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "SetFocus", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr IntSetFocus(HandleRef hWnd);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int CommDlgExtendedError();
        internal static IntPtr SetFocus(HandleRef hWnd)
        {
            SetLastError(0);
            IntPtr ptr = IntSetFocus(hWnd);
            int error = Marshal.GetLastWin32Error();
            if ((ptr == IntPtr.Zero) && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return ptr;
        }
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern void SetLastError(int dwErrorCode);

        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags);

        /// <summary>
        /// Find window ex
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="childAfter"></param>
        /// <param name="className"></param>
        /// <param name="windowTitle"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(HandleRef parentHandle, HandleRef childAfter, string className, string windowTitle);
        /// <summary>
        /// Get window rectangle
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="rect"></param>
        [SecurityCritical]
        public static void GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect)
        {
            if (!IntGetWindowRect(hWnd, ref rect))
            {
                throw new Win32Exception();
            }
        }
        /// <summary>
        /// Int get window rectangle
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool IntGetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);
        /// <summary>
        /// Get window's client rectangle
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="rect"></param>
        [SecurityCritical]
        public static void GetClientRect(HandleRef hWnd, [In, Out] ref RECT rect)
        {
            if (!IntGetClientRect(hWnd, ref rect))
            {
                throw new Win32Exception();
            }
        }
        /// <summary>
        /// Int get client rectangle
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", EntryPoint = "GetClientRect", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool IntGetClientRect(HandleRef hWnd, [In, Out] ref RECT rect);
        [SecurityCritical]
        internal static int GetWindowLong(HandleRef hWnd, int nIndex)
        {
            int num = 0;
            IntPtr zero = IntPtr.Zero;
            int num2 = 0;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                num = IntGetWindowLong(hWnd, nIndex);
                num2 = Marshal.GetLastWin32Error();
                zero = new IntPtr(num);
            }
            else
            {
                zero = IntGetWindowLongPtr(hWnd, nIndex);
                num2 = Marshal.GetLastWin32Error();
                num = (int)zero.ToInt64();
            }
            if (zero == IntPtr.Zero)
            {
            }
            return num;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntGetWindowLong(HandleRef hWnd, int nIndex);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntGetWindowLongPtr(HandleRef hWnd, int nIndex);
        [SecurityCritical]
        internal static IntPtr GetWindowLongPtr(HandleRef hWnd, GWL nIndex)
        {
            IntPtr zero = IntPtr.Zero;
            int num = 0;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                int num2 = IntGetWindowLong(hWnd, (int)nIndex);
                num = Marshal.GetLastWin32Error();
                zero = new IntPtr(num2);
            }
            else
            {
                zero = IntGetWindowLongPtr(hWnd, (int)nIndex);
                num = Marshal.GetLastWin32Error();
            }
            if (zero == IntPtr.Zero)
            {
            }
            return zero;
        }

        /// <summary>
        /// Register window's class
        /// </summary>
        /// <param name="hKey"></param>
        /// <param name="lpSubKey"></param>
        /// <param name="phkResult"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", EntryPoint = "RegCreateKeyW")]
        public static extern int RegCreateKeyW([In] UIntPtr hKey, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey, out IntPtr phkResult);
        /// <summary>
        /// Register override predefined key
        /// </summary>
        /// <param name="hKey"></param>
        /// <param name="hNewHKey"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", EntryPoint = "RegOverridePredefKey")]
        public static extern int RegOverridePredefKey([In] UIntPtr hKey, [In] IntPtr hNewHKey);
        /// <summary>
        /// Register close key
        /// </summary>
        /// <param name="hKey"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", EntryPoint = "RegCloseKey")]
        public static extern int RegCloseKey([In] IntPtr hKey);


        #region Enums

        #region ZOrderPos

        internal enum ZOrderPos
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum SetWindowPosFlags
        {
            /// <summary>
            /// 
            /// </summary>
            SWP_NOSIZE = 0x0001,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOMOVE = 0x0002,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOZORDER = 0x0004,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOREDRAW = 0x0008,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOACTIVATE = 0x0010,
            /// <summary>
            /// 
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,
            /// <summary>
            /// 
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,
            /// <summary>
            /// 
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,
            /// <summary>
            /// 
            /// </summary>
            SWP_DRAWFRAME = 0x0020,
            /// <summary>
            /// 
            /// </summary>
            SWP_NOREPOSITION = 0x0200,
            /// <summary>
            /// 
            /// </summary>
            SWP_DEFERERASE = 0x2000,
            /// <summary>
            /// 
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000
        }
        #endregion
        #region DialogChangeStatus
        internal enum DialogChangeStatus : int
        {
            CDN_FIRST = -601,
            CDN_INITDONE = (CDN_FIRST - 0x0000),
            CDN_SELCHANGE = (CDN_FIRST - 0x0001),
            CDN_FOLDERCHANGE = (CDN_FIRST - 0x0002),
            CDN_SHAREVIOLATION = (CDN_FIRST - 0x0003),
            CDN_HELP = (CDN_FIRST - 0x0004),
            CDN_FILEOK = (CDN_FIRST - 0x0005),
            CDN_TYPECHANGE = (CDN_FIRST - 0x0006),
            CDN_INCLUDEITEM = (CDN_FIRST - 0x0007)
        }
        #endregion


        #region NCCALCSIZE_PARAMS

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc0, rgrc1, rgrc2;
            public IntPtr lppos;
        }
        #endregion

        #region FolderViewMode


        /// <summary>
        /// Folder view mode
        /// </summary>
        public enum FolderViewMode
        {
            /// <summary>
            /// 
            /// </summary>
            Default = 0x7028,
            /// <summary>
            /// 
            /// </summary>
            Icon = Default + 1,
            /// <summary>
            /// 
            /// </summary>
            SmallIcon = Default + 2,
            /// <summary>
            /// 
            /// </summary>
            List = Default + 3,
            /// <summary>
            /// 
            /// </summary>
            Details = Default + 4,
            /// <summary>
            /// 
            /// </summary>
            Thumbnails = Default + 5,
            /// <summary>
            /// 
            /// </summary>
            Tiles = Default + 6,
            /// <summary>
            /// 
            /// </summary>
            Thumbstrip = Default + 7,
        }
        #endregion
        #region Window Styles

        [Flags]
        internal enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_TILED = 0x00000000,
            WS_ICONIC = 0x20000000,
            WS_SIZEBOX = 0x00040000,
            WS_POPUPWINDOW = 0x80880000,
            WS_OVERLAPPEDWINDOW = 0x00CF0000,
            WS_TILEDWINDOW = 0x00CF0000,
            WS_CHILDWINDOW = 0x40000000
        }
        #endregion

        #region Window Extended Styles

        [Flags]
        internal enum WindowExtendedStyles
        {
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = 0x00000300,
            WS_EX_PALETTEWINDOW = 0x00000188,
            WS_EX_LAYERED = 0x00080000
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public enum AddonWindowLocation
        {
            /// <summary>
            /// 
            /// </summary>
            BottomRight = 0,
            /// <summary>
            /// 
            /// </summary>
            Right = 1,
            /// <summary>
            /// 
            /// </summary>
            Bottom = 2
        }
        #region ControlIds
        internal enum ControlsId : int
        {
            ButtonOk = 0x1,
            ButtonCancel = 0x2,
            ButtonHelp = 0x40E,//0x0000040e
            GroupFolder = 0x440,
            LabelFileType = 0x441,
            LabelFileName = 0x442,
            LabelLookIn = 0x443,
            DefaultView = 0x461,
            LeftToolBar = 0x4A0,
            ComboFileName = 0x47c,
            ComboFileType = 0x470,
            ComboFolder = 0x471,
            CheckBoxReadOnly = 0x410
        }
        #endregion
        /// <summary>
        /// Window's command message
        /// </summary>
        public enum Msg
        {
            /// <summary>
            /// 
            /// </summary>
            WM_NULL = 0x0000,
            /// <summary>
            /// 
            /// </summary>
            WM_CREATE = 0x0001,
            /// <summary>
            /// 
            /// </summary>
            WM_DESTROY = 0x0002,
            /// <summary>
            /// 
            /// </summary>
            WM_MOVE = 0x0003,
            /// <summary>
            /// 
            /// </summary>
            WM_SIZE = 0x0005,
            /// <summary>
            /// 
            /// </summary>
            WM_ACTIVATE = 0x0006,
            /// <summary>
            /// 
            /// </summary>
            WM_SETFOCUS = 0x0007,
            /// <summary>
            /// 
            /// </summary>
            WM_KILLFOCUS = 0x0008,
            /// <summary>
            /// 
            /// </summary>
            WM_ENABLE = 0x000A,
            /// <summary>
            /// 
            /// </summary>
            WM_SETREDRAW = 0x000B,
            /// <summary>
            /// 
            /// </summary>
            WM_SETTEXT = 0x000C,
            /// <summary>
            /// 
            /// </summary>
            WM_GETTEXT = 0x000D,
            /// <summary>
            /// 
            /// </summary>
            WM_GETTEXTLENGTH = 0x000E,
            /// <summary>
            /// 
            /// </summary>
            WM_PAINT = 0x000F,
            /// <summary>
            /// 
            /// </summary>
            WM_CLOSE = 0x0010,
            /// <summary>
            /// 
            /// </summary>
            WM_QUERYENDSESSION = 0x0011,
            /// <summary>
            /// 
            /// </summary>
            WM_QUIT = 0x0012,
            /// <summary>
            /// 
            /// </summary>
            WM_QUERYOPEN = 0x0013,
            /// <summary>
            /// 
            /// </summary>
            WM_ERASEBKGND = 0x0014,
            /// <summary>
            /// 
            /// </summary>
            WM_SYSCOLORCHANGE = 0x0015,
            /// <summary>
            /// 
            /// </summary>
            WM_ENDSESSION = 0x0016,
            /// <summary>
            /// 
            /// </summary>
            WM_SHOWWINDOW = 0x0018,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLOR = 0x0019,
            /// <summary>
            /// 
            /// </summary>
            WM_WININICHANGE = 0x001A,
            /// <summary>
            /// 
            /// </summary>
            WM_SETTINGCHANGE = 0x001A,
            /// <summary>
            /// 
            /// </summary>
            WM_DEVMODECHANGE = 0x001B,
            /// <summary>
            /// 
            /// </summary>
            WM_ACTIVATEAPP = 0x001C,
            /// <summary>
            /// 
            /// </summary>
            WM_FONTCHANGE = 0x001D,
            /// <summary>
            /// 
            /// </summary>
            WM_TIMECHANGE = 0x001E,
            /// <summary>
            /// 
            /// </summary>
            WM_CANCELMODE = 0x001F,
            /// <summary>
            /// 
            /// </summary>
            WM_SETCURSOR = 0x0020,
            /// <summary>
            /// 
            /// </summary>
            WM_MOUSEACTIVATE = 0x0021,
            /// <summary>
            /// 
            /// </summary>
            WM_CHILDACTIVATE = 0x0022,
            /// <summary>
            /// 
            /// </summary>
            WM_QUEUESYNC = 0x0023,
            /// <summary>
            /// 
            /// </summary>
            WM_GETMINMAXINFO = 0x0024,
            /// <summary>
            /// 
            /// </summary>
            WM_PAINTICON = 0x0026,
            /// <summary>
            /// 
            /// </summary>
            WM_ICONERASEBKGND = 0x0027,
            /// <summary>
            /// 
            /// </summary>
            WM_NEXTDLGCTL = 0x0028,
            /// <summary>
            /// 
            /// </summary>
            WM_SPOOLERSTATUS = 0x002A,
            /// <summary>
            /// 
            /// </summary>
            WM_DRAWITEM = 0x002B,
            /// <summary>
            /// 
            /// </summary>
            WM_MEASUREITEM = 0x002C,
            /// <summary>
            /// 
            /// </summary>
            WM_DELETEITEM = 0x002D,
            /// <summary>
            /// 
            /// </summary>
            WM_VKEYTOITEM = 0x002E,
            /// <summary>
            /// 
            /// </summary>
            WM_CHARTOITEM = 0x002F,
            /// <summary>
            /// 
            /// </summary>
            WM_SETFONT = 0x0030,
            /// <summary>
            /// 
            /// </summary>
            WM_GETFONT = 0x0031,
            /// <summary>
            /// 
            /// </summary>
            WM_SETHOTKEY = 0x0032,
            /// <summary>
            /// 
            /// </summary>
            WM_GETHOTKEY = 0x0033,
            /// <summary>
            /// 
            /// </summary>
            WM_QUERYDRAGICON = 0x0037,
            /// <summary>
            /// 
            /// </summary>
            WM_COMPAREITEM = 0x0039,
            /// <summary>
            /// 
            /// </summary>
            WM_GETOBJECT = 0x003D,
            /// <summary>
            /// 
            /// </summary>
            WM_COMPACTING = 0x0041,
            /// <summary>
            /// 
            /// </summary>
            WM_COMMNOTIFY = 0x0044,
            /// <summary>
            /// 
            /// </summary>
            WM_WINDOWPOSCHANGING = 0x0046,
            /// <summary>
            /// 
            /// </summary>
            WM_WINDOWPOSCHANGED = 0x0047,
            /// <summary>
            /// 
            /// </summary>
            WM_POWER = 0x0048,
            /// <summary>
            /// 
            /// </summary>
            WM_COPYDATA = 0x004A,
            /// <summary>
            /// 
            /// </summary>
            WM_CANCELJOURNAL = 0x004B,
            /// <summary>
            /// 
            /// </summary>
            WM_NOTIFY = 0x004E,
            /// <summary>
            /// 
            /// </summary>
            WM_INPUTLANGCHANGEREQUEST = 0x0050,
            /// <summary>
            /// 
            /// </summary>
            WM_INPUTLANGCHANGE = 0x0051,
            /// <summary>
            /// 
            /// </summary>
            WM_TCARD = 0x0052,
            /// <summary>
            /// 
            /// </summary>
            WM_HELP = 0x0053,
            /// <summary>
            /// 
            /// </summary>
            WM_USERCHANGED = 0x0054,
            /// <summary>
            /// 
            /// </summary>
            WM_NOTIFYFORMAT = 0x0055,
            /// <summary>
            /// 
            /// </summary>
            WM_CONTEXTMENU = 0x007B,
            /// <summary>
            /// 
            /// </summary>
            WM_STYLECHANGING = 0x007C,
            /// <summary>
            /// 
            /// </summary>
            WM_STYLECHANGED = 0x007D,
            /// <summary>
            /// 
            /// </summary>
            WM_DISPLAYCHANGE = 0x007E,
            /// <summary>
            /// 
            /// </summary>
            WM_GETICON = 0x007F,
            /// <summary>
            /// 
            /// </summary>
            WM_SETICON = 0x0080,
            /// <summary>
            /// 
            /// </summary>
            WM_NCCREATE = 0x0081,
            /// <summary>
            /// 
            /// </summary>
            WM_NCDESTROY = 0x0082,
            /// <summary>
            /// 
            /// </summary>
            WM_NCCALCSIZE = 0x0083,
            /// <summary>
            /// 
            /// </summary>
            WM_NCHITTEST = 0x0084,
            /// <summary>
            /// 
            /// </summary>
            WM_NCPAINT = 0x0085,
            /// <summary>
            /// 
            /// </summary>
            WM_NCACTIVATE = 0x0086,
            /// <summary>
            /// 
            /// </summary>
            WM_GETDLGCODE = 0x0087,
            /// <summary>
            /// 
            /// </summary>
            WM_SYNCPAINT = 0x0088,
            /// <summary>
            /// 
            /// </summary>
            WM_NCMOUSEMOVE = 0x00A0,
            /// <summary>
            /// 
            /// </summary>
            WM_NCLBUTTONDOWN = 0x00A1,
            /// <summary>
            /// 
            /// </summary>
            WM_NCLBUTTONUP = 0x00A2,
            /// <summary>
            /// 
            /// </summary>
            WM_NCLBUTTONDBLCLK = 0x00A3,
            /// <summary>
            /// 
            /// </summary>
            WM_NCRBUTTONDOWN = 0x00A4,
            /// <summary>
            /// 
            /// </summary>
            WM_NCRBUTTONUP = 0x00A5,
            /// <summary>
            /// 
            /// </summary>
            WM_NCRBUTTONDBLCLK = 0x00A6,
            /// <summary>
            /// 
            /// </summary>
            WM_NCMBUTTONDOWN = 0x00A7,
            /// <summary>
            /// 
            /// </summary>
            WM_NCMBUTTONUP = 0x00A8,
            /// <summary>
            /// 
            /// </summary>
            WM_NCMBUTTONDBLCLK = 0x00A9,
            /// <summary>
            /// 
            /// </summary>
            WM_NCXBUTTONDOWN = 0x00AB,
            /// <summary>
            /// 
            /// </summary>
            WM_NCXBUTTONUP = 0x00AC,
            /// <summary>
            /// 
            /// </summary>
            WM_NCXBUTTONDBLCLK = 0x00AD,
            /// <summary>
            /// 
            /// </summary>
            WM_KEYDOWN = 0x0100,
            /// <summary>
            /// 
            /// </summary>
            WM_KEYUP = 0x0101,
            /// <summary>
            /// 
            /// </summary>
            WM_CHAR = 0x0102,
            /// <summary>
            /// 
            /// </summary>
            WM_DEADCHAR = 0x0103,
            /// <summary>
            /// 
            /// </summary>
            WM_SYSKEYDOWN = 0x0104,
            /// <summary>
            /// 
            /// </summary>
            WM_SYSKEYUP = 0x0105,
            /// <summary>
            /// 
            /// </summary>
            WM_SYSCHAR = 0x0106,
            /// <summary>
            /// 
            /// </summary>
            WM_SYSDEADCHAR = 0x0107,
            /// <summary>
            /// 
            /// </summary>
            WM_KEYLAST = 0x0108,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_STARTCOMPOSITION = 0x010D,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_ENDCOMPOSITION = 0x010E,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_COMPOSITION = 0x010F,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_KEYLAST = 0x010F,
            /// <summary>
            /// 
            /// </summary>
            WM_INITDIALOG = 0x0110,
            /// <summary>
            /// 
            /// </summary>
            WM_COMMAND = 0x0111,
            /// <summary>
            /// 
            /// </summary>
            WM_SYSCOMMAND = 0x0112,
            /// <summary>
            /// 
            /// </summary>
            WM_TIMER = 0x0113,
            /// <summary>
            /// 
            /// </summary>
            WM_HSCROLL = 0x0114,
            /// <summary>
            /// 
            /// </summary>
            WM_VSCROLL = 0x0115,
            /// <summary>
            /// 
            /// </summary>
            WM_INITMENU = 0x0116,
            /// <summary>
            /// 
            /// </summary>
            WM_INITMENUPOPUP = 0x0117,
            /// <summary>
            /// 
            /// </summary>
            WM_MENUSELECT = 0x011F,
            /// <summary>
            /// 
            /// </summary>
            WM_MENUCHAR = 0x0120,
            /// <summary>
            /// 
            /// </summary>
            WM_ENTERIDLE = 0x0121,
            /// <summary>
            /// 
            /// </summary>
            WM_MENURBUTTONUP = 0x0122,
            /// <summary>
            /// 
            /// </summary>
            WM_MENUDRAG = 0x0123,
            /// <summary>
            /// 
            /// </summary>
            WM_MENUGETOBJECT = 0x0124,
            /// <summary>
            /// 
            /// </summary>
            WM_UNINITMENUPOPUP = 0x0125,
            /// <summary>
            /// 
            /// </summary>
            WM_MENUCOMMAND = 0x0126,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLORMSGBOX = 0x0132,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLOREDIT = 0x0133,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLORLISTBOX = 0x0134,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLORBTN = 0x0135,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLORDLG = 0x0136,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLORSCROLLBAR = 0x0137,
            /// <summary>
            /// 
            /// </summary>
            WM_CTLCOLORSTATIC = 0x0138,
            /// <summary>
            /// 
            /// </summary>
            WM_MOUSEMOVE = 0x0200,
            /// <summary>
            /// 
            /// </summary>
            WM_LBUTTONDOWN = 0x0201,
            /// <summary>
            /// 
            /// </summary>
            WM_LBUTTONUP = 0x0202,
            /// <summary>
            /// 
            /// </summary>
            WM_LBUTTONDBLCLK = 0x0203,
            /// <summary>
            /// 
            /// </summary>
            WM_RBUTTONDOWN = 0x0204,
            /// <summary>
            /// 
            /// </summary>
            WM_RBUTTONUP = 0x0205,
            /// <summary>
            /// 
            /// </summary>
            WM_RBUTTONDBLCLK = 0x0206,
            /// <summary>
            /// 
            /// </summary>
            WM_MBUTTONDOWN = 0x0207,
            /// <summary>
            /// 
            /// </summary>
            WM_MBUTTONUP = 0x0208,
            /// <summary>
            /// 
            /// </summary>
            WM_MBUTTONDBLCLK = 0x0209,
            /// <summary>
            /// 
            /// </summary>
            WM_MOUSEWHEEL = 0x020A,
            /// <summary>
            /// 
            /// </summary>
            WM_XBUTTONDOWN = 0x020B,
            /// <summary>
            /// 
            /// </summary>
            WM_XBUTTONUP = 0x020C,
            /// <summary>
            /// 
            /// </summary>
            WM_XBUTTONDBLCLK = 0x020D,
            /// <summary>
            /// 
            /// </summary>
            WM_PARENTNOTIFY = 0x0210,
            /// <summary>
            /// 
            /// </summary>
            WM_ENTERMENULOOP = 0x0211,
            /// <summary>
            /// 
            /// </summary>
            WM_EXITMENULOOP = 0x0212,
            /// <summary>
            /// 
            /// </summary>
            WM_NEXTMENU = 0x0213,
            /// <summary>
            /// 
            /// </summary>
            WM_SIZING = 0x0214,
            /// <summary>
            /// 
            /// </summary>
            WM_CAPTURECHANGED = 0x0215,
            /// <summary>
            /// 
            /// </summary>
            WM_MOVING = 0x0216,
            /// <summary>
            /// 
            /// </summary>
            WM_DEVICECHANGE = 0x0219,
            /// <summary>
            /// 
            /// </summary>
            WM_MDICREATE = 0x0220,
            /// <summary>
            /// 
            /// </summary>
            WM_MDIDESTROY = 0x0221,
            /// <summary>
            /// 
            /// </summary>
            WM_MDIACTIVATE = 0x0222,
            /// <summary>
            /// 
            /// </summary>
            WM_MDIRESTORE = 0x0223,
            /// <summary>
            /// 
            /// </summary>
            WM_MDINEXT = 0x0224,
            /// <summary>
            /// 
            /// </summary>
            WM_MDIMAXIMIZE = 0x0225,
            /// <summary>
            /// 
            /// </summary>
            WM_MDITILE = 0x0226,
            /// <summary>
            /// 
            /// </summary>
            WM_MDICASCADE = 0x0227,
            /// <summary>
            /// 
            /// </summary>
            WM_MDIICONARRANGE = 0x0228,
            /// <summary>
            /// 
            /// </summary>
            WM_MDIGETACTIVE = 0x0229,
            /// <summary>
            /// 
            /// </summary>
            WM_MDISETMENU = 0x0230,
            /// <summary>
            /// 
            /// </summary>
            WM_ENTERSIZEMOVE = 0x0231,
            /// <summary>
            /// 
            /// </summary>
            WM_EXITSIZEMOVE = 0x0232,
            /// <summary>
            /// 
            /// </summary>
            WM_DROPFILES = 0x0233,
            /// <summary>
            /// 
            /// </summary>
            WM_MDIREFRESHMENU = 0x0234,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_SETCONTEXT = 0x0281,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_NOTIFY = 0x0282,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_CONTROL = 0x0283,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_COMPOSITIONFULL = 0x0284,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_SELECT = 0x0285,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_CHAR = 0x0286,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_REQUEST = 0x0288,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_KEYDOWN = 0x0290,
            /// <summary>
            /// 
            /// </summary>
            WM_IME_KEYUP = 0x0291,
            /// <summary>
            /// 
            /// </summary>
            WM_MOUSEHOVER = 0x02A1,
            /// <summary>
            /// 
            /// </summary>
            WM_MOUSELEAVE = 0x02A3,
            /// <summary>
            /// 
            /// </summary>
            WM_CUT = 0x0300,
            /// <summary>
            /// 
            /// </summary>
            WM_COPY = 0x0301,
            /// <summary>
            /// 
            /// </summary>
            WM_PASTE = 0x0302,
            /// <summary>
            /// 
            /// </summary>
            WM_CLEAR = 0x0303,
            /// <summary>
            /// 
            /// </summary>
            WM_UNDO = 0x0304,
            /// <summary>
            /// 
            /// </summary>
            WM_RENDERFORMAT = 0x0305,
            /// <summary>
            /// 
            /// </summary>
            WM_RENDERALLFORMATS = 0x0306,
            /// <summary>
            /// 
            /// </summary>
            WM_DESTROYCLIPBOARD = 0x0307,
            /// <summary>
            /// 
            /// </summary>
            WM_DRAWCLIPBOARD = 0x0308,
            /// <summary>
            /// 
            /// </summary>
            WM_PAINTCLIPBOARD = 0x0309,
            /// <summary>
            /// 
            /// </summary>
            WM_VSCROLLCLIPBOARD = 0x030A,
            /// <summary>
            /// 
            /// </summary>
            WM_SIZECLIPBOARD = 0x030B,
            /// <summary>
            /// 
            /// </summary>
            WM_ASKCBFORMATNAME = 0x030C,
            /// <summary>
            /// 
            /// </summary>
            WM_CHANGECBCHAIN = 0x030D,
            /// <summary>
            /// 
            /// </summary>
            WM_HSCROLLCLIPBOARD = 0x030E,
            /// <summary>
            /// 
            /// </summary>
            WM_QUERYNEWPALETTE = 0x030F,
            /// <summary>
            /// 
            /// </summary>
            WM_PALETTEISCHANGING = 0x0310,
            /// <summary>
            /// 
            /// </summary>
            WM_PALETTECHANGED = 0x0311,
            /// <summary>
            /// 
            /// </summary>
            WM_HOTKEY = 0x0312,
            /// <summary>
            /// 
            /// </summary>
            WM_PRINT = 0x0317,
            /// <summary>
            /// 
            /// </summary>
            WM_PRINTCLIENT = 0x0318,
            /// <summary>
            /// 
            /// </summary>
            WM_THEME_CHANGED = 0x031A,
            /// <summary>
            /// 
            /// </summary>
            WM_HANDHELDFIRST = 0x0358,
            /// <summary>
            /// 
            /// </summary>
            WM_HANDHELDLAST = 0x035F,
            /// <summary>
            /// 
            /// </summary>
            WM_AFXFIRST = 0x0360,
            /// <summary>
            /// 
            /// </summary>
            WM_AFXLAST = 0x037F,
            /// <summary>
            /// 
            /// </summary>
            WM_PENWINFIRST = 0x0380,
            /// <summary>
            /// 
            /// </summary>
            WM_PENWINLAST = 0x038F,
            /// <summary>
            /// 
            /// </summary>
            WM_APP = 0x8000,
            /// <summary>
            /// 
            /// </summary>
            WM_USER = 0x0400,
            /// <summary>
            /// 
            /// </summary>
            WM_REFLECT = WM_USER + 0x1c00
        }


        //[DllImport("user32.dll")]
        //public static extern IntPtr GetDC(HandleRef hWnd);
        //[DllImport("user32.dll")]
        //public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        //[DllImport("gdi32.dll")]
        //public static extern int GetBkColor(IntPtr hdc);
        //[DllImport("gdi32.dll")]
        //public static extern uint SetBkColor(IntPtr hdc, int crColor);
        //[StructLayout(LayoutKind.Explicit, Size = 4)]
        //public struct COLORREF
        //{
        //    public COLORREF(byte r, byte g, byte b)
        //    {
        //        this.Value = 0;
        //        this.R = r;
        //        this.G = g;
        //        this.B = b;
        //    }

        //    public COLORREF(int value)
        //    {
        //        this.R = 0;
        //        this.G = 0;
        //        this.B = 0;
        //        unchecked{
        //            this.Value = value & (int)0x00FFFFFF;
        //        };
        //    }

        //    [FieldOffset(0)]
        //    public byte R;
        //    [FieldOffset(1)]
        //    public byte G;
        //    [FieldOffset(2)]
        //    public byte B;

        //    [FieldOffset(0)]
        //    public int Value;
        //}
    }
}


