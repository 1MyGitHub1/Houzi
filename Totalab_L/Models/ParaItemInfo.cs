using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Models
{
    public class ParaItemInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string StepName
        {
            get => _stepName;
            set
            {
                _stepName = value;
                Notify("StepName");
            }
        }
        private string _stepName;


        public string WashAction
        {
            get => _washAction;
            set
            {
                _washAction = value;
                Notify("WashAction");
            }
        }
        private string _washAction;

        public int WashActionKey
        {
            get => _washActionKey;
            set
            {
                _washActionKey = value;
                Notify("WashActionKey");
            }
        }
        private int _washActionKey = 0;

        public int WashTime
        {
            get => _washTime;
            set
            {
                _washTime = value;
                Notify("WashTime");
            }
        }
        private int _washTime;

        public double WashPumpSpeed
        {
            get => _washPumpSpeed;
            set
            {
                _washPumpSpeed = value;
                Notify("WashPumpSpeed");
            }
        }
        private double _washPumpSpeed;

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

        public bool IsCanDelete
        {
            get => _isCanDelete;
            set
            {
                _isCanDelete = value;
                Notify("IsCanDelete");
            }
        }
        private bool _isCanDelete;
    }
}
