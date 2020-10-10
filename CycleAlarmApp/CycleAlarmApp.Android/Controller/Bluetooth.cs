using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using BLINK.Interface;
using Java.Lang;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(BLINK.Droid.Controller.Bluetooth))]
namespace BLINK.Droid.Controller
{
    [Activity(Label = "Bluetooth")]
    public class Bluetooth : Activity, IMenuPage
    {
        public static Bluetooth Singleton = null;
        private readonly BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;

        private static readonly UUID UUID = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
        private static readonly string Name = "BLINK";

        public Stream InStream;
        public Stream OutStream;

        private Sensors Sensors;
        public BluetoothSocket Socket = null;

        private IEnumerable<BluetoothDevice> BlinkDevices => from d in adapter.BondedDevices where d.Name == "BLINK" select d;

        public BluetoothState State
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;

            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

            Window.SetFlags(Android.Views.WindowManagerFlags.KeepScreenOn, Android.Views.WindowManagerFlags.KeepScreenOn);
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

        public bool Start()
        {
            try
            {
                if (BlinkDevices.Any())
                {
                    BluetoothDevice device = BlinkDevices.First();
                    if(!Connect(adapter.GetRemoteDevice(device.Address)))
                    {
                        return false;
                    }

                    try
                    {
                        Sensors.Start();
                    } catch (System.Exception e)
                    {
                        ShowToast($"센서를 불러오는 도중 오류가 발생했습니다.");
                        Log.Debug(Name, "Sensor Failed {0}", e);
                        return false;
                    }
                    
                    return true;
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
            return false;
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
            catch (NullReferenceException e)
            {
                Log.Error(Name, "NullReferenceException", e);
            }
            catch (System.Exception e)
            {
                Log.Error(Name, "Exception", e);
            }
        }

        private bool Connect(BluetoothDevice device)
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
                return false;
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
                    return false;
                }
                ConnectionFailed();
                ShowToast($"BLINK와의 연결에 실패했습니다.");
                return false;
            }

            Connected(bluetoothSocket);
            return true;
        }

        private void ShowToast(string str, ToastLength length = ToastLength.Short)
        {
            App.Current.Dispatcher.BeginInvokeOnMainThread(() => Toast.MakeText(Application.Context, str, length).Show());
        }

        private void Connected(BluetoothSocket bluetoothSocket)
        {
            Log.Debug(Name, $"create ConnectedThread");

            Socket = bluetoothSocket;

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

        class CallbackObject
        {
            public byte[] Buffer { get; set; }
            public string Text { get => Encoding.UTF8.GetString(Buffer); }
            public int Length { get => Buffer.Length; }
        }
    }
}