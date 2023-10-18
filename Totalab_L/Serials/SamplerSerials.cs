using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Totalab_L.Serials
{
    public class SamplerSerials : Serials
    {
        int CurRcvLen = 0;
        public enum RcvStatus
        {
            FrameStart,            //帧首
            EndAddress,           //目的地址
            StartAddress,        //源地址
            FunctionCode,       //功能码
            Cmd,                   //命令位
            DataLength,
            Data,                 //数据位
            CRCFirst,             //校验码
            CRCSecond,          //校验码
            FrameEnd,            //帧尾
            //ErrorCmd,            //故障处理命令位
            //ErrorData,           //故障处理数据位
        }
        private CmdMsg _RcvTempMsg = new CmdMsg();
        private RcvStatus _RcvStatus;
        //byte datalength = 0;
        public SamplerSerials()
        {
            SPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(SerialPort_DataReceived);
            SPort.BaudRate = 9600;
            SPort.DataBits = 8;
            SPort.StopBits = System.IO.Ports.StopBits.One;
            SPort.ReadTimeout = Timeout.Infinite;
            SPort.WriteTimeout = Timeout.Infinite;
            SPort.ReceivedBytesThreshold = 1;
            SPort.ReadBufferSize = 40960;
        }

        public int CRCVerify(byte[] msg)
        {
            int data = msg[1];
            for (int i = 2; i < msg.Length - 3; i++)
            {
                data += msg[i];
                //Trace.WriteLine(msg[i]);
            }
            return data;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (IsWorking == true && SPort.BytesToRead > 0)    //判断通讯是否工作和串口缓冲区中字节数>0
                {

                    byte curByte = (byte)SPort.ReadByte();     //输入缓冲区中同步读取一个字节
                    switch (_RcvStatus)
                    {
                        case RcvStatus.FrameStart:
                            _RcvTempMsg.FrameStart = curByte;
                            if (_RcvTempMsg.FrameStart == 0x5a)
                            {
                                _RcvStatus = RcvStatus.StartAddress;
                            }
                            break;
                        case RcvStatus.StartAddress:
                            _RcvTempMsg.StartAddress = curByte;
                            _RcvStatus = RcvStatus.EndAddress;
                            break;
                        case RcvStatus.EndAddress:
                            _RcvTempMsg.EndAddress = curByte;
                            _RcvStatus = RcvStatus.FunctionCode;
                            break;
                        case RcvStatus.FunctionCode:
                            _RcvTempMsg.FunctionCode = curByte;
                            _RcvStatus = RcvStatus.Cmd;
                            break;
                        case RcvStatus.Cmd:
                            _RcvTempMsg.Cmd = curByte;
                            _RcvStatus = RcvStatus.DataLength;
                            break;
                        case RcvStatus.DataLength:
                            _RcvTempMsg.DataLength = curByte;
                            if (_RcvTempMsg.FunctionCode == 0x85)
                            {
                                _RcvTempMsg.DataLength = 0;
                            }
                            if (_RcvTempMsg.DataLength == 0)
                            {
                                CurRcvLen = 0;
                                _RcvStatus = RcvStatus.CRCFirst;
                            }
                            else
                            {
                                if (_RcvTempMsg.DataLength > 35)
                                {
                                    _RcvStatus = RcvStatus.FrameStart;
                                    _RcvTempMsg = new CmdMsg();
                                }
                                else
                                {
                                    _RcvTempMsg.Data = new byte[_RcvTempMsg.DataLength];
                                    _RcvStatus = RcvStatus.Data;
                                }
                            }
                            //_RcvStatus = RcvStatus.Data;
                            break;
                        case RcvStatus.Data:
                            if (CurRcvLen < _RcvTempMsg.DataLength)
                            {
                                _RcvTempMsg.Data[CurRcvLen] = curByte;
                                CurRcvLen++;

                            }
                            if (CurRcvLen == _RcvTempMsg.DataLength)
                            {
                                CurRcvLen = 0;
                                _RcvStatus = RcvStatus.CRCFirst;
                            }
                            break;
                        case RcvStatus.CRCFirst:
                            _RcvTempMsg.CRCFirst = curByte;
                            _RcvStatus = RcvStatus.CRCSecond;
                            break;
                        case RcvStatus.CRCSecond:
                            _RcvTempMsg.CRCSecond = curByte;
                            int crc = CRCVerify(_RcvTempMsg.GetFrame());
                            if (_RcvTempMsg.CRCFirst == crc / 256 && _RcvTempMsg.CRCSecond == crc % 256)
                                _RcvStatus = RcvStatus.FrameEnd;
                            else
                            {
                                _RcvStatus = RcvStatus.FrameStart;
                                _RcvTempMsg = new CmdMsg();
                            }
                            break;
                        case RcvStatus.FrameEnd:
                            _RcvTempMsg.FrameEnd = curByte;
                            if (_RcvTempMsg.FrameEnd == 0xa5)   //////帧尾
                            {
                                respondTimer.Change(Timeout.Infinite, Timeout.Infinite);
                                _RcvStatus = RcvStatus.FrameStart;
                                if (_RcvTempMsg.FunctionCode == 0x85 && _RcvTempMsg.Cmd == 0x50)
                                {
                                    _RcvTempMsg = new CmdMsg();
                                }
                                else
                                {
                                    RcvQueue.Enqueue(_RcvTempMsg);
                                    _RcvTempMsg = new CmdMsg();
                                    //Thread.Sleep(100);
                                    MsgRcvCome.Set();
                                }
                            }
                            else
                            {
                                _RcvStatus = RcvStatus.FrameStart;
                                _RcvTempMsg = new CmdMsg();
                            }
                            break;
                        default:
                            _RcvStatus = RcvStatus.FrameStart;
                            _RcvTempMsg = new CmdMsg();
                            break;
                    }

                }
            }
            catch { }

        }

        public void Connect()
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[9];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x55;
            frameArray[4] = 0xA1;
            frameArray[5] = 0x00;
            int crc = CRCVerify(frameArray);
            frameArray[6] = (byte)(crc / 256);
            frameArray[7] = (byte)(crc % 256);
            frameArray[8] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
            //Send(frameArray);
        }

        public void Init()
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[9];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0xE8;
            frameArray[4] = 0x00;
            frameArray[5] = 0x00;
            int crc = CRCVerify(frameArray);
            frameArray[6] = (byte)(crc / 256);
            frameArray[7] = (byte)(crc % 256);
            frameArray[8] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        public void PumpRunSpeed(int dSpeed)  //  速度
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[11];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x21;
            frameArray[4] = 0x17;
            frameArray[5] = 0x02;
            frameArray[6] = (byte)(dSpeed / 256);
            frameArray[7] = (byte)(dSpeed % 256);
            int crc = CRCVerify(frameArray);
            frameArray[8] = (byte)(crc / 256);
            frameArray[9] = (byte)(crc % 256);
            frameArray[10] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }
        //蠕动泵
        public void PumpRun(byte dir = 0x01)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[10];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x21;
            frameArray[4] = 0x09;
            frameArray[5] = 0x01;
            frameArray[6] = dir;
            int crc = CRCVerify(frameArray);
            frameArray[7] = (byte)(crc / 256);
            frameArray[8] = (byte)(crc % 256);
            frameArray[9] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        //蠕动泵停止
        public void PumpStop()
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[9];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x21;
            frameArray[4] = 0x3d;
            frameArray[5] = 0x00;
            int crc = CRCVerify(frameArray);
            frameArray[6] = (byte)(crc / 256);
            frameArray[7] = (byte)(crc % 256);
            frameArray[8] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        public void TriggerOut(byte status)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[10];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x01;
            frameArray[4] = 0x53;
            frameArray[5] = 0x01;
            frameArray[6] = status;
            int crc = CRCVerify(frameArray);
            frameArray[7] = (byte)(crc / 256);
            frameArray[8] = (byte)(crc % 256);
            frameArray[9] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        public void TriggerIn(byte status)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[10];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x01;
            frameArray[4] = 0x54;
            frameArray[5] = 0x01;
            frameArray[6] = status;
            int crc = CRCVerify(frameArray);
            frameArray[7] = (byte)(crc / 256);
            frameArray[8] = (byte)(crc % 256);
            frameArray[9] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        //读取单片机串口
        public void ReadMcuSerials(byte length = 30)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[10];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0xAA;
            frameArray[4] = 0xA1;
            frameArray[5] = 0x01;
            frameArray[6] = length;
            int crc = CRCVerify(frameArray);
            frameArray[7] = (byte)(crc / 256);
            frameArray[8] = (byte)(crc % 256);
            frameArray[9] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }


        public void WriteMcuSerials(byte[] buf)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[9+ buf.Length];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0xAA;
            frameArray[4] = 0xA2;
            frameArray[5] = (byte)(buf.Length);
            for(int i = 0; i<buf.Length;i++)
            {
                frameArray[6 + i] = buf[i];
            }
            int crc = CRCVerify(frameArray);
            frameArray[6+ buf.Length] = (byte)(crc / 256);
            frameArray[7+  buf.Length] = (byte)(crc % 256);
            frameArray[8 + buf.Length] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        //设置电机工作类型
        public void SetMotorWorkType(byte motorID,byte workType)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[17];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x00;
            frameArray[4] = 0x22;
            frameArray[5] = 0x08;
            frameArray[6] = motorID;
            frameArray[7] = 0x60;
            frameArray[8] = 0x60;
            frameArray[9] = 0x00;
            frameArray[10] = workType;
            frameArray[11] = 0x00;
            frameArray[12] = 0x00;
            frameArray[13] = 0x00;
            int crc = CRCVerify(frameArray);
            frameArray[14] = (byte)(crc / 256);
            frameArray[15] = (byte)(crc % 256);
            frameArray[16] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }


        public void SetMotorHomeMode(byte motorID,int mode)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[17];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x00;
            frameArray[4] = 0x22;
            frameArray[5] = 0x08;
            frameArray[6] = motorID;
            frameArray[7] = 0x98;
            frameArray[8] = 0x60;
            frameArray[9] = 0x00;
            frameArray[10] = (byte)(mode %65536 %256);
            frameArray[11] = (byte)(mode % 65536 / 256);
            frameArray[12] = (byte)(mode / 65536 % 256);
            frameArray[13] = (byte)(mode / 65536 /256);
            int crc = CRCVerify(frameArray);
            frameArray[14] = (byte)(crc / 256);
            frameArray[15] = (byte)(crc % 256);
            frameArray[16] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        //设置目标位置
        public void SetTargetPosition(byte motorID,int step)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[17];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x00;
            frameArray[4] = 0x22;
            frameArray[5] = 0x08;
            frameArray[6] = motorID;
            frameArray[7] = 0x7a;
            frameArray[8] = 0x60;
            frameArray[9] = 0x00;
            if (step < 0)
            {
                frameArray[13] = (byte)((step & 0xFF000000) >> 8 >> 8 >> 8);
                frameArray[12] = (byte)((step & 0xFF0000) >> 8 >> 8);
                frameArray[11] = (byte)((step & 0xFF00) >> 8);
                frameArray[10] = (byte)(step & 0XFF);
            }
            else
            {
                frameArray[10] = (byte)(step % 65536 % 256);
                frameArray[11] = (byte)(step % 65536 / 256);
                frameArray[12] = (byte)(step / 65536 % 256);
                frameArray[13] = (byte)(step / 65536 / 256);
            }
            int crc = CRCVerify(frameArray);
            frameArray[14] = (byte)(crc / 256);
            frameArray[15] = (byte)(crc % 256);
            frameArray[16] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        public void MotorMove(byte motorID,int actionType)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[17];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x00;
            frameArray[4] = 0x22;
            frameArray[5] = 0x08;
            frameArray[6] = motorID;
            frameArray[7] = 0x40;
            frameArray[8] = 0x60;
            frameArray[9] = 0x00;
            frameArray[10] = (byte)(actionType % 65536 % 256);
            frameArray[11] = (byte)(actionType % 65536 / 256);
            frameArray[12] = (byte)(actionType / 65536 % 256);
            frameArray[13] = (byte)(actionType / 65536 / 256);
            int crc = CRCVerify(frameArray);
            frameArray[14] = (byte)(crc / 256);
            frameArray[15] = (byte)(crc % 256);
            frameArray[16] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        public void MotorAction(byte motorID, int actionType)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[17];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x00;
            frameArray[4] = 0x22;
            frameArray[5] = 0x08;
            frameArray[6] = motorID;
            frameArray[7] = 0x41;
            frameArray[8] = 0x60;
            frameArray[9] = 0x00;
            frameArray[10] = (byte)(actionType % 65536 % 256);
            frameArray[11] = (byte)(actionType % 65536 / 256);
            frameArray[12] = (byte)(actionType / 65536 % 256);
            frameArray[13] = (byte)(actionType / 65536 / 256);
            int crc = CRCVerify(frameArray);
            frameArray[14] = (byte)(crc / 256);
            frameArray[15] = (byte)(crc % 256);
            frameArray[16] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        //重新读取实际位置
        public void ReadMotorPosition(byte motorID)
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[17];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x00;
            frameArray[4] = 0x40;
            frameArray[5] = 0x08;
            frameArray[6] = motorID;
            frameArray[7] = 0x64;
            frameArray[8] = 0x60;
            frameArray[9] = 0x00;
            frameArray[10] = 0x00;
            frameArray[11] = 0x00;
            frameArray[12] = 0x00;
            frameArray[13] = 0x00;
            int crc = CRCVerify(frameArray);
            frameArray[14] = (byte)(crc / 256);
            frameArray[15] = (byte)(crc % 256);
            frameArray[16] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        public void XWZHome()
        {
            _TimeOutTime = Timeout.Infinite;
            byte[] frameArray = new byte[9];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x22;
            frameArray[4] = 0x03;
            frameArray[5] = 0x00;
            int crc = CRCVerify(frameArray);
            frameArray[6] = (byte)(crc / 256);
            frameArray[7] = (byte)(crc % 256);
            frameArray[8] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }

        /// <summary>
        /// 指示灯控制
        /// </summary>
        /// <param name="status">0灭，1常量，2闪烁</param>
        /// <param name="color">bit0红色，bit1绿色，bit2蓝色</param>
        public void SetLightStatus(byte status, byte color)
        {
            byte[] frameArray = new byte[11];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x1A;
            frameArray[2] = 0x00;
            frameArray[3] = 0x22;
            frameArray[4] = 0x11;
            frameArray[5] = 0x02;
            frameArray[6] = status;
            frameArray[7] = color;
            int crc = CRCVerify(frameArray);
            frameArray[8] = (byte)(crc / 256);
            frameArray[9] = (byte)(crc % 256);
            frameArray[10] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }
        /// <summary>
        /// 漏液槽开关
        /// </summary>
        public void SetLeakage_tank(byte status)
        {
            byte[] frameArray = new byte[11];
            frameArray[0] = 0x5a;
            frameArray[1] = 0x00;
            frameArray[2] = 0x00;
            frameArray[3] = 0x23;
            frameArray[4] = 0x01;
            frameArray[5] = 0x02;
            frameArray[6] = 0x00;
            frameArray[7] = status;
            int crc = CRCVerify(frameArray);
            frameArray[8] = (byte)(crc / 256);
            frameArray[9] = (byte)(crc % 256);
            frameArray[10] = 0xa5;
            SendMsg(CmdMsg.GetMsg(frameArray));
        }
    }
}
