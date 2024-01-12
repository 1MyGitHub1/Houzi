using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totalab_L.Serials;
using Totalab_L.Models;
using Totalab_L.Enum;
using LabTech.Common;
using System.Windows;

namespace Totalab_L
{
    public class GlobalInfo : ModelBaseData
    {
        public static GlobalInfo Instance
        {
            get { return _instance; }
        }
        private static readonly GlobalInfo _instance = new GlobalInfo();

        public const double XLengthPerCircle = 31.9966;                 //56.5487;///X齿轮转一圈的长度
        public const double ZLengthPerCircle = 31.9966;                 //48;///Z齿轮转一圈的长度

        public static bool IsAutoTuning = false;
        public static bool IsStopOptimization = false;                  //调谐--是否点了停止优化

        public static double Zlength = 1;                   //进样针下降深度
        public static double ZPut_up = 0;                   //进样针抬起高度

        public static bool Uplift = false;

        public static bool status = false;                  //漏液槽状态--复位后如果要关闭就置为true
        public static bool calibration_status=false;            //校准页漏液槽不需要关

        public static int returnPositionX = 0;              //移动后返回的值
        public static int returnPositionW = 0;

        public static double deviation_angle = 0;           //补偿角度
        public static string MethodName;                    //设置界面打开的方法名称
        public static double LastPositionX = 0;              //上一个位置X
        public static double LastPositionW = 0;              //上一个位置W

        public static bool IsLoctionError = false;            //输入的样品位置超限
        public static bool IsAgainPower = false;             //是否重新上电
        public static bool IsDisconnected = true;               //是否断连接

        public static ResourceDictionary LgDictionary = new ResourceDictionary();

        ///<summary>
        ///自动进样器通信接口
        ///</summary>
        public SamplerSerials Totalab_LSerials
        {
            get => _totalabSerials;
            set
            {
                this._totalabSerials = value;
                NotifyPropertyChanged("Totalab_LSerials");
            }
        }
        private SamplerSerials _totalabSerials = new SamplerSerials();


        ///<summary>
        ///自动进样器样品信息
        ///</summary>
        public ObservableCollection<SampleItemInfo> SampleInfos
        {
            get => _sampleInfos;
            set
            {
                this._sampleInfos = value;
                NotifyPropertyChanged("SampleInfos");
            }
        }
        private ObservableCollection<SampleItemInfo> _sampleInfos;


        ///<summary>
        ///是否和mass连接
        ///</summary>
        public bool IsHimassConnState
        {
            get => _isHimassConnState;
            set
            {
                this._isHimassConnState = value;
                NotifyPropertyChanged("IsHimassConnState");
            }
        }
        private bool _isHimassConnState = false;


        public MethodInfo CurrentMethod
        {
            get => _currentMethod;
            set
            {
                this._currentMethod = value;
                NotifyPropertyChanged("CurrentMethod");
            }
        }
        private MethodInfo _currentMethod = new MethodInfo();
        ///// <summary>
        ///// 设置界面的方法名称
        ///// </summary>
        //public string Method_Name
        //{
        //    get => _method_Name;
        //    set
        //    {
        //        _method_Name = value;
        //        NotifyPropertyChanged("Method_Name");
        //    }

        //}
        //private string _method_Name;

        public SettingInfo SettingInfo
        {
            get => _settingInfo;
            set
            {
                this._settingInfo = value;
                NotifyPropertyChanged("SettingInfo");
            }
        }
        private SettingInfo _settingInfo = new SettingInfo();


        public RunningStep_Status RunningStep
        {
            get => _runningStep;
            set
            {
                this._runningStep = value;
                NotifyPropertyChanged("RunningStep");
            }
        }
        private RunningStep_Status _runningStep;

        public ObservableCollection<string> LogInfo
        {
            get => _logInfo;
            set
            {
                this._logInfo = value;
                NotifyPropertyChanged("LogInfo");
            }
        }
        private ObservableCollection<string> _logInfo = new ObservableCollection<string>();

        public bool IsCanRunning
        {
            get => _isCanRunning;
            set
            {
                this._isCanRunning = value;
                NotifyPropertyChanged("IsCanRunning");
            }
        }
        private bool _isCanRunning = true;
        //当前是否正在忙碌
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                this._isBusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }
        private bool _isBusy;
        //public int SamplingDepth
        //{
        //    get => _samplingDepth;
        //    set => Set(ref _samplingDepth, value);
        //}
        //private int _samplingDepth;

