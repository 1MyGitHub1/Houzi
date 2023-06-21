using Totalab_L.Enum;
using Totalab_L.Models;
using DeviceInterface;
using LabTech.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Totalab_L.Common;

namespace Totalab_L
{
    /// <summary>
    /// SampleListModule.xaml 的交互逻辑
    /// </summary>
    public partial class SampleListModule : UserControl, INotifyPropertyChanged
    {
        public SampleListModule()
        {
            InitializeComponent();
            this.DataContext = this;
            SampleHelper.CreateSampleInfos(25);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region 属性
        public IEnumerable<EnumMemberModel> AnanlysisTypeList
        {
            get => this._ananlysisTypeList;
            set
            {
                _ananlysisTypeList = value;
                Notify("AnanlysisTypeList");
            }
        }
        private IEnumerable<EnumMemberModel> _ananlysisTypeList;

        public ShellPage Control_ParentView
        {
            get => _control_ParentView;
            set
            {
                _control_ParentView = value;
                Notify("Control_ParentView");
            }
        }
        private ShellPage _control_ParentView;

        public ObservableCollection<object> SelectedSampleInfo
        {
            get => _selectedSampleInfo;
            set
            {
                _selectedSampleInfo = value;
                Notify("SelectedSampleInfo");
            }
        }
        private ObservableCollection<object> _selectedSampleInfo;

        public int SelectedSampleIndex
        {
            get => _selectedSampleIndex;
            set
            {
                _selectedSampleIndex = value;
                Notify("SelectedSampleIndex");
            }
        }
        private int _selectedSampleIndex = -1;

        public bool IsSamplingSelectedAll
        {
            get => _isSamplingSelectedAll;
            set
            {
                _isSamplingSelectedAll = value;
                Notify("IsSamplingSelectedAll");
            }
        }
        private bool _isSamplingSelectedAll = true;

        public AddSamplePage Control_AddSampleView
        {
            get => _control_AddSampleView;
            set
            {
                _control_AddSampleView = value;
                Notify("AddSamplePage");
            }
        }
        private AddSamplePage _control_AddSampleView;

        public Exp_Status ExpStatus
        {
            get => _expStatus;
            set
            {
                _expStatus = value;
                Notify("ExpStatus");
            }
        }
        private Exp_Status _expStatus = Exp_Status.Free;

        public bool IsCanContinue
        {
            get => _isCanContinue;
            set
            {
                _isCanContinue = value;
                Notify("IsCanContinue");
            }
        }
        private bool _isCanContinue;

        public  EnumSamOperationMode? OperationMode
        {
            get => _operationMode;
            set
            {
                _operationMode = value;
                Notify("OperationMode");
            }
        }
        private EnumSamOperationMode? _operationMode;

        public bool IsStopWash = false;
        #endregion

        #region 变量
        
        SampleItemInfo CopySamplingInfo;
        CancellationTokenSource methodCancelSource;
        Thread methodTask;
        //public int pauseType;/// 0,没有暂停，1.立即暂停 2.结束当前样暂停
        public int stopType;//0没有停止，1.结束当前样停止，2.立即停止
        long pauseSeconds;
        bool isPumpRun;//判断蠕动泵是否运行
        public bool IsMassSamplingFinish = false;
        public bool IsMassWashFinish = false;
        public bool IsMassWashStart = false;
        public bool IsAutoSamplingFinish = false;
        public bool IsAutoSamplingWashFinish = false;
        public bool IsErrorWash = false;
        public int samplingIndex;
        //public EnumSamOperationMode? operationMode;
        public string CurrnentLoc;
        bool isAnayze;
        bool isWash;
        //public bool IsReciveSampleLoc;
        bool IsReachSampleLoc;
        private ScrollViewer _dgSamplingScrollViewer = null;
        public bool IsStdSample;
        CancellationTokenSource washCancelSource;
        Thread washTask;
        public bool IsStartTask;
        public bool IsRunningFlow = false;
        //private int ConnectMaxTimes = 10;
        #endregion

        #region 事件
        public void PreviewKeyUpCommand(object sender, KeyEventArgs e)
        {
            try
            {
                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                if (e.Key == Key.Delete && SelectedSampleInfo != null)
                {
                    List<SampleItemInfo> samplingInfoList = new List<SampleItemInfo>();
                    //bool isSendDelete = false;
                    for (int delIndex = 0; delIndex < SelectedSampleInfo.Count; delIndex++)
                    {
                        SampleItemInfo sample = SelectedSampleInfo[delIndex] as SampleItemInfo;
                        if (sample.ExpStatus == Exp_Status.Ready)
                        {
                            samplingInfoList.Add(sample);
                            if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                            {
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = sample.SampleGuid,
                                    SamName = sample.SampleName,
                                    Location = sample.SampleLoc.Value,
                                    IsAnalyze = sample.IsChecked,
                                    AnalysisType = sample.MethodType.Value,
                                    OverWash = sample.Overwash == null ? 0 : sample.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Delete
                                };
                                list.Add(info);
                            }
                        }
                    }

                    if (samplingInfoList.Count > 0)
                    {
                        for (int i = 0; i < samplingInfoList.Count; i++)
                        {
                            int count = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleLoc == samplingInfoList[i].SampleLoc).Count();
                            //if (count == 1)
                            //{
                            //    int index = int.Parse(samplingInfoList[i].SampleLoc.ToString().Substring(2));
                            //    GlobalInfo.Instance.CircleItemList[index - 1].ItemStatus = Item_Status.Free;
                            //}
                            GlobalInfo.Instance.SampleInfos.Remove(samplingInfoList[i]);
                        }
                    }
                    SampleHelper.RefreshSamplerItemStatus(ExpStatus);
                    RefreshSampleNum();
                    //CommonSampleMethod.RefreshSamplerItemStatus(ExpStatus);
                }
                else if (e.Key == Key.Enter)
                {
                    int rowIndex = SelectedSampleIndex;
                    int start = 0;
                    int end = 0;
                    {
                        SampleItemInfo sampling = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Free).FirstOrDefault();
                        int PreSampletcount;
                        if (sampling != null)
                        {
                            PreSampletcount = GlobalInfo.Instance.SampleInfos.IndexOf(sampling);
                        }
                        else
                        {
                            PreSampletcount = GlobalInfo.Instance.SampleInfos.Count();
                        }
                        if (PreSampletcount > rowIndex) /////如果在前面的行进行数据输入修改
                        {
                            start = rowIndex;
                            end = PreSampletcount;
                        }
                        if (PreSampletcount + 1 == rowIndex) /////如果在前面的行进行数据输入修改
                        {
                            end = rowIndex;
                            start = PreSampletcount;
                        }
                        else
                        {
                            end = rowIndex + 1;
                            start = PreSampletcount;
                        }
                        for (int i = start; i < end; i++)
                        {
                            SampleItemInfo newSamplingInfo = GlobalInfo.Instance.SampleInfos[i];
                            newSamplingInfo.SampleNum = (i + 1);
                            if (newSamplingInfo.ExpStatus == Exp_Status.Free)
                            {
                                newSamplingInfo.SampleGuid = Guid.NewGuid();
                                newSamplingInfo.ExpStatus = Exp_Status.Ready;
                                newSamplingInfo.IsChecked = true;
                            }
                            if (i == 0)
                            {
                                //if (newSamplingInfo.SampleId == null)
                                //    newSamplingInfo.SampleId = "sample1";
                                if (newSamplingInfo.SampleLoc == null)
                                {
                                    newSamplingInfo.SampleLoc = 1;
                                    newSamplingInfo.PreSampleLoc = newSamplingInfo.SampleLoc;
                                }
                                if (newSamplingInfo.MethodType == null)
                                    newSamplingInfo.MethodType = Enum_AnalysisType.Quantitative;
                                if (newSamplingInfo.Overwash == null)
                                {
                                    newSamplingInfo.Overwash = 0;
                                    newSamplingInfo.PreOverwash = newSamplingInfo.Overwash;
                                }
                            }
                            else
                            {
                                SampleItemInfo lastSamplingInfo = GlobalInfo.Instance.SampleInfos[i - 1];
                                if (newSamplingInfo.SampleName == null)
                                {
                                    newSamplingInfo.SampleName = string.IsNullOrEmpty(lastSamplingInfo.SampleName) ? null : GetNextSampleID(lastSamplingInfo.SampleName);
                                    newSamplingInfo.PreSampleName = newSamplingInfo.SampleName;
                                }
                                if (newSamplingInfo.SampleLoc == null)
                                {
                                    newSamplingInfo.SampleLoc = lastSamplingInfo.SampleLoc == null ? 1 : SampleHelper.GetNextSampleLocation(lastSamplingInfo.SampleLoc);
                                    newSamplingInfo.PreSampleLoc = newSamplingInfo.SampleLoc;
                                }
                                if (newSamplingInfo.MethodType == null)
                                    newSamplingInfo.MethodType = lastSamplingInfo.MethodType;
                                if (newSamplingInfo.Overwash == null)
                                {
                                    newSamplingInfo.Overwash = lastSamplingInfo.Overwash;
                                    newSamplingInfo.PreOverwash = newSamplingInfo.Overwash;
                                }
                            }
                            newSamplingInfo.PreMethodType = newSamplingInfo.MethodType;
                            if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                            {
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = newSamplingInfo.SampleGuid,
                                    SamName = newSamplingInfo.SampleName,
                                    Location = newSamplingInfo.SampleLoc.Value,
                                    IsAnalyze = newSamplingInfo.IsChecked,
                                    AnalysisType = newSamplingInfo.MethodType.Value,
                                    OverWash = newSamplingInfo.Overwash == null ? 0 : newSamplingInfo.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Add
                                };
                                list.Add(info);
                            }
                        }
                    }
                    SampleHelper.RefreshSamplerItemStatus(ExpStatus);
                }
                if (list.Count > 0)
                {
                    Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                    {
                        SamInfoList = list
                    });
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [DataGrid_PreviewKeyDown]：", ex);
            }
        }

        public void SamplingSelectedAllCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SampleItemInfo> samplingInfoList = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Ready).ToList();
                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                if (samplingInfoList != null)
                {
                    for (int i = 0; i < samplingInfoList.Count; i++)
                    {
                        SampleItemInfo samplingInfo = samplingInfoList[i];
                        if (samplingInfo.IsChecked != IsSamplingSelectedAll)
                        {
                            samplingInfo.IsChecked = IsSamplingSelectedAll;
                            if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                            {
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = samplingInfo.SampleGuid,
                                    SamName = samplingInfo.SampleName,
                                    Location = samplingInfo.SampleLoc.Value,
                                    IsAnalyze = samplingInfo.IsChecked,
                                    AnalysisType = samplingInfo.MethodType.Value,
                                    OverWash = samplingInfo.Overwash == null ? 0 : samplingInfo.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Update
                                };

                                list.Add(info);
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [CheckBox_SamplingAll_Click]：", ex);
            }
        }

        public void SampleSelectCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                     && frameworkElement.DataContext is SampleItemInfo info && info != null && info.ExpStatus == Exp_Status.Ready)
                {
                    IsSamplingSelectedAll = GlobalInfo.Instance.SampleInfos.Where(item => item.ExpStatus == Exp_Status.Ready).Count(item => item.IsChecked == false) == 0 ? true : false;
                    if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                    {
                        SampleItemInfo samplingInfo = info;
                        List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                        AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                        {
                            SamID = samplingInfo.SampleGuid,
                            SamName = samplingInfo.SampleName,
                            Location = samplingInfo.SampleLoc.Value,
                            IsAnalyze = samplingInfo.IsChecked,
                            AnalysisType = samplingInfo.MethodType.Value,
                            OverWash = samplingInfo.Overwash == null ? 0 : samplingInfo.Overwash.Value,
                            OperationMode = EnumSamOperationMode.Update
                        };
                        list.Add(tempinfo);
                        if (list.Count > 0)
                        {
                            Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                            {
                                SamInfoList = list
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [SampleSelectCommand]：", ex);
            }
        }

        public void InsertRowCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                      && frameworkElement.DataContext is SampleItemInfo info && info != null && info.ExpStatus == Exp_Status.Ready)
                {
                    SampleItemInfo samplingInfo = info;
                    if (samplingInfo != null)
                    {
                        int insertIndex = SelectedSampleIndex + 1;
                        SampleItemInfo newSamplingInfo = new SampleItemInfo()
                        {
                            SampleGuid = Guid.NewGuid(),
                            ExpStatus = Exp_Status.Ready,
                            IsChecked = true,
                            SampleNum = samplingInfo.SampleNum,
                            SampleLoc = samplingInfo.SampleLoc == null ? 1 : samplingInfo.SampleLoc.Value + 1,
                            MethodType = samplingInfo.MethodType,
                            PreMethodType = samplingInfo.MethodType,
                            Overwash = samplingInfo.Overwash,
                        };
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = newSamplingInfo.SampleGuid,
                                SamName = newSamplingInfo.SampleName,
                                Location = newSamplingInfo.SampleLoc.Value,
                                IsAnalyze = newSamplingInfo.IsChecked,
                                AnalysisType = newSamplingInfo.MethodType.Value,
                                OverWash = newSamplingInfo.Overwash == null ? 0 : newSamplingInfo.Overwash.Value,
                            };
                            if (GlobalInfo.Instance.SampleInfos.Count > insertIndex)
                            {
                                SampleItemInfo nextSamplingInfo = GlobalInfo.Instance.SampleInfos[insertIndex];
                                if (nextSamplingInfo.ExpStatus != Exp_Status.Free)
                                {
                                    tempinfo.NextSamID = nextSamplingInfo.SampleGuid;
                                    tempinfo.OperationMode = EnumSamOperationMode.Insert;
                                }
                                else
                                {
                                    tempinfo.OperationMode = EnumSamOperationMode.Add;
                                }
                            }
                            else
                            {
                                tempinfo.OperationMode = EnumSamOperationMode.Add;
                            }
                            list.Add(tempinfo);
                            if (list.Count > 0)
                            {
                                Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                {
                                    SamInfoList = list
                                });
                            }
                        }
                        GlobalInfo.Instance.SampleInfos.Insert(insertIndex, newSamplingInfo);
                        IsSamplingSelectedAll = GlobalInfo.Instance.SampleInfos.Where(item => item.ExpStatus == Exp_Status.Ready).Count(item => item.IsChecked == false) == 0 ? true : false;
                        RefreshSampleNum();
                        SampleHelper.RefreshSamplerItemStatus(ExpStatus);
                    }
                }
            }
            catch (Exception ex) { MainLogHelper.Instance.Error("RunningPage [InsertRowCommand]：", ex); }
        }

        public void CopyCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                      && frameworkElement.DataContext is SampleItemInfo info && info != null && info.ExpStatus == Exp_Status.Ready)
                {
                    CopySamplingInfo = info;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [CopyCommand]：", ex);
            }
        }

        public void PasteHereCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CopySamplingInfo != null)
                {
                    SampleItemInfo samplingInfo = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex];
                    int samplingCount = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Ready).Count();
                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    if (SelectedSampleIndex < samplingCount)
                    {
                        GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].ExpStatus = CopySamplingInfo.ExpStatus;
                        GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].IsChecked = CopySamplingInfo.IsChecked;
                        GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash = CopySamplingInfo.Overwash;
                        GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleName = CopySamplingInfo.SampleName;
                        GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleLoc = CopySamplingInfo.SampleLoc;
                        GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].MethodType = CopySamplingInfo.MethodType;
                        GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].PreMethodType = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].MethodType;
                        if (GlobalInfo.Instance.IsHimassConnState)
                        {
                            AutoSampler_SamInfo info = new AutoSampler_SamInfo
                            {
                                SamID = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleGuid,
                                SamName = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleName,
                                Location = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleLoc.Value,
                                IsAnalyze = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].IsChecked,
                                AnalysisType = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].MethodType.Value,
                                OverWash = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash == null ? 0 : GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(info);
                        }
                    }
                    else
                    {
                        for (int i = samplingCount; i < SelectedSampleIndex + 1; i++)
                        {
                            GlobalInfo.Instance.SampleInfos[i].IsChecked = CopySamplingInfo.IsChecked;
                            GlobalInfo.Instance.SampleInfos[i].ExpStatus = CopySamplingInfo.ExpStatus;
                            GlobalInfo.Instance.SampleInfos[i].Overwash = CopySamplingInfo.Overwash;
                            GlobalInfo.Instance.SampleInfos[i].SampleName = CopySamplingInfo.SampleName;
                            GlobalInfo.Instance.SampleInfos[i].SampleLoc = CopySamplingInfo.SampleLoc;
                            GlobalInfo.Instance.SampleInfos[i].MethodType = CopySamplingInfo.MethodType;
                            GlobalInfo.Instance.SampleInfos[i].SampleGuid = Guid.NewGuid();
                            if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                            {
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = GlobalInfo.Instance.SampleInfos[i].SampleGuid,
                                    SamName = GlobalInfo.Instance.SampleInfos[i].SampleName,
                                    Location = GlobalInfo.Instance.SampleInfos[i].SampleLoc.Value,
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
                            Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                            {
                                SamInfoList = list
                            });
                        }
                        RefreshSampleNum();
                    }
                }
            }
            catch (Exception ex) { MainLogHelper.Instance.Error("RunningPage [PasteHereCommand]：", ex); }
        }

        public void DeleteCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSampleInfo != null)
                {
                    List<SampleItemInfo> samplingInfoList = new List<SampleItemInfo>();
                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    for (int delIndex = 0; delIndex < SelectedSampleInfo.Count; delIndex++)
                    {
                        SampleItemInfo sample = SelectedSampleInfo[delIndex] as SampleItemInfo;
                        if (sample.ExpStatus == Exp_Status.Ready)
                        {
                            samplingInfoList.Add(sample);
                        }
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            AutoSampler_SamInfo info = new AutoSampler_SamInfo
                            {
                                SamID = sample.SampleGuid,
                                SamName = sample.SampleName,
                                Location = sample.SampleLoc.Value,
                                IsAnalyze = sample.IsChecked,
                                AnalysisType = sample.MethodType.Value,
                                OverWash = sample.Overwash == null ? 0 : sample.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Delete
                            };
                            list.Add(info);
                        }
                    }
                    if (list.Count > 0)
                    {
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                    if (samplingInfoList.Count > 0)
                    {
                        for (int i = 0; i < samplingInfoList.Count; i++)
                        {
                            //int count = GlobalInfo.Instance.SampleInfos.Where(m => m.SampleLoc == samplingInfoList[i].SampleLoc).Count();
                            //if (count == 1)
                            //{
                            //    int index = int.Parse(samplingInfoList[i].SampleLoc.ToString().Substring(2));
                            //    GlobalInfo.Instance.CircleItemList[index - 1].ItemStatus = Item_Status.Free;
                            //}
                            GlobalInfo.Instance.SampleInfos.Remove(samplingInfoList[i]);
                        }
                    }
                    SampleHelper.RefreshSamplerItemStatus(ExpStatus);
                    RefreshSampleNum();
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [DeleteCommand]：", ex);

            }
        }

        public void SkipCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                  && frameworkElement.DataContext is SampleItemInfo info && info != null && info.ExpStatus == Exp_Status.Ready)
                {
                    info.IsChecked = false;
                    IsSamplingSelectedAll = GlobalInfo.Instance.SampleInfos.Where(item => item.ExpStatus == Exp_Status.Ready).Count(item => item.IsChecked == false) == 0 ? true : false;
                    if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                    {
                        SampleItemInfo samplingInfo = info;
                        List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                        AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                        {
                            SamID = samplingInfo.SampleGuid,
                            SamName = samplingInfo.SampleName,
                            Location = samplingInfo.SampleLoc.Value,
                            IsAnalyze = samplingInfo.IsChecked,
                            AnalysisType = samplingInfo.MethodType.Value,
                            OverWash = samplingInfo.Overwash == null ? 0 : samplingInfo.Overwash.Value,
                            OperationMode = EnumSamOperationMode.Update
                        };
                        list.Add(tempinfo);
                        if (list.Count > 0)
                        {
                            Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                            {
                                SamInfoList = list
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { MainLogHelper.Instance.Error("RunningPage [SkipCommand]：", ex); }
        }

        public void SampleNameLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                   && frameworkElement.DataContext is SampleItemInfo info && info != null && info.ExpStatus == Exp_Status.Ready)
                {
                    if (info.PreSampleName != info.SampleName)
                    {
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = info.SampleGuid,
                                SamName = info.SampleName,
                                Location = info.SampleLoc.Value,
                                IsAnalyze = info.IsChecked,
                                AnalysisType = info.MethodType.Value,
                                OverWash = info.Overwash == null ? 0 : info.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(tempinfo);
                            if (list.Count > 0)
                            {
                                Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                {
                                    SamInfoList = list
                                });
                            }
                        }
                        info.PreSampleName = info.SampleName;
                    }
                }
            }
            catch
            {

            }
        }

        public void OverWashLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                      && frameworkElement.DataContext is SampleItemInfo info && info != null && info.ExpStatus == Exp_Status.Ready)
                {
                    if (info.PreOverwash != info.Overwash)
                    {
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = info.SampleGuid,
                                SamName = info.SampleName,
                                Location = info.SampleLoc.Value,
                                IsAnalyze = info.IsChecked,
                                AnalysisType = info.MethodType.Value,
                                OverWash = info.Overwash == null ? 0 : info.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(tempinfo);
                            if (list.Count > 0)
                            {
                                Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                {
                                    SamInfoList = list
                                });
                            }
                        }
                        info.PreOverwash = info.Overwash;
                    }
                }
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [OverWashLostFocus]：", ex);
            }
        }
        public void StartThisRowCommand(object sender, RoutedEventArgs e)
        {

            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                 && frameworkElement.DataContext is SampleItemInfo samplinginfo && samplinginfo != null && samplinginfo.ExpStatus == Exp_Status.Ready)
                {
                    int row = GlobalInfo.Instance.SampleInfos.IndexOf(samplinginfo);
                    for (int i = 0; i < row; i++)
                    {
                        GlobalInfo.Instance.SampleInfos[i].IsChecked = false;
                    }
                    IsSamplingSelectedAll = GlobalInfo.Instance.SampleInfos.Where(item => item.ExpStatus == Exp_Status.Ready).Count(item => item.IsChecked == false) == 0 ? true : false;
                }
            }
            catch (Exception ex) { MainLogHelper.Instance.Error("RunningPage [StartThisRowCommand]：", ex); }
        }

        public void BringToTopCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                 && frameworkElement.DataContext is SampleItemInfo samplinginfo && samplinginfo != null && samplinginfo.ExpStatus == Exp_Status.Ready)
                {
                    //int row = GlobalInfo.Instance.SampleInfos.IndexOf(samplinginfo);
                    int row = GlobalInfo.Instance.SampleInfos.IndexOf(GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Ready).FirstOrDefault());
                    GlobalInfo.Instance.SampleInfos.Remove(samplinginfo);
                    GlobalInfo.Instance.SampleInfos.Insert(row, samplinginfo);
                    if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                    {
                        int count = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).Count();
                        List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                        AutoSampler_SamInfo info = new AutoSampler_SamInfo
                        {
                            SamID = samplinginfo.SampleGuid,
                            SamName = samplinginfo.SampleName,
                            Location = samplinginfo.SampleLoc.Value,
                            IsAnalyze = samplinginfo.IsChecked,
                            AnalysisType = samplinginfo.MethodType.Value,
                            OverWash = samplinginfo.Overwash == null ? 0 : samplinginfo.Overwash.Value,
                            OperationMode = EnumSamOperationMode.Delete
                        };
                        list.Add(info);
                        Guid guid = Guid.Empty;
                        for (int i = row + 1; i < count; i++)
                        {
                            if (GlobalInfo.Instance.SampleInfos[i].MethodType == list[0].AnalysisType)
                            {
                                guid = GlobalInfo.Instance.SampleInfos[i].SampleGuid;
                                break;
                            }
                        }
                        info = new AutoSampler_SamInfo
                        {
                            SamID = samplinginfo.SampleGuid,
                            SamName = samplinginfo.SampleName,
                            Location = samplinginfo.SampleLoc.Value,
                            IsAnalyze = samplinginfo.IsChecked,
                            AnalysisType = samplinginfo.MethodType.Value,
                            OverWash = samplinginfo.Overwash == null ? 0 : samplinginfo.Overwash.Value,
                            OperationMode = EnumSamOperationMode.Insert,
                            NextSamID = guid
                        };
                        list.Add(info);
                        if (list.Count > 0)
                        {
                            Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                            {
                                SamInfoList = list
                            });
                        }
                    }
                    RefreshSampleNum();
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("SampleListViewModel [BringToTopCommand]：", ex);
            }
        }

        public void AddSampleCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                Control_AddSampleView = new AddSamplePage();
                Control_AddSampleView.Owner = Application.Current.MainWindow;
                Control_AddSampleView.ShowDialog(Control_ParentView);
                RefreshSampleNum();
                SampleHelper.RefreshSamplerItemStatus(ExpStatus);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [AddSampleCommand]：", ex);
            }
        }

        public void OverWashFillDownCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSampleIndex < 0)
                    return;
                List<SampleItemInfo> samplingList = GlobalInfo.Instance.SampleInfos.
                    Select((value, index) => new { value, index }).Where(item => item.index > SelectedSampleIndex && item.value.ExpStatus == Exp_Status.Ready).Select(item => item.value).ToList();
                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                if (samplingList != null)
                {
                    for (int i = 0; i < samplingList.Count; i++)
                    {
                        SampleItemInfo info = samplingList[i];
                        info.Overwash = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash;
                        info.PreOverwash = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash;
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = info.SampleGuid,
                                SamName = info.SampleName,
                                Location = info.SampleLoc.Value,
                                IsAnalyze = info.IsChecked,
                                AnalysisType = info.MethodType.Value,
                                OverWash = info.Overwash == null ? 0 : info.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(tempinfo);
                        }
                    }
                    if (list.Count > 0)
                    {
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [OverWashFillDownCommand]：", ex);
            }
        }

        public void OverWashFillIncrementCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSampleIndex < 0)
                    return;
                if (GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash != null)
                    {
                        List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                        List<SampleItemInfo> samplingInfoList = GlobalInfo.Instance.SampleInfos
                            .Select((value, index) => new { value, index }).Where(item => item.index > SelectedSampleIndex && item.value.ExpStatus == Exp_Status.Ready).Select(item => item.value).ToList();
                    if (samplingInfoList != null)
                    {
                        long overwash = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash.Value;
                        for (int i = 0; i < samplingInfoList.Count; i++)
                        {
                            overwash++;
                            SampleItemInfo samplingInfoTemp = samplingInfoList[i];
                            samplingInfoTemp.Overwash = overwash;
                            samplingInfoTemp.PreOverwash = overwash;
                            if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                            {
                                AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                                {
                                    SamID = samplingInfoTemp.SampleGuid,
                                    SamName = samplingInfoTemp.SampleName,
                                    Location = samplingInfoTemp.SampleLoc.Value,
                                    IsAnalyze = samplingInfoTemp.IsChecked,
                                    AnalysisType = samplingInfoTemp.MethodType.Value,
                                    OverWash = samplingInfoTemp.Overwash == null ? 0 : samplingInfoTemp.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Update
                                };
                                list.Add(tempinfo);
                            }
                        }
                        if (list.Count > 0)
                        {
                            Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                            {
                                SamInfoList = list
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [Btn_Overwash_Sampling_FillDownClick]：", ex);
            }
        }

        public void SampleLocationFillDownCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSampleIndex < 0)
                    return;
                List<SampleItemInfo> samplingList = GlobalInfo.Instance.SampleInfos.
                Select((value, index) => new { value, index }).Where(item => item.index > SelectedSampleIndex && item.value.ExpStatus == Exp_Status.Ready).Select(item => item.value).ToList();
                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                if (samplingList != null)
                {
                    for (int i = 0; i < samplingList.Count; i++)
                    {
                        SampleItemInfo info = samplingList[i];
                        info.SampleLoc = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleLoc;
                        info.PreSampleLoc = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleLoc;
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = info.SampleGuid,
                                SamName = info.SampleName,
                                Location = info.SampleLoc.Value,
                                IsAnalyze = info.IsChecked,
                                AnalysisType = info.MethodType.Value,
                                OverWash = info.Overwash == null ? 0 : info.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(tempinfo);
                        }
                    }
                    if (list.Count > 0)
                    {
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                }
                SampleHelper.RefreshSamplerItemStatus(ExpStatus);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [OverWashFillDownCommand]：", ex);
            }
        }

        public void SampleLocationFillIncrementCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSampleIndex < 0)
                    return;
                if (GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].Overwash != null)
                {
                    List<SampleItemInfo> samplingInfoList = GlobalInfo.Instance.SampleInfos
                        .Select((value, index) => new { value, index }).Where(item => item.index > SelectedSampleIndex && item.value.ExpStatus == Exp_Status.Ready).Select(item => item.value).ToList();
                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    if (samplingInfoList != null)
                    {
                        int? loc = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleLoc;
                        for (int i = 0; i < samplingInfoList.Count; i++)
                        {
                            SampleItemInfo samplingInfoTemp = samplingInfoList[i];
                            samplingInfoTemp.SampleLoc = SampleHelper.GetNextSampleLocation(loc);
                            if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                            {
                                AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                                {
                                    SamID = samplingInfoTemp.SampleGuid,
                                    SamName = samplingInfoTemp.SampleName,
                                    Location = samplingInfoTemp.SampleLoc.Value,
                                    IsAnalyze = samplingInfoTemp.IsChecked,
                                    AnalysisType = samplingInfoTemp.MethodType.Value,
                                    OverWash = samplingInfoTemp.Overwash == null ? 0 : samplingInfoTemp.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Update
                                };
                                list.Add(tempinfo);
                            }
                            loc = samplingInfoTemp.SampleLoc;
                        }
                        if (list.Count > 0)
                        {
                            Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                            {
                                SamInfoList = list
                            });
                        }
                    }
                    SampleHelper.RefreshSamplerItemStatus(ExpStatus);
                }

            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [Btn_Overwash_Sampling_FillDownClick]：", ex);
            }
        }

        public void SampleNameFillDownCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSampleIndex < 0)
                    return;
                    List<SampleItemInfo> samplingList = GlobalInfo.Instance.SampleInfos.
                    Select((value, index) => new { value, index }).Where(item => item.index > SelectedSampleIndex && item.value.ExpStatus == Exp_Status.Ready).Select(item => item.value).ToList();
                   List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                if (samplingList != null)
                {
                    for (int i = 0; i < samplingList.Count; i++)
                    {
                        SampleItemInfo info = samplingList[i];
                        info.SampleName = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleName;
                        info.PreSampleName = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleName;
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = info.SampleGuid,
                                SamName = info.SampleName,
                                Location = info.SampleLoc.Value,
                                IsAnalyze = info.IsChecked,
                                AnalysisType = info.MethodType.Value,
                                OverWash = info.Overwash == null ? 0 : info.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(tempinfo);
                        }
                    }
                    if (list.Count > 0)
                    {
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [Btn_SampleID_Sampling_FillDownClick]：", ex);
            }
        }

        public void SampleNameFillIncrementCommand(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SelectedSampleIndex < 0)
                    return;
                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    List<SampleItemInfo> samplingInfoList = GlobalInfo.Instance.SampleInfos
                        .Select((value, index) => new { value, index }).Where(item => item.index > SelectedSampleIndex && item.value.ExpStatus == Exp_Status.Ready).Select(item => item.value).ToList();
                if (samplingInfoList != null)
                {
                    string lastSampleId = GlobalInfo.Instance.SampleInfos[SelectedSampleIndex].SampleName;
                    for (int i = 0; i < samplingInfoList.Count; i++)
                    {
                        SampleItemInfo samplingInfoTemp = samplingInfoList[i];
                        string newSampleID = GetNextSampleID(lastSampleId);
                        samplingInfoTemp.SampleName = newSampleID;
                        lastSampleId = newSampleID;
                        samplingInfoTemp.PreSampleName = newSampleID;
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = samplingInfoTemp.SampleGuid,
                                SamName = samplingInfoTemp.SampleName,
                                Location = samplingInfoTemp.SampleLoc.Value,
                                IsAnalyze = samplingInfoTemp.IsChecked,
                                AnalysisType = samplingInfoTemp.MethodType.Value,
                                OverWash = samplingInfoTemp.Overwash == null ? 0 : samplingInfoTemp.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(tempinfo);
                        }
                    }
                    if (list.Count > 0)
                    {
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [Btn_SampleID_Sampling_FillIncrementClick]：", ex);
            }
        }

        public void SampleLocationLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                      && frameworkElement.DataContext is SampleItemInfo info && info != null && info.ExpStatus == Exp_Status.Ready)
                {
                    if (info.PreSampleLoc != info.SampleLoc)
                    {
                        info.PreSampleLoc = info.SampleLoc;
                        SampleHelper.RefreshSamplerItemStatus(ExpStatus);
                        if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                        {
                            List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                            AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo
                            {
                                SamID = info.SampleGuid,
                                SamName = info.SampleName,
                                Location = info.SampleLoc.Value,
                                IsAnalyze = info.IsChecked,
                                AnalysisType = info.MethodType.Value,
                                OverWash = info.Overwash == null ? 0 : info.Overwash.Value,
                                OperationMode = EnumSamOperationMode.Update
                            };
                            list.Add(tempinfo);
                            if (list.Count > 0)
                            {
                                Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                {
                                    SamInfoList = list
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [SampleLocationLostFocus]：", ex);
            }
        }

        public void MethodTypeChangedCommand(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement frameworkElement && frameworkElement != null
                    && frameworkElement.DataContext is SampleItemInfo info && info != null
                    && info.ExpStatus == Exp_Status.Ready)
                {
                    if (GlobalInfo.Instance.IsHimassConnState && Control_ParentView.IsUseAutoSampler)
                    {
                        List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                        if (GlobalInfo.Instance.SampleInfos != null && GlobalInfo.Instance.SampleInfos.Count > 0)
                        {
                            SampleItemInfo sample = info;

                            if (sample.ExpStatus == Exp_Status.Ready)
                            {
                                if (sample.MethodType != sample.PreMethodType)
                                {
                                    list.Add(new AutoSampler_SamInfo()
                                    {
                                        SamID = sample.SampleGuid,
                                        SamName = sample.SampleName,
                                        Location = sample.SampleLoc.Value,
                                        IsAnalyze = sample.IsChecked,
                                        AnalysisType = sample.PreMethodType.Value,
                                        OverWash = sample.Overwash == null ? 0 : sample.Overwash.Value,
                                        OperationMode = EnumSamOperationMode.Delete

                                    });
                                    sample.PreMethodType = sample.MethodType;
                                    list.Add(new AutoSampler_SamInfo()
                                    {
                                        SamID = sample.SampleGuid,
                                        SamName = sample.SampleName,
                                        //Location = sample.SampleLoc.Value,
                                        IsAnalyze = sample.IsChecked,
                                        AnalysisType = sample.MethodType.Value,
                                        OverWash = sample.Overwash == null ? 0 : sample.Overwash.Value,
                                        OperationMode = EnumSamOperationMode.Add

                                    });
                                }
                            }
                        }
                        if (list.Count > 0)
                        {
                            Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                            {
                                SamInfoList = list
                            });
                        }
                    }
                    info.PreMethodType = info.MethodType;
                }
               
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [Cbox_MethodType_SelectionChanged]：", ex);
            }
        }
        private void GoToAnalyze(object sender, RoutedEventArgs e)
        {
            Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null, new ObjectEventArgs() { MessParamType = EnumMessParamType.ToAnalyzeModule });
        }
        public void TaskRunningCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GlobalInfo.Instance.SampleInfos == null)
                    return;
                ExpStatus = Exp_Status.Running;
                GlobalInfo.Instance.IsCanRunning = false;
                CurrnentLoc = "";
                IsStartTask = true;
                if (!GlobalInfo.Instance.IsHimassConnState)
                {
                    samplingIndex = 0;
                }
                methodTask = new Thread(MethodRun);
                methodTask.Start();
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [TaskRunningCommand]：", ex);
            }
            //methodCancelSource = new CancellationTokenSource();
            //methodTask = Task.Factory.StartNew(() => MethodRun(), methodCancelSource.Token);
        }

        public void ContinueCommand(object sender, RoutedEventArgs e)
        {
            ExpStatus = Exp_Status.Running;
        }

        public void PauseImmediatelyCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                ////pauseType = 1;
                ExpStatus = Exp_Status.Pause;
                IsCanContinue = true;
                pauseSeconds = DateTime.Now.Ticks / 10000;
                if (RunningStep_Status.OpenPumpOk == GlobalInfo.Instance.RunningStep)
                {
                    isPumpRun = false;
                    GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                }
                this.Dispatcher.Invoke(new Action(delegate
                {
                    GlobalInfo.Instance.LogInfo.Insert(0,DateTime.Now.ToString("hh:mm:ss") + "立即暂停");
                    GlobalInfo.Instance.LogInfo.Insert(0,DateTime.Now.ToString("hh:mm:ss") + "蠕动泵停止");
                }));
            }
            catch { }
        }

        public void PauseAfterCurrentSamCommand(object sender, RoutedEventArgs e)
        {
            //pauseType = 2;
            ExpStatus = Exp_Status.PauseCurrentSample;
            IsCanContinue = false;
        }

        public void StopImmediatelyCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                //GlobalInfo.Instance.Totalab_LSerials.Init();
                stopType = 2;
                IsStartTask = false;
                IsStopWash = true;
                //if (methodCancelSource != null)
                //    methodCancelSource.Cancel();
                if (GlobalInfo.Instance.IsMotorXError == false && GlobalInfo.Instance.IsMotorWError == false && GlobalInfo.Instance.IsMotorZError == false)
                {
                    washTask = new Thread(WashRun);
                    washTask.Start();
                }
                if(!GlobalInfo.Instance.IsHimassConnState)
                {
                    ExpStatus = Exp_Status.Complete;
                    GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus = Exp_Status.Stop;
                }
               
                methodTask.Abort();
            }
            catch { }
        }
        
        /// <summary>
        /// 当前样品错误，走清洗流程
        /// </summary>
        public void ErrorWash()
        {
            try
            {
                IsStartTask = false;
                IsErrorWash = true;
                washTask = new Thread(WashRun);
                washTask.Start();
                methodTask.Abort();
            }
            catch
            {

            }
        }

        public void AfterWashStopImmediatelyCommand(object sender, RoutedEventArgs e)
        {
            ExpStatus = Exp_Status.Complete;
        }

        public void StopAfterCurrentSamCommand(object sender, RoutedEventArgs e)
        {
            stopType = 1;
            ExpStatus = Exp_Status.Complete;
            IsStartTask = false;
        }

        public void BackToReadyCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                stopType = 0;
                //pauseType = 0;
                ExpStatus = Exp_Status.Free;
                int count = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).Count();
                for (int i = 0; i < count; i++)
                    GlobalInfo.Instance.SampleInfos[i].ExpStatus = Exp_Status.Ready;
                SampleHelper.RefreshSamplerItemStatus(ExpStatus);
            }
            catch { }
        }
        #endregion

        #region 方法
        public void InitData(ShellPage shellPage)
        {
            Control_ParentView = shellPage;
            AnanlysisTypeList = EnumDataSource.FromType<Enum_AnalysisType>();
        }

        public static string GetNextSampleID(string sampleID)
        {
            try
            {
                Regex regex = new Regex(@"[1-9]\d*|0");
                if (regex.IsMatch(sampleID))
                {
                    Match last = regex.Matches(sampleID).OfType<Match>().Last();
                    if (int.TryParse(last.Value, out int num))
                    {
                        num++;
                        string prefix = string.Empty;
                        string suffix = string.Empty;
                        if (last.Index > 0)
                            prefix = sampleID.Substring(0, last.Index);
                        int sufIndex = last.Index + last.Length;
                        if (sufIndex < sampleID.Length)
                            suffix = sampleID.Substring(sufIndex);
                        sampleID = string.Format("{0}{1}{2}", prefix, num, suffix);
                    }
                    else
                    {
                        sampleID = string.Format("{0}{1}", sampleID, 1);
                    }
                }
                else
                {
                    sampleID = string.Format("{0}{1}", sampleID, 1);
                }
                return sampleID;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void RefreshSampleNum()
        {
            try
            {
                int Sampletcount = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).Count();
                for (int i = 0; i < Sampletcount; i++)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.SampleInfos[i].SampleNum = (i + 1);
                    }));
                }
                int freerowcount = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Free).Count();
                if (freerowcount < 25)
                {
                    SampleHelper.CreateSampleInfos(25 - freerowcount);
                }
            }
            catch
            {

            }
        }


        public void MethodRun()
        {
            try
            {
                IsRunningFlow = true;
                CurrnentLoc = "";
               // foreach (SampleItemInfo samplinginfo in GlobalInfo.Instance.SampleInfos)
                while(IsStartTask)
                //for (int j= samplingIndex; j< GlobalInfo.Instance.SampleInfos.Where(m=>m.ExpStatus != Exp_Status.Free).Count();j++)
                {
                    if (GlobalInfo.Instance.IsHimassConnState)
                    {
                        while (OperationMode != EnumSamOperationMode.StartInjection)
                        {
                            Thread.Sleep(20);
                            if (stopType == 2)
                                return;
                        }
                        OperationMode = null;
                    }
                    SampleItemInfo info = GlobalInfo.Instance.SampleInfos[samplingIndex];
                    if (GlobalInfo.Instance.IsHimassConnState)
                    {
                        info = GlobalInfo.Instance.SampleInfos[samplingIndex];
                    }
                    else
                    {
                        Control_ParentView._IsTestConnection = false;
                        samplingIndex = GlobalInfo.Instance.SampleInfos.IndexOf(GlobalInfo.Instance.SampleInfos.Where(m => m == info).FirstOrDefault());
                    }
                    long longseconds = 0;
                    if (info.ExpStatus != Exp_Status.Free)
                    {
                        if (!info.IsChecked)
                        {
                            if (!GlobalInfo.Instance.IsHimassConnState)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    info.ExpStatus = Exp_Status.Skip;
                                }));
                                samplingIndex++;
                                if (samplingIndex >= GlobalInfo.Instance.SampleInfos.Count())
                                {
                                    IsStartTask = false;
                                    ExpStatus = Exp_Status.Complete;
                                    IsRunningFlow = false;
                                    GlobalInfo.Instance.IsCanRunning = true;
                                    Control_ParentView._IsTestConnection = true;
                                    methodTask.Abort();
                                    //if (methodCancelSource != null)
                                    //    methodCancelSource.Cancel();
                                    break;
                                }
                                else
                                    continue;
                            }
                        }
                        if (samplingIndex != -1)
                        {
                            if (!GlobalInfo.Instance.IsHimassConnState)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    info.ExpStatus = Exp_Status.Running;
                                }));
                            }
                            SampleHelper.SetCircleStatus((int)info.SampleLoc, Item_Status.Running);
                            if (!GlobalInfo.Instance.IsHimassConnState)
                            {
                                if (GlobalInfo.Instance.SampleInfos.Count > samplingIndex + 1)
                                {
                                    if (GlobalInfo.Instance.SampleInfos[samplingIndex + 1].ExpStatus != Exp_Status.Free)
                                    {
                                        this.Dispatcher.Invoke((Action)(() =>
                                        {
                                            GlobalInfo.Instance.SampleInfos[samplingIndex + 1].ExpStatus = Exp_Status.Standby;
                                        }));
                                    }
                                }
                            }
                        }
                        if (!GlobalInfo.Instance.IsHimassConnState)
                        {
                            if (GlobalInfo.Instance.SettingInfo.SignalMode == 1)
                            {
                                if (stopType == 2)
                                {
                                    return;
                                }
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.RecvTrigger;
                            }
                            else
                            {
                                if (stopType == 2)
                                {
                                    return;
                                }
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.RecvTriggerOk;
                            }
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.RecvTriggerOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                            {
                                if (stopType == 2)
                                {
                                    return;
                                }
                                Thread.Sleep(20);
                            }
                        }
                        else
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.RecvTriggerOk;
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.RecvTriggerOk)
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2)
                            {
                                return;
                            }
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                        if(GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                        {
                            bool result = MotorActionHelper.MotorClearError();
                            if(result  == false)
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        longseconds = DateTime.Now.Ticks / 10000;
                        int count = 0;
                        if (GlobalInfo.Instance.SettingInfo.PreWashInfos[0].IsOpenAction)
                        {
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "开始预冲洗");
                            }));
                            for (int i = 0; i < GlobalInfo.Instance.SettingInfo.PreWashInfos.Count; i++)
                            {
                                if(GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc =="RinseLoc")
                                {
                                    if(GlobalInfo.Instance.SettingInfo.IsWash1Open)
                                    {
                                         if(CurrnentLoc != "W1")
                                        {
                                            GoToTargetPosition("W1");
                                        }
                                        else
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                    }
                                    else
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                }
                               else 
                               {
                                    if (GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc == "SampleLoc")
                                    {
                                        if (CurrnentLoc != GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc)
                                            GoToTargetPosition(info.SampleLoc.ToString());
                                        else
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;

                                    }
                                    else
                                    {
                                        if (CurrnentLoc != GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc)
                                            GoToTargetPosition(GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc);
                                        else
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                    }
                                        
                                }
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    break;
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                                {
                                    if (GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc == "RinseLoc"&& GlobalInfo.Instance.SettingInfo.IsWash1Open)
                                    {
                                        PumpRun((int)GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashTime);
                                    }
                                    else if(GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc != "RinseLoc")
                                    {
                                        long seconds = DateTime.Now.Ticks / 10000;
                                        long delaySeconds = 0;
                                        long delayPauseSends = 0;
                                        while ((GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashTime - delaySeconds) > 0)
                                        {
                                            if (stopType == 2)
                                                return;
                                            if (ExpStatus == Exp_Status.Pause)
                                            {
                                                Totalab_LCommon.DoEvents();
                                                Thread.Sleep(200);
                                                delayPauseSends = (DateTime.Now.Ticks / 10000 - pauseSeconds) / 1000;
                                                continue;
                                            }
                                            Totalab_LCommon.DoEvents();
                                            Thread.Sleep(200);
                                            delaySeconds = (DateTime.Now.Ticks / 10000 - seconds) / 1000 - delayPauseSends;
                                        }
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.PumpRunOk;
                                    }
                                }
                                else
                                {
                                    ConntectWaring();
                                }
                                if (GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc == "RinseLoc")
                                {
                                    if (GlobalInfo.Instance.SettingInfo.IsWash2Open)
                                    {
                                        if (CurrnentLoc != "W2")
                                        {
                                            GoToTargetPosition("W2");
                                        }
                                        else
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                    }
                                    else
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;

                                }
                                else
                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    break;
                                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                                {
                                    if (GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashLoc == "RinseLoc" && GlobalInfo.Instance.SettingInfo.IsWash2Open)
                                    {
                                        PumpRun((int)GlobalInfo.Instance.SettingInfo.PreWashInfos[i].WashTime);
                                    }
                                }
                                else
                                {
                                    ConntectWaring();
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {
                                if (!GlobalInfo.Instance.IsHimassConnState)
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        info.ExpStatus = Exp_Status.Error;
                                    }));
                                    SampleHelper.SetCircleStatus((int)info.SampleLoc, Item_Status.Error);
                                }
                                samplingIndex++;
                                continue;
                            }
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.PreWashOk;
                        }
                        else
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.PreWashOk;
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.PreWashOk)
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (CurrnentLoc != info.SampleLoc.ToString())
                            {
                                GoToTargetPosition(info.SampleLoc.ToString());
                            }
                            else
                                GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {
                                if (!GlobalInfo.Instance.IsHimassConnState)
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        info.ExpStatus = Exp_Status.Error;
                                    }));
                                    SampleHelper.SetCircleStatus((int)info.SampleLoc, Item_Status.Error);
                                }
                                samplingIndex++;
                                continue;
                            }
                            if (!GlobalInfo.Instance.IsHimassConnState)
                            {
                                if (GlobalInfo.Instance.SettingInfo.SignalMode == 2 || GlobalInfo.Instance.SettingInfo.SignalMode == 1)
                                {
                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SendTrigger;
                                    GlobalInfo.Instance.Totalab_LSerials.TriggerOut((byte)GlobalInfo.Instance.SettingInfo.SignalType);
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SendTriggerOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                                        Thread.Sleep(20);
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SendTriggerOk)
                                    {
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.CloseTrigger;
                                        if (GlobalInfo.Instance.SettingInfo.SignalType == 1)
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.TriggerOut(0x00);
                                        }
                                        else
                                            GlobalInfo.Instance.Totalab_LSerials.TriggerOut(0x01);
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                    }
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.CloseTriggerOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                                        Thread.Sleep(20);

                                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.CloseTriggerOk)
                                        ConntectWaring();
                                }
                                long seconds = DateTime.Now.Ticks / 10000;
                                long delaySeconds = 0;
                                long delayPauseSends = 0;
                                int time = GlobalInfo.Instance.SettingInfo.PreRunningInfo[0].WashTime + GlobalInfo.Instance.SettingInfo.PreRunningInfo[1].WashTime + GlobalInfo.Instance.SettingInfo.AnalysInfo.WashTime;
                                while ((time - delaySeconds) > 0)
                                {
                                    if (stopType == 2)
                                        return;
                                    if (ExpStatus == Exp_Status.Pause)
                                    {
                                       Totalab_LCommon.DoEvents();
                                        Thread.Sleep(200);
                                        delayPauseSends = (DateTime.Now.Ticks / 10000 - pauseSeconds) / 1000;
                                        continue;
                                    }
                                   Totalab_LCommon.DoEvents();
                                    Thread.Sleep(200);
                                    delaySeconds = (DateTime.Now.Ticks / 10000 - seconds) / 1000 - delayPauseSends;
                                }
                            }
                            else
                            {
                                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                                AutoSampler_SamInfo samplingtempinfo = new AutoSampler_SamInfo();
                                samplingtempinfo.SamID = info.SampleGuid;
                                samplingtempinfo.OperationMode = EnumSamOperationMode.LocationReady;
                                list.Add(samplingtempinfo);
                                Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                {
                                    SamInfoList = list
                                });
                                while (!IsMassSamplingFinish)
                                    Thread.Sleep(20);

                            }
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.AnalysOk;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.AnalysOk)
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            int time = 0;
                            if (!GlobalInfo.Instance.IsHimassConnState)
                                time = GlobalInfo.Instance.SettingInfo.AfterRunningInfo[0].WashTime + (int)info.Overwash;
                            else
                            {
                                if (IsStdSample)
                                    time = GlobalInfo.Instance.SettingInfo.AfterRunningInfo[1].WashTime + (int)info.Overwash;
                                else
                                    time = GlobalInfo.Instance.SettingInfo.AfterRunningInfo[0].WashTime + (int)info.Overwash;
                            }
                            for (int i = 2; i < GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Count; i++)
                                time += GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime;
                            if (time == 0)
                            {
                                if ((int)GlobalInfo.Instance.SettingInfo.Wash1Time + (int)info.Overwash != 0)
                                {

                                    this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "清洗");
                                    }));
                                    if (GlobalInfo.Instance.SettingInfo.IsWash1Open)
                                    {
                                        GoToTargetPosition("W1");
                                    }
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {
                                        if (!GlobalInfo.Instance.IsHimassConnState)
                                        {
                                            this.Dispatcher.Invoke((Action)(() =>
                                            {
                                                info.ExpStatus = Exp_Status.Error;
                                            }));
                                            SampleHelper.SetCircleStatus((int)info.SampleLoc, Item_Status.Error);
                                        }
                                        samplingIndex++;
                                        continue;
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                                    {
                                        PumpRun((int)GlobalInfo.Instance.SettingInfo.Wash1Time + (int)info.Overwash);
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                    }
                                    if (GlobalInfo.Instance.SettingInfo.IsWash1Open)
                                    {
                                        GoToTargetPosition("W2");
                                    }
                                    longseconds = DateTime.Now.Ticks / 10000;
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                    {
                                        if (!GlobalInfo.Instance.IsHimassConnState)
                                        {
                                            this.Dispatcher.Invoke((Action)(() =>
                                            {
                                                info.ExpStatus = Exp_Status.Error;
                                            }));
                                            SampleHelper.SetCircleStatus((int)info.SampleLoc, Item_Status.Error);
                                        }
                                        samplingIndex++;
                                        continue;
                                    }
                                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                                    {
                                        PumpRun((int)GlobalInfo.Instance.SettingInfo.Wash2Time + (int)info.Overwash);
                                    }
                                    else
                                    {
                                        ConntectWaring();
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < GlobalInfo.Instance.SettingInfo.AfterRunningInfo.Count; i++)
                                {
                                    if (GlobalInfo.Instance.IsHimassConnState)
                                    {
                                        if (IsStdSample && i == 0)
                                            continue;
                                        if (!IsStdSample && i == 1)
                                            continue;
                                    }
                                    else
                                    {
                                        if (i == 1)
                                            continue;
                                    }
                                    if ((int)GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime != 0)
                                    {
                                        this.Dispatcher.Invoke(new Action(delegate
                                        {
                                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "开始后运行" +
                                                GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashAction);
                                        }));
                                        if (CurrnentLoc != GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc)
                                        {
                                            if (GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc == "RinseLoc")
                                            {
                                                if (GlobalInfo.Instance.SettingInfo.IsWash1Open)
                                                    GoToTargetPosition("W1");
                                            }
                                            else
                                            {
                                                if (CurrnentLoc != GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc)
                                                    GoToTargetPosition(GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc);
                                                else
                                                    GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                                //GoToTargetPosition(GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc);
                                            }
                                        }
                                        else
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                            break;
                                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                                        {
                                            if (GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc == "RinseLoc" && GlobalInfo.Instance.SettingInfo.IsWash1Open)
                                            {
                                                int washtime = 0;
                                                if (i == 0 || i == 1)
                                                    washtime = (int)GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime + (int)info.Overwash;
                                                else
                                                    washtime = (int)GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime;
                                                PumpRun(washtime);
                                            }
                                            else if(GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc != "RinseLoc")
                                            {
                                                long seconds = DateTime.Now.Ticks / 10000;
                                                long delaySeconds = 0;
                                                long delayPauseSends = 0;
                                                int washtime = 0;
                                                if (i == 0 || i == 1)
                                                    washtime = (int)GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime + (int)info.Overwash;
                                                else
                                                    washtime = (int)GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime;
                                                while ((washtime - delaySeconds) > 0)
                                                {
                                                    if (stopType == 2)
                                                        return;
                                                    if (ExpStatus == Exp_Status.Pause)
                                                    {
                                                       Totalab_LCommon.DoEvents();
                                                        Thread.Sleep(200);
                                                        delayPauseSends = (DateTime.Now.Ticks / 10000 - pauseSeconds) / 1000;
                                                        continue;
                                                    }
                                                   Totalab_LCommon.DoEvents();
                                                    Thread.Sleep(200);
                                                    delaySeconds = (DateTime.Now.Ticks / 10000 - seconds) / 1000 - delayPauseSends;
                                                }
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.PumpRunOk;
                                            }
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                        }
                                        if (GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc == "RinseLoc")
                                        {
                                            if (GlobalInfo.Instance.SettingInfo.IsWash2Open)
                                                GoToTargetPosition("W2");
                                            else
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                        }
                                        else
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                                            break;
                                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                                        {
                                            if (GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashLoc == "RinseLoc" && GlobalInfo.Instance.SettingInfo.IsWash2Open)
                                            {
                                                int washtime = 0;
                                                if (i == 0 || i == 1)
                                                    washtime = (int)GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime + (int)info.Overwash;
                                                else
                                                    washtime = (int)GlobalInfo.Instance.SettingInfo.AfterRunningInfo[i].WashTime;
                                                PumpRun(washtime);
                                            }
                                        }
                                        else
                                        {
                                            ConntectWaring();
                                        }
                                    }
                                }
                            }
                            if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                            {
                                if (!GlobalInfo.Instance.IsHimassConnState)
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        info.ExpStatus = Exp_Status.Error;
                                    }));
                                    SampleHelper.SetCircleStatus((int)info.SampleLoc, Item_Status.Error);
                                }
                                samplingIndex++;
                                continue;
                            }
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.AfterRunningOk;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.AfterRunningOk)
                        {
                            //GlobalInfo.Instance.Totalab_LSerials.ZHOME();
                            GlobalInfo.Instance.RunningStep = RunningStep_Status.ZHomeOk;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                        //while (GlobalInfo.Instance.RunningStep != RunningStep_Status.ZHomeOk)
                        //{
                        //    Thread.Sleep(20);
                        //}
                        if (GlobalInfo.Instance.RunningStep == RunningStep_Status.ZHomeOk)
                        {
                            IsReachSampleLoc = false;
                            if (!GlobalInfo.Instance.IsHimassConnState)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    info.ExpStatus = Exp_Status.Complete;
                                }));
                                SampleHelper.SetCircleStatus((int)info.SampleLoc, Item_Status.Complete);
                            }
                            if (ExpStatus == Exp_Status.PauseCurrentSample)
                            {
                                //GlobalInfo.Instance.Totalab_LSerials.ZHOME();
                                CurrnentLoc = "";
                                IsCanContinue = true;
                            }
                            if (stopType == 1)
                            {
                                //methodCancelSource.Cancel();
                                ExpStatus = Exp_Status.Complete;
                                IsRunningFlow = false;
                                GlobalInfo.Instance.IsCanRunning = true;
                                methodTask.Abort();
                                break;
                            }
                            while (ExpStatus == Exp_Status.PauseCurrentSample)
                            {
                                Thread.Sleep(20);
                            }

                            if (GlobalInfo.Instance.IsHimassConnState)
                            {
                                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                                AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo();
                                tempinfo.SamID = info.SampleGuid;
                                tempinfo.OperationMode = EnumSamOperationMode.Complete;
                                GlobalInfo.Instance.IsBusy = false;
                                list.Add(tempinfo);
                                Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                                {
                                    SamInfoList = list
                                });
                               
                            }
                            else
                            {
                                samplingIndex++;
                                if (samplingIndex >= GlobalInfo.Instance.SampleInfos.Count())
                                 {
                                    IsStartTask = false;
                                    //GlobalInfo.Instance.Totalab_LSerials.ZHOME();
                                    ExpStatus = Exp_Status.Complete;
                                    IsRunningFlow = false;
                                    GlobalInfo.Instance.IsCanRunning = true;
                                    Control_ParentView._IsTestConnection = true;
                                    methodTask.Abort();
                                    //if (methodCancelSource != null)
                                    //    methodCancelSource.Cancel();
                                    break;
                                }
                            }
                        }
                    }

                    if (info.ExpStatus == Exp_Status.Free)
                    {
                        IsStartTask = false;
                        int count = 0;
                        //long longseconds = 0;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        //GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ +GlobalInfo.Instance.CalibrationInfo.ZResetPosition) / 50 * 3600));
                        //while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                        //{
                        //    Thread.Sleep(100);
                        //}
                        //GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        //GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                        //while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        //{
                        //    Thread.Sleep(100);
                        //}
                        //GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        //GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                        //while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        //{
                        //    Thread.Sleep(100);
                        //}
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition) / 50 * 3600));
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 )
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition / 50 * 3600));
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;

                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;

                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                        ExpStatus = Exp_Status.Complete;
                        IsRunningFlow = false;
                        GlobalInfo.Instance.IsCanRunning = true;
                        Control_ParentView._IsTestConnection = true;
                        methodTask.Abort();
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("MethodRun：", ex);
            }
        }

        public void ConntectWaring()
        {
            try
            {
                Control_ParentView.IsConnect = false;
                if (!GlobalInfo.Instance.IsHimassConnState)
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        new MessagePage().ShowDialog("Message_Error2013".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    }));
                }
                ExpStatus = Exp_Status.Complete;
                IsRunningFlow = false;
                GlobalInfo.Instance.IsCanRunning = true;
                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                if (GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus != Exp_Status.Complete)
                {
                    if (!GlobalInfo.Instance.IsHimassConnState)
                        GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus = Exp_Status.Error;
                }
                if (GlobalInfo.Instance.IsHimassConnState)
                {
                    Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                                  new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = Control_ParentView.IsConnect });
                          List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "进样失败" + GlobalInfo.Instance.SampleInfos[samplingIndex].SampleName);
                    }));
                    if (GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus != Exp_Status.Complete)
                    {
                        AutoSampler_SamInfo info = new AutoSampler_SamInfo
                        {
                            SamID = GlobalInfo.Instance.SampleInfos[samplingIndex].SampleGuid,
                            OperationMode = EnumSamOperationMode.Error
                        };
                        list.Add(info);
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                    //Control_ParentView.IsUseAutoSampler = false;
                }
                try
                {
                    if (methodTask != null)
                    {
                        methodTask.Abort();
                    }
                }
                catch
                { }
                try
                {
                    if (washTask != null)
                        washTask.Abort();
                }
                catch { }
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("ConntectWaring：", ex);
            }
        }
        public void GoToTargetPosition(string loc)
        {
            try
            {
                try
                {
                    long longseconds = DateTime.Now.Ticks / 10000;
                    int count = 0;
                    if (GlobalInfo.Instance.CurrentWorkType != Enum_MotorWorkType.Position)
                    {
                        GlobalInfo.Instance.IsMotorWSetWorkModeOk = false;
                        GlobalInfo.Instance.IsMotorXSetWorkModeOk = false;
                        GlobalInfo.Instance.IsMotorZSetWorkModeOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                        GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                        Thread.Sleep(300);
                        GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                        Thread.Sleep(300);
                        GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMortorWorkModeOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMortorWorkMode;
                                                GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x01, 0x01);
                                                Thread.Sleep(300);
                                                GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x02, 0x01);
                                                Thread.Sleep(300);
                                                GlobalInfo.Instance.Totalab_LSerials.SetMotorWorkType(0x03, 0x01);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                            
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition) / 50 * 3600));
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition / 50 * 3600));
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;

                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;

                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    Point pt = new Point();
                    int isCollisionStatus = 0;
                    if (loc == "W1")
                    {
                        pt = new Point((GlobalInfo.Instance.CalibrationInfo.W1PointX + GlobalInfo.Instance.TrayPanelHomeX) / 56.52 * 3600.0, GlobalInfo.Instance.CalibrationInfo.W1PointY * 3.0 * 10.0);
                    }
                    else if (loc == "W2")
                        pt = new Point((GlobalInfo.Instance.CalibrationInfo.W2PointX + GlobalInfo.Instance.TrayPanelHomeX) / 56.52 * 3600.0 , GlobalInfo.Instance.CalibrationInfo.W2PointY * 3.0 * 10.0);
                    else
                    {
                        pt = GetPositionInfoHelper.GetItemPosition(int.Parse(loc));
                        isCollisionStatus = GetPositionInfoHelper.GetXIsCollision(pt, int.Parse(loc));
                    }
                    if (isCollisionStatus == 1)
                    {
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((70 + GlobalInfo.Instance.TrayPanelHomeX) / 56.52 * 3600));
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
         
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((70 + GlobalInfo.Instance.TrayPanelHomeX) / 56.52 * 3600));
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                    }
                    else if (isCollisionStatus == 2)
                    {
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                        GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((265 + GlobalInfo.Instance.TrayPanelHomeX) / 56.52 * 3600));
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)((265 + GlobalInfo.Instance.TrayPanelHomeX) / 56.52 * 3600));
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        GlobalInfo.Instance.IsMotorXActionOk = false;
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                        count = 0;
                        while (true)
                        {
                            longseconds = DateTime.Now.Ticks / 10000;
                            while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                            {
                                Thread.Sleep(100);
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                            }
                            if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                            {
                                if (count < GlobalInfo.Instance.MaxConnectionTimes)
                                {
                                    GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                    try
                                    {
                                        foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                        {
                                            try
                                            {
                                                GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                                GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                            }
                                            catch (Exception ex)
                                            {
                                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }

                                    count++;
                                }
                                else
                                {
                                    ConntectWaring();
                                    return;
                                }
                            }
                            else
                            {
                                while (ExpStatus == Exp_Status.Pause)
                                {
                                    Thread.Sleep(20);
                                }
                                if (stopType == 2 && IsStopWash == false)
                                    return;
                                break;
                            }
                        }
                    }
                    //Point pt = GetPositionInfoHelper.GetWashPosition("W2");
                  
                    GlobalInfo.Instance.IsMotorXSetTargetPositionOk = false;
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                    if(isCollisionStatus == 0)
                    {
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);

                    }
                    else
                        GlobalInfo.Instance.IsMotorWSetTargetPositionOk = true;
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x01, (int)pt.X);
                                            if (isCollisionStatus == 0)
                                            {
                                                GlobalInfo.Instance.IsMotorWSetTargetPositionOk = false;
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x02, (int)pt.Y);

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    //
                    GlobalInfo.Instance.IsMotorXActionOk = false;
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                    if(isCollisionStatus == 0)
                    {
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                    }
                    else
                        GlobalInfo.Instance.IsMotorWActionOk = true;
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x01, 0x0f);
                                            if (isCollisionStatus == 0)
                                            {
                                                GlobalInfo.Instance.IsMotorWActionOk = false;
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x02, 0x0f);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    //GlobalInfo.Instance.IsMotorWActionOk = true;
                    GlobalInfo.Instance.IsMotorXActionOk = false;
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                 
                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                    if (isCollisionStatus == 0)
                    {
                        GlobalInfo.Instance.IsMotorWActionOk = false;
                        Thread.Sleep(50);
                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                    }
                    else
                        GlobalInfo.Instance.IsMotorWActionOk = true;
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                          
                                            //GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                            //Thread.Sleep(500);
                                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x01, 0x3f);
                                            if (isCollisionStatus == 0)
                                            {
                                                GlobalInfo.Instance.IsMotorWActionOk = false;
                                                Thread.Sleep(50);
                                                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x02, 0x3f);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.SettingInfo.SamplingDepth) / 50 * 3600));
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.SettingInfo.SamplingDepth) / 50 * 3600));
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 20)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    CurrnentLoc = loc;
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.GoToSampleLocOk;
                }
                catch (Exception ex)
                {
                    MainLogHelper.Instance.Error("SampleListModule [GoToTargetPosition]", ex);
                }
                finally
                {

                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("GoToTargetPosition：", ex);
            }
        }

        public void PumpRun(int pumptime)
        {
            try
            {
                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeed;
                GlobalInfo.Instance.Totalab_LSerials.PumpRunSpeed(GlobalInfo.Instance.SettingInfo.PumpSpeed1);
                long longseconds = DateTime.Now.Ticks / 10000;
                int count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetPumpSpeedOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                        if (stopType == 2)
                            return;
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetPumpSpeedOk)
                    {
                        if (GlobalInfo.Instance.IsHimassConnState)////和mass连接
                        {
                            if (isWash)
                            {
                                IsAutoSamplingWashFinish = false;
                                if (GlobalInfo.Instance.SettingInfo.AnalysMode == 1 && GlobalInfo.Instance.SettingInfo.AnalysMode == 2)////最小值
                                {
                                    long seconds = DateTime.Now.Ticks / 10000;
                                    while (!IsMassWashStart && (DateTime.Now.Ticks / 10000 - seconds) < 600000) ;
                                }
                            }

                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPump;
                        GlobalInfo.Instance.Totalab_LSerials.PumpRun();
                        isPumpRun = true;
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "蠕动泵运行");
                        }));
                        break;
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeed;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpRunSpeed(GlobalInfo.Instance.SettingInfo.PumpSpeed1);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MethodRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MethodRun]：", ex);
                            }

                          
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    Thread.Sleep(1);
                }
                longseconds = DateTime.Now.Ticks / 10000;
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.OpenPumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                        if (stopType == 2)
                            return;
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.OpenPumpOk)
                    {

                        long seconds = DateTime.Now.Ticks / 10000;
                        long delaySeconds = 0;
                        long delayPauseSends = 0;
                        while ((pumptime - delaySeconds) > 0)
                        {
                            if (stopType == 2)
                                return;
                            if (ExpStatus == Exp_Status.Pause)
                            {
                               Totalab_LCommon.DoEvents();
                                Thread.Sleep(200);
                                delayPauseSends = (DateTime.Now.Ticks / 10000 - pauseSeconds) / 1000;
                                continue;
                            }
                            if (isPumpRun == false)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.PumpRunSpeed(GlobalInfo.Instance.SettingInfo.PumpSpeed1);
                                isPumpRun = true;
                                this.Dispatcher.Invoke(new Action(delegate
                                {
                                    GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "蠕动泵运行");
                                }));
                            }
                           Totalab_LCommon.DoEvents();
                            Thread.Sleep(200);
                            delaySeconds = (DateTime.Now.Ticks / 10000 - seconds) / 1000 - delayPauseSends;
                        }
                        //Thread.Sleep(pumptime * 1000);
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                        break;
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPump;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpRun();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MethodRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MethodRun]：", ex);
                            }
                          
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    Thread.Sleep(1);
                }
                longseconds = DateTime.Now.Ticks / 10000;
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                        if (stopType == 2)
                            return;
                    }
                    if (stopType == 2)
                        return;
                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk)
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MethodRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MethodRun]：", ex);
                            }
                          
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    else
                        break;
                    Thread.Sleep(1);
                }
                GlobalInfo.Instance.RunningStep = RunningStep_Status.PumpRunOk;
                this.Dispatcher.Invoke(new Action(delegate
                {
                    GlobalInfo.Instance.LogInfo.Insert(0,DateTime.Now.ToString("hh:mm:ss") + "蠕动泵停止");
                }));
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("PumpRun：", ex);
            }
        }


        public void WashRun()
        {
            try
            {
                Thread.Sleep(100);
                GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                long longseconds = DateTime.Now.Ticks / 10000;
                int count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                    }
                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk)
                    {
                        if (count<GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MethodRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MethodRun]：", ex);
                            }
                          
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    else
                        break;
                    Thread.Sleep(1);
                }
                GlobalInfo.Instance.RunningStep = RunningStep_Status.PumpRunOk;
                //Thread.Sleep(20000);
                this.Dispatcher.Invoke(new Action(delegate
                {
                    GlobalInfo.Instance.LogInfo.Insert(0,DateTime.Now.ToString("hh:mm:ss") + "清洗");
                }));
                longseconds = DateTime.Now.Ticks / 10000;
                bool result = MotorActionHelper.MotorErrorStopImmediately();
                if(result == false)
                {
                    ConntectWaring();
                    return;
                }
                if (GlobalInfo.Instance.IsMotorXError || GlobalInfo.Instance.IsMotorWError || GlobalInfo.Instance.IsMotorZError)
                {
                    result = MotorActionHelper.MotorClearError();
                    if (result == false)
                    {
                        ConntectWaring();
                        return;
                    }
                }
                //MotorStop();
                CurrnentLoc = "W1";
                GoToTargetPosition("W1");
                while (GlobalInfo.Instance.RunningStep != RunningStep_Status.GoToSampleLocOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                {
                    Thread.Sleep(200);
                }
                if (GlobalInfo.Instance.RunningStep == RunningStep_Status.GoToSampleLocOk)
                {
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeed;
                    GlobalInfo.Instance.Totalab_LSerials.PumpRunSpeed(GlobalInfo.Instance.SettingInfo.PumpSpeed1);
                }
                longseconds = DateTime.Now.Ticks / 10000;
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetPumpSpeedOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.SetPumpSpeedOk)
                    {

                        GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPump;
                        GlobalInfo.Instance.Totalab_LSerials.PumpRun();
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "蠕动泵运行");
                        }));
                        break;
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetPumpSpeed;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpRunSpeed(GlobalInfo.Instance.SettingInfo.PumpSpeed1);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MethodRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MethodRun]：", ex);
                            }

                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    Thread.Sleep(1);
                }
                longseconds = DateTime.Now.Ticks / 10000;
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.OpenPumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.OpenPumpOk)
                    {
                        long seconds = DateTime.Now.Ticks / 10000;
                        long delaySeconds = 0;
                        while ((GlobalInfo.Instance.SettingInfo.Wash1Time - delaySeconds) > 0)
                        {
                           Totalab_LCommon.DoEvents();
                            Thread.Sleep(200);
                            delaySeconds = (DateTime.Now.Ticks / 10000 - seconds) / 1000;
                        }
                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                        break;
                    }
                    else
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.OpenPump;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpRun();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MethodRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MethodRun]：", ex);
                            }

                          
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    Thread.Sleep(1);
                }
                longseconds = DateTime.Now.Ticks / 10000;
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 5)
                    {
                        Thread.Sleep(20);
                    }
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "蠕动泵停止");
                    }));
                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.ClosePumpOk)
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.ClosePump;
                                        GlobalInfo.Instance.Totalab_LSerials.PumpStop();
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[MethodRun]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[MethodRun]：", ex);
                            }

                           
                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    else
                        break;
                    Thread.Sleep(1);
                }
                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition) / 50 * 3600));
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                    {
                        Thread.Sleep(100);
                    }
                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                        GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition / 50 * 3600));
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                            }

                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                    {
                        return;
                    }
                    else
                        break;
                }
                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                    {
                        Thread.Sleep(100);
                    }
                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                        GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                            }

                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                    {
                        return;
                    }
                    else
                        break;
                }
                GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                count = 0;
                while (true)
                {
                    longseconds = DateTime.Now.Ticks / 10000;
                    while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10 && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                    {
                        Thread.Sleep(100);
                    }
                    if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && GlobalInfo.Instance.RunningStep != RunningStep_Status.Error)
                    {
                        if (count < GlobalInfo.Instance.MaxConnectionTimes)
                        {
                            GlobalInfo.Instance.Totalab_LSerials.EndWork();
                            try
                            {
                                foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                {
                                    try
                                    {
                                        GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                        GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                        GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                        GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                    }
                                    catch (Exception ex)
                                    {
                                        MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                            }

                            count++;
                        }
                        else
                        {
                            ConntectWaring();
                            return;
                        }
                    }
                    if (GlobalInfo.Instance.RunningStep == RunningStep_Status.Error)
                    {
                        return;
                    }
                    else
                        break;
                }
                //GlobalInfo.Instance.Totalab_LSerials.ZHOME();
                GlobalInfo.Instance.IsCanRunning = true;
                IsErrorWash = false;
                IsRunningFlow = false;
                IsRunningFlow = false;
                GlobalInfo.Instance.IsBusy = false;
                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                AutoSampler_SamInfo tempinfo = new AutoSampler_SamInfo();
                tempinfo.SamID = GlobalInfo.Instance.SampleInfos[samplingIndex].SampleGuid;
                tempinfo.OperationMode = EnumSamOperationMode.Complete;
                list.Add(tempinfo);
                Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                {
                    SamInfoList = list
                });
                if (!GlobalInfo.Instance.IsHimassConnState)
                {
                    ExpStatus = Exp_Status.Complete;
                    GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus = Exp_Status.Stop;
                    Control_ParentView._IsTestConnection = true;
                }
                IsStopWash = false;
                stopType = 0;
                washTask.Abort();
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("WashRun：", ex);
            }
        }

 
        public void ThreadStop()
        {
            try
            {
                if (methodTask != null)
                {
                    methodTask.Abort();
                }
            }
            catch
            { }
            try
            {
                if (washTask != null)
                    washTask.Abort();
            }
            catch { }
            try
            {
                if (GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus != Exp_Status.Complete)
                {
                    if (!GlobalInfo.Instance.IsHimassConnState)
                        GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus = Exp_Status.Error;
                }
                ExpStatus = Exp_Status.Complete;
                bool result = MotorActionHelper.MotorErrorStopImmediately();
                if (!GlobalInfo.Instance.IsHimassConnState)
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        new MessagePage().ShowDialog("机械臂出错".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                    }));
                }
                if (result == false)
                {
                    Control_ParentView.IsConnect = false;
                    if (GlobalInfo.Instance.IsHimassConnState)
                        Control_ParentView.MainWindow_AutoSamplerSendObjectDataEvent(null,
                               new ObjectEventArgs() { MessParamType = EnumMessParamType.ASSerialPortConnOpen, Parameter = Control_ParentView.IsConnect });
                }

                if (GlobalInfo.Instance.IsHimassConnState)
                {
                    List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        GlobalInfo.Instance.LogInfo.Insert(0, DateTime.Now.ToString("hh:mm:ss") + "进样失败" + GlobalInfo.Instance.SampleInfos[samplingIndex].SampleName);
                    }));
                    if (GlobalInfo.Instance.SampleInfos[samplingIndex].ExpStatus != Exp_Status.Complete)
                    {
                        AutoSampler_SamInfo info = new AutoSampler_SamInfo
                        {
                            SamID = GlobalInfo.Instance.SampleInfos[samplingIndex].SampleGuid,
                            OperationMode = EnumSamOperationMode.Error
                        };
                        list.Add(info);
                        Control_ParentView.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                        {
                            SamInfoList = list
                        });
                    }
                    //Control_ParentView.IsUseAutoSampler = false;
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("ThreadStop：", ex);
            }
            finally
            {
                IsRunningFlow = false;
                GlobalInfo.Instance.IsCanRunning = true;
            }
        }
           
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dgSamplingScrollViewer = FindVisualChild<ScrollViewer>(Sample_DataGrid);
            Sample_DataGrid.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(DataGridSampling_ScrollChanged));
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                if (obj != null)
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                        if (child != null && child is T)
                        {
                            return (T)child;
                        }
                        T childItem = FindVisualChild<T>(child);
                        if (childItem != null) return childItem;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [FindVisualChild]：", ex);
                return null;
            }
        }

       public void StopMethod()
        {
            try
            {

                int count = 0;
                long longseconds = 0;
                //GlobalInfo.Instance.IsCanRunning = true;
                try
                {
                    if (methodTask != null)
                    {
                        methodTask.Abort();
                    }
                }
                catch
                { }
                if (!IsErrorWash)
                {
                    try
                    {
                        if (washTask != null)
                            washTask.Abort();
                    }
                    catch { }
                    if (!(GlobalInfo.Instance.IsMotorXError == false && GlobalInfo.Instance.IsMotorWError == false && GlobalInfo.Instance.IsMotorZError == false))
                    {
                        GlobalInfo.Instance.IsBusy = false;
                        ExpStatus = Exp_Status.Complete;
                        GlobalInfo.Instance.IsCanRunning = true;
                        return;
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                    GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)((GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition) / 50 * 3600));
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;
                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetTargetPositionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetTargetPosition;
                                            GlobalInfo.Instance.Totalab_LSerials.SetTargetPosition(0x03, (int)(GlobalInfo.Instance.TrayPanelHomeZ + GlobalInfo.Instance.CalibrationInfo.ZResetPosition / 50 * 3600));
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                    GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;

                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                            GlobalInfo.Instance.Totalab_LSerials.MotorAction(0x03, 0x0f);
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                    GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                    count = 0;
                    while (true)
                    {
                        longseconds = DateTime.Now.Ticks / 10000;

                        while (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk && (DateTime.Now.Ticks / 10000 - longseconds) / 1000 < 10)
                        {
                            Thread.Sleep(100);
                            if (stopType == 2 && IsStopWash == false)
                                return;
                        }
                        if (GlobalInfo.Instance.RunningStep != RunningStep_Status.SetMotorActionOk)
                        {
                            if (count < GlobalInfo.Instance.MaxConnectionTimes)
                            {
                                GlobalInfo.Instance.Totalab_LSerials.EndWork();
                                try
                                {
                                    foreach (string PortName in System.IO.Ports.SerialPort.GetPortNames())
                                    {
                                        try
                                        {
                                            GlobalInfo.Instance.Totalab_LSerials.SPort.PortName = PortName;
                                            GlobalInfo.Instance.Totalab_LSerials.StartWork();
                                            GlobalInfo.Instance.RunningStep = RunningStep_Status.SetMotorAction;
                                            GlobalInfo.Instance.Totalab_LSerials.MotorMove(0x03, 0x3f);
                                        }
                                        catch (Exception ex)
                                        {
                                            MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MainLogHelper.Instance.Error("[GoToTargetPosition]：", ex);
                                }

                                count++;
                            }
                            else
                            {
                                ConntectWaring();
                                return;
                            }
                        }
                        else
                        {
                            while (ExpStatus == Exp_Status.Pause)
                            {
                                Thread.Sleep(20);
                            }
                            if (stopType == 2 && IsStopWash == false)
                                return;
                            break;
                        }
                    }
                    GlobalInfo.Instance.IsBusy = false;
                }
              
                ExpStatus = Exp_Status.Complete;
                GlobalInfo.Instance.IsCanRunning = true;
                //methodTask.Abort();
            }
            catch
            {

            }
        }

        private void DataGridSampling_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (_dgSamplingScrollViewer.ExtentHeight == _dgSamplingScrollViewer.VerticalOffset + _dgSamplingScrollViewer.ViewportHeight)
                {

                    int freerowcount = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Free).Count();
                    if (GlobalInfo.Instance.SampleInfos.Count < 500 && freerowcount < 25)
                    {
                        SampleHelper.CreateSampleInfos(25 - freerowcount);
                    }
                }

            }

            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("RunningPage [DataGridSampling_ScrollChanged]：", ex);
            }
        }

       
    }
}