using LabTech.Common;
using LabTech.UITheme;
using Mass.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using UtilityLibrary;

namespace Totalab_L
{
    /// <summary>
    /// RegisterSNPage.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterSNPage : CustomWindow, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public RegisterSNPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region 属性
        public string LicenceFilePath
        {
            get => this._licenceFilePath;
            set
            {
                _licenceFilePath = value;
                NotifyPropertyChanged("LicenceFilePath");
            }
        }
        private string _licenceFilePath;

        public string VerifyMessage
        {
            get => this._verifyMessage;
            set
            {
                _verifyMessage = value;
                NotifyPropertyChanged("VerifyMessage");
            }
        }
        private string _verifyMessage;
        #endregion

        #region 事件
        private void Btn_BrowseFiles_Click(object sender, RoutedEventArgs e)
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
                    licenseFileExtension, licenseFileExtension)
                };
                if (openDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(openDialog.FileName))
                {
                    VerifyMessage = string.Empty;
                    LicenceFilePath = openDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("Totalab-L RegisterSNPage [Btn_BrowseFiles_Click]", ex);
                new MessagePage().ShowDialog("Message_Error1000".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        private void CustomWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Btn_Register_Click(null, null);
            }
        }

        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VerifyMessage = string.Empty;
                if (string.IsNullOrWhiteSpace(LicenceFilePath))
                {
                    VerifyMessage = "Common_Content_LicenseFileNull".GetWord();
                    return;
                }
                //调用激活方法
                RegistSN registSN = RegistSN.GetInstance("LabMonsterTotalab-L");
                ResultData resultData = registSN.ActiveByFilePath(LicenceFilePath, false);
                if (resultData != null && resultData.IsSuccessful)
                {
                    new MessagePage().ShowDialog("Common_Content_RegisterSuccessful".GetWord());
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    VerifyMessage = "Common_Content_LicenseFileError".GetWord();
                    MainLogHelper.Instance.Error("Totalab-L RegisterSNPage [Btn_Register_Click]" + resultData != null ? resultData.Result : string.Empty);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("Totalab-L RegisterSNPage [Btn_Register_Click]", ex);
                new MessagePage().ShowDialog("Message_Error1000".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                //this.DialogResult = false;
                //this.Close();
            }
        }
        #endregion
    }
}