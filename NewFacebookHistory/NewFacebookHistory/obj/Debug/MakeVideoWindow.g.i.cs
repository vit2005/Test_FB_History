﻿#pragma checksum "..\..\MakeVideoWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7890273E4609BDBCC42B3039F377B26B"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18408
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
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


namespace NewFacebookHistory {
    
    
    /// <summary>
    /// MakeVideoWindow
    /// </summary>
    public partial class MakeVideoWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid advanture_settings;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DateFrom;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DateDue;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox minutes_textbox;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox seconds_textbox;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox audio_textbox;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Start_button;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\MakeVideoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar ProgressBar;
        
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
            System.Uri resourceLocater = new System.Uri("/NewFacebookHistory;component/makevideowindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MakeVideoWindow.xaml"
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
            this.advanture_settings = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.DateFrom = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            this.DateDue = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 4:
            this.minutes_textbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.seconds_textbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.audio_textbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            
            #line 16 "..\..\MakeVideoWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_2);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Start_button = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\MakeVideoWindow.xaml"
            this.Start_button.Click += new System.Windows.RoutedEventHandler(this.Start_button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.ProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 10:
            
            #line 22 "..\..\MakeVideoWindow.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Click += new System.Windows.RoutedEventHandler(this.CheckBox_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

