﻿#pragma checksum "..\..\ShellPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "71A9556276C866C7CCB45124547D2531D89C58EB93C804EA954D0BCE1268138C"
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
using Mass.Common;
using Mass.UITheme;
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
using Totalab_L.Themes;


namespace Totalab_L {
    
    
    /// <summary>
    /// ShellPage
    /// </summary>
    public partial class ShellPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 127 "..\..\ShellPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock NameText;
        
        #line default
        #line hidden
        
        
        #line 254 "..\..\ShellPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_About;
        
        #line default
        #line hidden
        
        
        #line 705 "..\..\ShellPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContentControl Content_ActiveItem;
        
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
            System.Uri resourceLocater = new System.Uri("/Totalab_L;component/shellpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ShellPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            
            #line 13 "..\..\ShellPage.xaml"
            ((Totalab_L.ShellPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 48 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Click += new System.Windows.RoutedEventHandler(this.UseAutoSamplerCommand);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 100 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ConnectionCommand);
            
            #line default
            #line hidden
            return;
            case 4:
            this.NameText = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            
            #line 133 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CreateMethodCommand);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 160 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenMethodCommand);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 187 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveMethodCommand);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 219 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveAsMethodCommand);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 238 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AdvancedSetCommand);
            
            #line default
            #line hidden
            return;
            case 10:
            this.Btn_About = ((System.Windows.Controls.Button)(target));
            
            #line 254 "..\..\ShellPage.xaml"
            this.Btn_About.Click += new System.Windows.RoutedEventHandler(this.Btn_About_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 273 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.SampleListCommand);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 275 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.SettingCommand);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 339 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.TextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.CurrentPositionLostFocusCommand);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 340 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 343 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToPositionCommand);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 345 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZHomeCommand);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 347 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToHomeCommand);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 356 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand);
            
            #line default
            #line hidden
            return;
            case 20:
            
            #line 422 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand);
            
            #line default
            #line hidden
            return;
            case 22:
            
            #line 477 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.StdTrayTypeBtn_ClickCommand);
            
            #line default
            #line hidden
            return;
            case 26:
            
            #line 585 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand);
            
            #line default
            #line hidden
            return;
            case 28:
            
            #line 638 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand);
            
            #line default
            #line hidden
            return;
            case 30:
            this.Content_ActiveItem = ((System.Windows.Controls.ContentControl)(target));
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
            case 19:
            
            #line 384 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayAItemClickCommand);
            
            #line default
            #line hidden
            break;
            case 21:
            
            #line 437 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayBItemClickCommand);
            
            #line default
            #line hidden
            break;
            case 23:
            
            #line 494 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayCItemClickCommand);
            
            #line default
            #line hidden
            break;
            case 24:
            
            #line 528 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayCItemClickCommand);
            
            #line default
            #line hidden
            break;
            case 25:
            
            #line 545 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayCItemClickCommand);
            
            #line default
            #line hidden
            break;
            case 27:
            
            #line 600 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayDItemClickCommand);
            
            #line default
            #line hidden
            break;
            case 29:
            
            #line 653 "..\..\ShellPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayEItemClickCommand);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

