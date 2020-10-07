using System;
using System.Collections.Generic;
using System.Text;

namespace CycleAlarmApp.Interface
{
    public interface ISettingMenu
    {
        void SetThreshold(float threshold);
        void SetCenter(float center);
        float GetThreshold();
        float GetCenter();
    }
}
