﻿using Totalab_L.Models;
using LabTech.Common;
using LabTech.UITheme;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Totalab_L.Common;
using System.Threading;
using Totalab_L.Enum;
using DeviceInterface;

namespace Totalab_L
{
    /// <summary>
    /// SamplerPosSetPage.xaml 的交互逻辑
    /// </summary>
    public partial class SamplerPosSetPage : CustomWindow, INotifyPropertyChanged
    {
        public SamplerPosSetPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 属性
        /// <summary>
        /// 进样针下降位置
        /// </summary>
        public double ZCurrentPosition
        {
            get => _zCurrentPosition;
            set
            {
                _zCurrentPosition = value;
                Notify("ZCurrentPosition");
            }
        }
        private double _zCurrentPosition = 100;

        public TrayPanelCalibrationInfo CalibrationInfo
        {
            get => _calibrationInfo;
            set
            {
                _calibrationInfo = value;
                Notify("CalibrationInfo");
            }
        }
        private TrayPanelCalibrationInfo _calibrationInfo = new TrayPanelCalibrationInfo();
        public bool IsCloseWin
        {
            get => _isCloseWin;
            set
            {
                _isCloseWin = value;
                Notify("IsCloseWin");
            }
        }
        private bool _isCloseWin = false;

        public ShellPage Control_Shell
        {
            get => _control_Shell;
            set
            {
                _control_Shell = value;
                Notify("Control_Shell");
            }
        }
        private ShellPage _control_Shell;

        public bool IsConnect
        {
            get => _isConnect;
            set
            {
                _isConnect = value;
                Notify("IsConnect");
            }
        }
        private bool _isConnect;


        //实时显示
        public double CalibrationLeftShowX
        {
            get => _calibrationLeftShowX;
            set
            {
                _calibrationLeftShowX = value;
                Notify("CalibrationLeftShowX");
            }
        }
        private double _calibrationLeftShowX;
        public double CalibrationLeftShowW
        {
            get => _calibrationLeftShowW;
            set
            {
                _calibrationLeftShowW = value;
                Notify("CalibrationLeftShowW");
            }
        }
        private double _calibrationLeftShowW;


        #endregion

        #region 事件

        #region 未使用
        private void MoveToXCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        long longseconds = DateTime.Now.Ticks / 10000;
                        int count = 0;
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = true;
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((CalibrationInfo.XCalibrationPosition + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((CalibrationInfo.XCalibrationPosition + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = true;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = true;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }
                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }

                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("ShellPage [MoveToPositionCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToXCommand]", ex); }
        }

