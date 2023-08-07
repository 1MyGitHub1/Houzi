using DeviceInterface;
using LabTech.Common;
using Mass.Common.Enums;
using Monster.AutoSampler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Totalab_L.Enum;

namespace Totalab_L.Common
{
    public static class MotorActionHelper///电机动作操作
    {
        public static bool MotorErrorStopImmediately()
        {
            try
            {
                if (!GlobalInfo.Instance.IsMotorXError)
                {
                    GlobalInfo.Instance.IsMotorXStop = false;
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0B);
                }
                else
                    GlobalInfo.Instance.IsMotorXStop = true;
                if (!GlobalInfo.Instance.IsMotorWError)
                {
                    GlobalInfo.Instance.IsMotorWStop = false;
                    Thread.Sleep(300);
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0B);
                }
                else
                    GlobalInfo.Instance.IsMotorWStop = true;
                if (!GlobalInfo.Instance.IsMotorZError)
                {
                    GlobalInfo.Instance.IsMotorZStop = false;
                    Thread.Sleep(300);
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0B);
                }
                else
                    GlobalInfo.Instance.IsMotorZStop = true;
                long longseconds = DateTime.Now.Ticks / 10000;
                int count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (!(GlobalInfo.Instance.IsMotorXStop && GlobalInfo.Instance.IsMotorWStop && GlobalInfo.Instance.IsMotorZStop) 
                        && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 )
                    {
                        Thread.Sleep(100);
                    }
                    if (!(GlobalInfo.Instance.IsMotorXStop && GlobalInfo.Instance.IsMotorWStop && GlobalInfo.Instance.IsMotorZStop))
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
                                        if(!GlobalInfo.Instance.IsMotorXSetWorkModeOk)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0B);
                                        if (!GlobalInfo.Instance.IsMotorWSetWorkModeOk)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0B);
                                        if (!GlobalInfo.Instance.IsMotorZSetWorkModeOk)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0B);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MotorErrorStopImmediately]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MotorErrorStopImmediately]：", ex);
                            }

                            count++;
                            continue;
                        }
                        else
                        {

                            return false;
                        }
                    }
                    else
                        break;
                }
                return true;
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("MotorActionHelper [MotorErrorStopImmediately]", ex);
                return false;
            }
        }

        public static bool MotorStopImmediately()
        {
            try
            {
                GlobalInfo.Instance.IsMotorXStop = false;
                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0B);
                GlobalInfo.Instance.IsMotorWStop = false;
                Thread.Sleep(300);
                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0B);
                GlobalInfo.Instance.IsMotorZStop = false;
                Thread.Sleep(300);
                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0B);
                long longseconds = DateTime.Now.Ticks / 10000;
                int count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (!(GlobalInfo.Instance.IsMotorXStop && GlobalInfo.Instance.IsMotorWStop && GlobalInfo.Instance.IsMotorZStop)
                        && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                    {
                        Thread.Sleep(100);
                    }
                    if (!(GlobalInfo.Instance.IsMotorXStop && GlobalInfo.Instance.IsMotorWStop && GlobalInfo.Instance.IsMotorZStop))
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
                                        if (!GlobalInfo.Instance.IsMotorXSetWorkModeOk)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0B);
                                        if (!GlobalInfo.Instance.IsMotorWSetWorkModeOk)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0B);
                                        if (!GlobalInfo.Instance.IsMotorZSetWorkModeOk)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0B);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MotorStopImmediately]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MotorStopImmediately]：", ex);
                            }

                            count++;
                            continue;
                        }
                        else
                        {

                            return false;

                        }
                    }
                    else
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MotorActionHelper [MotorStopImmediately]", ex);
                return false;
            }
        }

        public static bool MotorClearError()
        {
            try
            {
                if (GlobalInfo.Instance.IsMotorXError)
                {
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x80);
                }
                if (GlobalInfo.Instance.IsMotorWError)
                {
                    Thread.Sleep(300);
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x80);
                }
                if (GlobalInfo.Instance.IsMotorZError)
                {
                    Thread.Sleep(300);
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x80);
                }
                long longseconds = DateTime.Now.Ticks / 10000;
                int count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (!(GlobalInfo.Instance.IsMotorXError == false && GlobalInfo.Instance.IsMotorWError == false && GlobalInfo.Instance.IsMotorZError ==false)
                        && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                    {
                        Thread.Sleep(100);
                    }
                    if (GlobalInfo.Instance.IsMotorXError != false || GlobalInfo.Instance.IsMotorWError != false || GlobalInfo.Instance.IsMotorZError != false)
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
                                        if (GlobalInfo.Instance.IsMotorXError)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x80);
                                        if (GlobalInfo.Instance.IsMotorWError)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x80);
                                        if (GlobalInfo.Instance.IsMotorZError)
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x80);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MotorClearError]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MotorClearError]：", ex);
                            }

                            count++;
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        break;
                }
                GlobalInfo.Instance.Totalab_LSerials.SetLightStatus(0x00, 0x07);
                Thread.Sleep(100);
                GlobalInfo.Instance.Totalab_LSerials.SetLightStatus(0x01, 0x02);
                Thread.Sleep(100);
                return true;
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MotorActionHelper [MotorClearError]", ex);
                return false;
            }
        }

        /// <summary>
        ///电机移动到目标位置
        /// </summary>
        /// <returns></returns>
        public static bool MotorMoveToTargetPosition(double xMoveDistance, double wMoveAngle)
        {
            bool result = true;
            try
            {
                GlobalInfo.Instance.IsBusy = true;
                GlobalInfo.Instance.IsCanRunning = false;
                long longseconds = 0;
                int count = 0;
                Point pt = new Point();
                pt = new Point((xMoveDistance + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600.0, wMoveAngle * 3.0 * 10.0);
                try
                {
                    if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                    {
                        result = MotorActionHelper.MotorClearError();
                        if (result == false)
                        {
                            return result;
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
                                    result = false;
                                    return result;
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {
                                result = false;
                                return result;
                            }
                            else
                                break;
                        }
                    }
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
                                            MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]：", ex);
                                }

                                count++;
                                continue;
                            }
                            else
                            {
                                result = false;
                                return result;
                            }
                        }
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                        {
                            result = false;
                            return result;
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
                                            MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]：", ex);
                                }

                                count++;
                                continue;
                            }
                            else
                            {
                                result = false;
                                return result;
                            }
                        }
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                        {
                            result = false;
                            return result;
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
                                            MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]：", ex);
                                }

                                count++;
                                continue;
                            }
                            else
                            {
                                result = false;
                                return result;
                            }
                        }
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                        {
                            result = false;
                            return result;
                        }
                        else
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]", ex);
                }
                finally
                {
                    GlobalInfo.Instance.IsBusy = false;
                    GlobalInfo.Instance.IsCanRunning = true;
                }
            }
            catch (Exception ex) { MainLogHelper.Instance.Error("MotorActionHelper [MotorMoveToTargetPosition]", ex); }
            return result;

        }
    }
}