        public bool IsReadMCUOk
        {
            get { return _isReadMCUOk; }
            set
            {
                _isReadMCUOk = value;
                NotifyPropertyChanged("IsReadMCUOk");
            }
        }
        private bool _isReadMCUOk;

        public bool IsWriteMCUOk
        {
            get { return _isWriteMCUOk; }
            set
            {
                _isWriteMCUOk = value;
                NotifyPropertyChanged("IsWriteMCUOk");
            }
        }
        private bool _isWriteMCUOk;

        public byte[] MCUData
        {
            get { return _MCUData; }
            set
            {
                _MCUData = value;
                NotifyPropertyChanged("MCUData");
            }
        }
        private byte[] _MCUData;
        
        ////
        ///最大联机测试次数
        ///
        public int MaxConnectionTimes
        {
            get { return _maxConnectionTimes; }
            set
            {
                _maxConnectionTimes = value;
                NotifyPropertyChanged("MaxConnectionTimes");
            }
        }
        private int _maxConnectionTimes;

        #region 样品架信息

        #region 五个区域
        public TrayInfo TrayAInfos
        {
            get => _trayAInfos;

            set
            {
                _trayAInfos = value;
                NotifyPropertyChanged("TrayAInfos");
            }
        }
        private TrayInfo _trayAInfos = new TrayInfo();

        public TrayInfo TrayBInfos
        {
            get => _trayBInfos;

            set
            {
                _trayBInfos = value;
                NotifyPropertyChanged("TrayBInfos");
            }
        }
        private TrayInfo _trayBInfos = new TrayInfo();

        public TrayInfo TrayCleanInfos
        {
            get => _trayCleanInfos;

            set
            {
                _trayCleanInfos = value;
                NotifyPropertyChanged("TrayCleanInfos");
            }
        }
        private TrayInfo _trayCleanInfos = new TrayInfo();

        public TrayInfo TraySTD1Infos
        {
            get => _traySTD1Infos;

            set
            {
                _traySTD1Infos = value;
                NotifyPropertyChanged("TraySTD1Infos");
            }
        }
        private TrayInfo _traySTD1Infos = new TrayInfo();


        public TrayInfo TraySTD2Infos
        {
            get => _traySTD2Infos;

            set
            {
                _traySTD2Infos = value;
                NotifyPropertyChanged("TraySTD2Infos");
            }
        }
        private TrayInfo _traySTD2Infos = new TrayInfo();

        public TrayInfo TrayDInfos
        {
            get => _trayDInfos;

            set
            {
                _trayDInfos = value;
                NotifyPropertyChanged("TrayDInfos");
            }
        }
        private TrayInfo _trayDInfos = new TrayInfo();
        public TrayInfo TrayEInfos
        {
            get => _trayEInfos;

            set
            {
                _trayEInfos = value;
                NotifyPropertyChanged("TrayEInfos");
            }
        }
        private TrayInfo _trayEInfos = new TrayInfo();

        #endregion

        ///架子的数据信息列表
        public List<TrayMechanicalData> TrayDataList
        {
            get => _trayDataList;
            set
            {
                _trayDataList = value;
                NotifyPropertyChanged("TrayDataList");
            }
        }
        private List<TrayMechanicalData> _trayDataList = new List<TrayMechanicalData>();

        public List<CustomTrayMechaincalData> StdTrayDataList
        {
            get => _stdTrayDataList;
            set
            {
                _stdTrayDataList = value;
                NotifyPropertyChanged("StdTrayDataList");
            }
        }
        private List<CustomTrayMechaincalData> _stdTrayDataList = new List<CustomTrayMechaincalData>();


        public int STD1TrayHeight
        {
            get => _std1TrayHeight;
            set
            {
                _std1TrayHeight = value;
                NotifyPropertyChanged("STD1TrayHeight");
            }
        }
        private int _std1TrayHeight;

        public int STD2TrayHeight
        {
            get => _std2TrayHeight;
            set
            {
                _std2TrayHeight = value;
                NotifyPropertyChanged("STD2TrayHeight");
            }
        }
        private int _std2TrayHeight;


