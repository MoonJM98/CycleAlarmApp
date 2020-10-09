using BLINK.Interface;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BLINK.Page
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        IMenuPage menuPage;
        public LoadingPage()
        {
            InitializeComponent();

            DependencyService.Register<IMenuPage>();
            menuPage = DependencyService.Get<IMenuPage>();

            Thread thread = new Thread(Start);

            thread.Start();
        }

        private async void Start()
        {
            if (menuPage.Start())
            {
                App.Current.Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushModalAsync(new MenuPage());
                });
            } else
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}