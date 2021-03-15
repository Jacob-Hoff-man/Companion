using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Companion.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ButtonClickedDeleteAllGoalProfiles(System.Object sender, System.EventArgs e)
        {
            App.GoalProfileDatabase.DeleteAllGoalProfiles();


        }

        private void ButtonClickedDeleteAllTrips(System.Object sender, System.EventArgs e)
        {
            App.TripsDatabase.DeleteAllTrips();

        }
        //private async void ButtonClickedDeleteGoalProfilesAsync(object sender, EventArgs e)
        //{
        //    //App.GoalProfileDatabase.;
        //}

        //private async void ButtonClickedDeleteTripsAsync(object sender, EventArgs e)
        //{
        //    //await App.TripsDatabase.DeleteAllTrips();
        //}
    }
}
