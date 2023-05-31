using bbv.Common.EventBroker;
using bbv.Common.EventBroker.Handlers;
using Totalab_L.Enum;
using Totalab_L.Models;
using DeviceInterface;
using LabTech.Common;
using LabTech.UITheme.Enums;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.BaseServer;
using System.Windows.Controls;
using Totalab_L.Common;
using System.Windows.Controls.Primitives;

namespace Totalab_L
{
    /// <summary>
    /// ShellPage.xaml 的交互逻辑
    /// </summary>
    public partial class ShellPage : Page, IQuickSamplerPlug, INotifyPropertyChanged
    {
        public ShellPage()
        {
            InitializeComponent();
            this.DataContext = this;
            GlobalInfo.Instance.Totalab_LSerials.MsgCome += Sampler_MsgCome;
            ConnectThread = new Thread(Connect);
            ConnectThread.Start();
            ConnectThread.IsBackground = true;
            if (Control_SampleListView == null)
            {
                Control_SampleListView = new SampleListModule();
                Control_SampleListView.InitData(this);
            }
            if (Control_SettingView == null)
            {
                Control_SettingView = new SettingModule();
                Control_SettingView.InitData(this);
            }
            InitData();
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = @".\SamplerPlugins\CircleAutoSampler.dll.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            IsMedicalSoft = Convert.ToBoolean(config.AppSettings.Settings["IsMedical"].Value);
            GlobalInfo.Instance.MaxConnectionTimes = Convert.ToInt32(config.AppSettings.Settings["MaxConnectTimes"].Value);
        }

        #region 变量
        Thread ConnectThread;
        public bool _IsTestConnection;
        Thread ConnectionTestThead;
        bool _IsRecived;
        bool _IsSNWindowShow;////当前SN窗口是否弹出，避免和mass联机的时候Load和重连的时候重复弹出
        ContextMenu TrayTypeMenu = new ContextMenu();
        ContextMenu StdTrayTypeMenu = new ContextMenu();
        string ButtonContent;
        //private int ConnectMaxTimes = 10;
        #endregion

        #region 属性      
        /// <summary>
        /// 序列号管理是否已注册
        /// </summary>
        public bool IsSNRegistered
        {
            get => this._isSNRegistered;
            set
            {
                _isSNRegistered = value;
                Notify("IsSNRegistered");
            }
        }
        private bool _isSNRegistered = true;

        /// <summary>
        /// 软件脱机使用中
        /// </summary>
        public bool IsSoftwareOffline
        {
            get => this._isSoftwareOffline;
            set
            {
                _isSoftwareOffline = value;
                Notify("IsSoftwareOffline");
            }
        }
        private bool _isSoftwareOffline = false;

        /// <summary>
        /// 是否启用自动进样器
        /// </summary>
        public bool IsUseAutoSampler
        {
            get => _isUseAutoSampler;
            set
            {
                _isUseAutoSampler = value;
                Notify("IsUseAutoSampler");
            }
        }
        private bool _isUseAutoSampler;

        public SamplerPosSetPage Control_SamplerPosSetView
        {
            get => _control_SamplerPosSetView;
            set
            {
                _control_SamplerPosSetView = value;
                Notify("Control_SamplerPosSetView");
            }
        }
        private SamplerPosSetPage _control_SamplerPosSetView;

        public bool IsConnect
        {
            get => _isConnect;
            set
            {
                _isConnect = value;
                Notify("IsConnect");
            }
        }
        private bool _isConnect = true;

        public bool IsLoad
        {
            get => _isLoad;
            set
            {
                _isLoad = value;
                Notify("IsLoad");
            }
        }
        private bool _isLoad = false;

        public string RackType
        {
            get => _rackType;
            set
            {
                _rackType = value;
                Notify("RackType");
            }
        }
        private string _rackType = "Rack-89";

        /// <summary>
        /// SampleListView
        /// </summary>
        public SampleListModule Control_SampleListView
        {
            get => _control_SampleListView;
            set
            {
                _control_SampleListView = value;
                Notify("Control_SampleListView");
            }
        }
        private SampleListModule _control_SampleListView;

        public SettingModule Control_SettingView
        {
            get => _control_SettingView;
            set
            {
                _control_SettingView = value;
                Notify("Control_SettingView");
            }
        }
        private SettingModule _control_SettingView;

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

        /// <summary>
        /// 是否是医疗软件
        /// </summary>
        public bool IsMedicalSoft
        {
            get => _isMedicalSoft;
            set
            {
                _isMedicalSoft = value;
                Notify("IsMedicalSoft");
            }
        }
        private bool _isMedicalSoft = true;

        public string SamplerType
        {
            get => _sampleType;
            set
            {
                _sampleType = value;
                Notify("SamplerType");
            }
        }
        private string _sampleType = "10";

        /// <summary>
        /// List.Count代表总圈数
        /// Key 当前所在圈索引1----N，1代表最外圈，依次向里
        /// Value 当前圈里的Item项的数量
        /// </summary>
        public Dictionary<int, int> CircleGroupList
        {
            get => this._circleGroupList;
            set
            {
                _circleGroupList = value;
                Notify("CircleGroupList");
            }
        }
        private Dictionary<int, int> _circleGroupList;

