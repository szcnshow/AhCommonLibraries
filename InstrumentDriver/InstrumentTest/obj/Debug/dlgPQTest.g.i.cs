﻿#pragma checksum "..\..\DlgPQTest.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4BDEE5F29ABC36FF221AA711F93B81041C54F792"
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
using Ai.Hong.Driver.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace Ai.Hong.Driver.IT {
    
    
    /// <summary>
    /// dlgPQTest
    /// </summary>
    public partial class dlgPQTest : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\DlgPQTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid rootGrid;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\DlgPQTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Ai.Hong.Controls.VectorImageButton btnStartScan;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\DlgPQTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Ai.Hong.Driver.Controls.ClockScannerPanel scanProgress;
        
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
            System.Uri resourceLocater = new System.Uri("/AHInstrumentTest;component/dlgpqtest.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DlgPQTest.xaml"
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
            this.rootGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.btnStartScan = ((Ai.Hong.Controls.VectorImageButton)(target));
            
            #line 24 "..\..\DlgPQTest.xaml"
            this.btnStartScan.Clicked += new System.Windows.RoutedEventHandler(this.btnStartScan_Clicked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.scanProgress = ((Ai.Hong.Driver.Controls.ClockScannerPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

