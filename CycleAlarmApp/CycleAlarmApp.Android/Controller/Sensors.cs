
using Android.Util;
using Java.IO;
using Java.Lang;
using System;
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
        bool left = false;
        bool right = false;

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
                if (!left && AccelerometerData.Acceleration.X > AccelThreshold + Center)
                {
                    left = true;
                    Bluetooth.Write("led0 on");
                }
                else if (left && AccelerometerData.Acceleration.X < AccelThreshold + Center)
                {
                    left = false;
                    Bluetooth.Write("led0 off");
                }
                else if (!right && AccelerometerData.Acceleration.X < -AccelThreshold + Center)
                {
                    right = true;
                    Bluetooth.Write("led1 on");
                }
                else if (right && AccelerometerData.Acceleration.X > -AccelThreshold + Center)
                {
                    right = false;
                    Bluetooth.Write("led1 off");
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