using System;
using System.Collections.Generic;
using Companion.ViewModels;
using Companion.Models;
using Xamarin.Forms;
using System.Collections.ObjectModel;


namespace Companion.Views
{
    public partial class GoalsPage : ContentPage
    {

        GoalsViewModel GoalsViewModel;
        bool IsEditing;
        GoalProfile OldGoalProfile; 

        public GoalsPage()
        {
            //this construct is used when creating a new goal profile
            InitializeComponent();
            GoalsViewModel = new GoalsViewModel();
            BindingContext = GoalsViewModel;
            label.Text = "Add Goal Profile";
            IsEditing = false;

        }

        public GoalsPage(GoalProfile goalProfile)
        {
            //this constructor is used when editing an existing goal profile
            InitializeComponent();
            GoalsViewModel = new GoalsViewModel(goalProfile);
            BindingContext = GoalsViewModel;
            label.Text = "Edit Goal Profile";
            IsEditing = true;
            OldGoalProfile = goalProfile;

        }

        async void Save_Clicked(object sender, System.EventArgs e)
        {
            GoalsViewModel.Save();
            //this is a cheap way of doing this last minute :D
            if (IsEditing)
            {
                await App.GoalProfileRepo.DeleteRepoItem(OldGoalProfile);
            }
            await Navigation.PopAsync();
        }

    }
}
