
using Android.Gms.Tasks;
using Android.Util;
using BLINK.Interface;
using Java.IO;
using Java.Lang;
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(BLINK.Droid.Controller.Sensors))]
namespace BLINK.Droid.Controller
{
    public class Sensors : IDisposable, IMapPage
    {
        SensorSpeed speed = SensorSpeed.UI;
        Bluetooth Bluetooth;

        public static Sensors Singleton = null;

        public LightState State = LightState.None;
        public bool IsBreak = false;

        public enum LightState { Left = -1, None = 0, Right = 1 };

        AccelerometerData AccelerometerData;

        private static Thread thread;

        public Sensors()
        {

        }
        public Sensors(Bluetooth bluetooth)
        {
            if (Singleton != null)
            {
                thread = new Thread(TickChecker);
                Accelerometer.ReadingChanged += AcceleroChanged;
                Compass.ReadingChanged += CompassChanged;
                Bluetooth = bluetooth;
            }
        }

        private void CompassChanged(object sender, CompassChangedEventArgs e)
        {
            MenuPage.Degree = e.Reading.HeadingMagneticNorth;
        }

        public void Start()
        {
            if (!Accelerometer.IsMonitoring)
            {
                Accelerometer.Start(speed);
            }
            if (!thread.IsAlive)
            {
                thread.Start();
            }

            if(!Compass.IsMonitoring)
            {
                Compass.Start(speed, true);
            }
        }
        public void Stop()
        {
            if (Accelerometer.IsMonitoring)
            {
                Accelerometer.Stop();
            }

            if (Compass.IsMonitoring)
            {
                Compass.Stop();
            }
            if (thread != null && thread.IsAlive)
            {
                try
                {
                    thread.Dispose();
                } catch { }
            }
        }


        private void TickChecker()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (AccelerometerData == null) return;
                if(!IsBreak && AccelerometerData.Acceleration.Z > BLINK.Settings.Singleton.AcceleroThreshold)
                {
                    IsBreak = true;
                    Bluetooth.Write("BREAK ON");
                    Log.Debug("BLINK", $"BREAK ON SENT!, AccelZ: {AccelerometerData.Acceleration.Z}, Threshold: {BLINK.Settings.Singleton.AcceleroThreshold}");
                }
                else if (IsBreak && AccelerometerData.Acceleration.Z < BLINK.Settings.Singleton.AcceleroThreshold)
                {
                    IsBreak = false;
                    Bluetooth.Write("BREAK OFF");
                    Log.Debug("BLINK", $"BREAK OFF SENT!, AccelZ: {AccelerometerData.Acceleration.Z}, Threshold: {BLINK.Settings.Singleton.AcceleroThreshold}");
                }
                else if (State != LightState.Left && AccelerometerData.Acceleration.X > BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint)
                {
                    State = LightState.Left;
                    Bluetooth.Write("LEFT");
                    Log.Debug("BLINK", $"LEFT SENT!, AccelX: {AccelerometerData.Acceleration.X}, Threshold: {BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint}");
                }
                else if (State == LightState.Left && AccelerometerData.Acceleration.X < BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint)
                {
                    State = LightState.None;
                    Bluetooth.Write("NONE");
                    Log.Debug("BLINK", $"NONE SENT!, AccelX: {AccelerometerData.Acceleration.X}, Threshold: {BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint}");
                }
                else if (State != LightState.Right && AccelerometerData.Acceleration.X < -BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint) 
                {
                    State = LightState.Right;
                    Bluetooth.Write("RIGHT");
                    Log.Debug("BLINK", $"RIGHT SENT!, AccelX: {AccelerometerData.Acceleration.X}, Threshold: {-BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint}");
                }
                else if (State == LightState.Right && AccelerometerData.Acceleration.X > -BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint)
                {
                    State = LightState.None;
                    Bluetooth.Write("NONE");
                    Log.Debug("BLINK", $"NONE SENT!, AccelX: {AccelerometerData.Acceleration.X}, Threshold: {-BLINK.Settings.Singleton.TurnThreshold - BLINK.Settings.Singleton.CenterPoint}");
                }
            }
        }

        private void AcceleroChanged(object sender, AccelerometerChangedEventArgs e)
        {
            AccelerometerData = e.Reading;
        }

        public void Dispose()
        {
            try
            {
                thread.Dispose();
                Stop();
            } catch { }
        }

        public Task<Location> GetLocation()
        {
            return Geolocation.GetLocationAsync();
        }
    }
}