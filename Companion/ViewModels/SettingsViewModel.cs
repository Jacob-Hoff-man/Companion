using System;
namespace Companion.ViewModels
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
        }

        public async void DeleteGoalProfiles()
        {
            await App.GoalProfileRepo.DeleteRepoItems();
        }

        public async void DeleteTrips()
        {
            await App.GoalProfileRepo.DeleteRepoItems();
        }
    }
}
