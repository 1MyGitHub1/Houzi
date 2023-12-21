using Totalab_L.Models;
using DeviceInterface;
using LabTech.Common;
using LabTech.UITheme;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
    /// MethodSelectorPage.xaml 的交互逻辑
    /// </summary>
    public partial class MethodSelectorPage : CustomWindow, INotifyPropertyChanged
    {
        public MethodSelectorPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region 属性
        public ListSortDirection SortDirection
        {
            get => _sortDirection;
            set
            {
                _sortDirection = value;
                NotifyPropertyChanged("SortDirection");
            }
        }
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        public int SelectionMethodIndex
        {
            get => _selectionMethodIndex;
            set
            {
                _selectionMethodIndex = value;
                NotifyPropertyChanged("SelectionMethodIndex");
            }
        }
        private int _selectionMethodIndex;


        public ObservableCollection<MethodSelectorItem> MethodSelectorList
        {
            get => _methodSelectorList;
            set
            {
                _methodSelectorList = value;
                NotifyPropertyChanged("MethodSelectorList");
            }
        }
        private ObservableCollection<MethodSelectorItem> _methodSelectorList = new ObservableCollection<MethodSelectorItem>();

        public string MethodFolderPath
        {
            get => _methodFolderPath;
            set
            {
                _methodFolderPath = value;
                NotifyPropertyChanged("MethodFolderPath");
            }
        }
        private string _methodFolderPath = string.Empty;

        public ShellPage ShellPageModule
        {
            get => _shellPageModule;
            set
            {
                _shellPageModule = value;
                NotifyPropertyChanged("ShellPageModule");
            }
        }
        private ShellPage _shellPageModule;

        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set
            {
                _isSettingsOpen = value;
                NotifyPropertyChanged("IsSettingsOpen");
            }
        }
        private bool _isSettingsOpen;
        #endregion

        #region 事件
        //双击和完成按钮
        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectionMethodIndex < 0)
                    return;
                if (!IsSettingsOpen)
                {
                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    int count = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Enum.Exp_Status.Free).Count();
                    byte[] content = FileHelper.ReadDecrypt(MethodFolderPath + "\\" + MethodSelectorList[SelectionMethodIndex].MethodName + ".mth");
                    if (content != null)
                    {
                        GlobalInfo.Instance.CurrentMethod = XmlObjSerializer.Deserialize<MethodInfo>(content);
                    }
                    else
                    {
                        //GlobalInfo.Instance.CurrentMethod = new MethodInfo();
                        //GlobalInfo.Instance.SettingInfo = new SettingInfo();
                        //InitSettingInfo();
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            new MessagePage().ShowDialog("Message_Error1004".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information,this);
                        }));
                        return;
                    }
                    //GlobalInfo.Instance.CurrentMethod = (MethodInfo)DataToXml.LoadFromXml(MethodFolderPath + "\\" + MethodSelectorList[SelectionMethodIndex].MethodName + ".mth", typeof(MethodInfo));
                    if (!(GlobalInfo.Instance.IsHimassConnState && ShellPageModule.IsUseAutoSampler))
                    {
                        GlobalInfo.Instance.SampleInfos = new ObservableCollection<SampleItemInfo>();
                        //SampleHelper.CreateSampleInfos(1);//运行显示25
                        if (GlobalInfo.Instance.CurrentMethod.SampleInfos != null)
                        {
                            for (int i = 0; i < GlobalInfo.Instance.CurrentMethod.SampleInfos.Count; i++)
                            {
                                if (i < 25)
                                    GlobalInfo.Instance.SampleInfos[i] = GlobalInfo.Instance.CurrentMethod.SampleInfos[i];
                                else
                                    GlobalInfo.Instance.SampleInfos.Add(GlobalInfo.Instance.CurrentMethod.SampleInfos[i]);
                            }
                        }
                    }
                    GlobalInfo.Instance.SettingInfo = GlobalInfo.Instance.CurrentMethod.MethodSettingInfo;
                    //GlobalInfo.Instance.CurrentMethod.MethodName = MethodSelectorList[SelectionMethodIndex].MethodName;
                    GlobalInfo.Instance.CurrentMethod.MethodName = MethodFolderPath + "\\" + MethodSelectorList[SelectionMethodIndex].MethodName + ".mth";
                    SampleHelper.RefreshSamplerItemStatus(Enum.Exp_Status.Free);
                    if (GlobalInfo.Instance.IsHimassConnState && ShellPageModule.IsUseAutoSampler)
                    {
                        count = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Enum.Exp_Status.Free).Count();
                        for (int i = 0; i < count; i++)
                        {
                            AutoSampler_SamInfo info = new AutoSampler_SamInfo
                            {
                                SamID = GlobalInfo.Instance.SampleInfos[i].SampleGuid,
                                SamName = GlobalInfo.Instance.SampleInfos[i].SampleName,
                                //Location = GlobalInfo.Instance.SampleInfos[i].SampleLoc.Value,
                                IsAnalyze = GlobalInfo.Instance.SampleInfos[i].IsChecked,
                                AnalysisType = GlobalInfo.Instance.SampleInfos[i].MethodType.Value,
                                OverWash = GlobalInfo.Instance.SampleInfos[i].Overwash == null ? 0 : GlobalInfo.Instance.SampleInfos[i].Overwash.Value,
                                OperationMode = EnumSamOperationMode.Add
                            };
                            list.Add(info);
                        }
                    }
                    if (list.Count > 0)
                    {
                        ShellPageModule.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                }
                else
                {
                    byte[] content = FileHelper.ReadDecrypt(MethodFolderPath + "\\" + MethodSelectorList[SelectionMethodIndex].MethodName + ".para");
                    if (content != null)
                    {
                        GlobalInfo.MethodName = MethodSelectorList[SelectionMethodIndex].MethodName + ".para";
                        GlobalInfo.Instance.SettingInfo = XmlObjSerializer.Deserialize<SettingInfo>(content);
                        //InitSettingInfo();
                    }
                    else
                    {
                        //GlobalInfo.Instance.SettingInfo = new SettingInfo();
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            new MessagePage().ShowDialog("Message_Error1004".GetWord(), "MessageTitle_Information".GetWord(), false, Enum_MessageType.Information,this);
                        }));
                        return;
                    }
                    //GlobalInfo.Instance.SettingInfo = (SettingInfo)DataToXml.LoadFromXml(MethodFolderPath + "\\" + MethodSelectorList[SelectionMethodIndex].MethodName + ".para", typeof(SettingInfo));
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MethodSelectorPage [Btn_Done_Click]", ex);
                new MessagePage().ShowDialog("Message_Error1004".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                //CommonSampleMethod.ShowErrorMessagePage();

            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Btn_OpenMethodFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog()
                {
                    ShowNewFolderButton = false,
                    SelectedPath = MethodFolderPath,
                    Description = "Common_FolderBrowser_SelDestinaDir".GetWord()
                };
                if (!Directory.Exists(dialog.SelectedPath))
                    Directory.CreateDirectory(dialog.SelectedPath);
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if ((result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                    && dialog.SelectedPath != MethodFolderPath)
                {
                    MethodFolderPath = dialog.SelectedPath;
                    //初始化并解析这个文件夹下的方法文件
                    LoadDataInfoList(true);
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MethodSelectorPage [Btn_OpenMethodFolder_Click]", ex);
                SampleHelper.ShowErrorMessagePage();
            }
        }

        private void ToggBtn_SortDirection_Click(object sender, RoutedEventArgs e)
        {
            SortDirection = SortDirection == ListSortDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            LoadDataInfoList(false);
        }

        /// <summary>
        /// 双击打开方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MethodSeclectCommand(object sender, MouseButtonEventArgs e)
        {
            Btn_Done_Click(null, null);
        }
        #endregion

        #region 方法
        public bool? ShowDialog(ShellPage shellPage)
        {
            ShellPageModule = shellPage;

            try
            {
                if (string.IsNullOrWhiteSpace(MethodFolderPath))
                {
                    if(IsSettingsOpen)
                    {
                        string path = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "SettingPara");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        MethodFolderPath = path;
                    }
                    else
                    {
                        string path = System.IO.Path.Combine(SampleHelper.AssemblyDirectory, "Parameters", "Method");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        MethodFolderPath = path;
                    }
                    
                }
                if (!Directory.Exists(MethodFolderPath))
                    Directory.CreateDirectory(MethodFolderPath);
                LoadDataInfoList(true);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MethodSelectorPage [ShowDialog]", ex);
                SampleHelper.ShowErrorMessagePage();
            }
            return this.ShowDialog();
        }
        #endregion

        #region 私有方法
        private void LoadDataInfoList(bool isInitDataCount)
        {
            try
            {
                if (isInitDataCount)
                {
                    MethodSelectorList = new ObservableCollection<MethodSelectorItem>();
                    if (!string.IsNullOrWhiteSpace(MethodFolderPath))
                    {
                        FileInfo[] fileList = null;
                        if (!IsSettingsOpen)
                            fileList = FileHelper.GetFilesByCreationTimeDesc(MethodFolderPath, "*.mth");
                        else
                            fileList = FileHelper.GetFilesByCreationTimeDesc(MethodFolderPath, "*.para");
                        if (fileList != null)
                        {
                            for (int fileIndex = 0; fileIndex < fileList.Length; fileIndex++)
                            {
                                MethodSelectorItem methodSelectorItem = new MethodSelectorItem();
                                methodSelectorItem.MethodName = fileList[fileIndex].Name.Replace(fileList[fileIndex].Extension, "");
                                methodSelectorItem.MethodDate = fileList[fileIndex].LastWriteTime.ToString();
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    MethodSelectorList.Add(methodSelectorItem);
                                }));
                            }
                        }
                    }
                }
                //按时间升序
                IOrderedEnumerable<MethodSelectorItem> sqlOrder = null;
                if (SortDirection == ListSortDirection.Ascending)
                    sqlOrder = MethodSelectorList.OrderBy(item => item.MethodDate);
                else
                    sqlOrder = MethodSelectorList.OrderByDescending(item => item.MethodDate);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    MethodSelectorList = new ObservableCollection<MethodSelectorItem>(sqlOrder);
                }));
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MethodSelectorPage [LoadDataInfoList]", ex);
                SampleHelper.ShowErrorMessagePage();
            }
        }
        public void InitSettingInfo()
        {
            GlobalInfo.Instance.SettingInfo.PreWashInfos = new ObservableCollection<PreWashItemInfo>
            {
                    new PreWashItemInfo
                    {
                        StepName = "AutoSampler_Main_PreFlush".GetWord(),
                        IsOpenAction = true,
                        WashLoc="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
            };
            GlobalInfo.Instance.SettingInfo.PreRunningInfo = new ObservableCollection<AnalysInfo>
            {
                    new AnalysInfo
                    {
                        WashPumpSpeed = 40,
                        WashTimeTypeIndex = 1,
                        WashTime = 10
                    },
                    new AnalysInfo
                    {
                        //WashSpeedTypeIndex = 1,
                        WashPumpSpeed = 40,
                        //WashTimeTypeIndex = 1,
                        WashTime = 10
                    },
            };
            GlobalInfo.Instance.SettingInfo.AfterRunningInfo = new ObservableCollection<ParaItemInfo>
            { 
                    new ParaItemInfo
                    {
                        StepName = "AutoSampler_Main_PostRun".GetWord(),
                        WashAction="AutoSampler_Main_InjectNeedleFlushSam".GetWord(),
                        WashActionKey=1,
                        WashLoc="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
                    new ParaItemInfo
                    {
                        WashAction="AutoSampler_Main_InjectNeedleFlushStdSam".GetWord(),
                        WashActionKey=2,
                        WashLoc ="1",
                        WashPumpSpeed = 40,
                        WashTime = 3
                    },
            };
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