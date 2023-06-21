using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Enum
{
    public enum Enum_MotorWorkType
    {
        [Description("Home模式")]
        Home,
        [Description("位置模式")]
        Position
    }
}
