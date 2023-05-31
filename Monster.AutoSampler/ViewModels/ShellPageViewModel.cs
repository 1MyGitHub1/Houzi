using Caliburn.Micro;
using LabTech.Common;
using Mass.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Monster.AutoSampler.ViewModels
{
    [Export(typeof(ShellPageViewModel))]
    public class ShellPageViewModel : BaseData.BaseViewModel
    {
        #region 构造函数
        [ImportingConstructor]
        public ShellPageViewModel(IEventAggregator events, IWindowManager windowManager)
            : base(events, windowManager)
        {
            DisplayName = Win32Helper.GetEntryAssemblyTitle();
            if (Control_SubPageModule == null)
            {
                Control_SubPageModule = this.GetInstance<DeviceModuleViewModel>();
                Control_SubPageModule.InitData();
            }
            this.ActivateItem(Control_SubPageModule);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否关闭Window窗体
        /// </summary>
        public bool IsCloseWin
        {
            get => this._isCloseWin;
            private set => this.Set(ref this._isCloseWin, value);
        }
        private bool _isCloseWin = false;

        public DeviceModuleViewModel Control_SubPageModule
        {
            get => this._control_SubPageModule;
            set => this.Set(ref this._control_SubPageModule, value);
        }
        private DeviceModuleViewModel _control_SubPageModule;
        #endregion

        #region 事件
        /// <summary>
        /// 准备退出程序
        /// </summary>
        /// <param name="args"></param>
        public void MainClosing(CancelEventArgs args)
        {
            if (!IsCloseWin)
            {
                args.Cancel = true;
                bool? messResult = this.GetObject<MessagePageViewModel>().ShowDialog("Message_Error1027".GetWord(), "MessageTitle_Exit".GetWord(), true,
                    Enum_MessageType.Question, "ButtonContent_Yes".GetWord(), "ButtonContent_No".GetWord());
                if (messResult != true)
                    return;
                //会紧接着调用Bootstrapper中的OnExit事件
                args.Cancel = false;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Environment.Exit(0);
                MainLogHelper.Instance.Info("main thread exit.");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
     
            }
        }
        #endregion
    }
}