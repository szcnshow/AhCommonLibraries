﻿#pragma checksum "..\..\..\Controls\TxtDataBrowser.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "82A4D04BA783560463057DF20E2F1FAFAECC7F78"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Ai.Hong.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TAlex.WPF.Controls;


namespace Ai.Hong.Controls {
    
    
    /// <summary>
    /// TxtDataBrowser
    /// </summary>
    public partial class TxtDataBrowser : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox listSeparator;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TAlex.WPF.Controls.NumericUpDown numFirstRow;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock labelFirstCol;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TAlex.WPF.Controls.NumericUpDown numFirstCol;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox XAxisUnit;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox YAxisUnit;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock labelYCols;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TAlex.WPF.Controls.NumericUpDown numYCols;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textDataViewer;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\Controls\TxtDataBrowser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid textDataList;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AHCommonControls;component/controls/txtdatabrowser.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\TxtDataBrowser.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.listSeparator = ((System.Windows.Controls.ComboBox)(target));
            
            #line 26 "..\..\..\Controls\TxtDataBrowser.xaml"
            this.listSeparator.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.listSeparator_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.numFirstRow = ((TAlex.WPF.Controls.NumericUpDown)(target));
            
            #line 32 "..\..\..\Controls\TxtDataBrowser.xaml"
            this.numFirstRow.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<decimal>(this.numFirstRow_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.labelFirstCol = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.numFirstCol = ((TAlex.WPF.Controls.NumericUpDown)(target));
            
            #line 34 "..\..\..\Controls\TxtDataBrowser.xaml"
            this.numFirstCol.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<decimal>(this.numFirstCol_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.XAxisUnit = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.YAxisUnit = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.labelYCols = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.numYCols = ((TAlex.WPF.Controls.NumericUpDown)(target));
            
            #line 49 "..\..\..\Controls\TxtDataBrowser.xaml"
            this.numYCols.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<decimal>(this.numYCols_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.textDataViewer = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.textDataList = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

