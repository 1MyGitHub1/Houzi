using DeviceInterface;
using LabTech.Common;
using LabTech.UITheme;
using Mass.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.BaseServer;
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
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : CustomWindow, INotifyPropertyChanged
    {
        public AboutPage()
        {
            InitializeComponent();
            this.DataContext = this;
            SoftwareVersion = "V1.0.0.7";
            try
            {
                RegistSN registSN = RegistSN.GetInstance("LabMonsterTotalab-L");
                LicenseType = registSN.GetPower();
                ValidityPeriod = registSN.GetValidPeriod();
                RemainingValidity = registSN.GetCanUseDays();
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("Totalab-L AboutPage", ex);
                new MessagePage().ShowDialog("Message_Error1000".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
            //Thread thread = new Thread(() =>
            //{
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        new MessagePage().ShowDialog("Message_Error1000".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            //    });
            //});
            //thread.Start();
        }

        #region 属性
        /// <summary>
        /// 软件版本
        /// </summary>
        public string SoftwareVersion
        {
            get => this._softwareVersion;
            set
            {
                _softwareVersion = value;
                NotifyPropertyChanged("SoftwareVersion");
            }
        }
        private string _softwareVersion = "n/a";

        /// <summary>
        /// 硬件版本
        /// </summary>
        public string HardwareVersion
        {
            get => this._hardwareVersion;
            set
            {
                _hardwareVersion = value;
                NotifyPropertyChanged("HardwareVersion");
            }
        }
        private string _hardwareVersion = "V000.000.001";

        /// <summary>
        /// 序列号许可类型
        /// </summary>
        public string LicenseType
        {
            get => this._licenseType;
            set
            {
                _licenseType = value;
                NotifyPropertyChanged("LicenseType");
            }
        }
        private string _licenseType = "n/a";

        /// <summary>
        /// 序列号有效期（天数）
        /// </summary>
        public string ValidityPeriod
        {
            get => this._validityPeriod;
            set
            {
                _validityPeriod = value;
                NotifyPropertyChanged("ValidityPeriod");
            }
        }
        private string _validityPeriod = "n/a";

        /// <summary>
        /// 序列号剩余有效期（天数）
        /// </summary>
        public string RemainingValidity
        {
            get => this._remainingValidity;
            set
            {
                _remainingValidity = value;
                NotifyPropertyChanged("RemainingValidity");
            }
        }
        private string _remainingValidity = "n/a";

        /// <summary>
        /// 技术支持
        /// </summary>
        public string TechSupport
        {
            get => this._techSupport;
            set
            {
                _techSupport = value;
                NotifyPropertyChanged("TechSupport");
            }
        }
        private string _techSupport = "400-070-8778";

        /// <summary>
        /// 主页
        /// </summary>
        public ShellPage Control_ParentView
        {
            get => _control_ParentView;
            set
            {
                _control_ParentView = value;
                NotifyPropertyChanged("Control_ParentView");
            }
        }
        private ShellPage _control_ParentView;
        #endregion

        #region 事件
        private void Btn_ReplaceLicense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string licenseFileExtension = RegistSN.GetInstance("LabMonsterTotalab-L").GetLicenseFileExtension();

                //选择序列号的文件
                RadOpenFileDialogHelper openDialog = new RadOpenFileDialogHelper()
                {
                    Multiselect = false,
                    RestoreDirectory = false,
                    DefaultExt = licenseFileExtension,
                    Title = "Common_FileDialog_SelLicenseFile".GetWord(),
                    Filter = string.Format("{0}(*.{1})|*.{2}", "Common_FileDialog_LicenseFiles".GetWord(),
                    licenseFileExtension,
                    licenseFileExtension)
                };
                if (openDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(openDialog.FileName))
                {
                    //调用激活方法
                    RegistSN registSN = RegistSN.GetInstance("LabMonsterTotalab-L");
                    //registSN = RegistSN.GetInstance("LabMonsterAutoSampler");
                    ResultData resultData = registSN.ActiveByFilePath(openDialog.FileName, true);
                    if (resultData != null && resultData.IsSuccessful)
                    {
                        //更换许可文件成功
                        new MessagePage().ShowDialog("AboutInfo_ReplaceLicSuccessful".GetWord());
                        LicenseType = registSN.GetPower();
                        ValidityPeriod = registSN.GetValidPeriod();
                        RemainingValidity = registSN.GetCanUseDays();
                        Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.AutoSamplerDeviceType, Parameter = registSN.GetProductSN() });
                        if (Control_ParentView._IsFirst)
                        {
                            GlobalInfo.Instance.IsCanRunning = false;
                            GlobalInfo.Instance.IsBusy = true;
                            GlobalInfo.Instance.Totalab_LSerials.XWZHome();
                        }
                    }
                    else
                    {
                        MainLogHelper.Instance.Error("Totalab-L AboutPage [ReplaceLicenseCommand]" + resultData != null ? resultData.Result : string.Empty);
                        new MessagePage().ShowDialog("AboutInfo_ReplaceLicFailed".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("Totalab-L AboutPage [ReplaceLicenseCommand]", ex);
                new MessagePage().ShowDialog("AboutInfo_ReplaceLicFailed".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }
        #endregion

        #region 方法
        public void InitData(ShellPage shell)
        {
            Control_ParentView = shell;
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