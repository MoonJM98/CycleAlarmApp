using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CycleAlarmApp
{
    [Serializable]
    public class Configs
    {
        public static bool LightOn { get; set; } = true;
        public static byte LightColorR { get; set; }
        public static byte LightColorG { get; set; }
        public static byte LightColorB { get; set; }
        public static byte FrontLightPower { get; set; }
    }
}
