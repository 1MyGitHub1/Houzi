using LabTech.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
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

namespace Totalab_L
{
    /// <summary>
    /// MessagePage.xaml 的交互逻辑
    /// </summary>
    public partial class MessagePage : Window, INotifyPropertyChanged
    {
        public MessagePage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region 属性
        /// <summary>
        /// 消息的标题，默认：Information
        /// </summary>
        public string PageTitle
        {
            get => this._pageTitle;
            private set
            {
                _pageTitle = value;
                NotifyPropertyChanged("PageTitle");
            }
        }
        private string _pageTitle;

        /// <summary>
        /// 确认按钮的文本
        /// </summary>
        public string YesContent
        {
            get => this._yesContent;
            private set
            {
                _yesContent = value;
                NotifyPropertyChanged("YesContent");
            }
        }
        private string _yesContent;

        /// <summary>
        /// 取消按钮的文本
        /// </summary>
        public string CancelContent
        {
            get => this._cancelContent;
            private set
            {
                _cancelContent = value;
                NotifyPropertyChanged("CancelContent");
            }
        }
        private string _cancelContent;

        /// <summary>
        /// 消息的内容
        /// </summary>
        public string MessageContent
        {
            get => this._messageContent;
            private set
            {
                _messageContent = value;
                NotifyPropertyChanged("MessageContent");
            }
        }
        private string _messageContent;

        /// <summary>
        /// 是否显示【确认 or 是】按钮，默认：false
        /// </summary>
        public bool IsShowYes
        {
            get => this._isShowYes;
            private set
            {
                _isShowYes = value;
                NotifyPropertyChanged("IsShowYes");
            }
        }
        private bool _isShowYes;

        /// <summary>
        /// 窗体类型，默认：消息
        /// </summary>
        public Enum_MessageType MessType
        {
            get => this._messType;
            private set
            {
                _messType = value;
                NotifyPropertyChanged("MessType");
            }
        }
        private Enum_MessageType _messType;
        #endregion

        #region 事件
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 打开消息窗体
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="isShowYes">是否显示完成按钮</param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public bool? ShowDialog(string content, string title = null, bool isShowYes = false, Enum_MessageType type = Enum_MessageType.Information,
            Window ownerWindows = null, string yesContent = null, string cancelContent = null)
        {
            MessageContent = content;
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
                PageTitle = title;
            }
            else
            {
                PageTitle = "MessageTitle_Information".GetWord();
            }
            IsShowYes = isShowYes;
            MessType = type;
            if (ownerWindows != null && ownerWindows.IsVisible)
            {
                this.Owner = ownerWindows;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow != this && Application.Current.MainWindow.IsVisible)
            {
                Type mainType = Application.Current.MainWindow.GetType();
                if (mainType.Name != "MessagePage")
                {
                    this.Owner = Application.Current.MainWindow;
                    this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
            }
            return this.ShowDialog();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}