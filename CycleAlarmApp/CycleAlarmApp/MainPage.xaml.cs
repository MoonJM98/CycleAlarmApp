using Android.Content;
using Android.Views;
using BLINK.Interface;
using BLINK.Page;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BLINK
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            DependencyService.Register<IFileMenu>();
            DependencyService.Get<IFileMenu>().SaveSettings(ref Settings.Singleton);
        }

        private async void Start(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoadingPage());
        }

        private async void Setting(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SettingPage());
        }
    }
}
