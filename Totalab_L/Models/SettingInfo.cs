using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Models
{
   public  class SettingInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int SamplingDepth
        {
            get => _samplingDepth;
            set
            {
                _samplingDepth = value;
                Notify("SamplingDepth");
            }
        }
        private int _samplingDepth = 100;

        public int Wash1Time
        {
            get => _wash1Time;
            set
            {
                _wash1Time = value;
                Notify("Wash1Time");
            }
        }
        private int _wash1Time = 10;

        public int PumpSpeed1
        {
            get => _pumpSpeed1;
            set
            {
                _pumpSpeed1 = value;
                Notify("PumpSpeed1");
            }
        }
        private int _pumpSpeed1 = 20;


        public int Wash2Time
        {
            get => _wash2Time;
            set
            {
                _wash2Time = value;
                Notify("Wash2Time");
            }
        }
        private int _wash2Time = 10;

        public int PumpSpeed2
        {
            get => _pumpSpeed2;
            set
            {
                _pumpSpeed2 = value;
                Notify("PumpSpeed2");
            }
        }
        private int _pumpSpeed2 = 20;
        ///<summary>
        ///预冲洗信息
        ///</summary>
        public ObservableCollection<PreWashItemInfo> PreWashInfos
        {
            get => _preWashInfos;
            set
            {
                _preWashInfos = value;
                Notify("PreWashInfos");
            }
        }
        private ObservableCollection<PreWashItemInfo> _preWashInfos = new ObservableCollection<PreWashItemInfo>
        {
            //new PreWashItemInfo
            //{
            //    StepName = "预冲洗",
            //    IsOpenAction = true,
            //},
        };

        ///<summary>
        /// 预运行信息
        ///</summary>
        public ObservableCollection<AnalysInfo> PreRunningInfo
        {
            get => _preRunningInfo;
            set
            {
                _preRunningInfo = value;
                Notify("PreRunningInfo");
            }
        }
        private ObservableCollection<AnalysInfo> _preRunningInfo = new ObservableCollection<AnalysInfo>
        {
            // new AnalysInfo
            //{
            //   WashSpeedTypeIndex = 1,
            //   WashPumpSpeed = 40,
            //   WashTimeTypeIndex = 1,
            //   WashTime = 10
            //},
            // new AnalysInfo
            //{
            //    WashSpeedTypeIndex = 1,
            //   WashPumpSpeed = 40,
            //   WashTimeTypeIndex = 1,
            //   WashTime = 10
            //},
        };

        ///<summary>
        ///后冲洗信息
        ///</summary>
        public ObservableCollection<ParaItemInfo> AfterRunningInfo
        {
            get => _afterRunningInfo;
            set
            {
                _afterRunningInfo = value;
                Notify("AfterRunningInfo");
            }
        }
        private ObservableCollection<ParaItemInfo> _afterRunningInfo = new ObservableCollection<ParaItemInfo>
        {
            //   new ParaItemInfo
            //{
            //    StepName = "后运行",
            //    WashAction="进样针冲洗(样品)",
            //},
            // new ParaItemInfo
            //{
            //    WashAction="进样针冲洗(标准)",
            //},
        };


        public AnalysInfo AnalysInfo
        {
            get => _analysInfo;
            set
            {
                _analysInfo = value;
                Notify("AnalysInfo");
            }
        }
        private AnalysInfo _analysInfo = new AnalysInfo();

        public int SignalType
        {
            get => _signalType;
            set
            {
                _signalType = value;
                Notify("SignalType");
            }
        }
        private int _signalType;

        public int SignalMode
        {
            get => _signalMode;
            set
            {
                _signalMode = value;
                Notify("SignalMode");
            }
        }
        private int _signalMode = 3;

        public int AnalysMode
        {
            get => _analysMode;
            set
            {
                _analysMode = value;
                Notify("AnalysMode");
            }
        }
        private int _analysMode = 3;

        public bool IsWash1Open
        {
            get => _isWash1Open;
            set
            {
                _isWash1Open = value;
                Notify("IsWash1Open");
            }
        }
        private bool _isWash1Open = true;



        public bool IsWash2Open
        {
            get => _isWash2Open;
            set
            {
                _isWash2Open = value;
                Notify("IsWash2Open");
            }
        }
        private bool _isWash2Open;
        //漏液盘是否常开
        public bool IsLeakage_tankOpen
        {
            get => _isLeakage_tankOpen;
            set
            {
                _isLeakage_tankOpen = value;
                Notify("IsLeakage_tankOpen");
            }
        }
        private bool _isLeakage_tankOpen;
    }
}
