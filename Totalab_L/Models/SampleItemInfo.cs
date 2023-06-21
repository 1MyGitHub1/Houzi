using Totalab_L.Enum;
using DeviceInterface;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Totalab_L.Models
{
   public class SampleItemInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Guid SampleGuid
        {
            get => _sampleGuid;
            set
            {
                _sampleGuid = value;
                Notify("SampleGuid");
            }
        }
        public Guid _sampleGuid;

        public int? SampleNum
        {
            get => _sampleNum;
            set
            {
                _sampleNum = value;
                Notify("SampleNum");
            }
        }
        private int? _sampleNum;


        public string SampleName
        {
            get => _sampleName;
            set
            {
                _sampleName = value;
                Notify("SampleName");
            }
        }
        private string _sampleName;

        public string PreSampleName
        {
            get => _preSampleName;
            set
            {
                _preSampleName = value;
                Notify("PreSampleName");
            }
        }
        private string _preSampleName;

        public int? SampleLoc
        {
            get => _sampleLoc;
            set
            {
                _sampleLoc = value;
                Notify("SampleLoc");
            }
        }
        private int? _sampleLoc;


        public int? PreSampleLoc
        {
            get => _preSampleLoc;
            set
            {
                _preSampleLoc = value;
                Notify("PreSampleLoc");
            }
        }
        private int? _preSampleLoc;

        public long? Overwash
        {
            get => _overWash;
            set
            {
                _overWash = value;
                Notify("Overwash");
            }
        }
        private long? _overWash;

        public long? PreOverwash
        {
            get => _preOverWash;
            set
            {
                _preOverWash = value;
                Notify("PreOverwash");
            }
        }
        private long? _preOverWash;

        public Exp_Status ExpStatus
        {
            get => _expStatus;
            set
            {
                _expStatus = value;
                Notify("ExpStatus");
            }
        }
        public Exp_Status _expStatus = Exp_Status.Free;

        public Exp_Status PreExpStatus
        {
            get => _preExpStatus;
            set
            {
                _preExpStatus = value;
                Notify("PreExpStatus");
            }
        }
        public Exp_Status _preExpStatus = Exp_Status.Free;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                Notify("IsChecked");
            }
        }
        private bool _isChecked = true;

        public Enum_AnalysisType? MethodType
        {
            get => _methodType;
            set
            {
                _methodType = value;
                Notify("MethodType");
            }
        }
        private Enum_AnalysisType? _methodType = Enum_AnalysisType.Quantitative;

        public Enum_AnalysisType? PreMethodType { get; set; }



    }
}
