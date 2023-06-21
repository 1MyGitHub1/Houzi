using Totalab_L.Enum;
using Totalab_L.Models;
using DeviceInterface;
using LabTech.Common;
using LabTech.UITheme;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Totalab_L;
using Totalab_L.Common;

namespace Totalab_L
{
    /// <summary>
    /// AddSamplePage.xaml 的交互逻辑
    /// </summary>
    public partial class AddSamplePage : CustomWindow, INotifyPropertyChanged
    {
        public AddSamplePage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region 属性
        public int AddNumber
        {
            get => _addNumber;
            set
            {
                _addNumber = value;
                NotifyPropertyChanged("AddNumber");
            }
        }
        private int _addNumber = 1;

        public ObservableCollection<string> SampleNameList
        {
            get => _sampleNameList;
            set
            {
                _sampleNameList = value;
                NotifyPropertyChanged("SampleNameList");
            }
        }
        private ObservableCollection<string> _sampleNameList = new ObservableCollection<string>();

        public ObservableCollection<string> SampleNamePosList
        {
            get => _sampleNamePosList;
            set
            {
                _sampleNamePosList = value;
                NotifyPropertyChanged("SampleNamePosList");
            }
        }
        private ObservableCollection<string> _sampleNamePosList = new ObservableCollection<string>();

        public string SampleName
        {
            get => _sampleName;
            set
            {
                _sampleName = value;
                NotifyPropertyChanged("SampleName");
            }
        }
        private string _sampleName;

        public string SamplePosName
        {
            get => _samplePosName;
            set
            {
                _samplePosName = value;
                NotifyPropertyChanged("SamplePosName");
            }
        }
        private string _samplePosName;

        public Enum_AnalysisType Add_AnanlysisType
        {
            get => _add_AnanlysisType;
            set
            {
                _add_AnanlysisType = value;
                NotifyPropertyChanged("Add_AnanlysisType");
            }
        }
        private Enum_AnalysisType _add_AnanlysisType;

        public IEnumerable<EnumMemberModel> AnanlysisTypeList
        {
            get => this._ananlysisTypeList;
            set
            {
                _ananlysisTypeList = value;
                NotifyPropertyChanged("AnanlysisTypeList");
            }
        }
        private IEnumerable<EnumMemberModel> _ananlysisTypeList;

        public bool IsAddNewSample
        {
            get => _isAddNewSample;
            set
            {
                _isAddNewSample = value;
                NotifyPropertyChanged("IsAddNewSample");
            }
        }
        private bool _isAddNewSample = true;

        public bool IsAddLast
        {
            get => _isAddLast;
            set
            {
                _isAddLast = value;
                NotifyPropertyChanged("IsAddLast");
            }
        }
        private bool _isAddLast = true;

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
        #endregion

