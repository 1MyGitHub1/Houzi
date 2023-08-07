using LabTech.Common;
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
        public static double ArmLength = 152.5;/// 机械臂的长度
        public static double TrayCenterOffset=0;
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
        public static Point GetItemPosition(int itemNum)
        {
            try
            {
                double length = 0;
                int xCount = 0;
                int yCount = 0;
                double trayFirstRowToCenter = 0;
                int index = 1;
                Point xwMovePoint = new Point();
                TrayMechanicalData trayData = new TrayMechanicalData();
                CustomTrayMechaincalData customTrayData = new CustomTrayMechaincalData();
                bool isCustomtray = false;
                if (itemNum >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayAInfos.TrayEndNumber)
                {
                    index = itemNum;
                    xCount = GlobalInfo.Instance.TrayAInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayAInfos.YCount;
                    trayData = TrayInfoHelper.GetTrayData(GlobalInfo.Instance.TrayAInfos.TrayType);
                    trayFirstRowToCenter = TrayACenterToCenter + trayData.YCenterDistance / 2;
                }
                else if (itemNum >= GlobalInfo.Instance.TrayBInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayBInfos.TrayEndNumber)
                {
                    index = itemNum - GlobalInfo.Instance.TrayAInfos.TrayEndNumber;
                    xCount = GlobalInfo.Instance.TrayBInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayBInfos.YCount;
                    trayData = TrayInfoHelper.GetTrayData(GlobalInfo.Instance.TrayBInfos.TrayType);
                    trayFirstRowToCenter = TrayBCenterToCenter + trayData.YCenterDistance / 2;
                }
                else if (itemNum >= GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber)
                {
                    index = itemNum - GlobalInfo.Instance.TrayBInfos.TrayEndNumber;
                   
                    xCount = GlobalInfo.Instance.TraySTD1Infos.XCount;
                    customTrayData = TrayInfoHelper.GetSTDTrayData(GlobalInfo.Instance.TrayCleanInfos.TrayType, 1);
                    //customTrayData.XCenterInterval = 33;
                    //customTrayData.RowToCenterXList = new System.Collections.ObjectModel.ObservableCollection<double>
                    //{
                    //    38,5
                    //};
                    //customTrayData.XCenterDistance = 108.5;
                    //customTrayData.XCenterInterval =89;
                    //xCount = 2;
                    //customTrayData.RowToCenterXList = new System.Collections.ObjectModel.ObservableCollection<double>
                    //{
                    //   24
                    //};
                    //customTrayData.XCenterDistance = 44.5;
                }
                else if (itemNum >= GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                {
                    index = itemNum - GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber;
                   
                    xCount = GlobalInfo.Instance.TraySTD2Infos.XCount;
                    customTrayData = TrayInfoHelper.GetSTDTrayData(GlobalInfo.Instance.TrayCleanInfos.TrayType,2);
                    //customTrayData.XCenterInterval = 22;
                    //customTrayData.RowToCenterXList = new System.Collections.ObjectModel.ObservableCollection<double>
                    //{
                    //    23,45
                    //};
                    //customTrayData.XCenterDistance = 113.5;
                    //customTrayData.XCenterInterval =89;
                    //xCount = 3;
                    //customTrayData.RowToCenterXList = new System.Collections.ObjectModel.ObservableCollection<double>
                    //{
                    //    24
                    //};
                    //customTrayData.XCenterDistance = 91.5;
                }
                else if (itemNum >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayDInfos.TrayEndNumber)
                {
                    index = itemNum - GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber;
                    xCount = GlobalInfo.Instance.TrayDInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayDInfos.YCount;
                    trayData = TrayInfoHelper.GetTrayData(GlobalInfo.Instance.TrayDInfos.TrayType);
                    trayFirstRowToCenter = TrayDCenterToCenter - trayData.YCenterDistance / 2 ;
                }

                else if (itemNum >= GlobalInfo.Instance.TrayEInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
                {
                    index = itemNum - GlobalInfo.Instance.TrayDInfos.TrayEndNumber;
                    xCount = GlobalInfo.Instance.TrayEInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayEInfos.YCount;
                    trayData = TrayInfoHelper.GetTrayData(GlobalInfo.Instance.TrayEInfos.TrayType);
                    trayFirstRowToCenter = TrayECenterToCenter - trayData.YCenterDistance / 2;
                }
             
                double xInterval =0;
                double yInterval =0;
                int itemRow = 0;
                double itemXToCenter = 0;
                double xStep = 0;
                double yStep = 0;
                double angle = 0;
                if(!isCustomtray)
                {
                    xInterval = trayData.XCenterDistance / (xCount - 1);
                    yInterval = trayData.YCenterDistance / (yCount - 1);
                    itemRow = (index - 1) / xCount;
                    itemXToCenter = Math.Abs((trayData.XCenterDistance / 2 - TrayCenterOffset) - xInterval * ((index - 1) % xCount));
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                }
                if (itemNum >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayBInfos.TrayEndNumber)//旋转左
                {
                    xStep = (GlobalInfo.Instance.TrayPanelCenter - (trayFirstRowToCenter - xInterval * ((index-1) / xCount) - length) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600 ;
                    angle = 180.0 *  Math.Atan(itemXToCenter/length)/Math.PI;
                    if (index % xCount <= xCount / 2 && index%xCount !=0)
                        angle = 90 + angle;
                    else
                        angle = 90 - angle;
                    yStep = GlobalInfo.Instance.TrayPanelHomeW - angle*10*3;
                }
                else if (itemNum >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
                {
                    xStep = (GlobalInfo.Instance.TrayPanelCenter + (trayFirstRowToCenter + xInterval * ((index - 1) / xCount) - length) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600 ;
                    angle = 180 * Math.Atan(itemXToCenter / length) / Math.PI;
                    if (index % xCount <= xCount / 2 && index % xCount != 0)
                        angle = 90 + angle;
                    else
                        angle = 90 - angle;
                    yStep = GlobalInfo.Instance.TrayPanelHomeW + angle * 10 * 3;
                }
                else if (itemNum >= GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber)
                {
                    double trayRowToCenter = customTrayData.RowToCenterXList[(index-1) / GlobalInfo.Instance.TraySTD1Infos.XCount];
                    itemXToCenter = Math.Abs((xCount - (index- (index - 1) / xCount * xCount) % (xCount+1)) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    xStep = (GlobalInfo.Instance.TrayPanelCenter - trayRowToCenter + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Atan(itemXToCenter / length) / Math.PI;
                    if ((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval>customTrayData.XCenterDistance)
                        angle = 90 + angle;
                    else
                        angle = 90 - angle;
                    yStep = GlobalInfo.Instance.TrayPanelHomeW - angle * 10 * 3;

                }
                else if (itemNum >= GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                {
                    double trayRowToCenter = customTrayData.RowToCenterXList[(index-1) / GlobalInfo.Instance.TraySTD2Infos.XCount];
                    itemXToCenter = Math.Abs((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    xStep = (GlobalInfo.Instance.TrayPanelCenter - (length- trayRowToCenter) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Atan(itemXToCenter / length) / Math.PI;
                    if ((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval > customTrayData.XCenterDistance)
                        angle = 90 + angle;
                    else
                        angle = 90 - angle;
                    yStep = GlobalInfo.Instance.TrayPanelHomeW + angle * 10 * 3;
                }

                xwMovePoint.X = xStep;
                xwMovePoint.Y = yStep;
                return xwMovePoint;
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("GetPositionInfoHelper [GetTrayItemInfo]", ex);
                return new Point(GlobalInfo.Instance.TrayPanelCenter / GlobalInfo.XLengthPerCircle * 3600, GlobalInfo.Instance.TrayPanelHomeW);
            }
        }

        public static Point GetWashPosition(string washPos)
        {
            try
            {
                double length = 0;
                Point xwMovePoint = new Point();
                double itemXToCenter = 93;
                double xStep = 0;
                double yStep = 0;
                double angle = 0;
                if (washPos == "W1")
                {
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    xStep = (GlobalInfo.Instance.TrayPanelCenter - 27-20.5 + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Atan(itemXToCenter / length) / Math.PI;
                    angle = 90 + angle;
                    yStep = GlobalInfo.Instance.TrayPanelHomeW - angle * 10 * 3;
                }
                else if (washPos == "W2")
                {
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    xStep = (GlobalInfo.Instance.TrayPanelCenter  - 20.5 + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Atan(itemXToCenter / length) / Math.PI;
                    angle = 90 + angle;
                    yStep = GlobalInfo.Instance.TrayPanelHomeW - angle * 10 * 3;
                }
                xwMovePoint.X = xStep;
                xwMovePoint.Y = yStep;
                return xwMovePoint;
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("GetPositionInfoHelper [GetWashPosition]", ex);
                return new Point();
            }
        }

        /// <summary>
        /// 判断当前位置是否会碰撞,返回0不碰撞，返回1左边碰撞，所以需要X先后退155 ，返回2 右边碰撞，需要X先后退340-155
        /// </summary>
        public static int GetXIsCollision(Point pt, int itemNum)
        {
            int isCollisionStatus = 0;
            if (itemNum >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayBInfos.TrayEndNumber)//旋转左
            {
                if (pt.X / 3600.0 * GlobalInfo.XLengthPerCircle - GlobalInfo.Instance.TrayPanelHomeX < 152.5-109.5)
                    isCollisionStatus = 1;
            }
            else if (itemNum >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
            {
                if (559 - pt.X / 3600.0 * GlobalInfo.XLengthPerCircle + GlobalInfo.Instance.TrayPanelHomeX - 109.5 < 152.5)
                    isCollisionStatus = 2;
            }

            return isCollisionStatus;
        }
    }
}
