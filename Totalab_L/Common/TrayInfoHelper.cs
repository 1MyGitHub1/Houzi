using LabTech.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totalab_L.Enum;
using Totalab_L.Models;

namespace Totalab_L.Common
{
    public static class TrayInfoHelper
    {
        public static TrayInfo GetTrayInfo(Enum_TrayType? trayType)
        {
            TrayInfo info = new TrayInfo();
            try
            {
                switch (trayType)
                {
                    case Enum_TrayType._15mL:
                        info.XCount = 12;
                        info.YCount = 5;
                        info.ItemsSize = new System.Windows.Size(20, 20);
                        info.TrayBckPath = "/Totalab_L;component/Images/Tray_5Rows.png";
                        info.TrayType = EnumHelper.GetDescription(Enum_TrayType._15mL);
                        info.TrayItemCount = 60;
                        break;
                    case Enum_TrayType._60mL:
                        info.XCount = 7;
                        info.YCount = 3;
                        info.ItemsSize = new System.Windows.Size(32, 32);
                        info.TrayBckPath = "/Totalab_L;component/Images/Tray_3Rows.png";
                        info.TrayType = EnumHelper.GetDescription(Enum_TrayType._60mL);
                        info.TrayItemCount = 21;
                        break;
                    default:
                        info.TrayBckPath = "/Totalab_L;component/Images/row4.png";
                        GlobalInfo.Instance.TraySTD1Infos.XCount = 12;
                        GlobalInfo.Instance.TraySTD1Infos.YCount = 2;
                        GlobalInfo.Instance.TraySTD1Infos.ItemsSize = new System.Windows.Size(20, 20);
                        GlobalInfo.Instance.TraySTD2Infos.XCount = 6;
                        GlobalInfo.Instance.TraySTD2Infos.YCount = 2;
                        GlobalInfo.Instance.TraySTD2Infos.ItemsSize = new System.Windows.Size(32, 32);

                        break;
                }
                return info;
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("TrayInfoHelper [GetTrayInfo]", ex);
                return info;
            }
        }

