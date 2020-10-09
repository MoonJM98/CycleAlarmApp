using BLINK.Interface;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BLINK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        IFileMenu file;
        public SettingPage()
        {
            InitializeComponent();
            DependencyService.Register<IFileMenu>();

            file = DependencyService.Get<IFileMenu>();

            TurnThresholdValue.Value = Settings.Singleton.TurnThreshold;
            CenterPointValue.Value = Settings.Singleton.CenterPoint;
            AccelThresholdValue.Value = Settings.Singleton.AcceleroThreshold;

            Disappearing += Closing;
        }

        private void Closing(object sender, EventArgs e)
        {
            file.SaveSettings(ref Settings.Singleton);
        }

        private void CenterPointChanged(object sender, ValueChangedEventArgs e)
        {
            Settings.Singleton.CenterPoint = (float)CenterPointValue.Value;
            CenterPointLabel.Text = Settings.Singleton.CenterPoint.ToString();
        }

        private void TurnThresholdChanged(object sender, ValueChangedEventArgs e)
        {
            Settings.Singleton.TurnThreshold = (float)TurnThresholdValue.Value;
            TurnThresholdLabel.Text = Settings.Singleton.TurnThreshold.ToString();
        }
        private void AccelThresholdChanged(object sender, ValueChangedEventArgs e)
        {
            Settings.Singleton.AcceleroThreshold = (float)AccelThresholdValue.Value;
            AccelThresholdLabel.Text = Settings.Singleton.AcceleroThreshold.ToString();
        }

        private void ResetValues(object sender, EventArgs e)
        {
            CenterPointValue.Value = 0f;
            TurnThresholdValue.Value = 0.1f;
            AccelThresholdValue.Value = 2.5f;

            Settings.Singleton.AcceleroThreshold = 2.5f;
            Settings.Singleton.TurnThreshold = 0.1f;
            Settings.Singleton.CenterPoint = 0f;
        }

    }
}