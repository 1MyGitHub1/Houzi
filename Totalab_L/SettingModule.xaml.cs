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
        private int _posNumber = 1001;

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

        public ObservableCollection<string> PreWashLocList
        {
            get { return _preWashLocList; }
            set
            {
                _preWashLocList = value;
                Notify("PreWashLocList");
            }
        }
        private ObservableCollection<string> _preWashLocList = new ObservableCollection<string>
        {
            "1",
            "2",
            "3",
            "RinseLoc",
            "SampleLoc",
        };

        public ObservableCollection<string> AfterWashLocList
        {
            get { return _afterWashLocList; }
            set
            {
                _afterWashLocList = value;
                Notify("AfterWashLocList");
            }
        }
        private ObservableCollection<string> _afterWashLocList = new ObservableCollection<string>
        {
            "1",
            "2",
            "3",
            "RinseLoc",
            //"SampleLoc",
        };

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

        public void SetCommand(object sender, RoutedEventArgs e)
        {
            IsSetDepth = !IsSetDepth;
        }

        public void GoToCommand(object sender, RoutedEventArgs e)
        {
            Control_ParentView.IsSamplerManual = true;
            GlobalInfo.Instance.IsBusy = true;
            //GlobalInfo.Instance.Totalab_LSerials.MoveToXYZAlone(0x02, GlobalInfo.Instance.SettingInfo.SamplingDepth);
            IsSetDepth = !IsSetDepth;
        }

        public void GoToXYCommand(object sender, RoutedEventArgs e)
        {
            Control_ParentView.IsSamplerManual = true;
            GlobalInfo.Instance.IsBusy = true;
            SampleHelper.GoToSamplerXYPos(PosNumber);
            //GlobalInfo.Instance.Totalab_LSerials.MoveToXY(GlobalInfo.Instance.XStep, GlobalInfo.Instance.YStep);
            IsCanZ = true;
        }
        public void GoToHomeCommand(object sender, RoutedEventArgs e)
        {
            Control_ParentView.IsSamplerManual = true;
            GlobalInfo.Instance.IsBusy = true;
            //GlobalInfo.Instance.Totalab_LSerials.XYZHome();
            IsCanZ = false;
        }

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
                GlobalInfo.Instance.IsCanRunning = false;
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
        }

        public void RinseRun()
        {
            try
            {
                //GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleXY;
                //GlobalInfo.Instance.Totalab_LSerials.MoveToXY(GlobalInfo.Instance.SamplerPos.HomeX, 0);

                //this.Dispatcher.Invoke(new Action(delegate
                //{
                //    GlobalInfo.Instance.LogInfo.Add(DateTime.Now.ToString("hh:mm:ss") + "走到清洗位");
                //}));
                //while (GlobalInfo.Instance.RunningStep != RunningStep_Status.GoToSampleXYOk)
                //{
                //    Thread.Sleep(20);
                //}
                //if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleXYOk)
                //{
                //    GlobalInfo.Instance.Totalab_LSerials.MoveToXYZAlone(0x02, GlobalInfo.Instance.SettingInfo.SamplingDepth);
                //    GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleDepth;
                //    this.Dispatcher.Invoke(new Action(delegate
                //    {
                //        GlobalInfo.Instance.LogInfo.Add(DateTime.Now.ToString("hh:mm:ss") + "下降到样品深度");
                //    }));
                //}
                //while (GlobalInfo.Instance.RunningStep != RunningStep_Status.GoToSampleDepthOk)
                //{
                //    Thread.Sleep(200);
                //}
                //if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleDepthOk)
                //{
                //    GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                //}
                //if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                //{
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
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
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
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
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
                        //GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHome;
                        //GlobalInfo.Instance.Totalab_LSerials.XYZHome();
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
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
                        //ConntectWaring();
                        //return;
                    }
                }
                //while (GlobalInfo.Instance.RunningStep != RunningStep_Status.XYZHomeOk)
                //{
                //    Thread.Sleep(20);
                //}
                //if(GlobalInfo.Instance.RunningStep == RunningStep_Status.XYZHomeOk)
                //{
                //    GlobalInfo.Instance.IsCanRunning = true;
                //    GlobalInfo.Instance.IsBusy = false;
                //    if(GlobalInfo.Instance.IsHimassConnState)
                //        Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                //            new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerState, Parameter = GlobalInfo.Instance.IsBusy });
                //    //methodCancelSource.Cancel();
                //}

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
    }
}
