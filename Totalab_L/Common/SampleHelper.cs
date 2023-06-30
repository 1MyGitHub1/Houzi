using LabTech.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Totalab_L.Enum;

namespace Totalab_L.Common
{
    public static class SampleHelper
    {
        public static void CreateSampleInfos(int number = 25)
        {
            try
            {
                if (GlobalInfo.Instance.SampleInfos == null)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.SampleInfos = new ObservableCollection<Models.SampleItemInfo>();
                    }));
                }
                for (int i = 0; i < number; i++)
                {
                    Models.SampleItemInfo samplingInfo = new Models.SampleItemInfo
                    {
                        SampleNum = null,
                        SampleName = null,
                        SampleLoc = null,
                        Overwash = null,
                        ExpStatus = Exp_Status.Free,
                        IsChecked = false
                    };
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.SampleInfos.Add(samplingInfo);
                    }));
                }
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("SampleHelper [CreateSampleInfos]", ex);
                ShowErrorMessagePage();
            }
        }

        public static void ShowErrorMessagePage(string messContent = null)
        {
            if (string.IsNullOrWhiteSpace(messContent))
            {
                messContent = "Message_Error1000".GetWord();
            }
            if (!GlobalInfo.Instance.IsHimassConnState)
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    new MessagePage().ShowDialog(messContent, "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
                }));
            }
        }



        public static bool GoToSamplerXYPos(int number)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("SampleHelper [GoToSamplerXYPos]", ex);
                ShowErrorMessagePage();
            }
            return false;
        }

        public static void RefreshSamplerItemStatus( Exp_Status status)
        {
            try
            {
                int sampleCount = GlobalInfo.Instance.SampleInfos.Where(m => m.ExpStatus != Exp_Status.Free).Count();
                if (status == Exp_Status.Free)
                {
                    for (int i = 0; i < GlobalInfo.Instance.TrayAInfos.TrayItemCount; i++)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.TrayAInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                        }));
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TrayBInfos.TrayItemCount; i++)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.TrayBInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                        }));
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TraySTD1Infos.TrayItemCount; i++)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.TraySTD1Infos.TrayItemList[i].ItemStatus = Item_Status.Free;
                        }));
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TraySTD2Infos.TrayItemCount; i++)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.TraySTD2Infos.TrayItemList[i].ItemStatus = Item_Status.Free;
                        }));
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TrayDInfos.TrayItemCount; i++)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.TrayDInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                        }));
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TrayEInfos.TrayItemCount; i++)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            GlobalInfo.Instance.TrayEInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                        }));
                    }
                }
                else
                {
                    for (int i = 0; i < GlobalInfo.Instance.TrayAInfos.TrayItemCount; i++)
                    {
                        if (GlobalInfo.Instance.TrayAInfos.TrayItemList[i].ItemStatus == Item_Status.Ready)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayAInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                            }));
                        }

                    }
                    for (int i = 0; i < GlobalInfo.Instance.TrayBInfos.TrayItemCount; i++)
                    {
                        if (GlobalInfo.Instance.TrayBInfos.TrayItemList[i].ItemStatus == Item_Status.Ready)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayBInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                            }));
                        }
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TraySTD1Infos.TrayItemCount; i++)
                    {
                        if (GlobalInfo.Instance.TraySTD1Infos.TrayItemList[i].ItemStatus == Item_Status.Ready)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TraySTD1Infos.TrayItemList[i].ItemStatus = Item_Status.Free;
                            }));
                        }
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TraySTD2Infos.TrayItemCount; i++)
                    {
                        if (GlobalInfo.Instance.TraySTD2Infos.TrayItemList[i].ItemStatus == Item_Status.Ready)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TraySTD2Infos.TrayItemList[i].ItemStatus = Item_Status.Free;
                            }));
                        }
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TrayDInfos.TrayItemCount; i++)
                    {
                        if (GlobalInfo.Instance.TrayDInfos.TrayItemList[i].ItemStatus == Item_Status.Ready)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayDInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                            }));
                        }
                    }
                    for (int i = 0; i < GlobalInfo.Instance.TrayEInfos.TrayItemCount; i++)
                    {
                        if (GlobalInfo.Instance.TrayEInfos.TrayItemList[i].ItemStatus == Item_Status.Ready)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayEInfos.TrayItemList[i].ItemStatus = Item_Status.Free;
                            }));
                        }
                    }
                }
                for (int i = 0; i < sampleCount; i++)
                {
                    int index = GlobalInfo.Instance.SampleInfos[i].SampleLoc.Value;
                    if (GlobalInfo.Instance.SampleInfos[i].ExpStatus == Exp_Status.Ready || GlobalInfo.Instance.SampleInfos[i].ExpStatus == Exp_Status.Standby)
                    {
                        if (index >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && index <= GlobalInfo.Instance.TrayAInfos.TrayEndNumber)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayAInfos.TrayItemList[index - 1].ItemStatus = Item_Status.Ready;
                            }));
                        }
                        else if (index >= GlobalInfo.Instance.TrayBInfos.TrayStartNumber && index <= GlobalInfo.Instance.TrayBInfos.TrayEndNumber)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayBInfos.TrayItemList[index - GlobalInfo.Instance.TrayBInfos.TrayStartNumber].ItemStatus = Item_Status.Ready;
                            }));
                        }
                        else if (index >= GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber && index <= GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TraySTD1Infos.TrayItemList[index - GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber].ItemStatus = Item_Status.Ready;
                            }));
                        }
                        else if (index >= GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber && index <= GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TraySTD2Infos.TrayItemList[index - GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber].ItemStatus = Item_Status.Ready;
                            }));
                        }
                        else if (index >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && index <= GlobalInfo.Instance.TrayDInfos.TrayEndNumber)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayDInfos.TrayItemList[index - GlobalInfo.Instance.TrayDInfos.TrayStartNumber].ItemStatus = Item_Status.Ready;
                            }));
                        }

                        else if (index >= GlobalInfo.Instance.TrayEInfos.TrayStartNumber && index <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                GlobalInfo.Instance.TrayEInfos.TrayItemList[index - GlobalInfo.Instance.TrayEInfos.TrayStartNumber].ItemStatus = Item_Status.Ready;
                            }));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("SampleHelper [RefreshSamplerItemStatus]", ex);
                //ShowErrorMessagePage();
            }
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
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("SampleHelper [GetNextSampleID]", ex);
                ShowErrorMessagePage();
            }
            return sampleID;
        }

        public static void SetCircleStatus(int pos, Item_Status status)
        {
            try
            {
                if (pos >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && pos <= GlobalInfo.Instance.TrayAInfos.TrayEndNumber)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.TrayAInfos.TrayItemList[pos - 1].ItemStatus = status;
                    }));
                }
                else if (pos >= GlobalInfo.Instance.TrayBInfos.TrayStartNumber && pos <= GlobalInfo.Instance.TrayBInfos.TrayEndNumber)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.TrayBInfos.TrayItemList[pos - GlobalInfo.Instance.TrayBInfos.TrayStartNumber].ItemStatus = status;
                    }));
                }
                else if (pos >= GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber && pos <= GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemList[pos - GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber].ItemStatus = status;
                    }));
                }
                else if (pos >= GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber && pos <= GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.TraySTD2Infos.TrayItemList[pos - GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber].ItemStatus = status;
                    }));
                }
                else if (pos >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && pos <= GlobalInfo.Instance.TrayDInfos.TrayEndNumber)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.TrayDInfos.TrayItemList[pos - GlobalInfo.Instance.TrayDInfos.TrayStartNumber].ItemStatus = status;
                    }));
                }

                else if (pos >= GlobalInfo.Instance.TrayEInfos.TrayStartNumber && pos <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GlobalInfo.Instance.TrayEInfos.TrayItemList[pos - GlobalInfo.Instance.TrayEInfos.TrayStartNumber].ItemStatus = status;
                    }));
                }
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("SampleHelper [SetCircleStatus]", ex);
            }
        }

        public static int GetNextSampleLocation(int? pos)
        {
            try
            {
                if (pos != null)
                {
                    if (pos < GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
                    {
                       pos++;
                        return (int)pos;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
               
                MainLogHelper.Instance.Error("SampleHelper [GetNextSampleLocation]", ex);
                return 1;
            }
        }

        public static bool GetSampleLocationMatch(string pos)
        {
            Regex regex = new Regex(@"[A-E]\d");
            if (regex.IsMatch(pos))
            {
                return true;
            }
            return false;
        }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(path);
            }
        }

        public static bool CheckSampleLocationLegal(string pos)
        {
            if (pos.IsNotNullOrEmpty())
            {

                if (GetSampleLocationMatch(pos))
                {
                    int index = int.Parse(pos.Substring(1));
                    string tray = pos.Substring(0, 1);
                    if (tray == "A")
                    {
                        if (index >= 1 && index <= GlobalInfo.Instance.TrayAInfos.XCount * GlobalInfo.Instance.TrayAInfos.YCount)
                        {
                            return true;
                        }
                    }
                    else if (tray == "B")
                    {
                        if (index >= 1 && index <= GlobalInfo.Instance.TrayBInfos.XCount * GlobalInfo.Instance.TrayBInfos.YCount)
                        {
                            return true;
                        }
                    }
                    else if (tray == "C")
                    {
                        if (index >= 1 && index <= GlobalInfo.Instance.TraySTD1Infos.XCount * GlobalInfo.Instance.TraySTD1Infos.YCount + GlobalInfo.Instance.TraySTD2Infos.XCount * GlobalInfo.Instance.TraySTD2Infos.YCount)
                        {
                            return true;
                        }
                    }
                    else if (tray == "D")
                    {
                        if (index >= 1 && index <= GlobalInfo.Instance.TrayDInfos.XCount * GlobalInfo.Instance.TrayDInfos.YCount)
                        {
                            return true;
                        }
                    }
                    else if (tray == "E")
                    {
                        if (index >= 1 && index <= GlobalInfo.Instance.TrayEInfos.XCount * GlobalInfo.Instance.TrayEInfos.YCount)
                        {
                            return true;
                        }
                    }
                    else
                        return false;
                }
            }
            return false;
        }
    }
}
