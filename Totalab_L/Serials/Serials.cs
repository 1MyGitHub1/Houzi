using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Totalab_L.Serials
{
    public class Serials
    {
        #region Type
        //--------------------------------------------------------------------------

        //消息到来事件
        public class MsgComeEventArgs : EventArgs
        {
            private CmdMsg m_Msg;

            public CmdMsg Msg
            {
                get
                {
                    return m_Msg;
                }
                set
                {
                    m_Msg = value;
                }
            }

            public MsgComeEventArgs(CmdMsg msg)
            {
                Msg = msg;
            }

        }

        //--------------------------------------------------------------------------
        #endregion Type

        #region structors
        //--------------------------------------------------------------------------
        /// <summary>
        /// 构造函数,启动发送线程
        /// </summary>
        public Serials()
        {

        }

        //--------------------------------------------------------------------------
        #endregion structors
        #region fields
        //--------------------------------------------------------------------------

        /// <summary>
        /// 消息到来事件
        /// </summary>
        public event EventHandler<MsgComeEventArgs> MsgCome;

        /// <summary>
        /// 消息的发送队列
        /// </summary>
        private ConcurrentQueue<CmdMsg> SendQueue = new ConcurrentQueue<CmdMsg>();
        /// <summary>
        /// 信号量,用于等待前一帧收到应答
        /// </summary>
        public AutoResetEvent WaitSendSuccess = new AutoResetEvent(true);

        /// <summary>
        /// 发送信息到达的自动重置事件
        /// </summary>
        public AutoResetEvent MsgSendCome = new AutoResetEvent(false);

        /// <summary>
        /// 接收信息到达的自动重置事件
        /// </summary>
        public AutoResetEvent MsgRcvCome = new AutoResetEvent(false);
        public ConcurrentQueue<CmdMsg> RcvQueue = new ConcurrentQueue<CmdMsg>();
        /// <summary>
        /// 发送队列的发送线程
        /// </summary>
        private Thread SendFrameThread;
        /// <summary>
        /// 接收队列的处理线程
        /// </summary>
        public Thread RcvMsgThread;
        public System.Threading.Timer respondTimer;

        public event EventHandler<EventArgs> StartWorkEvent;
        ///<summary>
        ///用来存放数据
        ///</summary>
        public List<byte> buffer = new List<byte>(4096);

        private List<byte> RcvByteList = new List<byte>();

        /// <summary>
        /// 发送超时定时器
        /// </summary>
       // public System.Threading.[Timer respondTimer;
        public CmdMsg SendTempMsg = new CmdMsg();
        private byte _TimeOutNum = 0;
        /// <summary>
        /// 超时时间
        /// </summary>
        public int _TimeOutTime = 5000;
        public bool ischeck = false;
        #endregion fields
        #region methods
        //------------------------------------------------------------------------
        /// <summary>
        /// 消息到来
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMsgCome(MsgComeEventArgs e)
        {
            EventHandler<MsgComeEventArgs> temp = MsgCome;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// 通信开始工作
        /// </summary>
        public bool StartWork()
        {
            respondTimer = new System.Threading.Timer(RespondTimeOut, null, Timeout.Infinite, Timeout.Infinite);
            SPort.Open();
            IsWorking = true;
            IsConnect = true;
            WaitSendSuccess = new AutoResetEvent(true);
            MsgSendCome = new AutoResetEvent(false);
            MsgRcvCome = new AutoResetEvent(false);
            buffer.Clear();
            SendQueue = new ConcurrentQueue<CmdMsg>();
            SendFrameThread = new Thread(new ThreadStart(SendFrame));
            SendFrameThread.Start();
            RcvQueue = new ConcurrentQueue<CmdMsg>();
            RcvByteList.Clear();
            RcvMsgThread = new Thread(new ThreadStart(RcvMsg));
            RcvMsgThread.Start();
            EventHandler<EventArgs> temp = StartWorkEvent;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
            return true;

        }

        /// <summary>
        /// 通信停止工作
        /// </summary>
        public void EndWork()
        {
          
            try
            {
                SPort.Close();
                try
                {
                    SendFrameThread.Abort();
                }
                catch
                {

                }
                try
                {
                    RcvMsgThread.Abort();
                }
                catch { }
                IsWorking = false;
                IsConnect = false;
            }
            catch
            {
                //MessageBox.Show(exc.Message);
                return;
            }
        }
        /// <summary>
        /// 发送消息队列的发送线程
        /// </summary>
        public void SendFrame()
        {
            while (IsWorking)
            {
                //等待上一帧被应答消息到来,触发事件触发
                MsgSendCome.WaitOne();

                //lock (SendQueue)
                {
                    while (SendQueue.Count > 0)
                    {
                        //NS_WaitSendSuccess.WaitOne();

                        //取出下标为0的msg,暂存并发送,设置超时

                        SendQueue.TryDequeue(out CmdMsg msg);
                        if (msg == null)
                        {
                            //MessageBox.Show("发送队列为空");
                            continue;
                        }
                        //SendQueue.RemoveAt(0);
                        SendTempMsg = msg; //暂时保存这条发送的消息，以便于在下位机没有收到的情况下，重发这条信息
                        Send(msg.GetFrame());
                        respondTimer.Change(_TimeOutTime, Timeout.Infinite);
                    }
                }
            }
        }
        /// <summary>
        /// 根据端口,发送帧
        /// </summary>
        /// <param name="frame">要发送帧的字节数组</param>
        public void Send(byte[] frame)
        {
            try
            {
                SPort.Write(frame, 0, frame.Length);
                // respondTimer.Change(_TimeOutTime, Timeout.Infinite);
            }
            catch
            {
                //MessageBox.Show(exc.Message);
            }
        }

        public bool SendMsg(CmdMsg sMsg)
        {
            SendQueue.Enqueue(sMsg);
            MsgSendCome.Set();
            return true;
        }

        public void SendString(string str)
        {
            try
            {
                Thread.Sleep(300);
                SPort.WriteLine(str);
            }
            catch
            {
            }
        }
        /// <summary>
        /// 接收消息队列的处理线程
        /// </summary>
        private void RcvMsg()
        {
            while (IsWorking)
            {
                MsgRcvCome.WaitOne();
                //lock (RcvQueue)
                {
                    while (RcvQueue.Count > 0)
                    {
                        RcvQueue.TryDequeue(out CmdMsg msg);

                        MsgComeEventArgs e = new MsgComeEventArgs(msg);
                        OnMsgCome(e);
                    }
                }
            }
        }

        private void RespondTimeOut(object o)
        {
            respondTimer.Change(Timeout.Infinite, Timeout.Infinite);

            _TimeOutNum++;

            if (_TimeOutNum < 3)                    //连续超时3次就出错。  
            {
                Send(SendTempMsg.GetFrame());
                respondTimer.Change(_TimeOutTime, Timeout.Infinite);
            }
            else
            {
                _TimeOutNum = 0;
                // MessageBox.Show("信息没有收到应答", "通信", MessageBoxButtons.OK, MessageBoxIcon.Warning);     //在最后发布时，这一句可以进行屏弊掉  2008-12-12
                MsgSendCome.Set();
                WaitSendSuccess.Set();
                MsgRcvCome.Set();
                // _RcvStatus = RcvStatus.Cmd;
            }
        }
        //--------------------------------------------------------------------------
        #endregion methods
        #region properties
        //--------------------------------------------------------------------------

        /// <summary>
        /// 表示通讯是否处于工作状态
        /// </summary>
        public bool IsWorking { set; get; } = false;

        /// <summary>
        /// 串行通信端口
        /// </summary>
        public System.IO.Ports.SerialPort SPort { set; get; } = new System.IO.Ports.SerialPort();

        public bool IsConnect { set; get; } = false;
        //--------------------------------------------------------------------------
        #endregion properties
    }
}
