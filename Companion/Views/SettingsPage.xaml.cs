using System;
using System.Collections.Generic;
using Companion.ViewModels;

using Xamarin.Forms;

namespace Companion.Views
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel SettingsViewModel;

        public SettingsPage()
        {
            InitializeComponent();
            SettingsViewModel = new SettingsViewModel();
            //BindingContext = SettingsViewModel;
        }

        private void DeleteGoalProfiles_Clicked(System.Object sender, System.EventArgs e)
        {
            SettingsViewModel.DeleteGoalProfiles();
        }

        private void DeleteTrips_Clicked(System.Object sender, System.EventArgs e)
        {
            SettingsViewModel.DeleteTrips();
        }

    }
}
