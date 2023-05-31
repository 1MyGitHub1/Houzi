using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Enum
{
   public enum Exp_Status
    {
        /// <summary>
        /// 准备
        /// </summary>
        Ready,

        /// <summary>
        /// 倒计时中
        /// </summary>
        Countdown,

        /// <summary>
        /// 运行
        /// </summary>
        Running,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause,

        /// <summary>
        /// 暂停完成当前样
        /// </summary>
        PauseCurrentSample,

        /// <summary>
        /// 停止
        /// </summary>
        Stop,

        /// <summary>
        /// 实验已完成
        /// </summary>
        Complete,

        /// <summary>
        /// 继续分析
        /// </summary>
        Continuation,
        /// <summary>
        /// 空白状态
        /// </summary>
        Free,
        /// <summary>
        /// 锁死状态
        /// </summary>
        Standby,
        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 跳过
        /// </summary>
       Skip,

        Terminated,
    }
}
