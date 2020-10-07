using System;
using System.Collections.Generic;
using System.Text;

namespace CycleAlarmApp
{
    public enum MessageTypes
    {
        None = 0x00,
        Cancel = 0x01,

        // Bluetooth

        Ack = 0x10,
        LightOn = 0x11,
        LightOff = 0x12,
        ChangeBright = 0x13,
        ChangeColor = 0x14,

        // Options

        SafetyOn = 0x90,
        SafetyOff = 0x91
    }
}