        public int STD1TrayWidth
        {
            get => _std1TrayWidth;
            set
            {
                _std1TrayWidth = value;
                NotifyPropertyChanged("STD1TrayWidth");
            }
        }
        private int _std1TrayWidth;

        public Thickness CleanTrayMargin
        {
            get => _cleanTrayMargin;
            set
            {
                _cleanTrayMargin = value;
                NotifyPropertyChanged("CleanTrayMargin");
            }
        }
        private Thickness _cleanTrayMargin;

        public Enum_MotorWorkType CurrentWorkType
        {
            get => _currentWorkType;
            set
            {
                _currentWorkType = value;
                NotifyPropertyChanged("CurrentWorkType");
            }
        }
        private Enum_MotorWorkType _currentWorkType;

        public bool IsMotorXSetWorkModeOk
        {
            get => _isMotorXSetWorkModeOk;
            set
            {
                _isMotorXSetWorkModeOk = value;
                NotifyPropertyChanged("IsMotorXSetWorkModeOk");
            }
        }
        private bool _isMotorXSetWorkModeOk;

        public bool IsMotorWSetWorkModeOk
        {
            get => _isMotorWSetWorkModeOk;
            set
            {
                _isMotorWSetWorkModeOk = value;
                NotifyPropertyChanged("IsMotorWSetWorkModeOk");
            }
        }
        private bool _isMotorWSetWorkModeOk;
        public bool IsMotorZSetWorkModeOk
        {
            get => _isMotorZSetWorkModeOk;
            set
            {
                _isMotorZSetWorkModeOk = value;
                NotifyPropertyChanged("IsMotorZSetWorkModeOk");
            }
        }
        private bool _isMotorZSetWorkModeOk;

        public bool IsMotorXMoveOk
        {
            get => _isMotorXMoveOk;
            set
            {
                _isMotorXMoveOk = value;
                NotifyPropertyChanged("IsMotorXMoveOk");
            }
        }
        private bool _isMotorXMoveOk;


        public bool IsMotorWMoveOk
        {
            get => _isMotorWMoveOk;
            set
            {
                _isMotorWMoveOk = value;
                NotifyPropertyChanged("IsMotorWMoveOk");
            }
        }
        private bool _isMotorWMoveOk;

        public bool IsMotorXSetTargetPositionOk
        {
            get => _isMotorXSetTargetPositionOk;
            set
            {
                _isMotorXSetTargetPositionOk = value;
                NotifyPropertyChanged("IsMotorXSetTargetPositionOk");
            }
        }
        private bool _isMotorXSetTargetPositionOk;

        public bool IsMotorWSetTargetPositionOk
        {
            get => _isMotorWSetTargetPositionOk;
            set
            {
                _isMotorWSetTargetPositionOk = value;
                NotifyPropertyChanged("IsMotorWSetTargetPositionOk");
            }
        }
        private bool _isMotorWSetTargetPositionOk;

        public bool IsMotorZSetTargetPositionOk
        {
            get => _isMotorZSetTargetPositionOk;
            set
            {
                _isMotorZSetTargetPositionOk = value;
                NotifyPropertyChanged("IsMotorZSetTargetPositionOk");
            }
        }
        private bool _isMotorZSetTargetPositionOk;

        public bool IsMotorXActionOk
        {
            get => _isMotorXActionOk;
            set
            {
                _isMotorXActionOk = value;
                NotifyPropertyChanged("IsMotorXActionOk");
            }
        }
        private bool _isMotorXActionOk;

        public bool IsMotorWActionOk
        {
            get => _isMotorWActionOk;
            set
            {
                _isMotorWActionOk = value;
                NotifyPropertyChanged("IsMotorWActionOk");
            }
        }
        private bool _isMotorWActionOk;
        public bool IsMotorZActionOk
        {
            get => _isMotorZActionOk;
            set
            {
                _isMotorZActionOk = value;
                NotifyPropertyChanged("IsMotorZActionOk");
            }
        }
        private bool _isMotorZActionOk;

        public bool IsMotorXError
        {
            get => _isMotorXError;
            set
            {
                _isMotorXError = value;
                NotifyPropertyChanged("IsMotorXError");
            }
        }
        private bool _isMotorXError;

        public bool IsMotorWError
        {
            get => _isMotorWError;
            set
            {
                _isMotorWError = value;
                NotifyPropertyChanged("IsMotorWError");
            }
        }
        private bool _isMotorWError;

