
using Android.Util;
using Java.IO;
using Java.Lang;
using System;
using System.Diagnostics.Tracing;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CycleAlarmApp.Droid.Controller
{
    public class Sensors : IDisposable
    {
        SensorSpeed speed = SensorSpeed.UI;
        Bluetooth Bluetooth;

        public float Center = 0f;
        public float AccelThreshold = 0.1f;
        public float BreakThreshold = 1f;
        public LightState State = LightState.None;
        public bool IsBreak = false;

        public enum LightState { Left = -1, None = 0, Right = 1 };

        // GyroscopeData GyroscopeData;
        AccelerometerData AccelerometerData;

        Thread thread;

        public Sensors(Bluetooth bluetooth)
        {
            thread = new Thread(TickChecker);
            Accelerometer.ReadingChanged += AcceleroChanged;
            // Gyroscope.ReadingChanged += GyroChanged;
            Bluetooth = bluetooth;
        }

        public void Start()
        {
            if (!Accelerometer.IsMonitoring)
            {
                Accelerometer.Start(speed);
            }
            //if (!Gyroscope.IsMonitoring)
            //{
            //    Gyroscope.Start(speed);
            //}

            if (!thread.IsAlive)
            {
                thread.Start();
            }
        }
        public void Stop()
        {
            if (Accelerometer.IsMonitoring)
            {
                Accelerometer.Stop();
            }
            //if (Gyroscope.IsMonitoring)
            //{
            //    Gyroscope.Stop();
            //}
        }

        private void TickChecker()
        {
            while (true)
            {
                Thread.Sleep(100);
                //if (AccelerometerData != null && GyroscopeData != null)
                if (AccelerometerData != null)
                {
                    Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        Log.Debug("BLINK", $"Accel = {AccelerometerData.Acceleration.X}, {AccelerometerData.Acceleration.Y}, {AccelerometerData.Acceleration.Z}");
                        //Log.Debug("BLINK", $"Gyros = {GyroscopeData.AngularVelocity.X}, {GyroscopeData.AngularVelocity.Y}, {GyroscopeData.AngularVelocity.Z}");
                    });
                }
                if(!IsBreak && AccelerometerData.Acceleration.Z > BreakThreshold)
                {
                    IsBreak = true;
                    Bluetooth.Write("BREAK ON");
                }
                else if (IsBreak && AccelerometerData.Acceleration.Z < BreakThreshold)
                {
                    IsBreak = false;
                    Bluetooth.Write("BREAK OFF");
                }
                else if (State != LightState.Left && AccelerometerData.Acceleration.X > AccelThreshold + Center)
                {
                    State = LightState.Left;
                    Bluetooth.Write("LEFT");
                }
                else if (State == LightState.Left && AccelerometerData.Acceleration.X < AccelThreshold + Center)
                {
                    State = LightState.None;
                    Bluetooth.Write("NONE");
                }
                else if (State != LightState.Right && AccelerometerData.Acceleration.X < -AccelThreshold + Center)
                {
                    State = LightState.Right;
                    Bluetooth.Write("RIGHT");
                }
                else if (State == LightState.Right && AccelerometerData.Acceleration.X > -AccelThreshold + Center)
                {
                    State = LightState.None;
                    Bluetooth.Write("NONE");
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

        //private void GyroChanged(object sender, GyroscopeChangedEventArgs e)
        //{
        //    GyroscopeData = e.Reading;
        //}

    }
}