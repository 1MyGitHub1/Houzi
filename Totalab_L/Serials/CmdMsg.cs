using LabTech.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Serials
{
    public class CmdMsg
    {
        #region structure
        //--------------------------------------------------------------------------
        /// <summary>
        /// 构造函数
        /// </summary>
        public CmdMsg()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CmdMsg(byte framstart, byte endaddress, byte startaddress, byte functioncode, byte cmd,byte datalength, byte[] data, byte CRCfirst, byte CRCsecond, byte frameend)
        {
            _FrameStart = framstart;
            _EndAddress = endaddress;
            _StartAddress = startaddress;
            _FunctionCode = functioncode;
            _Cmd = cmd;
            _DataLength = datalength;
            if (data == null)
            {

                _Data = new byte[] { };
            }
            else
            {

                _Data = data;
            }
            _CRCFirst = CRCfirst;
            _CRCSecond = CRCsecond;
            _FrameEnd = frameend;
        }

        public CmdMsg(byte[] data, byte datalength)
        {

            if (data == null)
            {

                _Data = new byte[] { };
            }
            else
            {

                _Data = data;
            }

            _DataLength = datalength;
        }

        public CmdMsg(byte cmd, byte[] data, byte datalength)
        {

            _Cmd = cmd;

            if (data == null)
            {

                _Data = new byte[] { };
            }
            else
            {

                _Data = data;
            }

            _DataLength = datalength;
        }
        public CmdMsg(byte data)  //2014-8-1 添加构造函数  发送E8
        {
            if (data == 0)
            {

                _FrameStart = new byte { };
            }
            else
            {

                _FrameStart = data;
            }
        }
        //--------------------------------------------------------------------------
        #endregion structure
        #region fields
        //--------------------------------------------------------------------------
        /// <summary>
        /// 数据,字节数组
        /// </summary>
        private byte _Cmd = 0;
        private byte[] _Data;
        private byte _FrameStart = 0;
        private byte _EndAddress = 0;
        private byte _StartAddress = 0;
        private byte _FunctionCode = 0;
        private byte _CRCFirst = 0;
        private byte _CRCSecond = 0;
        private byte _FrameEnd = 0;
        /// <summary>
        /// 数据长度,以字节为单位
        /// </summary>
        private byte _DataLength = 0;

        private string _Strl = "";
        //--------------------------------------------------------------------------
        #endregion fields


        #region methods
        //--------------------------------------------------------------------------

        /// <summary>
        /// </summary>
        /// <returns>返回由各个部分组合的字节数组帧</returns>
        public byte[] GetFrame()
        {
            System.Collections.ArrayList frameArray = new System.Collections.ArrayList();
            frameArray.Add(FrameStart);
            frameArray.Add(StartAddress);
            frameArray.Add(EndAddress);
            frameArray.Add(FunctionCode);
            frameArray.Add(Cmd);
            frameArray.Add(DataLength);
            if (DataLength != 0)
            {
                for (int i = 0; i < DataLength; i++)
                {
                    frameArray.Add(Data[i]);
                }
                frameArray.Add(CRCFirst);
                frameArray.Add(CRCSecond);
                frameArray.Add(FrameEnd);
            }
            else
            {
                frameArray.Add(CRCFirst);
                frameArray.Add(CRCSecond);
                frameArray.Add(FrameEnd);
            }
            return (byte[])frameArray.ToArray(typeof(byte));
        }


        public static CmdMsg GetMsg(byte[] msgList)
        {
            CmdMsg msg = new CmdMsg();
            //msg.DataLength = Convert.ToByte((msgList[2] >> 4) & 0x0f);
            msg.FrameStart = msgList[0];
            msg.EndAddress = msgList[2];
            msg.StartAddress = msgList[1];
            //msg.EndAddress = msgList[3];
            msg.FunctionCode = msgList[3];
            msg.Cmd = msgList[4];
            msg.DataLength = msgList[5];
            msg.Data = new byte[msg.DataLength];
            for (int i = 0; i < msg.DataLength; i++)
            {
                msg.Data[i] = msgList[i + 6];
            }

            //if (msg.FunctionCode == 0x05)
            // {
            // msg.EndAddress = msgList[6 + msg.DataLength];
            //}
            // else
            {
                msg.CRCFirst = msgList[6 + msg.DataLength];
                msg.CRCSecond = msgList[7 + msg.DataLength];
                msg.FrameEnd = msgList[8 + msg.DataLength];
            }
            //msg.CRCFirst = msgList[
            return msg;
        }
        //--------------------------------------------------------------------------
        #endregion methods


        #region properties
        //--------------------------------------------------------------------------
        public byte Cmd
        {
            get
            {
                return _Cmd;
            }
            set
            {
                _Cmd = value;
            }
        }
        /// <summary>
        /// 数据,字节数组
        /// </summary>
        public byte[] Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }
        ///<summary>
        ///帧首
        ///</summary>
        public byte FrameStart
        {
            get
            {
                return _FrameStart;
            }
            set
            {
                _FrameStart = value;
            }
        }
        /// <summary>
        /// 目的地址
        /// </summary>
        public byte EndAddress
        {
            get
            {
                return _EndAddress;
            }
            set
            {
                _EndAddress = value;
            }
        }
        ///<summary>
        ///源地址
        ///</summary>
        public byte StartAddress
        {
            get
            {
                return _StartAddress;
            }
            set
            {
                _StartAddress = value;
            }
        }
        /////<summary>
        /////功能码
        /////</summary>
        public byte FunctionCode
        {
            get
            {
                return _FunctionCode;
            }
            set
            {
                _FunctionCode = value;
            }
        }
        ///<summary>
        ///校验码
        ///</summary>
        public byte CRCFirst
        {
            get
            {
                return _CRCFirst;
            }
            set
            {
                _CRCFirst = value;
            }
        }
        public byte CRCSecond
        {
            get
            {
                return _CRCSecond;
            }
            set
            {
                _CRCSecond = value;
            }
        }
        ///<summary>
        ///帧尾
        ///</summary>
        public byte FrameEnd
        {
            get
            {
                return _FrameEnd;
            }
            set
            {
                _FrameEnd = value;
            }
        }
        /// <summary>
        /// 数据长度,以字节为单位
        /// </summary>
        public byte DataLength
        {
            get
            {
                return _DataLength;
            }
            set
            {
                _DataLength = value;
            }
        }

        public string Strl
        {
            get
            {
                return _Strl;
            }
            set
            {
                _Strl = value;
            }
        }

        //--------------------------------------------------------------------------
        #endregion properties
    }
}
