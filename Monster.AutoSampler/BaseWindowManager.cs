using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Monster.AutoSampler
{
    public class BaseWindowManager : WindowManager
    {
        /// <summary>
        /// 上一次打开的窗体
        /// </summary>
        private Window _lastOpenWindow = null;

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            Window window = base.EnsureWindow(model, view, isDialog);
            if (window is Views.ShellPageView && !(Application.Current.MainWindow is Views.ShellPageView))
            {
                //将ShellPage窗体设为主窗体
                Application.Current.MainWindow = window;
            }
            _lastOpenWindow = window;
            return window;
        }
    }
}