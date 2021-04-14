using System;
using System.Collections.Generic;
using Companion.ViewModels;
using Xamarin.Forms;
using System.Collections.ObjectModel;


namespace Companion.Views
{
    public partial class GoalsPage : ContentPage
    {


        public GoalsPage()
        {
            InitializeComponent();
            //BindingContext = new GoalsViewModel();

        }

        //private void ButtonClickedOnDelete(System.Object sender, System.EventArgs e)
        //{
        //    var item = (MenuItem)sender;
        //    var model = (GoalProfile)item.CommandParameter;
        //    GoalProfiles.Remove(model);
        //    App.GoalProfileDatabase.DeleteGoalProfile(model.ID);
        //}

    }
}
