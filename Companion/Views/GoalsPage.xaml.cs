using System;
using System.Collections.Generic;
using Companion.Models;
using Xamarin.Forms;
using System.Collections.ObjectModel;


namespace Companion.Views
{
    public partial class GoalsPage : ContentPage
    {

        
        private ObservableCollection<GoalProfile> GoalProfiles = new ObservableCollection<GoalProfile>();

        public GoalsPage()
        {
            InitializeComponent();
            Init();

        }

        public void Init()
        {
            var enumerator = App.GoalProfileDatabase.GetGoalProfileEnumerated();
            if (enumerator == null)
            {
                App.GoalProfileDatabase.SaveGoalProfile(new GoalProfile { Name = "Cricket", Weight = 69 });
            }

            while (enumerator.MoveNext())
            {
                this.GoalProfiles.Add(enumerator.Current);
            }

            listViewGoalProfiles.ItemsSource = GoalProfiles;
        }

        private void ButtonClickedSaveGoalProfile(System.Object sender, System.EventArgs e)
        {
            var newGoalProfile = new GoalProfile()
            {
                Name = "Jacob",
                BirthDate = datePicker.Date,
                Gender = genderPicker.ToString() == "male" ? Gender.male : genderPicker.ToString() == "female" ? Gender.female : Gender.unspecified,
                Weight = 60, 

            };

            GoalProfiles.Add(newGoalProfile);
            App.GoalProfileDatabase.SaveGoalProfile(newGoalProfile);

        }

        private void ButtonClickedOnDelete(System.Object sender, System.EventArgs e)
        {
            var item = (MenuItem)sender;
            var model = (GoalProfile)item.CommandParameter;
            GoalProfiles.Remove(model);
            App.GoalProfileDatabase.DeleteGoalProfile(model.ID);
        }

        ////create a GoalProfile based on the inputs, use view model to save it to database
        //private async void ButtonClickedSaveGoalProfileAsync(object sender, EventArgs e)
        //{
        //    GoalProfile temp = new GoalProfile
        //    {
        //        Name = nameEntry.ToString()


        //    };
        //}

        //private async void ButtonClickedAddGoalProfileAsync(object sender, EventArgs e)
        //{
        //    //statusMessage.Text = "";
        //    //await App.GoalProfileDatabase.AddNewGoalProfile(newGoalProfile.Text);
        //    //statusMessage.Text = App.GoalProfileDatabase.StatusMessage;
        //}

        //private async void ButtonClickedGetAllGoalProfilesAsync(object sender, EventArgs e)
        //{
        //    //statusMessage.Text = "";
        //    //List<GoalProfile> goalProfiles = await App.GoalProfileDatabase.GetAllGoalProfiles();
        //    //goalProfileList.ItemsSource = goalProfiles;
        //}
    }
}
