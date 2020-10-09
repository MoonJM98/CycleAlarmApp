using System;
using System.Collections.Generic;
using System.Text;

namespace BLINK.Interface
{
    public interface IFileMenu
    {
        void LoadSettings(ref Settings settings);
        void SaveSettings(ref Settings settings);
    }
}
