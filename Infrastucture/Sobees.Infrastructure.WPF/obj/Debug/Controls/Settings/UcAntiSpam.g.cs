﻿#pragma checksum "..\..\..\..\Controls\Settings\UcAntiSpam.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "321631BC044056D194B01764EF59F5CFD74BDFD476628A1F4DD168FB50746761"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Sobees.Library.BLocalizeLib;
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


namespace Sobees.Infrastructure.Controls.Settings {
    
    
    /// <summary>
    /// UcAntiSpam
    /// </summary>
    public partial class UcAntiSpam : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 93 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtBlAntiSpamx;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel spEnterSpam;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtBlEnterYourSpam;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSpam;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAddSpam;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstSpam;
        
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
            System.Uri resourceLocater = new System.Uri("/Sobees.Infrastructure;component/controls/settings/ucantispam.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
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
            this.txtBlAntiSpamx = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.spEnterSpam = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 3:
            this.txtBlEnterYourSpam = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.txtSpam = ((System.Windows.Controls.TextBox)(target));
            
            #line 114 "..\..\..\..\Controls\Settings\UcAntiSpam.xaml"
            this.txtSpam.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtKeywords1_KeyDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnAddSpam = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.lstSpam = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

