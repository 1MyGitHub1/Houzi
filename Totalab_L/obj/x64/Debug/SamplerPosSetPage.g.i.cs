﻿#pragma checksum "..\..\..\SamplerPosSetPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "792875E63AD5347F6FDDB9226FB55D3C45160B094D0585BF23305174367A6D89"
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
    /// SamplerPosSetPage
    /// </summary>
    public partial class SamplerPosSetPage : LabTech.UITheme.CustomWindow, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 41 "..\..\..\SamplerPosSetPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image left_point;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\SamplerPosSetPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image right_point;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\SamplerPosSetPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid position_move;
        
        #line default
        #line hidden
        
        
        #line 390 "..\..\..\SamplerPosSetPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SetData1_Visibility;
        
        #line default
        #line hidden
        
        
        #line 628 "..\..\..\SamplerPosSetPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SetData2_Visibility;
        
        #line default
        #line hidden
        
        
        #line 705 "..\..\..\SamplerPosSetPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SetData3_Visibility;
        
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
            System.Uri resourceLocater = new System.Uri("/Totalab_L;component/samplerpossetpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SamplerPosSetPage.xaml"
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
            
            #line 14 "..\..\..\SamplerPosSetPage.xaml"
            ((Totalab_L.SamplerPosSetPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.CustomWindow_Loaded);
            
            #line default
            #line hidden
            
            #line 14 "..\..\..\SamplerPosSetPage.xaml"
            ((Totalab_L.SamplerPosSetPage)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.WindowCloseCommand);
            
            #line default
            #line hidden
            return;
            case 2:
            this.left_point = ((System.Windows.Controls.Image)(target));
            return;
            case 3:
            this.right_point = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.position_move = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            
            #line 63 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand_para);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 108 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand_para);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 158 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.StdTrayTypeBtn_ClickCommand_para);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 277 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand_para);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 328 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayTypeBtn_ClickCommand_para);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 384 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.SetCalibrationCommand);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 386 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.SetTestCommand);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 388 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.SetSampleCommand);
            
            #line default
            #line hidden
            return;
            case 20:
            this.SetData1_Visibility = ((System.Windows.Controls.Grid)(target));
            return;
            case 21:
            
            #line 422 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.HomeCommand);
            
            #line default
            #line hidden
            return;
            case 22:
            
            #line 438 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearErrorCommand);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 465 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToTargetLocationCommand);
            
            #line default
            #line hidden
            return;
            case 24:
            
            #line 485 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 25:
            
            #line 500 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 26:
            
            #line 539 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToTargetLocationCommand_calibration);
            
            #line default
            #line hidden
            return;
            case 27:
            
            #line 557 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToTargetLocationCommand_calibration);
            
            #line default
            #line hidden
            return;
            case 28:
            
            #line 582 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TurnToTargetLocationCommand_0);
            
            #line default
            #line hidden
            return;
            case 29:
            
            #line 600 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TurnToTargetLocationCommand_0);
            
            #line default
            #line hidden
            return;
            case 30:
            
            #line 621 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CancelCommand);
            
            #line default
            #line hidden
            return;
            case 31:
            
            #line 623 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SavePosCommand);
            
            #line default
            #line hidden
            return;
            case 32:
            this.SetData2_Visibility = ((System.Windows.Controls.Grid)(target));
            return;
            case 33:
            
            #line 652 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToTargetLocationCommand);
            
            #line default
            #line hidden
            return;
            case 34:
            
            #line 672 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 35:
            
            #line 687 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 36:
            this.SetData3_Visibility = ((System.Windows.Controls.Grid)(target));
            return;
            case 37:
            
            #line 730 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.GoToXYCommand);
            
            #line default
            #line hidden
            return;
            case 38:
            
            #line 750 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 39:
            
            #line 765 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 40:
            
            #line 780 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveToZCommand);
            
            #line default
            #line hidden
            return;
            case 41:
            
            #line 801 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SavePosCommand);
            
            #line default
            #line hidden
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
            case 6:
            
            #line 79 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayAItemClickCommand_para);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 124 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayBItemClickCommand_para);
            
            #line default
            #line hidden
            break;
            case 10:
            
            #line 177 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayCItemClickCommand_para);
            
            #line default
            #line hidden
            break;
            case 11:
            
            #line 208 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayCItemClickCommand_para);
            
            #line default
            #line hidden
            break;
            case 12:
            
            #line 241 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayCItemClickCommand_para);
            
            #line default
            #line hidden
            break;
            case 14:
            
            #line 294 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayDItemClickCommand_para);
            
            #line default
            #line hidden
            break;
            case 16:
            
            #line 345 "..\..\..\SamplerPosSetPage.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Click += new System.Windows.RoutedEventHandler(this.TrayEItemClickCommand_para);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

