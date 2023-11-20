using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Enum
{
    public enum RunningStep_Status
    {
        RecvTrigger,
        RecvTriggerOk,
        GoToSampleLoc,
        GoToSampleLocOk,
        GoToSampleXY,
        GoToSampleXYOk,
        GoToSampleDepth,
        GoToSampleDepthOk,
        GoToWashLoc,
        GoToWashLocOk,
        SetPumpSpeed,
        SetPumpSpeedOk,
        PreWashOk,
        OpenPump,
        OpenPumpOk,
        ClosePump,
        ClosePumpOk,
        PumpRunOk,
        ZHome,
        ZHomeOk,
        SendTrigger,
        SendTriggerOk,
        CloseTrigger,
        CloseTriggerOk,
        MethodPause,
        MethodPauseOk,
        XYZHome,
        XYZHomeOk,
        PreRunningOk,
        Wash,
        WashOk,
        AfterRunningOk,
        AnalysOk,
        Error,
        SetMortorWorkMode,
        SetMortorWorkModeOk,
        SetHomeWorkMode,
        SetHomeWorkModeOk,
        SetTargetPosition,
        SetTargetPositionOk,
        SetMotorAction,
        SetMotorActionOk,
        ReadCurrentPosition,
        ReadCurrentPositionOk,
        SetZSpeed,          //设置Z轴升降速度
        SetZSpeedOK,
        OpenWettedParts,        //打开接液盘
        OpenWettedPartsOK
    }
}
