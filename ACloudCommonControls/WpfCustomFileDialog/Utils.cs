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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.ComponentModel;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// IFileDlgExt
    /// </summary>
    public interface IFileDlgExt : IWin32Window
    {
        /// <summary>
        /// EventFileNameChanged
        /// </summary>
        event PathChangedEventHandler EventFileNameChanged;
        /// <summary>
        /// 
        /// </summary>
        event PathChangedEventHandler EventFolderNameChanged;
        /// <summary>
        /// 
        /// </summary>
        event FilterChangedEventHandler EventFilterChanged;
        /// <summary>
        /// 
        /// </summary>
        AddonWindowLocation FileDlgStartLocation
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        string FileDlgOkCaption
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        bool FileDlgEnableOkBtn
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        bool FixedSize
        {
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        NativeMethods.FolderViewMode FileDlgDefaultViewMode
        {
            set;
            get;
        }
    }
    //consider http://geekswithblogs.net/lbugnion/archive/2007/03/02/107747.aspx instead
    /// <summary>
    /// 
    /// </summary>
    public interface IWindowExt //: IWin32Window
    {

        /// <summary>
        /// 
        /// </summary>
        HwndSource Source
        {
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        IFileDlgExt ParentDlg
        {
            set;
        }
    }

}
