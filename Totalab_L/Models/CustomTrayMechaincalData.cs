using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Models
{
    public class CustomTrayMechaincalData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ///架子类型
        public string TrayType
        {
            get => _trayType;
            set
            {
                _trayType = value;
                Notify("TrayType");
            }
        }
        private string _trayType;

        public ObservableCollection<double> RowToCenterXList
        {
            get => _rowToCenterXList;
            set
            {
                _rowToCenterXList = value;
                Notify("RowToCenterXList");
            }
        }
        private ObservableCollection<double> _rowToCenterXList = new ObservableCollection<double>();

        public double XCenterInterval
        {
            get => _xCenterInterval;
            set
            {
                _xCenterInterval = value;
                Notify("XCenterInterval");
            }
        }
        private double _xCenterInterval;

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
    }
}
