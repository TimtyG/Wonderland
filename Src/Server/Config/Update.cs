using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Config
{
    public enum UpdtSetting
    {
        Never,
        Auto,
        AutoandForce,
    }
    public class UpdateSetting
    {
        public bool EnableSchedUpdate;
        public bool WarnofUpdate;
        public UpdtSetting UpdtControl;
        public TimeSpan UpdtChk_Interval;
        public TimeSpan AutoUpdt_Schedule;

        public UpdateSetting()
        {
            UpdtControl = UpdtSetting.Never;
            UpdtChk_Interval = new TimeSpan(0, 10, 0);
            AutoUpdt_Schedule = new TimeSpan(1, 0, 0);
        }
    }
}
