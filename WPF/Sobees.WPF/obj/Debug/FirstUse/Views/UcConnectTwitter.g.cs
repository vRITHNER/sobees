﻿#pragma checksum "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "CE03548185524F2EFB9C5C78EF1CC0D005EDC8841BD27F7940E0AF210B58B449"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Converters;
using Sobees.Library.BLocalizeLib;
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


namespace Sobees.FirstUse.Views {
    
    
    /// <summary>
    /// UcConnectTwitter
    /// </summary>
    public partial class UcConnectTwitter : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnTwitterPinCodeLaunchBrowser;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel spSecurityCode;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtTwitterPinCode;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel spButtonLogin;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnTwitterPinCodeSignIn;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtblError;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstAccountsTwitter;
        
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
            System.Uri resourceLocater = new System.Uri("/Sobees;component/firstuse/views/ucconnecttwitter.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
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
            this.grid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.btnTwitterPinCodeLaunchBrowser = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.spSecurityCode = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 4:
            this.txtTwitterPinCode = ((System.Windows.Controls.TextBox)(target));
            
            #line 61 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
            this.txtTwitterPinCode.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtTwitterLogin_TextChanged);
            
            #line default
            #line hidden
            
            #line 62 "..\..\..\..\FirstUse\Views\UcConnectTwitter.xaml"
            this.txtTwitterPinCode.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtTwitterLogin_KeyDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.spButtonLogin = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.btnTwitterPinCodeSignIn = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.txtblError = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.lstAccountsTwitter = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

