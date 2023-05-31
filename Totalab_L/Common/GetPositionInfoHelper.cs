using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Totalab_L.Enum;
using Totalab_L.Models;

namespace Totalab_L.Common
{
    public static class GetPositionInfoHelper
    {
        public static int TrayWidth;
        public static int TrayLength;
        public static int XCenter;
        public static int ArmLength;/// 机械臂的长度
        public static double TrayCenterOffset;
        public static double TrayACenterToCenter = 223.5;
        public static double TrayBCenterToCenter = 114.5;
        public static double TrayDCenterToCenter = 114.5;
        public static double TrayECenterToCenter = 223.5;
        public static  int GetPositionX(string pos)
        {
            int length = 0;

            return length;
        }


        /// <summary>
        /// 获取架子上的孔到中心轴的位置
        /// </summary>
        /// <param name="trayName"></param>
        /// <param name="itemNum"></param>
        /// <returns></returns>
        public static double GetTrayItemInfo(Enum_TrayName trayName, int itemNum)
        {
            double length = 0;
            int xCount = 0;
            int yCount = 0;
            TrayMechanicalData trayData = new TrayMechanicalData();
            switch(trayName)
            {
                case Enum_TrayName.TrayA:
                    xCount = GlobalInfo.Instance.TrayAInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayAInfos.YCount;
                    trayData = GlobalInfo.Instance.TrayDataList[0];
                    break;
                case Enum_TrayName.TrayB:
                    xCount = GlobalInfo.Instance.TrayBInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayBInfos.YCount;
                    trayData = GlobalInfo.Instance.TrayDataList[1];
                    break;
                //case Enum_TrayName.TrayC:
                //    xCount = GlobalInfo.Instance.TrayCInfos.XCount;
                //    yCount = GlobalInfo.Instance.TrayCInfos.YCount;
                //    break;
                case Enum_TrayName.TrayD:
                    xCount = GlobalInfo.Instance.TrayDInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayDInfos.YCount;
                    trayData = GlobalInfo.Instance.TrayDataList[2];
                    break;
                case Enum_TrayName.TrayE:
                    xCount = GlobalInfo.Instance.TrayEInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayEInfos.YCount;
                    trayData = GlobalInfo.Instance.TrayDataList[4];
                    break;
            }
            double xInterval = trayData.XCenterDistance / (xCount - 1);
            double yInterval = trayData.YCenterDistance / (yCount - 1);
            int itemRow = (itemNum -1)/ xCount;
            double itemXToCenter = 0;
            if(itemRow %2==0)
               itemXToCenter = Math.Abs((trayData.XCenterDistance / 2 - TrayCenterOffset) - xInterval * ((itemNum - 1) % xCount));
            //else
            //    itemXToCenter = Math.Abs((trayData.XCenterDistance / 2 - TrayCenterOffset) - xInterval * ((xCount-1)-(itemNum - 1) % xCount));
            length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
            if(trayName == Enum_TrayName.TrayA || trayName == Enum_TrayName.TrayB)//旋转左
            {
                
            }
            return length;
        }
    }
}
