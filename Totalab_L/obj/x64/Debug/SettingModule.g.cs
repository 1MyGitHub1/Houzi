﻿#pragma checksum "..\..\..\SettingModule.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "694DFAA6F2B7EF73B66D184A5BEABA5DA041A300AD15E79600C3E7AC69F4D8CE"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using LabTech.UITheme;
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
using Totalab_L;


namespace Totalab_L {
    
    
    /// <summary>
    /// SettingModule
    /// </summary>
    public partial class SettingModule : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\..\SettingModule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock name_parameter;
        
        #line default
        #line hidden
        
        
        #line 341 "..\..\..\SettingModule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox FlushTime_ComboBox;
        
        #line default
        #line hidden
        
        
        #line 529 "..\..\..\SettingModule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox AnalysTime_ComboBox;
        
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
            System.Uri resourceLocater = new System.Uri("/Totalab_L;component/settingmodule.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SettingModule.xaml"
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
            this.name_parameter = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            
            #line 41 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenParaCommand);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 61 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveParaCommand);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 116 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.GoToXYCommand);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 126 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SetCommand);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 129 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.GoToCommand);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 166 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RinseCommand);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 171 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.StopRinseCommand);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 245 "..\..\..\SettingModule.xaml"
            ((LabTech.UITheme.CustomDataGrid)(target)).BtnAddNew_Click += new System.Windows.RoutedEventHandler(this.AddPreWashInfoCommand);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 248 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.DeletePreWashCommand);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 249 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearPreWashCommand);
            
            #line default
            #line hidden
            return;
            case 12:
            this.FlushTime_ComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 13:
            this.AnalysTime_ComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 14:
            
            #line 578 "..\..\..\SettingModule.xaml"
            ((LabTech.UITheme.CustomDataGrid)(target)).BtnAddNew_Click += new System.Windows.RoutedEventHandler(this.AddAfterRunningInfoCommand);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 581 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteAfterRunningCommand);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 582 "..\..\..\SettingModule.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearAfterRunningCommand);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

