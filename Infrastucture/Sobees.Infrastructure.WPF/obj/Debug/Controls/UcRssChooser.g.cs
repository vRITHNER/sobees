﻿#pragma checksum "..\..\..\Controls\UcRssChooser.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E3F6D9BDDF69D1ACD4B4BE279AA6AEB78CE21730DD8239902A7AEE5B26CC7837"
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


namespace Sobees.Infrastructure.Controls {
    
    
    /// <summary>
    /// UcRssChooser
    /// </summary>
    public partial class UcRssChooser : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 6 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Sobees.Infrastructure.Controls.UcRssChooser ucRssChooser;
        
        #line default
        #line hidden
        
        
        #line 244 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstCategories;
        
        #line default
        #line hidden
        
        
        #line 249 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstItems;
        
        #line default
        #line hidden
        
        
        #line 260 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbxLanguage;
        
        #line default
        #line hidden
        
        
        #line 277 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtUrl;
        
        #line default
        #line hidden
        
        
        #line 284 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCheck;
        
        #line default
        #line hidden
        
        
        #line 291 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Sobees.Infrastructure.Controls.UcWaiting isWaiting;
        
        #line default
        #line hidden
        
        
        #line 295 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtError;
        
        #line default
        #line hidden
        
        
        #line 298 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAddRss;
        
        #line default
        #line hidden
        
        
        #line 300 "..\..\..\Controls\UcRssChooser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstItems2;
        
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
            System.Uri resourceLocater = new System.Uri("/Sobees.Infrastructure;component/controls/ucrsschooser.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\UcRssChooser.xaml"
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
            this.ucRssChooser = ((Sobees.Infrastructure.Controls.UcRssChooser)(target));
            return;
            case 6:
            this.lstCategories = ((System.Windows.Controls.ListBox)(target));
            return;
            case 7:
            this.lstItems = ((System.Windows.Controls.ListBox)(target));
            return;
            case 8:
            this.cbxLanguage = ((System.Windows.Controls.ComboBox)(target));
            
            #line 260 "..\..\..\Controls\UcRssChooser.xaml"
            this.cbxLanguage.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.CbxLanguageSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.txtUrl = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.btnCheck = ((System.Windows.Controls.Button)(target));
            
            #line 287 "..\..\..\Controls\UcRssChooser.xaml"
            this.btnCheck.Click += new System.Windows.RoutedEventHandler(this.VerifiyUrl);
            
            #line default
            #line hidden
            return;
            case 11:
            this.isWaiting = ((Sobees.Infrastructure.Controls.UcWaiting)(target));
            return;
            case 12:
            this.txtError = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.btnAddRss = ((System.Windows.Controls.Button)(target));
            return;
            case 14:
            this.lstItems2 = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            
            #line 154 "..\..\..\Controls\UcRssChooser.xaml"
            ((System.Windows.Controls.Grid)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Grid_Loaded);
            
            #line default
            #line hidden
            break;
            case 3:
            
            #line 155 "..\..\..\Controls\UcRssChooser.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnAddRssClick);
            
            #line default
            #line hidden
            break;
            case 4:
            
            #line 181 "..\..\..\Controls\UcRssChooser.xaml"
            ((System.Windows.Controls.Grid)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Grid_Loaded);
            
            #line default
            #line hidden
            break;
            case 5:
            
            #line 183 "..\..\..\Controls\UcRssChooser.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnAddRssClick);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

