﻿#pragma checksum "..\..\..\Update.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C5E231D0EFC14EEE141A5E6571A84F24"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace ACPLogAnalyzer {
    
    
    /// <summary>
    /// Update
    /// </summary>
    public partial class Update : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LabelTitle;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LabelVersionRunningx;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LabelVersionAvailablex;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar ProgressBarDownload;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonDownloadUpdate;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CheckBoxCheck4Updates;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBoxAvailable;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBoxRunning;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox GroupBox1;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WebBrowser WebBrowserRelNotes;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Update.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonClose;
        
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
            System.Uri resourceLocater = new System.Uri("/ACPLogAnalyzer;component/update.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Update.xaml"
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
            
            #line 4 "..\..\..\Update.xaml"
            ((ACPLogAnalyzer.Update)(target)).Activated += new System.EventHandler(this.WindowActivated);
            
            #line default
            #line hidden
            
            #line 4 "..\..\..\Update.xaml"
            ((ACPLogAnalyzer.Update)(target)).Loaded += new System.Windows.RoutedEventHandler(this.WindowLoaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LabelTitle = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.LabelVersionRunningx = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.LabelVersionAvailablex = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.ProgressBarDownload = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 6:
            this.ButtonDownloadUpdate = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\..\Update.xaml"
            this.ButtonDownloadUpdate.Click += new System.Windows.RoutedEventHandler(this.ButtonDownloadUpdateClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CheckBoxCheck4Updates = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 8:
            this.TextBoxAvailable = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.TextBoxRunning = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.GroupBox1 = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 11:
            this.WebBrowserRelNotes = ((System.Windows.Controls.WebBrowser)(target));
            
            #line 20 "..\..\..\Update.xaml"
            this.WebBrowserRelNotes.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(this.WebBrowserRelNotesLoadCompleted);
            
            #line default
            #line hidden
            return;
            case 12:
            this.ButtonClose = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\Update.xaml"
            this.ButtonClose.Click += new System.Windows.RoutedEventHandler(this.ButtonCloseClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

