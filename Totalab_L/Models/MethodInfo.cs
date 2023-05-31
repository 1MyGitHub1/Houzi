using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Models
{
   public  class MethodInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public SettingInfo MethodSettingInfo
        {
            get => _methodSettingInfo;
            set
            {
                _methodSettingInfo = value;
                Notify("MethodSettingInfo");
            }
        }
        private SettingInfo _methodSettingInfo = new SettingInfo();


        public string MethodName
        {
            get => _methodName;
            set
            {
                _methodName = value;
                Notify("MethodName");
            }
        }
        private string _methodName;

        public List<SampleItemInfo> SampleInfos
        {
            get => _sampleInfos;
            set
            {
                _sampleInfos = value;
                Notify("SampleInfos");
            }
        }
        private List<SampleItemInfo> _sampleInfos;
    }
}
