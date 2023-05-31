using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Totalab_L.Models
{
    public class PreWashItemInfo : INotifyPropertyChanged
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


        public bool IsOpenAction
        {
            get => _isOpenAction;
            set
            {
                _isOpenAction = value;
                Notify("IsOpenAction");
            }
        }
        private bool _isOpenAction;

        public Visibility ActionSwitchVisibility
        {
            get => _actionSwitchVisibility;
            set
            {
                _actionSwitchVisibility = value;
                Notify("ActionSwitchVisibility");
            }
        }
        private Visibility _actionSwitchVisibility;
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
        private string _washLoc = "1";
    }
}
