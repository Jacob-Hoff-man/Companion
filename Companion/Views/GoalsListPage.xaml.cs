using System;
using System.Collections.Generic;
using Companion.Data;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Companion.Models;
using System.Collections.ObjectModel;

namespace Companion.Views
{
    public partial class GoalsListPage : ContentPage
    {

        private ObservableCollection<GoalProfile> GoalProfiles = new ObservableCollection<GoalProfile>();

        public GoalsListPage()
        {
            InitializeComponent();
            Init();

        }

        public void Init()
        {
            var enumerator = App.GoalProfileDatabase.GetGoalProfileEnumerated();
            if(enumerator == null)
            {
                App.GoalProfileDatabase.SaveGoalProfile(new GoalProfile { Name = "Cricket", Weight=69 });
            }

            while(enumerator.MoveNext())
            {
                this.GoalProfiles.Add(enumerator.Current);
            }

            listViewGoalProfiles.ItemsSource = GoalProfiles;
        }

        private void ButtonClickedOnDelete(System.Object sender, System.EventArgs e)
        {
            var item = (MenuItem)sender;
            var model = (GoalProfile)item.CommandParameter;
            GoalProfiles.Remove(model);
            App.GoalProfileDatabase.DeleteGoalProfile(model.ID);
        }
    }
}
