using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Totalab_L.Models
{
    public class TrayPanelCalibrationInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public double XCalibrationPosition
        {
            get => _xCalibrationPosition;
            set
            {
                _xCalibrationPosition = value;
                Notify("XCalibrationPosition");
            }
        }
        private double _xCalibrationPosition;

        public double WCalibrationPosition
        {
            get => _wCalibrationPosition;
            set
            {
                _wCalibrationPosition = value;
                Notify("WCalibrationPosition");
            }
        }
        private double _wCalibrationPosition;

        public double ZResetPosition
        {
            get => _zResetPosition;
            set
            {
                _zResetPosition = value;
                Notify("ZResetPosition");
            }
        }
        private double _zResetPosition = 20;

        public double W1PointX
        {
            get => _w1PointX;
            set
            {
                _w1PointX = value;
                Notify("W1PointX");
            }
        }
        private double _w1PointX;
        public double W1PointY
        {
            get => _w1PointY;
            set
            {
                _w1PointY = value;
                Notify("W1PointY");
            }
        }
        private double _w1PointY;

        public double W2PointX
        {
            get => _w2PointX;
            set
            {
                _w2PointX = value;
                Notify("W2PointX");
            }
        }
        private double _w2PointX;
        public double W2PointY
        {
            get => _w2PointY;
            set
            {
                _w2PointY = value;
                Notify("W2PointY");
            }
        }
        private double _w2PointY;

        public double CalibrationLeftX
        {
            get => _calibrationLeftX;
            set
            {
                _calibrationLeftX = value;
                Notify("CalibrationLeftX");
            }
        }
        private double _calibrationLeftX;

        public double CalibrationLeftW
        {
            get => _calibrationLeftW;
            set
            {
                _calibrationLeftW = value;
                Notify("CalibrationLeftW");
            }
        }
        private double _calibrationLeftW;

        public double CalibrationRightX
        {
            get => _calibrationRightX;
            set
            {
                _calibrationRightX = value;
                Notify("CalibrationRightX");
            }
        }
        private double _calibrationRightX;

        public double CalibrationRightW
        {
            get => _calibrationRightW;
            set
            {
                _calibrationRightW = value;
                Notify("CalibrationRightW");
            }
        }
        private double _calibrationRightW;
    }
}
