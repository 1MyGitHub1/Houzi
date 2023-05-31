using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Models
{
    public class AnalysInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int WashTime
        {
            get => _washTime;
            set
            {
                _washTime = value;
                Notify("WashTime");
            }
        }
        private int _washTime = 60;

        public double WashPumpSpeed
        {
            get => _washPumpSpeed;
            set
            {
                _washPumpSpeed = value;
                Notify("WashPumpSpeed");
            }
        }
        private double _washPumpSpeed = 40;

        public string WashLoc
        {
            get => _washLoc;
            set
            {
                _washLoc = value;
                Notify("WashLoc");
            }
        }
        private string _washLoc = "SampleLoc";

        public int WashTimeTypeIndex
        {
            get => _washTimeTypeIndex;
            set
            {
                _washTimeTypeIndex = value;
                Notify("WashTimeTypeIndex");
            }
        }
        private int _washTimeTypeIndex = 1;
        //public int WashSpeedTypeIndex
        //{
        //    get => _washSpeedTypeIndex;
        //    set => Set(ref _washSpeedTypeIndex, value);
        //}
        //private int _washSpeedTypeIndex = 1;
    }
}
