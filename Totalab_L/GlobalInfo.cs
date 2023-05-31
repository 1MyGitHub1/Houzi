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


        //public AnalysInfo AnalysInfo
        //{
        //    get => _analysInfo;
        //    set => Set(ref _analysInfo, value);
        //}
        //private AnalysInfo _analysInfo = new AnalysInfo();

        public SamperPosInfo SamplerPos
        {
            get => _samplerPos;
            set
            {
                this._samplerPos = value;
                NotifyPropertyChanged("SamplerPos");
            }
        }
        private SamperPosInfo _samplerPos = new SamperPosInfo();

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

        public int XStep
        {
            get => _xStep;
            set
            {
                this._xStep = value;
                NotifyPropertyChanged("XStep");
            }
        }
        private int _xStep;

        public int YStep
        {
            get => _yStep;
            set
            {
                this._yStep = value;
                NotifyPropertyChanged("YStep");
            }
        }
        private int _yStep;

        public ObservableCollection<ItemData> CircleItemList
        {
            get => this._circleItemList;
            set
            {
                this._circleItemList = value;
                NotifyPropertyChanged("CircleItemList");
            }
        }
        private ObservableCollection<ItemData> _circleItemList = new ObservableCollection<ItemData>();

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

        public Item_Status Sample2ItemStatus
        {
            get => _sample2ItemStatus;
            set
            {
                this._sample2ItemStatus = value;
                NotifyPropertyChanged("Sample2ItemStatus");
            }
        }
        private Item_Status _sample2ItemStatus = Item_Status.Free;

        public Item_Status Sample1ItemStatus
        {
            get => _sample1ItemStatus;
            set
            {
                this._sample1ItemStatus = value;
                NotifyPropertyChanged("Sample1ItemStatus");
            }
        }
        private Item_Status _sample1ItemStatus = Item_Status.Free;

        public Item_Status Sample3ItemStatus
        {
            get => _sample3ItemStatus;
            set
            {
                this._sample3ItemStatus = value;
                NotifyPropertyChanged("Sample3ItemStatus");
            }
        }
        private Item_Status _sample3ItemStatus = Item_Status.Free;

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


        ///架子的数据信息列表
        public ObservableCollection<TrayMechanicalData> TrayDataList
        {
            get => _trayDataList;
            set
            {
                _trayDataList = value;
                NotifyPropertyChanged("TrayDataList");
            }
        }
        private ObservableCollection<TrayMechanicalData> _trayDataList = new ObservableCollection<TrayMechanicalData>();

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


        public int STD2TrayWidth
        {
            get => _std2TrayWidth;
            set
            {
                _std2TrayWidth = value;
                NotifyPropertyChanged("STD2TrayWidth");
            }
        }
        private int _std2TrayWidth;

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
        #endregion
    }
}
