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
        public static double ArmLength = 120.75;                            //152.5;             /// 机械臂的旋转半径
        public static double TrayCenterOffset=0;            //原间隔3.5
        public static double TrayACenterToCenter = 221.5;                   //223.5;
        public static double TrayBCenterToCenter = 113.5;                   //114.5;
        public static double TrayDCenterToCenter = 113.5;                   //114.5;
        public static double TrayECenterToCenter = 221.5;                   //223.5;
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
                int xCount = 0;             //长的一排孔数
                int yCount = 0;             //短的一排孔数
                double trayFirstRowToCenter = 0;        //试管架左边第一排的孔到模拟原点的距离
                int index = 1;              //孔号
                Point xwMovePoint = new Point();
                TrayMechanicalData trayData = new TrayMechanicalData();
                CustomTrayMechaincalData customTrayData = new CustomTrayMechaincalData();
                bool isCustomtray = false;
                if (itemNum >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayAInfos.TrayEndNumber)
                {
                    index = itemNum;
                    xCount = GlobalInfo.Instance.TrayAInfos.XCount;
                    yCount = GlobalInfo.Instance.TrayAInfos.YCount;
                    trayData = TrayInfoHelper.GetTrayData(GlobalInfo.Instance.TrayAInfos.TrayType);     //架子类型
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
             
                double xInterval =0;            //长的一排一个孔加一个缝的距离
                double yInterval =0;
                int itemRow = 0;                //几个孔加间隔的距离
                double itemXToCenter = 0;
                double xStep = 0;
                double yStep = 0;
                double angle = 0;               //机械臂要转的角度
                double distance = 0;            //点位距中心
                if (!isCustomtray)
                {
                    xInterval = trayData.XCenterDistance / (xCount - 1);
                    yInterval = trayData.YCenterDistance / (yCount - 1);
                    itemRow = (index - 1) / xCount;
                    itemXToCenter = Math.Round( Math.Abs((trayData.XCenterDistance / 2 - TrayCenterOffset) - xInterval * ((index - 1) % xCount)),2);//X直角边1
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));                                        //Y直角边2

                }
                if (itemNum >= GlobalInfo.Instance.TrayAInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayBInfos.TrayEndNumber)//旋转左
                {
                    #region 以中心方式
                    //distance = trayFirstRowToCenter - yInterval * itemRow;           //点位距中心
                    //xStep = (227 - distance + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    ////xStep = (GlobalInfo.Instance.TrayPanelCenter_left - distance + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600 ;
                    ////angle = 180.0 *  Math.Atan(itemXToCenter/length) /Math.PI;
                    //angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.Round(Math.PI, 2);
                    //if (index % xCount <= xCount / 2 && index % xCount != 0)
                    //    angle = 90 + angle;                     //左边大角度
                    //else
                    //    angle = 90 - angle;                 //左边小角度
                    //yStep = 10708 + GlobalInfo.Instance.TrayPanelHomeW - angle * 10 * 6;
                    #endregion

                    #region 以两侧方式
                    distance = trayFirstRowToCenter - yInterval * itemRow;           //点位距中心
                    //xStep = (GlobalInfo.Instance.TrayPanelCenter - (trayFirstRowToCenter - yInterval * ((index - 1) / xCount) - length) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    xStep = (GlobalInfo.Instance.TrayPanelCenter - distance + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.Round(Math.PI, 2);
                    if (index % xCount <= xCount / 2 && index % xCount != 0)
                        angle = GlobalInfo.Instance.TrayPanel_leftW - angle;                     
                    else
                        angle = GlobalInfo.Instance.TrayPanel_leftW + angle;                 
                    yStep = angle * 10 * 6 + GlobalInfo.Instance.TrayPanelHomeW;
                    #endregion

                }
                else if (itemNum >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
                {
                    #region 以中心方式
                    //distance = trayFirstRowToCenter + yInterval * itemRow;           //点位距中心
                    ////xStep = (GlobalInfo.Instance.TrayPanelCenter + (trayFirstRowToCenter - yInterval * ((index - 1) / xCount) - length) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    //xStep = (227 + distance - length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    //angle = 180 * Math.Asin(itemXToCenter / ArmLength) / Math.Round(Math.PI, 2);
                    //if (index % xCount <= xCount / 2 && index % xCount != 0)
                    //    angle = 90 + angle;                   //右边小角度
                    //else
                    //    angle = 90 - angle;
                    //yStep = 10708 + GlobalInfo.Instance.TrayPanelHomeW + angle * 10 * 6;
                    #endregion

                    #region 以两侧方式
                    distance = trayFirstRowToCenter + yInterval * itemRow;           //点位距中心
                    //xStep = (GlobalInfo.Instance.TrayPanelCenter + (trayFirstRowToCenter - yInterval * ((index - 1) / xCount) - length) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    xStep = (GlobalInfo.Instance.TrayPanelCenter + distance - length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180 * Math.Asin(itemXToCenter / ArmLength) / Math.Round(Math.PI, 2);
                    if (index % xCount <= xCount / 2 && index % xCount != 0)
                        angle = GlobalInfo.Instance.TrayPanel_rightW + angle;                   
                    else
                        angle = GlobalInfo.Instance.TrayPanel_rightW - angle;
                    yStep = angle * 10 * 6 + GlobalInfo.Instance.TrayPanelHomeW;
                    #endregion

                }
                else if (itemNum >= GlobalInfo.Instance.TraySTD1Infos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TraySTD1Infos.TrayEndNumber)
                {
                    #region 以中心方式
                    //double trayRowToCenter = customTrayData.RowToCenterXList[(index-1) / GlobalInfo.Instance.TraySTD1Infos.XCount];
                    //itemXToCenter = Math.Abs((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    ////itemXToCenter = Math.Abs((xCount - index%6) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    //length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    //xStep = (227 + (length - trayRowToCenter) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    ////xStep = (GlobalInfo.Instance.TrayPanelCenter_left - trayRowToCenter + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    //angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.PI;
                    //if ((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval>customTrayData.XCenterDistance)
                    //    angle = 90 + angle;
                    //else
                    //    angle = 90 - angle;
                    //yStep = 10708 + GlobalInfo.Instance.TrayPanelHomeW - angle * 10 * 6;
                    #endregion

                    #region 以两侧方式
                    double trayRowToCenter = customTrayData.RowToCenterXList[(index - 1) / GlobalInfo.Instance.TraySTD1Infos.XCount];
                    itemXToCenter = Math.Abs((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    //itemXToCenter = Math.Abs((xCount - index%6) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    xStep = (GlobalInfo.Instance.TrayPanelCenter + (length - trayRowToCenter) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    //xStep = (GlobalInfo.Instance.TrayPanelCenter_left - trayRowToCenter + length + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.PI;
                    if ((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval > customTrayData.XCenterDistance)
                        angle = GlobalInfo.Instance.TrayPanel_leftW - angle;
                    else
                        angle = GlobalInfo.Instance.TrayPanel_leftW + angle;
                    yStep = angle * 10 * 6;
                    #endregion

                }
                else if (itemNum >= GlobalInfo.Instance.TraySTD2Infos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TraySTD2Infos.TrayEndNumber)
                {
                    #region 以中心方式
                    //double trayRowToCenter = customTrayData.RowToCenterXList[(index-1) / GlobalInfo.Instance.TraySTD2Infos.XCount];
                    //itemXToCenter = Math.Abs((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    //length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    //xStep = (227 - (length- trayRowToCenter) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    //angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.PI;
                    //if ((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval > customTrayData.XCenterDistance)
                    //    angle = 90 + angle;
                    //else
                    //    angle = 90 - angle;
                    //yStep = 10708 + GlobalInfo.Instance.TrayPanelHomeW + angle * 10 * 6;
                    #endregion

                    #region 以两侧方式
                    double trayRowToCenter = customTrayData.RowToCenterXList[(index - 1) / GlobalInfo.Instance.TraySTD2Infos.XCount];
                    itemXToCenter = Math.Abs((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval - customTrayData.XCenterDistance);
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2));
                    xStep = (GlobalInfo.Instance.TrayPanelCenter - (length - trayRowToCenter) + GlobalInfo.Instance.TrayPanelHomeX) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.PI;
                    if ((xCount - (index - (index - 1) / xCount * xCount) % (xCount + 1)) * customTrayData.XCenterInterval > customTrayData.XCenterDistance)
                        angle = GlobalInfo.Instance.TrayPanel_rightW + angle;
                    else
                        angle = GlobalInfo.Instance.TrayPanel_rightW - angle;
                    yStep = angle * 10 * 6;
                    #endregion

                }
                xwMovePoint.X = xStep;
                xwMovePoint.Y = yStep;
                MainLogHelper.Instance.Info("下发时的绝对距离：" +"["+ (xStep - GlobalInfo.Instance.TrayPanelHomeX / GlobalInfo.XLengthPerCircle * 3600) + "," + (yStep - GlobalInfo.Instance.TrayPanelHomeW) + "]");
                return xwMovePoint;
            }
            catch(Exception ex)
            {
                MainLogHelper.Instance.Error("GetPositionInfoHelper [GetTrayItemInfo]", ex);
                return new Point(GlobalInfo.Instance.TrayPanelCenter / GlobalInfo.XLengthPerCircle * 3600, GlobalInfo.Instance.TrayPanelHomeW);
            }
        }
        //获取清洗位置坐标
        public static Point GetWashPosition(string washPos)
        {
            try
            {
                double length = 0;
                Point xwMovePoint = new Point();
                double itemXToCenter = 107;              //93;
                double xStep = 0;
                double yStep = 0;
                double angle = 0;
                if (washPos == "W1")
                {
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2)) - 49;
                    xStep = (GlobalInfo.Instance.TrayPanelCenter + GlobalInfo.Instance.TrayPanelHomeX + length) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.PI;
                    angle = GlobalInfo.Instance.TrayPanel_leftW + GlobalInfo.Instance.TrayPanelHomeW / 60 - angle;
                    yStep = angle * 10 * 6;
                }
                else if (washPos == "W2")
                {
                    length = Math.Sqrt(Math.Pow(ArmLength, 2) - Math.Pow(itemXToCenter, 2)) - 22;
                    xStep = (GlobalInfo.Instance.TrayPanelCenter + GlobalInfo.Instance.TrayPanelHomeX + length) / GlobalInfo.XLengthPerCircle * 3600;
                    angle = 180.0 * Math.Asin(itemXToCenter / ArmLength) / Math.PI;
                    angle = GlobalInfo.Instance.TrayPanel_leftW + GlobalInfo.Instance.TrayPanelHomeW / 60 - angle;
                    yStep = angle * 10 * 6;
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
                //if (pt.X / 3600.0 * GlobalInfo.XLengthPerCircle - GlobalInfo.Instance.TrayPanelHomeX < 152.5 - 109.5)
                //    isCollisionStatus = 1;
                if (pt.X / 3600.0 * GlobalInfo.XLengthPerCircle - GlobalInfo.Instance.TrayPanelHomeX < 152.5 - 109.5)
                    isCollisionStatus = 1;
                MainLogHelper.Instance.Info("左边当前会碰撞点的位置：" + pt.X.ToString());
            }
            else if (itemNum >= GlobalInfo.Instance.TrayDInfos.TrayStartNumber && itemNum <= GlobalInfo.Instance.TrayEInfos.TrayEndNumber)
            {
                if (559 - pt.X / 3600.0 * GlobalInfo.XLengthPerCircle + GlobalInfo.Instance.TrayPanelHomeX - 109.5 < 152.5)
                    isCollisionStatus = 2;
                //if (583.5 - pt.X / 3600.0 * GlobalInfo.XLengthPerCircle + GlobalInfo.Instance.TrayPanelHomeX < 120.75)
                //    isCollisionStatus = 2;
                MainLogHelper.Instance.Info("右边当前会碰撞点的位置：" + pt.X.ToString());
            }

            return isCollisionStatus;
        }
    }
}
