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
using System.Windows.Media;
using LabTech.UITheme.Helper;

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
            //IsConnect = true;                 //单机调试使用
            ConnectThread = new Thread(Connect);
            ConnectThread.Start();

            //GlobalInfo.LgDictionary = Application.LoadComponent(new Uri(@"/Totalab_L;component/Themes/AutoSampler_Chinese.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

            //ConnectionTestThead = new Thread(ConnectStatus);
            //ConnectionTestThead.Start();

            ConnectThread.IsBackground = true;
            if (Control_SampleListView == null)
            {
                Control_SampleListView = new SampleListModule();
                Control_SampleListView.InitData(this);
            }
            //if (Control_SettingView == null)
            //{
            //    Control_SettingView = new SettingModule();
            //    Control_SettingView.InitData(this);
            //}
            InitData();
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = @".\SamplerPlugins\Totalab_L.dll.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            IsMedicalSoft = Convert.ToBoolean(config.AppSettings.Settings["IsMedical"].Value);
            GlobalInfo.Instance.MaxConnectionTimes = Convert.ToInt32(config.AppSettings.Settings["MaxConnectTimes"].Value);
        }

        public void ConnectStatus()
        {  
            try
            {
                while (true)
                {
                    if (System.IO.Ports.SerialPort.GetPortNames()!= null)
                    {
                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                        {
                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                            GlobalInfo.Instance.Totalab_LSerials.Connect();
                        }
                        Thread.Sleep(500);
                        if (IsConnect)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                new MessagePage().ShowDialog("Message_Connection".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                            }));
                            if (GlobalInfo.Instance.IsHimassConnState)
                            {
                                MainWindow_AutoSamplerSendObjectDataEvent(null,new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                            }
                            GlobalInfo.Instance.IsCanRunning = true;
                            GlobalInfo.Instance.IsBusy = false;
                            MoveToHomeCommand(null, null);
                            //GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                            break;
                        }
                        else
                        {
                            //MainLogHelper.Instance.Info($"[ 端口号 ={PortName}");
                            ////GlobalInfo.Instance.Totalab_LSerials.MsgCome -= Sampler_MsgCome;
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            //IsConnect = false;
                            //MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                        }

                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellViewModel [Connect]：", ex);
            }
            
        }

        #region 变量
        Thread ConnectThread;
        public bool _IsTestConnection;
        //Thread ConnectionTestThead;
        bool _IsRecived;
        bool _IsSNWindowShow;//                                                         //当前SN窗口是否弹出，避免和mass联机的时候Load和重连的时候重复弹出
        ContextMenu TrayTypeMenu = new ContextMenu();
        ContextMenu StdTrayTypeMenu = new ContextMenu();
        string ButtonContent;
        public bool _IsFirst = true;                                                        //刚打开软件
        private bool _isHandReconnection = false;                                           //是否手动重新连接（避免勾选使用进样器得时候再次初始化）
        //private int ConnectMaxTimes = 10;

        SamplerPosSetPage samplerPosSetPage = new SamplerPosSetPage();                      //点位移之前先Z轴复位

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
        private bool _isSNRegistered = false;            //许可证初始值

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
        private bool _isConnect = false;

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

        public string CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                Notify("CurrentPosition");
            }
        }
        private string _currentPosition = "1";

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

        public ObservableCollection<string> CurrentTrayTypeList
        {
            get => _currentTrayTypeList;
            set
            {
                _currentTrayTypeList = value;
                Notify("CurrentTrayTypeList");
            }
        }
        private ObservableCollection<string> _currentTrayTypeList = new ObservableCollection<string>();

        public SolidColorBrush StatusColors
        {
            get => _statusColors;
            set
            {
                _statusColors = value;
                Notify("StatusColors");
            }
        }
        private SolidColorBrush _statusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBDBDBD"));

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                Notify("StatusText");
            }

        }
        private string _statusText = "D/C";
        #endregion

        #region 事件
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> languageList = new List<string>
                {
                    "/Totalab_L;component/Themes/AutoSampler_"
                };
                ResourceDictionaryHelper.SwitchLanguage(Enum_Languages.Chinese, languageList);
                //ConfigurationHelper.UpdateAppSettings("LanguageName", Enum_Languages.English.ToString());

                if (!IsSNRegistered)
                {
                    IsSoftwareOffline = false;
                    //序列号授权验证
                    RegistSN registSN = RegistSN.GetInstance("LabMonsterTotalab-L");
                    registSN.Init(new InstrumentSerialNum(), "LabMonsterTotalab-L");
                    ResultData resultData = registSN.Register();
                    if (resultData != null && resultData.IsSuccessful
                             && (resultData.ErrorCodeList == null || (resultData.ErrorCodeList != null && resultData.ErrorCodeList.Count == 0)))
                    {
                        IsSNRegistered = true;
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });

                        //if (_IsFirst)
                        //{
                        //    GlobalInfo.Instance.IsCanRunning = false;
                        //    GlobalInfo.Instance.IsBusy = true;
                        //    //GlobalInfo.Instance.Totalab_LSerials.XWZHome();                 //系统复位初始化
                        //}
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;                        //放开

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
                            //bool? rlt = registerSNPage.ShowDialog();
                            _IsSNWindowShow = false;
                            //if (rlt == true)
                            {
                                IsSNRegistered = true;
                                MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });
                                //if (_IsFirst)
                                //{
                                //    GlobalInfo.Instance.IsCanRunning = false;
                                //    GlobalInfo.Instance.IsBusy = true;
                                //    GlobalInfo.Instance.Totalab_LSerials.XWZHome();           //系统复位初始化
                                //}
                                GlobalInfo.Instance.IsBusy = false;
                                GlobalInfo.Instance.IsCanRunning = true;                        //放开
                                //GlobalInfo.Instance.IsSNRegistered = true;
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
                            WashLoc="RinseLoc",
                            WashPumpSpeed = 40,
                            WashTime = 5
                        },
                    };
                    GlobalInfo.Instance.SettingInfo.PreRunningInfo = new ObservableCollection<AnalysInfo>
                    {
                        new AnalysInfo
                        {
                            //WashSpeedTypeIndex = 1,
                            WashPumpSpeed = 40,
                            WashTimeTypeIndex = 1,
                            WashTime = 5
                        },
                        new AnalysInfo
                        {
                            //WashSpeedTypeIndex = 1,
                            WashPumpSpeed = 40,
                            //WashTimeTypeIndex = 1,
                            WashTime = 5
                        },
                    };
                    GlobalInfo.Instance.SettingInfo.AfterRunningInfo = new ObservableCollection<ParaItemInfo>
                    {
                        new ParaItemInfo
                        {
                            StepName = "AutoSampler_Main_PostRun".GetWord(),
                            WashAction="AutoSampler_Main_InjectNeedleFlushSam".GetWord(),
                            WashActionKey=1,
                            WashLoc="RinseLoc",
                            WashPumpSpeed = 40,
                            WashTime = 20
                        },
                        new ParaItemInfo
                        {
                            WashAction="AutoSampler_Main_InjectNeedleFlushStdSam".GetWord(),
                            WashActionKey=2,
                            WashLoc="RinseLoc",
                            WashPumpSpeed = 40,
                            WashTime = 20
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
        /// <summary>
        /// 样品列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SampleListCommand(object sender, RoutedEventArgs e)
        {
            this.Content_ActiveItem.Content = Control_SampleListView;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SettingCommand(object sender, RoutedEventArgs e)
        {
            if (Control_SettingView == null)
            {
                Control_SettingView = new SettingModule();
                Control_SettingView.InitData(this);
            }
            this.Content_ActiveItem.Content = Control_SettingView;
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AdvancedSetCommand(object sender, RoutedEventArgs e)
        {
            Control_SamplerPosSetView = new SamplerPosSetPage()
            {
                Owner = Application.Current.MainWindow
            };
            Control_SamplerPosSetView.InitData(this);
            Control_SamplerPosSetView.ShowDialog();
        }
        //更换进样针
        private void Btn_Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();

                bool result = false;
                Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        bool? result1 = new MessagePage().ShowDialog("Message_ReplaceNeedle".GetWord(), "Message_TitleReplaceNeedle".GetWord(), true, yesContent: "Message_ButtonOK".GetWord(), cancelContent: "ButtonContent_Cancel".GetWord());
                        if (result1 == true)
                        {
                            //更换前
                            #region  防止撞针
                            samplerPosSetPage.MoveToZ_0Command();
                            Thread.Sleep(500);
                            #endregion
                            result = MotorActionHelper.MotorMoveToTargetPosition(GlobalInfo.Instance.TrayPanelCenter + GlobalInfo.Instance.TrayPanelHomeX * GlobalInfo.XLengthPerCircle / 3600.0, GlobalInfo.Instance.TrayPanel_leftW + 90);
                            if (result == false)
                            {
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    ConntectWaring();
                                }
                            }
                            else
                            {
                                //更换完后
                                samplerPosSetPage.MoveToZ_ChangeCommand();
                                bool? result2 = new MessagePage().ShowDialog("Message_ReplaceNeedleEnd".GetWord(), "Message_TitleReplaceNeedle".GetWord(), true, yesContent: "Message_ButtonOK".GetWord());
                                if (result2 == true)
                                {
                                    GlobalInfo.Instance.IsCanRunning = false;
                                    GlobalInfo.Instance.IsBusy = true;
                                    CurrentPosition = "W1";
                                    MoveToPositionCommand(null, null);
                                    //GlobalInfo.Instance.Totalab_LSerials.XWZHome();           //系统复位初始化
                                }
                            }
                        //MessageBoxResult result1 = MessageBox.Show("确定要更换进样针吗！" ,"警告", MessageBoxButton.OKCancel,MessageBoxImage.Warning);
                        // if (result1 == System.Windows.MessageBoxResult.OK)
                        // {
                        //     MessageBox.Show("请先移动C号试管底座！", "温馨提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        // } 
                        }
                    }));
                    if (IsConnect)
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                    }
                    source?.Cancel();
                    source?.Dispose();

                }, source.Token);

            }
            catch (Exception)
            {

            }

        }
        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_About_Click(object sender, RoutedEventArgs e)
        {
            AboutPage aboutPage = new AboutPage()
            {
                Owner = Application.Current.MainWindow
            };
            aboutPage.InitData(this);
            aboutPage.ShowDialog();
        }
        bool isHand = false;
        public void ConnectionCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsConnect)
                {
                    isHand = true;
                    Connect();
                    if (GlobalInfo.Instance.Totalab_LSerials.IsConnect)
                    {
                        //GlobalInfo.Instance.Totalab_LSerials.Init();
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

        #region   样品列表内位置按钮
        private void TrayEItemClickCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton radioButton && radioButton != null)
                {
                    ItemData item = radioButton.DataContext as ItemData;
                    CurrentPosition = item.ItemContent;
                    //SelectedTrayName = Enum_TrayName.TrayE;
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
                    CurrentPosition = item.ItemContent;
                    //SelectedTrayName = Enum_TrayName.TrayD;
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
                    CurrentPosition = item.ItemContent;
                    //SelectedTrayName = Enum_TrayName.TrayC;
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
                    CurrentPosition = item.ItemContent;
                    //SelectedTrayName = Enum_TrayName.TrayB;
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
                    CurrentPosition = item.ItemContent;
                    //SelectedTrayName = Enum_TrayName.TrayA;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [ TrayAItemClickCommand]", ex);
            }
        }

        #endregion

        /// <summary>
        /// 位移指定位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveToPositionCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                IsSamplerManual = true;
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
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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

                        //#region  防止撞针
                        samplerPosSetPage.MoveToZ_0Command();
                        Thread.Sleep(100);
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                            else
                            {
                                samplerPosSetPage.MoveToZ_0Command();
                            }
                        }
                        Point pt = new Point();
                        int isCollisionStatus = 0;
                        if (CurrentPosition == "W1")
                        {
                            pt = GetPositionInfoHelper.GetWashPosition(CurrentPosition);
                            //pt = new Point((GlobalInfo.Instance.TrayPanelCenter + GlobalInfo.Instance.TrayPanelHomeX + W1_valueX ) / GlobalInfo.XLengthPerCircle * 3600.0, GlobalInfo.Instance.CalibrationInfo.W1PointY * 3.0 * 10.0);
                        }
                        else if (CurrentPosition == "W2")
                        {
                             pt = GetPositionInfoHelper.GetWashPosition(CurrentPosition);
                            //pt = new Point((GlobalInfo.Instance.TrayPanelCenter + GlobalInfo.Instance.TrayPanelHomeX + W2_valueX) / GlobalInfo.XLengthPerCircle * 3600.0, GlobalInfo.Instance.CalibrationInfo.W2PointY * 3.0 * 10.0);
                        }
                        else
                        {
                            pt = GetPositionInfoHelper.GetItemPosition(int.Parse(CurrentPosition));                     //要去的位置
                            isCollisionStatus = GetPositionInfoHelper.GetXIsCollision(pt, int.Parse(CurrentPosition));
                        }
                        if (isCollisionStatus == 1)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)(140 / GlobalInfo.XLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeX));
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 2 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)(100 / GlobalInfo.XLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeX));
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                        }               //左边碰撞时
                        else if (isCollisionStatus == 2)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)(354 / GlobalInfo.XLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeX));
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 2 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)(354 / GlobalInfo.XLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeX));
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                        }           //右边碰撞时
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
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    Thread.Sleep(100);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        if (isCollisionStatus == 0)
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
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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

                        //GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        //Thread.Sleep(500);
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
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
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
                        MainLogHelper.Instance.Error("ShellPage [MoveToPositionCommand]", ex);
                    }
                    finally
                    {
                        IsSamplerManual = false;
                        #region 分段抬针
                        //Control_SettingView.ZReset();
                        #endregion
                        if (IsConnect)
                        {
                            GlobalInfo.Instance.IsBusy = false;
                            GlobalInfo.Instance.IsCanRunning = true;
                        }
                        source?.Cancel();
                        source?.Dispose();
                    }

                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("ShellPage [MoveToPositionCommand]", ex); }
        }
        /// <summary>
        /// Z针抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveToZHomeCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                IsSamplerManual = true;
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
                        Thread.Sleep(200);
                        //samplerPosSetPage.MoveToZ_0Command();
                        //Thread.Sleep(1000);

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
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
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

                        #region 分段抬针
                        Control_SettingView.ZReset();
                        #endregion

                        #region 直接抬起
                        ////设置Z轴速度
                        //GlobalInfo.Instance.RunningStep = RunningStep_Status.SetZSpeed;
                        //GlobalInfo.Instance.Totalab_LSerials.SetZSpeed(0x03, (int)(Math.Round(GlobalInfo.Instance.CalibrationInfo.Speed2_value / 100, 2) * 700));
                        //while (true)
                        //{
                        //    longseconds = DateTime.Now.Ticks / 10000;
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetZSpeedOK)
                        //    {
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetZSpeedOK && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //        {
                        //            Thread.Sleep(100);
                        //        }
                        //        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetZSpeedOK)
                        //        {
                        //            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        //            {
                        //                //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                        //                try
                        //                {
                        //                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetZSpeed;
                        //                    GlobalInfo.Instance.Totalab_LSerials.SetZSpeed(0x03, (int)(Math.Round(GlobalInfo.Instance.CalibrationInfo.Speed2_value / 100, 2) * 700));
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                        //                }

                        //                count++;
                        //            }
                        //            else
                        //            {
                        //                ConntectWaring();
                        //                return;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            //while (ExpStatus == Exp_Status.Pause)
                        //            //{
                        //            //    Thread.Sleep(20);
                        //            //}
                        //            //if (stopType == 2 && IsStopWash == false)
                        //            //    return;
                        //            break;
                        //        }

                        //    }
                        //}
                        ////抬起针
                        //GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        //GlobalInfo.status = true;
                        //GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.CalibrationInfo.ZResetPosition / GlobalInfo.ZLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeZ));
                        //longseconds = DateTime.Now.Ticks / 10000;
                        //count = 0;
                        //while (true)
                        //{
                        //    longseconds = DateTime.Now.Ticks / 10000;
                        //    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //    {
                        //        Thread.Sleep(100);
                        //    }
                        //    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //    {
                        //        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        //        {
                        //            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                        //            try
                        //            {
                        //                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                        //                {
                        //                    try
                        //                    {
                        //                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                        //                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                        //                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        //                        GlobalInfo.status = true;
                        //                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.CalibrationInfo.ZResetPosition / GlobalInfo.ZLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeZ));
                        //                    }
                        //                    catch (Exception ex)
                        //                    {
                        //                        MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
                        //                    }
                        //                }

                        //            }
                        //            catch (Exception ex)
                        //            {
                        //                MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
                        //            }

                        //            count++;
                        //            continue;
                        //        }
                        //        else
                        //        {
                        //            ConntectWaring();
                        //            return;
                        //        }
                        //    }
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                        //    {

                        //        return;
                        //    }
                        //    else
                        //        break;
                        //}
                        //GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        //GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                        //count = 0;
                        //while (true)
                        //{
                        //    longseconds = DateTime.Now.Ticks / 10000;
                        //    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //    {
                        //        Thread.Sleep(100);
                        //    }
                        //    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //    {
                        //        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        //        {
                        //            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                        //            try
                        //            {
                        //                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                        //                {
                        //                    try
                        //                    {
                        //                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                        //                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                        //                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        //                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                        //                    }
                        //                    catch (Exception ex)
                        //                    {
                        //                        MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
                        //                    }
                        //                }

                        //            }
                        //            catch (Exception ex)
                        //            {
                        //                MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
                        //            }

                        //            count++;
                        //            continue;
                        //        }
                        //        else
                        //        {
                        //            ConntectWaring();
                        //            return;
                        //        }
                        //    }
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                        //    {

                        //        return;
                        //    }
                        //    else
                        //        break;
                        //}
                        //GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        //GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                        //count = 0;
                        //while (true)
                        //{
                        //    longseconds = DateTime.Now.Ticks / 10000;
                        //    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //    {
                        //        Thread.Sleep(100);
                        //    }
                        //    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //    {
                        //        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        //        {
                        //            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                        //            try
                        //            {
                        //                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                        //                {
                        //                    try
                        //                    {
                        //                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                        //                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                        //                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        //                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                        //                    }
                        //                    catch (Exception ex)
                        //                    {
                        //                        MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
                        //                    }
                        //                }

                        //            }
                        //            catch (Exception ex)
                        //            {
                        //                MainLogHelper.Instance.Error("[MoveToZHomeCommand]：", ex);
                        //            }

                        //            count++;
                        //            continue;
                        //        }
                        //        else
                        //        {
                        //            ConntectWaring();
                        //            return;
                        //        }
                        //    }
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                        //    {

                        //        return;
                        //    }
                        //    else
                        //        break;
                        //}
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("ShellPage [MoveToZHomeCommand]", ex);
                    }
                    finally
                    {
                        IsSamplerManual = false;
                        if (IsConnect)
                        {
                            GlobalInfo.Instance.IsBusy = false;
                            GlobalInfo.Instance.IsCanRunning = true;
                        }
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("ShellPage [MoveToZHomeCommand]", ex); }
        }
        //移动到初始位置--复位
        public void MoveToHomeCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                IsSamplerManual = true;
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        GlobalInfo.Instance.IsMotorXError = true;
                        GlobalInfo.Instance.IsMotorWError = true;
                        GlobalInfo.Instance.IsMotorZError = true;
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                //MainLogHelper.Instance.Info("清错失败，进入重连状态");
                                ConntectWaring();
                                //return;
                            }
                            else
                            {
                                GlobalInfo.Instance.IsMotorXError = false;
                                GlobalInfo.Instance.IsMotorWError = false;
                                GlobalInfo.Instance.IsMotorZError = false;
                            }
                        }

                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                        while (true)
                        {
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk)
                            {
                                break;
                            }
                            else
                            {
                                int count = 0;
                                long longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                                            GlobalInfo.Instance.Totalab_LSerials.PumpStop();

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToHomeCommand]：", ex);
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
                        Thread.Sleep(100);
                        //this.Dispatcher.Invoke(new Action(delegate
                        //{
                        //    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "蠕动泵停止");
                        //}));

                        GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHome;
                        GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                        while (true)
                        {
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.XYZHomeOk)
                            {
                                break;
                            }
                            else
                            {
                                int count = 0;
                                long longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.XYZHomeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    MainLogHelper.Instance.Error("[MoveToHomeCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToHomeCommand]：", ex);
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
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("ShellPage[MoveToHomeCommand]", ex);
                    }
                    finally
                    {
                        IsSamplerManual = false;
                        //GlobalInfo.Instance.IsBusy = false;
                        //GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("ShellPage[MoveToHomeCommand]]", ex); }
        }
        //Z的位置
        public void MoveToZCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                IsSamplerManual = true;
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
                        Thread.Sleep(500);

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
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                    MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
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
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.SettingInfo.SamplingDepth / GlobalInfo.ZLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeZ));
                        longseconds = DateTime.Now.Ticks / 10000;
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.SettingInfo.SamplingDepth / GlobalInfo.ZLengthPerCircle * 3600.0 + GlobalInfo.Instance.TrayPanelHomeZ));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
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
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
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
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                                                MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
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
                        MainLogHelper.Instance.Error("ShellPage [MoveToZCommand]", ex);
                    }
                    finally
                    {
                        IsSamplerManual = false;
                        if (IsConnect)
                        {
                            GlobalInfo.Instance.IsBusy = false;
                            GlobalInfo.Instance.IsCanRunning = true;
                        }
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("ShellPage [MoveToZCommand]", ex); }
        }


        //使用自动进样器
        public void UseAutoSamplerCommand(object sender, RoutedEventArgs e)
        {
            if (GlobalInfo.Instance.IsHimassConnState && IsUseAutoSampler)
            {
                GlobalInfo.Instance.SampleInfos = new ObservableCollection<SampleItemInfo>();
                //SampleHelper.CreateSampleInfos(1);//运行显示25

                //更新是否使用自动进样器的状态消息
                this.Dispatcher.Invoke((Action)(() =>
                {
                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "自动进样器使用");
                }));

                //勾选进样器后复位初始化
                if (_isHandReconnection==false)
                {
                    MoveToHomeCommand(null, null);
                }
                //GlobalInfo.Instance.IsBusy = true;
                //GlobalInfo.Instance.Totalab_LSerials.XWZHome();           //系统复位初始化
            }
            else
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "自动进样器不使用");
                }));
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
                if (Control_SettingView != null)
                {
                    Control_SettingView.Method_Name = "";                   //清空一下方法名称
                }
                GlobalInfo.Instance.SampleInfos = new ObservableCollection<SampleItemInfo>();
                //SampleHelper.CreateSampleInfos(1);//运行显示25
                CurrentRotationIndex = 0;
                GlobalInfo.Instance.SettingInfo = new SettingInfo();
                SampleHelper.RefreshSamplerItemStatus(Exp_Status.Free);

                GlobalInfo.Instance.SettingInfo.PreWashInfos = new ObservableCollection<PreWashItemInfo>
                {
                    new PreWashItemInfo
                    {
                        StepName = "AutoSampler_Main_PreFlush".GetWord(),
                        IsOpenAction = true,
                        WashLoc="RinseLoc",
                        WashPumpSpeed = 40,
                        WashTime = 5
                    },
                };
                GlobalInfo.Instance.SettingInfo.PreRunningInfo = new ObservableCollection<AnalysInfo>
                {
                    new AnalysInfo
                    {
                        WashPumpSpeed = 40,
                        WashTimeTypeIndex = 1,
                        WashTime = 5
                    },
                    new AnalysInfo
                    {
                        //WashSpeedTypeIndex = 1,
                        WashPumpSpeed = 40,
                        //WashTimeTypeIndex = 1,
                        WashTime = 5
                    },
                };
                GlobalInfo.Instance.SettingInfo.AfterRunningInfo = new ObservableCollection<ParaItemInfo>
                {
                    new ParaItemInfo
                    {
                        StepName = "AutoSampler_Main_PostRun".GetWord(),
                        WashAction="AutoSampler_Main_InjectNeedleFlushSam".GetWord(),
                        WashActionKey=1,
                        WashLoc="RinseLoc",
                        WashPumpSpeed = 40,
                        WashTime = 20
                    },
                    new ParaItemInfo
                    {
                        WashAction="AutoSampler_Main_InjectNeedleFlushStdSam".GetWord(),
                        WashActionKey=2,
                        WashLoc ="RinseLoc",
                        WashPumpSpeed = 40,
                        WashTime = 20
                    },
                };
                GlobalInfo.Instance.CurrentMethod.MethodName = "";
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
             //GlobalInfo.Instance.MethodName;
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
                CurrentPositionLostFocusCommand(null, null);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [RackTypeChangedCommand]", ex);
            }
        }

        private void CurrentPositionLostFocusCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CurrentPosition.All(char.IsDigit))
                {
                    if (CurrentPosition != "W1" && CurrentPosition != "W2")
                        CurrentPosition = "W1";
                }
                else
                {
                    if (int.Parse(CurrentPosition) < 1 || int.Parse(CurrentPosition) > GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                        CurrentPosition = "1";
                }
                if (CurrentPosition == "W1")
                {
                    GlobalInfo.Instance.TrayCleanInfos.TrayItemList[0].IsItemSelected = true;
                }
                else if (CurrentPosition == "W2")
                {
                    GlobalInfo.Instance.TrayCleanInfos.TrayItemList[0].IsItemSelected = true;
                }
                else if (CurrentPosition.All(char.IsDigit))
                {
                    if (int.Parse(CurrentPosition) >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && int.Parse(CurrentPosition) <= GlobalInfo.Instance.TrayAInfos.TrayEndNumber)
                    {
                        GlobalInfo.Instance.TrayAInfos.TrayItemList[int.Parse(CurrentPosition) - 1].IsItemSelected = true;
                    }
                    else if (int.Parse(CurrentPosition) >= GlobalInfo.Instance.TrayBInfos.TrayStartNumber && int.Parse(CurrentPosition) <= GlobalInfo.Instance.TrayBInfos.TrayEndNumber)
                    {
                        GlobalInfo.Instance.TrayBInfos.TrayItemList[int.Parse(CurrentPosition) - GlobalInfo.Instance.TrayBInfos.TrayStartNumber].IsItemSelected = true;
                    }
                    else if (int.Parse(CurrentPosition) >= GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber && int.Parse(CurrentPosition) <= GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber)
                    {
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemList[int.Parse(CurrentPosition) - GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber].IsItemSelected = true;
                    }
                    else if (int.Parse(CurrentPosition) >= GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber && int.Parse(CurrentPosition) <= GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                    {
                        GlobalInfo.Instance.TraySTD2Infos.TrayItemList[int.Parse(CurrentPosition) - GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber].IsItemSelected = true;
                    }
                    else if (int.Parse(CurrentPosition) >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && int.Parse(CurrentPosition) <= GlobalInfo.Instance.TrayDInfos.TrayEndNumber)
                    {
                        GlobalInfo.Instance.TrayDInfos.TrayItemList[int.Parse(CurrentPosition) - GlobalInfo.Instance.TrayDInfos.TrayStartNumber].IsItemSelected = true;
                    }
                    else if (int.Parse(CurrentPosition) >= GlobalInfo.Instance.TrayEInfos.TrayStartNumber && int.Parse(CurrentPosition) <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
                    {
                        GlobalInfo.Instance.TrayEInfos.TrayItemList[int.Parse(CurrentPosition) - GlobalInfo.Instance.TrayEInfos.TrayStartNumber].IsItemSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {

                MainLogHelper.Instance.Error("ShellPage [CurrentPositionLostFocusCommand]", ex);
            }
        }

        private void TrayTypeChangedCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = sender as MenuItem;
                Enum_TrayType type = EnumHelper.GetEnumValue<Enum_TrayType>(item.Header.ToString());
                if (ButtonContent == "Tray A")
                    GlobalInfo.Instance.TrayAInfos = TrayInfoHelper.GetTrayInfo(type);
                if (ButtonContent == "Tray B")
                    GlobalInfo.Instance.TrayBInfos = TrayInfoHelper.GetTrayInfo(type);
                if (ButtonContent == "Tray D")
                    GlobalInfo.Instance.TrayDInfos = TrayInfoHelper.GetTrayInfo(type);
                if (ButtonContent == "Tray E")
                    GlobalInfo.Instance.TrayEInfos = TrayInfoHelper.GetTrayInfo(type);
                TrayInfoHelper.GetTrayNumber();
                SampleHelper.RefreshSamplerItemStatus(Exp_Status.Ready);
                CurrentPositionLostFocusCommand(null, null);
                Control_SettingView?.RefreshList();
                SaveTrayType();
                MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerTrayFile, Parameter = GetTrayInfoString() });
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [TrayTypeChangedCommand]", ex);
            }
        }

        private void StdTrayTypeChangedCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = sender as MenuItem;
                Enum_StdTrayType type = EnumHelper.GetEnumValue<Enum_StdTrayType>(item.Header.ToString());
                TrayInfoHelper.GetStdTrayInfo(type);
                TrayInfoHelper.GetTrayNumber();
                SampleHelper.RefreshSamplerItemStatus(Exp_Status.Ready);
                CurrentPositionLostFocusCommand(null, null);
                Control_SettingView?.RefreshList();
                SaveTrayType();
                MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerTrayFile, Parameter = GetTrayInfoString() });
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [StdTrayTypeChangedCommand]", ex);
            }
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
            RegistSN registSN = RegistSN.GetInstance("LabMonsterTotalab-L");
            registSN.Init(new InstrumentSerialNum(), "LabMonster Totalab-L");
            ResultData resultData = registSN.Register();
            if (resultData != null && resultData.IsSuccessful
                             && (resultData.ErrorCodeList == null || (resultData.ErrorCodeList != null && resultData.ErrorCodeList.Count == 0)))
            {
                IsSNRegistered = true;
                MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });
                //if (_IsFirst)
                //{
                GlobalInfo.Instance.IsBusy = false;
                GlobalInfo.Instance.IsCanRunning = true;                        //放开
                //    GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                //}
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
                            //if (_IsFirst)
                            //{
                            GlobalInfo.Instance.IsBusy = false;
                            GlobalInfo.Instance.IsCanRunning = true;                        //放开
                            //    GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                            //}
                        }
                        registerSNPage = null;
                    }));
                }
            }
        }
        //保存架子的类型
        private void SaveTrayType()
        {
            try
            {
                string SavePath = "";
                CurrentTrayTypeList[0] = GlobalInfo.Instance.TrayAInfos.TrayType;
                CurrentTrayTypeList[1] = GlobalInfo.Instance.TrayBInfos.TrayType;
                CurrentTrayTypeList[2] = GlobalInfo.Instance.TrayCleanInfos.TrayType;
                CurrentTrayTypeList[3] = GlobalInfo.Instance.TrayDInfos.TrayType;
                CurrentTrayTypeList[4] = GlobalInfo.Instance.TrayEInfos.TrayType;
                SavePath = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "TrayTypeList.ini");
                byte[] content = XmlObjSerializer.Serialize(CurrentTrayTypeList);
                FileHelper.WriteEncrypt(SavePath, content);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[ShellPage]SaveTrayType：", ex);
                new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }
        //运行初始化架子数据
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
                if (File.Exists(Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "TrayTypeList.ini")))
                {
                    byte[] content = FileHelper.ReadDecrypt(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "TrayTypeList.ini"));
                    if (content != null)
                    {
                        CurrentTrayTypeList = XmlObjSerializer.Deserialize<ObservableCollection<string>>(content);
                        //InitSettingInfo();
                    }
                }
                if (CurrentTrayTypeList.IsNullOrEmpty() || CurrentTrayTypeList.Count == 0)
                {
                    CurrentTrayTypeList = new ObservableCollection<string>
                    {
                        "15mL * 60",
                        "15mL * 60",
                        "250mL * 5",
                        "15mL * 60",
                        "15mL * 60"
                    };
                }
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
                    GlobalInfo.Instance.TrayAInfos = TrayInfoHelper.GetTrayInfo(EnumHelper.GetEnumValue<Enum_TrayType>(CurrentTrayTypeList[0]));
                    GlobalInfo.Instance.TrayBInfos = TrayInfoHelper.GetTrayInfo(EnumHelper.GetEnumValue<Enum_TrayType>(CurrentTrayTypeList[1]));
                    //GlobalInfo.Instance.TrayCleanInfos = TrayInfoHelper.GetTrayInfo(null);
                    GlobalInfo.Instance.TrayDInfos = TrayInfoHelper.GetTrayInfo(EnumHelper.GetEnumValue<Enum_TrayType>(CurrentTrayTypeList[3]));
                    GlobalInfo.Instance.TrayEInfos = TrayInfoHelper.GetTrayInfo(EnumHelper.GetEnumValue<Enum_TrayType>(CurrentTrayTypeList[4]));
                    GlobalInfo.Instance.TrayCleanInfos.ItemsSize = new Size(20, 20);
                    for (int i = 1; i < 3; i++)
                    {
                        ItemData item = new ItemData
                        {
                            ItemContent = "W" + i.ToString()
                        };
                        GlobalInfo.Instance.TrayCleanInfos.TrayItemList.Add(item);
                    }
                    TrayInfoHelper.GetStdTrayInfo(EnumHelper.GetEnumValue<Enum_StdTrayType>(CurrentTrayTypeList[2]));
                    TrayInfoHelper.GetTrayNumber();
                    GlobalInfo.Instance.TrayAInfos.TrayItemList[0].IsItemSelected = true;
                }));

                GlobalInfo.Instance.SettingInfo.PreWashInfos = new ObservableCollection<PreWashItemInfo>
                {

                    new PreWashItemInfo
                    {
                        StepName = "AutoSampler_Main_PreFlush".GetWord(),
                        IsOpenAction = true,
                        WashLoc="RinseLoc",
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
                        WashLoc="RinseLoc",
                        WashPumpSpeed = 40,
                        WashTime = 20
                    },
                    new ParaItemInfo
                    {
                        WashAction="AutoSampler_Main_InjectNeedleFlushStdSam".GetWord(),
                        WashActionKey=2,
                        WashLoc="RinseLoc",
                        WashPumpSpeed = 40,
                        WashTime = 20
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
                            if (isHand)
                            {
                                _isHandReconnection = true;
                                Application.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    new MessagePage().ShowDialog("Message_Connection".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                                }));
                                if (GlobalInfo.Instance.IsHimassConnState)
                                {
                                    MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                                }
                                isHand = false;
                                GlobalInfo.Instance.IsCanRunning = true;
                                GlobalInfo.Instance.IsBusy = false;
                                MoveToHomeCommand(null, null);
                                //GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                                //MoveToWashW1Command(null,null);
                            }
                            //if (ConnectionTestThead == null)
                            //{
                            //    ConnectionTestThead = new Thread(ConnectionTest);
                            //    ConnectionTestThead.Start();
                            //}
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
                //byte[] content = FileHelper.ReadDecrypt(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.ini"));
                //if (content != null)
                //{
                //    GlobalInfo.Instance.CalibrationInfo = XmlObjSerializer.Deserialize<TrayPanelCalibrationInfo>(content);
                //}
                //else
                //{

                //    this.Dispatcher.Invoke((Action)(() =>
                //    {
                //        new MessagePage().ShowDialog("进样器位置参数调用错误，请重新设置", "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                //    }));
                //}
                byte[] content = FileHelper.Read(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "TrayInfo.ini"));
                if (content != null)
                {
                    GlobalInfo.Instance.TrayDataList = XmlObjSerializer.Deserialize<List<TrayMechanicalData>>(content);
                }
                else
                {

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        new MessagePage().ShowDialog("Message_TrayInfoError".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                    }));
                }
                content = FileHelper.Read(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "StdTrayInfo.ini"));
                if (content != null)
                {
                    GlobalInfo.Instance.StdTrayDataList = XmlObjSerializer.Deserialize<List<CustomTrayMechaincalData>>(content);
                }
                else
                {

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        new MessagePage().ShowDialog("Message_TrayInfoError".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                    }));
                }
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
                                this.Dispatcher.Invoke(new Action(delegate
                                {
                                    StatusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBDBDBD"));
                                    StatusText = "D/C";
                                }));
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex.ToString());
                            }
                        }
                        else
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
        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sampler_MsgCome(object sender, Serials.SamplerSerials.MsgComeEventArgs e)
        {
            try
            {
                GlobalInfo.IsDisconnected = true;                       //判断返回指令前置为true
                GlobalInfo.Instance.Totalab_LSerials.WaitSendSuccess.Set();
                _IsRecived = true;
                if (e == null)
                    return;
                switch (e.Msg.FunctionCode)
                {
                    #region 0x55 0xa1 连接广播
                    case 0x55:
                        if (e.Msg.Cmd == 0xA1)
                        {
                            //this.Dispatcher.Invoke((Action)(() =>
                            //{
                            GlobalInfo.IsDisconnected = false;                      //运行过程中如果有返回85 50，说明未断连接
                            IsConnect = true;
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                StatusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF64DD17"));
                                StatusText = "Normal";
                            }));
                            //if (_IsFirst)
                            //{
                            //    GlobalInfo.Instance.IsCanRunning = false;
                            //    GlobalInfo.Instance.IsBusy = true;
                            //    GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                            //}
                            //}));
                            //_IsRecived = true;
                        }
                        break;
                    #endregion

                    #region 0xAA 底层版本号、序列号读取、断电重连
                    case 0xAA:
                        if (e.Msg.Cmd == 0xA1)
                        {
                            GlobalInfo.Instance.IsReadMCUOk = true;
                            GlobalInfo.Instance.MCUData = new byte[30];
                            GlobalInfo.Instance.MCUData = e.Msg.Data;
                        }
                        if (e.Msg.Cmd == 0xA2)
                            GlobalInfo.Instance.IsWriteMCUOk = true;
                        if (e.Msg.Cmd == 0xAA)
                        {
                            GlobalInfo.IsAgainPower = true;
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "进样器已重启,重新连接");
                                //new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                            }));
                        }
                        break;
                    #endregion

                    #region 0x85 功能码、校验码
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
                            Thread.Sleep(50);
                            break;
                        }
                        break;
                    #endregion

                    #region 0x81--Z轴抬针速度
                    case 0x81:
                        if (e.Msg.Cmd == 0x99)
                        {

                        }
                        else if (e.Msg.Cmd == 0x9A)
                        {

                        }
                        else if (e.Msg.Cmd == 0x9B)
                        {
                            if (IsSamplerManual)
                            {
                                GlobalInfo.Instance.IsBusy = false;
                                IsSamplerManual = false;
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.ZHome)
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.ZHomeOk;
                        }
                        else if (e.Msg.Data[1] == 0x03)               //针抬起速度
                        {
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetZSpeed)
                            {
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetZSpeedOK;
                            }
                        }

                        //else if (e.Msg.Cmd == 0x3d)
                        //{
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.ClosePump)
                        //        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePumpOk;
                        //}
                        //else if (e.Msg.Cmd == 0x09)
                        //{
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.OpenPump)
                        //        GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPumpOk;
                        //}
                        //else if (e.Msg.Cmd == 0x17)
                        //{
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetPumpSpeed)
                        //        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeedOk;
                        //}
                        break;
                    #endregion

                    #region  0x21 蠕动泵
                    case 0x21:
                        if (e.Msg.Cmd == 0x3d)
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
                    #endregion

                    #region 0x61 触发模式
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
                    #endregion

                    case 0x98:
                        break;

                    #region 0x60 工作模式
                    case 0x60:                                                          //工作模式
                        if (e.Msg.Data[1] == 0x01)
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = true;
                        else if (e.Msg.Data[1] == 0x02)
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = true;
                        else if (e.Msg.Data[1] == 0x03)
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = true;

                        if (GlobalInfo.Instance.IsMotorXSetWorkModeOk && GlobalInfo.Instance.IsMotorWSetWorkModeOk && GlobalInfo.Instance.IsMotorZSetWorkModeOk)
                        {
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkModeOk;
                            GlobalInfo.Instance.CurrentWorkType = Enum_MotorWorkType.Position;
                        }
                        break;
                    #endregion

                    #region 0x7a 设置目标位置
                    case 0x7a:                                                      ///设置目标位置
                        if (e.Msg.Data[1] == 0x01)
                            GlobalInfo.Instance.IsMotorXSetTargetPositionOk = true;
                        else if (e.Msg.Data[1] == 0x02)
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = true;
                        else if (e.Msg.Data[1] == 0x03)
                        {
                            GlobalInfo.Instance.IsMotorZSetTargetPositionOk = true;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPositionOk;
                        }
                        if (GlobalInfo.Instance.IsMotorWSetTargetPositionOk && GlobalInfo.Instance.IsMotorXSetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                        {
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPositionOk;

                        }
                        break;
                    #endregion

                    #region 0x40 执行 3f
                    case 0x40:                                                  ///执行   3f                         
                        //int returnPositionX = 0;
                        //int returnPositionW = 0;
                        if (e.Msg.Data[1] == 0x01)
                        {
                            GlobalInfo.Instance.IsMotorXActionOk = true;
                            byte[] returnBytesX = new byte[4];
                            returnBytesX[0] = e.Msg.Data[2];
                            returnBytesX[1] = e.Msg.Data[3];
                            returnBytesX[2] = e.Msg.Data[4];
                            returnBytesX[3] = e.Msg.Data[5];
                            GlobalInfo.returnPositionX = BitConverter.ToInt32(returnBytesX, 0);
                            //MainLogHelper.Instance.Info("移动完成后返回的位置（含Zero）：" + "X---" + GlobalInfo.returnPositionX * GlobalInfo.XLengthPerCircle / 3600.0 + "\n" 
                            //    +"(不含Zero)："+ "X---" + (GlobalInfo.returnPositionX - GlobalInfo.Instance.TrayPanelHomeX) * GlobalInfo.XLengthPerCircle / 3600.0 + "\n"
                            //    + "转换成十六进制："+ Convert.ToString(GlobalInfo.returnPositionX,16).ToUpper().PadLeft(8,'0'));

                            //GlobalInfo.Instance.Totalab_LSerials.ReadMotorPosition((byte)0x01);
                        }
                        else if (e.Msg.Data[1] == 0x02)
                        {
                            GlobalInfo.Instance.IsMotorWActionOk = true;
                            byte[] returnBytesW = new byte[4];
                            returnBytesW[0] = e.Msg.Data[2];
                            returnBytesW[1] = e.Msg.Data[3];
                            returnBytesW[2] = e.Msg.Data[4];
                            returnBytesW[3] = e.Msg.Data[5];
                            GlobalInfo.returnPositionW = BitConverter.ToInt32(returnBytesW, 0);
                            //MainLogHelper.Instance.Info("移动完成后返回的位置（含Zero）：" + "W---" + GlobalInfo.returnPositionW / 60.0 + "\n"
                            //    + "(不含Zero)：" + "W---" + (GlobalInfo.returnPositionW - GlobalInfo.Instance.TrayPanelHomeW) / 60.0 + "\n"
                            //    + "转换成十六进制：" + Convert.ToString(GlobalInfo.returnPositionW, 16).ToUpper().PadLeft(8, '0'));

                            //GlobalInfo.Instance.Totalab_LSerials.ReadMotorPosition((byte)0x02);
                        }
                        else if (e.Msg.Data[1] == 0x03)
                        {
                            GlobalInfo.Uplift = true;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorActionOk;
                            if (GlobalInfo.status && !GlobalInfo.calibration_status && !GlobalInfo.Instance.SettingInfo.IsLeakage_tankOpen)
                            {
                                GlobalInfo.status = false;
                                GlobalInfo.Instance.Totalab_LSerials.SetLeakage_tank(0x00);     //关闭漏液槽
                            }
                        }
                        if (GlobalInfo.Instance.IsMotorWActionOk && GlobalInfo.Instance.IsMotorXActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorActionOk;

                        }
                        break;
                    #endregion

                    #region 0x41 使能，急停，清除错误    0f
                    case 0x41:
                        if (e.Msg.Data[2] == 0x80)          //清除错误
                        {
                            if (e.Msg.Data[1] == 0x01)
                                GlobalInfo.Instance.IsMotorXError = false;
                            else if (e.Msg.Data[1] == 0x02)
                                GlobalInfo.Instance.IsMotorWError = false;
                            else if (e.Msg.Data[1] == 0x03)
                                GlobalInfo.Instance.IsMotorZError = false;
                            if (GlobalInfo.Instance.IsMotorXError == false && GlobalInfo.Instance.IsMotorWError == false && GlobalInfo.Instance.IsMotorZError == false)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    StatusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF64DD17"));
                                    StatusText = "Normal";
                                }));
                            }
                        }
                        else if (e.Msg.Data[2] == 0x0B)             //急停
                        {
                            if (e.Msg.Data[1] == 0x01)
                                GlobalInfo.Instance.IsMotorXStop = true;
                            else if (e.Msg.Data[1] == 0x02)
                                GlobalInfo.Instance.IsMotorWStop = true;
                            else if (e.Msg.Data[1] == 0x03)
                                GlobalInfo.Instance.IsMotorZStop = true;
                        }
                        else                                      //使能0f
                        {
                            if (e.Msg.Data[1] == 0x01)
                                GlobalInfo.Instance.IsMotorXActionOk = true;
                            else if (e.Msg.Data[1] == 0x02)
                                GlobalInfo.Instance.IsMotorWActionOk = true;
                            else if (e.Msg.Data[1] == 0x03)
                            {
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorActionOk;
                            }
                            if (GlobalInfo.Instance.IsMotorWActionOk && GlobalInfo.Instance.IsMotorXActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                            {
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorActionOk;
                            }
                        }
                        break;
                    #endregion

                    #region 0x64 当前实际位置读取
                    case 0x64:                                                  //当前实际位置读取
                        if (_IsFirst)
                        {
                            _IsFirst = false;
                            //GlobalInfo.Instance.TrayPanelCenter = (e.Msg.Data[5] * 16777216 + e.Msg.Data[4] * 65536 + e.Msg.Data[3] * 256 + e.Msg.Data[2]) / 3600.0 * GlobalInfo.XLengthPerCircle - GlobalInfo.Instance.TrayPanelHomeX;
                            GlobalInfo.Instance.IsCanRunning = true;
                            GlobalInfo.Instance.IsBusy = false;

                            //string SavePath = "";
                            //SavePath = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.ini");
                            //byte[] content_write = XmlObjSerializer.Serialize(GlobalInfo.Instance.CalibrationInfo);
                            //bool result = FileHelper.WriteEncrypt(SavePath, content_write);
                            ////if (result == true)
                            ////    new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                            ////else
                            ////    new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);


                            byte[] content = FileHelper.ReadDecrypt(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.ini"));
                            if (content != null)
                            {
                                GlobalInfo.Instance.CalibrationInfo = XmlObjSerializer.Deserialize<TrayPanelCalibrationInfo>(content);
                                GlobalInfo.Instance.TrayPanelCenter = GlobalInfo.Instance.CalibrationInfo.TrayPanelCenterX;
                                GlobalInfo.Instance.TrayPanel_leftW = GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW;
                                GlobalInfo.Instance.TrayPanel_rightW = GlobalInfo.Instance.CalibrationInfo.CalibrationRightW;
                                MainLogHelper.Instance.Info("读取配置文件中心点位置：X：" + GlobalInfo.Instance.TrayPanelCenter + "\n" +
                                                                            "Left_point:" + GlobalInfo.Instance.TrayPanel_leftW + " \n" +
                                                                            "Right_point:" + GlobalInfo.Instance.TrayPanel_rightW);

                                //GoToXYCommand_linshi();
                            }
                            else
                            {
                                new MessagePage().ShowDialog("Message_CalibrationError".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                                GlobalInfo.Instance.TrayPanelCenter = GlobalInfo.Instance.CalibrationInfo.TrayPanelCenterX;
                                GlobalInfo.Instance.TrayPanel_leftW = GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW;
                                GlobalInfo.Instance.TrayPanel_rightW = GlobalInfo.Instance.CalibrationInfo.CalibrationRightW;
                            }
                        }
                        //if (!_IsFirst && e.Msg.Data[1] == 0x01)
                        //{
                        //    byte[] bytes = new byte[4];
                        //    bytes[0] = e.Msg.Data[2];
                        //    bytes[1] = e.Msg.Data[3];
                        //    bytes[2] = e.Msg.Data[4];
                        //    bytes[3] = e.Msg.Data[5];
                        //    GlobalInfo.Instance.PositionX = BitConverter.ToInt32(bytes, 0);
                        //    MainLogHelper.Instance.Info("重新读取当前实际X位置：" + GlobalInfo.Instance.PositionX * GlobalInfo.XLengthPerCircle / 3600.0);

                        //}
                        //if (!_IsFirst && e.Msg.Data[1] == 0x02)
                        //{
                        //    byte[] bytes = new byte[4];
                        //    bytes[0] = e.Msg.Data[2];
                        //    bytes[1] = e.Msg.Data[3];
                        //    bytes[2] = e.Msg.Data[4];
                        //    bytes[3] = e.Msg.Data[5];
                        //    GlobalInfo.Instance.PositionW = BitConverter.ToInt32(bytes, 0);
                        //    MainLogHelper.Instance.Info("重新读取当前实际W位置：" + GlobalInfo.Instance.PositionW / 60);

                        //}

                        break;
                    #endregion

                    #region 0x22 系统初始化返回
                    case 0x22:
                        if (e.Msg.DataLength == 12)
                        {
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHomeOk;
                            //if (_IsFirst)
                            //{
                            //GlobalInfo.Instance.TrayPanelHomeX = (e.Msg.Data[3] * 16777216 + e.Msg.Data[2] * 65536 + e.Msg.Data[1] * 256 + e.Msg.Data[0]) / 3600.0 * GlobalInfo.XLengthPerCircle;
                            //GlobalInfo.Instance.TrayPanelHomeW = e.Msg.Data[7] * 16777216 + e.Msg.Data[6] * 65536 + e.Msg.Data[5] * 256 + e.Msg.Data[4];
                            //GlobalInfo.Instance.TrayPanelHomeZ = (e.Msg.Data[11] * 16777216 + e.Msg.Data[10] * 65536 + e.Msg.Data[9] * 256 + e.Msg.Data[8]) / 3600.0 * GlobalInfo.ZLengthPerCircle;
                            GlobalInfo.Instance.TrayPanelHomeX = e.Msg.Data[3] * 16777216 + e.Msg.Data[2] * 65536 + e.Msg.Data[1] * 256 + e.Msg.Data[0];
                            GlobalInfo.Instance.TrayPanelHomeW = e.Msg.Data[7] * 16777216 + e.Msg.Data[6] * 65536 + e.Msg.Data[5] * 256 + e.Msg.Data[4];
                            GlobalInfo.Instance.TrayPanelHomeZ = e.Msg.Data[11] * 16777216 + e.Msg.Data[10] * 65536 + e.Msg.Data[9] * 256 + e.Msg.Data[8];

                            MainLogHelper.Instance.Info("--X：" + GlobalInfo.Instance.TrayPanelHomeX + "--W：" + GlobalInfo.Instance.TrayPanelHomeW + "--Z：" + GlobalInfo.Instance.TrayPanelHomeZ);
                            //GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            //Thread.Sleep(300);
                            //GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            //Thread.Sleep(300);
                            //}
                            GlobalInfo.Instance.IsBusy = false;
                            GlobalInfo.Instance.IsCanRunning = true;

                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.ReadMotorPosition((byte)0x01);
                        }
                        else if (e.Msg.DataLength == 1)
                        {
                            if (e.Msg.Data[0] == 0xee)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    new MessagePage().ShowDialog("Message_InitializeError".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                                }));
                            }
                        }
                        break;
                    #endregion

                    #region  0xee 报错类别
                    case 0xee:
                        if (e.Msg.Data[1] == 0x01)
                        {
                            GlobalInfo.Instance.IsMotorXError = true;
                            if (e.Msg.Data[2] == 0xe3)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "X轴位置超限");
                                }));
                            }
                            else if (e.Msg.Data[2] == 0xe6)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "X轴追随错误");
                                }));
                            }
                        }
                        else if (e.Msg.Data[1] == 0x02)
                        {
                            GlobalInfo.Instance.IsMotorWError = true;
                            if (e.Msg.Data[2] == 0xe3)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "T轴位置超限");
                                }));
                            }
                            else if (e.Msg.Data[2] == 0xe6)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "T轴追随错误");
                                }));
                            }

                        }
                        else if (e.Msg.Data[1] == 0x03)
                        {
                            GlobalInfo.Instance.IsMotorZError = true;
                            if (e.Msg.Data[2] == 0xe3)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Z轴位置超限");
                                }));
                            }
                            else if (e.Msg.Data[2] == 0xe6)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Z轴追随错误");
                                }));
                            }
                        }

                        if (e.Msg.Data[2] == 0xe1)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "复位出错");
                            }));
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.Error;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            StatusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF44336"));
                            StatusText = "Error";
                        }));
                        CancellationTokenSource source = new CancellationTokenSource();
                        Task.Factory.StartNew(() =>
                        {
                            GlobalInfo.Instance.Totalab_LSerials.SetLightStatus(0x00, 0x07);
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.SetLightStatus(0x02, 0x03);
                            Thread.Sleep(100);
                            if (Control_SampleListView.IsRunningFlow)
                                Control_SampleListView.ThreadStop();
                            else
                            {
                                bool result = MotorActionHelper.MotorErrorStopImmediately();
                                if (result == false)
                                {
                                    ConntectWaring();
                                }
                            }
                        }, source.Token);
                        break;
                    #endregion

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [Sampler_MsgCome]：", ex);
            }
        }
        public void ConntectWaring()
        {
            try
            {
                if (GlobalInfo.IsAgainPower)
                {
                    GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                    GlobalInfo.IsAgainPower = false;
                }
                else if (GlobalInfo.IsDisconnected == false)
                {
                    if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            StatusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBDBDBD"));
                            StatusText = "D/C";
                        }));
                        if (GlobalInfo.Instance.IsMotorXError)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "X轴出错");
                            }));
                        }
                        if (GlobalInfo.Instance.IsMotorWError)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "T轴出错");
                            }));
                        }
                        if (GlobalInfo.Instance.IsMotorZError)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Z轴出错");
                            }));
                        }
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            bool? ErrorResult = new MessagePage().ShowDialog("Message_RunError".GetWord(), "MessageTitle_Error".GetWord(), true, Enum_MessageType.Error, yesContent: "Message_ButtonOK".GetWord());
                            if ((bool)ErrorResult)
                            {
                            //清错
                            MotorActionHelper.MotorClearError();
                            }
                        }));
                    }
                }
                else
                {
                    GlobalInfo.Instance.IsBusy = true;
                    GlobalInfo.Instance.IsCanRunning = false;

                    IsConnect = false;
                    if (!GlobalInfo.Instance.IsHimassConnState)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            new MessagePage().ShowDialog("Message_Error2013".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                        }));
                    }
                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                    if (GlobalInfo.Instance.IsHimassConnState)
                    {
                        MainWindow_AutoSamplerSendObjectDataEvent(null,
                                      new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = IsConnect });
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            new MessagePage().ShowDialog("Message_Error2013".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                        }));

                    }
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        StatusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBDBDBD"));
                        StatusText = "D/C";
                    }));
                    Thread threadConnect1 = new Thread(ConnectStatus);
                    threadConnect1.Start();
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ConntectWaring：", ex);
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
                        //MainLogHelper.Instance.Info("进样器发送：" + samInfo.SamName + "[" + samInfo.SamID + "]" + samInfo.Location + samInfo.OperationMode + samInfo.IsAnalyze + samInfo.AnalysisType + samInfo.NextSamID);
                        //Trace.WriteLine("进样器发送：" + samInfo.SamName + "[" + samInfo.SamID + "]" + samInfo.Location + samInfo.OperationMode + samInfo.IsAnalyze + samInfo.AnalysisType + samInfo.NextSamID);
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
            if (msgArg.MessParamType == EnumMessParamType.UseAutoSampler)
            {
                MainLogHelper.Instance.Info($"[PublisherAutoSamplerSendObjectDataEvent IsUseAutoSampler ={IsUseAutoSampler}");
            }
        }
        public void MainWindow_AutoSamplerSendObjectDataEvent(object sender, ObjectEventArgs e)
        {
            PublisherAutoSamplerSendObjectDataEvent(e);
            //SubscribeMassSendObjectEvent(sender, e);
        }
        #endregion
        /// <summary>
        /// 连接Mass时接收/发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msgArg"></param>
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
                                SampleLoc = samInfo.Location,
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
                            SampleHelper.SetCircleStatus((int)samplingInfo.SampleLoc, Item_Status.Ready);
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
                            SampleHelper.SetCircleStatus((int)samplingInfo.SampleLoc, Item_Status.Free);
                            Control_SampleListView.RefreshSampleNum();
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Update)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo.SampleLoc != samInfo.Location)
                            {
                                SampleHelper.SetCircleStatus(samInfo.Location, Item_Status.Ready);
                                if (GlobalInfo.Instance.SampleInfos.Where(item => item.SampleLoc == samplingInfo.SampleLoc).Count() == 1)
                                {
                                    SampleHelper.SetCircleStatus((int)samplingInfo.SampleLoc, Item_Status.Free);
                                }
                            }
                            if (samplingInfo != null)
                            {
                                samplingInfo.IsChecked = samInfo.IsAnalyze;
                                samplingInfo.SampleName = samInfo.SamName;
                                samplingInfo.SampleLoc = samInfo.Location;
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
                        else if (samInfo.OperationMode == EnumSamOperationMode.StartInjection)          //分析
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
                                if (samplingInfo.SampleLoc > GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                                {

                                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                                    AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                    {
                                        SamID = samplingInfo.SampleGuid,
                                        SamName = samplingInfo.SampleName,
                                        Location = samplingInfo.SampleLoc.Value,
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
                                    Application.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        new MessagePage().ShowDialog("Message_PositionError".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Information);
                                    }));
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
                        else if (samInfo.OperationMode == EnumSamOperationMode.StartAutoTuning)         //调谐
                        {
                            //MainLogHelper.Instance.Info(DateTime.Now.ToString("hh:mm:ss") + "到达位置-超出" + samInfo.Location + "," + GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber);
                            GlobalInfo.Instance.IsCanRunning = false;
                            if (samInfo.Location <= GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                            {
                                int location = samInfo.Location;
                                GlobalInfo.Instance.IsBusy = true;
                                Control_SampleListView.ExcuteGotoTargetLocation(location, samInfo.SamID);           //调谐
                                //MainLogHelper.Instance.Info(DateTime.Now.ToString("hh:mm:ss") + "hyx2" + location);
                            }
                            else
                            {
                                GlobalInfo.IsLoctionError = true;               //位置超限了
                                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                                AutoSampler_SamInfo samplingtempinfo = new AutoSampler_SamInfo();
                                //samplingtempinfo.SamName = "";
                                //samplingtempinfo.Location = 0;
                                //samplingtempinfo.IsAnalyze = false;
                                //samplingtempinfo.OverWash = 0;
                                samplingtempinfo.AnalysisType = Enum_AnalysisType.AutoTune;
                                samplingtempinfo.SamID = samInfo.SamID;
                                samplingtempinfo.OperationMode = EnumSamOperationMode.Error;
                                MainLogHelper.Instance.Info(DateTime.Now.ToString("hh:mm:ss") + "到达位置-超出");
                                list.Add(samplingtempinfo);
                                MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                {
                                    SamInfoList = list
                                });
                                Application.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    new MessagePage().ShowDialog("Message_PositionError".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Information);
                                }));
                            }
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
                                SampleHelper.SetCircleStatus((int)samplingInfo.SampleLoc, Item_Status.Ready);
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
                            if (!GlobalInfo.IsAutoTuning)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "Mass开始清洗");
                                }));
                            }
                            else
                            {
                                Control_SampleListView.ExcuteWash(samInfo.SamID, samInfo.TotalWashTime);
                            }
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
                            Control_SampleListView.PauseImmediatelyCommand(null, null);
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
                        else if (samInfo.OperationMode == EnumSamOperationMode.Error)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Error;
                                    //GlobalInfo.Instance.SampleInfos[GlobalInfo.Instance.SampleInfos.IndexOf(samplingInfo)].ExpStatus = Exp_Status.Error;
                                }));
                                SampleHelper.SetCircleStatus((int)samplingInfo.SampleLoc, Item_Status.Error);
                                if (GlobalInfo.Instance.IsBusy)
                                {
                                    Control_SampleListView.IsMassWashStart = true;
                                    Control_SampleListView.IsAutoSamplingWashFinish = true;
                                    Control_SampleListView.IsMassWashFinish = true;
                                    if (GlobalInfo.Instance.IsMotorXError == false && GlobalInfo.Instance.IsMotorWError == false && GlobalInfo.Instance.IsMotorZError == false)
                                    {
                                        Control_SampleListView.ErrorWash();
                                        MainLogHelper.Instance.Error("清洗错误ErrorWash：");
                                    }
                                }
                            }
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Complete)
                        {
                            SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleGuid == samInfo.SamID).FirstOrDefault();
                            if (samplingInfo != null)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    samplingInfo.ExpStatus = Exp_Status.Complete;
                                }));
                                SampleHelper.SetCircleStatus((int)samplingInfo.SampleLoc, Item_Status.Complete);

                            }
                        }
                        else if (samInfo.OperationMode == EnumSamOperationMode.Ongoing)
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
                        else if (samInfo.OperationMode == EnumSamOperationMode.Terminated)
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
                        //MainLogHelper.Instance.Info(string.Format("Mass发送：Name:[{0}] Mode:[{1}] Location:[{2}] Type:[{3}] ID:[{4}]", samInfo.SamName, samInfo.OperationMode, samInfo.Location, samInfo.AnalysisType, samInfo.SamID));
                        //Trace.WriteLine(string.Format("Name:[{0}] Mode:[{1}] Location:[{2}] Type:[{3}] ID:[{4}]", samInfo.SamName, samInfo.OperationMode, samInfo.Location, samInfo.AnalysisType, samInfo.SamID));
                    }
                }
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("MainWindow [SubscribeMassSendSamDataEvent]：", e);

            }
        }
        [EventSubscription("topic://AutoSampler_MassSendObjectData", typeof(Publisher))]

        /// <summary>
        /// 连接Mass时接收处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msgArg"></param>
        public void SubscribeMassSendObjectEvent(object sender, ObjectEventArgs msgArg)
        {
            try
            {
                if (msgArg != null)
                {
                    MainLogHelper.Instance.Info(string.Format("Auto Sampler ObjectData Type:[{0}]", msgArg.MessParamType));
                    Trace.WriteLine(string.Format("Auto Sampler ObjectData Type:[{0}]", msgArg.MessParamType));

                    //更新是否使用自动进样器的状态消息
                    if (msgArg.MessParamType == EnumMessParamType.UseAutoSampler && msgArg.Parameter is bool useState)
                    {
                        IsUseAutoSampler = useState;
                        if (IsUseAutoSampler)
                        {
                            //CommonSampleMethod.CreateSampleInfos(500);
                            GlobalInfo.Instance.SampleInfos = new ObservableCollection<SampleItemInfo>();
                            //SampleHelper.CreateSampleInfos(1);                          //运行显示25
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "自动进样器使用");
                            }));
                            ////勾选进样器后复位初始化
                            //GlobalInfo.Instance.IsBusy = true;
                            //GlobalInfo.Instance.Totalab_LSerials.XWZHome();           //系统复位初始化

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
                        Control_SampleListView.IsStopWash = false;
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
                        //MainLogHelper.Instance.Info(DateTime.Now.ToString("hh:mm:ss") + "立刻停止");
                        Control_SampleListView.StopMethod();
                        //else if (msgArg.MessParamType == EnumMessParamType.ChlPump_LoadFinish)
                        //    IsQuickSamplerLoadFinish = true;
                        //else if (msgArg.MessParamType == EnumMessParamType.ChlPump_SamWashEnd)
                        //    IsQuickSamplerWashFinish = true;
                    }
                    else if (msgArg.MessParamType == EnumMessParamType.UseAutoSampler_Enable && msgArg.Parameter is bool useEnable)
                    {
                        IsAutoSamplerEnable = useEnable;
                    }
                    else if (msgArg.MessParamType == EnumMessParamType.StartAutoTuningTask)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "开始自动调谐");
                        }));
                        GlobalInfo.Instance.IsBusy = true;              //新增11-30
                        _IsTestConnection = false;
                        GlobalInfo.IsAutoTuning = true;
                        Control_SampleListView.stopType = 0;
                        Control_SampleListView.CurrnentLoc = "";
                        Control_SampleListView.IsStopWash = false;
                        GlobalInfo.IsStopOptimization = false;              //为了在电点击停止优化的时候更及时的停止进样器
                    }
                    else if (msgArg.MessParamType == EnumMessParamType.StopAutoTuningTask)
                    {
                        GlobalInfo.IsStopOptimization = true;               //停止优化
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "自动调谐完成");
                        }));
                        //_IsTestConnection = true;
                        if (GlobalInfo.IsLoctionError == false)
                        {
                            GlobalInfo.IsAutoTuning = false;
                            Control_SampleListView.AutoTuningStop(Guid.Empty);
                        }
                        GlobalInfo.Instance.IsCanRunning = false;
                    }
                    else if (msgArg.MessParamType == EnumMessParamType.CurrentLanguage)
                    {
                        //this.Dispatcher.Invoke((Action)(() =>
                        //{
                        //    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "语言切换");
                        //}));
                        //MainLogHelper.Instance.Error(string.Format("语言切换"));
                        List<string> languageList = new List<string>
                        {
                            "/Totalab_L;component/Themes/AutoSampler_"
                        };
                        if (msgArg.Parameter is Enum_Languages.Chinese)
                        {
                            GlobalInfo.Imagevisibility = true;
                            ResourceDictionaryHelper.SwitchLanguage(Enum_Languages.Chinese, languageList);
                            MainLogHelper.Instance.Error("语言切换Chinese");
                            //GlobalInfo.LgDictionary = Application.LoadComponent(new Uri(@"/Totalab_L;component/Themes/AutoSampler_Chinese.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
                            //Application.Current.Resources.MergedDictionaries.Remove(GlobalInfo.LgDictionary);
                            //Application.Current.Resources.MergedDictionaries.Add(GlobalInfo.LgDictionary);
                        }
                        else if(msgArg.Parameter is Enum_Languages.English)
                        {
                            GlobalInfo.Imagevisibility = false;
                            ResourceDictionaryHelper.SwitchLanguage(Enum_Languages.English, languageList);
                            MainLogHelper.Instance.Error("语言切换English");
                        }
                        //Enum.TryParse(runLanguage, out Enum_Languages language)
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
                        MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerTrayFile, Parameter = GetTrayInfoString() });
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

        public string GetTrayInfoString()
        {
            try
            {
                string str = string.Empty;
                str = GlobalInfo.Instance.TrayAInfos.TrayType + "," + GlobalInfo.Instance.TrayBInfos.TrayType + "," + GlobalInfo.Instance.TrayCleanInfos.TrayType
                    + "," + GlobalInfo.Instance.TrayDInfos.TrayType + "," + GlobalInfo.Instance.TrayEInfos.TrayType;
                return str;

            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ShellPage [GetTrayInfoString]：", ex);
                return null;
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


        #region 临时测试
        private void GoToXYCommand_linshi()
        {
            try
            {
                //Control_ParentView.IsSamplerManual = true;
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
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                        }
                        Point pt = new Point();
                        int isCollisionStatus = 0;
                        pt.X = (227+ GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600.0;
                        pt.Y = GlobalInfo.Instance.TrayPanelHomeW + 10708;
                        MainLogHelper.Instance.Error("X的位置zero：" + GlobalInfo.Instance.TrayPanelHomeX + "Y的位置zero：" + GlobalInfo.Instance.TrayPanelHomeW);
                        MainLogHelper.Instance.Error("X的位置："+ pt.X+ "Y的位置：" + pt.Y );
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
                            Thread.Sleep(300);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                            {
                                break;
                            }
                            else
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                        }

                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        if (isCollisionStatus == 0)
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
                            Thread.Sleep(300);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                            {
                                break;
                            }
                            else
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
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
                            Thread.Sleep(300);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                            {
                                break;
                            }
                            else
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
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SettingModule [GoToXYCommand]", ex);
                    }
                    finally
                    {
                        //Control_ParentView.IsSamplerManual = false;
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }

                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SettingModule [GoToXYCommand]", ex); }
            //IsCanZ = true;

        }

        #endregion
        //移动去清洗位
        public void MoveToWashW1Command(object sender, RoutedEventArgs e)
        {
            CurrentPosition = "W1";
            MoveToPositionCommand(null,null);
        }
    }
}