// Copyright © Decebal Mihailescu 2010

// All rights reserved.
// This code is released under The Code Project Open License (CPOL) 1.02
// The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
// REMAINS UNCHANGED.
using System;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows;

namespace Ai.Hong.Controls
{
    //see http://msdn.microsoft.com/en-us/magazine/cc300434.aspx
    //public  class FileDialogPlaces
    public abstract partial class FileDialogExt<T> //: Microsoft.Win32.CommonDialog, IFileDlgExt where T : System.Windows.Controls.ContentControl, IWindowExt, new()
    {
        private readonly string TempKeyName = "TempPredefKey_" + Guid.NewGuid().ToString();
        private const string Key_PlacesBar = @"Software\Microsoft\Windows\CurrentVersion\Policies\ComDlg32\PlacesBar";
        private RegistryKey _fakeKey;
        private IntPtr _overriddenKey;
        private object[] m_places;

        /// <summary>
        /// Set dialog places
        /// </summary>
        /// <param name="places"></param>
        public void SetPlaces(object[] places)
        {
            if (m_places == null)
                m_places = new object[5];
            else
                m_places.Initialize();

            if (places != null)
            {
                for (int i = 0; i < m_places.GetLength(0); i++)
                {
                    m_places[i] = places[i];

                }
            }
        }

        /// <summary>
        /// Reset dialog places
        /// </summary>
        public void ResetPlaces()
        {
            if (_overriddenKey != IntPtr.Zero)
            {
                ResetRegistry(_overriddenKey);
                _overriddenKey = IntPtr.Zero;
            }
            if (_fakeKey != null)
            {
                _fakeKey.Close();
                _fakeKey = null;
            }
            //delete the key tree
            Registry.CurrentUser.DeleteSubKeyTree(TempKeyName);
            m_places = null;
        }

        private void SetupFakeRegistryTree()
        {
            _fakeKey = Registry.CurrentUser.CreateSubKey(TempKeyName);
            _overriddenKey = InitializeRegistry();

            // at this point, m_TempKeyName equals places key
            // write dynamic places here reading from Places
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(Key_PlacesBar);
            for (int i = 0; i < m_places.GetLength(0); i++)
            {
                if (m_places[i] != null)
                {
                    reg.SetValue("Place" + i.ToString(), m_places[i]);
                }
            }
        }

        //public  IntPtr GetRegistryHandle(RegistryKey registryKey)
        //{
        //    Type type = registryKey.GetType();
        //    FieldInfo fieldInfo = type.GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic);
        //    return (IntPtr)fieldInfo.GetValue(registryKey);
        //}
        readonly UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
        private IntPtr InitializeRegistry()
        {
            IntPtr hkMyCU;
            NativeMethods.RegCreateKeyW(HKEY_CURRENT_USER, TempKeyName, out hkMyCU);
            NativeMethods.RegOverridePredefKey(HKEY_CURRENT_USER, hkMyCU);
            return hkMyCU;
        }


        private void ResetRegistry(IntPtr hkMyCU)
        {
            NativeMethods.RegOverridePredefKey(HKEY_CURRENT_USER, IntPtr.Zero);
            NativeMethods.RegCloseKey(hkMyCU);
            return;
        }
    }

