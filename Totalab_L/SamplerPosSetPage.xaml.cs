using Totalab_L.Models;
using LabTech.Common;
using LabTech.UITheme;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Totalab_L.Common;

namespace Totalab_L
{
    /// <summary>
    /// SamplerPosSetPage.xaml 的交互逻辑
    /// </summary>
    public partial class SamplerPosSetPage : CustomWindow, INotifyPropertyChanged
    {
        public SamplerPosSetPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 属性
        public int XCurrentPosition
        {
            get => _xCurrentPosition;
            set
            {
                _xCurrentPosition = value;
                Notify("XCurrentPosition");
            }
        }
        private int _xCurrentPosition;

        public int YCurrentPosition
        {
            get => _yCurrentPosition;
            set
            {
                _yCurrentPosition = value;
                Notify("YCurrentPosition");
            }
        }
        private int _yCurrentPosition;

        public int ZCurrentPosition
        {
            get => _zCurrentPosition;
            set
            {
                _zCurrentPosition = value;
                Notify("ZCurrentPosition");
            }
        }
        private int _zCurrentPosition = 6000;

        public bool IsNeedYPos
        {
            get => _isNeedYPos;
            set
            {
                _isNeedYPos = value;
                Notify("IsNeedYPos");
            }
        }
        private bool _isNeedYPos;

        public ObservableCollection<ItemData> CircleItemList
        {
            get => this._circleItemList;
            set
            {
                _circleItemList = value;
                Notify("CircleItemList");
            }
        }
        private ObservableCollection<ItemData> _circleItemList = new ObservableCollection<ItemData>();

        public Dictionary<int, int> CircleGroupList
        {
            get => this._circleGroupList;
            set
            {
                _circleGroupList = value;
                Notify("CircleGroupList");
            }
        }
        private Dictionary<int, int> _circleGroupList;

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                Notify("CurrentPosition");
            }
        }
        private int _currentPosition;

        public string PosType
        {
            get => _posType;
            set
            {
                _posType = value;
                Notify("PosType");
            }
        }
        private string _posType = "home";

        #endregion

        #region 事件
        public void GoToWash1Pos(object sender, RoutedEventArgs e)
        {
            IsNeedYPos = false;
            PosType = "wash1";
            XCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash1X;
            ZCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash1Z;
        }

        public void GoToWash2Pos(object sender, RoutedEventArgs e)
        {
            IsNeedYPos = false;
            PosType = "wash2";
            XCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash1X;
            ZCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash1Z;
        }

        public void GoToWash3Pos(object sender, RoutedEventArgs e)
        {
            IsNeedYPos = false;
            PosType = "wash3";
            XCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash1X;
            ZCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash1Z;
        }
        public void GoToHomePos(object sender, RoutedEventArgs e)
        {
            IsNeedYPos = false;
            PosType = "home";
            XCurrentPosition = GlobalInfo.Instance.SamplerPos.HomeX;
            ZCurrentPosition = GlobalInfo.Instance.SamplerPos.HomeZ;
        }
        public void SetGoToPos(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton radioButton = sender as RadioButton;
                int num = Convert.ToInt32(radioButton.Tag);
                IsNeedYPos = true;
                if (num == 1)
                {
                    PosType = "1";
                    XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e1X;
                    YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e1Y;
                }
                if (num == 33)
                {
                    PosType = "2";
                    XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e2X;
                    YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e2Y;
                }
                if (num == 58)
                {
                    PosType = "3";
                    XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e3X;
                    YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e3Y;
                }
                if (num == 78)
                {
                    PosType = "4";
                    XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e4X;
                    YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e4Y;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[SamplerPosSetPage]SetGoToPos：", ex);
            }

        }
        public void MoveToZCommand(object sender, RoutedEventArgs e)
        {
            //GlobalInfo.Instance.Totalab_LSerials.MoveToXYZAlone(0x02, ZCurrentPosition);
        }

        public void MoveToXYCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNeedYPos)
                {
                    if (PosType == "1")
                    {

                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e1X;
                        YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e1Y;
                    }
                    if (PosType == "2")
                    {
                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e2X;
                        YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e2Y;
                    }
                    if (PosType == "3")
                    {
                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e3X;
                        YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e3Y;
                    }
                    if (PosType == "4")
                    {
                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e4X;
                        YCurrentPosition = GlobalInfo.Instance.SamplerPos.Samp1e4Y;
                    }

                    //GlobalInfo.Instance.Totalab_LSerials.MoveToXY(XCurrentPosition, YCurrentPosition);
                }
                else
                {
                    if (PosType == "wash1")
                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash1X;
                    if (PosType == "wash2")
                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash2X;
                    if (PosType == "wash3")
                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.Wash3X;
                    if (PosType == "home")
                        XCurrentPosition = GlobalInfo.Instance.SamplerPos.HomeX;
                    //GlobalInfo.Instance.Totalab_LSerials.MoveToXYZAlone(0x00, XCurrentPosition);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[SamplerPosSetPage]MoveToXYCommand：", ex);
            }
        }

        public void SavePosCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                string SavePath = "";
                SavePath = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SamplerPos.dat");
                //DataToXml.SaveToXml(SavePath, GlobalInfo.Instance.SamplerPos);
                byte[] content = XmlObjSerializer.Serialize(GlobalInfo.Instance.SamplerPos);
                bool result = FileHelper.WriteEncrypt(SavePath, content);
                if (result == true)
                    new MessagePage().ShowDialog("MessageContent_SaveSuccessful".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information, this);
                else
                    new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[SamplerPosSetPage]SavePosCommand：", ex);
                new MessagePage().ShowDialog("Message_Error1002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        public void HomeCommand(object sender, RoutedEventArgs e)
        {
            //GlobalInfo.Instance.Totalab_LSerials.XYZHome();
        }
        #endregion

        #region 方法
        //public bool? ShowDialog()
        //{
        //    //bool? result = null;
        //    //this.OnUIThread(() =>
        //    //{
        //    //    result = this._windowManager.ShowDialog(this);
        //    //});
        //    return this.ShowDialog();
        //}

        public void InitData()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("[SamplerPosSetPage]InitData：", ex);
            }
        }
        #endregion

        private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();
        }
    }
}
