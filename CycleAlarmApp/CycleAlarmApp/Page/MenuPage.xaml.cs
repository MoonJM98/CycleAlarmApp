using CycleAlarmApp.Interface;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CycleAlarmApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        Thread timeRefreshThread;
        Thread screenRefreshThread;

        public MenuPage()
        {
            InitializeComponent();

            Disappearing += Closing;

            timeRefreshThread = new Thread(TimeRefresh);
            screenRefreshThread = new Thread(screenRefresh);

            timeRefreshThread.Start();
            screenRefreshThread.Start();
        }

        private void TimeRefresh()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while(true)
            {
                App.Current.Dispatcher.BeginInvokeOnMainThread(() => {
                    TimeLabel.Text = $"{DateTime.Now:tt h:mm}";
                    ElapsedTimeLabel.Text = sw.Elapsed.Hours > 0 ? $"{sw.Elapsed.Hours}시간 {sw.Elapsed.Minutes}분" : $"{sw.Elapsed.Minutes}분";
                });
                Thread.Sleep(1000);
            }
        }
        private void screenRefresh()
        {
            while (true)
            {
                int x = 20;
                int y = 20;
                for(; x <= 200; x++)
                {
                    App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                        BackgroundTheme.Padding = new Thickness(x, y, 0, 0)
                    );
                    Thread.Sleep(60000);
                }

                for (; y <= 200; y++)
                {
                    App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                        BackgroundTheme.Padding = new Thickness(x, y, 0, 0)
                    );
                    Thread.Sleep(60000);
                }

                for (; x > 0; x--)
                {
                    App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                        BackgroundTheme.Padding = new Thickness(x, y, 0, 0)
                    );
                    Thread.Sleep(60000);
                }

                for (; y > 0; y--)
                {
                    App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                        BackgroundTheme.Padding = new Thickness(x, y, 0, 0)
                    );
                    Thread.Sleep(60000);
                }
            }
        }

        private void Closing(object sender, EventArgs e)
        {
            try
            {
                timeRefreshThread.Dispose();
            } catch { }
            try
            {
                screenRefreshThread.Dispose();
            } catch { }
        }
    }
}