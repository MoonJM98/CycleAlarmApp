using Android.Util;
using BLINK.Interface;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace BLINK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public static Xamarin.Forms.Maps.Map Map;

        public static double Degree = 0d;

        Thread timeRefreshThread;
        IMapPage mapPage;

        public MenuPage()
        {
            InitializeComponent();

            DependencyService.Register<IMapPage>();

            mapPage = DependencyService.Get<IMapPage>();

            Disappearing += Closing;

            timeRefreshThread = new Thread(TimeRefresh);

            timeRefreshThread.Start();

            Map = map;
        }

        private void TimeRefresh()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            map.HasZoomEnabled = false;
            map.HasScrollEnabled = false;
            map.IsShowingUser = true;

            DateTime StartTime = DateTime.Now;
            DateTime LastTime = DateTime.Now;
            double totalDistance = 0;
            double totalSpeed = 0;
            Location prevLocation = null;

            while (true)
            {
                Location loc = mapPage.GetLocation().GetAwaiter().GetResult();
                if(prevLocation != null) totalDistance += loc.CalculateDistance(prevLocation, DistanceUnits.Kilometers);
                DateTime now = DateTime.Now;
                totalSpeed += (now - LastTime).TotalHours * (loc.Speed ?? 0);
                double avgSpeed = totalSpeed / (now - StartTime).TotalHours;
                App.Current.Dispatcher.BeginInvokeOnMainThread(() => {
                    SpeedLabel.Text = $"속도: {loc.Speed ?? 0:0.##}km/h, 평균: {avgSpeed:0.##}km/h, 거리: {totalDistance:0.##}km";
                    TimeLabel.Text = $"현재: {DateTime.Now:tt h:mm}, 경과: " + (sw.Elapsed.Hours > 0 ? $"{sw.Elapsed.Hours}시간 {sw.Elapsed.Minutes}분" : $"{sw.Elapsed.Minutes}분");
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(loc.Latitude, loc.Longitude), Distance.FromMeters(100)));
                });
                prevLocation = loc;
                Thread.Sleep(1000);
            }
        }

        private void Closing(object sender, EventArgs e)
        {
            try
            {
                timeRefreshThread.Dispose();
            }
            catch { }
            App.Current.Quit();
        }
    }
}