using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totalab_L.Enum;

namespace Totalab_L.Models
{
    public class TrayMechanicalData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ///架子类型
        public string  TrayType
        {
            get => _trayType;
            set
            {
                _trayType = value;
                Notify("TrayType");
            }
        }
        private string _trayType;

        public double XCenterDistance
        {
            get => _XCenterDistance;
            set
            {
                _XCenterDistance = value;
                Notify("XCenterDistance");
            }
        }
        private double _XCenterDistance;

        public double YCenterDistance
        {
            get => _YCenterDistance;
            set
            {
                _YCenterDistance = value;
                Notify("YCenterDistance");
            }
        }
        private double _YCenterDistance;
    }
}