        public static  void GetStdTrayInfo(Enum_StdTrayType? trayType)
        {
            try
            {
                GlobalInfo.Instance.TraySTD1Infos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                GlobalInfo.Instance.TraySTD2Infos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                switch (trayType)
                {
                    case Enum_StdTrayType._250mL:
                        GlobalInfo.Instance.TrayCleanInfos.TrayType = EnumHelper.GetDescription(Enum_StdTrayType._250mL);
                        GlobalInfo.Instance.TrayCleanInfos.TrayBckPath = "/Totalab_L;component/Images/row0.png";
                        GlobalInfo.Instance.TraySTD2Infos.XCount = 3;
                        GlobalInfo.Instance.TraySTD2Infos.YCount = 1;
                        GlobalInfo.Instance.TraySTD2Infos.ItemsSize = new System.Windows.Size(50, 50);
                        GlobalInfo.Instance.TraySTD2Infos.TrayItemCount = 3;
                        GlobalInfo.Instance.TraySTD1Infos.XCount = 2;
                        GlobalInfo.Instance.TraySTD1Infos.YCount = 1;
                        GlobalInfo.Instance.TraySTD1Infos.ItemsSize = new System.Windows.Size(50, 50);
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemCount = 2;
                        GlobalInfo.Instance.STD2TrayHeight = 60;
                        GlobalInfo.Instance.STD1TrayHeight = 60;
                        GlobalInfo.Instance.STD1TrayWidth = 302;
                        GlobalInfo.Instance.CleanTrayMargin = new System.Windows.Thickness(0, 12, 0, 0);
                        break;
                    case Enum_StdTrayType._50mL:
                        GlobalInfo.Instance.TrayCleanInfos.TrayType = EnumHelper.GetDescription(Enum_StdTrayType._50mL);
                        GlobalInfo.Instance.TrayCleanInfos.TrayBckPath = "/Totalab_L;component/Images/row4.png";
                        GlobalInfo.Instance.TraySTD2Infos.XCount = 11;
                        GlobalInfo.Instance.TraySTD2Infos.YCount = 2;
                        GlobalInfo.Instance.TraySTD2Infos.TrayItemCount = 22;
                        GlobalInfo.Instance.TraySTD2Infos.ItemsSize = new System.Windows.Size(20, 20);
                        GlobalInfo.Instance.TraySTD1Infos.XCount = 6;
                        GlobalInfo.Instance.TraySTD1Infos.YCount = 2;
                        GlobalInfo.Instance.TraySTD1Infos.ItemsSize = new System.Windows.Size(32, 32);
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemCount = 12;
                        GlobalInfo.Instance.STD2TrayHeight = 48;
                        GlobalInfo.Instance.STD1TrayHeight = 72;
                        GlobalInfo.Instance.STD1TrayWidth = 402;
                        GlobalInfo.Instance.CleanTrayMargin = new System.Windows.Thickness(0, 24, 0, 0);
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("TrayInfoHelper [GetStdTrayInfo]", ex);
            }
        }
        public static void GetTrayNumber()
       {
            try
            {
                GlobalInfo.Instance.TrayAInfos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                GlobalInfo.Instance.TrayBInfos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                GlobalInfo.Instance.TraySTD1Infos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                GlobalInfo.Instance.TraySTD2Infos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                GlobalInfo.Instance.TrayDInfos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                GlobalInfo.Instance.TrayEInfos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
                GlobalInfo.Instance.TrayAInfos.TrayStartNumber = 1;
                GlobalInfo.Instance.TrayAInfos.TrayEndNumber = GlobalInfo.Instance.TrayAInfos.TrayItemCount;
                for (int i = 1; i < GlobalInfo.Instance.TrayAInfos.TrayEndNumber + 1; i++)
                {
                    ItemData item = new ItemData
                    {
                        ItemContent = i.ToString()
                    };
                    GlobalInfo.Instance.TrayAInfos.TrayItemList.Add(item);
                }
                GlobalInfo.Instance.TrayBInfos.TrayStartNumber = GlobalInfo.Instance.TrayAInfos.TrayEndNumber + 1;
                GlobalInfo.Instance.TrayBInfos.TrayEndNumber = GlobalInfo.Instance.TrayAInfos.TrayItemCount + GlobalInfo.Instance.TrayBInfos.TrayItemCount;
                for (int i = GlobalInfo.Instance.TrayBInfos.TrayStartNumber; i < GlobalInfo.Instance.TrayBInfos.TrayEndNumber + 1; i++)
                {
                    ItemData item = new ItemData
                    {
                        ItemContent = i.ToString()
                    };
                    GlobalInfo.Instance.TrayBInfos.TrayItemList.Add(item);
                }
                //GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber = GlobalInfo.Instance.TrayBInfos.TrayEndNumber + 1;
                //GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber = GlobalInfo.Instance.TrayBInfos.TrayEndNumber + GlobalInfo.Instance.TraySTD1Infos.TrayItemCount;
                //for (int i = GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber; i < GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber + 1; i++)
                //{
                //    ItemData item = new ItemData
                //    {
                //        ItemContent = i.ToString()
                //    };
                //    GlobalInfo.Instance.TraySTD1Infos.TrayItemList.Add(item);
                //}
                //GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber = GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber + 1;
                //GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber = GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber + GlobalInfo.Instance.TraySTD2Infos.TrayItemCount;
                //for (int i = GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber; i < GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber + 1; i++)
                //{
                //    ItemData item = new ItemData
                //    {
                //        ItemContent = i.ToString()
                //    };
                //    GlobalInfo.Instance.TraySTD2Infos.TrayItemList.Add(item);
                //}
                GlobalInfo.Instance.TrayDInfos.TrayStartNumber = GlobalInfo.Instance.TrayBInfos.TrayEndNumber + 1;
                GlobalInfo.Instance.TrayDInfos.TrayEndNumber = GlobalInfo.Instance.TrayBInfos.TrayEndNumber + GlobalInfo.Instance.TrayDInfos.TrayItemCount;
                for (int i = GlobalInfo.Instance.TrayDInfos.TrayStartNumber; i < GlobalInfo.Instance.TrayDInfos.TrayEndNumber + 1; i++)
                {
                    ItemData item = new ItemData
                    {
                        ItemContent = i.ToString()
                    };
                    GlobalInfo.Instance.TrayDInfos.TrayItemList.Add(item);
                }
                GlobalInfo.Instance.TrayEInfos.TrayStartNumber = GlobalInfo.Instance.TrayDInfos.TrayEndNumber + 1;
                GlobalInfo.Instance.TrayEInfos.TrayEndNumber = GlobalInfo.Instance.TrayDInfos.TrayEndNumber + GlobalInfo.Instance.TrayEInfos.TrayItemCount;
                for (int i = GlobalInfo.Instance.TrayEInfos.TrayStartNumber; i < GlobalInfo.Instance.TrayEInfos.TrayEndNumber + 1; i++)
                {
                    ItemData item = new ItemData
                    {
                        ItemContent = i.ToString()
                    };
                    GlobalInfo.Instance.TrayEInfos.TrayItemList.Add(item);
                }
                GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber = GlobalInfo.Instance.TrayEInfos.TrayEndNumber + 1;
                GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber = GlobalInfo.Instance.TrayEInfos.TrayEndNumber + GlobalInfo.Instance.TraySTD1Infos.TrayItemCount;
                for (int i = GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber; i < GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber + 1; i++)
                {
                    ItemData item = new ItemData
                    {
                        ItemContent = i.ToString()
                    };
                    GlobalInfo.Instance.TraySTD1Infos.TrayItemList.Add(item);
                }
                GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber = GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber + 1;
                GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber = GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber + GlobalInfo.Instance.TraySTD2Infos.TrayItemCount;
                for (int i = GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber; i < GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber + 1; i++)
                {
                    ItemData item = new ItemData
                    {
                        ItemContent = i.ToString()
                    };
                    GlobalInfo.Instance.TraySTD2Infos.TrayItemList.Add(item);
                }

            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("TrayInfoHelper [GetTrayNumber]", ex);
            }
        }
        /// <summary>
        /// 判断架子类型
        /// </summary>
        /// <param name="trayType"></param>
        /// <returns></returns>
        public static TrayMechanicalData GetTrayData(string trayType)
        {
            TrayMechanicalData info = new TrayMechanicalData();
            try
            {
                info = GlobalInfo.Instance.TrayDataList.Where(item => item.TrayType == trayType).FirstOrDefault();
                return info;
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("TrayInfoHelper [GetTrayData]", ex);
                return info;
            }
        }

        public static CustomTrayMechaincalData GetSTDTrayData(string trayType,int stdTray)
        {
            CustomTrayMechaincalData info = new CustomTrayMechaincalData();
            try
            {

                string stdTrayType = null;
                if (trayType == EnumHelper.GetDescription(Enum_StdTrayType._250mL))
                {
                    if (stdTray == 1)
                        stdTrayType = "250mL * 2";
                    else if (stdTray == 2)
                        stdTrayType = "250mL * 3";

                }
                else if (trayType == EnumHelper.GetDescription(Enum_StdTrayType._50mL))
                {
                    if (stdTray == 1)
                        stdTrayType = "50mL * 12";
                    else if (stdTray == 2)
                        stdTrayType = "15mL * 22";
                }
                info = GlobalInfo.Instance.StdTrayDataList.Where(item => item.TrayType == stdTrayType).FirstOrDefault();
                return info;
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("TrayInfoHelper [GetSTDTrayData]", ex);
                return info;
            }
        }
    }
}
