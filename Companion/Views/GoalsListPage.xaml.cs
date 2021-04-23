using System;
using System.Collections.Generic;
using Companion.Data;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Companion.ViewModels;
using Companion.Models;
using System.Collections.ObjectModel;

namespace Companion.Views
{
    public partial class GoalsListPage : ContentPage
    {
        GoalsViewModel GoalsViewModel;

        public GoalsListPage()
        {
            InitializeComponent();
            GoalsViewModel = new GoalsViewModel();
            BindingContext = GoalsViewModel;

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            GoalsViewModel.GoalProfiles = new ObservableCollection<GoalProfile>(await App.GoalProfileRepo.GetRepoItems());
        }

        async void GoalProfile_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new GoalsPage(e.Item as GoalProfile));
        }

        async void NewGoalPofile_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new GoalsPage());
        }

    }
}