        private void ResetXCommand(object sender, RoutedEventArgs e)
        {

            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        long longseconds = DateTime.Now.Ticks / 10000;
                        int count = 0;
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = true;
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)(GlobalInfo.Instance.TrayPanelHomeX / GlobalInfo.XLengthPerCircle * 3600));
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((CalibrationInfo.XCalibrationPosition + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = true;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = true;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }
                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }

                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("ShellPage [MoveToPositionCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToXCommand]", ex); }
        }

        private void MoveToWCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                long longseconds = 0;
                int count = 0;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = true;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)(CalibrationInfo.WCalibrationPosition * 10 * 6));
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)(CalibrationInfo.WCalibrationPosition * 10 * 6));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = true;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = true;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToXCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToXCommand]", ex); }
        }

        private void ResetWCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                long longseconds = 0;
                int count = 0;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = true;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)GlobalInfo.Instance.TrayPanelHomeW);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)GlobalInfo.Instance.TrayPanelHomeW);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = true;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = true;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToPositionCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToXCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToXCommand]", ex); }
        }

        #endregion



        //进样针上升/下降
        private void MoveToZCommand(object sender, RoutedEventArgs e)
        {
            try
            {

                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                long longseconds = 0;
                int count = 0;
                Button btn = sender as Button;
                //double Zlength = 0;
                if (btn.Tag.ToString() == "ResetZ")
                {
                    GlobalInfo.status = true;       //关
                    GlobalInfo.Zlength = CalibrationInfo.ZResetPosition;
                }
                else
                {
                    GlobalInfo.status = false;
                    GlobalInfo.Zlength = ZCurrentPosition;
                }
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        GlobalInfo.Instance.Totalab_LSerials.SetLeakage_tank(0x14);     //打开
                        Thread.Sleep(100);

                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Zlength) / GlobalInfo.ZLengthPerCircle * 3600));
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Zlength) / GlobalInfo.ZLengthPerCircle * 3600));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToZCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }

                        //if (btn.Tag.ToString() == "ResetZ")
                        //{
                        //GlobalInfo.Instance.Totalab_LSerials.SetLeakage_tank(0x00);     //打开

                        //}
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SamplerSetPage [MoveToZCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerSetPage [MoveToZCommand]", ex); }
        }
        //限位按钮
        private void ResetZCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                long longseconds = 0;
                int count = 0;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        GlobalInfo.Instance.Totalab_LSerials.SetLeakage_tank(0x14);     //打开
                        Thread.Sleep(100);

                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.status = true;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + CalibrationInfo.ZResetPosition) / GlobalInfo.ZLengthPerCircle * 3600));
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.status = true;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + CalibrationInfo.ZResetPosition) / GlobalInfo.ZLengthPerCircle * 3600));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[ResetZCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SamplerPosSetPage [ResetZCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [ResetZCommand]", ex); }
        }
        //保存校准值
        public void SavePosCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                //GlobalInfo.Instance.TrayPanelCenter = (CalibrationInfo.CalibrationRightX - CalibrationInfo.CalibrationLeftX) / 2;
                //GlobalInfo.Instance.TrayPanelCenter_left = (CalibrationInfo.CalibrationRightX - CalibrationInfo.CalibrationLeftX) / 2 + CalibrationInfo.OffsetValueLeftX;
                //GlobalInfo.Instance.TrayPanelCenter_right = (CalibrationInfo.CalibrationRightX - CalibrationInfo.CalibrationLeftX) / 2 + CalibrationInfo.OffsetValueRightX;
                //GlobalInfo.Instance.TrayPanelHomeW = (CalibrationInfo.CalibrationRightW + CalibrationInfo.CalibrationLeftW) / 2 * 10.0 * 6.0;
                #region 复位返回的值
                //GlobalInfo.Instance.CalibrationInfo.Return_TrayPanelCenterX = 25540 + GlobalInfo.Instance.TrayPanelHomeX / GlobalInfo.XLengthPerCircle * 3600;
                //GlobalInfo.Instance.CalibrationInfo.Return_TrayPanelCenterW = 10708 + GlobalInfo.Instance.TrayPanelHomeW;
                //GlobalInfo.Instance.CalibrationInfo.ZResetPosition = CalibrationInfo.ZResetPosition;

                #endregion
                //Zero不能存
                GlobalInfo.Instance.CalibrationInfo.CalibrationLeftX = CalibrationInfo.CalibrationLeftX;
                GlobalInfo.Instance.CalibrationInfo.CalibrationRightX = CalibrationInfo.CalibrationLeftX + 167.5 * 2;
                GlobalInfo.Instance.CalibrationInfo.CalibrationLeftW = CalibrationInfo.CalibrationLeftW;
                GlobalInfo.Instance.CalibrationInfo.CalibrationRightW = CalibrationInfo.CalibrationLeftW + 180;
                //X中心和左边角度
                GlobalInfo.Instance.CalibrationInfo.TrayPanelCenterX = CalibrationInfo.CalibrationLeftX + 167.5;
                GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW = CalibrationInfo.CalibrationLeftW;

                //GlobalInfo.Instance.CalibrationInfo.TrayPanelCenterX = GlobalInfo.returnPositionW * GlobalInfo.XLengthPerCircle / 3600 + (167.5 - 120.75) - GlobalInfo.Instance.TrayPanelHomeX;
                //GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW = (CalibrationLeftShowW - GlobalInfo.Instance.TrayPanelHomeW) / 60;
                GlobalInfo.Instance.TrayPanelCenter = CalibrationInfo.CalibrationLeftX + 167.5;
                GlobalInfo.Instance.TrayPanel_leftW = CalibrationInfo.CalibrationLeftW;
                GlobalInfo.Instance.TrayPanel_rightW = CalibrationInfo.CalibrationLeftW + 180;

                //GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftPoint = 167.5 - 120.75;

                string SavePath = "";
                SavePath = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.ini");
                byte[] content = XmlObjSerializer.Serialize(GlobalInfo.Instance.CalibrationInfo);
                bool result = FileHelper.WriteEncrypt(SavePath, content);
                if (result == true)
                    new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information, this);
                else
                    new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[SamplerPosSetPage]SavePosCommand：", ex);
                new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }
        //移动到清洗位置W1/W2
        private void MoveToWashCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                long longseconds = 0;
                int count = 0;
                Button btn = sender as Button;
                Point pt = new Point();
                if (btn.Tag.ToString() == "W1")
                {
                    pt = new Point((CalibrationInfo.W1PointX + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600.0, CalibrationInfo.W1PointY * 6.0 * 10.0);
                }
                else
                {
                    pt = new Point((CalibrationInfo.W2PointX + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600.0, CalibrationInfo.W2PointY * 6.0 * 10.0);
                }
                MainLogHelper.Instance.Info(btn.Tag.ToString()+ "发送位置：(" + pt.X +"," + pt.Y + ")");

                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                    Thread.Sleep(300);
                                                    GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        //Point pt = GetPositionInfoHelper.GetWashPosition("W2");
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                        Thread.Sleep(300);
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                                                Thread.Sleep(300);
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        Thread.Sleep(300);
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                Thread.Sleep(300);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        Thread.Sleep(500);
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                Thread.Sleep(100);
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                Thread.Sleep(500);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MoveToWashCommand]：", ex);
                                    }

                                    count++;
                                    continue;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {

                                return;
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToWashCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [MoveToWashCommand]", ex); }
        }
        //复位
        public void HomeCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHome;
                        GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                        //int count = 0;
                        //while (true)
                        //{
                        //    Thread.Sleep(100);
                        //    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.XYZHomeOk)
                        //    {
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        long longseconds = DateTime.Now.Ticks / 10000;
                        //        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.XYZHomeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //        {
                        //            Thread.Sleep(100);
                        //        }
                        //        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.XYZHomeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        //        {
                        //            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        //            {
                        //                //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                        //                try
                        //                {
                        //                    //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                        //                    {
                        //                        try
                        //                        {
                        //                            //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                        //                            //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                        //                            GlobalInfo.Instance.RunningStep = RunningStep_Status.XYZHome;
                        //                            GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                        //                        }
                        //                        catch (Exception ex)
                        //                        {
                        //                            MainLogHelper.Instance.Error("HomeCommand]：", ex);
                        //                        }
                        //                    }

                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    MainLogHelper.Instance.Error("[HomeCommand]：", ex);
                        //                }
                        //                count++;
                        //                continue;
                        //            }
                        //            else
                        //            {
                        //                ConntectWaring();
                        //                return;
                        //            }

                        //        }
                        //        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                        //        {

                        //            return;
                        //        }
                        //        else
                        //            break;
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SamplerPosSetPage [HomeCommand]", ex);
                    }
                    finally
                    {
                        Thread.Sleep(300);
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [HomeCommand]", ex); }
        }
        private void WindowCloseCommand(object sender, CancelEventArgs e)
        {
            if (!IsCloseWin)
            {
                e.Cancel = true;
                if (GlobalInfo.Instance.IsCanRunning == false)
                {
                    new MessagePage().ShowDialog("仪器正在运行中，请稍后关闭窗口".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information);
                    return;
                }
                e.Cancel = false;
            }

        }
        //移动到当前设置位置
        private void MoveToTargetLocationCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Button btn = sender as Button;
                string str = btn.Tag.ToString();
                bool result = false;
                Task.Factory.StartNew(() =>
                {
                    if (str == "right")
                    {
                        MainLogHelper.Instance.Info("右边校准点位置信息：" + "(" + CalibrationInfo.CalibrationRightX + "," + CalibrationInfo.CalibrationRightW + ")");
                        result = MotorActionHelper.MotorMoveToTargetPosition(CalibrationInfo.CalibrationLeftX + 167.5 * 2 - 120.75, CalibrationInfo.CalibrationLeftW + 180);
                    }
                    if (str == "left")
                    {
                        MainLogHelper.Instance.Info("左边校准点位置信息：" + "(" + CalibrationInfo.CalibrationLeftX + "," + CalibrationInfo.CalibrationLeftW + ")");
                        result = MotorActionHelper.MotorMoveToTargetPosition(CalibrationInfo.CalibrationLeftX + 120.75, CalibrationInfo.CalibrationLeftW);
                    }
                    if (result == false)
                    {
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        {
                            ConntectWaring();
                        }
                    }
                    CalibrationLeftShowX = GlobalInfo.returnPositionX * GlobalInfo.XLengthPerCircle / 3600.0 - GlobalInfo.Instance.TrayPanelHomeX;
                    CalibrationLeftShowW = (GlobalInfo.returnPositionW - GlobalInfo.Instance.TrayPanelHomeW) / 60;
                    GlobalInfo.Instance.IsBusy = false;
                    GlobalInfo.Instance.IsCanRunning = true;
                    source?.Cancel();
                    source?.Dispose();

                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [HomeCommand]", ex); }
        }
        #endregion

        #region 方法
        //public bool? ShowDialog()
        //{
        //    //bool? result = null;
        //    //this.OnUIThread(() =>
        //    //{
        //    //    result = this._windowManager.ShowDialog(this);
        //    //});
        //    return this.ShowDialog();
        //}
        //打开校准界面初始化
        public void InitData(ShellPage shell)
        {
            try
            {
                Control_Shell = shell;
                IsConnect = true;
                byte[] content = FileHelper.ReadDecrypt(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.ini"));
                if (content != null)
                {
                    CalibrationInfo = XmlObjSerializer.Deserialize<TrayPanelCalibrationInfo>(content);
                    GlobalInfo.Instance.TrayPanelCenter = CalibrationInfo.TrayPanelCenterX;
                    CalibrationInfo.ZResetPosition = 0;
                    //InitSettingInfo();
                }
                else
                {
                    //GlobalInfo.Instance.TrayPanelCenter = GlobalInfo.Instance.CalibrationInfo.TrayPanelCenterX;
                    //GlobalInfo.Instance.TrayPanel_leftW = GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW;
                    //GlobalInfo.Instance.TrayPanel_rightW = GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW + 180;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[SamplerPosSetPage]InitData：", ex);
            }
        }
        #endregion

        private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //InitData();
        }

        public void ConntectWaring()
        {
            try
            {
                Control_Shell.IsConnect = false;
                IsConnect = false;
                if (!GlobalInfo.Instance.IsHimassConnState)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        new MessagePage().ShowDialog("Message_Error2013".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    }));
                }
                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                if (GlobalInfo.Instance.IsHimassConnState)
                {
                    Control_Shell.MainWindow_AutoSamplerSendObjectDataEvent(null,
                                  new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = Control_Shell.IsConnect });
                }
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Control_Shell.StatusColors = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBDBDBD"));
                    Control_Shell.StatusText = "D/C";
                }));
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ConntectWaring：", ex);
            }
        }
        //清错
        private void ClearErrorCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SamplerPosSetPage[ClearErrorCommand]", ex);
                    }
                    finally
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }
                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage[ClearErrorCommand]]", ex); }
        }
        //取消按钮
        private void CancelCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] content = FileHelper.ReadDecrypt(System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.ini"));
                if (content != null)
                {
                    CalibrationInfo = XmlObjSerializer.Deserialize<TrayPanelCalibrationInfo>(content);
                    //InitSettingInfo();
                }
                else
                {
                    //GlobalInfo.Instance.TrayPanelCenter = GlobalInfo.Instance.CalibrationInfo.TrayPanelCenterX;
                    //GlobalInfo.Instance.TrayPanel_leftW = GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW;
                    //GlobalInfo.Instance.TrayPanel_rightW = GlobalInfo.Instance.CalibrationInfo.TrayCenterToLeftW + 180;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[SamplerPosSetPage]InitData：", ex);
            }
        }
        //进样针位置
        private void GoToXYCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                //Control_ParentView.IsSamplerManual = true;
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        long longseconds = DateTime.Now.Ticks / 10000;
                        int count = 0;
                        if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if (result == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                        {
                            GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                            GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                            Thread.Sleep(300);
                            GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                            longseconds = DateTime.Now.Ticks / 10000;
                            count = 0;
                            while (true)
                            {
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                        {
                                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                            try
                                            {
                                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                                {
                                                    try
                                                    {
                                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                        GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                        Thread.Sleep(300);
                                                        GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                        Thread.Sleep(300);
                                                        GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }

                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                            return;
                                        }
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {

                                        return;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        #region  防止撞针
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.TrayPanelHomeZ / GlobalInfo.ZLengthPerCircle * 3600));
                        Thread.Sleep(300);
                        #endregion

                        Point pt = new Point();
                        int isCollisionStatus = 0;
                        pt = GetPositionInfoHelper.GetItemPosition(CalibrationInfo.PosNumber);
                        MainLogHelper.Instance.Info(CalibrationInfo.PosNumber + "号发送位置：(" + pt.X + "," + pt.Y + ")");

                        isCollisionStatus = GetPositionInfoHelper.GetXIsCollision(pt, CalibrationInfo.PosNumber);
                        if (isCollisionStatus == 1)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((100 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                            Thread.Sleep(200);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                            count = 0;
                            while (true)
                            {
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                        {
                                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                            try
                                            {
                                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                                {
                                                    try
                                                    {
                                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((100 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                                                        Thread.Sleep(100);
                                                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }
                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                            return;
                                        }
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {

                                        return;
                                    }
                                    else
                                        break;
                                }
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                            Thread.Sleep(200);
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                            count = 0;
                            while (true)
                            {
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                        {
                                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                            try
                                            {
                                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                                {
                                                    try
                                                    {
                                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                        Thread.Sleep(100);
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }

                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                            return;
                                        }
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {

                                        return;
                                    }
                                    else
                                        break;
                                }
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                            Thread.Sleep(200);
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                            count = 0;
                            while (true)
                            {
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                        {
                                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                            try
                                            {
                                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                                {
                                                    try
                                                    {
                                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                        Thread.Sleep(100);
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }

                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                            return;
                                        }
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {

                                        return;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        else if (isCollisionStatus == 2)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((354 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                            count = 0;
                            while (true)
                            {
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                        {
                                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                            //try
                                            //{
                                            //    //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            //    {
                                            try
                                            {
                                                //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((354 + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600));
                                                Thread.Sleep(100);
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }
                                            //    }

                                            //}
                                            //catch (Exception ex)
                                            //{
                                            //    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            //}

                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                            return;
                                        }
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {

                                        return;
                                    }
                                    else
                                        break;
                                }
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                            count = 0;
                            while (true)
                            {
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                        {
                                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                            try
                                            {
                                                //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                                {
                                                    try
                                                    {
                                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                        Thread.Sleep(100);
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }

                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                            return;
                                        }
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {

                                        return;
                                    }
                                    else
                                        break;
                                }
                            }
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            GlobalInfo.Instance.IsMotorXActionOk = false;
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                            count = 0;
                            while (true)
                            {
                                Thread.Sleep(300);
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                                {
                                    break;
                                }
                                else
                                {
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                    {
                                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                        {
                                            //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                            try
                                            {
                                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                                {
                                                    try
                                                    {
                                                        //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                        //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                        Thread.Sleep(100);
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                            }

                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                            return;
                                        }
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {
                                        return;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        //Thread.Sleep(100);  //---
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                        if (isCollisionStatus == 0)
                        {
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                        }
                        else
                            GlobalInfo.Instance.IsMotorWSetTargetPositionOk = true;
                        count = 0;
                        while (true)
                        {
                            Thread.Sleep(300);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                            {
                                break;
                            }
                            else
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                                                    if (isCollisionStatus == 0)
                                                    {
                                                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                                                        Thread.Sleep(300);
                                                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                    }
                                                    //Thread.Sleep(300);
                                                    //GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }

                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        if (isCollisionStatus == 0)
                        {
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                        }
                        else
                            GlobalInfo.Instance.IsMotorWActionOk = true;
                        count = 0;
                        while (true)
                        {
                            Thread.Sleep(300);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                            {
                                break;
                            }
                            else
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                    if (isCollisionStatus == 0)
                                                    {
                                                        GlobalInfo.Instance.IsMotorWActionOk = false;
                                                        Thread.Sleep(100);
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }

                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                        //GlobalInfo.Instance.IsMotorWActionOk = true;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                        if (isCollisionStatus == 0)
                        {
                            GlobalInfo.Instance.IsMotorWActionOk = false;
                            Thread.Sleep(100);
                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        }
                        else
                            GlobalInfo.Instance.IsMotorWActionOk = true;
                        count = 0;
                        while (true)
                        {
                            Thread.Sleep(300);
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetMortorWorkModeOk)
                            {
                                break;
                            }
                            else
                            {
                                longseconds = DateTime.Now.Ticks / 10000;
                                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    Thread.Sleep(100);
                                }
                                if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                                {
                                    if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                    {
                                        //GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                        try
                                        {
                                            //foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                            {
                                                try
                                                {
                                                    //GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                    //GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                    //GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                    //Thread.Sleep(500);
                                                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                    if (isCollisionStatus == 0)
                                                    {
                                                        GlobalInfo.Instance.IsMotorWActionOk = false;
                                                        Thread.Sleep(100);
                                                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToXYCommand]：", ex);
                                        }
                                        count++;
                                        continue;
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                        return;
                                    }

                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                {

                                    return;
                                }
                                else
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogHelper.Instance.Error("SettingModule [GoToXYCommand]", ex);
                    }
                    finally
                    {
                        CalibrationLeftShowX = GlobalInfo.returnPositionX * GlobalInfo.XLengthPerCircle / 3600.0 - GlobalInfo.Instance.TrayPanelHomeX;
                        CalibrationLeftShowW = (GlobalInfo.returnPositionW - GlobalInfo.Instance.TrayPanelHomeW) / 60;

                        //Control_ParentView.IsSamplerManual = false;
                        GlobalInfo.Instance.IsBusy = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        source?.Cancel();
                        source?.Dispose();
                    }

                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SettingModule [GoToXYCommand]", ex); }
            //IsCanZ = true;

        }

        private void LouYeCao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;

                //GlobalInfo.Instance.Totalab_LSerials.SetLeakage_tank(0x00);     //打开

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SettingModule [GoToXYCommand]", ex); }
            finally
            {
                GlobalInfo.Instance.IsBusy = false;
                GlobalInfo.Instance.IsCanRunning = true;
            }

        }
        /// <summary>
        /// 校准角度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TurnToTargetLocationCommand_0(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Button btn = sender as Button;
                string str = btn.Tag.ToString();
                bool result = false;
                Task.Factory.StartNew(() =>
                {
                    if (str == "right_Turn")
                    {
                        CalibrationInfo.CalibrationLeftW = CalibrationInfo.CalibrationLeftW + 0.2;
                        result = MotorActionHelper.MotorMoveToTargetPosition(CalibrationInfo.CalibrationLeftX + 120.75, CalibrationInfo.CalibrationLeftW);
                    }
                    if (str == "left_Turn")
                    {
                        CalibrationInfo.CalibrationLeftW = CalibrationInfo.CalibrationLeftW - 0.2;
                        result = MotorActionHelper.MotorMoveToTargetPosition(CalibrationInfo.CalibrationLeftX + 120.75, CalibrationInfo.CalibrationLeftW);
                    }
                    if (result == false)
                    {
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        {
                            ConntectWaring();
                        }
                    }
                    CalibrationLeftShowX = GlobalInfo.returnPositionX * GlobalInfo.XLengthPerCircle / 3600.0 - GlobalInfo.Instance.TrayPanelHomeX;
                    CalibrationLeftShowW = (GlobalInfo.returnPositionW - GlobalInfo.Instance.TrayPanelHomeW) / 60;
                    GlobalInfo.Instance.IsBusy = false;
                    GlobalInfo.Instance.IsCanRunning = true;
                    source?.Cancel();
                    source?.Dispose();

                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [HomeCommand]", ex); }

        }
        /// <summary>
        /// 校准距离
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToTargetLocationCommand_calibration(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();
                Button btn = sender as Button;
                string str = btn.Tag.ToString();
                bool result = false;
                Task.Factory.StartNew(() =>
                {
                    if (str == "right_Move")
                    {
                        CalibrationInfo.CalibrationLeftX = CalibrationInfo.CalibrationLeftX + 0.2;
                        result = MotorActionHelper.MotorMoveToTargetPosition(CalibrationInfo.CalibrationLeftX + 120.75, CalibrationInfo.CalibrationLeftW);
                    }
                    if (str == "left_Move")
                    {
                        CalibrationInfo.CalibrationLeftX = CalibrationInfo.CalibrationLeftX - 0.2;
                        result = MotorActionHelper.MotorMoveToTargetPosition(CalibrationInfo.CalibrationLeftX + 120.75, CalibrationInfo.CalibrationLeftW);
                    }
                    if (result == false)
                    {
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        {
                            ConntectWaring();
                        }
                    }
                    CalibrationLeftShowX = GlobalInfo.returnPositionX * GlobalInfo.XLengthPerCircle / 3600.0 - GlobalInfo.Instance.TrayPanelHomeX;
                    CalibrationLeftShowW = (GlobalInfo.returnPositionW - GlobalInfo.Instance.TrayPanelHomeW )/ 60;
                    GlobalInfo.Instance.IsBusy = false;
                    GlobalInfo.Instance.IsCanRunning = true;
                    source?.Cancel();
                    source?.Dispose();

                }, source.Token);

            }
            catch (Exception ex) { MainLogHelper.Instance.Error("SamplerPosSetPage [HomeCommand]", ex); }

        }
        /// <summary>
        /// 更换进样针
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeSyringeCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                CancellationTokenSource source = new CancellationTokenSource();

                bool result = false;
                Task.Factory.StartNew(() =>
                {
                    result = MotorActionHelper.MotorMoveToTargetPosition(CalibrationInfo.CalibrationLeftX + 167.5, CalibrationInfo.CalibrationLeftW + 90);

                    if (result == false)
                    {
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                        {
                            ConntectWaring();
                        }
                    }
                    else
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                           bool? result1 = new MessagePage().ShowDialog("确定要更换进样针吗！", "警告", true, Enum_MessageType.Warning, yesContent: "确定", cancelContent: "取消");
                            if (result1 == true)
                            {
                                new MessagePage().ShowDialog("请先移动C号试管底座！", "温馨提示", true, Enum_MessageType.Warning, yesContent: "确定", cancelContent: "取消");
                            }
                        }));

                        //MessageBoxResult result1 = MessageBox.Show("确定要更换进样针吗！" ,"警告", MessageBoxButton.OKCancel,MessageBoxImage.Warning);
                        // if (result1 == System.Windows.MessageBoxResult.OK)
                        // {
                        //     MessageBox.Show("请先移动C号试管底座！", "温馨提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        // } 
                    }

                    GlobalInfo.Instance.IsBusy = false;
                    GlobalInfo.Instance.IsCanRunning = true;
                    source?.Cancel();
                    source?.Dispose();

                }, source.Token);

            }
            catch (Exception)
            {

            }
        }
    }
}
