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
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
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
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
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
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
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
    }
}
