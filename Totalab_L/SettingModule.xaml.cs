using Totalab_L.Enum;
using Totalab_L.Models;
using DeviceInterface;
using LabTech.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Totalab_L.Common;

namespace Totalab_L
{
    /// <summary>
    /// SettingModule.xaml 的交互逻辑
    /// </summary>
    public partial class SettingModule : UserControl, INotifyPropertyChanged
    {
        public SettingModule()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 属性
        public ShellPage Control_ParentView
        {
            get => _control_ParentView;
            set
            {
                _control_ParentView = value;
                Notify("Control_ParentView");
            }
        }
        private ShellPage _control_ParentView;
        public bool IsSetDepth
        {
            get => _isSetDepth;
            set
            {
                _isSetDepth = value;
                Notify("IsSetDepth");
            }
        }
        private bool _isSetDepth = true;

        public int PosNumber
        {
            get => _posNumber;
            set
            {
                _posNumber = value;
                Notify("PosNumber");
            }
        }
        private int _posNumber = 1;

        public bool IsCanZ
        {
            get => _isCanZ;
            set
            {
                _isCanZ = value;
                Notify("IsCanZ");
            }
        }
        private bool _isCanZ;

        public bool IsRinse
        {
            get { return _isRinse; }
            set
            {
                _isRinse = value;
                Notify("IsRinse");
            }
        }
        private bool _isRinse;
        public string Method_Name
        {
            get { return _method_Name; }
            set
            {
                _method_Name = value;
                Notify("Method_Name");
            }

        }
        private string _method_Name;

        public ObservableCollection<string> PreWashLocList
        {
            get { return _preWashLocList; }
            set
            {
                _preWashLocList = value;
                Notify("PreWashLocList");
            }
        }
        private ObservableCollection<string> _preWashLocList = new ObservableCollection<string>();
        public ObservableCollection<string> AfterWashLocList
        {
            get { return _afterWashLocList; }
            set
            {
                _afterWashLocList = value;
                Notify("AfterWashLocList");
            }
        }
        private ObservableCollection<string> _afterWashLocList = new ObservableCollection<string>();

        public MethodSelectorPage Control_MethodSelectorView
        {
            get => _control_MethodSelectorView;
            set
            {
                _control_MethodSelectorView = value;
                Notify("Control_MethodSelectorView");
            }
        }
        private MethodSelectorPage _control_MethodSelectorView;

        #endregion

        SamplerPosSetPage samplerPosSetPage = new SamplerPosSetPage();

        CancellationTokenSource methodCancelSource;
        Thread methodTask;
        //private int ConnectMaxTimes = 10;
        #region 事件
        //public void AddPreRunningInfoCommand(object sender, RoutedEventArgs e)
        //{
        //    if (GlobalInfo.Instance.SettingInfo.PreRunningInfo.Count < 6)
        //    {
        //        string action = GlobalInfo.Instance.SettingInfo.PreRunningInfo.Last().WashAction;
        //        if (action.Length == 5)
        //            action = "1";
        //        else
        //            action = (int.Parse(action.Substring(5, action.Length - 5)) + 1).ToString();

        //        GlobalInfo.Instance.SettingInfo.PreRunningInfo.Add(new Models.ParaItemInfo
        //        {
        //            WashAction = "Flush" + action,
        //            IsCanDelete = true
        //        });
        //        GlobalInfo.Instance.SettingInfo.PreRunningInfo.Add(new Models.ParaItemInfo
        //        {
        //            WashAction = "Delay" + action,
        //            IsCanDelete = true
        //        });
        //    }
        //}

        public void AddPreWashInfoCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!GlobalInfo.Instance.SettingInfo.PreWashInfos[0].IsOpenAction || GlobalInfo.Instance.SettingInfo.PreWashInfos.Count >= 10)
                    return;