        #region 事件
        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int PreSampletcount = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).Count();
                List<AutoSampler_SamInfo> list = new List<AutoSampler_SamInfo>();
                SampleItemInfo sampleInfo = new SampleItemInfo();
                if (IsAddNewSample)
                {
                    if (PreSampletcount == 0)
                    {
                        sampleInfo.SampleLoc = 1;
                        sampleInfo.SampleName = "sample";
                        sampleInfo.Overwash = 0;
                    }
                    else
                    {
                        sampleInfo.SampleName = string.IsNullOrEmpty(GlobalInfo.Instance.SampleInfos[PreSampletcount - 1].SampleName) ?
                               "sample" : SampleHelper.GetNextSampleID(GlobalInfo.Instance.SampleInfos[PreSampletcount - 1].SampleName);
                        sampleInfo.SampleLoc = GlobalInfo.Instance.SampleInfos[PreSampletcount - 1].SampleLoc == null ?
                           1 : SampleHelper.GetNextSampleLocation(GlobalInfo.Instance.SampleInfos[PreSampletcount - 1].SampleLoc);
                        sampleInfo.MethodType = GlobalInfo.Instance.SampleInfos[PreSampletcount - 1].MethodType;
                        sampleInfo.PreMethodType = GlobalInfo.Instance.SampleInfos[PreSampletcount - 1].PreMethodType;
                        sampleInfo.Overwash = GlobalInfo.Instance.SampleInfos[PreSampletcount - 1].Overwash;
                    }
                }
                if (!IsAddNewSample)
                {
                    sampleInfo.SampleName = SampleName;
                    int row = GlobalInfo.Instance.SampleInfos.IndexOf(GlobalInfo.Instance.SampleInfos.Where(m => m.SampleName == sampleInfo.SampleName).FirstOrDefault());
                    sampleInfo.SampleLoc = GlobalInfo.Instance.SampleInfos[row].SampleLoc;
                    sampleInfo.MethodType = GlobalInfo.Instance.SampleInfos[row].MethodType;
                    sampleInfo.PreMethodType = GlobalInfo.Instance.SampleInfos[row].PreMethodType;
                    sampleInfo.Overwash = GlobalInfo.Instance.SampleInfos[row].Overwash;
                }
                if (IsAddLast)///添加列表最后添加
                {
                    for (int i = 0; i < AddNumber; i++)
                    {
                        if (PreSampletcount + i < GlobalInfo.Instance.SampleInfos.Count)
                        {
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleGuid = Guid.NewGuid();
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleName = sampleInfo.SampleName;
                            sampleInfo.SampleName = IsAddNewSample == false ? sampleInfo.SampleName : SampleHelper.GetNextSampleID(sampleInfo.SampleName);
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].ExpStatus = Exp_Status.Ready;
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].IsChecked = true;
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleLoc = sampleInfo.SampleLoc;
                            sampleInfo.SampleLoc = IsAddNewSample == true ? SampleHelper.GetNextSampleLocation(sampleInfo.SampleLoc) : sampleInfo.SampleLoc;
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].MethodType = Add_AnanlysisType;
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].PreMethodType = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].MethodType;
                            GlobalInfo.Instance.SampleInfos[PreSampletcount + i].Overwash = sampleInfo.Overwash;
                            if (GlobalInfo.Instance.IsHimassConnState)
                            {
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleGuid,
                                    SamName = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleName,
                                    Location = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleLoc.Value,
                                    IsAnalyze = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].IsChecked,
                                    AnalysisType = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].MethodType.Value,
                                    OverWash = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].Overwash == null ? 0 : GlobalInfo.Instance.SampleInfos[PreSampletcount + i].Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Add
                                };
                                list.Add(info);
                            }
                        }
                        else
                        {
                            SampleItemInfo info = new SampleItemInfo();
                            info.SampleGuid = Guid.NewGuid();
                            info.SampleName = sampleInfo.SampleName;
                            sampleInfo.SampleName = IsAddNewSample == false ? sampleInfo.SampleName : SampleHelper.GetNextSampleID(sampleInfo.SampleName);
                            info.ExpStatus = Exp_Status.Ready;
                            info.IsChecked = true;
                            info.MethodType = Add_AnanlysisType;
                            info.PreMethodType = info.MethodType;
                            info.SampleLoc = sampleInfo.SampleLoc;
                            sampleInfo.SampleLoc = IsAddNewSample == true ? SampleHelper.GetNextSampleLocation(sampleInfo.SampleLoc) : sampleInfo.SampleLoc;
                            info.Overwash = sampleInfo.Overwash;
                            GlobalInfo.Instance.SampleInfos.Add(info);
                            if (GlobalInfo.Instance.IsHimassConnState)
                            {
                                AutoSampler_SamInfo samplinginfo = new AutoSampler_SamInfo
                                {
                                    SamID = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleGuid,
                                    SamName = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleName,
                                    Location = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].SampleLoc.Value,
                                    IsAnalyze = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].IsChecked,
                                    AnalysisType = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].MethodType.Value,
                                    OverWash = GlobalInfo.Instance.SampleInfos[PreSampletcount + i].Overwash == null ? 0 : GlobalInfo.Instance.SampleInfos[PreSampletcount + i].Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Add
                                };
                                list.Add(samplinginfo);
                            }
                        }

                    }
                }
                if (!IsAddLast)////插入
                {
                    int InsertRow = 0;
                    SampleItemInfo sampling = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus == Exp_Status.Ready).FirstOrDefault();
                    Guid guid = Guid.Empty;
                    if (sampling != null)
                    {
                        for (int i = GlobalInfo.Instance.SampleInfos.IndexOf(sampling); i < PreSampletcount; i++)
                        {
                            if (GlobalInfo.Instance.SampleInfos[i].SampleName == SamplePosName)
                            {
                                InsertRow = i;
                                if (GlobalInfo.Instance.SampleInfos[i].MethodType == Add_AnanlysisType)
                                    guid = GlobalInfo.Instance.SampleInfos[i].SampleGuid;
                                else
                                {
                                    List<SampleItemInfo> templist = GlobalInfo.Instance.SampleInfos.Where(m => GlobalInfo.Instance.SampleInfos.IndexOf(m) > InsertRow).ToList();
                                    if (list != null)
                                    {
                                        SampleItemInfo samplingInfo = templist.Where(m => m.MethodType == Add_AnanlysisType).FirstOrDefault();
                                        if (samplingInfo != null)
                                            guid = samplingInfo.SampleGuid;
                                        else
                                            guid = Guid.Empty;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        InsertRow = PreSampletcount;
                    }
                    for (int i = 0; i < AddNumber; i++)
                    {
                        if (InsertRow >= 0)
                        {
                            sampling = new SampleItemInfo
                            {
                                SampleName = sampleInfo.SampleName,
                                SampleLoc = sampleInfo.SampleLoc
                            };
                            sampleInfo.SampleName = IsAddNewSample == false ? sampleInfo.SampleName : SampleHelper.GetNextSampleID(sampleInfo.SampleName);
                            sampling.ExpStatus = Exp_Status.Ready;
                            sampling.IsChecked = true;
                            sampling.SampleGuid = Guid.NewGuid();
                            sampling.MethodType = Add_AnanlysisType;
                            sampling.PreMethodType = sampling.MethodType;
                            sampleInfo.SampleLoc = IsAddNewSample == true ? SampleHelper.GetNextSampleLocation(sampleInfo.SampleLoc) : sampleInfo.SampleLoc;
                            sampling.Overwash = sampleInfo.Overwash;
                            GlobalInfo.Instance.SampleInfos.Insert(InsertRow, sampling);
                            if (GlobalInfo.Instance.IsHimassConnState)
                            {
                                AutoSampler_SamInfo info = new AutoSampler_SamInfo
                                {
                                    SamID = sampling.SampleGuid,
                                    SamName = sampling.SampleName,
                                    Location = sampling.SampleLoc.Value,
                                    IsAnalyze = sampling.IsChecked,
                                    AnalysisType = sampling.MethodType.Value,
                                    OverWash = sampling.Overwash == null ? 0 : sampling.Overwash.Value,
                                    OperationMode = EnumSamOperationMode.Insert,
                                    NextSamID = guid
                                };
                                list.Add(info);
                            }
                        }
                    }
                }
                if (list.Count > 0)
                {
                    ShellPageModule.MainWindow_AutoSamplerSendSamDataEvent(null, new AutoSamplerEventArgs()
                    {
                        SamInfoList = list
                    });
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("AddSamplePage [Btn_Done_Click]", ex);
                new MessagePage().ShowDialog("Message_Error2002".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                //CommonSampleMethod.ShowErrorMessagePage();
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion

        #region 方法
        public bool? ShowDialog(ShellPage shellPage)
        {
            ShellPageModule = shellPage;

            try
            {

                    AnanlysisTypeList = EnumDataSource.FromType<Enum_AnalysisType>();

                    List<SampleItemInfo> samplingInfoList = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).ToList();
                    if (samplingInfoList != null)
                    {
                        for (int i = 0; i < samplingInfoList.Count; i++)
                        {
                            SampleItemInfo samplingInfo = samplingInfoList[i];
                            if (!string.IsNullOrWhiteSpace(samplingInfo.SampleName))
                            {
                                if (!SampleNameList.Contains(samplingInfo.SampleName))
                                    SampleNameList.Add(samplingInfo.SampleName);
                                if (!SampleNamePosList.Contains(samplingInfo.SampleName))
                                {
                                    if (samplingInfo.ExpStatus == Exp_Status.Ready)
                                        SampleNamePosList.Add(samplingInfo.SampleName);
                                }
                            }
                        }
                        if (SampleNameList.Count != 0)
                        {
                            SampleName = SampleNameList[0];
                        }
                        if (SampleNamePosList.Count != 0)
                            SamplePosName = SampleNamePosList[SampleNamePosList.Count - 1];
                    }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("AddSamplePage [ShowDialog]", ex);
                SampleHelper.ShowErrorMessagePage();
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