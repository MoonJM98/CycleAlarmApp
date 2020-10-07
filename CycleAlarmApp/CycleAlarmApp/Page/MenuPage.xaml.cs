using CycleAlarmApp.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public MenuPage()
        {
            InitializeComponent();
            DependencyService.Register<IMenuPage>();
            DependencyService.Get<IMenuPage>().StartActivityInAndroid();
        }

        private async void ShowFrontLightMenu(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SettingPage());
        }

        private async void ShowSettingMenu(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SettingPage());
        }

        private async void ShowBatteryMenu(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new BatteryMenuPage());
        }

        private async void ShowBackLightMenu(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SettingPage());
        }
    }
}