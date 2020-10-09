using CycleAlarmApp.Interface;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CycleAlarmApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        ISettingMenu menu;
        public SettingPage()
        {
            InitializeComponent();
            DependencyService.Register<ISettingMenu>();
            menu = DependencyService.Get<ISettingMenu>();

            ThresholdVal.Value = menu.GetThreshold();
            CenterVal.Value = menu.GetCenter();
            BreakVal.Value = menu.GetBreak();
        }

        private void CenterChanged(object sender, ValueChangedEventArgs e)
        {
            menu.SetCenter((float)CenterVal.Value);
            CenterLabel.Text = ((float)CenterVal.Value).ToString();
        }

        private void ThresholdChanged(object sender, ValueChangedEventArgs e)
        {
            menu.SetThreshold((float)ThresholdVal.Value);
            ThresholdLabel.Text = ((float)ThresholdVal.Value).ToString();
        }
        private void BreakChanged(object sender, ValueChangedEventArgs e)
        {
            menu.SetBreak((float)BreakVal.Value);
            BreakLabel.Text = ((float)BreakVal.Value).ToString();
        }

        private void ResetValues(object sender, EventArgs e)
        {
            CenterVal.Value = 0f;
            ThresholdVal.Value = 0.1f;
            BreakVal.Value = 2.5f;
        }
    }
}