using Caliburn.Micro;
using LabTech.Common;
using Mass.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.AutoSampler.ViewModels
{
    [Export(typeof(MessagePageViewModel))]
    [Export(typeof(BaseData.IContentService))]
    public class MessagePageViewModel : BaseData.BaseViewModel
    {
        #region 构造函数
        [ImportingConstructor]
        public MessagePageViewModel(IEventAggregator events, IWindowManager windowManager)
            : base(events, windowManager)
        {
            DisplayName = "Message prompt page";
        }
        #endregion

        #region 属性
        /// <summary>
        /// 消息的标题，默认：Information
        /// </summary>
        public string Title
        {
            get => this._title;
            private set => this.Set(ref this._title, value);
        }
        private string _title;

        /// <summary>
        /// 确认按钮的文本
        /// </summary>
        public string YesContent
        {
            get => this._yesContent;
            private set => this.Set(ref this._yesContent, value);
        }
        private string _yesContent;

        /// <summary>
        /// 取消按钮的文本
        /// </summary>
        public string CancelContent
        {
            get => this._cancelContent;
            private set => this.Set(ref this._cancelContent, value);
        }
        private string _cancelContent;

        /// <summary>
        /// 消息的内容
        /// </summary>
        public string Content
        {
            get => this._content;
            private set => this.Set(ref this._content, value);
        }
        private string _content;

        /// <summary>
        /// 是否显示【确认 or 是】按钮，默认：false
        /// </summary>
        public bool IsShowYes
        {
            get => this._isShowYes;
            private set => this.Set(ref this._isShowYes, value);
        }
        private bool _isShowYes = false;

        /// <summary>
        /// 窗体类型，默认：消息
        /// </summary>
        public Enum_MessageType MessType
        {
            get => this._messType;
            private set => this.Set(ref this._messType, value);
        }
        private Enum_MessageType _messType = Enum_MessageType.Information;
        #endregion

        #region Command事件
        /// <summary>
        /// 完成事件
        /// </summary>
        public void YesCommand()
        {
            TryClose(true);
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        public void CancelCommand()
        {
            TryClose(false);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="isShowYes">是否显示完成按钮</param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public bool? ShowDialog(string content, string title = null, bool isShowYes = false, Enum_MessageType type = Enum_MessageType.Information,
            string yesContent = null, string cancelContent = null)
        {
            Content = content;
            if (!string.IsNullOrWhiteSpace(yesContent))
            {
                YesContent = yesContent;
            }
            else
            {
                YesContent = "ButtonContent_Done".GetWord();
            }
            if (!string.IsNullOrWhiteSpace(cancelContent))
            {
                CancelContent = cancelContent;
            }
            else
            {
                if (isShowYes)
                {
                    CancelContent = "ButtonContent_Cancel".GetWord();
                }
                else
                {
                    CancelContent = "ButtonContent_Close".GetWord();
                }
            }
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
            }
            else
            {
                Title = "MessageTitle_Information".GetWord();
            }
            IsShowYes = isShowYes;
            MessType = type;
            bool? result = null;
            this.OnUIThread(() =>
            {
                result = this._windowManager.ShowDialog(this);
            });
            return result;
        }
        #endregion
    }
}