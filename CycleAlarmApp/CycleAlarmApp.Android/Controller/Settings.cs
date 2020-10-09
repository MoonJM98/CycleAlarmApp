using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BLINK.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(BLINK.Droid.Controller.Settings))]
namespace BLINK.Droid.Controller 
{
    public class Settings : IFileMenu
    {
        string PersonalFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
        public void LoadSettings(ref BLINK.Settings settings)
        {
            if (!File.Exists(Path.Combine(PersonalFolder, "settings")))
            {
                SaveSettings(ref settings);
            }
            else
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(Path.Combine(PersonalFolder, "settings"), FileMode.Open))
                {
                    BLINK.Settings.Singleton = binaryFormatter.Deserialize(fs) as BLINK.Settings;
                    Log.Debug("BLINK", $"Setting Loaded: CenterPoint - {settings.CenterPoint}");
                    Log.Debug("BLINK", $"Setting Loaded: TurnThreshold - {settings.TurnThreshold}");
                    Log.Debug("BLINK", $"Setting Loaded: AcceleroThreshold - {settings.AcceleroThreshold}");
                    fs.Close();
                }
            }
        }

        public void SaveSettings(ref BLINK.Settings settings)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(PersonalFolder, "settings"), FileMode.Create))
            {
                if(settings == null) settings = new BLINK.Settings();
                binaryFormatter.Serialize(fs, settings);
                Log.Debug("BLINK", $"Setting Saved: CenterPoint - {settings.CenterPoint}");
                Log.Debug("BLINK", $"Setting Saved: TurnThreshold - {settings.TurnThreshold}");
                Log.Debug("BLINK", $"Setting Saved: AcceleroThreshold - {settings.AcceleroThreshold}");
                fs.Close();
            }
        }
    }
}