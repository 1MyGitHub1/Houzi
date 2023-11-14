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


        #region 校准点位
        public double TrayPanelCenterX
        {
            get => _trayPanelCenterX;
            set
            {
                _trayPanelCenterX = value;
                Notify("TrayPanelCenterX");
            }
        }
        private double _trayPanelCenterX;

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
        //左校准点实际位置到中心点距离
        public double TrayCenterToLeftW
        {
            get => _trayCenterToLeftW;
            set
            {
                _trayCenterToLeftW = value;
                Notify("TrayCenterToLeftW");
            }
        }
        private double _trayCenterToLeftW;
        #endregion

        #region 复位返回的中心点位置及Zero
        public double Return_TrayPanelCenterX
        {
            get => _return_TrayPanelCenterX;
            set
            {
                _return_TrayPanelCenterX = value;
                Notify("Return_TrayPanelCenterX");
            }
        }
        private double _return_TrayPanelCenterX;
        public double Return_TrayPanelCenterW
        {
            get => _return_TrayPanelCenterW;
            set
            {
                _return_TrayPanelCenterW = value;
                Notify("Return_TrayPanelCenterW");
            }
        }
        private double _return_TrayPanelCenterW;

        #endregion

        //进样针向左时的中心位置
        public double XCalibrationPosition_left
        {
            get => _xCalibrationPosition_left;
            set
            {
                _xCalibrationPosition_left = value;
                Notify("XCalibrationPosition_left");
            }
        }
        private double _xCalibrationPosition_left;
        //进样针向左时的中心位置
        public double XCalibrationPosition_right
        {
            get => _xCalibrationPosition_right;
            set
            {
                _xCalibrationPosition_right = value;
                Notify("XCalibrationPosition_right");
            }
        }
        private double _xCalibrationPosition_right;
        //X轴中心位置
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

        //角度中心值
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
        /// <summary>
        /// 移动位置
        /// </summary>
        public string PosNumber
        {
            get => _posNumber;
            set
            {
                _posNumber = value;
                Notify("PosNumber");
            }
        }
        private string _posNumber = "60";

        /// <summary>
        /// 进样针复位位置
        /// </summary>
        public double ZResetPosition
        {
            get => _zResetPosition;
            set
            {
                _zResetPosition = value;
                Notify("ZResetPosition");
            }
        }
        private double _zResetPosition = 0;

        #region 校准清洗位置
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
        #endregion



        #region 偏移量
        //平移偏移量左边
        public double OffsetValueLeftX
        {
            get => _offsetValueLeftX;
            set
            {
                _offsetValueLeftX = value;
                Notify("OffsetValueLeftX");
            }
        }
        private double _offsetValueLeftX;
        //偏移角度左边
        public double OffsetCalibrationLeftW
        {
            get => _offsetCalibrationLeftW;
            set
            {
                _offsetCalibrationLeftW = value;
                Notify("OffsetCalibrationLeftW");
            }
        }
        private double _offsetCalibrationLeftW;
        //平移偏移量右边
        public double OffsetValueRightX
        {
            get => _offsetValueRightX;
            set
            {
                _offsetValueRightX = value;
                Notify("OffsetValueRightX");
            }
        }
        private double _offsetValueRightX;
        //偏移角度右边
        public double OffsetCalibrationRightW
        {
            get => _offsetCalibrationRightW;
            set
            {
                _offsetCalibrationRightW = value;
                Notify("OffsetCalibrationRightW");
            }
        }
        private double _offsetCalibrationRightW;
        #endregion
    }
}
