using LabTech.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Totalab_L
{
    /// <summary>
    /// 实现序列化约定的接口
    /// </summary>
    public class InstrumentSerialNum : CommonLibrary.IInstrument
    {
        /// <summary>
        ///  从仪器读回MCU
        /// </summary>
        /// <returns></returns>
        public byte[] ReadFromToInstrument()
        {
            try
            {
                GlobalInfo.Instance.IsReadMCUOk = false;
                GlobalInfo.Instance.Totalab_LSerials.ReadMcuSerials();
                int time = 0;
                while (!GlobalInfo.Instance.IsReadMCUOk)
                {
                    if (time < 4)
                    {
                        Thread.Sleep(1000);
                        GlobalInfo.Instance.Totalab_LSerials.ReadMcuSerials();
                        time++;
                    }
                    else
                    {
                        MainLogHelper.Instance.Info($"[ ReadFromToInstrument  bitMCU =null");
                        return null;
                    }
                }
                string bitMCU = BitConverter.ToString(GlobalInfo.Instance.MCUData);
                MainLogHelper.Instance.Info($"[ ReadFromToInstrument  bitMCU ={bitMCU}");
                return GlobalInfo.Instance.MCUData;
            }
            catch
            {
                MainLogHelper.Instance.Info($"[ ReadFromToInstrument  bitMCU =null");
                return null;
            }
        }

        /// <summary>
        /// 将MCU写入仪器
        /// </summary>
        /// <param name="mcu"></param>
        /// <returns></returns>
        public bool WriteToInstrument(byte[] mcu)
        {
            string res = string.Empty;
            try
            {
                GlobalInfo.Instance.IsWriteMCUOk = false;
                GlobalInfo.Instance.Totalab_LSerials.WriteMcuSerials(mcu);
                string bitMCU = BitConverter.ToString(mcu);
                MainLogHelper.Instance.Info($"[WriteToInstrument  bitMCU ={bitMCU}");
                int time = 0;
                while (!GlobalInfo.Instance.IsWriteMCUOk)
                {
                    if (time < 4)
                    {
                        Thread.Sleep(1000);
                        GlobalInfo.Instance.Totalab_LSerials.WriteMcuSerials(mcu);
                        time++;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckConnection(string CanDevName = null)
        {
            return GlobalInfo.Instance.Totalab_LSerials.IsConnect;
        }
    }
}