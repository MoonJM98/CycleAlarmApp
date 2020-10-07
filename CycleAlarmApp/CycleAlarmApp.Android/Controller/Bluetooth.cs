using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Util;
using Android.Widget;
using CycleAlarmApp.Interface;
using Java.Lang;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(CycleAlarmApp.Droid.Controller.Bluetooth))]
namespace CycleAlarmApp.Droid.Controller
{
    [Activity(Label = "Bluetooth")]
    public class Bluetooth : IMenuPage, ISettingMenu
    {
        private static Bluetooth Singleton = null;

        private readonly BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;

        private static readonly UUID UUID = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
        private static readonly string Name = "BLINK";

        public Stream InStream;
        public Stream OutStream;

        private Sensors Sensors;

        private IEnumerable<BluetoothDevice> BlinkDevices => from d in adapter.BondedDevices where d.Name == "BLINK" select d;

        public BluetoothState State
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;

            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public Bluetooth()
        {
            Singleton = this;

            try
            {
                if (Sensors == null)
                {
                    Sensors = new Sensors(this);
                }

                this.State = BluetoothState.None;

                if (!adapter.IsEnabled) adapter.Enable();

                adapter.StartDiscovery();
            }
            catch (System.Exception e)
            {
                Log.Error("BLINK", "Bluetooth Error!", e);
            }
        }
        public void Start()
        {
            try
            {
                if (BlinkDevices.Any())
                {
                    ShowToast($"BLINK 기기를 찾았습니다.");
                    BluetoothDevice device = BlinkDevices.First();

                    Connect(adapter.GetRemoteDevice(device.Address));

                    Sensors.Start();
                }
                else
                {
                    ShowToast($"BLINK 기기를 찾을 수 없습니다.\n기기를 켜고 페어링 한 후 앱을 다시 켜주세요.");
                    Sensors.Dispose();
                    Sensors = null;
                }
            }
            catch (System.Exception e)
            {
                Log.Error("BLINK", "Bluetooth Error!", e);
            }
        }

        public void Write(string str)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                OutStream.Write(buffer, 0, buffer.Length);
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(Name, "Exception during write", e);
            }
        }

        private void Connect(BluetoothDevice device)
        {
            BluetoothSocket bluetoothSocket = null;
            try
            {
                bluetoothSocket = device.CreateRfcommSocketToServiceRecord(UUID);
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(Name, "create() failed", e);
                ShowToast($"BLINK와의 연결에 실패했습니다.");
            }
            State = BluetoothState.Connecting;

            adapter.CancelDiscovery();

            try
            {
                bluetoothSocket.Connect();
            }
            catch (Java.IO.IOException)
            {
                try
                {
                    bluetoothSocket.Close();
                }
                catch (Java.IO.IOException e)
                {
                    Log.Error(Name, $"unable to close() socket during connection failure.", e);
                    ShowToast($"BLINK와의 연결에 실패했습니다.");
                }
                ConnectionFailed();
                ShowToast($"BLINK와의 연결에 실패했습니다.");
            }

            Connected(bluetoothSocket);
        }

        private void ShowToast(string str, ToastLength length = ToastLength.Short)
        {
            App.Current.Dispatcher.BeginInvokeOnMainThread(() => Toast.MakeText(Application.Context, str, length).Show());
        }

        private void Connected(BluetoothSocket bluetoothSocket)
        {
            Log.Debug(Name, $"create ConnectedThread");

            try
            {
                InStream = bluetoothSocket.InputStream;
                OutStream = bluetoothSocket.OutputStream;
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(Name, "temp sockets not created", e);
                ConnectionLost();
            }

            State = BluetoothState.Connected;

            Log.Info(Name, "BEGIN mConnectedThread");

            CallbackObject callback = new CallbackObject
            {
                Buffer = new byte[1024]
            };

            InStream.BeginRead(callback.Buffer, 0, callback.Length, ReadCallback, callback);
        }

        private void ReadCallback(IAsyncResult result)
        {
            try
            {
                CallbackObject obj = result.AsyncState as CallbackObject;

                ShowToast(obj.Text);

                CallbackObject callback = new CallbackObject
                {
                    Buffer = new byte[1024]
                };
                InStream.BeginRead(callback.Buffer, 0, callback.Length, ReadCallback, callback);
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(Name, "disconnected", e);
                ConnectionLost();
            }
            catch (Java.Lang.RuntimeException e)
            {
                Log.Error(Name, "disconnected", e);
                ConnectionLost();
            }
        }

        private void ConnectionLost()
        {
            State = BluetoothState.None;
            Start();
        }

        private void ConnectionFailed()
        {
            State = BluetoothState.Listen;
        }

        public void StartActivityInAndroid()
        {
            Start();
        }

        public void SetThreshold(float threshold)
        {
            Sensors.AccelThreshold = threshold;
        }

        public void SetCenter(float center)
        {
            Sensors.Center = center;
        }

        public float GetThreshold()
        {
            return Sensors.AccelThreshold;
        }

        public float GetCenter()
        {
            return Sensors.Center;
        }

        class CallbackObject
        {
            public byte[] Buffer { get; set; }
            public string Text { get => Encoding.UTF8.GetString(Buffer); }
            public int Length { get => Buffer.Length; }
        }
    }
}