    //http://www.codeguru.com/cpp/misc/misc/system/article.php/c13407/
    // fot .net 4.0 and up use CustomPlaces instead :http://msdn.microsoft.com/en-us/library/microsoft.win32.filedialog.customplaces.aspx
    /// <summary>
    /// Places
    /// </summary>
    public enum Places
    {
        /// <summary>
        /// Desktop
        /// </summary>
        [Description("Desktop")]
        Desktop = 0,
        /// <summary>
        /// Internet explorer
        /// </summary>
        [Description("Internet Explorer ")]
        InternetExplorer = 1,
        /// <summary>
        /// Program files
        /// </summary>
        [Description("Program Files")]
        Programs = 2,
        /// <summary>
        /// Control Panel
        /// </summary>
        [Description("Control Panel")]
        ControlPanel = 3,
        /// <summary>
        /// Printers
        /// </summary>
        [Description("Printers")]
        Printers = 4,
        /// <summary>
        /// My Documents
        /// </summary>
        [Description("My Documents")]
        MyDocuments = 5,
        /// <summary>
        /// Favorites
        /// </summary>
        [Description("Favorites")]
        Favorites = 6,
        /// <summary>
        /// Startup folder
        /// </summary>
        [Description("Startup folder")]
        StartupFolder = 7,
        /// <summary>
        /// Recent Files
        /// </summary>
        [Description("Recent Files")]
        RecentFiles = 8,
        /// <summary>
        /// Send To
        /// </summary>
        [Description("Send To")]
        SendTo = 9,
        /// <summary>
        /// Recycle Bin
        /// </summary>
        [Description("Recycle Bin")]
        RecycleBin = 0xa,
        /// <summary>
        /// Start menu
        /// </summary>
        [Description("Start menu")]
        StartMenu = 0xb,
        /// <summary>
        /// Logical My Documents
        /// </summary>
        [Description("Logical My Documents")]
        Logical_MyDocuments = 0xc,
        /// <summary>
        /// My Music
        /// </summary>
        [Description("My Music")]
        MyMusic = 0xd,
        /// <summary>
        /// My Videos
        /// </summary>
        [Description("My Videos")]
        MyVideos = 0xe,
        /// <summary>
        /// user name\Desktop
        /// </summary>
        [Description("<user name>\\Desktop")]
        UserName_Desktop = 0x10,
        /// <summary>
        /// My Computer
        /// </summary>
        [Description("My Computer")]
        MyComputer = 0x11,
        /// <summary>
        /// My Network Places
        /// </summary>
        [Description("My Network Places")]
        MyNetworkPlaces = 18,
        /// <summary>
        /// user name\nethood
        /// </summary>
        [Description("<user name>\nethood")]
        User_Name_Nethood = 0x13,
        /// <summary>
        /// Fonts
        /// </summary>
        [Description("Fonts")]
        Fonts = 0x14,
        /// <summary>
        /// All Users\\Start Menu
        /// </summary>
        [Description("All Users\\Start Menu")]
        All_Users_StartMenu = 0x16,
        /// <summary>
        /// All Users\\Start Menu\\Programs 
        /// </summary>
        [Description("All Users\\Start Menu\\Programs ")]
        All_Users_StartMenu_Programs = 0x17,
        /// <summary>
        /// All Users\\Startup
        /// </summary>
        [Description("All Users\\Startup")]
        All_Users_Startup = 0x18,
        /// <summary>
        /// All Users\\Desktop
        /// </summary>
        [Description("All Users\\Desktop")]
        All_Users_Desktop = 0x19,
        /// <summary>
        /// user name\\Application Data 
        /// </summary>
        [Description("<user name>\\Application Data ")]
        User_name_ApplicationData = 0x1a,
        /// <summary>
        /// user name\PrintHood
        /// </summary>
        [Description("<user name>\\PrintHood ")]
        User_Name_PrintHood = 0x1b,
        /// <summary>
        /// user name\\Local Settings\\Applicaiton Data (nonroaming)
        /// </summary>
        [Description("<user name>\\Local Settings\\Applicaiton Data (nonroaming)")]
        Local_ApplicaitonData = 0x1c,
        /// <summary>
        /// Nonlocalized common startup 
        /// </summary>
        [Description("Nonlocalized common startup ")]
        NonlocalizedCommonStartup = 0x1e,
        /// <summary>
        /// CommonFavorites
        /// </summary>
        [Description("Common Favorites")]
        CommonFavorites = 0x1f,
        /// <summary>
        /// Internet Cache
        /// </summary>
        [Description("Internet Cache ")]
        InternetCache = 0x20,
        /// <summary>
        /// Cookies
        /// </summary>
        [Description("Cookies ")]
        Cookies = 0x21,
        /// <summary>
        /// History
        /// </summary>
        [Description("History")]
        History = 0x22,
        /// <summary>
        /// All Users\\Application Data
        /// </summary>
        [Description("All Users\\Application Data ")]
        All_Users_ApplicationData = 0x23,
        /// <summary>
        /// Windows Directory
        /// </summary>
        [Description("Windows Directory")]
        WindowsDirectory = 0x24,
        /// <summary>
        /// System Directory
        /// </summary>
        [Description("System Directory")]
        SystemDirectory = 0x25,
        /// <summary>
        /// Program Files
        /// </summary>
        [Description("Program Files ")]
        ProgramFiles = 0x26,
        /// <summary>
        /// My Pictures
        /// </summary>
        [Description("My Pictures ")]
        MyPictures = 0x27,
        /// <summary>
        /// USERPROFILE
        /// </summary>
        [Description("USERPROFILE")]
        USERPROFILE = 0x28,
        /// <summary>
        /// system directory on RISC
        /// </summary>
        [Description("system directory on RISC")]
        SYSTEN_RISC = 0x29,
        /// <summary>
        /// Program Files on RISC
        /// </summary>
        [Description("Program Files on RISC ")]
        Program_Files_RISC = 0x2a,
        /// <summary>
        /// Program Files\\Common
        /// </summary>
        [Description("Program Files\\Common")]
        Common = 0x2b,
        /// <summary>
        /// Program Files\\Common on RISC
        /// </summary>
        [Description("Program Files\\Common on RISC")]
        Common_RISC = 0x2c,
        /// <summary>
        /// All Users\\Templates
        /// </summary>
        [Description("All Users\\Templates ")]
        Templates = 0x2d,
        /// <summary>
        /// All Users\\Documents
        /// </summary>
        [Description("All Users\\Documents")]
        All_Users_Documents = 0x2e,
        /// <summary>
        /// All Users\\Start Menu\\Programs\\Administrative Tools
        /// </summary>
        [Description("All Users\\Start Menu\\Programs\\Administrative Tools")]
        AdministrativeTools = 0x2f,
        /// <summary>
        /// user name\\Start Menu\\Programs\\Administrative Tools
        /// </summary>
        [Description("<user name>\\Start Menu\\Programs\\Administrative Tools")]
        USER_AdministrativeTools = 0x30,
        /// <summary>
        /// Network and Dial-up Connections
        /// </summary>
        [Description("Network and Dial-up Connections")]
        Network_DialUp_Connections = 0x31,
        /// <summary>
        /// All Users\\My Music
        /// </summary>
        [Description("All Users\\My Music")]
        All_Users_MyMusic = 0x35,
        /// <summary>
        /// All Users\\My Pictures
        /// </summary>
        [Description("All Users\\My Pictures")]
        All_Users_MyPictures = 0x36,
        /// <summary>
        /// All Users\\My Video
        /// </summary>
        [Description("All Users\\My Video")]
        All_Users_MyVideo = 0x37,
        /// <summary>
        /// Resource Directory
        /// </summary>
        [Description("Resource Directory")]
        Resource = 0x38,
        /// <summary>
        /// Localized Resource Directory
        /// </summary>
        [Description("Localized Resource Directory ")]
        Localized_Resource = 0x39,
        /// <summary>
        /// OEM specific apps
        /// </summary>
        [Description("OEM specific apps")]
        OEM_Specific = 0x3a,
        /// <summary>
        /// USERPROFILE\\Local Settings\\Application Data\\Microsoft\\CD Burning
        /// </summary>
        [Description("USERPROFILE\\Local Settings\\Application Data\\Microsoft\\CD Burning")]
        CDBurning = 0x3b

    }
}