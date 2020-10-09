using Android.Content;
using Android.Views;
using CycleAlarmApp.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CycleAlarmApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            DependencyService.Register<IMenuPage>();
        }

        private async void Start(object sender, EventArgs e)
        {
            if(DependencyService.Get<IMenuPage>().Start())
            {
                MenuPage page = new MenuPage();
                await Navigation.PushModalAsync(page);
            }
        }

        private async void Setting(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SettingPage());
        }
    }
}