        public bool IsMotorZError
        {
            get => _isMotorZError;
            set
            {
                _isMotorZError = value;
                NotifyPropertyChanged("IsMotorZError");
            }
        }
        private bool _isMotorZError;

        public bool IsMotorXStop
        {
            get => _isMotorXStop;
            set
            {
                _isMotorXStop = value;
                NotifyPropertyChanged("IsMotorXStop");
            }
        }
        private bool _isMotorXStop;

        public bool IsMotorWStop
        {
            get => _isMotorWStop;
            set
            {
                _isMotorWStop = value;
                NotifyPropertyChanged("IsMotorWStop");
            }
        }
        private bool _isMotorWStop;

        public bool IsMotorZStop
        {
            get => _isMotorZStop;
            set
            {
                _isMotorZStop = value;
                NotifyPropertyChanged("IsMotorZStop");
            }
        }
        private bool _isMotorZStop;

        public TrayPanelCalibrationInfo CalibrationInfo
        {
            get => _calibrationInfo;
            set
            {
                _calibrationInfo = value;
                NotifyPropertyChanged("CalibrationInfo");
            }
        }
        private TrayPanelCalibrationInfo _calibrationInfo = new TrayPanelCalibrationInfo();
        #endregion

        ///架子中心坐标
        public double TrayPanelCenter
        {
            get => _trayPanelCenter;
            set
            {
                _trayPanelCenter = value;
                NotifyPropertyChanged("TrayPanelCenter");
            }
        }
        private double _trayPanelCenter;
        ///架子中心坐标左hyx
        public double TrayPanelCenter_left
        {
            get => _trayPanelCenter_left;
            set
            {
                _trayPanelCenter_left = value;
                NotifyPropertyChanged("TrayPanelCenter_left");
            }
        }
        private double _trayPanelCenter_left;
        ///架子中心坐标右hyx
        public double TrayPanelCenter_right
        {
            get => _trayPanelCenter_right;
            set
            {
                _trayPanelCenter_right = value;
                NotifyPropertyChanged("TrayPanelCenter_right");
            }
        }
        private double _trayPanelCenter_right;

        ///角度左hyx
        public double TrayPanel_leftW
        {
            get => _trayPanel_leftW;
            set
            {
                _trayPanel_leftW = value;
                NotifyPropertyChanged("TrayPanel_leftW");
            }
        }
        private double _trayPanel_leftW;
        ///角度右hyx
        public double TrayPanel_rightW
        {
            get => _trayPanel_rightW;
            set
            {
                _trayPanel_rightW = value;
                NotifyPropertyChanged("TrayPanel_rightW");
            }
        }
        private double _trayPanel_rightW;


        //X轴的实际初始位置(0位置前需要走的距离)
        public double TrayPanelHomeX
        {
            get => _trayPanelHomeX;
            set
            {
                _trayPanelHomeX = value;
                NotifyPropertyChanged("TrayPanelHomeX");
            }

        }
        private double _trayPanelHomeX;

        //角度的实际初始位置
        public double TrayPanelHomeW
        {
            get => _trayPanelHomeW;
            set
            {
                _trayPanelHomeW = value;
                NotifyPropertyChanged("TrayPanelHomeW");
            }

        }
        private double _trayPanelHomeW;

        //针的实际初始位置
        public double TrayPanelHomeZ
        {
            get => _trayPanelHomeZ;
            set
            {
                _trayPanelHomeZ = value;
                NotifyPropertyChanged("TrayPanelHomeZ");
            }

        }
        private double _trayPanelHomeZ;

        public double PositionX
        {
            get => _positionX;
            set
            {
                _positionX = value;
                NotifyPropertyChanged("PositionX");
            }

        }
        private double _positionX;
        public double PositionW
        {
            get => _positionW;
            set
            {
                _positionW = value;
                NotifyPropertyChanged("PositionW");
            }

        }
        private double _positionW;

        /// <summary>
        /// 更换语言包
        /// </summary>
        public static void UpdateLanguage(string Language)
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedLanguage = string.Format(@"/Common/{0}.xaml", Language);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
            if (resourceDictionary == null)
            {
                requestedLanguage = @"/Common/zh-cn.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
            }
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

    }
}