        public int CurrentRotationIndex
        {
            get => this._currentRotationIndex;
            set
            {
                _currentRotationIndex = value;
                Notify("CurrentRotationIndex");
            }
        }
        private int _currentRotationIndex = -1;

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                Notify("CurrentPosition");
            }
        }
        private int _currentPosition = 1;

        public bool IsSample1ItemSelected
        {
            get { return _isSample1ItemSelected; }
            set
            {
                _isSample1ItemSelected = value;
                Notify("IsSample1ItemSelected");
            }
        }
        private bool _isSample1ItemSelected;

        public bool IsSample2ItemSelected
        {
            get { return _isSample2ItemSelected; }
            set
            {
                _isSample2ItemSelected = value;
                Notify("IsSample2ItemSelected");
            }
        }
        private bool _isSample2ItemSelected;

        public bool IsSample3ItemSelected
        {
            get { return _isSample3ItemSelected; }
            set
            {
                _isSample3ItemSelected = value;
                Notify("IsSample3ItemSelected");
            }
        }
        private bool _isSample3ItemSelected;

        /// <summary>
        /// 是否可以启用自动进样器的check
        public bool IsAutoSamplerEnable
        {
            get => _isAutoSamplerEnable;
            set
            {
                _isAutoSamplerEnable = value;
                Notify("IsAutoSamplerEnable");
            }
        }
        private bool _isAutoSamplerEnable = true;

        /// <summary>
        /// 进样器是否手动走位
        /// </summary>
        public bool IsSamplerManual
        {
            get => _isSamplerManual;
            set
            {
                _isSamplerManual = value;
                Notify("IsSamplerManual");
            }
        }
        private bool _isSamplerManual;

        public IEnumerable<EnumMemberModel> TrayNameList
        {
            get => this._trayNameList;
            set
            {
                _trayNameList = value;
                Notify("TrayNameList");
            }
        }
        private IEnumerable<EnumMemberModel> _trayNameList;

        public IEnumerable<EnumMemberModel> TrayTypeList
        {
            get => this._trayTypeList;
            set
            {
                _trayTypeList = value;
                Notify("TrayTypeList");
            }
        }
        private IEnumerable<EnumMemberModel> _trayTypeList;

        public IEnumerable<EnumMemberModel> StdTrayTypeList
        {
            get => this._stdTrayTypeList;
            set
            {
                _stdTrayTypeList = value;
                Notify("StdTrayTypeList");
            }
        }
        private IEnumerable<EnumMemberModel> _stdTrayTypeList;

        public Enum_TrayName SelectedTrayName
        {
            get => this._selectedTrayName;
            set
            {
                _selectedTrayName = value;
                Notify("SelectedTrayName");
            }
        }
        private Enum_TrayName _selectedTrayName;

        public int TrayMaxValue
        {
            get => _trayMaxValue;
            set
            {
                _trayMaxValue = value;
                Notify("TrayMaxValue");
            }
        }
        private int _trayMaxValue;
        #endregion

        #region 事件
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsSNRegistered)
                {
                    IsSoftwareOffline = false;
                    //序列号授权验证
                    RegistSN registSN = RegistSN.GetInstance("LabMonsterTotalab-I");
                    registSN.Init(new InstrumentSerialNum(), "LabMonster Totalab-I");
                    ResultData resultData = registSN.Register();
                    if (resultData != null && resultData.IsSuccessful
                             && (resultData.ErrorCodeList == null || (resultData.ErrorCodeList != null && resultData.ErrorCodeList.Count == 0)))
                    {
                        IsSNRegistered = true;
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });
                    }
                    else
                    {
                        MainLogHelper.Instance.Error("ShellPage [Page_Loaded]" + resultData != null ? resultData.Result : string.Empty);
                        bool isShowRegisterSNPage = false;
                        if (resultData != null && resultData.IsOnlySoft == true)
                        {
                            isShowRegisterSNPage = true;
                        }
                        else
                        {
                            if (registSN.IsExistsAlisql())
                            {
                                if (resultData != null && resultData.ErrorCodeList != null && resultData.ErrorCodeList.Count == 1
                                    && (resultData.ErrorCodeList[0] == UtilityLibrary.Enums.Enum_RegisterErrorCode.ReadInstMCUErr
                                    || resultData.ErrorCodeList[0] == UtilityLibrary.Enums.Enum_RegisterErrorCode.ReadInstMCUNullErr))
                                {
                                    //表示读取仪器中MCU出错，支持脱机打开软件
                                    IsSoftwareOffline = true;
                                }
                                else
                                {
                                    isShowRegisterSNPage = true;
                                }
                            }
                            else
                            {
                                isShowRegisterSNPage = true;
                            }
                        }
                        if (isShowRegisterSNPage && !_IsSNWindowShow)
                        {
                            RegisterSNPage registerSNPage = new RegisterSNPage()
                            {
                                Owner = Application.Current.MainWindow
                            };
                            _IsSNWindowShow = true;
                            bool? rlt = registerSNPage.ShowDialog();
                            _IsSNWindowShow = false;
                            if (rlt == true)
                            {
                                IsSNRegistered = true;
                                MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });
                            }
                            registerSNPage = null;
                        }
                       
                    }
                }
                if (!IsLoad)
                {
                    IsLoad = true;
                    GlobalInfo.Instance.SettingInfo.PreWashInfos = new ObservableCollection<PreWashItemInfo>
                    {

                        new PreWashItemInfo
                        {
                            StepName = "AutoSampler_Main_PreFlush".GetWord(),
                            IsOpenAction = true,
                            WashLoc="1",
                            WashPumpSpeed = 40,
                            WashTime = 3
                        },
                    };
                    GlobalInfo.Instance.SettingInfo.PreRunningInfo = new ObservableCollection<AnalysInfo>
                    {
                        new AnalysInfo
                        {
                            //WashSpeedTypeIndex = 1,
                            WashPumpSpeed = 40,
                            WashTimeTypeIndex = 1,
                            WashTime = 10
                        },
                        new AnalysInfo
                        {
                            //WashSpeedTypeIndex = 1,
                            WashPumpSpeed = 40,
                            //WashTimeTypeIndex = 1,
                            WashTime = 10
                        },
                    };
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo = new ObservableCollection<ParaItemInfo>
                    { 
                        new ParaItemInfo
                        {
                            StepName = "AutoSampler_Main_PostRun".GetWord(),
                            WashAction="AutoSampler_Main_InjectNeedleFlushSam".GetWord(),
                            WashActionKey=1,
                            WashLoc="1",
                            WashPumpSpeed = 40,
                            WashTime = 3
                        },
                        new ParaItemInfo
                        {
                            WashAction="AutoSampler_Main_InjectNeedleFlushStdSam".GetWord(),
                            WashActionKey=2,
                            WashLoc="1",
                            WashPumpSpeed = 40,
                            WashTime = 3
                        },
                    };
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [Page_Loaded]", ex);
                new MessagePage().ShowDialog("Message_Error1000".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        public void SampleListCommand(object sender, RoutedEventArgs e)
        {
            this.Content_ActiveItem.Content = Control_SampleListView;
        }

        public void SettingCommand(object sender, RoutedEventArgs e)
        {
            this.Content_ActiveItem.Content = Control_SettingView;
        }

        public void AdvancedSetCommand(object sender, RoutedEventArgs e)
        {
            Control_SamplerPosSetView = new SamplerPosSetPage()
            {
                Owner = Application.Current.MainWindow
            };
            //Control_SamplerPosSetView.InitData();
            Control_SamplerPosSetView.ShowDialog();
        }

        private void Btn_About_Click(object sender, RoutedEventArgs e)
        {
            AboutPage aboutPage = new AboutPage()
            {
                Owner = Application.Current.MainWindow
            };
            aboutPage.InitData(this);
            aboutPage.ShowDialog();
        }

        public void ConnectionCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!GlobalInfo.Instance.Totalab_LSerials.IsConnect)
                {
                    Connect();
                    if (GlobalInfo.Instance.Totalab_LSerials.IsConnect)
                    {
                        GlobalInfo.Instance.Totalab_LSerials.Init();
                        if (IsSoftwareOffline || !IsSNRegistered)
                        {
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            Task.Factory.StartNew(() =>
                            {
                                SNRegistered();
                                cancellationTokenSource?.Cancel();
                            }, cancellationTokenSource.Token);
                        }
                        GlobalInfo.Instance.IsBusy = false;
                    }
                    if (GlobalInfo.Instance.IsHimassConnState)
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                    MainLogHelper.Instance.Info($"[ ConnectionCommand IsConnect ={IsConnect}");
                }
                //else
                //{
                //    GlobalInfo.Instance.Totalab_LSerials.MsgCome -= Sampler_MsgCome;
                //    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                //    IsConnect = false;
                //    _IsTestConnection = false;
                //    //MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                //}
                //if(GlobalInfo.Instance.IsHimassConnState)
                //    MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ConnectionCommand]", ex);
            }

        }

  
        private void TrayEItemClickCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton radioButton && radioButton != null)
                {
                    ItemData item = radioButton.DataContext as ItemData;
                    CurrentPosition = int.Parse(item.ItemContent);
                    SelectedTrayName = Enum_TrayName.TrayE;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ TrayEItemClickCommand]", ex);
            }
        }

        private void TrayDItemClickCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton radioButton && radioButton != null)
                {
                    ItemData item = radioButton.DataContext as ItemData;
                    CurrentPosition = int.Parse(item.ItemContent);
                    SelectedTrayName = Enum_TrayName.TrayD;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ TrayDItemClickCommand]", ex);
            }
        }

        private void TrayCItemClickCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton radioButton && radioButton != null)
                {
                    ItemData item = radioButton.DataContext as ItemData;
                    CurrentPosition = int.Parse(item.ItemContent);
                    SelectedTrayName = Enum_TrayName.TrayC;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ TrayCItemClickCommand]", ex);
            }
        }

        private void TrayBItemClickCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton radioButton && radioButton != null)
                {
                    ItemData item = radioButton.DataContext as ItemData;
                    CurrentPosition = int.Parse(item.ItemContent);
                    SelectedTrayName = Enum_TrayName.TrayB;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ TrayBItemClickCommand]", ex);
            }
        }

        private void TrayAItemClickCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton radioButton && radioButton != null)
                {
                    ItemData item = radioButton.DataContext as ItemData;
                    CurrentPosition = int.Parse(item.ItemContent);
                    SelectedTrayName = Enum_TrayName.TrayA;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ TrayAItemClickCommand]", ex);
            }
        }

        public void MoveToPositionCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentPosition == 1)
                {
                    //GlobalInfo.Instance.Totalab_LSerials.MoveToXY(GlobalInfo.Instance.SamplerPos.Wash1X, 0);
                    IsSamplerManual = true;
                    GlobalInfo.Instance.IsBusy = true;
                }
                else if (CurrentPosition == 2)
                {
                    //GlobalInfo.Instance.Totalab_LSerials.MoveToXY(GlobalInfo.Instance.SamplerPos.Wash2X, 0);
                    IsSamplerManual = true;
                    GlobalInfo.Instance.IsBusy = true;
                }
                else if (CurrentPosition == 3)
                {
                    //GlobalInfo.Instance.Totalab_LSerials.MoveToXY(GlobalInfo.Instance.SamplerPos.Wash3X, 0);
                    IsSamplerManual = true;
                    GlobalInfo.Instance.IsBusy = true;
                }
                else if (CurrentPosition == 0)
                {
                    //GlobalInfo.Instance.Totalab_LSerials.MoveToXY(GlobalInfo.Instance.SamplerPos.HomeX, 0);
                    IsSamplerManual = true;
                    GlobalInfo.Instance.IsBusy = true;
                }
                else
                {
                }
            }
            catch (Exception ex) { MainLogHelper.Instance.Error("ShellPage [MoveToPositionCommand]", ex); }
        }

        public void MoveToHomeCommand(object sender, RoutedEventArgs e)
        {
            IsSamplerManual = true;
            GlobalInfo.Instance.IsBusy = true;
            //GlobalInfo.Instance.Totalab_LSerials.XYZHome();
        }

        public void MoveToZCommand(object sender, RoutedEventArgs e)
        {
            IsSamplerManual = true;
            GlobalInfo.Instance.IsBusy = true;
            //GlobalInfo.Instance.Totalab_LSerials.MoveToXYZAlone(0x02, GlobalInfo.Instance.SettingInfo.SamplingDepth);
        }

        public void UseAutoSamplerCommand(object sender, RoutedEventArgs e)
        {
            if (GlobalInfo.Instance.IsHimassConnState && IsUseAutoSampler)
            {
                GlobalInfo.Instance.SampleInfos = new ObservableCollection<SampleItemInfo>();
                SampleHelper.CreateSampleInfos(25);
            }
            MainWindow_AutoSamplerSendObjectDataEvent(null,
                          new ObjectEventArgs() { MessParamType = EnumMessParamType.UseAutoSampler, Parameter = IsUseAutoSampler });
        }

        public void SaveMethodCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.OnUIThread(() =>
                {
                    if (string.IsNullOrEmpty(GlobalInfo.Instance.CurrentMethod.MethodName))
                    {
                        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                        dlg.DefaultExt = ".mth";
                        dlg.Filter = "MTH Files (*.mth)|*.mth";
                        string path = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "Method");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        dlg.InitialDirectory = path;
                        //dlg.InitialDirectory = GlobalInfo.Instance.UserMethodPath;
                        Nullable<bool> result = dlg.ShowDialog();
                        if (result == true)
                        {
                            GlobalInfo.Instance.CurrentMethod.MethodName = dlg.FileName;
                            GlobalInfo.Instance.CurrentMethod.SampleInfos =
                            GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).ToList();
                            GlobalInfo.Instance.CurrentMethod.MethodSettingInfo = GlobalInfo.Instance.SettingInfo;
                            byte[] content = XmlObjSerializer.Serialize(GlobalInfo.Instance.CurrentMethod);
                            bool isSuccessful = FileHelper.WriteEncrypt(dlg.FileName, content);
                            if (isSuccessful == true)
                                new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                            else
                                new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                            //DataToXml.SaveToXml(dlg.FileName, GlobalInfo.Instance.CurrentMethod);
                            //new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                        }
                    }
                    else
                    {
                        GlobalInfo.Instance.CurrentMethod.SampleInfos =
                            GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).ToList();
                        GlobalInfo.Instance.CurrentMethod.MethodSettingInfo = GlobalInfo.Instance.SettingInfo;
                        byte[] content = XmlObjSerializer.Serialize(GlobalInfo.Instance.CurrentMethod);
                        bool isSuccessful = FileHelper.WriteEncrypt(GlobalInfo.Instance.CurrentMethod.MethodName, content);
                        if (isSuccessful == true)
                            new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                        else
                            new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    }
                  
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [SaveMethodCommand]", ex);
                new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        public void CreateMethodCommand(object sender, RoutedEventArgs e)
        {
            try
            {

                if (GlobalInfo.Instance.IsHimassConnState)
                {
                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    int count = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Enum.Exp_Status.Free).Count();
                    for (int i = 0; i < count; i++)
                    {
                        AutoSampler_SamInfo info = new AutoSampler_SamInfo
                        {
                            SamID = GlobalInfo.Instance.SampleInfos[i].SampleGuid,
                            SamName = GlobalInfo.Instance.SampleInfos[i].SampleName,
                            //Location = GlobalInfo.Instance.SampleInfos[i].SampleLoc.Value,
                            IsAnalyze = GlobalInfo.Instance.SampleInfos[i].IsChecked,
                            AnalysisType = GlobalInfo.Instance.SampleInfos[i].MethodType.Value,
                            OverWash = GlobalInfo.Instance.SampleInfos[i].Overwash == null ? 0 : GlobalInfo.Instance.SampleInfos[i].Overwash.Value,
                            OperationMode = EnumSamOperationMode.Delete
                        };
                        list.Add(info);
                    }
                    if (list.Count > 0)
                    {
                        MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                }
                GlobalInfo.Instance.SampleInfos = new ObservableCollection<SampleItemInfo>();
                SampleHelper.CreateSampleInfos(25);
                CurrentRotationIndex = 0;
                GlobalInfo.Instance.SettingInfo = new SettingInfo();
                SampleHelper.RefreshSamplerItemStatus(Exp_Status.Free);
       
                GlobalInfo.Instance.SettingInfo.PreWashInfos = new ObservableCollection<PreWashItemInfo>
                {
                    new PreWashItemInfo
                    {
                        StepName = "AutoSampler_Main_PreFlush".GetWord(),
                        IsOpenAction = true,
                        WashLoc="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
                };
                GlobalInfo.Instance.SettingInfo.PreRunningInfo = new ObservableCollection<AnalysInfo>
                {
                    new AnalysInfo
                    {
                        WashPumpSpeed = 40,
                        WashTimeTypeIndex = 1,
                        WashTime = 10
                    },
                    new AnalysInfo
                    {
                        //WashSpeedTypeIndex = 1,
                        WashPumpSpeed = 40,
                        //WashTimeTypeIndex = 1,
                        WashTime = 10
                    },
                };
                GlobalInfo.Instance.SettingInfo.AfterRunningInfo = new ObservableCollection<ParaItemInfo>
                {
                    new ParaItemInfo
                    {
                        StepName = "AutoSampler_Main_PostRun".GetWord(),
                        WashAction="AutoSampler_Main_InjectNeedleFlushSam".GetWord(),
                        WashActionKey=1,
                        WashLoc="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
                    new ParaItemInfo
                    {
                        WashAction="AutoSampler_Main_InjectNeedleFlushStdSam".GetWord(),
                        WashActionKey=2,
                        WashLoc ="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
                };
                GlobalInfo.Instance.CurrentMethod.MethodName = "";
                MainLogHelper.Instance.Info("End3" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [CreateMethodCommand]", ex);
                new MessagePage().ShowDialog("Message_Error1000".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        public void OpenMethodCommand(object sender, RoutedEventArgs e)
        {
            Control_MethodSelectorView = new MethodSelectorPage();
            Control_MethodSelectorView.IsSettingsOpen = false;
            Control_MethodSelectorView.ShowDialog(this);
        }
        private void SaveAsMethodCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".mth";
                dlg.Filter = "MTH Files (*.mth)|*.mth";
                string path = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "Method");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                dlg.InitialDirectory = path;
                //dlg.InitialDirectory = GlobalInfo.Instance.UserMethodPath;
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    GlobalInfo.Instance.CurrentMethod.MethodName = dlg.FileName;
                    GlobalInfo.Instance.CurrentMethod.SampleInfos =
                    GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).ToList();
                    GlobalInfo.Instance.CurrentMethod.MethodSettingInfo = GlobalInfo.Instance.SettingInfo;
                    byte[] content = XmlObjSerializer.Serialize(GlobalInfo.Instance.CurrentMethod);
                    bool isSuccessful = FileHelper.WriteEncrypt(dlg.FileName, content);
                    if (result == true)
                        new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                    else
                        new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    //DataToXml.SaveToXml(dlg.FileName, GlobalInfo.Instance.CurrentMethod);
                    //new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [SaveAsMethodCommand]", ex);
                new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        private void RackTypeChangedCommand(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (GlobalInfo.Instance.IsHimassConnState)
                    MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerTrayFile, Parameter = RackType });
                if (SelectedTrayName == Enum_TrayName.TrayA)
                {
                    TrayMaxValue = GlobalInfo.Instance.TrayAInfos.XCount * GlobalInfo.Instance.TrayAInfos.YCount;
                    GlobalInfo.Instance.TrayAInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
                }
                else if (SelectedTrayName == Enum_TrayName.TrayB)
                {
                    TrayMaxValue = GlobalInfo.Instance.TrayBInfos.XCount * GlobalInfo.Instance.TrayBInfos.YCount;
                    GlobalInfo.Instance.TrayBInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
                }
                else if (SelectedTrayName == Enum_TrayName.TrayC)
                {
                    TrayMaxValue = GlobalInfo.Instance.TraySTD1Infos.XCount * GlobalInfo.Instance.TraySTD1Infos.YCount
                        + GlobalInfo.Instance.TraySTD2Infos.XCount * GlobalInfo.Instance.TraySTD2Infos.YCount;
                    if (CurrentPosition <= GlobalInfo.Instance.TraySTD1Infos.XCount * GlobalInfo.Instance.TraySTD1Infos.YCount)
                    {
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
                    }
                    else
                    {
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemList[CurrentPosition - 1 - GlobalInfo.Instance.TraySTD1Infos.XCount * GlobalInfo.Instance.TraySTD1Infos.YCount].IsItemSelected = true;
                    }
                }
                else if (SelectedTrayName == Enum_TrayName.TrayD)
                {
                    TrayMaxValue = GlobalInfo.Instance.TrayDInfos.XCount * GlobalInfo.Instance.TrayDInfos.YCount;
                    GlobalInfo.Instance.TrayDInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
                }
                else if (SelectedTrayName == Enum_TrayName.TrayE)
                {
                    TrayMaxValue = GlobalInfo.Instance.TrayEInfos.XCount * GlobalInfo.Instance.TrayEInfos.YCount;
                    GlobalInfo.Instance.TrayEInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
                }
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [RackTypeChangedCommand]", ex);
            }
        }

        private void CurrentPositionLostFocusCommand(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (SelectedTrayName == Enum_TrayName.TrayA)
            {
                GlobalInfo.Instance.TrayAInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
            }
            else if (SelectedTrayName == Enum_TrayName.TrayB)
            {
                GlobalInfo.Instance.TrayBInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
            }
            else if (SelectedTrayName == Enum_TrayName.TrayC)
            {
                if (CurrentPosition <= GlobalInfo.Instance.TraySTD1Infos.XCount * GlobalInfo.Instance.TraySTD2Infos.YCount)
                    GlobalInfo.Instance.TraySTD1Infos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
                else
                    GlobalInfo.Instance.TraySTD2Infos.TrayItemList[CurrentPosition - GlobalInfo.Instance.TraySTD1Infos.XCount * GlobalInfo.Instance.TraySTD2Infos.YCount - 1].IsItemSelected = true;
            }
            else if (SelectedTrayName == Enum_TrayName.TrayD)
            {
                GlobalInfo.Instance.TrayDInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
            }
            else if (SelectedTrayName == Enum_TrayName.TrayE)
            {
                GlobalInfo.Instance.TrayEInfos.TrayItemList[CurrentPosition - 1].IsItemSelected = true;
            }
        }

        private  void TrayTypeChangedCommand(object sender,RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            Enum_TrayType type = EnumHelper.GetEnumValue<Enum_TrayType>(item.Header.ToString());
            if(ButtonContent == "Tray A")
               GlobalInfo.Instance.TrayAInfos = TrayInfoHelper.GetTrayInfo(type);
            if (ButtonContent == "Tray B")
                GlobalInfo.Instance.TrayBInfos = TrayInfoHelper.GetTrayInfo(type);
            if (ButtonContent == "Tray D")
                GlobalInfo.Instance.TrayDInfos = TrayInfoHelper.GetTrayInfo(type);
            if (ButtonContent == "Tray E")
                GlobalInfo.Instance.TrayEInfos = TrayInfoHelper.GetTrayInfo(type);
        }


        private void StdTrayTypeChangedCommand(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            Enum_StdTrayType type = EnumHelper.GetEnumValue<Enum_StdTrayType>(item.Header.ToString());
            TrayInfoHelper.GetStdTrayInfo(type);
        }
        private void TrayTypeBtn_ClickCommand(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ContextMenu cmnu = new ContextMenu();
            ButtonContent = btn.Content.ToString();
            TrayTypeMenu.PlacementTarget = btn;
            //位置
            TrayTypeMenu.Placement = PlacementMode.Bottom;
            //显示菜单
            TrayTypeMenu.IsOpen = true;
        }
        private void StdTrayTypeBtn_ClickCommand(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ContextMenu cmnu = new ContextMenu();
            ButtonContent = btn.Content.ToString();
            StdTrayTypeMenu.PlacementTarget = btn;
            //位置
            StdTrayTypeMenu.Placement = PlacementMode.Bottom;
            //显示菜单
            StdTrayTypeMenu.IsOpen = true;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 脱机状态下使用该方法
        /// </summary>
        private void SNRegistered()
        {
            IsSoftwareOffline = false;
            Thread.Sleep(20000);
            //序列号授权验证
            RegistSN registSN = RegistSN.GetInstance("LabMonsterTotalab-I");
            registSN.Init(new InstrumentSerialNum(), "LabMonster Totalab-I");
            ResultData resultData = registSN.Register();
            if (resultData != null && resultData.IsSuccessful
                             && (resultData.ErrorCodeList == null || (resultData.ErrorCodeList != null && resultData.ErrorCodeList.Count == 0)))
            {
                IsSNRegistered = true;
                MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });
            }
            else
            {
                MainLogHelper.Instance.Error("ShellPage [SNRegistered]" + resultData != null ? resultData.Result : string.Empty);
                bool isShowRegisterSNPage = false;
                if (resultData != null && resultData.IsOnlySoft == true)
                {
                    isShowRegisterSNPage = true;
                }
                else
                {
                    if (registSN.IsExistsAlisql())
                    {
                        if (resultData != null && resultData.ErrorCodeList != null && resultData.ErrorCodeList.Count == 1
                            && (resultData.ErrorCodeList[0] == UtilityLibrary.Enums.Enum_RegisterErrorCode.ReadInstMCUErr
                            || resultData.ErrorCodeList[0] == UtilityLibrary.Enums.Enum_RegisterErrorCode.ReadInstMCUNullErr))
                        {
                            //表示读取仪器中MCU出错，支持脱机打开软件
                            IsSoftwareOffline = true;
                        }
                        else
                        {
                            isShowRegisterSNPage = true;
                        }
                    }
                    else
                    {
                        isShowRegisterSNPage = true;
                    }
                }
               
                if (isShowRegisterSNPage && !_IsSNWindowShow && this.IsVisible == true)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        RegisterSNPage registerSNPage = new RegisterSNPage()
                        {
                            Owner = Application.Current.MainWindow
                        };
                        _IsSNWindowShow = true;
                        bool? rlt = registerSNPage.ShowDialog();
                        _IsSNWindowShow = false;
                        if (rlt == true)
                        {
                            IsSNRegistered = true;
                            MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });
                        }
                        registerSNPage = null;
                    }));
                }
            }
        }
        private void InitData()
        {
            try
            {
                SampleListCommand(null, null);
                TrayTypeList = EnumDataSource.FromType<Enum_TrayType>();
                TrayNameList = EnumDataSource.FromType<Enum_TrayName>();
                StdTrayTypeList = EnumDataSource.FromType<Enum_StdTrayType>();
                List<EnumMemberModel> trayTypeList = TrayTypeList.ToList();
                List<EnumMemberModel> stdTrayTypeList = StdTrayTypeList.ToList();
                TrayTypeMenu.Style = Application.Current.Resources["ContextMenuSty"] as Style;
                StdTrayTypeMenu.Style = Application.Current.Resources["ContextMenuSty"] as Style;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    for (int i = 0; i < TrayTypeList.Count(); i++)
                    {
                        MenuItem item = new MenuItem();
                        item.Style = Application.Current.Resources["ContextMenuItemSty"] as Style;
                        item.Header = trayTypeList[i].ToString();
                        item.Click += TrayTypeChangedCommand;
                        //TrayTypeButtonList.Add(item);
                        TrayTypeMenu.Items.Add(item);
                    }
                    for (int i = 0; i < StdTrayTypeList.Count(); i++)
                    {
                        MenuItem item = new MenuItem();
                        item.Style = Application.Current.Resources["ContextMenuItemSty"] as Style;
                        item.Header = stdTrayTypeList[i].ToString();
                        item.Click += StdTrayTypeChangedCommand;
                        //TrayTypeButtonList.Add(item);
                        StdTrayTypeMenu.Items.Add(item);
                    }
                    GlobalInfo.Instance.TrayAInfos = TrayInfoHelper.GetTrayInfo(Enum_TrayType._15mL);
                    GlobalInfo.Instance.TrayBInfos = TrayInfoHelper.GetTrayInfo(Enum_TrayType._60mL);
                    //GlobalInfo.Instance.TrayCleanInfos = TrayInfoHelper.GetTrayInfo(null);
                    GlobalInfo.Instance.TrayDInfos = TrayInfoHelper.GetTrayInfo(Enum_TrayType._15mL);
                    GlobalInfo.Instance.TrayEInfos = TrayInfoHelper.GetTrayInfo(Enum_TrayType._15mL);
                    GlobalInfo.Instance.TrayCleanInfos.ItemSize = new Size(20, 20);
                    for (int i = 1; i < 3; i++)
                    {
                        ItemData item = new ItemData
                        {
                            ItemContent = "CL" + i.ToString()
                        };
                        GlobalInfo.Instance.TrayCleanInfos.TrayItemList.Add(item);
                    }
                    TrayInfoHelper.GetStdTrayInfo(Enum_StdTrayType._250mL);
                }));
                GlobalInfo.Instance.SettingInfo.PreWashInfos = new ObservableCollection<PreWashItemInfo>
                {

                    new PreWashItemInfo
                    {
                        StepName = "AutoSampler_Main_PreFlush".GetWord(),
                        IsOpenAction = true,
                        WashLoc="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
                };
                GlobalInfo.Instance.SettingInfo.PreRunningInfo = new ObservableCollection<AnalysInfo>
                {
                    new AnalysInfo
                    {
                        //WashSpeedTypeIndex = 1,
                        WashPumpSpeed = 40,
                        WashTimeTypeIndex = 1,
                        WashTime = 10
                    },
                    new AnalysInfo
                    {
                        //WashSpeedTypeIndex = 1,
                        WashPumpSpeed = 40,
                        //WashTimeTypeIndex = 1,
                        WashTime = 10
                    },
                };
                GlobalInfo.Instance.SettingInfo.AfterRunningInfo = new ObservableCollection<ParaItemInfo>
                {
                    new ParaItemInfo
                    {
                        StepName = "AutoSampler_Main_PostRun".GetWord(),
                        WashAction="AutoSampler_Main_InjectNeedleFlushSam".GetWord(),
                        WashActionKey=1,
                        WashLoc="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
                    new ParaItemInfo
                    {
                        WashAction="AutoSampler_Main_InjectNeedleFlushStdSam".GetWord(),
                        WashActionKey=2,
                        WashLoc="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
                };
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [InitData]", ex);
            }
        }

        public void Connect()
        {
            try
            {
                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                {
                    try
                    {
                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                        GlobalInfo.Instance.Totalab_LSerials.Connect();
                        Thread.Sleep(1000);
                        if (IsConnect)
                        {
                            _IsTestConnection = true;
                            if (ConnectionTestThead == null)
                            {
                                ConnectionTestThead = new Thread(ConnectionTest);
                                ConnectionTestThead.Start();
                            }
                            break;
                        }
                        else
                        {
                            //GlobalInfo.Instance.Totalab_LSerials.MsgCome -= Sampler_MsgCome;
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            IsConnect = false;
                            //MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("ShellViewModel [Connect]：", ex);
                    }
                }

            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellViewModel [Connect]：", ex);
            }
            finally
            {
                if (GlobalInfo.Instance.IsHimassConnState)
                {
                    MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                    MainLogHelper.Instance.Info($"[ Connect IsConnect ={IsConnect}");
                }
                byte[] content = FileHelper.ReadDecrypt(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.dat"));
                if (content != null)
                {
                    GlobalInfo.Instance.SamplerPos = XmlObjSerializer.Deserialize<SamperPosInfo>(content);
                }
                else
                {
                    GlobalInfo.Instance.SamplerPos = new SamperPosInfo();
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        new MessagePage().ShowDialog("进样器位置参数调用错误，请重新设置", "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                    }));
                }
                //GlobalInfo.Instance.SamplerPos = (SamperPosInfo)DataToXml.LoadFromXml(System.IO.Path.Combine(CommonSampleMethod.AssemblyDirectory, "Parameters", "SamplerPos.dat"), typeof(SamperPosInfo));
                //_IsTestConnection = true;
                //if (ConnectionTestThead == null)
                //{
                //    ConnectionTestThead = new Thread(ConnectionTest);
                //    ConnectionTestThead.Start();
                //}
            }
        }

        public void ConnectionTest()
        {
            int count = 0;
            while (true)
            {
                if (_IsTestConnection)
                {
                    Thread.Sleep(10000);
                    if (_IsRecived)
                    {
                        GlobalInfo.Instance.Totalab_LSerials.Connect();
                        count = 0;
                        _IsRecived = false;
                    }
                    else
                    {
                        if (count > GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            try
                            {
                                IsConnect = false;
                                GlobalInfo.Instance.IsCanRunning = true;
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                _IsTestConnection = false;
                                if (GlobalInfo.Instance.IsHimassConnState)
                                {
                                    MainWindow_AutoSamplerSendObjectDataEvent(null,
                                                   new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                                }
                                else
                                {
                                    this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        new MessagePage().ShowDialog("Message_Error2013".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                                    }));
                                }
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex.ToString());
                            }
                        }
                        else
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
                                        GlobalInfo.Instance.Totalab_LSerials.Connect();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("ShellViewModel [ConnectionTest]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("ShellViewModel [ConnectionTest]：", ex);
                            }
                          
                            //GlobalInfo.Instance.Totalab_LSerials.Connect();
                            count++;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void Sampler_MsgCome(object sender, Serials.SamplerSerials.MsgComeEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.Totalab_LSerials.WaitSendSuccess.Set();
                _IsRecived = true;
                if (e == null)
                    return;
                switch (e.Msg.FunctionCode)
                {
                    case 0x55:
                        if (e.Msg.Cmd == 0xA1)
                        {
                            //this.Dispatcher.Invoke((Action)(() =>
                            //{
                                IsConnect = true;
                            //}));
                            //_IsRecived = true;
                        }
                        break;
                    case 0xAA:
                        if (e.Msg.Cmd == 0xA1)
                        {
                            GlobalInfo.Instance.IsReadMCUOk = true;
                            GlobalInfo.Instance.MCUData = new byte[30];
                            GlobalInfo.Instance.MCUData = e.Msg.Data;
                        }
                        if (e.Msg.Cmd == 0xA2)
                            GlobalInfo.Instance.IsWriteMCUOk = true;
                        break;
                    case 0x85:
                        if (e.Msg.Cmd == 0x50)//////////////////////正确的校验
                        {
                            GlobalInfo.Instance.Totalab_LSerials.respondTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            GlobalInfo.Instance.Totalab_LSerials.WaitSendSuccess.Set();
                            Thread.Sleep(50);
                            break;
                        }
                        else if (e.Msg.Cmd == 0x51)/////////////////////////////错误校验
                        {
                            GlobalInfo.Instance.Totalab_LSerials.respondTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            if (GlobalInfo.Instance.Totalab_LSerials.SendTempMsg != null)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.Send(GlobalInfo.Instance.Totalab_LSerials.SendTempMsg.GetFrame());
                                GlobalInfo.Instance.Totalab_LSerials.respondTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            }
                            Thread.Sleep(50);
                            break;
                        }
                        else if (e.Msg.Cmd == 0x56)/////////////////////////////下位机接收错误的数据格式
                        {
                            GlobalInfo.Instance.Totalab_LSerials.respondTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            //if (GlobalInfo.Instance.Totalab_LSerials.SendTempMsg != null)
                            //{
                            //    Thread.Sleep(50);
                            //    GlobalInfo.Instance.Totalab_LSerials.Send(GlobalInfo.Instance.Totalab_LSerials.SendTempMsg.GetFrame());
                            //    GlobalInfo.Instance.Totalab_LSerials.respondTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            //}
                            Thread.Sleep(50);
                            break;
                        }
                        break;
                    case 0x81:
                        if (e.Msg.Cmd == 0x99)
                        {

                        }
                        else if (e.Msg.Cmd == 0x9A)
                        {

                        }
                        else if (e.Msg.Cmd == 0x9B)
                        {
                            if(IsSamplerManual)
                            {
                                GlobalInfo.Instance.IsBusy = false;
                                IsSamplerManual = false;
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.ZHome)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.ZHomeOk;
                        }
                        else if (e.Msg.Cmd == 0x3d)
                        {
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.ClosePump)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePumpOk;
                        }
                        else if (e.Msg.Cmd == 0x09)
                        {
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.OpenPump)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPumpOk;
                        }
                        else if (e.Msg.Cmd == 0x17)
                        {
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetPumpSpeed)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeedOk;
                        }
                        break;
                    case 0x61:
                        if (e.Msg.Cmd == 0x60)
                        {
                            if (IsSamplerManual)
                            {
                                GlobalInfo.Instance.IsBusy = false;
                                IsSamplerManual = false;
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.XYZHome)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHomeOk;
                        }
                        else if (e.Msg.Cmd == 0x68)
                        {
                            if (IsSamplerManual)
                            {
                                GlobalInfo.Instance.IsBusy = false;
                                IsSamplerManual = false;
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleXY)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleXYOk;
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleDepth)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleDepthOk;
                        }
                        else if (e.Msg.Cmd == 0x53)
                        {
                            Thread.Sleep(500);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SendTrigger)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SendTriggerOk;
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.CloseTrigger)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.CloseTriggerOk;
                        }
                        else if (e.Msg.Cmd == 0x79)
                        {
                            if (IsSamplerManual)
                            {
                                GlobalInfo.Instance.IsBusy = false;
                                IsSamplerManual = false;
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleXY)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleXYOk;
                        }
                        else if (e.Msg.Cmd == 0x54 && e.Msg.Data[0] == 0x01)
                        {
                            Thread.Sleep(500);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.RecvTrigger)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.RecvTriggerOk;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MainWindow [Sampler_MsgCome]：", ex);
            }
        }

        #region AutoSamplerSendSamDataEvent
        [EventPublication("topic://AutoSampler_AutoSamplerSendSamData")]
        public event EventHandler<AutoSamplerEventArgs> AutoSamplerSendSamDataEvent;

        /// <summary>
        /// 由自动进样器给调用者发送同步数据命令的带参消息
        /// </summary>
        /// <param name="msgArg">消息参数</param>
        public void PublisherAutoSamplerSendSamDataEvent(AutoSamplerEventArgs msgArg)
        {
            AutoSamplerSendSamDataEvent?.Invoke(this, msgArg);
            try
            {
                if (msgArg != null && msgArg.SamInfoList != null)
                {
                    for (int i = 0; i < msgArg.SamInfoList.Count; i++)
                    {
                        AutoSampler_SamInfo samInfo = msgArg.SamInfoList[i];
                        //MainLogHelper.Instance.Info(samInfo.SamName + "[" + samInfo.SamID + "]" + samInfo.Location + samInfo.OperationMode + samInfo.IsAnalyze + samInfo.AnalysisType+ samInfo.NextSamID);
                    }
                }
                
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("AutoSamplerModule [SubscribeAutoSamSendSyncDataEvent]：", e);
            }
        }

        public void MainWindow_AutoSamplerSendSamDataEvent(object sender, AutoSamplerEventArgs e)
        {
            if (IsUseAutoSampler)
            {
                PublisherAutoSamplerSendSamDataEvent(e);
            }
        }
        #endregion

        #region AutoSamplerSendObjectDataEvent
        [EventPublication("topic://AutoSampler_AutoSamplerSendObjectData")]
        public event EventHandler<ObjectEventArgs> AutoSamplerSendObjectDataEvent;

        /// <summary>
        /// 由自动进样器给调用者发送是否使用自动进样器的带参消息
        /// </summary>
        /// <param name="msgArg">消息参数</param>
        public void PublisherAutoSamplerSendObjectDataEvent(ObjectEventArgs msgArg)
        {
            AutoSamplerSendObjectDataEvent?.Invoke(this, msgArg);
            if (msgArg.MessParamType == EnumMessParamType.ASSerialPortConnOpen)
                MainLogHelper.Instance.Info($"[PublisherAutoSamplerSendObjectDataEvent IsConnect ={IsConnect}");
            if(msgArg.MessParamType == EnumMessParamType.UseAutoSampler)
            {
                MainLogHelper.Instance.Info($"[PublisherAutoSamplerSendObjectDataEvent IsUseAutoSampler ={IsUseAutoSampler}");
            }
        }

        public void MainWindow_AutoSamplerSendObjectDataEvent(object sender, ObjectEventArgs e)
        {
            PublisherAutoSamplerSendObjectDataEvent(e);
        }
        #endregion

        [EventSubscription("topic://AutoSampler_MassSendSamData", typeof(Publisher))]
        public void SubscribeMassSendSamDataEvent(object sender, AutoSamplerEventArgs msgArg)
        {
            try
            {
                if (msgArg != null && msgArg.SamInfoList != null)
                {
                    for (int i = 0; i < msgArg.SamInfoList.Count; i++)
                    {
                        AutoSampler_SamInfo samInfo = msgArg.SamInfoList[i];
                        if (samInfo.OperationMode == EnumSamOperationMode.Add || samInfo.OperationMode == EnumSamOperationMode.Insert)
                        {
                            SampleItemInfo samplingInfo = new SampleItemInfo
                            {
                                SampleGuid = samInfo.SamID,
                                IsChecked = samInfo.IsAnalyze,
                                SampleName = samInfo.SamName,
                                SampleLoc = samInfo.Location.ToString(),
                                MethodType = samInfo.AnalysisType,
                                PreMethodType = samInfo.AnalysisType,
                                Overwash = samInfo.OverWash,
                                ExpStatus = Exp_Status.Ready,
                            };
                            if (GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samplingInfo.SampleGuid).FirstOrDefault() != null)
                                break;
                            int samplingIndex = -1;
                            if (samInfo.OperationMode == EnumSamOperationMode.Add)
                            {
                                SampleItemInfo sampling = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Free).FirstOrDefault();
                                if (sampling != null)
                                {
                                    samplingIndex = GlobalInfo.Instance.SampleInfos.IndexOf(sampling);
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        GlobalInfo.Instance.SampleInfos[samplingIndex] = samplingInfo;
                                    }));
                                }
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "添加样品" + samplingInfo.SampleName);
                                }));
                            }
                            else
                            {
                                SampleItemInfo nextSamplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.NextSamID).FirstOrDefault();
                                if (nextSamplingInfo != null)
                                {
                                    samplingIndex = GlobalInfo.Instance.SampleInfos.IndexOf(nextSamplingInfo);
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        GlobalInfo.Instance.SampleInfos.Insert(samplingIndex, samplingInfo);
                                    }));
                                }
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "插入样品" + samplingInfo.SampleName);
                                }));
                            }
                            if (samplingIndex == -1)
                            {
                                SampleItemInfo sampling = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Ready).LastOrDefault();
                                if (sampling != null)
                                {
                                    samplingIndex = GlobalInfo.Instance.SampleInfos.IndexOf(sampling);
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        GlobalInfo.Instance.SampleInfos.Add(samplingInfo);
                                    }));
                                }
                                else
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        GlobalInfo.Instance.SampleInfos.Add(samplingInfo);
                                    }));
                                }
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "插入样品" + samplingInfo.SampleName);
                                }));
                            }
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                Control_SampleListView.IsSamplingSelectedAll = GlobalInfo.Instance.SampleInfos.Where(item => item.ExpStatus == Exp_Status.Ready).Count(item => item.IsChecked == false) == 0 ? true : false;
                            }));
                            SampleHelper.SetCircleStatus(samplingInfo.SampleLoc, Item_Status.Ready);
                            Control_SampleListView.RefreshSampleNum();
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Delete)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.SampleInfos.Remove(samplingInfo);
                                }));
                            }
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                Control_SampleListView.IsSamplingSelectedAll = GlobalInfo.Instance.SampleInfos.Where(item => item.ExpStatus == Exp_Status.Ready).Count(item => item.IsChecked == false) == 0 ? true : false;
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "删除样品" + samplingInfo.SampleName);
                            }));
                            SampleHelper.SetCircleStatus(samplingInfo.SampleLoc, Item_Status.Free);
                            Control_SampleListView.RefreshSampleNum();
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Update)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo.SampleLoc != samInfo.Location.ToString())
                            {
                                SampleHelper.SetCircleStatus(samInfo.Location.ToString(), Item_Status.Ready);
                                if (GlobalInfo.Instance.SampleInfos.Where(item => item.SampleLoc == samplingInfo.SampleLoc).Count() == 1)
                                {
                                    SampleHelper.SetCircleStatus(samplingInfo.SampleLoc, Item_Status.Free);
                                }
                            }
                            if (samplingInfo != null)
                            {
                                samplingInfo.IsChecked = samInfo.IsAnalyze;
                                samplingInfo.SampleName = samInfo.SamName;
                                samplingInfo.SampleLoc = samInfo.Location.ToString();
                                samplingInfo.Overwash = samInfo.OverWash;
                                samplingInfo.MethodType = samInfo.AnalysisType;
                                samplingInfo.PreMethodType = samInfo.AnalysisType;
                            }
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                Control_SampleListView.IsSamplingSelectedAll = GlobalInfo.Instance.SampleInfos.Where(item => item.ExpStatus == Exp_Status.Ready).Count(item => item.IsChecked == false) == 0 ? true : false;
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "更新样品" + samplingInfo.SampleName);
                            }));

                            //CommonSampleMethod.RefreshSamplerItemStatus(Control_SampleListView.ExpStatus);
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.StartInjection)
                        {

                            Control_SampleListView.ExpStatus = Exp_Status.Running;
                            Control_SampleListView.stopType = 0;
                            GlobalInfo.Instance.IsCanRunning = false;
                            Control_SampleListView.IsMassSamplingFinish = false;
                            Control_SampleListView.IsMassWashFinish = false;
                            Control_SampleListView.IsMassWashStart = false;
                            Control_SampleListView.IsAutoSamplingFinish = false;
                            Control_SampleListView.IsAutoSamplingWashFinish = false;
                            Control_SampleListView.IsStdSample = samInfo.IsStdSample;
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            //if (GlobalInfo.Instance.SettingInfo.AnalysInfo.WashTimeTypeIndex == 0)
                            //    GlobalInfo.Instance.SettingInfo.AnalysInfo.WashTime = samInfo.AnalysisTime / 1000;
                            //if (GlobalInfo.Instance.SettingInfo.AnalysInfo.WashSpeedTypeIndex == 0)
                            //    GlobalInfo.Instance.SettingInfo.AnalysInfo.WashPumpSpeed = (int)samInfo.AnalysisSpeed;
                            if (samplingInfo != null)
                            {
                                if (SampleHelper.CheckSampleLocationLegal(samplingInfo.SampleLoc))
                                {

                                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                                    AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                    {
                                        SamID = samplingInfo.SampleGuid,
                                        SamName = samplingInfo.SampleName,
                                        //Location = samplingInfo.SampleLoc.Value,
                                        IsAnalyze = samplingInfo.IsChecked,
                                        AnalysisType = samplingInfo.MethodType.Value,
                                        OverWash = samplingInfo.Overwash == null ? 0 : samplingInfo.Overwash.Value,
                                        OperationMode = EnumSamOperationMode.Error
                                    };
                                    list.Add(info);
                                    if (list.Count > 0)
                                    {
                                        MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                        {
                                            SamInfoList = list
                                        });
                                    }
                                }
                                else
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "开始进样" + samplingInfo.SampleName);
                                    }));
                                    GlobalInfo.Instance.IsBusy = true;
                                    Control_SampleListView.samplingIndex = GlobalInfo.Instance.SampleInfos.IndexOf(samplingInfo);
                                    Control_SampleListView.OperationMode = EnumSamOperationMode.StartInjection;
                                }
                            }
                            else
                            {
                                //this.Dispatcher.Invoke((Action)(() =>
                                //{
                                //    GlobalInfo.Instance.SampleInfos[GlobalInfo.Instance.SampleInfos.IndexOf(samplingInfo)].ExpStatus = Exp_Status.Error;
                                //}));
                                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = samplingInfo.SampleGuid,
                                    SamName = samplingInfo.SampleName,
                                    //Location = samplingInfo.SampleLoc.Value,
                                    IsAnalyze = samplingInfo.IsChecked,
                                    AnalysisType = samplingInfo.MethodType.Value,
                                    OverWash = samplingInfo.Overwash == null ? 0 : samplingInfo.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Error
                                };
                                list.Add(info);
                                if (list.Count > 0)
                                {
                                    MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                    {
                                        SamInfoList = list
                                    });
                                }
                            }

                            ////没有找到guid发送错误
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Ready)
                        {
                            //Control_SampleListView.IsMassSamplingFinish = true;
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Ready;
                                    //GlobalInfo.Instance.SampleInfos[GlobalInfo.Instance.SampleInfos.IndexOf(samplingInfo)].ExpStatus = Exp_Status.Ready;
                                }));
                                SampleHelper.SetCircleStatus(samplingInfo.SampleLoc, Item_Status.Ready);
                            }
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.InjectionComplete)////////Mass进样结束
                        {
                            Control_SampleListView.IsMassSamplingFinish = true;
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Mass进样结束");
                            }));
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.StartWash)
                        {
                            Control_SampleListView.IsMassWashStart = true;
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Mass开始清洗");
                            }));
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.WashComplete)////////Mass清洗结束
                        {
                            Control_SampleListView.IsMassWashFinish = true;
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Mass清洗结束");
                            }));
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.PauseImmediately)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Mass立即暂停");
                            }));
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.StopImmediately)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Mass立即停止");
                            }));
                            Control_SampleListView.IsMassWashStart = true;
                            Control_SampleListView.IsAutoSamplingWashFinish = true;
                            Control_SampleListView.IsMassWashFinish = true;
                            if (IsConnect)
                            {
                                Control_SampleListView.StopImmediatelyCommand(null, null);
                            }
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Standby)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Standby;
                                    //GlobalInfo.Instance.SampleInfos[GlobalInfo.Instance.SampleInfos.IndexOf(samplingInfo)].ExpStatus = Exp_Status.Standby;
                                }));
                            }
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Skip)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Skip;
                                    //GlobalInfo.Instance.SampleInfos[GlobalInfo.Instance.SampleInfos.IndexOf(samplingInfo)].ExpStatus = Exp_Status.Skip;
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "跳过" + samplingInfo.SampleName);
                                }));
                            }
                            else
                            {
                                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = samplingInfo.SampleGuid,
                                    SamName = samplingInfo.SampleName,
                                    //Location = samplingInfo.SampleLoc.Value,
                                    IsAnalyze = samplingInfo.IsChecked,
                                    AnalysisType = samplingInfo.MethodType.Value,
                                    OverWash = samplingInfo.Overwash == null ? 0 : samplingInfo.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Error
                                };
                                list.Add(info);
                                if (list.Count > 0)
                                {
                                    MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                    {
                                        SamInfoList = list
                                    });
                                }
                            }
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Continue)
                        {
                            Control_SampleListView.ExpStatus = Exp_Status.Running;
                        }
                        else if(samInfo.OperationMode == EnumSamOperationMode.Error)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Error;
                                    //GlobalInfo.Instance.SampleInfos[GlobalInfo.Instance.SampleInfos.IndexOf(samplingInfo)].ExpStatus = Exp_Status.Error;
                                }));
                                SampleHelper.SetCircleStatus(samplingInfo.SampleLoc, Item_Status.Error);
                                if (GlobalInfo.Instance.IsBusy)
                                {
                                    Control_SampleListView.IsMassWashStart = true;
                                    Control_SampleListView.IsAutoSamplingWashFinish = true;
                                    Control_SampleListView.IsMassWashFinish = true;
                                    Control_SampleListView.ErrorWash();
                                }
                            }
                        }
                        else if(samInfo.OperationMode == EnumSamOperationMode.Complete)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Complete;
                                }));
                                SampleHelper.SetCircleStatus(samplingInfo.SampleLoc, Item_Status.Complete);

                            }
                        }
                        else if(samInfo.OperationMode == EnumSamOperationMode.Ongoing)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Running;
                                }));
                            }
                        }
                        else if(samInfo.OperationMode == EnumSamOperationMode.Terminated)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Stop;
                                }));
                            }
                        }
                        Trace.WriteLine(string.Format("Name:[{0}] Mode:[{1}] Location:[{2}] Type:[{3}] ID:[{4}]", samInfo.SamName, samInfo.OperationMode, samInfo.Location, samInfo.AnalysisType, samInfo.SamID));
                    }
                }
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("MainWindow [SubscribeMassSendSamDataEvent]：", e);

            }
        }
        [EventSubscription("topic://AutoSampler_MassSendObjectData", typeof(Publisher))]
        public void SubscribeMassSendObjectEvent(object sender, ObjectEventArgs msgArg)
        {
            try
            {
                if (msgArg != null)
                {
                    Trace.WriteLine(string.Format("Auto Sampler ObjectData Type:[{0}]", msgArg.MessParamType));

                    //更新是否使用自动进样器的状态消息
                    if (msgArg.MessParamType == EnumMessParamType.UseAutoSampler && msgArg.Parameter is bool useState)
                    {
                        IsUseAutoSampler = useState;
                        if (IsUseAutoSampler)
                        {
                            //CommonSampleMethod.CreateSampleInfos(500);
                            GlobalInfo.Instance.SampleInfos = new ObservableCollection<SampleItemInfo>();
                            SampleHelper.CreateSampleInfos(25);
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "自动进样器使用");
                            }));
                        }
                        else
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "自动进样器不使用");
                            }));
                        }
                    }
                    else if (msgArg.MessParamType == EnumMessParamType.GetAutoSamplerState)
                    {
                        MainWindow_AutoSamplerSendObjectDataEvent(null,
                            new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerState, Parameter = GlobalInfo.Instance.IsBusy });
                    }
                    else if (msgArg.MessParamType == EnumMessParamType.StartInjectionTask)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            Control_SampleListView.ExpStatus = Exp_Status.Running;
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "开始进样");
                        }));
                        Control_SampleListView.stopType = 0;
                        _IsTestConnection = false;
                        Control_SampleListView.CurrnentLoc = "";
                        List<InjectionTaskParam> list = new List<InjectionTaskParam>();
                        InjectionTaskParam injectionTaskParam = new InjectionTaskParam
                        {
                            Step = Enum_ParamStep.PreWash,
                            IsAction = GlobalInfo.Instance.SettingInfo.PreWashInfos[0].IsOpenAction,
                            StepInfoList = new List<ParamStepInfo>()
                        };
                        for (int i = 0; i < GlobalInfo.Instance.SettingInfo.PreWashInfos.Count; i++)
                        {
                            injectionTaskParam.StepInfoList.Add(new ParamStepInfo()
                            {
                                Time = GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashTime,
                                Speed = GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashPumpSpeed,
                                IsCustomized = false,
                                StepAction = Enum_StepAction.None,

                            });
                        }
                        list.Add(injectionTaskParam);
                        InjectionTaskParam injectionPreRunTaskParam = new InjectionTaskParam
                        {
                            Step = Enum_ParamStep.PreRun,
                            StepInfoList = new List<ParamStepInfo>()
                        };
                        injectionPreRunTaskParam.StepInfoList.Add(new ParamStepInfo()
                        {
                            Time = GlobalInfo.Instance.SettingInfo.PreRunningInfo[0].WashTime,
                            Speed = GlobalInfo.Instance.SettingInfo.PreRunningInfo[0].WashPumpSpeed,
                            IsCustomized = GlobalInfo.Instance.SettingInfo.PreRunningInfo[0].WashTimeTypeIndex == 1 ? true : false,
                            StepAction = Enum_StepAction.PreRun_Flush,

                        });
                        injectionPreRunTaskParam.StepInfoList.Add(new ParamStepInfo()
                        {
                            Time = GlobalInfo.Instance.SettingInfo.PreRunningInfo[1].WashTime,
                            Speed = GlobalInfo.Instance.SettingInfo.PreRunningInfo[1].WashPumpSpeed,
                            IsCustomized = GlobalInfo.Instance.SettingInfo.PreRunningInfo[0].WashTimeTypeIndex == 1 ? true : false,
                            StepAction = Enum_StepAction.PreRun_Delay,

                        });
                        list.Add(injectionPreRunTaskParam);
                        InjectionTaskParam injectionAnalysisTaskParam = new InjectionTaskParam
                        {
                            Step = Enum_ParamStep.Analysis,
                            StepInfoList = new List<ParamStepInfo>()
                        };
                        injectionAnalysisTaskParam.StepInfoList.Add(new ParamStepInfo()
                        {
                            Time = GlobalInfo.Instance.SettingInfo.AnalysInfo.WashTime,
                            Speed = GlobalInfo.Instance.SettingInfo.AnalysInfo.WashPumpSpeed,
                            IsCustomized = GlobalInfo.Instance.SettingInfo.AnalysInfo.WashTimeTypeIndex == 1 ? true : false,
                            StepAction = Enum_StepAction.None,

                        });
                        list.Add(injectionAnalysisTaskParam);
                        InjectionTaskParam injectionAfterRunningTaskParam = new InjectionTaskParam
                        {
                            Step = Enum_ParamStep.PostRun,
                            StepInfoList = new List<ParamStepInfo>()
                        };
                        for (int i = 0; i < GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Count; i++)
                        {
                            ParamStepInfo info = new ParamStepInfo();
                            if (i == 0)
                                info.StepAction = Enum_StepAction.PostRun_SampleWash;
                            else if (i == 1)
                                info.StepAction = Enum_StepAction.PostRun_StdSamWash;
                            else
                                info.StepAction = Enum_StepAction.None;
                            info.Time = GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime;
                            info.Speed = GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashPumpSpeed;
                            info.IsCustomized = false;
                            injectionAfterRunningTaskParam.StepInfoList.Add(info);
                        }
                        list.Add(injectionAfterRunningTaskParam);
                        InjectionTaskParam injectionPostWashTaskParam = new InjectionTaskParam
                        {
                            Step = Enum_ParamStep.PostWash,
                            StepInfoList = new List<ParamStepInfo>()
                        };
                        ParamStepInfo washinfo = new ParamStepInfo();
                        washinfo.Time = GlobalInfo.Instance.SettingInfo.Wash1Time;
                        washinfo.Speed = GlobalInfo.Instance.SettingInfo.PumpSpeed1;
                        washinfo.StepAction = Enum_StepAction.PostWash_Wash;
                        washinfo.IsCustomized = false;
                        injectionPostWashTaskParam.StepInfoList.Add(washinfo);
                        list.Add(injectionPostWashTaskParam);
                        MainWindow_AutoSamplerSendObjectDataEvent(null,
                         new ObjectEventArgs() { MessParamType = EnumMessParamType.InjectionTaskParam, Parameter = list });

                        Control_SampleListView.TaskRunningCommand(null, null);
                    }
                    else if (msgArg.MessParamType == EnumMessParamType.StopInjectionTask)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "样品序列完成");
                        }));
                        _IsTestConnection = true;
                        //GlobalInfo.Instance.Totalab_LSerials.ZHOME();
                        Control_SampleListView.StopMethod();
                        //else if (msgArg.MessParamType == EnumMessParamType.ChlPump_LoadFinish)
                        //    IsQuickSamplerLoadFinish = true;
                        //else if (msgArg.MessParamType == EnumMessParamType.ChlPump_SamWashEnd)
                        //    IsQuickSamplerWashFinish = true;
                    }
                    else if(msgArg.MessParamType == EnumMessParamType.UseAutoSampler_Enable && msgArg.Parameter is bool useEnable)
                    {
                        IsAutoSamplerEnable = useEnable;
                    }
                    Trace.WriteLine(string.Format("ObjectData Type:[{0}]", msgArg.MessParamType));
                }
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("MainWindow [SubscribeMassSendObjectEvent]：", e);

            }
        }
        #endregion

        #region IQuickSamplerPlug
        public new string ToolTip
        {
            get => _toolTip;
            set
            {
                _toolTip = value;
                Notify("ToolTip");
            }
        }
        private string _toolTip = "quicksampler";

        public string GetDeviceName()
        {
            return null;
        }

        public string GetDeviceSn()
        {
            return null;
        }

        public byte ConnectionInit(object ComDevice)
        {
            //表示集成在HiMass里
            try
            {
                if (ComDevice is bool isHiMassConnState)
                {
                    //是否为连接HiMass状态
                    GlobalInfo.Instance.IsHimassConnState = isHiMassConnState;
                    if (GlobalInfo.Instance.IsHimassConnState)
                    {
                        //发送握手成功（已与Mass连接）
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerModule, Parameter = true });
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerProduct, Parameter = Enum_AutoSamplerProduct.AutoSampler_Circle });
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerTrayFile, Parameter = RackType });
                    }
                    else
                    {
                        //GlobalInfo.Instance.IsUseAutoSampler = true;
                    }
                }
                int retVal = 0;
                return (byte)retVal;
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ConnectionInit]：", ex);
                SampleHelper.ShowErrorMessagePage();
                return 0;
            }
        }

        public void Dispose()
        {

        }

        public int SendMsg(byte[] msgBuf)
        {
            int rtVal = 0;
            return rtVal;
        }

        public int SendMsg(Object frameData)
        {
            int rtVal = 0;
            return rtVal;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

     
    }
}