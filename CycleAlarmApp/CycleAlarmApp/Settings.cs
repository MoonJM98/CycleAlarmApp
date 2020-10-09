using System;
using System.Collections.Generic;
using System.Text;

namespace BLINK
{
    [Serializable]
    public class Settings : object
    {
        [NonSerialized]
        public static Settings Singleton;
        public float TurnThreshold { get; set; } = 0.1f;
        public float AcceleroThreshold { get; set; } = 2.5f;
        public float CenterPoint { get; set; } = 0f;
    }
}
