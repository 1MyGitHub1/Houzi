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
            switch(trayType)
            {
                case Enum_TrayType._15mL:
                    info.XCount = 12;
                    info.YCount = 5;
                    info.ItemSize = new System.Windows.Size(20, 20);
                    info.TrayBckPath = "/Totalab_L;component/Images/Tray_5Rows.png";
                    info.TrayType = EnumHelper.GetDescription(Enum_TrayType._15mL);
                    break;
                case Enum_TrayType._60mL:
                    info.XCount = 7;
                    info.YCount = 3;
                    info.ItemSize = new System.Windows.Size(32, 32);
                    info.TrayBckPath = "/Totalab_L;component/Images/Tray_3Rows.png";
                    info.TrayType = EnumHelper.GetDescription(Enum_TrayType._60mL);
                    break;
                default:
                    info.TrayBckPath = "/Totalab_L;component/Images/row4.png";
                    GlobalInfo.Instance.TraySTD1Infos.XCount = 12;
                    GlobalInfo.Instance.TraySTD1Infos.YCount = 2;
                    GlobalInfo.Instance.TraySTD1Infos.ItemSize = new System.Windows.Size(20, 20);
                    GlobalInfo.Instance.TraySTD2Infos.XCount = 6;
                    GlobalInfo.Instance.TraySTD2Infos.YCount = 2;
                    GlobalInfo.Instance.TraySTD2Infos.ItemSize = new System.Windows.Size(32, 32);

                    break;
            }
            for (int i = 1; i < info.XCount * info.YCount + 1; i++)
            {
                ItemData item = new ItemData
                {
                    ItemContent = i.ToString()
                };
               info.TrayItemList.Add(item);
            }
            return info;
        }

        public static  void GetStdTrayInfo(Enum_StdTrayType? trayType)
        {
            GlobalInfo.Instance.TraySTD1Infos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
            GlobalInfo.Instance.TraySTD2Infos.TrayItemList = new System.Collections.ObjectModel.ObservableCollection<ItemData>();
            switch (trayType)
            {
                case Enum_StdTrayType._250mL:
                    GlobalInfo.Instance.TrayCleanInfos.TrayBckPath = "/Totalab_L;component/Images/row0.png";
                    GlobalInfo.Instance.TraySTD1Infos.XCount = 3;
                    GlobalInfo.Instance.TraySTD1Infos.YCount = 1;
                    GlobalInfo.Instance.TraySTD1Infos.ItemSize = new System.Windows.Size(50, 50);
                    GlobalInfo.Instance.TraySTD2Infos.XCount = 2;
                    GlobalInfo.Instance.TraySTD2Infos.YCount = 1;
                    GlobalInfo.Instance.TraySTD2Infos.ItemSize = new System.Windows.Size(50, 50);
                    for (int i = 1; i < 4; i++)
                    {
                        ItemData item = new ItemData
                        {
                            ItemContent = i.ToString()
                        };
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemList.Add(item);
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        ItemData item = new ItemData
                        {
                            ItemContent = i.ToString()
                        };
                        GlobalInfo.Instance.TraySTD2Infos.TrayItemList.Add(item);
                    }
                    GlobalInfo.Instance.STD1TrayHeight = 60;
                    GlobalInfo.Instance.STD2TrayHeight = 60;
                    GlobalInfo.Instance.STD2TrayWidth = 302;
                    GlobalInfo.Instance.CleanTrayMargin = new System.Windows.Thickness(0, 12, 0, 0);
                    break;
                case Enum_StdTrayType._50mL:
                    GlobalInfo.Instance.TrayCleanInfos.TrayBckPath = "/Totalab_L;component/Images/row4.png";
                    GlobalInfo.Instance.TraySTD1Infos.XCount = 12;
                    GlobalInfo.Instance.TraySTD1Infos.YCount = 2;
                    GlobalInfo.Instance.TraySTD1Infos.ItemSize = new System.Windows.Size(20, 20);
                    GlobalInfo.Instance.TraySTD2Infos.XCount = 6;
                    GlobalInfo.Instance.TraySTD2Infos.YCount = 2;
                    GlobalInfo.Instance.TraySTD2Infos.ItemSize = new System.Windows.Size(32, 32);
                    for (int i = 1; i < 25; i++)
                    {
                        ItemData item = new ItemData
                        {
                            ItemContent = i.ToString()
                        };
                        GlobalInfo.Instance.TraySTD1Infos.TrayItemList.Add(item);
                    }
                    for (int i = 1; i < 13; i++)
                    {
                        ItemData item = new ItemData
                        {
                            ItemContent = i.ToString()
                        };
                        GlobalInfo.Instance.TraySTD2Infos.TrayItemList.Add(item);
                    }
                    GlobalInfo.Instance.STD1TrayHeight = 48;
                    GlobalInfo.Instance.STD2TrayHeight = 72;
                    GlobalInfo.Instance.STD2TrayWidth = 402;
                    GlobalInfo.Instance.CleanTrayMargin = new System.Windows.Thickness(0, 24, 0, 0);
                    break;
                default:
                    break;
            }
        }
            
    }
}