                GlobalInfo.Instance.SettingInfo.PreWashInfos.Add(new Models.PreWashItemInfo
                {
                    StepName = "",
                    ActionSwitchVisibility = Visibility.Collapsed,
                });
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("AddPreWashInfoCommand：", ex);
            }
        }

        public void AddAfterRunningInfoCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Count < 8)
                {
                    ParaItemInfo paraItemInfo = GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Last();
                    string action = "";

                    if (paraItemInfo != null)
                    {
                        if (paraItemInfo.WashActionKey == 2)
                        {
                            action = "1";
                        }
                        else
                        {
                            action = paraItemInfo.WashAction.Substring(paraItemInfo.WashAction.Length - 1, 1);
                            if (int.TryParse(action, out int num))
                            {
                                num++;
                                action = num.ToString();
                            }
                        }
                    }
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Add(new Models.ParaItemInfo
                    {
                        WashAction = "AutoSampler_Main_Wash".GetWord() + action,
                        IsCanDelete = true,
                        WashLoc = "1"
                    });
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Add(new Models.ParaItemInfo
                    {
                        WashAction = "AutoSampler_Main_WashNeedle".GetWord() + action,
                        IsCanDelete = true,
                        WashLoc = "1"
                    });
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("AddAfterRunningInfoCommand：", ex);
            }
        }

        public void DeletePreWashCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                PreWashItemInfo perWashInfo = (sender as MenuItem).DataContext as PreWashItemInfo;
                GlobalInfo.Instance.SettingInfo.PreWashInfos.Remove(perWashInfo);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("DeletePreWashCommand：", ex);
            }
        }

        public void ClearPreWashCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 1; i < GlobalInfo.Instance.SettingInfo.PreWashInfos.Count; i = 1)
                    GlobalInfo.Instance.SettingInfo.PreWashInfos.RemoveAt(1);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ClearPreWashCommand：", ex);
            }
        }

        //public void DeletePreRunningCommand(object sender, RoutedEventArgs e)
        //{
        //    ParaItemInfo itemInfo = (sender as MenuItem).DataContext as ParaItemInfo;
        //    int index = GlobalInfo.Instance.SettingInfo.PreRunningInfo.IndexOf(GlobalInfo.Instance.SettingInfo.PreRunningInfo.Where(m => m == itemInfo).FirstOrDefault());
        //    if (index % 2 == 1)
        //    {
        //        GlobalInfo.Instance.SettingInfo.PreRunningInfo.RemoveAt(index - 1);
        //        GlobalInfo.Instance.SettingInfo.PreRunningInfo.RemoveAt(index - 1);
        //    }
        //    else
        //    {
        //        GlobalInfo.Instance.SettingInfo.PreRunningInfo.RemoveAt(index);
        //        GlobalInfo.Instance.SettingInfo.PreRunningInfo.RemoveAt(index);
        //    }
        //}
        private void DeleteAfterRunningCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                ParaItemInfo itemInfo = (sender as MenuItem).DataContext as ParaItemInfo;
                int index = GlobalInfo.Instance.SettingInfo.AfterRunningInfo.IndexOf(GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Where(m => m == itemInfo).FirstOrDefault());
                if (index % 2 == 1)
                {
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index - 1);
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index - 1);
                }
                else
                {
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index);
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("DeleteAfterRunningCommand：", ex);
            }
        }
        public void ClearPreRunningCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 2; i < GlobalInfo.Instance.SettingInfo.PreRunningInfo.Count; i = 2)
                {
                    GlobalInfo.Instance.SettingInfo.PreRunningInfo.RemoveAt(2);
                    GlobalInfo.Instance.SettingInfo.PreRunningInfo.RemoveAt(2);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ClearPreRunningCommand：", ex);
            }
        }

        public void DeleteAfterWashCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                ParaItemInfo itemInfo = (sender as MenuItem).DataContext as ParaItemInfo;
                int index = GlobalInfo.Instance.SettingInfo.AfterRunningInfo.IndexOf(GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Where(m => m == itemInfo).FirstOrDefault());
                if (index % 2 == 1)
                {
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index - 1);
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index - 1);
                }
                else
                {
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index);
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("DeleteAfterWashCommand：", ex);
            }
        }

        public void ClearAfterRunningCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 2; i < GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Count; i = 2)
                {
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(2);
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo.RemoveAt(2);
                }
            }
            catch (Exception ex) { MainLogHelper.Instance.Error("ClearAfterRunningCommand：", ex); }
        }
        //Z设置
        public void SetCommand(object sender, RoutedEventArgs e)
        {
            IsSetDepth = !IsSetDepth;
        }
        //设置---进样针下降Go
        public void GoToCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Zlength = 30;
                Control_ParentView.IsSamplerManual = true;
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        long longseconds = DateTime.Now.Ticks / 10000;
                        int count = 0;
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        GlobalInfo.Instance.Totalab_LSerials.SetLeakage_tank(0x14);     //打开
                        Thread.Sleep(100);

                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.SettingInfo.SamplingDepth) / GlobalInfo.ZLengthPerCircle * 3600));
                        longseconds = DateTime.Now.Ticks / 10000;
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.SettingInfo.SamplingDepth) / GlobalInfo.ZLengthPerCircle * 3600));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SettingModule [GoToCommand]", ex);
                    }
                    finally
                    {
                        Control_ParentView.IsSamplerManual = false;
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SettingModule[GoToCommand]", ex); }
            IsSetDepth = !IsSetDepth;
        }
        //设置----进样针位置
        public void GoToXYCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                Control_ParentView.IsSamplerManual = true;
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        long longseconds = DateTime.Now.Ticks / 10000;
                        int count = 0;
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }

                        samplerPosSetPage.MoveToZ_0Command();
                        Thread.Sleep(1000);

                        Point pt = new Point();
                        int isCollisionStatus = 0;
                        pt = GetPositionInfoHelper.GetItemPosition(PosNumber);
                        isCollisionStatus = GetPositionInfoHelper.GetXIsCollision(pt, PosNumber);
                        if (isCollisionStatus == 1)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((100 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((100 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }
                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        else if (isCollisionStatus == 2)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((354 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((354 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {
                                    return;
                                }
                                else
                                    break;
                            }
                        }
                      
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                        if (isCollisionStatus == 0)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                        }
                        else
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = true;
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                                                if (isCollisionStatus == 0)
                                                {
                                                    GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                }
                                                //Thread.Sleep(300);
                                                //GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                       
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        if(isCollisionStatus == 0)
                        {
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                        }
                        else
                            GlobalInfo.Instance.IsMotorWActionOk = true;
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                if (isCollisionStatus == 0)
                                                {
                                                    GlobalInfo.Instance.IsMotorWActionOk = false;
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        //GlobalInfo.Instance.IsMotorWActionOk = true;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                        if (isCollisionStatus == 0)
                        {
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        }
                        else
                            GlobalInfo.Instance.IsMotorWActionOk = true;
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                //GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                //Thread.Sleep(500);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                if (isCollisionStatus == 0)
                                                {
                                                    GlobalInfo.Instance.IsMotorWActionOk = false;
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                    }
                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }

                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SettingModule [GoToXYCommand]", ex);
                    }
                    finally
                    {
                        Control_ParentView.IsSamplerManual = false;
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }

                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SettingModule [GoToXYCommand]", ex); }
            IsCanZ = true;
        }
        //设置---复位
        public void GoToHomeCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                Control_ParentView.IsSamplerManual = true;
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHome;
                        GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                        int count = 0;
                        while (true)
                        {
                            long longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.XYZHomeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.XYZHomeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHome;
                                                GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToHomeCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToHomeCommand]：", ex);
                                    }
                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }

                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SettingModule[GoToHomeCommand]", ex);
                    }
                    finally
                    {
                        Control_ParentView.IsSamplerManual = false;
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SettingModule[GoToHomeCommand]", ex); }
            IsCanZ = false;
        }
        //设置---清洗
        public void RinseCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                if (GlobalInfo.Instance.IsHimassConnState)
                {
                    Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                                   new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerState, Parameter = GlobalInfo.Instance.IsBusy });
                }
                IsRinse = true;
                //GlobalInfo.Instance.IsCanRunning = false;
                methodTask = new Thread(RinseRun);
                methodTask.Start();
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RinseCommand：", ex);
            }
            //methodCancelSource = new CancellationTokenSource();
            //methodTask = Task.Factory.StartNew(() => RinseRun(), methodCancelSource.Token);
        }

        public void StopRinseCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                IsRinse = false;
                Control_ParentView._IsTestConnection = true;
                GlobalInfo.Instance.Totalab_LSerials.Init();
                GlobalInfo.Instance.IsCanRunning = true;
                GlobalInfo.Instance.IsBusy = false;
                if (GlobalInfo.Instance.IsHimassConnState)
                    Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                        new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerState, Parameter = GlobalInfo.Instance.IsBusy });
                methodTask.Abort();
            }
            catch { }
        }
        #endregion

        public void InitData(ShellPage shell)
        {
            Control_ParentView = shell;
            RefreshList();
        }

        public void RinseRun()
        {
            try
            {
                Control_ParentView._IsTestConnection = false;
                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeed;
                GlobalInfo.Instance.Totalab_LSerials.PumpRunSpeed(GlobalInfo.Instance.SettingInfo.PumpSpeed1);
                //}
                long longseconds = DateTime.Now.Ticks / 10000;
                int count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetPumpSpeedOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetPumpSpeedOk)
                    {

                        GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPump;
                        GlobalInfo.Instance.Totalab_LSerials.PumpRun();
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "蠕动泵运行");
                        }));
                        break;
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeed;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpRunSpeed(GlobalInfo.Instance.SettingInfo.PumpSpeed1);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[RinseRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[RinseRun]：", ex);
                            }
                          
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                }
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.OpenPumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                    }

                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.OpenPumpOk)
                    {
                        long seconds = DateTime.Now.Ticks / 10000;
                        long delaySeconds = 0;
                        while ((GlobalInfo.Instance.SettingInfo.Wash1Time - delaySeconds) > 0)
                        {
                           Totalab_LCommon.DoEvents();
                            Thread.Sleep(200);
                            delaySeconds = (DateTime.Now.Ticks / 10000 - seconds) / 1000;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "蠕动泵停止");
                        }));
                        break;
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPump;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpRun();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[RinseRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[RinseRun]：", ex);
                            }
                           
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                        //ConntectWaring();
                        //return;
                    }
                }
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);

                    }

                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.ClosePumpOk)
                    {
                        GlobalInfo.Instance.IsCanRunning = true;
                        GlobalInfo.Instance.IsBusy = false;
                        if (GlobalInfo.Instance.IsHimassConnState)
                            Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                                new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerState, Parameter = GlobalInfo.Instance.IsBusy });
                        break;
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[RinseRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[RinseRun]：", ex);
                            }
                           
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                }
                Control_ParentView._IsTestConnection = true;
                IsRinse = false;
                methodTask.Abort();
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("DeviceControlThreadMethod：", ex);
            }
        }

        public void ConntectWaring()
        {
            try
            {
                Control_ParentView.IsConnect = false;
                GlobalInfo.Instance.IsCanRunning = true;
                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                GlobalInfo.Instance.IsBusy = false;
                IsRinse = false;
                if (GlobalInfo.Instance.IsHimassConnState)
                {
                    Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                                  new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = Control_ParentView.IsConnect });
                        Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                     new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerState, Parameter = GlobalInfo.Instance.IsBusy });
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        new MessagePage().ShowDialog("Message_Error2013".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    }));
                }
                if(methodTask != null)
                   methodTask.Abort();
            }
            catch
            {

            }
        }

        private void OpenParaCommand(object sender, RoutedEventArgs e)
        {
            Control_MethodSelectorView = new MethodSelectorPage();
            Control_MethodSelectorView.IsSettingsOpen = true;
            Control_MethodSelectorView.ShowDialog(Control_ParentView);
            Method_Name = GlobalInfo.MethodName;
        }

        private void SaveParaCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".para";
                dlg.Filter = "Para Files (*.para)|*.para";
                string path = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SettingPara");
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                dlg.InitialDirectory = path;
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    byte[] content = XmlObjSerializer.Serialize(GlobalInfo.Instance.SettingInfo);
                    bool isSuccessful = FileHelper.WriteEncrypt(dlg.FileName, content);
                    if (result == true)
                        new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                    else
                        new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    //DataToXml.SaveToXml(dlg.FileName, GlobalInfo.Instance.SettingInfo);
                    //new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("SettingModule [SaveParaCommand]", ex);
                new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        public void RefreshList()
        {
            List<string> preRunningLocList = new List<string>();
            List<string> afterWashLocList = new List<string>();
            for (int i = 0;  i< GlobalInfo.Instance.SettingInfo.PreWashInfos.Count;i++)
            {
                preRunningLocList.Add(GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc);
            }
            for(int i = 0; i<GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Count;i++)
            {
                afterWashLocList.Add(GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc);
            }
            PreWashLocList = new ObservableCollection<string>
            {
               "RinseLoc",
               "SampleLoc",
             };
            AfterWashLocList = new ObservableCollection<string>
            {
               "RinseLoc",
             };
            for (int i = 0; i < GlobalInfo.Instance.TraySTD1Infos.TrayItemCount; i++)
            {
                PreWashLocList.Add(GlobalInfo.Instance.TraySTD1Infos.TrayItemList[i].ItemContent);
                AfterWashLocList.Add(GlobalInfo.Instance.TraySTD1Infos.TrayItemList[i].ItemContent);
            }
            for (int i = 0; i < GlobalInfo.Instance.TraySTD2Infos.TrayItemCount; i++)
            {
                PreWashLocList.Add(GlobalInfo.Instance.TraySTD2Infos.TrayItemList[i].ItemContent);
                AfterWashLocList.Add(GlobalInfo.Instance.TraySTD2Infos.TrayItemList[i].ItemContent);
            }
            for(int i = 0; i<preRunningLocList.Count();i++)
            {
                if (preRunningLocList[i] == "RinseLoc".ToString() || preRunningLocList[i] == "SampleLoc")
                {
                    GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc = preRunningLocList[i];
                }
                else
                    GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc = PreWashLocList[2];
            }
            for (int i = 0; i < afterWashLocList.Count(); i++)
            {
                if (afterWashLocList[i] == "RinseLoc".ToString() || afterWashLocList[i] == "SampleLoc")
                {
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc = afterWashLocList[i];
                }
                else
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc = AfterWashLocList[1];
            }
        }
    }
}
